using Avalonia;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Panelverse.App;

[assembly: AvaloniaTestApplication(typeof(Panelverse.App.Tests.HeadlessSetup))]

namespace Panelverse.App.Tests;

public static class HeadlessSetup
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions
            {
                UseHeadlessDrawing = true
            });
}


