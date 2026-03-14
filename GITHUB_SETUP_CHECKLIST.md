# GitHub Repository Setup Checklist

This checklist helps ensure your repository is professionally configured for submission to the Godot Asset Store.

## ✅ Completed

- [x] **License** - MIT License added
- [x] **Documentation**
  - [x] Root README.md with feature overview, badges, and installation
  - [x] INSTALLATION.md with troubleshooting guide
  - [x] Addon README.md with API documentation
  - [x] CHANGELOG.md with version history
- [x] **Community**
  - [x] CODE_OF_CONDUCT.md
  - [x] CONTRIBUTING.md with development guidelines
  - [x] SECURITY.md 
- [x] **Issue Templates**
  - [x] Bug report template
  - [x] Feature request template
- [x] **Pull Request Template** - PR_TEMPLATE.md
- [x] **Plugin Metadata** - plugin.cfg with tags and URLs
- [x] **.gitignore** - Comprehensive Godot/C# patterns
- [x] **Asset Store Description** - ASSET_STORE_DESCRIPTION.md

## 📋 Recommended GitHub Settings (Manual Setup)

### Repository Settings → General

- [x] ✅ **Description**: "Professional in-game performance overlay for Godot 4 .NET projects"
- [ ] **Website**: https://github.com/Synaptikal/GodotPulse#documentation
- [x] ✅ **Topics**: godot, performance, overlay, profiling, dotnet, plugin
  - Add manually in GitHub: Settings → About → Add Topics

### Repository Settings → Code Security

- [ ] **Enable branch protection rules** (optional for production)
  - Protect: `main` branch
  - Require pull request reviews before merging
  - Require status checks to pass before merging

### Repository Features

- [ ] **Enable Discussions** 
  - Go to: Settings → Features → Check "Discussions"
  - Useful for community Q&A and feature discussions

- [ ] **Enable Sponsors** (optional)
  - Go to: Settings → Sponsors
  - Helps community support your work

### Labels

Common labels to create in GitHub (Settings → Labels):
- `bug` - Bug reports (already created by template)
- `enhancement` - Feature requests (already created by template)
- `documentation` - Documentation updates
- `performance` - Performance-related issues
- `help wanted` - Community help needed
- `good first issue` - Good for new contributors
- `critical` - Critical issues affecting functionality

## 📦 Asset Store Submission Checklist

Before submitting to the Asset Library:

### Code Quality
- [x] All source code is clean and well-documented
- [x] No debug console output unless necessary
- [x] Configuration is properly handled
- [ ] **Run final tests**: `dotnet test`

### Release Preparation
- [ ] Create a git tag for the release: `git tag -a v1.0.0 -m "Release version 1.0.0"`
- [ ] Push tags: `git push origin v1.0.0`
- [ ] Create a [GitHub Release](../../releases) with:
  - Release notes from CHANGELOG.md
  - Attach the plugin zip file
  - Mark as latest release

### Documentation
- [x] README is comprehensive and clear
- [x] Installation instructions are step-by-step
- [x] API documentation is included
- [x] Examples are provided

### Asset Store Specific
- [ ] Copy content from `ASSET_STORE_DESCRIPTION.md` to Asset Store listing
- [ ] Provide a good thumbnail/icon (if possible)
- [ ] Include screenshots showing the overlay in action
- [ ] Set category: **Profiling** or **UI Tools**
- [ ] Set version: **1.0.0**
- [ ] Accept asset store terms and conditions

## 🔗 Helpful Links

- [Godot Asset Store](https://godotengine.org/asset-library)
- [Asset Store Submission Guidelines](https://docs.godotengine.org/en/stable/community/asset_library/)
- [GitHub Release Creating](https://docs.github.com/en/repositories/releasing-projects-on-github/creating-releases)

## 📞 Support

After launch:
- Monitor **Issues** for bug reports
- Engage in **Discussions** for community support
- Update **CHANGELOG.md** with maintenance releases
- Create new releases when appropriate

---

**Status**: ✅ Repository is ready for Asset Store submission!

See [ASSET_STORE_DESCRIPTION.md](ASSET_STORE_DESCRIPTION.md) for asset store listing content.
