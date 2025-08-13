# 06 — Build & run (dev setup)

> Implementation hasn’t started yet. These steps outline how we will bootstrap the project once coding begins.

## Prerequisites
- .NET SDK 8.x (LTS)
- Git
- Optional: 7-Zip runtime for RAR support during development (see below)

### OS-specific notes
- Windows
  - Install .NET SDK 8
  - For RAR: place `7z.dll`/`7z.exe` in a known path or configure via app settings
- macOS
  - Install .NET SDK 8 (via installer or Homebrew)
- Linux
  - Install .NET SDK 8 for your distro

## Bootstrap plan
1. Install Avalonia templates
   ```bash
   dotnet new install Avalonia.Templates
   ```
2. Create the solution and projects
   ```bash
   dotnet new sln -n Panelverse
   dotnet new avalonia.app -n Panelverse.App -o src/Panelverse.App
   dotnet new classlib -n Panelverse.Core -o src/Panelverse.Core
   dotnet sln add src/Panelverse.App src/Panelverse.Core
   dotnet add src/Panelverse.App reference src/Panelverse.Core
   ```
3. Add dependencies (examples)
   ```bash
   dotnet add src/Panelverse.Core package Microsoft.Data.Sqlite
   dotnet add src/Panelverse.Core package SharpCompress
   dotnet add src/Panelverse.Core package MetadataExtractor
   dotnet add src/Panelverse.App package CommunityToolkit.Mvvm
   dotnet add src/Panelverse.App package Serilog.Sinks.Console
   # Optional RAR via 7-Zip
   # dotnet add src/Panelverse.Core package SevenZipExtractor
   ```
4. Implement minimal vertical slice
   - Open `.cbz`, list pages, render first page, next/prev in Avalonia `Image` control
5. Add library scanning and thumbnails
6. Package desktop builds (Win/macOS/Linux)

## Running (future)
```bash
dotnet build
dotnet run --project src/Panelverse.App
```

## Packaging (future)
- Windows: MSIX/installer
- macOS: app bundle + notarization
- Linux: .deb/.rpm/AppImage

## RAR support during development
- If using 7-Zip, ensure platform runtime is present (e.g., ship `7z.dll` on Windows, `7zz` on macOS/Linux) and configure its path in app settings.
