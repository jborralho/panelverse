using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Panelverse.Core.Library;
using Panelverse.Core.Cache;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Panelverse.App.ViewModels;

public partial class LibraryItemViewModel : ObservableObject
{
	[ObservableProperty] private string _title = "Sample Title";
	[ObservableProperty] private string _seriesAndVolume = "Series • Vol 1";
	[ObservableProperty] private string _pagesLabel = "0/0";
	[ObservableProperty] private double _progressPercent;
	[ObservableProperty] private bool _isUnread;
	[ObservableProperty] private bool _isInProgress;
	[ObservableProperty] private bool _isCompleted;
	[ObservableProperty] private string _statusBrush = "#888";
	[ObservableProperty] private string? _thumbnailPath;
	public long Id { get; init; }
	public string LocationPath { get; init; } = string.Empty;
}

public partial class LibraryViewModel : ObservableObject
{
	[ObservableProperty] private string _searchText = string.Empty;
	public ObservableCollection<string> FilterOptions { get; } = new(["All","Unread","In Progress","Completed"]);
	public ObservableCollection<string> SortOptions { get; } = new(["Title","Recently Added","Progress"]);
	[ObservableProperty] private string _selectedFilter = "All";
	[ObservableProperty] private string _selectedSort = "Title";

	public bool IsDebug
	{
		get
		{
#if DEBUG
			return true;
#else
			return false;
#endif
		}
	}

    public ObservableCollection<LibraryItemViewModel> Items { get; } = new();
    private readonly LibraryRepository _repository;
    private readonly CacheService _cache;

    public LibraryViewModel() : this(new LibraryRepository(System.IO.Path.Combine(System.AppContext.BaseDirectory, "panelverse.db")), null) {}

    public LibraryViewModel(LibraryRepository repository, CacheService? cache = null)
    {
        _repository = repository;
        _cache = cache ?? new CacheService(CacheService.GetDefaultCacheRoot(), _repository);
        _cache.ThumbnailReady += OnThumbnailReady;
        _cache.Start();
        _ = LoadAsync();
    }

    private static string BuildSeriesAndVolume(string? series, int? volume)
        => series is { Length: > 0 } ? (volume.HasValue ? $"{series} • Vol {volume}" : series) : string.Empty;

    private async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        await _repository.InitializeAsync(cancellationToken);

        await foreach (var dto in _repository.GetItemsAsync(cancellationToken))
        {
            var isCompleted = dto.PagesTotal > 0 && dto.PagesRead >= dto.PagesTotal;
            var isUnread = dto.PagesRead <= 0;
            var isInProgress = !isUnread && !isCompleted;
            var percent = dto.PagesTotal > 0 ? (dto.PagesRead * 100.0) / dto.PagesTotal : 0;

            Items.Add(new LibraryItemViewModel
            {
                Id = dto.Id,
                Title = dto.Title,
                SeriesAndVolume = BuildSeriesAndVolume(dto.Series, dto.Volume),
                PagesLabel = $"{dto.PagesRead}/{dto.PagesTotal}",
                ProgressPercent = percent,
                IsUnread = isUnread,
                IsInProgress = isInProgress,
                IsCompleted = isCompleted,
                StatusBrush = isCompleted ? "#10B981" : isInProgress ? "#3B82F6" : "#9CA3AF",
                ThumbnailPath = _cache.GetExistingThumbnailPath(dto.LocationPath),
                LocationPath = dto.LocationPath
            });

            _cache.QueueEnsure(dto);
        }
    }

    private void OnThumbnailReady(string locationPath, string thumbnailPath)
    {
        var name = System.IO.Path.GetFileNameWithoutExtension(locationPath);
        var item = Items.FirstOrDefault(i => string.Equals(i.Title, name, System.StringComparison.OrdinalIgnoreCase) || i.ThumbnailPath is null);
        if (item is not null)
        {
            item.ThumbnailPath = thumbnailPath;
        }
    }

	[RelayCommand]
	private async Task AddFileAsync()
	{
		var dialog = new Avalonia.Controls.OpenFileDialog
		{
			AllowMultiple = true,
			Filters =
			{
				new Avalonia.Controls.FileDialogFilter { Name = "Comic Archives", Extensions = { "cbz", "cbr", "cb7", "cbt", "zip", "rar" } }
			}
		};
		var topLevel = Avalonia.Application.Current?.ApplicationLifetime as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime;
		var filePaths = await dialog.ShowAsync(topLevel?.MainWindow!);
		if (filePaths is null) return;
		foreach (var path in filePaths)
		{
			await _repository.UpsertByLocationAsync(path, isFolder: false, title: System.IO.Path.GetFileNameWithoutExtension(path));
		}
		Items.Clear();
		await LoadAsync();
	}

	[RelayCommand]
	private async Task RefreshAsync()
	{
		Items.Clear();
		await LoadAsync();
	}

	[RelayCommand]
    private async Task ResetDatabaseAsync()
	{
        await _repository.ResetAsync();
        await _cache.ClearCacheAsync();
        Items.Clear();
        await LoadAsync();
	}

	[RelayCommand]
	private async Task AddFolderAsync()
	{
		var dialog = new Avalonia.Controls.OpenFolderDialog();
		var topLevel = Avalonia.Application.Current?.ApplicationLifetime as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime;
		var folder = await dialog.ShowAsync(topLevel?.MainWindow!);
		if (string.IsNullOrWhiteSpace(folder)) return;
		await _repository.UpsertByLocationAsync(folder, isFolder: true, title: System.IO.Path.GetFileName(folder));
		Items.Clear();
		await LoadAsync();
	}
}


