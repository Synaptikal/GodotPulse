# GodotPulse

**GodotPulse** is an in-game, toggleable runtime performance overlay and diagnostics plugin for Godot 4 .NET projects.

## Requirements

- **Godot Version:** 4.2+
- **.NET Version:** .NET 8.0 or later
- **.NET Support:** Godot must be built with .NET support enabled (`dotnet` feature available)
- **Export Builds:** Only tested on Windows and Linux desktop exports. Mobile (Android/iOS) may require additional configuration for System.Text.Json

## Features

- **.NET Runtime Metrics:** Tracks Managed Heap size, GC Collections (Gen 0/1/2), and GC Pause Durations.
- **Engine Metrics:** FPS, Frame Time, Draw Calls, VRAM usage, Physics Bodies (2D/3D), and Node counts.
- **GC Spike Visualization:** Red flash on the FPS graph when a GC collection occurs.
- **Custom Metrics API:** Easy registration of user-defined metrics.
- **Event Log:** Integrated logging for performance-related events.
- **Export-Safe:** Configurable to be completely disabled in release builds.
- **Configurable Hotkey:** Change the toggle key in Project Settings -> Input Map.

## Installation

1. Copy the `addons/godot_pulse` folder to your project's `addons/` directory.
   - **Important:** Do NOT use the `tools-testing/addons/godotpulse/` folder (legacy stub)
2. Enable the plugin in **Project Settings -> Plugins**.
3. The plugin will automatically:
   - Register the `GodotPulse` C# autoload (editor-level registration; auto-enabled at runtime)
   - Add the `godot_pulse_toggle` input action (default: F3)

## Usage

### Toggle Overlay
Press `F3` during gameplay (or your custom key binding).

To change the hotkey:
1. Go to **Project Settings -> Input Map**
2. Find `godot_pulse_toggle`
3. Add or change the key binding

### Custom Metrics
```csharp
// Register a custom metric
GodotPulse.RegisterMetric("AI/Enemies", () => EnemyCount);

// Unregister when no longer needed
GodotPulse.UnregisterMetric("AI/Enemies");
```

### Logging Events
```csharp
GodotPulse.LogEvent("Boss spawned!", GodotPulse.EventLevel.Warning);
GodotPulse.LogEvent("Level loaded", GodotPulse.EventLevel.Info);
GodotPulse.LogEvent("Out of memory!", GodotPulse.EventLevel.Critical);
```

### Configuration
Create a `pulse_config.tres` resource to customize:
- Toggle action name
- Overlay opacity and scale
- Enable in release builds
- Performance thresholds

## API Changes

### Deprecated Methods (v1.1+)
- `GetBuffer(string key)` — **Use `GetBufferAsPackedArray(string key)` instead** for safe GDScript interop. The CLR generic `RingBuffer<float>` type cannot be reliably marshaled to GDScript.
  - `GetBuffer()` is marked `[Obsolete]` and will be removed in v2.0.

### Safe GDScript Buffer Access (Recommended)
```gdscript
# GDScript: Get buffer as PackedFloat32Array (safe and zero-copy)
var fps_buffer = GodotPulse.GetBufferAsPackedArray("fps")
graph.link_buffer(fps_buffer, 0, 120, Color.SPRING_GREEN)

# Or get raw float array
var fps_array = GodotPulse.GetBufferSnapshot("fps")
```

## Compatibility

- **Exported Builds:** Overlay is **automatically disabled** in release builds unless `GodotPulseConfig.EnableInRelease` is true.
- **Multiplayer:** Each peer has its own overlay instance. No networking overhead.
- **Mobile Export:** Android/iOS may require conditional build flags for System.Text.Json. Test thoroughly.
- **GDScript:** All UI integration with GDScript has been hardened with null-checks. Safe to use with optional C# support.

## Known Limitations

- Draw call tracking is GDScript-managed (RenderPanel.gd) and requires manual buffer setup. Engine-level draw call metrics may not be available on all platforms.
- System.Text.Json availability depends on Godot .NET SDK version and target platform.
- res:// (resource) paths are read-only on export builds; metric export to file uses user:// only.

## Building from Source

```bash
dotnet restore GoDotPulse.sln
dotnet build -c Release
```

## License

MIT
