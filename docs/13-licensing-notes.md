# 13 — Licensing notes

## Project license
- The application code is released under **Apache License 2.0**.
- Repository includes `LICENSE` and `NOTICE` files. Retain these when redistributing binaries or source.

## Third‑party dependencies
The project consumes the following NuGet packages (see `NOTICE` for versions and license identifiers):

- UI/App: Avalonia (and related packages) — MIT, CommunityToolkit.Mvvm — MIT, Serilog.Sinks.Console — Apache-2.0
- Core: MetadataExtractor — Apache-2.0, Microsoft.Data.Sqlite — MIT, SharpCompress — MIT
- Test-only: Avalonia.Headless — MIT, xUnit — Apache-2.0, Microsoft.NET.Test.Sdk — MIT, coverlet.collector — MIT, FluentAssertions — Apache-2.0

Refer to upstream repositories/NuGet pages for full texts. Keep `NOTICE` up to date when you change package versions.

## RAR/CBR support
- 7-Zip components are licensed under LGPL with an additional unRAR restriction. If bundling `7z.dll`/`7zz`, include license texts and attributions.
- The official UnRAR library is licensed under the UnRAR License, which imposes restrictions.
- RAR support will be optional and behind a feature flag. Users who do not need RAR can build without it and use `.cbz` exclusively.

Implementation note: Current RAR handling in code uses `SharpCompress` (MIT). If you later opt to bundle platform 7-Zip or UnRAR binaries for broader RAR/RAR5 coverage, you must:
- Ship the corresponding license files alongside your distribution
- Update `NOTICE` with attributions
- Gate the feature and document how to build without RAR

Consult legal guidance if distributing binaries with RAR support in certain jurisdictions.
