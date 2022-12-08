using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public static class RDEditorUtils
{
	private const string Tab = "\t";

	public static string Indentation = "\t";

	private const string DoubleQuote = "\"";

	private static readonly string[] logDirectories = new string[2]
	{
		Application.persistentDataPath,
		Application.dataPath
	};

	private static readonly string[] logFileNames = new string[2]
	{
		"output_log.txt",
		"Player.log"
	};

	private static string lastLogDirectory = null;

	public static int IndentationLevel
	{
		set
		{
			Indentation = "";
			for (int i = 0; i < value; i++)
			{
				Indentation += "\t";
			}
		}
	}

	private static string[] macDirectories => new string[1]
	{
		Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/Library/Logs/7th Beat Games/A Dance of Fire and Ice/"
	};

	public static bool IsValidHexColor(this string s)
	{
		int result;
		if (s.Length == 6 || s.Length == 8)
		{
			return int.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result);
		}
		return false;
	}

	public static bool IsValidHexColor(this string s, bool hasAlpha)
	{
		int num = hasAlpha ? 8 : 6;
		int result;
		if (s.Length == num)
		{
			return int.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result);
		}
		return false;
	}

	public static string OpenArray(string header)
	{
		string text = "";
		return text + Indentation + "\"" + header + "\":" + Environment.NewLine + Indentation + "[" + Environment.NewLine;
	}

	public static string OpenDictionary(string header)
	{
		string text = "";
		return text + Indentation + "\"" + header + "\":" + Environment.NewLine + Indentation + "{" + Environment.NewLine;
	}

	public static string OpenDictionary()
	{
		return "{" + Environment.NewLine;
	}

	public static string CloseDictionary(bool isLastMember = false)
	{
		return Indentation + (isLastMember ? "}" : "},") + Environment.NewLine;
	}

	public static string CloseArray(bool isLastMember = false)
	{
		return Indentation + (isLastMember ? "]" : "],") + Environment.NewLine;
	}

	public static string EscapeTextForJSON(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return "";
		}
		return text.Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\t", "\t")
			.Replace("\"", "\\\"");
	}

	public static int IndexOfStringInEnumArray<T>(T[] array, string s) where T : struct
	{
		T value = RDUtils.ParseEnum<T>(s);
		if (value.Equals(default(T)))
		{
			return -1;
		}
		return Array.IndexOf(array, value);
	}

	public static string EncodeInt(string key, int intValue, bool lastValue = false)
	{
		return Indentation + "\"" + key + "\"" + ": " + intValue + (lastValue ? " " : ", ");
	}

	public static string EncodeBool(string key, bool boolValue, bool lastValue = false)
	{
		return Indentation + "\"" + key + "\"" + ": " + boolValue.ToString().ToLower() + (lastValue ? " " : ", ");
	}

	public static string EncodeFloat(string key, float floatValue, bool lastValue = false)
	{
		return Indentation + "\"" + key + "\"" + ": " + floatValue + (lastValue ? " " : ", ");
	}

	public static string EncodeVector2(string key, Vector2 vecValue, bool lastValue = false)
	{
		return EncodeFloatArray(key, new float[2]
		{
			vecValue.x,
			vecValue.y
		}, lastValue);
	}

	public static string EncodeTile(string key, Tuple<int, TileRelativeTo> tupValue, bool lastValue = false)
	{
		string text = tupValue.Item1.ToString();
		string text2 = tupValue.Item2.ToString();
		return Indentation + "\"" + key + "\"" + ": " + "[" + text + ", " + "\"" + text2 + "\"" + "]" + (lastValue ? " " : ", ");
	}

	public static string EncodeUnicodeString(string key, string stringValue, bool lastValue = false)
	{
		stringValue = EscapeTextForJSON(stringValue);
		return EncodeString(key, stringValue, lastValue);
	}

	public static string EncodeString(string key, string stringValue, bool lastValue = false)
	{
		return Indentation + "\"" + key + "\"" + ": " + "\"" + stringValue + "\"" + (lastValue ? " " : ", ");
	}

	public static string EncodeIntArray(string key, int[] array, bool lastValue = false)
	{
		string text = "";
		for (int i = 0; i < array.Length; i++)
		{
			text += array[i].ToString();
			if (i != array.Length - 1)
			{
				text += ", ";
			}
		}
		return Indentation + "\"" + key + "\": [" + text + "]" + (lastValue ? " " : ", ");
	}

	public static string EncodeModsArray(string key, object[] array, bool lastValue = false)
	{
		return Indentation + "\"" + key + "\": []" + (lastValue ? "  " : ", ");
	}

	public static string EncodeFloatArray(string key, float[] array, bool lastValue = false)
	{
		string text = "";
		for (int i = 0; i < array.Length; i++)
		{
			text += array[i].ToString();
			if (i != array.Length - 1)
			{
				text += ", ";
			}
		}
		return Indentation + "\"" + key + "\": [" + text + "]" + (lastValue ? " " : ", ");
	}

	public static string EncodeEnumArray<T>(string key, T[] array, bool lastValue = false)
	{
		string text = "";
		for (int i = 0; i < array.Length; i++)
		{
			text = text + "\"" + array[i].ToString() + "\"";
			if (i != array.Length - 1)
			{
				text += ", ";
			}
		}
		return Indentation + "\"" + key + "\": [" + text + "]" + (lastValue ? " " : ", ");
	}

	public static bool DecodeBool(object dictValue)
	{
		return (bool)dictValue;
	}

	public static float DecodeFloat(object dictValue)
	{
		return Convert.ToSingle(dictValue);
	}

	public static int DecodeInt(object dictValue)
	{
		return Convert.ToInt32(dictValue);
	}

	public static string DecodeString(object dictValue)
	{
		return dictValue as string;
	}

	public static T DecodeEnum<T>(object dictValue) where T : struct
	{
		return RDUtils.ParseEnum<T>(dictValue as string);
	}

	public static T[] DecodeEnumArray<T>(object dictValue) where T : struct
	{
		List<object> obj = dictValue as List<object>;
		T[] array = new T[obj.Count];
		int num = 0;
		foreach (object item in obj)
		{
			array[num] = RDUtils.ParseEnum<T>(item as string);
			num++;
		}
		return array;
	}

	public static int[] DecodeIntArray(object dictValue)
	{
		List<object> obj = dictValue as List<object>;
		int[] array = new int[obj.Count];
		int num = 0;
		foreach (object item in obj)
		{
			array[num] = Convert.ToInt32(item);
			num++;
		}
		return array;
	}

	public static float[] DecodeFloatArray(object dictValue)
	{
		List<object> obj = dictValue as List<object>;
		float[] array = new float[obj.Count];
		int num = 0;
		foreach (object item in obj)
		{
			array[num] = Convert.ToSingle(item);
			num++;
		}
		return array;
	}

	public static string[] DecodeStringArray(object dictValue)
	{
		List<object> obj = dictValue as List<object>;
		string[] array = new string[obj.Count];
		int num = 0;
		foreach (object item in obj)
		{
			array[num] = (item as string);
			num++;
		}
		return array;
	}

	public static object[] DecodeModsArray(object dictValue)
	{
		List<object> list = dictValue as List<object>;
		return new object[Math.Min(1, list.Count)];
	}

	public static string ShowFileSelectorForAudio(string title, long maximumSize = -1L)
	{
		return ShowFileSelector(title, RDString.Get("editor.dialog.audioFileFormat"), GCS.SupportedAudioFiles, RDString.Get("editor.dialog.saveBeforeImportingSounds"), maximumSize);
	}

	public static string ShowFileSelectorForImage(string title, long maximumSize = -1L)
	{
		return ShowFileSelector(title, RDString.Get("editor.dialog.imageFileFormat"), GCS.SupportedImageFiles, RDString.Get("editor.dialog.saveBeforeImportingImages"), maximumSize);
	}

	public static string ShowFileSelectorForVideo(string title, long maximumSize = -1L)
	{
		return ShowFileSelector(title, RDString.Get("editor.dialog.videoFileFormat"), GCS.SupportedVideoFiles, RDString.Get("editor.dialog.saveBeforeImportingVideos"), maximumSize);
	}

	public static string ShowFileSelector(string title, string extensionDescription, string[] extensions, string levelNotSavedMessage, long maximumSize = -1L)
	{
		CustomLevel instance = CustomLevel.instance;
		if (string.IsNullOrEmpty(instance.levelPath))
		{
			return null;
		}
		ExtensionFilter[] extensions2 = new ExtensionFilter[1]
		{
			new ExtensionFilter(extensionDescription, extensions)
		};
		string[] array = StandaloneFileBrowser.OpenFilePanel(title, Persistence.GetLastUsedFolder(), extensions2, multiselect: false);
		if (array.Length == 0 || string.IsNullOrEmpty(array[0]))
		{
			return null;
		}
		string text = Uri.UnescapeDataString(array[0].Replace("file:", ""));
		string directoryName = Path.GetDirectoryName(text);
		string fileName = Path.GetFileName(text);
		string directoryName2 = Path.GetDirectoryName(instance.levelPath);
		string text2 = Path.Combine(directoryName2, fileName);
		long length = new FileInfo(text).Length;
		if (directoryName != directoryName2 && !RDFile.Exists(text2))
		{
			if (maximumSize != -1)
			{
				if (length >= maximumSize)
				{
					return maximumSize.ToString();
				}
				RDFile.Copy(text, text2);
			}
			else
			{
				RDFile.Copy(text, text2);
			}
		}
		return fileName;
	}

	public static bool HasAudioFileExtension(this string filename)
	{
		for (int i = 0; i < GCS.SupportedAudioFiles.Length; i++)
		{
			string value = "." + GCS.SupportedAudioFiles[i];
			if (filename.EndsWith(value, StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	public static bool HasImageFileExtension(this string filename)
	{
		for (int i = 0; i < GCS.SupportedImageFiles.Length; i++)
		{
			string value = "." + GCS.SupportedImageFiles[i];
			if (filename.EndsWith(value, StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	public static RDAudioLoadType FindClip(string filename)
	{
		if (!filename.HasAudioFileExtension())
		{
			string clipName = "";
			if (AudioManager.Instance.FindOrLoadAudioClip(clipName) != null)
			{
				return RDAudioLoadType.SuccessInternalClipLoaded;
			}
		}
		else if (RDFile.Exists(Path.GetDirectoryName(ADOBase.levelPath) + Path.DirectorySeparatorChar.ToString() + filename))
		{
			return RDAudioLoadType.SuccessExternalClipLoaded;
		}
		UnityEngine.Debug.LogWarning("RDEditorUtils - Audio not found: " + filename);
		return RDAudioLoadType.ErrorFileNotFound;
	}

	public static string GetCurrentLevelFolderPath()
	{
		scnEditor instance = scnEditor.instance;
		return Path.GetDirectoryName(ADOBase.levelPath);
	}

	public static IEnumerator AudioClipFromFilename(string filename)
	{
		bool num = filename.HasAudioFileExtension();
		bool flag = false;
		if (!num)
		{
			string clipName = "" + "/" + filename;
			AudioClip audioClip = AudioManager.Instance.FindOrLoadAudioClip(clipName);
			if (audioClip != null)
			{
				yield return new RDAudioLoadResult(RDAudioLoadType.SuccessInternalClipLoaded, audioClip);
				flag = true;
			}
		}
		else
		{
			string path = GetCurrentLevelFolderPath() + Path.DirectorySeparatorChar.ToString() + filename;
			if (RDFile.Exists(path))
			{
				MonoBehaviour instance = scnEditor.instance;
				CoroutineWithData loadAudio = new CoroutineWithData(instance, AudioManager.Instance.FindOrLoadAudioClipExternal(path, mp3Streaming: false));
				yield return loadAudio.coroutine;
				yield return (RDAudioLoadResult)loadAudio.result;
				flag = true;
			}
		}
		if (!flag)
		{
			UnityEngine.Debug.LogWarning("AudioClip doesn't exist: " + filename);
			yield return new RDAudioLoadResult(RDAudioLoadType.ErrorFileNotFound, null);
		}
	}

	public static bool IsNullOrEmpty(this string s)
	{
		return string.IsNullOrEmpty(s);
	}

	public static bool CheckForKeyCombo(bool control, bool shift, KeyCode key)
	{
		bool flag = ControlIsPressed();
		bool holdingShift = RDInput.holdingShift;
		bool keyDown = UnityEngine.Input.GetKeyDown(key);
		if (control && shift)
		{
			return (flag && holdingShift) & keyDown;
		}
		if (control && !shift)
		{
			return flag && keyDown;
		}
		if (shift && !control)
		{
			return holdingShift && keyDown;
		}
		return false;
	}

	public static string KeyComboToString(bool control, bool shift, KeyCode keyCode)
	{
		return KeyComboToString(control, shift, alt: false, keyCode);
	}

	public static string KeyComboToString(bool control, bool shift, bool alt, KeyCode keyCode)
	{
		bool flag = ADOBase.platform == Platform.Mac && !Application.isEditor;
		string str = "";
		if (control)
		{
			str += (flag ? "cmd-" : "ctrl-");
		}
		if (shift)
		{
			str += "shift-";
		}
		if (alt)
		{
			str += (flag ? "opt-" : "alt-");
		}
		bool exists = false;
		string text = RDString.GetWithCheck("KeyCode." + keyCode.ToString(), out exists);
		if (!exists)
		{
			text = keyCode.ToString();
		}
		return str + text.ToLower();
	}

	public static bool ControlIsPressed()
	{
		if (ADOBase.platform != Platform.Mac || Application.isEditor)
		{
			if (!Input.GetKey(KeyCode.LeftControl))
			{
				return UnityEngine.Input.GetKey(KeyCode.RightControl);
			}
			return true;
		}
		if (!Input.GetKey(KeyCode.LeftMeta))
		{
			return UnityEngine.Input.GetKey(KeyCode.RightMeta);
		}
		return true;
	}

	public static string CombinePaths(params string[] paths)
	{
		if (paths == null)
		{
			throw new ArgumentNullException("paths");
		}
		return paths.Aggregate(Path.Combine);
	}

	public static string LogPath()
	{
		return CombinePaths(Environment.GetEnvironmentVariable("AppData"), "..", "LocalLow", Application.companyName, Application.productName, "Player.log");
	}

	public static void RevealInExplorer(string path)
	{
		if (!string.IsNullOrEmpty(path))
		{
			RuntimePlatform platform = Application.platform;
			switch (platform)
			{
			case RuntimePlatform.WindowsPlayer:
			case RuntimePlatform.WindowsEditor:
				OpenInWinFileBrowser(path);
				break;
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.OSXPlayer:
				OpenInMacFileBrowser(path);
				break;
			case RuntimePlatform.LinuxPlayer:
			case RuntimePlatform.LinuxEditor:
				OpenInLinuxFileBrowser(path);
				break;
			default:
				UnityEngine.Debug.LogError("RevealInExplorer not implemented for " + platform.ToString() + " platform, path: " + path);
				break;
			}
		}
	}

	public static void OpenInLinuxFileBrowser(string path)
	{
		string text = path.Replace("\\", "/");
		if (Directory.Exists(text))
		{
			Process.Start("xdg-open", "\"file://" + text + "\"");
			return;
		}
		string arguments = "--session --dest=org.freedesktop.FileManager1 --type=method_call /org/freedesktop/FileManager1 org.freedesktop.FileManager1.ShowItems array:string:\"file://" + text + "\" string:\"\"";
		Process.Start("dbus-send", arguments);
	}

	public static void OpenInMacFileBrowser(string path)
	{
		bool flag = false;
		string text = path.Replace("\\", "/");
		if (Directory.Exists(text))
		{
			flag = true;
		}
		if (!text.StartsWith("\""))
		{
			text = "\"" + text;
		}
		if (!text.EndsWith("\""))
		{
			text += "\"";
		}
		string arguments = (flag ? "" : "-R ") + text;
		try
		{
			Process.Start("open", arguments);
		}
		catch (Win32Exception ex)
		{
			ex.HelpLink = "";
		}
	}

	public static void OpenInWinFileBrowser(string path)
	{
		bool flag = false;
		string text = path.Replace("/", "\\");
		if (Directory.Exists(text))
		{
			flag = true;
		}
		try
		{
			Process.Start("explorer.exe", (flag ? "/root," : "/select,") + text);
		}
		catch (Win32Exception ex)
		{
			ex.HelpLink = "";
		}
	}

	public static bool CheckModsDependency(object[] mods)
	{
		return mods != null && mods.Length != 0;
	}

	public static void OpenLogDirectory()
	{
		if (!string.IsNullOrEmpty(lastLogDirectory) && RDDirectory.Exists(lastLogDirectory))
		{
			RevealInExplorer(lastLogDirectory);
			return;
		}
		string text = null;
		string[] array = (ADOBase.platform == Platform.Mac) ? macDirectories : logDirectories;
		foreach (string text2 in array)
		{
			UnityEngine.Debug.Log("trying to find dir: " + text2);
			string[] array2 = logFileNames;
			foreach (string path in array2)
			{
				if (RDFile.Exists(Path.Combine(text2, path)))
				{
					lastLogDirectory = text2;
					RevealInExplorer(text2);
					return;
				}
			}
			if (string.IsNullOrEmpty(text) && RDDirectory.Exists(text2))
			{
				text = text2;
			}
		}
		if (text != null)
		{
			lastLogDirectory = text;
			RevealInExplorer(text);
		}
	}

	public static bool CheckPlayerLogKeyCombo()
	{
		bool flag = UnityEngine.Input.GetKey(KeyCode.LeftAlt) || UnityEngine.Input.GetKey(KeyCode.RightAlt);
		bool flag2 = UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift);
		if (CheckForKeyCombo(control: true, shift: false, KeyCode.L))
		{
			return flag | flag2;
		}
		return false;
	}

	public static bool CheckPointerInObject(GameObject obj)
	{
		RectTransform component = obj.GetComponent<RectTransform>();
		Rect rect = component.rect;
		Vector2 point = component.InverseTransformPoint(UnityEngine.Input.mousePosition);
		return rect.Contains(point);
	}

	public static bool CheckPointerInObject(UnityEngine.Component obj)
	{
		return CheckPointerInObject(obj.gameObject);
	}

	public static GameObject[] ObjectsAtPointer()
	{
		PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
		pointerEventData.position = UnityEngine.Input.mousePosition;
		List<RaycastResult> list = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerEventData, list);
		return (from r in list
			select r.gameObject).ToArray();
	}

	public static T GetComponentInAllParents<T>(GameObject gameObject) where T : UnityEngine.Component
	{
		T val = null;
		Transform transform = gameObject.transform;
		while (transform != null && (UnityEngine.Object)val == (UnityEngine.Object)null)
		{
			val = transform.GetComponent<T>();
			transform = transform.parent;
		}
		return val;
	}
}
