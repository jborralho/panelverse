# 11 — Security & Privacy

## Privacy by default
- No analytics or telemetry by default; opt-in for crash reports
- All data stored locally (SQLite, cache directories)

## Permissions
- Minimal file system access; user-chosen directories only
- No network access unless user enables online features (e.g., metadata APIs)

## Hardening
- Validate file paths and sanitize archive entries; deny absolute/parent traversal
- Use temp directories securely; clean up on crash/restart
- Handle external tool invocation (7-Zip) safely; escape arguments; restrict paths
