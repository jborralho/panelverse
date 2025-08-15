using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Panelverse.App.ViewModels;

public partial class ShellViewModel : ObservableObject
{
	[ObservableProperty] private object? _currentViewModel;

	public LibraryViewModel Library { get; }
    private ReaderViewModel? _activeReader;

	public ShellViewModel()
	{
		Library = new LibraryViewModel();
		CurrentViewModel = Library;
	}

	[RelayCommand]
	public void OpenItem(LibraryItemViewModel item)
	{
		_activeReader?.Dispose();
		_activeReader = new ReaderViewModel(item.Id, item.Title, item.LocationPath, startIndex: item.PagesRead);
		CurrentViewModel = _activeReader;
	}

	[RelayCommand]
	public void NavigateBackToLibrary()
	{
		_activeReader?.Dispose();
		_activeReader = null;
		CurrentViewModel = Library;
	}
}


