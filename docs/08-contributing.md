# 08 — Contributing

## Ground rules
- Be respectful; prefer issues first for larger changes
- Small, focused PRs with clear descriptions
- Add tests where practical; keep code readable

## Code style
- .NET: `dotnet format`, analyzers (StyleCop/FxCop), `Serilog` for logs, early returns
- Avalonia: MVVM (CommunityToolkit.Mvvm), accessible UI components

## Commits
- Conventional commits preferred (feat, fix, docs, chore, refactor, test)
- Reference issues (e.g., `Fixes #42`)

## PR checklist
- Feature behind flags when risky
- Docs updated (README or `docs/`)
- No new warnings; CI green
