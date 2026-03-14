# GodotPulse

**GodotPulse** is a professional in-game, toggleable runtime performance overlay and diagnostics plugin for Godot 4 .NET projects.

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Godot Version](https://img.shields.io/badge/Godot-4.2+-blue.svg)](https://godotengine.org)
[![.NET Version](https://img.shields.io/badge/.NET-8.0+-5C2D91.svg)](https://dotnet.microsoft.com)

## Overview

GodotPulse provides real-time performance monitoring directly in your game with minimal overhead. Toggle the overlay during gameplay with a hotkey to inspect engine metrics, .NET runtime statistics, and custom metrics.

### Key Features

- 🎮 **In-Game Overlay** - Real-time performance monitoring with toggleable UI
- 📊 **Runtime Metrics** - Managed heap size, GC collections (Gen 0/1/2), GC pause durations
- ⚙️ **Engine Metrics** - FPS, frame time, draw calls, VRAM usage, physics bodies, node counts
- 📈 **GC Visualization** - Red flash on FPS graph when garbage collection occurs
- 🔧 **Custom Metrics API** - Easily register and track user-defined metrics
- 📝 **Event Log** - Integrated performance event logging
- 🚀 **Export Safe** - Automatically disabled in release builds (configurable)
- ⌨️ **Configurable Hotkey** - Customize toggle key in Project Settings
- 🎯 **Zero Overhead (When Disabled)** - No performance impact when overlay is off

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

### Basic Usage

**Toggle the overlay during gameplay:**
- Press `F3` (or your custom binding)

**Change the hotkey:**
1. Open **Project → Project Settings → Input Map**
2. Find `godot_pulse_toggle`
3. Modify the key binding

**Register custom metrics:**
```csharp
// Add a custom metric
GodotPulse.RegisterMetric("AI/Enemies", () => EnemyManager.Count);

// Log performance events
GodotPulse.LogEvent("Boss spawned", GodotPulse.EventLevel.Warning);
```

For detailed usage, see [full documentation](addons/godot_pulse/README.md).

## Documentation

- 📖 [Full API Documentation](addons/godot_pulse/README.md)
- 🔧 [Configuration Guide](addons/godot_pulse/README.md#configuration)
- 🎨 [Customization & Theming](addons/godot_pulse/theme/)
- 🧪 [Testing & Building](CONTRIBUTING.md#building-from-source)

## Installation Methods

### Method 1: Asset Store (Recommended)
Install directly from the [Godot Asset Store](https://godotengine.org/asset-library)

### Method 2: GitHub Releases
Download and extract the latest release to your `addons/` folder

### Method 3: From Source
```bash
git clone https://github.com/Synaptikal/GodotPulse.git
cp -r GodotPulse/addons/godot_pulse your-project/addons/
```

## Compatibility

| Feature | Support |
|---------|---------|
| Export Builds | ✅ Automatically disabled in release builds |
| Multiplayer | ✅ Each peer has independent overlay instance |
| Mobile (Android/iOS) | ⚠️ Requires System.Text.Json support testing |
| GDScript Integration | ✅ Full C# ↔ GDScript interoperability |

## Contributing

We welcome contributions! Please read our [Contributing Guidelines](CONTRIBUTING.md) for details on:
- Code style and standards
- Submitting bug reports
- Proposing features
- Pull request process

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

## Support & Community

- 🐛 [Report Issues](../../issues)
- 💬 [Discussions](../../discussions)
- 📧 Email: justin@synaptikal.dev

## Author

**Justin Davis** - [Synaptikal](https://github.com/Synaptikal)

---

**Made with ❤️ for the Godot community**
