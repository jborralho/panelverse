using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using System.Threading.Tasks;
using Panelverse.App;

namespace Panelverse.App.Tests;

public class MainWindowTests
{
    [AvaloniaFact]
    public void MainWindow_Should_Render()
    {
        var window = new MainWindow();
        window.Show();
        Dispatcher.UIThread.RunJobs();
        Assert.NotNull(window);
        Assert.IsAssignableFrom<Window>(window);
    }
}


