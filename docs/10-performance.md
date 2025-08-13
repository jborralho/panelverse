# 10 — Performance

## Strategy
- Decode on demand with lookahead prefetch window (background Tasks)
- Memory LRU for N recent images; disk cache for decoded/resized pages and thumbnails
- Resize images to display target using Skia; avoid over-decoding huge assets
- Stream pages directly from archive when possible; memory-map large files on desktop

## Prefetching
- Dynamically adjust window based on user turn rate and scroll velocity
- Pause prefetch under CPU/IO pressure; back off using a token/budget

## Image pipeline
- Use Avalonia/Skia `Bitmap` for display; optional ImageSharp for transformations
- Honor EXIF orientation; pre-rotate at decode to reduce runtime transforms
- Prefer GPU surfaces where possible to minimize CPU copies

## Metrics (internal)
- Time-to-first-page, page turn latency, cache hit rate, memory use, GC stats
