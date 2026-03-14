# Contributing to GodotPulse

Thank you for your interest in contributing to GodotPulse! This document provides guidelines and instructions for contributing.

## Code of Conduct

We are committed to providing a welcoming and inspiring community for all. Please read and adhere to our principles of respectful collaboration.

## How to Contribute

### Reporting Bugs

**Before submitting a bug report:**
1. Check the [existing issues](../../issues) to avoid duplicates
2. Check the [documentation](addons/godot_pulse/README.md) for known limitations

**When submitting a bug report, include:**
- Your Godot version (e.g., 4.2.1)
- Your .NET version (e.g., .NET 8.0)
- OS and platform (Windows 10, Linux, etc.)
- Steps to reproduce the issue
- Expected behavior vs. actual behavior
- Screenshots or error logs if applicable
- GodotPulse version and whether you're using it in an exported build

### Suggesting Enhancements

**Feature requests should include:**
- Clear description of the feature
- Use cases and benefits
- Possible implementation approach (optional)
- Any trade-offs or concerns

### Submitting Pull Requests

#### Before You Start
1. Fork the repository
2. Create a feature branch: `git checkout -b feature/your-feature-name`
3. Make your changes
4. Test thoroughly (see [Building](#building) section)
5. Commit with clear messages
6. Push and submit a pull request

#### PR Guidelines
- **One feature/fix per PR** - Keep PRs focused and reviewable
- **Clear description** - Explain what changes and why
- **Reference issues** - Link to related issues with `Fixes #123`
- **Tests** - Add or update tests for your changes
- **Documentation** - Update docs if API or behavior changes
- **Code style** - Follow existing patterns in the codebase

## Development Setup

### Prerequisites
- Godot 4.2+ (with .NET support)
- .NET 8.0 SDK or later
- Git
- VS Code or your preferred C# editor

### Building from Source

```bash
# Clone the repository
git clone https://github.com/Synaptikal/GodotPulse.git
cd GodotPulse

# Restore dependencies
dotnet restore GoDotPulse.sln

# Build the solution
dotnet build -c Release

# Run tests
dotnet test GoDotPulse.Tests/GoDotPulse.Tests.csproj
```

### Project Structure

```
GoDotPulse/
├── addons/godot_pulse/          # Main plugin directory
│   ├── autoload/                # C# autoload classes
│   │   ├── GodotPulse.cs        # Main API
│   │   └── GodotPulseConfig.cs  # Configuration
│   ├── theme/                   # UI themes and resources
│   ├── ui/                       # UI components (GDScript)
│   │   └── panels/              # Metric panels
│   └── plugin.gd                # Plugin entry point
├── GoDotPulse.Tests/            # Unit tests
├── GoDotPulse.csproj            # C# project file
└── GoDotPulse.sln              # Solution file
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "ClassName"

# Run with verbose output
dotnet test -v d
```

## Code Style

### C# Guidelines

- **Naming**: Use PascalCase for public members, camelCase for private
- **Formatting**: Follow C# conventions (4 spaces for indentation)
- **Comments**: Document public API members with XML comments
- **Error Handling**: Use exceptions for exceptional cases; null-checks for C# ↔ GDScript boundaries

Example:
```csharp
/// <summary>
/// Registers a custom metric that will be displayed in the overlay.
/// </summary>
/// <param name="metricName">The unique name for the metric</param>
/// <param name="getValue">Delegate that returns the metric value</param>
public static void RegisterMetric(string metricName, Func<float> getValue)
{
    // Implementation...
}
```

### GDScript Guidelines

- **Naming**: Use snake_case for functions and variables
- **Signals**: Prefix with `on_` (e.g., `on_panel_opened`)
- **Constants**: Use UPPER_SNAKE_CASE
- **Comments**: Document complex logic

### Documentation

- Update [README.md](addons/godot_pulse/README.md) for user-facing changes
- Add API documentation comments in C# code
- Include examples for new public APIs
- Update [CHANGELOG.md](CHANGELOG.md) with breaking changes

## Commit Messages

Write clear, descriptive commit messages:

```
feat: add memory profiling panel

- Implement memory usage tracking
- Add memory graph visualization
- Integrate with existing metric system

Fixes #42
```

**Prefix types:**
- `feat:` - New feature
- `fix:` - Bug fix
- `docs:` - Documentation
- `style:` - Code style (no logic changes)
- `refactor:` - Code restructuring
- `test:` - Test additions/modifications
- `chore:` - Maintenance tasks

## Release Process

Releases are typically handled by the maintainer. Version numbering follows [Semantic Versioning](https://semver.org/):

- `MAJOR.MINOR.PATCH` (e.g., `1.0.0`)
- Breaking changes increment MAJOR
- New features increment MINOR
- Bug fixes increment PATCH

## Getting Help

- 📖 Check the [documentation](addons/godot_pulse/README.md)
- 💬 Ask in [Discussions](../../discussions)
- 📧 Contact: justin@synaptikal.dev

---

Thank you for contributing to GodotPulse! 🎉
