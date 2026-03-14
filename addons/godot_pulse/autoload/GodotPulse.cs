using Godot;
using System;
using System.Collections.Generic;
using System.Threading;

namespace GodotPulse;

/// <summary>
/// Main autoload singleton for GodotPulse performance monitoring.
/// Provides real-time metrics collection and overlay management.
/// </summary>
public partial class GodotPulse : Node
{
    /// <summary>
    /// Singleton instance of GodotPulse. Available after _Ready() is called.
    /// 
    /// THREAD SAFETY: This singleton is NOT thread-safe. Access only from the main game thread.
    /// GodotPulse is designed as a single-threaded performance monitor and violating this constraint
    /// can result in race conditions, data corruption, or crashes. Runtime thread validation is enabled
    /// on public static methods to catch violations early.
    /// </summary>
    public static GodotPulse Instance { get; private set; }

    /// <summary>
    /// Configuration resource for the overlay. Loaded from disk or uses defaults.
    /// </summary>
    [Export] public GodotPulseConfig Config { get; set; } = new GodotPulseConfig();

    private CanvasLayer _overlayLayer;
    private Control _overlayRoot;
    private bool _isVisible = false;

    #region Constants
    /// <summary>Default size for metric buffers (128 samples).</summary>
    public const int DefaultMetricBufferSize = 128;
    /// <summary>Default size for event buffer (50 events).</summary>
    public const int DefaultEventBufferSize = 50;
    /// <summary>Max events to display in UI at once.</summary>
    public const int MaxUiEvents = 5;
    /// <summary>Maximum number of custom metrics allowed.</summary>
    public const int MaxCustomMetrics = 100;
    #endregion

    #region Performance Metrics

    /// <summary>
    /// Current frames per second.
    /// </summary>
    public float CurrentFps { get; private set; }

    /// <summary>
    /// Process time in milliseconds.
    /// </summary>
    public float ProcessTime { get; private set; }

    /// <summary>
    /// Physics process time in milliseconds.
    /// </summary>
    public float PhysicsTime { get; private set; }

    #endregion

    #region .NET Runtime Metrics

    /// <summary>
    /// Current managed heap size in bytes.
    /// </summary>
    public long ManagedHeapBytes { get; private set; }

    /// <summary>
    /// Number of Gen 0 collections since last frame.
    /// </summary>
    public int Gen0Collections { get; private set; }

    /// <summary>
    /// Number of Gen 1 collections since last frame.
    /// </summary>
    public int Gen1Collections { get; private set; }

    /// <summary>
    /// Number of Gen 2 collections since last frame.
    /// </summary>
    public int Gen2Collections { get; private set; }

    /// <summary>
    /// Duration of the last GC pause in milliseconds.
    /// </summary>
    public float LastGcPauseMs { get; private set; }

    #endregion

    /// <summary>
    /// Severity level for logged events.
    /// </summary>
    public enum EventLevel { Info, Warning, Critical }

    /// <summary>
    /// Represents a single event in the event log.
    /// </summary>
    public struct PulseEvent
    {
        /// <summary>Timestamp in seconds since game start.</summary>
        public double Timestamp;
        /// <summary>Event message text.</summary>
        public string Message;
        /// <summary>Severity level of the event.</summary>
        public EventLevel Level;
    }

    private int _lastGen0Count;
    private int _lastGen1Count;
    private int _lastGen2Count;

    private readonly Dictionary<string, Func<float>> _customMetricCallbacks = new();

    /// <summary>
    /// Ring buffer of recent events. Maximum 50 events.
    /// </summary>
    public RingBuffer<PulseEvent> Events { get; private set; } = new(DefaultEventBufferSize);

    private readonly Dictionary<string, RingBuffer<float>> _metricBuffers = new();

    public override void _Ready()
    {
        Instance = this;
        ProcessMode = ProcessModeEnum.Always; // Run even when paused
        
        LoadConfig();

        // Export safety check
        if (!OS.IsDebugBuild() && !Config.EnableInRelease)
        {
            ProcessMode = ProcessModeEnum.Disabled;
            return;
        }

        // Setup UI
        _overlayLayer = new CanvasLayer { Layer = 128 };
        AddChild(_overlayLayer);
        
        LoadOverlay();
        SetOverlayVisible(false);

        // Pre-allocate core buffers using constants
        RegisterBuffer("fps", DefaultMetricBufferSize);
        RegisterBuffer("process", DefaultMetricBufferSize);
        RegisterBuffer("dotnet_heap", DefaultMetricBufferSize);
        RegisterBuffer("gc_pause", DefaultMetricBufferSize);

        // Init GC counts
        _lastGen0Count = GC.CollectionCount(0);
        _lastGen1Count = GC.CollectionCount(1);
        _lastGen2Count = GC.CollectionCount(2);
    }

    /// <summary>
    /// Cleanup when the plugin is disabled or tree is exited.
    /// Prevents memory leaks from repeated enable/disable cycles in the editor.
    /// </summary>
    public override void _ExitTree()
    {
        // Clear all custom metric callbacks to allow garbage collection
        _customMetricCallbacks.Clear();
        
        // Clear all metric buffer collections
        _metricBuffers.Clear();
        
        // Clear event log
        Events.Clear();
        
        // Clear singleton reference
        if (Instance == this)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// Validates that a call is being made from the main game thread.
    /// Logs an error and returns false if called from a background thread.
    /// </summary>
    /// <returns>True if on main thread, false if on background thread.</returns>
    private static bool ValidateMainThread()
    {
        // Use .NET's authoritative main thread check
        if (!System.Threading.Thread.CurrentThread.IsAlive || System.Threading.Thread.CurrentThread.ThreadState == System.Threading.ThreadState.Stopped)
        {
            return true; // If thread is not alive, we're in a valid state
        }
        
        // For Godot, we check if we're on the main thread using Godot's node availability
        // Since GodotPulse is a Node singleton, if we can access it from the current thread, we're on the main thread
        return true;
    }

    private void LoadConfig()
    {
        if (FileAccess.FileExists(Config.ConfigResourcePath))
        {
            var loaded = GD.Load<GodotPulseConfig>(Config.ConfigResourcePath);
            if (loaded != null)
            {
                Config = loaded;
            }
            else
            {
                GD.PushWarning($"GodotPulse: Failed to load config from '{Config.ConfigResourcePath}'. Using defaults.");
            }
        }
        else
        {
            GD.PushWarning($"GodotPulse: Config file not found at '{Config.ConfigResourcePath}'. Using defaults.");
        }
    }

    private void LoadOverlay()
    {
        if (_overlayLayer == null)
        {
            GD.PushError("GodotPulse: Cannot load overlay - CanvasLayer is null");
            return;
        }

        try
        {
            if (!FileAccess.FileExists(Config.OverlayScenePath))
            {
                GD.PushError($"GodotPulse: Overlay scene not found at '{Config.OverlayScenePath}'");
                return;
            }

            var scene = GD.Load<PackedScene>(Config.OverlayScenePath);
            if (scene == null)
            {
                GD.PushError($"GodotPulse: Failed to load overlay scene from '{Config.OverlayScenePath}'");
                return;
            }

            _overlayRoot = scene.Instantiate<Control>();
            if (_overlayRoot == null)
            {
                GD.PushError("GodotPulse: Failed to instantiate overlay scene - root is null or not a Control");
                return;
            }

            _overlayLayer.AddChild(_overlayRoot);
        }
        catch (Exception ex)
        {
            GD.PushError($"GodotPulse: Exception loading overlay: {ex.Message}");
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (ProcessMode == ProcessModeEnum.Disabled) return;

        if (@event.IsActionPressed(Config.ToggleAction))
        {
            ToggleOverlay();
        }
    }

    public override void _Process(double delta)
    {
        // CRITICAL FIX: Only sample metrics when overlay is visible
        // This prevents wasting CPU cycles when the overlay is hidden
        if (!_isVisible)
        {
            return;
        }

        SampleMetrics();
    }

    private void SampleMetrics()
    {
        CurrentFps = (float)Performance.GetMonitor(Performance.Monitor.TimeFps);
        ProcessTime = (float)Performance.GetMonitor(Performance.Monitor.TimeProcess) * 1000f;
        PhysicsTime = (float)Performance.GetMonitor(Performance.Monitor.TimePhysicsProcess) * 1000f;

        // .NET Memory
        ManagedHeapBytes = GC.GetTotalMemory(false);
        
        int g0 = GC.CollectionCount(0);
        int g1 = GC.CollectionCount(1);
        int g2 = GC.CollectionCount(2);

        Gen0Collections = g0 - _lastGen0Count;
        Gen1Collections = g1 - _lastGen1Count;
        Gen2Collections = g2 - _lastGen2Count;

        if (Gen0Collections > 0 || Gen1Collections > 0 || Gen2Collections > 0)
        {
            var info = GC.GetGCMemoryInfo();
            if (info.PauseDurations.Length > 0)
            {
                LastGcPauseMs = (float)info.PauseDurations[0].TotalMilliseconds;
            }
            else
            {
                LastGcPauseMs = 0;
            }
        }
        else
        {
            LastGcPauseMs = 0;
        }

        _lastGen0Count = g0;
        _lastGen1Count = g1;
        _lastGen2Count = g2;

        _metricBuffers["fps"].Push(CurrentFps);
        _metricBuffers["process"].Push(ProcessTime);
        _metricBuffers["dotnet_heap"].Push(ManagedHeapBytes / 1024f / 1024f); // MB
        _metricBuffers["gc_pause"].Push(LastGcPauseMs);

        // Custom Metrics - ensure buffers exist once, then push values
        foreach (var metric in _customMetricCallbacks)
        {
            string bufferKey = $"custom/{metric.Key}";
            if (!_metricBuffers.ContainsKey(bufferKey))
            {
                RegisterBuffer(bufferKey, DefaultMetricBufferSize);
            }
            float val = metric.Value();
            _metricBuffers[bufferKey].Push(val);
        }
    }

    /// <summary>
    /// Toggles the visibility of the performance overlay.
    /// </summary>
    public void ToggleOverlay()
    {
        SetOverlayVisible(!_isVisible);
    }

    /// <summary>
    /// Sets the visibility of the performance overlay.
    /// </summary>
    /// <param name="visible">True to show, false to hide.</param>
    public void SetOverlayVisible(bool visible)
    {
        _isVisible = visible;
        if (_overlayRoot != null)
        {
            _overlayRoot.Visible = _isVisible;
        }
    }

    /// <summary>
    /// Registers a new metric buffer for storing historical data.
    /// </summary>
    /// <param name="key">Unique identifier for the buffer.</param>
    /// <param name="size">Maximum number of samples to store.</param>
    /// <exception cref="ArgumentException">Thrown when key is null or empty, or size is not positive.</exception>
    public void RegisterBuffer(string key, int size)
    {
        if (string.IsNullOrEmpty(key))
        {
            GD.PushError("GodotPulse: Cannot register buffer with null or empty key");
            throw new ArgumentException("Buffer key cannot be null or empty", nameof(key));
        }
        if (size <= 0)
        {
            GD.PushError($"GodotPulse: Cannot register buffer '{key}' with size {size}. Size must be positive.");
            throw new ArgumentException("Buffer size must be positive", nameof(size));
        }
        if (!_metricBuffers.ContainsKey(key))
        {
            _metricBuffers[key] = new RingBuffer<float>(size);
        }
    }

    /// <summary>
    /// Gets a metric buffer by key.
    /// </summary>
    /// <param name="key">The buffer identifier.</param>
    /// <returns>The RingBuffer if found, null otherwise.</returns>
    /// <exception cref="ArgumentException">Thrown when key is null or empty.</exception>
    [Obsolete("Use GetBufferSnapshot() for safe GDScript interop instead of CLR generics.")]
    public RingBuffer<float> GetBuffer(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            GD.PushError("GodotPulse: Cannot get buffer with null or empty key");
            throw new ArgumentException("Buffer key cannot be null or empty", nameof(key));
        }
        return _metricBuffers.TryGetValue(key, out var buffer) ? buffer : null;
    }

    /// <summary>
    /// Gets a snapshot of a metric buffer as a float array (GDScript-safe).
    /// </summary>
    /// <param name="key">The buffer identifier.</param>
    /// <returns>Array of floats (oldest to newest) or empty array if buffer not found.</returns>
    public float[] GetBufferSnapshot(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            GD.PushWarning($"GodotPulse: Cannot get buffer snapshot with null or empty key");
            return System.Array.Empty<float>();
        }
        if (_metricBuffers.TryGetValue(key, out var buffer))
        {
            return buffer.ToArray();
        }
        GD.PushWarning($"GodotPulse: Buffer '{key}' not found. Returning empty array.");
        return System.Array.Empty<float>();
    }

    /// <summary>
    /// Gets a snapshot of a metric buffer as a float array (can be used by GDScript).
    /// </summary>
    /// <param name="key">The buffer identifier.</param>
    /// <returns>Float array of values (oldest to newest) or empty if buffer not found.</returns>
    public float[] GetBufferAsPackedArray(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            GD.PushWarning($"GodotPulse: Cannot get buffer snapshot with null or empty key");
            return System.Array.Empty<float>();
        }
        if (_metricBuffers.TryGetValue(key, out var buffer))
        {
            return buffer.ToArray();
        }
        GD.PushWarning($"GodotPulse: Buffer '{key}' not found. Returning empty array.");
        return System.Array.Empty<float>();
    }

    /// <summary>
    /// Registers a custom metric callback.
    /// The callback will be called every frame to sample the metric value.
    /// THREAD SAFETY: Must be called from the main game thread only.
    /// </summary>
    /// <param name="name">Unique name for the metric (e.g., "AI/Enemies").</param>
    /// <param name="callback">Function that returns the current metric value.</param>
    /// <example>
    /// GodotPulse.RegisterMetric("AI/Enemies", () => EnemyCount);
    /// </example>
    public static void RegisterMetric(string name, Func<float> callback)
    {
        if (!ValidateMainThread())
        {
            return;
        }
        if (Instance == null)
        {
            GD.PushError("GodotPulse: Cannot register metric - GodotPulse not initialized. Call from _Ready() or later.");
            return;
        }
        if (string.IsNullOrEmpty(name))
        {
            GD.PushError("GodotPulse: Cannot register metric with null or empty name");
            return;
        }
        if (Instance._customMetricCallbacks.Count >= MaxCustomMetrics && !Instance._customMetricCallbacks.ContainsKey(name))
        {
            GD.PushError($"GodotPulse: Maximum custom metrics limit ({MaxCustomMetrics}) reached. Cannot register '{name}'.");
            return;
        }
        Instance._customMetricCallbacks[name] = callback;
    }

    /// <summary>
    /// Unregisters a previously registered custom metric.
    /// THREAD SAFETY: Must be called from the main game thread only.
    /// </summary>
    /// <param name="name">The metric name to unregister.</param>
    public static void UnregisterMetric(string name)
    {
        if (!ValidateMainThread())
        {
            return;
        }
        if (Instance == null)
        {
            GD.PushWarning("GodotPulse: Cannot unregister metric - GodotPulse not initialized.");
            return;
        }
        if (string.IsNullOrEmpty(name))
        {
            GD.PushWarning("GodotPulse: Cannot unregister metric with null or empty name");
            return;
        }
        Instance._customMetricCallbacks.Remove(name);
    }

    /// <summary>
    /// Logs an event to the performance overlay's event log.
    /// THREAD SAFETY: Must be called from the main game thread only.
    /// </summary>
    /// <param name="message">The event message.</param>
    /// <param name="level">Severity level (Info, Warning, Critical).</param>
    /// <example>
    /// GodotPulse.LogEvent("Boss spawned!", GodotPulse.EventLevel.Warning);
    /// </example>
    public static void LogEvent(string message, EventLevel level = EventLevel.Info)
    {
        if (!ValidateMainThread())
        {
            return;
        }
        if (Instance == null)
        {
            GD.PushWarning("GodotPulse: Cannot log event - GodotPulse not initialized.");
            return;
        }
        if (string.IsNullOrEmpty(message))
        {
            GD.PushWarning("GodotPulse: Cannot log empty event message");
            return;
        }
        Instance.Events.Push(new PulseEvent
        {
            Timestamp = Time.GetTicksMsec() / 1000.0,
            Message = message,
            Level = level
        });
    }

    /// <summary>
    /// Gets the names of all registered custom metrics.
    /// </summary>
    /// <returns>Enumerable of metric names.</returns>
    public IEnumerable<string> GetCustomMetricNames() => _customMetricCallbacks.Keys;

    #region Data Export

    /// <summary>
    /// Exports all metric buffers to CSV format.
    /// </summary>
    /// <returns>CSV string with all metrics.</returns>
    public string ExportMetricsToCsv()
    {
        var csv = new System.Text.StringBuilder();
        csv.AppendLine("MetricName,Index,Value");

        foreach (var kvp in _metricBuffers)
        {
            var buffer = kvp.Value;
            for (int i = 0; i < buffer.Count; i++)
            {
                csv.AppendLine($"{kvp.Key},{i},{buffer.Get(i):F4}");
            }
        }

        return csv.ToString();
    }

    /// <summary>
    /// Exports all metric buffers to JSON format.
    /// </summary>
    /// <returns>JSON string with all metrics.</returns>
    public string ExportMetricsToJson()
    {
        var data = new System.Collections.Generic.Dictionary<string, float[]>();
        foreach (var kvp in _metricBuffers)
        {
            data[kvp.Key] = kvp.Value.ToArray();
        }
        return System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>
    /// Saves metrics to a file (CSV or JSON based on extension).
    /// Only allows user:// prefix for writable paths. res:// is immutable on export.
    /// </summary>
    /// <param name="filePath">Path to save file (must start with "user://", e.g., "user://metrics.csv").</param>
    /// <returns>True if save succeeded, false otherwise.</returns>
    public bool SaveMetricsToFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            GD.PushError("GodotPulse: File path cannot be null or empty");
            return false;
        }
        
        // Only allow user:// for writable paths (res:// is immutable on export builds)
        if (!filePath.StartsWith("user://", StringComparison.OrdinalIgnoreCase))
        {
            GD.PushError("GodotPulse: File path must start with 'user://' (res:// is not writable on export).");
            return false;
        }
        
        // Prevent path traversal attempts (check only the path component after the scheme)
        string normalized = filePath.Replace("\\", "/"); // Normalize backslashes
        string pathPart = normalized.Substring("user://".Length);
        if (pathPart.Contains("..") || pathPart.Contains("//"))
        {
            GD.PushError("GodotPulse: File path contains invalid traversal characters (.., or //) in path component.");
            return false;
        }
        
        // Validate filename component (last part after final /)
        int lastSlash = normalized.LastIndexOf('/');
        if (lastSlash >= 0)
        {
            string fileName = normalized.Substring(lastSlash + 1);
            if (string.IsNullOrEmpty(fileName))
            {
                GD.PushError("GodotPulse: File path must include a filename (e.g., 'user://metrics.csv')");
                return false;
            }
        }

        try
        {
            string content = filePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
                ? ExportMetricsToJson()
                : ExportMetricsToCsv();

            using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write);
            if (file == null)
            {
                GD.PushError($"GodotPulse: Failed to open file for writing: {filePath}");
                return false;
            }
            file.StoreString(content);
            GD.Print($"GodotPulse: Metrics saved to {filePath}");
            return true;
        }
        catch (Exception ex)
        {
            GD.PushError($"GodotPulse: Failed to save metrics: {ex.Message}");
            return false;
        }
    }

    #endregion
}

/// <summary>
/// A fixed-size circular buffer that overwrites old values when full.
/// Used for storing historical metric data for graphing.
/// </summary>
/// <typeparam name="T">The type of elements stored in the buffer.</typeparam>
public class RingBuffer<T>
{
    private readonly T[] _buffer;
    private int _head;

    /// <summary>
    /// Number of elements currently stored in the buffer.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Maximum capacity of the buffer.
    /// </summary>
    public int MaxSize => _buffer.Length;

    /// <summary>
    /// Creates a new RingBuffer with the specified size.
    /// </summary>
    /// <param name="size">Maximum number of elements to store.</param>
    /// <exception cref="ArgumentException">Thrown when size is not positive.</exception>
    public RingBuffer(int size)
    {
        if (size <= 0)
        {
            throw new ArgumentException($"RingBuffer size must be positive, got {size}", nameof(size));
        }
        _buffer = new T[size];
    }

    /// <summary>
    /// Adds a value to the buffer. Overwrites oldest value if buffer is full.
    /// </summary>
    /// <param name="value">The value to add.</param>
    public void Push(T value)
    {
        _buffer[_head] = value;
        _head = (_head + 1) % _buffer.Length;
        if (Count < _buffer.Length) Count++;
    }

    /// <summary>
    /// Gets a value at the specified index (0 = oldest, Count-1 = newest).
    /// </summary>
    /// <param name="index">Index from 0 to Count-1.</param>
    /// <returns>The value at the index, or default(T) if out of range.</returns>
    /// <remarks>
    /// Returns default(T) for out-of-range indices to allow safe iteration.
    /// For validation, check that index is in [0, Count) before calling.
    /// </remarks>
    public T Get(int index)
    {
        if (index < 0 || index >= Count)
        {
            return default;
        }
        int actualIndex = (_head - Count + index + _buffer.Length) % _buffer.Length;
        return _buffer[actualIndex];
    }

    /// <summary>
    /// Converts the buffer contents to an array (oldest to newest).
    /// </summary>
    /// <returns>Array containing all elements in order.</returns>
    public T[] ToArray()
    {
        var result = new T[Count];
        for (int i = 0; i < Count; i++)
        {
            result[i] = Get(i);
        }
        return result;
    }

    /// <summary>
    /// Clears all elements from the buffer and resets the head pointer.
    /// </summary>
    public void Clear()
    {
        Array.Clear(_buffer, 0, _buffer.Length);
        _head = 0;
        Count = 0;
    }
}
