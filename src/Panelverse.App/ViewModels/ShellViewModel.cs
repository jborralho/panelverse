using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Panelverse.Core.Library;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel;

namespace Panelverse.App.ViewModels;

public partial class ShellViewModel : ObservableObject
{
	[ObservableProperty] private object? _currentViewModel;

	public LibraryViewModel Library { get; }
    private ReaderViewModel? _activeReader;
    private readonly LibraryRepository _repository = new LibraryRepository(System.IO.Path.Combine(System.AppContext.BaseDirectory, "panelverse.db"));

	public ShellViewModel()
	{
		Library = new LibraryViewModel();
		CurrentViewModel = Library;
	}

	[RelayCommand]
	public async Task OpenItem(LibraryItemViewModel item)
	{
		_activeReader?.Dispose();
		var dto = await _repository.GetByIdAsync(item.Id);
		var startIndex = dto?.PagesRead ?? item.PagesRead;
		_ = _repository.UpdateLastOpenedAtAsync(item.Id); // fire-and-forget timestamp update
		_activeReader = new ReaderViewModel(item.Id, item.Title, item.LocationPath, startIndex: startIndex);
		_activeReader.PropertyChanged += ChildPropertyChanged;
        CurrentViewModel = _activeReader;
		OnPropertyChanged(nameof(CurrentViewModel));
    }

    public event PropertyChangedEventHandler? ChildPropertyChanged;


    [RelayCommand]
	public void NavigateBackToLibrary()
	{
		var reader = _activeReader;
		_activeReader = null;
		CurrentViewModel = Library;
		OnPropertyChanged(nameof(CurrentViewModel));
		try
		{
			reader?.Dispose();
			if (reader is not null)
			{
				_ = RefreshItemAsync(reader.Id);
			}
		}
		catch { }
	}

	private async Task RefreshItemAsync(long id)
	{
		var dto = await _repository.GetByIdAsync(id);
		if (dto is null) return;
		var vm = Library.Items.FirstOrDefault(i => i.Id == id);
		if (vm is null) return;
		var isCompleted = dto.PagesTotal > 0 && dto.PagesRead >= dto.PagesTotal;
		var isUnread = dto.PagesRead <= 0;
		var isInProgress = !isUnread && !isCompleted;
		var percent = dto.PagesTotal > 0 ? (dto.PagesRead * 100.0) / dto.PagesTotal : 0;
		vm.PagesLabel = $"{dto.PagesRead}/{dto.PagesTotal}";
		vm.ProgressPercent = percent;
		vm.IsUnread = isUnread;
		vm.IsInProgress = isInProgress;
		vm.IsCompleted = isCompleted;
		vm.StatusBrush = isCompleted ? "#10B981" : isInProgress ? "#3B82F6" : "#9CA3AF";
		vm.Title = dto.Title; // in case title changed elsewhere
		// start index for next open is fetched live from repository
	}
}


