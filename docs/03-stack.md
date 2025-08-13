# 03 — Stack decisions

## Decision summary
- Shell/UI: **Avalonia UI (.NET 8)** for cross-platform desktop (Windows/macOS/Linux) and a path to mobile
- Core: **C#/.NET** for archive IO, caching, database, and performance-sensitive logic
- UI pattern: **MVVM** using CommunityToolkit.Mvvm (or ReactiveUI as an alternative)
- Data: **SQLite** via `Microsoft.Data.Sqlite`; thumbnails on disk; memory LRU for decoded images

## Alternatives considered

### .NET MAUI
- Pros: first-party, mobile-centric
- Cons: desktop polish and control density less mature for reader UX; theming differences; Linux support via community

### Electron / Tauri
- Pros: rich web ecosystem; fast prototyping
- Cons: runtime overhead (Electron) or polyglot stack (Tauri) not aligned with .NET expertise

### Flutter
- Pros: strong mobile story, performant UI
- Cons: CBR/CBZ pipeline needs native plugins; packaging size/runtime overhead

## Why Avalonia + .NET
- C#/XAML MVVM aligns with your experience; rapid developer productivity
- Skia-backed rendering suits image-heavy scenarios
- Mature desktop support; viable mobile pathway

## RAR/CBR support options (.NET)
- Option A — **7-Zip** runtime (DLL/exe) via bindings (e.g., `SevenZipExtractor`, `SevenZipSharp`)
  - Pros: broad RAR/RAR5 read support; actively used in many apps
  - Cons: ship per-OS binaries; comply with LGPL and unRAR restrictions; mobile story TBD
- Option B — **UnRAR** library via P/Invoke (feature flag `rar`)
  - Pros: best compatibility
  - Cons: restrictive UnRAR license; distribution considerations
- Option C — **SharpCompress** for RAR (limited) + ZIP
  - Pros: pure .NET
  - Cons: RAR5 support is limited; not reliable for all CBRs

Decision: Support **CBZ/ZIP** by default (System.IO.Compression/SharpCompress). Provide RAR via a separate feature flag using 7-Zip on desktop, with optional UnRAR integration. Mobile RAR support will be revisited in the mobile milestone.

## Core libraries (proposed)
- Async & concurrency: `System.Threading.Channels`, `IAsyncEnumerable`
- Logging: `Serilog`
- Archives: `System.IO.Compression`, `SharpCompress`, `SevenZipExtractor`/`SevenZipSharp` (feature)
- Images: Avalonia `Bitmap`/Skia, optional `ImageSharp` for processing; EXIF via `MetadataExtractor`
- DB: `Microsoft.Data.Sqlite`
- Caching: custom LRU + `MemoryCache`

## UI libraries
- MVVM: CommunityToolkit.Mvvm
- Styling: Avalonia themes, Fluent styles
- Testing: `Avalonia.Headless`, `Avalonia.UITesting`

## Packaging/OS integration
- Single-file, self-contained .NET deployments per OS
- File associations for `.cbz`, `.cbr` via platform manifests (Windows appx/installer, macOS Info.plist, Linux .desktop + mime)
