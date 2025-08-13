# Panelverse Reader (Spec)

[![CI](https://github.com/jborralho/panelverse/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/jborralho/panelverse/actions/workflows/ci.yml)

A fast, privacy-first, multi-platform reader for `CBR`/`CBZ` (and friends). Built with .NET and Avalonia.

> Status: This repository currently contains specifications and documentation only. Implementation comes next.

## Why Panelverse
- **Multi-platform**: Windows, macOS, Linux at launch; path to iOS/Android with Avalonia Mobile.
- **Performance**: Pre-fetching, caching, and native archive decoding for smooth page turns.
- **Reading-first UX**: Right-to-left (manga), single/double page, continuous scroll, gestures, and rich shortcuts.
- **Library management**: Folder scanning, ComicInfo metadata, covers, reading progress, search.
- **Privacy**: Local-first; no trackers, no analytics by default.

## Recommended Stack (Decision)
- **App shell**: Avalonia UI (.NET 8)
- **Core**: C#/.NET (archive I/O, image pipeline, caching, database)
- **UI**: Avalonia XAML + MVVM (CommunityToolkit.Mvvm)
- **Data**: SQLite (`Microsoft.Data.Sqlite` or `SQLitePCLRaw`), thumbnails on disk, in-memory LRU image cache
- **Archives**:
  - CBZ/ZIP: built-in `System.IO.Compression` or `SharpCompress`
  - CBR/RAR: optional via bundled 7-Zip runtime (e.g., `SevenZipExtractor`/`SevenZipSharp`) or UnRAR (feature-flagged)
  - Others (CBT/TAR, PDF): optional feature flags

Rationale and alternatives are detailed in `docs/03-stack.md`.

## Core Features (MVP → Plus)
- **Reader**: single/double page, RTL/LTR, fit-width/height, pan/zoom, continuous scroll
- **Performance**: prefetch next pages, lazy image decode, GPU-accelerated rendering via Skia
- **Library**: scan folders, parse `ComicInfo.xml`, covers/thumbnails, progress tracking, bookmarks
- **Convenience**: file associations for `.cbr`, `.cbz`; drag & drop; keyboard + touch gestures
- **Accessibility**: themes, font scaling for UI, high-contrast mode

Extended features and milestones live in `docs/04-features.md` and `docs/07-roadmap.md`.

## Documentation
- Start with `docs/README.md` for the full map:
  - Product spec: `docs/01-product-spec.md`
  - Architecture: `docs/02-architecture.md`
  - Stack decisions: `docs/03-stack.md`
  - Features: `docs/04-features.md`
  - File formats: `docs/05-file-formats.md`
  - Build/run (dev setup): `docs/06-build-run.md`
  - Roadmap: `docs/07-roadmap.md`
  - Contributing: `docs/08-contributing.md`
  - UX notes: `docs/09-ux.md`
  - Performance: `docs/10-performance.md`
  - Security & Privacy: `docs/11-security-privacy.md`
  - Testing: `docs/12-testing.md`
  - Licensing notes: `docs/13-licensing-notes.md`
  - CI: GitHub Actions workflow at `.github/workflows/ci.yml` runs build, tests, and coverage on push/PR.
    - Outputs HTML coverage report (`coveragereport/`) and uploads artifacts per OS.

## Getting Started (Spec-Only Phase)
Implementation hasn’t started yet. When we begin coding, follow `docs/06-build-run.md` to bootstrap the Avalonia app and core library.

## License
TBD (project code). RAR decoding may rely on the UnRAR library or 7-Zip components; see `docs/13-licensing-notes.md` for details.
