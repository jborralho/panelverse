using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using System;
using System.Linq;

namespace Panelverse.App;

public partial class MainWindow : Window
{
    private ScrollViewer? _readerScroll;
    public MainWindow()
    {
        InitializeComponent();
        this.DataContextChanged += OnDataContextChanged;
        _readerScroll = this.FindControl<ScrollViewer>("ReaderScroll");
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
                        // Find the ScrollViewer in the visual tree after the template is applied
                        var contentControl = this.GetVisualDescendants().OfType<ContentControl>().FirstOrDefault();
                        if (contentControl != null)
                        {
                            var scrollViewer = contentControl.GetVisualDescendants().OfType<ScrollViewer>()
                                .FirstOrDefault(s => s.Name == "ReaderScroll");

                            if (scrollViewer != null)
                            {
                                scrollViewer.Offset = new Vector(0, 0);
                            }
                        }
                    });
                }
            };
        }
    }
}