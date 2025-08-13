# 12 — Testing

## Core (.NET)
- Unit tests with xUnit for archive adapters (ZIP, RAR behind feature)
- Property-based tests for filename sorting and page ordering (FsCheck)
- Integration tests for prefetch window behavior and cache eviction

## UI (Avalonia)
- Component tests with `Avalonia.Headless`
- UI interaction tests with `Avalonia.UITesting`

## E2E (desktop)
- Smoke tests launching the app and reading a sample book per OS (GitHub Actions matrix)

## Benchmarks
- Micro-benchmarks for image resize and decode; track regressions (BenchmarkDotNet)
