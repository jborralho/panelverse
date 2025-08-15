using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharpCompress.Archives; // added
using SharpCompress.Archives.Rar;
using SharpCompress.Common;
using Panelverse.App.Services;
using Panelverse.Core.Sorting;
using Avalonia.Media.Imaging;
using Panelverse.Core.Library;

namespace Panelverse.App.ViewModels;

public partial class ReaderViewModel : ObservableObject, System.IDisposable
{
	public long Id { get; }
	public string Title { get; }
	public string? LocationPath { get; }
	public string? ExtractedFolder { get; private set; }
	public bool IsBusy { get; private set; }
    public int CurrentIndex { get; private set; }
    public Bitmap? CurrentImage { get; private set; }
    public int TotalPages { get; private set; }

	private readonly CancellationTokenSource _cts = new();
	private Task? _workTask;
	private bool _shouldDeleteOnDispose;
    private readonly LibraryRepository _repository;

	public ReaderViewModel(long id, string title, string? locationPath, int startIndex = 0)
	{
		Id = id;
		Title = title;
		LocationPath = locationPath;
		CurrentIndex = Math.Max(0, startIndex);
		_repository = new LibraryRepository(System.IO.Path.Combine(System.AppContext.BaseDirectory, "panelverse.db"));
		InitializeExtraction();
	}

	private void InitializeExtraction()
	{
		if (string.IsNullOrWhiteSpace(LocationPath)) return;

		try
		{
			if (Directory.Exists(LocationPath))
			{
				ExtractedFolder = LocationPath;
				_shouldDeleteOnDispose = false;
				LoadCurrentImage(initial: true);
				return;
			}

			if (File.Exists(LocationPath))
			{
				ExtractedFolder = BuildExtractFolder(LocationPath);
				Directory.CreateDirectory(ExtractedFolder);
				_shouldDeleteOnDispose = true;
				IsBusy = true;
				OnPropertyChanged(nameof(IsBusy));
				_workTask = Task.Run(() => ExtractAllAsync(_cts.Token));
			}
		}
		catch
		{
			// swallow for baseline
		}
	}

	private async Task ExtractAllAsync(CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(LocationPath) || string.IsNullOrWhiteSpace(ExtractedFolder))
		{
			IsBusy = false;
			OnPropertyChanged(nameof(IsBusy));
			return;
		}

		try
		{
			var ext = Path.GetExtension(LocationPath).ToLowerInvariant();
			if (ext is ".cbz" or ".zip")
			{
				// System.IO.Compression handles ZIP/CBZ
				ZipFile.ExtractToDirectory(LocationPath, ExtractedFolder, overwriteFiles: true);
			}
			else if (ext is ".cbr" or ".rar")
			{
				using var archive = RarArchive.Open(LocationPath);
				foreach (var entry in archive.Entries)
				{
					if (entry.IsDirectory) continue;
					entry.WriteToDirectory(ExtractedFolder, new SharpCompress.Common.ExtractionOptions
					{
						ExtractFullPath = true,
						Overwrite = true
					});
				}
			}
			// Other formats (7z/tar) not implemented in baseline

			// After extraction, show current page
			LoadCurrentImage(initial: true);
		}
		catch
		{
			// ignore errors in baseline
		}
		finally
		{
			IsBusy = false;
			OnPropertyChanged(nameof(IsBusy));
		}
	}

	private void LoadCurrentImage(bool initial)
	{
		if (string.IsNullOrWhiteSpace(ExtractedFolder)) return;
		var images = Directory.EnumerateFiles(ExtractedFolder, "*.*", SearchOption.AllDirectories)
			.Where(p => IsImage(p))
			.OrderBy(p => p, Comparer<string>.Create(NaturalSort.Compare))
			.ToList();
		TotalPages = images.Count;
		OnPropertyChanged(nameof(TotalPages));
		if (images.Count == 0) return;
		if (initial)
		{
			CurrentIndex = Math.Clamp(CurrentIndex, 0, images.Count - 1);
		}
		var targetPath = images[CurrentIndex];
		try
		{
			using var fs = File.OpenRead(targetPath);
			CurrentImage = new Bitmap(fs);
			OnPropertyChanged(nameof(CurrentImage));
		}
		catch { }
	}

	private static bool IsImage(string path)
	{
		var ext = Path.GetExtension(path).ToLowerInvariant();
		return ext is ".jpg" or ".jpeg" or ".png" or ".webp" or ".bmp";
	}

	[ObservableProperty]
	private double _zoom = 1.0;

	[RelayCommand]
	public void NextPage()
	{
		if (string.IsNullOrWhiteSpace(ExtractedFolder)) return;
		LoadCurrentImage(initial: false);
		if (TotalPages <= 0) return;
		if (CurrentIndex < TotalPages - 1)
		{
			CurrentIndex++;
			LoadCurrentImage(initial: false);
			_ = _repository.UpdatePagesReadAsync(Id, CurrentIndex);
		}
	}

	[RelayCommand]
	public void PrevPage()
	{
		if (string.IsNullOrWhiteSpace(ExtractedFolder)) return;
		if (CurrentIndex > 0)
		{
			CurrentIndex--;
			LoadCurrentImage(initial: false);
			_ = _repository.UpdatePagesReadAsync(Id, CurrentIndex);
		}
	}

	private static string BuildExtractFolder(string locationPath)
	{
		var root = GetReaderExtractRoot();
		Directory.CreateDirectory(root);
		using var sha = SHA256.Create();
		var now = System.DateTime.UtcNow.Ticks.ToString();
		var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(locationPath + "|" + now));
		var sb = new StringBuilder(bytes.Length * 2);
		foreach (var b in bytes) sb.Append(b.ToString("x2"));
		return Path.Combine(root, sb.ToString());
	}

	private static string GetReaderExtractRoot() => ReaderTempService.GetExtractRoot();

	public void Dispose()
	{
		try
		{
			_cts.Cancel();
			try { _workTask?.Wait(1000); } catch { /* ignore */ }
		}
		finally
		{
			_cts.Dispose();
		}

		if (_shouldDeleteOnDispose && !string.IsNullOrWhiteSpace(ExtractedFolder))
		{
			try
			{
				if (Directory.Exists(ExtractedFolder))
				{
					Directory.Delete(ExtractedFolder, recursive: true);
				}
			}
			catch { /* ignore */ }
		}
	}
}


