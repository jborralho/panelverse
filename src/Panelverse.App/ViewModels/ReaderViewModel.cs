using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharpCompress.Archives; // added
using SharpCompress.Archives.Rar;
using SharpCompress.Common;
using Panelverse.App.Services;

namespace Panelverse.App.ViewModels;

public partial class ReaderViewModel : ObservableObject, System.IDisposable
{
	public long Id { get; }
	public string Title { get; }
	public string? LocationPath { get; }
	public string? ExtractedFolder { get; private set; }
	public bool IsBusy { get; private set; }

	private readonly CancellationTokenSource _cts = new();
	private Task? _workTask;
	private bool _shouldDeleteOnDispose;

	public ReaderViewModel(long id, string title, string? locationPath)
	{
		Id = id;
		Title = title;
		LocationPath = locationPath;
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
					entry.WriteToDirectory(ExtractedFolder, new ExtractionOptions
					{
						ExtractFullPath = true,
						Overwrite = true
					});
				}
			}
			// Other formats (7z/tar) not implemented in baseline
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


