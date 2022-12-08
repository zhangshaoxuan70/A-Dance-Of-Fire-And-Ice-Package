using ADOFAI;
using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class RDFile
{
	public static readonly Encoding SwitchDefaultEncoding = Encoding.UTF8;

	public static readonly Encoding NonSwitchDefaultEncoding = PlayerPrefsJson.DefaultLevelEncoding;

	public static void WriteAllText(string path, string data, Encoding encoding = null)
	{
		if (encoding == null)
		{
			encoding = NonSwitchDefaultEncoding;
		}
		File.WriteAllText(path, data, encoding);
	}

	public static void WriteAllBytes(string path, byte[] bytes)
	{
		File.WriteAllBytes(path, bytes);
	}

	public static string ReadAllText(string path, Encoding encoding = null)
	{
		if (encoding == null)
		{
			encoding = NonSwitchDefaultEncoding;
		}
		return File.ReadAllText(path, encoding);
	}

	public static byte[] ReadAllBytes(string path, out LoadResult status)
	{
		status = LoadResult.Error;
		try
		{
			status = LoadResult.Successful;
			return File.ReadAllBytes(path);
		}
		catch (UnauthorizedAccessException ex)
		{
			status = LoadResult.UnauthorizedAccess;
			UnityEngine.Debug.LogError("RDIO ReadAllBytes: UnauthorizedAccessException - " + ex.Message);
			return new byte[0];
		}
		catch (Exception ex2)
		{
			UnityEngine.Debug.LogError("RDIO ReadAllBytes: Error - " + ex2.Message);
			return new byte[0];
		}
	}

	public static bool Exists(string path)
	{
		return File.Exists(path);
	}

	public static void Copy(string sourceFileName, string destFileName, bool overwrite = false)
	{
		File.Copy(sourceFileName, destFileName, overwrite);
	}

	public static void Delete(string path)
	{
		File.Delete(path);
	}

	public static FileStream Create(string path)
	{
		return File.Create(path);
	}
}
