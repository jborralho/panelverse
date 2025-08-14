using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Channels;
using Panelverse.Core.Library;
using Panelverse.Core.Sorting;
using SharpCompress.Archives.Rar;
using SharpCompress.Readers;

namespace Panelverse.Core.Thumbnails;

public sealed class ThumbnailCacheService : IDisposable
{
	public string CacheRoot { get; }
	private readonly Channel<LibraryItemDto> _queue = Channel.CreateUnbounded<LibraryItemDto>();
	private readonly CancellationTokenSource _cts = new();
	private Task? _workerTask;

	public event Action<string, string>? ThumbnailReady; // (locationPath, thumbnailPath)

	public ThumbnailCacheService(string cacheRoot)
	{
		CacheRoot = cacheRoot;
		Directory.CreateDirectory(CacheRoot);
	}

	public static string GetDefaultCacheRoot()
	{
		var root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
		return Path.Combine(root, "Panelverse", "thumbs");
	}

	public void Start()
	{
		_workerTask = Task.Run(() => WorkerAsync(_cts.Token));
	}

	public void QueueEnsureThumbnail(LibraryItemDto item)
	{
		_queue.Writer.TryWrite(item);
	}

	public string? GetExistingThumbnailPath(string locationPath)
	{
		var baseName = ComputeBaseName(locationPath);
		if (!Directory.Exists(CacheRoot)) return null;
		var files = Directory.GetFiles(CacheRoot, baseName + ".*");
		return files.FirstOrDefault();
	}

	private async Task WorkerAsync(CancellationToken ct)
	{
		while (!ct.IsCancellationRequested)
		{
			LibraryItemDto item;
			try
			{
				item = await _queue.Reader.ReadAsync(ct);
			}
			catch (OperationCanceledException)
			{
				break;
			}

			try
			{
				await CreateThumbnailIfMissingAsync(item, ct);
			}
			catch
			{
				// ignore individual failures; could log
			}
		}
	}

	private async Task CreateThumbnailIfMissingAsync(LibraryItemDto item, CancellationToken ct)
	{
		Directory.CreateDirectory(CacheRoot);
		if (GetExistingThumbnailPath(item.LocationPath) is not null) return;

		string? ext = null;
		Stream? sourceStream = null;
		string? tmpFile = null;
		try
		{
			if (item.IsFolder)
			{
				ext = TryGetFirstImageInFolder(item.LocationPath, out var imagePath);
				if (ext != null && imagePath != null)
				{
					var dest = BuildPath(item.LocationPath, ext);
					File.Copy(imagePath, dest, overwrite: true);
					ThumbnailReady?.Invoke(item.LocationPath, dest);
				}
				return;
			}

            var fileExt = Path.GetExtension(item.LocationPath).ToLowerInvariant();
            if (fileExt is ".cbz" or ".zip")
			{
				ext = await TryExtractFirstImageFromZipAsync(item.LocationPath, ct);
				if (ext is not null)
				{
					var dest = BuildPath(item.LocationPath, ext);
					if (File.Exists(dest)) return;
					// already extracted within TryExtractFirstImageFromZipAsync into dest name, so just signal
					ThumbnailReady?.Invoke(item.LocationPath, dest);
				}
			}
            else if (fileExt is ".cbr" or ".rar")
            {
                ext = await TryExtractFirstImageFromRarAsync(item.LocationPath, ct);
                if (ext is not null)
                {
                    var dest = BuildPath(item.LocationPath, ext);
                    if (File.Exists(dest))
                    {
                        ThumbnailReady?.Invoke(item.LocationPath, dest);
                    }
                }
            }
            // NOTE: 7z support to be added later
		}
		finally
		{
			sourceStream?.Dispose();
			if (tmpFile != null && File.Exists(tmpFile))
			{
				try { File.Delete(tmpFile); } catch { /* ignore */ }
			}
		}
	}

	private string BuildPath(string locationPath, string ext)
	{
		var baseName = ComputeBaseName(locationPath);
		return Path.Combine(CacheRoot, baseName + ext);
	}

	private static string ComputeBaseName(string locationPath)
	{
		using var sha = SHA256.Create();
		var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(locationPath));
		var sb = new StringBuilder(bytes.Length * 2);
		foreach (var b in bytes) sb.Append(b.ToString("x2"));
		return sb.ToString();
	}

	private static readonly string[] ImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".bmp" };

	private static string? TryGetFirstImageInFolder(string folder, out string? imagePath)
	{
		imagePath = null;
		if (!Directory.Exists(folder)) return null;
		var files = Directory.EnumerateFiles(folder)
			.Where(f => ImageExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
			.OrderBy(f => f, Comparer<string>.Create(NaturalSort.Compare))
			.ToList();
		if (files.Count == 0) return null;
		imagePath = files[0];
		return Path.GetExtension(imagePath).ToLowerInvariant();
	}

	private async Task<string?> TryExtractFirstImageFromZipAsync(string archivePath, CancellationToken ct)
	{
		await using var fs = new FileStream(archivePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		using var zip = new ZipArchive(fs, ZipArchiveMode.Read, leaveOpen: true);
		var imageEntries = zip.Entries
			.Where(e => !string.IsNullOrEmpty(e.Name))
			.Where(e => ImageExtensions.Contains(Path.GetExtension(e.Name).ToLowerInvariant()))
			.OrderBy(e => e.FullName, Comparer<string>.Create(NaturalSort.Compare))
			.ToList();
		if (imageEntries.Count == 0) return null;
		var entry = imageEntries[0];
		var ext = Path.GetExtension(entry.Name).ToLowerInvariant();
		var dest = BuildPath(archivePath, ext);
		Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
		await using var es = entry.Open();
		await using var outFs = new FileStream(dest, FileMode.Create, FileAccess.Write, FileShare.None);
		await es.CopyToAsync(outFs, ct);
		return ext;
	}

    private async Task<string?> TryExtractFirstImageFromRarAsync(string archivePath, CancellationToken ct)
    {
        using var archive = RarArchive.Open(archivePath, new ReaderOptions { LeaveStreamOpen = false });
        var imageEntries = archive.Entries
            .Where(e => !e.IsDirectory)
            .Where(e => ImageExtensions.Contains(Path.GetExtension(e.Key).ToLowerInvariant()))
            .OrderBy(e => e.Key, Comparer<string?>.Create(NaturalSort.Compare))
            .ToList();
        if (imageEntries.Count == 0) return null;
        var entry = imageEntries[0];
        var ext = Path.GetExtension(entry.Key).ToLowerInvariant();
        var dest = BuildPath(archivePath, ext);
        Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
        await using var es = entry.OpenEntryStream();
        await using var outFs = new FileStream(dest, FileMode.Create, FileAccess.Write, FileShare.None);
        await es.CopyToAsync(outFs, ct);
        return ext;
    }

	public void Dispose()
	{
		_cts.Cancel();
		try { _workerTask?.Wait(1000); } catch { /* ignore */ }
		_cts.Dispose();
	}
}


