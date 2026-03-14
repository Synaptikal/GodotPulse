# Changelog

All notable changes to GodotPulse will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-03-14

### Added
- Initial public release on Godot Asset Store
- In-game toggleable performance overlay with F3 hotkey
- Real-time .NET runtime metrics:
  - Managed heap size tracking
  - Garbage collection statistics (Gen 0/1/2)
  - GC pause duration visualization
  - GC spike detection (red flash on FPS graph)
- Engine performance metrics:
  - Frames per second (FPS) with graph
  - Frame time tracking
  - Draw call count
  - VRAM usage monitoring
  - Physics bodies count (2D/3D)
  - Scene node count
- Custom metrics API for user-defined tracking
- Event log panel for performance events
- Five configurable performance metric panels:
  - DotNet panel (GC and heap metrics)
  - Engine panel (FPS, frame time, draw calls)
  - Physics panel (2D/3D bodies)
  - Render panel (GPU metrics)
  - Memory panel (memory usage)
- Mini graph widget for metric visualization
- Configurable UI scale and opacity
- Dark theme with professional styling
- Automatic disable in release builds (configurable)
- GDScript and C# API support
- Comprehensive documentation and examples
- Unit test suite with RingBuffer tests
- CI/CD pipeline with GitHub Actions
- MIT License

### Documentation
- Comprehensive README with feature overview
- Detailed API documentation
- Installation and configuration guides
- Usage examples for custom metrics and events
- Contributing guidelines

## [Unreleased]

### Planned Features
- [ ] Light theme variant
- [ ] Network profiling panel
- [ ] Asset memory tracking
- [ ] Save/export metrics data
- [ ] Real-time metric alerts
- [ ] Plugin settings UI in-editor
- [ ] Support for Godot 4.3+

---

## Notes for Contributors

When adding new features, please:
1. Update this CHANGELOG under the `[Unreleased]` section
2. Use the format: `- [Component] Description of change`
3. Link to related issues with `Fixes #123` in your PR
4. Include breaking changes in bold: `**BREAKING**: Description`

## Version History

| Version | Release Date | Notes |
|---------|--------------|-------|
| 1.0.0 | 2026-03-14 | Initial release |

---

For detailed upgrade guides, see [CONTRIBUTING.md](CONTRIBUTING.md).
