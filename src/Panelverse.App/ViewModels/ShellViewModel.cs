using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Panelverse.App.ViewModels;

public partial class ShellViewModel : ObservableObject
{
	[ObservableProperty] private object? _currentViewModel;

	public LibraryViewModel Library { get; }

	public ShellViewModel()
	{
		Library = new LibraryViewModel();
		CurrentViewModel = Library;
	}

	[RelayCommand]
	public void OpenItem(LibraryItemViewModel item)
	{
		CurrentViewModel = new ReaderViewModel(item.Id, item.Title, item.LocationPath);
	}

	[RelayCommand]
	public void NavigateBackToLibrary()
	{
		CurrentViewModel = Library;
	}
}


