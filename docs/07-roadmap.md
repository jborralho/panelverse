# 07 — Roadmap

## M1 — Vertical slice (CBZ only)
- Avalonia shell + MVVM scaffolding
- Open `.cbz`, show pages, next/prev, fit-to-width/height
- Basic prefetch (next page), in-memory LRU
- Acceptance: First page < 500 ms; page turns < 100 ms on test device

## M2 — Library basics
- Folder scan, covers/thumbnails, ComicInfo parsing
- SQLite library; recent and progress tracking
- Keyboard shortcuts and basic gestures
- Acceptance: Scan 1k books < a few minutes; UI stays responsive

## M3 — Reader polish
- Continuous scroll; two-page; RTL; spread detection
- Disk cache; configurable prefetch window
- Theming; accessibility passes

## M4 — CBR (optional) + packaging
- RAR via 7-Zip feature flag; optional UnRAR integration
- File associations; installers for Win/macOS/Linux

## M5 — Mobile pathfinding
- Evaluate Avalonia mobile readiness; file pickers; perf tuning

## M6 — Integrations and extras
- OPDS import; optional PDF support; i18n
