# Security Policy

## Supported Versions

| Version | Supported          | Notes |
|---------|-------------------|-------|
| 1.0.x   | ✅ Yes            | Latest release |
| < 1.0   | ❌ No             | Pre-release |

## Reporting a Vulnerability

**Do not** open a public GitHub issue for security vulnerabilities.

Instead, please email security concerns to: **justin@synaptikal.dev**

Please include:
1. Description of the vulnerability
2. Steps to reproduce (if applicable)
3. Affected version(s)
4. Suggested fix (if available)

I will acknowledge receipt of your report within 48 hours and will work on a fix.

## Security Considerations

GodotPulse is designed strictly as a **development/debugging tool** and should:
- ✅ Be used primarily in Development and Debug builds
- ✅ Be disabled in Release builds (automatic by default)
- ⚠️ Not be relied upon for gameplay mechanics or critical systems

## Dependencies

GodotPulse's security depends on:
- Godot Engine's stability and security
- .NET runtime security
- Third-party libraries (see GoDotPulse.csproj for dependencies)

---

Thank you for helping keep GodotPulse secure.
