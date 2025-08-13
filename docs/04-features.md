# 04 — Features

## MVP
- Open and read `.cbz` files
- Reader modes: single page, double page, RTL/LTR, fit width/height, zoom/pan
- Prefetch next pages; fast next/prev navigation
- Library: scan folders, covers/thumbnails, reading progress, recent
- Metadata: parse `ComicInfo.xml` when present
- Shortcuts and gestures; theming (light/dark/high-contrast)
- File associations for `.cbz`; drag and drop

## Post-MVP (Core)
- Optional `.cbr` support via 7-Zip feature flag; UnRAR optional
- Continuous scroll mode with smooth prefetch windowing
- Bookmarks; per-book notes
- Search library; filters (unread, in-progress, completed)
- Spread detection and alignment

## Nice to have
- OPDS import; remote catalogs
- PDF support
- Cloud sync (settings/progress) via user-chosen provider
- Advanced image adjustments (gamma, contrast, sharpen)
- i18n, full RTL UI support
- Hardware-accelerated image decode paths (Skia)
