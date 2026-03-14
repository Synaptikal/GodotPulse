using Godot;
using System;

namespace GodotPulse;

/// <summary>
/// Configuration resource for GodotPulse performance overlay.
/// </summary>
public partial class GodotPulseConfig : Resource
{
    /// <summary>
    /// Input action name for toggling the overlay. Default: "godot_pulse_toggle"
    /// </summary>
    [Export] public string ToggleAction = "godot_pulse_toggle";

    /// <summary>
    /// Overlay background opacity (0.0 - 1.0). Default: 0.85
    /// </summary>
    [Export(PropertyHint.Range, "0,1,0.05")] public float Opacity = 0.85f;

    /// <summary>
    /// UI scale multiplier. Default: 1.0
    /// </summary>
    [Export(PropertyHint.Range, "0.5,2,0.1")] public float Scale = 1.0f;

    /// <summary>
    /// Enable overlay in release builds. Default: false (debug only)
    /// </summary>
    [Export] public bool EnableInRelease = false;

    /// <summary>
    /// Path to the theme resource. Default: "res://addons/godot_pulse/theme/pulse_dark.tres"
    /// </summary>
    [Export] public string ThemePath = "res://addons/godot_pulse/theme/pulse_dark.tres";

    /// <summary>
    /// Path to the overlay scene. Default: "res://addons/godot_pulse/ui/PulseOverlay.tscn"
    /// Configurable to support addon relocation or custom overlays.
    /// </summary>
    [Export] public string OverlayScenePath = "res://addons/godot_pulse/ui/PulseOverlay.tscn";

    /// <summary>
    /// Path to the config resource. Default: "res://addons/godot_pulse/pulse_config.tres"
    /// Configurable to support addon relocation or custom configs.
    /// </summary>
    [Export] public string ConfigResourcePath = "res://addons/godot_pulse/pulse_config.tres";

    [ExportGroup("Thresholds")]

    /// <summary>
    /// Target FPS for color coding. Above = green, below = yellow/red.
    /// </summary>
    [Export] public int TargetFps = 60;

    /// <summary>
    /// Draw call count that triggers warning (yellow).
    /// </summary>
    [Export] public int DrawCallWarning = 500;

    /// <summary>
    /// Draw call count that triggers critical (red).
    /// </summary>
    [Export] public int DrawCallCritical = 1500;
}
