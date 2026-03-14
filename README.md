# GodotPulse

**GodotPulse** is a professional in-game, toggleable runtime performance overlay and diagnostics plugin for Godot 4 .NET projects.

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Godot Version](https://img.shields.io/badge/Godot-4.2+-blue.svg)](https://godotengine.org)
[![.NET Version](https://img.shields.io/badge/.NET-8.0+-5C2D91.svg)](https://dotnet.microsoft.com)

## Overview

GodotPulse provides real-time performance monitoring directly in your game with minimal overhead. Toggle the overlay during gameplay with a hotkey to inspect engine metrics, .NET runtime statistics, and custom metrics.

### Features

- In-game performance overlay - toggle with F3 during gameplay
- Runtime metrics: managed heap size, GC collections (Gen 0/1/2), GC pause durations
- Engine metrics: FPS, frame time, draw calls, VRAM, physics bodies, node counts
- GC visualization on FPS graph (red flash when GC occurs)
- Custom metrics API for game-specific tracking
- Event log for performance events
- Automatically disabled in release builds (configurable)
- Customizable hotkey via Project Settings
- Zero overhead when disabled

## Requirements

| Requirement | Version |
|------------|---------|
| Godot | 4.2+ |
| .NET | 8.0+ |
| Build Type | Godot with .NET support enabled |
| Platforms | Windows, Linux (desktop exports primary; mobile tested) |

## Quick Start

### Installation

1. **Download** the latest release from [Releases](../../releases)
2. **Copy** `addons/godot_pulse` to your project's `addons/` directory
3. **Enable** the plugin in **Project → Project Settings → Plugins**
4. **Done!** The autoload will initialize automatically

### Usage

**Toggle the overlay:**
- Press `F3` during gameplay (or bind a custom key in Project Settings > Input Map > godot_pulse_toggle)

**Track custom metrics:**
```csharp
// Register a metric
GodotPulse.RegisterMetric("AI/Enemies", () => EnemyManager.Count);

// Log performance events
GodotPulse.LogEvent("Boss spawned", GodotPulse.EventLevel.Warning);
```

See [full documentation](addons/godot_pulse/README.md) for the complete API.

## Documentation

- [Full API Documentation](addons/godot_pulse/README.md)
- [Configuration Guide](addons/godot_pulse/README.md#configuration)
- [Theme Customization](addons/godot_pulse/theme/)
- [Building from Source](CONTRIBUTING.md#building-from-source)

## Installation

**Using Godot Asset Library (recommended):**
1. Open Asset Library in the Godot editor
2. Search for "GodotPulse"
3. Download and install

**Manual installation:**
1. Download the latest release from [Releases](../../releases)
2. Extract `addons/godot_pulse` to your project's `addons/` folder
3. Enable the plugin in Project Settings > Plugins

**From source:**
```bash
git clone https://github.com/Synaptikal/GodotPulse.git
cp -r GodotPulse/addons/godot_pulse your-project/addons/
```

## Compatibility

| Feature | Status |
|---------|--------|
| Export Builds | Automatically disabled in release builds |
| Multiplayer | Each peer has independent overlay instance |
| Mobile (Android/iOS) | Requires System.Text.Json support testing |
| GDScript Integration | Full C# ↔ GDScript interoperability |

## Contributing

Interested in contributing? Check out [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines on code style, bug reports, feature proposals, and PRs.

## Known Limitations

- Draw call tracking uses GDScript management and manual buffer setup
- System.Text.Json availability depends on Godot .NET SDK and platform
- Resource paths (res://) are read-only in exports; metric export uses user:// only

## Performance Impact

- **With Overlay Off**: Negligible (< 0.1ms per frame)
- **With Overlay On**: 1-3ms per frame depending on resolution and graph complexity

## License

This project is licensed under the **MIT License** - see [LICENSE](LICENSE) file for details.

## Changelog

See [CHANGELOG.md](CHANGELOG.md) for version history and release notes.

## Support

- [Report Issues](../../issues)
- [Discussions](../../discussions)
- Email: justin@synaptikal.dev

## Author

**Justin Davis** - [Synaptikal](https://github.com/Synaptikal)
