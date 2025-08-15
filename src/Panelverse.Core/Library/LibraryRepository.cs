using Microsoft.Data.Sqlite;

namespace Panelverse.Core.Library;

public sealed class LibraryRepository
{
	private readonly string _connectionString;

	public LibraryRepository(string databasePath)
	{
		var builder = new SqliteConnectionStringBuilder
		{
			DataSource = databasePath,
			Mode = SqliteOpenMode.ReadWriteCreate,
			Pooling = false
		};
		_connectionString = builder.ToString();
	}

    public string DatabasePath => new SqliteConnectionStringBuilder(_connectionString).DataSource;

	public async Task InitializeAsync(CancellationToken cancellationToken = default)
	{
		await using var conn = new SqliteConnection(_connectionString);
		await conn.OpenAsync(cancellationToken);
		await using var cmd = conn.CreateCommand();
		cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS books (
  id INTEGER PRIMARY KEY,
  title TEXT NOT NULL,
  series TEXT NULL,
  volume INTEGER NULL,
  pages_total INTEGER NOT NULL,
  pages_read INTEGER NOT NULL DEFAULT 0,
  location_path TEXT NOT NULL,
  is_folder INTEGER NOT NULL DEFAULT 0,
  thumbnail_path TEXT NULL,
  added_at TEXT NOT NULL,
  last_opened_at TEXT NULL
);
CREATE UNIQUE INDEX IF NOT EXISTS idx_books_location ON books(location_path);
";
		await cmd.ExecuteNonQueryAsync(cancellationToken);
	}

	public async Task UpdatePagesTotalAsync(long id, int pagesTotal, CancellationToken cancellationToken = default)
	{
		await using var conn = new SqliteConnection(_connectionString);
		await conn.OpenAsync(cancellationToken);
		await using var cmd = conn.CreateCommand();
		cmd.CommandText = "UPDATE books SET pages_total=$p WHERE id=$id;";
		cmd.Parameters.AddWithValue("$p", pagesTotal);
		cmd.Parameters.AddWithValue("$id", id);
		await cmd.ExecuteNonQueryAsync(cancellationToken);
	}

	public async Task UpdatePagesReadAsync(long id, int pagesRead, CancellationToken cancellationToken = default)
	{
		await using var conn = new SqliteConnection(_connectionString);
		await conn.OpenAsync(cancellationToken);
		await using var cmd = conn.CreateCommand();
		cmd.CommandText = "UPDATE books SET pages_read=$r, last_opened_at=$t WHERE id=$id;";
		cmd.Parameters.AddWithValue("$r", pagesRead);
		cmd.Parameters.AddWithValue("$t", DateTimeOffset.UtcNow.ToString("O"));
		cmd.Parameters.AddWithValue("$id", id);
		await cmd.ExecuteNonQueryAsync(cancellationToken);
	}

	public async Task<LibraryItemDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
	{
		await using var conn = new SqliteConnection(_connectionString);
		await conn.OpenAsync(cancellationToken);
		await using var cmd = conn.CreateCommand();
		cmd.CommandText = "SELECT id, title, series, volume, pages_total, pages_read, location_path, is_folder, thumbnail_path, added_at, last_opened_at FROM books WHERE id=$id LIMIT 1;";
		cmd.Parameters.AddWithValue("$id", id);
		await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
		if (await reader.ReadAsync(cancellationToken))
		{
			return new LibraryItemDto(
				Id: reader.GetInt64(0),
				Title: reader.GetString(1),
				Series: reader.IsDBNull(2) ? null : reader.GetString(2),
				Volume: reader.IsDBNull(3) ? null : reader.GetInt32(3),
				PagesTotal: reader.GetInt32(4),
				PagesRead: reader.GetInt32(5),
				LocationPath: reader.GetString(6),
				IsFolder: reader.GetInt32(7) != 0,
				ThumbnailPath: reader.IsDBNull(8) ? null : reader.GetString(8),
				AddedAt: DateTimeOffset.Parse(reader.GetString(9)),
				LastOpenedAt: reader.IsDBNull(10) ? null : DateTimeOffset.Parse(reader.GetString(10))
			);
		}
		return null;
	}

	public async Task ResetAsync(CancellationToken cancellationToken = default)
	{
		var path = DatabasePath;
		try
		{
			SqliteConnection.ClearAllPools();
			const int maxAttempts = 5;
			for (int attempt = 1; attempt <= maxAttempts; attempt++)
			{
				try
				{
					if (File.Exists(path))
					{
						File.Delete(path);
					}
					break;
				}
				catch when (attempt < maxAttempts)
				{
					await Task.Delay(100, cancellationToken);
				}
			}
		}
		catch
		{
			// ignore; caller can retry
		}
		await InitializeAsync(cancellationToken);
	}

	public async Task<long> UpsertByLocationAsync(string path, bool isFolder, string? title = null, int pagesTotal = 0, CancellationToken cancellationToken = default)
	{
		await using var conn = new SqliteConnection(_connectionString);
		await conn.OpenAsync(cancellationToken);
		await using (var insert = conn.CreateCommand())
		{
			insert.CommandText = @"
INSERT INTO books (title, series, volume, pages_total, pages_read, location_path, is_folder, thumbnail_path, added_at, last_opened_at)
VALUES ($title, NULL, NULL, $pages_total, 0, $location_path, $is_folder, NULL, $added_at, NULL)
ON CONFLICT(location_path) DO UPDATE SET title=excluded.title;";
			insert.Parameters.AddWithValue("$title", title ?? System.IO.Path.GetFileNameWithoutExtension(path));
			insert.Parameters.AddWithValue("$pages_total", pagesTotal);
			insert.Parameters.AddWithValue("$location_path", path);
			insert.Parameters.AddWithValue("$is_folder", isFolder ? 1 : 0);
			insert.Parameters.AddWithValue("$added_at", DateTimeOffset.UtcNow.ToString("O"));
			await insert.ExecuteNonQueryAsync(cancellationToken);
		}

		await using (var select = conn.CreateCommand())
		{
			select.CommandText = "SELECT id FROM books WHERE location_path=$p LIMIT 1;";
			select.Parameters.AddWithValue("$p", path);
			var result = await select.ExecuteScalarAsync(cancellationToken);
			return result is long l ? l : Convert.ToInt64(result);
		}
	}

	public async Task<LibraryItemDto?> GetByLocationAsync(string path, CancellationToken cancellationToken = default)
	{
		await using var conn = new SqliteConnection(_connectionString);
		await conn.OpenAsync(cancellationToken);
		await using var cmd = conn.CreateCommand();
		cmd.CommandText = "SELECT id, title, series, volume, pages_total, pages_read, location_path, is_folder, thumbnail_path, added_at, last_opened_at FROM books WHERE location_path=$p LIMIT 1;";
		cmd.Parameters.AddWithValue("$p", path);
		await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
		if (await reader.ReadAsync(cancellationToken))
		{
			return new LibraryItemDto(
				Id: reader.GetInt64(0),
				Title: reader.GetString(1),
				Series: reader.IsDBNull(2) ? null : reader.GetString(2),
				Volume: reader.IsDBNull(3) ? null : reader.GetInt32(3),
				PagesTotal: reader.GetInt32(4),
				PagesRead: reader.GetInt32(5),
				LocationPath: reader.GetString(6),
				IsFolder: reader.GetInt32(7) != 0,
				ThumbnailPath: reader.IsDBNull(8) ? null : reader.GetString(8),
				AddedAt: DateTimeOffset.Parse(reader.GetString(9)),
				LastOpenedAt: reader.IsDBNull(10) ? null : DateTimeOffset.Parse(reader.GetString(10))
			);
		}
		return null;
	}

	public async IAsyncEnumerable<LibraryItemDto> GetItemsAsync([System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		await using var conn = new SqliteConnection(_connectionString);
		await conn.OpenAsync(cancellationToken);
		await using var cmd = conn.CreateCommand();
		cmd.CommandText = "SELECT id, title, series, volume, pages_total, pages_read, location_path, is_folder, thumbnail_path, added_at, last_opened_at FROM books ORDER BY title;";
		await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
		while (await reader.ReadAsync(cancellationToken))
		{
			yield return new LibraryItemDto(
				Id: reader.GetInt64(0),
				Title: reader.GetString(1),
				Series: reader.IsDBNull(2) ? null : reader.GetString(2),
				Volume: reader.IsDBNull(3) ? null : reader.GetInt32(3),
				PagesTotal: reader.GetInt32(4),
				PagesRead: reader.GetInt32(5),
				LocationPath: reader.GetString(6),
				IsFolder: reader.GetInt32(7) != 0,
				ThumbnailPath: reader.IsDBNull(8) ? null : reader.GetString(8),
				AddedAt: DateTimeOffset.Parse(reader.GetString(9)),
				LastOpenedAt: reader.IsDBNull(10) ? null : DateTimeOffset.Parse(reader.GetString(10))
			);
		}
	}
}


