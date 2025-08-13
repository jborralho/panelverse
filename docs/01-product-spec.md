# 01 — Product spec

## Problem
Reading comics and manga across platforms is fragmented. Many readers compromise on performance, UX, or format support—particularly for RAR/CBR.

## Goals
- Smooth reading of large archives with instant page turns
- First-class support for CBZ, optional CBR support
- Cross-platform desktop at launch; mobile path identified
- Library management with metadata and reading progress
- Privacy by default; no cloud required; offline-first

## Non-goals (initially)
- DRM-protected content
- Cloud sync and accounts
- Online storefronts or purchases
- Full annotation/mark-up

## Personas
- Casual reader: wants simplicity, keyboard shortcuts, resume where left off
- Manga reader: requires RTL, two-page spreads, continuous scroll
- Collector: cares about library scanning, metadata, covers, and search

## Primary use cases
- Open a `.cbz` and read instantly; resume last page
- Scan a folder of archives into a library (covers + metadata)
- Toggle reading modes (single, double, RTL, continuous)
- Bookmark pages and see reading progress per book
- Search library by series/title/author

## Constraints & risks
- RAR format licensing is restrictive; needs optional integration or fallback
- Large image assets can be memory-heavy; must aggressively cache and pre-scale
- Mobile constraints (iOS file access, background IO) require careful design

## Success metrics
- Time-to-first-page < 500 ms for typical books on SSD
- Page turn latency < 80 ms for next/prev on prefetched pages
- App cold start < 1000 ms on mid-range hardware
- No crashes in common workflows (open, navigate, close) in >99.9% sessions
