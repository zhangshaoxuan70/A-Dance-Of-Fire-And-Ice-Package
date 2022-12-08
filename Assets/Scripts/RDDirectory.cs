using System.IO;

public static class RDDirectory
{
	public static bool Exists(string path)
	{
		return Directory.Exists(path);
	}

	public static DirectoryInfo CreateDirectory(string path)
	{
		return Directory.CreateDirectory(path);
	}
}
