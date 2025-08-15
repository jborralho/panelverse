using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Panelverse.App.ViewModels;
using Panelverse.App.Services;

namespace Panelverse.App;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Cleanup any leftover reader temp folders on startup
            try { ReaderTempService.ClearAll(); } catch { }
            desktop.MainWindow = new MainWindow
            {
                DataContext = new ShellViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}