using ADOFAI;
using RDTools;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class scrExtImgHolder
{
	public class CustomTexture
	{
		public Texture2D texture;

		public DateTime fileLastModified;

		public bool inUse;

		public CustomTexture(Texture2D texture, DateTime fileLastModified)
		{
			this.texture = texture;
			this.fileLastModified = fileLastModified;
		}
	}

	public class CustomSprite
	{
		public Sprite sprite;

		public DateTime fileLastModified;

		public bool inUse;

		public Texture2D texture => sprite.texture;

		public CustomSprite(Sprite sprite, DateTime fileLastModified)
		{
			this.sprite = sprite;
			this.fileLastModified = fileLastModified;
		}
	}

	public Dictionary<string, CustomTexture> customTextures = new Dictionary<string, CustomTexture>();

	public Dictionary<string, CustomSprite> customSprites = new Dictionary<string, CustomSprite>();

	public static Sprite LoadNewSprite(string FilePath, out LoadResult status, float PixelsPerUnit = 100f, SpriteMeshType spriteType = SpriteMeshType.FullRect)
	{
		Texture2D texture2D = LoadTexture(FilePath, out status);
		if (texture2D != null)
		{
			return Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), PixelsPerUnit, 0u, spriteType);
		}
		return null;
	}

	public static Texture2D LoadTexture(string FilePath, out LoadResult status, int maxSideSize = -1)
	{
		status = LoadResult.MissingFile;
		if (RDFile.Exists(FilePath))
		{
			byte[] data = RDFile.ReadAllBytes(FilePath, out status);
			Texture2D texture2D = new Texture2D(2, 2, TextureFormat.RGBA32, mipChain: false);
			if (texture2D.LoadImage(data))
			{
				if (maxSideSize != -1)
				{
					ShrinkImage(texture2D, maxSideSize);
				}
				texture2D.name = FilePath;
				texture2D.wrapMode = TextureWrapMode.Repeat;
				return texture2D;
			}
		}
		return null;
	}

	public static void ShrinkImage(Texture2D tex2D, int maxSideSize)
	{
		if (tex2D.width > maxSideSize || tex2D.height > maxSideSize)
		{
			int num = Mathf.Max(tex2D.width, tex2D.height);
			float num2 = (float)maxSideSize * 1f / (float)num;
			new TextureScale().Bilinear(tex2D, Mathf.RoundToInt((float)tex2D.width * num2), Mathf.RoundToInt((float)tex2D.height * num2));
		}
	}

	public Texture2D AddTexture(string key, out LoadResult status, string filePath = null, int maxSideSize = -1)
	{
		status = LoadResult.Error;
		if (key.IsNullOrEmpty())
		{
			UnityEngine.Debug.LogError("File no exist 3");
			return null;
		}
		bool flag = true;
		DateTime lastWriteTimeUtc = new FileInfo(filePath).LastWriteTimeUtc;
		if (customTextures.ContainsKey(key) && DateTime.Compare(customTextures[key].fileLastModified, lastWriteTimeUtc) == 0)
		{
			flag = false;
		}
		RDBaseDll.printem($"key {key}, reload: {flag}");
		if (flag)
		{
			if (customTextures.ContainsKey(key))
			{
				UnloadTexture(key);
			}
			if (filePath.IsNullOrEmpty())
			{
				return null;
			}
			Texture2D texture = LoadTexture(filePath, out status, maxSideSize);
			customTextures.Add(key, new CustomTexture(texture, lastWriteTimeUtc));
		}
		else
		{
			status = LoadResult.Successful;
		}
		customTextures[key].inUse = true;
		return customTextures[key].texture;
	}

	public Sprite AddSprite(string key, string filePath, out LoadResult status)
	{
		status = LoadResult.Error;
		if (key.IsNullOrEmpty())
		{
			return null;
		}
		bool flag = true;
		DateTime lastWriteTimeUtc = new FileInfo(filePath).LastWriteTimeUtc;
		if (customSprites.ContainsKey(key) && DateTime.Compare(customSprites[key].fileLastModified, lastWriteTimeUtc) == 0)
		{
			flag = false;
		}
		status = LoadResult.Successful;
		if (flag)
		{
			if (customSprites.ContainsKey(key))
			{
				UnloadSprite(key);
			}
			if (filePath.IsNullOrEmpty())
			{
				return null;
			}
			Sprite sprite = LoadNewSprite(filePath, out status);
			customSprites.Add(key, new CustomSprite(sprite, lastWriteTimeUtc));
		}
		customSprites[key].inUse = true;
		return customSprites[key].sprite;
	}

	public override string ToString()
	{
		string text = "Textures:";
		foreach (KeyValuePair<string, CustomTexture> customTexture in customTextures)
		{
			text = text + "\n" + customTexture.Key;
		}
		return text;
	}

	public void Unload(bool onlyIfUnused)
	{
		UnloadAllTextures(onlyIfUnused);
		UnloadAllSprites(onlyIfUnused);
	}

	public void UnloadAllTextures(bool onlyIfUnused = false)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, CustomTexture> customTexture in customTextures)
		{
			if (!onlyIfUnused || !customTexture.Value.inUse)
			{
				list.Add(customTexture.Key);
			}
		}
		foreach (string item in list)
		{
			UnloadTexture(item);
		}
	}

	public void UnloadTexture(string key)
	{
		UnityEngine.Object.Destroy(customTextures[key].texture);
		customTextures.Remove(key);
		RDBaseDll.printem("Unloaded texture " + key);
	}

	public void UnloadAllSprites(bool onlyIfUnused = false)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, CustomSprite> customSprite in customSprites)
		{
			if (!onlyIfUnused || !customSprite.Value.inUse)
			{
				list.Add(customSprite.Key);
			}
		}
		foreach (string item in list)
		{
			UnloadSprite(item);
		}
	}

	public void UnloadSprite(string key)
	{
		UnityEngine.Object.Destroy(customSprites[key].sprite);
		customSprites.Remove(key);
	}

	public void MarkAllUnused()
	{
		foreach (CustomSprite value in customSprites.Values)
		{
			value.inUse = false;
		}
		foreach (CustomTexture value2 in customTextures.Values)
		{
			value2.inUse = false;
		}
	}

	~scrExtImgHolder()
	{
		Unload(onlyIfUnused: false);
	}
}
