using Avalonia.Controls;
using Avalonia.Input;

namespace Panelverse.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Reader_OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (DataContext is ViewModels.ShellViewModel shell && shell.CurrentViewModel is ViewModels.ReaderViewModel reader)
        {
            if (e.Delta.Y < 0)
            {
                reader.NextPage();
            }
            else if (e.Delta.Y > 0)
            {
                reader.PrevPage();
            }
        }
    }
}