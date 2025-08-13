# 05 — File formats

## Archives (initial)
- CBZ — ZIP container (supported by default via System.IO.Compression/SharpCompress)
- CBR — RAR container (optional feature; see licensing notes)
- CBT — TAR (optional)
- CB7 — 7z (via 7-Zip fallback; optional)
- PDF — optional (non-MVP)

## Images inside archives
- Supported: JPEG, PNG, WebP (read), AVIF (read if available)
- Ordering: natural sort by filename (e.g., `001.jpg`, `002.jpg`)
- Orientation: honor EXIF rotation where present

## Metadata
- Prefer `ComicInfo.xml` inside archive
- Fallback: infer from filename and folder structure
- Stored in SQLite alongside derived fields (series, volume, issue, title)

## File associations
- Desktop: register `.cbz` (MVP), `.cbr` (when enabled) via OS manifests/installers
- Drag-and-drop a file/folder into the app to open/import
