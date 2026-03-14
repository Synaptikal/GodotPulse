# Installation Guide

## Where to Find GodotPulse

### Option 1: Godot Asset Store (Recommended)
Install directly from the [Godot Asset Library](https://godotengine.org/asset-library):
1. Open Godot Editor
2. Go to **AssetLib** tab
3. Search for "GodotPulse"
4. Click **Download**
5. Extract to your project

### Option 2: GitHub Releases
Download from [GitHub Releases](https://github.com/Synaptikal/GodotPulse/releases):
1. Download the latest `godot_pulse_v*.zip`
2. Extract to your project's `addons/` folder

### Option 3: Git Clone
```bash
cd your-project/addons
git clone https://github.com/Synaptikal/GodotPulse.git godot_pulse
```

---

## Requirements Checklist

Before installation, verify:

- [ ] **Godot Version**: 4.2+ (check in Godot → Help → About)
- [ ] **.NET Support**: Godot is built with .NET support enabled
  - On startup, check Godot console for: `Mono: Glue version mismatch` should NOT appear at startup with .NET projects
  - Create a C# script and compile it successfully to verify
- [ ] **.NET SDK**: .NET 8.0 or later installed
  - Verify: `dotnet --version` in terminal

## Installation Steps

### Step 1: Copy Files
```
your-project/
├── addons/
│   └── godot_pulse/              ← Copy here
│       ├── autoload/
│       ├── theme/
│       ├── ui/
│       ├── plugin.cfg
│       ├── plugin.gd
│       ├── README.md
│       └── LICENSE
```

### Step 2: Enable Plugin
1. Open **Project → Project Settings → Plugins**
2. Search for **"GodotPulse"**
3. Click **Enable** checkbox
4. Plugin should show as **Active**

### Step 3: Verify Installation
1. Restart Godot Editor (if needed)
2. Check the **Output** tab for any errors
3. Run your project (Play button)
4. Press **F3** during gameplay to toggle the overlay

✅ **Success!** The overlay should appear.

---

## Troubleshooting

### Problem: Plugin doesn't appear in Plugins list

**Solution:**
1. Verify the folder structure is exactly as shown above
2. Ensure `plugin.cfg` is in the plugin root directory
3. Try **Tools → Reload Current Scene**
4. Restart Godot Editor completely

### Problem: Overlay doesn't toggle with F3

**Check:**
- Is the project running? (Overlay only shows during gameplay)
- Try pressing **F3** a few times
- Check **Project Settings → Input Map** for `godot_pulse_toggle` action

**Custom Hotkey:**
1. Go to **Project Settings → Input Map**
2. Find `godot_pulse_toggle`
3. Change the key binding
4. Run project again

### Problem: Compilation errors with .NET

**Solution:**
1. Ensure .NET 8.0+ SDK is installed: `dotnet --version`
2. Run: `dotnet clean && dotnet restore GoDotPulse.sln`
3. Build in Godot: Press **Build C#** button or **Build → Build Project**
4. Check **Output** tab for specific errors

### Problem: Overlay appears but shows no data

**Solution:**
1. Let the game run for a few frames (data takes time to initialize)
2. Check **Output** for any warnings
3. Verify you're in a scene with active nodes
4. Try toggling overlay off and on (F3)

### Problem: Overlay is very slow / Performance impact

**Solution:**
1. Try reducing the overlay scale: Open `pulse_config.tres` if it exists
2. Disable graph widgets if not needed
3. Verify your game isn't already at 100% CPU before enabling overlay
4. Report performance issues with your hardware specs and scene complexity

---

## Platform-Specific Notes

### Windows
- ✅ Fully supported
- Tested on Windows 10/11
- Monitor performance in Task Manager

### Linux
- ✅ Fully supported
- Monitor performance with `top` or `htop`

### macOS
- ⚠️ Untested (may work, but not officially supported)
- Report issues if you test it

### Mobile (Android/iOS)
- ⚠️ Requires additional configuration
- System.Text.Json availability depends on build configuration
- Test thoroughly before release

---

## What's Next?

- 📖 Read the [full documentation](../addons/godot_pulse/README.md)
- 🔧 Learn about [custom metrics](../addons/godot_pulse/README.md#custom-metrics)
- 📝 See [API documentation](../addons/godot_pulse/README.md#usage)
- 🐛 Report issues or ask questions on [GitHub](https://github.com/Synaptikal/GodotPulse)

---

## Need Help?

- 📖 [Full Documentation](../addons/godot_pulse/README.md)
- 🆘 [GitHub Issues](https://github.com/Synaptikal/GodotPulse/issues)
- 💬 [Discussions](https://github.com/Synaptikal/GodotPulse/discussions)
- 📧 Email: justin@synaptikal.dev

**Happy profiling! 🎮**
