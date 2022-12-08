using ADOFAI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;
using UnityEngine.Scripting;

public static class RDUtils
{
	private static bool gcEnabled = true;

	public static Vector2Int GetImageDimensions(string path)
	{
		return default(Vector2Int);
	}

	public static AudioMixerGroup GetMixerGroup(string groupPath)
	{
		AudioMixerGroup[] array = AudioManager.Instance.Mixer.FindMatchingGroups(groupPath);
		if (array != null && array.Length != 0)
		{
			return array[0];
		}
		return null;
	}

	public static void SetMixerParameter(string exposedParameter, float value)
	{
		if (!AudioManager.Instance.Mixer.SetFloat(exposedParameter, value))
		{
			UnityEngine.Debug.LogWarning("Audio Parameter not found: $" + exposedParameter + "!");
		}
	}

	public static void SetMixerVolume(string exposedParameterVolume, float value)
	{
		float value2 = (value > 1f) ? ((value - 1f) * 10f) : ((!(value > 0f)) ? (-80f) : ((value - 1f) * 20f));
		SetMixerParameter(exposedParameterVolume, value2);
	}

	public static float GetMixerParameter(string exposedParameterName)
	{
		AudioManager.Instance.Mixer.GetFloat(exposedParameterName, out float value);
		return value;
	}

	public static float PitchPercentToSemitones(float percent)
	{
		return (float)Math.Log(percent, 2.0) * 12f;
	}

	public static float PitchSemitonestoPercent(float semitones)
	{
		return (float)Math.Pow(2.0, (double)semitones / 12.0);
	}

	public static string GetAvailableDirectoryName(string directoryPath)
	{
		int num = -1;
		string text = "";
		do
		{
			string str = (num >= 0) ? num.ToString() : "";
			text = directoryPath + str;
			num++;
		}
		while (RDDirectory.Exists(text) || RDFile.Exists(text));
		return text;
	}

	public static bool IsHex(string s)
	{
		foreach (char c in s)
		{
			if ((c < '0' || c > '9') && (c < 'a' || c > 'f') && (c < 'A' || c > 'F'))
			{
				return false;
			}
		}
		return true;
	}

	public static Color HexToColor(this string hex)
	{
		TryHexToColor(hex, out Color color);
		return color;
	}

	public static bool TryHexToColor(string hex, out Color color)
	{
		if (hex != null)
		{
			bool flag = hex.Length < 8;
			if (hex.Length >= 6)
			{
				byte result = byte.MaxValue;
				if (byte.TryParse(hex.Substring(0, 2), NumberStyles.HexNumber, null, out byte result2) && byte.TryParse(hex.Substring(2, 2), NumberStyles.HexNumber, null, out byte result3) && byte.TryParse(hex.Substring(4, 2), NumberStyles.HexNumber, null, out byte result4) && (flag || byte.TryParse(hex.Substring(6, 2), NumberStyles.HexNumber, null, out result)))
				{
					color = new Color32(result2, result3, result4, result);
					return true;
				}
			}
		}
		color = Color.black;
		return false;
	}

	public static string ToHex(this Color c)
	{
		return $"#{ToByte(c.r):X2}{ToByte(c.g):X2}{ToByte(c.b):X2}";
	}

	private static byte ToByte(float f)
	{
		f = Mathf.Clamp01(f);
		return (byte)(f * 255f);
	}

	public static Vector2 StringToVector2(string sVector)
	{
		if (sVector.StartsWith("(") && sVector.EndsWith(")"))
		{
			sVector = sVector.Substring(1, sVector.Length - 2);
		}
		string[] array = sVector.Split(',');
		return new Vector2(float.Parse(array[0]), float.Parse(array[1]));
	}

	public static float GetRandomFloat(LevelEvent evnt, string key)
	{
		return evnt.GetFloat(key);
	}

	public static int GetRandomInt(LevelEvent evnt, string key)
	{
		return evnt.GetInt(key);
	}

	public static Vector2 GetRandomVector2(LevelEvent evnt, string key)
	{
		return (Vector2)evnt.data[key];
	}

	public static T ParseEnum<T>(string str, T defaultValue = default(T)) where T : struct
	{
		if (Enum.TryParse(str, ignoreCase: true, out T result))
		{
			return result;
		}
		UnityEngine.Debug.LogWarning("ParseEnum(): Returned default value " + defaultValue.ToString() + " because couldn't find string value " + str);
		return defaultValue;
	}

	public static T ToEnum<T>(this string str, T defaultValue = default(T), bool showWarning = true)
	{
		try
		{
			return (T)Enum.Parse(typeof(T), str, ignoreCase: true);
		}
		catch (Exception)
		{
			if (showWarning)
			{
				UnityEngine.Debug.LogWarning("ParseEnum(): Returned default value " + defaultValue.ToString() + " because couldn't find string value " + str);
			}
			return defaultValue;
		}
	}

	public static Array GetValues<T>()
	{
		return Enum.GetValues(typeof(T));
	}

	public static GameObject SpawnIfNotFound(string GOname, GameObject prefab = null)
	{
		GameObject gameObject = null;
		GameObject gameObject2 = GameObject.Find(GOname);
		if (gameObject2 == null)
		{
			if (prefab != null)
			{
				gameObject = UnityEngine.Object.Instantiate(prefab);
				gameObject.name = GOname;
			}
			else
			{
				gameObject = new GameObject(GOname);
			}
			return gameObject;
		}
		return gameObject2;
	}

	public static float OutQuad(float t, float d)
	{
		return 1f - Mathf.Pow(1f - t / d, 2f);
	}

	public static void ShowDebugLabel(string s)
	{
		GUI.Label(new Rect(0f, 0f, 400f, 400f), s);
	}

	public static int Clamp(int value, int min, int max)
	{
		return Math.Min(Math.Max(value, min), max);
	}

	public static int Clamp01(int value)
	{
		return Clamp(value, 0, 1);
	}

	public static Color CycleColor(Color currentcolour)
	{
		uint[] array = new uint[11]
		{
			4291189578u,
			4291198794u,
			4283090599u,
			4283081414u,
			4283070918u,
			4283058886u,
			4285745862u,
			4288039622u,
			4290136774u,
			4291185316u,
			4291185279u
		};
		Color color = currentcolour;
		while (currentcolour == color)
		{
			int num = Mathf.FloorToInt(UnityEngine.Random.value * (float)array.Length);
			color = array[num].ARGBToColor();
		}
		return color;
	}

	public static void Rotate2D(Transform trans, float rotation, bool relative = false)
	{
		if (!relative)
		{
			trans.localEulerAngles = new Vector3(trans.localEulerAngles.x, trans.localEulerAngles.y, rotation);
		}
		else
		{
			trans.localEulerAngles = new Vector3(trans.localEulerAngles.x, trans.localEulerAngles.y, trans.localEulerAngles.z + rotation);
		}
	}

	public static float CeilToNearestMultiple(float n, float multiple)
	{
		float num = Mathf.Round(n / multiple) * multiple;
		if (WithinSmallMargin(num, n))
		{
			return num;
		}
		return Mathf.Ceil(n / multiple) * multiple;
	}

	public static bool WithinSmallMargin(float a, float b)
	{
		return (double)Mathf.Abs(a - b) < 0.001;
	}

	public static T Create<T>(string name, bool makeContainer = false, int suffixNumber = -1, string customContainer = null) where T : Component
	{
		GameObject gameObject = new GameObject();
		GameObject gameObject2 = gameObject.Instantiate(name, makeContainer, customContainer, suffixNumber);
		UnityEngine.Object.Destroy(gameObject);
		return gameObject2.AddComponent<T>();
	}

	private static bool CheckForProcess(string processName)
	{
		if (Process.GetProcessesByName(processName).Length == 0)
		{
			return false;
		}
		return true;
	}

	public static bool IsDigitsOnly(string str)
	{
		foreach (char c in str)
		{
			if (c < '0' || c > '9')
			{
				return false;
			}
		}
		return true;
	}

	public static T Pop<T>(this List<T> list)
	{
		T result = list[list.Count - 1];
		list.RemoveAt(list.Count - 1);
		return result;
	}

	public static float GetHue(Color color)
	{
		int num = Mathf.RoundToInt(color.r * 255f);
		int num2 = Mathf.RoundToInt(color.g * 255f);
		int num3 = Mathf.RoundToInt(color.b * 255f);
		float num4 = Mathf.Min(Mathf.Min(num, num2), num3);
		float num5 = Mathf.Max(Mathf.Max(num, num2), num3);
		if (num4 == num5)
		{
			return 0f;
		}
		float num6 = 0f;
		num6 = ((num5 == (float)num) ? ((float)(num2 - num3) / (num5 - num4)) : ((num5 != (float)num2) ? (4f + (float)(num - num2) / (num5 - num4)) : (2f + (float)(num3 - num) / (num5 - num4))));
		num6 *= 60f;
		if (num6 < 0f)
		{
			num6 += 360f;
		}
		return num6;
	}

	public static Color GetColor(this PlanetColor planetColor)
	{
		return RDConstants.data.GetPlanetColor(planetColor);
	}

	public static void Add<T>(this List<T> list, params T[] items)
	{
		for (int i = 0; i < items.Length; i++)
		{
			list.Add(items[i]);
		}
	}

	public static string BreakRichText(string text)
	{
		if (text.Length <= 1)
		{
			return text;
		}
		int num = 0;
		char[] array = text.ToCharArray();
		for (int i = 1; i < text.Length; i++)
		{
			if ((array[i] == '<' || array[i] == '>') && array[i - 1] != '\\')
			{
				text.Insert(i + num, "\\");
			}
		}
		return text;
	}

	public static string RemoveRichTags(string text)
	{
		int num = 0;
		char[] array = text.ToCharArray();
		string text2 = text;
		bool flag = false;
		int num2 = 0;
		for (int i = 0; i < text.Length; i++)
		{
			if (array[i] == '<')
			{
				flag = true;
				num2 = i;
			}
			else if (array[i] == '>' && flag)
			{
				int num3 = i - num2 + 1;
				text2 = text2.Remove(num2 - num, num3);
				num += num3;
				flag = false;
			}
		}
		return text2;
	}

	public static bool IsXtra(this string s)
	{
		if (!s.StartsWith("X") && !(s == "PA"))
		{
			return s == "RJ";
		}
		return true;
	}

	public static bool IsCrownWorld(this string s)
	{
		return Array.IndexOf(GCNS.crownWorlds, s) != -1;
	}

	public static bool IsMuseDashWorld(this string s)
	{
		return s.StartsWith("M");
	}

	public static bool Approximately(this Vector2 v, Vector2 v2)
	{
		if (Mathf.Approximately(v.x, v2.x))
		{
			return Mathf.Approximately(v.y, v2.y);
		}
		return false;
	}

	public static bool ApproximatelyXY(this Vector3 v, Vector3 v2)
	{
		if (Mathf.Approximately(v.x, v2.x))
		{
			return Mathf.Approximately(v.y, v2.y);
		}
		return false;
	}

	public static void SetGarbageCollectionEnabled(bool enabled)
	{
		if (RDC.debug)
		{
			UnityEngine.Debug.Log("GC enabled: " + enabled.ToString());
		}
		if (enabled && gcEnabled && RDC.debug)
		{
			UnityEngine.Debug.Log("Attempted to enable GC when it was already enabled.");
		}
		else if (!enabled && !gcEnabled)
		{
			UnityEngine.Debug.LogError("Attempted to disable GC when it was already disabled. Please fix this or the game will run out of memory!");
		}
		gcEnabled = enabled;
		GarbageCollector.GCMode = (enabled ? GarbageCollector.Mode.Enabled : GarbageCollector.Mode.Disabled);
		GCSettings.LatencyMode = (enabled ? GCLatencyMode.Interactive : GCLatencyMode.SustainedLowLatency);
		if (enabled)
		{
			GC.Collect();
			Resources.UnloadUnusedAssets();
		}
	}

	public static bool GetGarbageCollectionEnabled()
	{
		return gcEnabled;
	}

	public static string TrimAllSpaces(this string value)
	{
		return value.Replace(" ", string.Empty);
	}

	public static bool IsTaro(this string levelName)
	{
		if (levelName.Length > 0)
		{
			return levelName[0] == 'T';
		}
		return false;
	}

	public static bool IsBossLevel(this string levelName)
	{
		return levelName.EndsWith("-X");
	}

	public static (int year, int month, int day) GetDateOfChineseNewYear()
	{
		ChineseLunisolarCalendar chineseLunisolarCalendar = new ChineseLunisolarCalendar();
		GregorianCalendar gregorianCalendar = new GregorianCalendar();
		DateTime time = chineseLunisolarCalendar.ToDateTime(DateTime.UtcNow.Year, 1, 1, 0, 0, 0, 0);
		int year = gregorianCalendar.GetYear(time);
		int month = gregorianCalendar.GetMonth(time);
		int dayOfMonth = gregorianCalendar.GetDayOfMonth(time);
		return (year, month, dayOfMonth);
	}

	public static bool IsSubDirectoryOf(this string candidate, string other)
	{
		bool result = false;
		try
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(candidate);
			DirectoryInfo directoryInfo2 = new DirectoryInfo(other);
			while (directoryInfo.Parent != null)
			{
				if (directoryInfo.Parent.FullName.ToLower() == directoryInfo2.FullName.ToLower())
				{
					result = true;
					return result;
				}
				directoryInfo = directoryInfo.Parent;
			}
			return result;
		}
		catch (Exception arg)
		{
			UnityEngine.Debug.Log($"Unable to check directories {candidate} and {other}: {arg}");
			return result;
		}
	}

	public static bool HasConnectionError(this UnityWebRequest webRequest)
	{
		UnityWebRequest.Result result = webRequest.result;
		if (result != UnityWebRequest.Result.ConnectionError)
		{
			return result == UnityWebRequest.Result.ProtocolError;
		}
		return true;
	}
}
