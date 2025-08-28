AGENT QUICK REF (Panelverse)
1. Build: dotnet restore; dotnet build -c Debug (or Release). Format: dotnet format (enforce before PR). CI uses Release.
2. Run app: dotnet run --project src/Panelverse.App
3. Test all: dotnet test -c Debug (CI: dotnet test -c Release --collect:"XPlat Code Coverage").
4. Test single project: dotnet test tests/Panelverse.Core.Tests/Panelverse.Core.Tests.csproj
5. Test single class/file: dotnet test --filter FullyQualifiedName~NaturalSortTests
6. Test single method: dotnet test --filter "FullyQualifiedName=Panelverse.Core.Tests.NaturalSortTests.Null_safety"
7. Coverage local (example): dotnet test -c Release --collect:"XPlat Code Coverage" && reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coveragereport
8. Conventions: C# 12/.NET 8, file-scoped namespaces, PascalCase types/methods, camelCase locals/parameters, UPPER_SNAKE for const, private fields _camelCase.
9. Nullability: treat reference types as nullable-aware; validate public inputs early (ArgumentNullException.ThrowIfNull). Prefer explicit types over var when not obvious.
10. Imports: place System.* first, then third-party, then internal (Panelverse.*); inside namespace (file-scoped). Remove unused (dotnet format). Keep using directives sorted.
11. Error handling: fail fast, narrow try/catch; swallow only when safe (TODO: add logging later via Serilog). Return null vs throwing for expected absence; throw for programmer error.
12. Async: use async suffix, pass CancellationToken, avoid async void (except event handlers). ConfigureAwait(false) not required for console/desktop but acceptable.
13. Data access: parameterize all SQL (already using $params). Avoid string interpolation inside SQL text.
14. Testing: xUnit + FluentAssertions; UI uses Avalonia.Headless [AvaloniaFact]. Use Theory+InlineData for matrix; cover null/edge paths. Prefer deterministic ordering (NaturalSort).
15. Sorting: NaturalSort.Compare for human ordering; keep logic pure/static; add property-based tests (FsCheck future).
16. Threading: background workers cancellable (see CacheService); dispose CancellationTokenSource; avoid blocking waits except controlled shutdown.
17. Performance: prefer streaming (ZipArchive entries) and pre-calculate hashes with SHA256 for cache keys; avoid premature allocation.
18. Security/privacy: no telemetry (DOTNET_CLI_TELEMETRY_OPTOUT=1 in CI). Validate file paths before IO; future: sandbox archive extraction.
19. Commits: Conventional (feat/fix/refactor/test/docs/chore); small focused PRs; update docs. Ensure dotnet format passes and no new warnings.
20. Flags/future: feature-gate optional RAR/7z/PDF; keep thumbnails/cache logic resilient (ignore individual failures, log later).
