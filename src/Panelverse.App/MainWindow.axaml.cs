using Avalonia.Controls;
using Avalonia.Input;
using Avalonia;
using Avalonia.Threading;
using System;

namespace Panelverse.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.DataContextChanged += OnDataContextChanged;
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

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        // When the reader's CurrentImage changes, reset the ScrollViewer to top.
        if (DataContext is ViewModels.ShellViewModel shell)
        {
            shell.ChildPropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(ViewModels.ReaderViewModel.CurrentImage))
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        var scroll = this.FindControl<ScrollViewer>("ReaderScroll");
                        //TODO: on runtime, scroll its always null
                        if (scroll != null)
                        {
                            scroll.Offset = new Vector(scroll.Offset.Y, 0);
                        }
                    });
                }
            };
        }
    }
}