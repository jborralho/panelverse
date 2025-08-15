using CommunityToolkit.Mvvm.ComponentModel;

namespace Panelverse.App.ViewModels;

public partial class ReaderViewModel : ObservableObject
{
	public long Id { get; }
	public string Title { get; }
	public string? LocationPath { get; }

	public ReaderViewModel(long id, string title, string? locationPath)
	{
		Id = id;
		Title = title;
		LocationPath = locationPath;
	}
}


