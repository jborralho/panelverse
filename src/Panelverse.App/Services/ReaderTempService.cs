using System.IO;
using System.Threading.Tasks;

namespace Panelverse.App.Services;

public static class ReaderTempService
{
	public static string GetExtractRoot()
	{
		var appData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
		return Path.Combine(appData, "Panelverse", "reader");
	}

	public static void ClearAll()
	{
		var root = GetExtractRoot();
		if (!Directory.Exists(root)) return;
		try
		{
			foreach (var dir in Directory.EnumerateDirectories(root))
			{
				try { Directory.Delete(dir, recursive: true); } catch { }
			}
			foreach (var file in Directory.EnumerateFiles(root))
			{
				try { File.Delete(file); } catch { }
			}
		}
		catch { }
	}

	public static Task ClearAllAsync()
	{
		return Task.Run(ClearAll);
	}
}


