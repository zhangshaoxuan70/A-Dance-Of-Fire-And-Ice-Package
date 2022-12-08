using GDMiniJSON;
using RDTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlayerPrefsJson
{
	public enum SaveFileLocation
	{
		Automatic,
		UserFolder,
		PersistentDataFolder
	}

	private static Dictionary<SaveFileType, PlayerPrefsJson> saveFiles = new Dictionary<SaveFileType, PlayerPrefsJson>();

	private static Dictionary<SaveFileType, string> saveFilesPath = new Dictionary<SaveFileType, string>();

	public static readonly SaveFileType[] LoadedSaveFileTypes = saveFiles.Keys.ToArray();

	public static readonly SaveFileType[] AllSaveFileTypes = Enum.GetValues(typeof(SaveFileType)) as SaveFileType[];

	public static readonly Encoding DefaultLevelEncoding = Encoding.UTF8;

	public static readonly Dictionary<SaveFileType, string> Filenames = new Dictionary<SaveFileType, string>
	{
		{
			SaveFileType.General,
			"data"
		},
		{
			SaveFileType.CustomWorld,
			"custom_data"
		}
	};

	public readonly SaveFileType fileType;

	public readonly Dictionary<string, object> dict = new Dictionary<string, object>();

	public readonly DeltaDictionary<string, object> deltaDict = new DeltaDictionary<string, object>();

	public readonly Dictionary<SaveFileType, Dictionary<string, object>> selectedDicts = new Dictionary<SaveFileType, Dictionary<string, object>>();

	public bool IsMultiselectInstance => selectedDicts.Count > 0;

	public static bool LoadFile(SaveFileType saveFileType, bool loadBackup, out Dictionary<string, object> fileContent)
	{
		fileContent = null;
		string saveFilePath = GetSaveFilePath(saveFileType, loadBackup);
		if (RDFile.Exists(saveFilePath))
		{
			string json = RDFile.ReadAllText(saveFilePath, DefaultLevelEncoding);
			if (!ValidateJson(json))
			{
				return false;
			}
			Dictionary<string, object> dictionary = null;
			try
			{
				dictionary = (Json.Deserialize(json) as Dictionary<string, object>);
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log("There was an error deserializing save file in path " + saveFilePath + ".\nException: " + ex.ToString());
			}
			if (dictionary == null)
			{
				UnityEngine.Debug.Log("Deserializing save in path " + saveFilePath + " is null.");
				return false;
			}
			RDBaseDll.printem("JSON deserialized successfully.");
			fileContent = dictionary;
			return true;
		}
		return false;
	}

	public static bool ValidateJson(string json)
	{
		if (json.Length < 10)
		{
			UnityEngine.Debug.LogWarning("PlayerPrefsJson: encoded JSON is way too short, seems corrupted: " + json);
			return false;
		}
		if (json[0] != '{' && json[1] != '{' && json[2] != '{')
		{
			UnityEngine.Debug.LogWarning("PlayerPrefsJson: doesn't start with {");
			return false;
		}
		return true;
	}

	public static string GetSaveFilePath(SaveFileType saveFileType, bool loadBackup = false, bool corruptedFile = false, SaveFileLocation savefileFolderLocation = SaveFileLocation.Automatic)
	{
		string text = Filenames[saveFileType];
		string text2;
		if (!corruptedFile && saveFilesPath.TryGetValue(saveFileType, out string value))
		{
			text2 = value;
		}
		else
		{
			RDBaseDll.printem("Loading '" + text + "'");
			if (savefileFolderLocation == SaveFileLocation.Automatic)
			{
				bool appIsInSteamLibrary = ADOBase.appIsInSteamLibrary;
			}
			text2 = GetSaveFileFolderPath(ADOBase.appIsInSteamLibrary);
			text2 += Path.DirectorySeparatorChar.ToString();
			text2 += (corruptedFile ? $"corrupted_{text}_{DateTime.Now:yyyy'-'MM'-'dd'_'HH'-'mm'-'ss}" : text);
			text2 += ".sav";
			if (!corruptedFile)
			{
				saveFilesPath.Add(saveFileType, text2);
			}
		}
		if (loadBackup)
		{
			text2 += ".old";
		}
		return text2;
	}

	public static string GetSaveFileFolderPath(bool isSteam)
	{
		string text;
		if (ADOBase.appIsInSteamLibrary)
		{
			text = Application.dataPath;
			if (ADOBase.platform == Platform.Windows || ADOBase.platform == Platform.Linux)
			{
				text = Directory.GetParent(text).FullName;
			}
			else if (ADOBase.platform == Platform.Mac)
			{
				text = Directory.GetParent(text).Parent.FullName;
			}
			text = text + Path.DirectorySeparatorChar.ToString() + "User";
			if (!RDDirectory.Exists(text))
			{
				RDDirectory.CreateDirectory(text);
			}
		}
		else
		{
			text = Application.persistentDataPath;
		}
		return text;
	}

	public static bool SaveFileExists(SaveFileType fileType, SaveFileLocation location = SaveFileLocation.Automatic)
	{
		return RDFile.Exists(GetSaveFilePath(fileType, loadBackup: false, corruptedFile: false, location));
	}

	public static void AddSaveFile(SaveFileType fileType, PlayerPrefsJson data)
	{
		if (!saveFiles.ContainsKey(fileType))
		{
			saveFiles.Add(fileType, data);
		}
	}

	public static PlayerPrefsJson CreateSaveFile(SaveFileType fileType)
	{
		PlayerPrefsJson playerPrefsJson = new PlayerPrefsJson(fileType);
		playerPrefsJson.Save();
		AddSaveFile(fileType, playerPrefsJson);
		return playerPrefsJson;
	}

	public static void SaveAllFiles()
	{
		foreach (PlayerPrefsJson value in saveFiles.Values)
		{
			value.Save();
		}
	}

	public static void MarkCorruptFile(SaveFileType fileType, bool isBackup)
	{
		string saveFilePath = GetSaveFilePath(fileType, isBackup);
		if (RDFile.Exists(saveFilePath))
		{
			RDFile.Copy(saveFilePath, GetSaveFilePath(fileType, isBackup, corruptedFile: true));
			RDFile.Delete(saveFilePath);
		}
	}

	public static PlayerPrefsJson Select(SaveFileType saveFileType)
	{
		if (saveFiles.TryGetValue(saveFileType, out PlayerPrefsJson value))
		{
			return value;
		}
		return CreateSaveFile(saveFileType);
	}

	public static PlayerPrefsJson SelectMany(params SaveFileType[] saveFileTypes)
	{
		return new PlayerPrefsJson(saveFileTypes);
	}

	public static PlayerPrefsJson SelectAll()
	{
		return new PlayerPrefsJson(AllSaveFileTypes);
	}

	public PlayerPrefsJson(SaveFileType saveFileType, Dictionary<string, object> data = null)
	{
		fileType = saveFileType;
		dict = (data ?? dict);
	}

	private PlayerPrefsJson(SaveFileType[] fileTypes)
	{
		foreach (SaveFileType saveFileType in fileTypes)
		{
			selectedDicts.Add(saveFileType, Select(saveFileType).dict);
		}
	}

	public void ApplyDeltaDict()
	{
		deltaDict.Apply(selectedDicts.Values);
	}

	public void Save()
	{
		if (IsMultiselectInstance)
		{
			return;
		}
		if (dict == null)
		{
			UnityEngine.Debug.LogWarning("PlayerPrefsJson: 'dict' is null");
			return;
		}
		if (dict.Keys.Count == 0)
		{
			UnityEngine.Debug.LogWarning("PlayerPrefsJson: 'dict' is empty");
			return;
		}
		string text = Json.Serialize(dict);
		if (text == null)
		{
			UnityEngine.Debug.LogWarning("PlayerPrefsJson: encoded JSON is null");
		}
		else if (ValidateJson(text))
		{
			RDFile.WriteAllText(GetSaveFilePath(fileType), text, DefaultLevelEncoding);
		}
	}

	public void SaveBackup()
	{
		if (!IsMultiselectInstance)
		{
			UnityEngine.Debug.Log("Saving backup.");
			RDFile.Copy(GetSaveFilePath(fileType), GetSaveFilePath(fileType, loadBackup: true), overwrite: true);
		}
	}

	public int GetInt(string key, int defaultValue = 0, bool checkPlayerPrefsForHighestValue = false)
	{
		if (IsMultiselectInstance)
		{
			return defaultValue;
		}
		checkPlayerPrefsForHighestValue = false;
		int num = Get(key, defaultValue);
		if (checkPlayerPrefsForHighestValue)
		{
			int num2 = Math.Max(num, PlayerPrefs.GetInt(key, defaultValue));
			if (num2 != num)
			{
				SetInt(key, num2);
				num = num2;
			}
		}
		return num;
	}

	public float GetFloat(string key, float defaultValue = 0f, bool checkPlayerPrefsForHighestValue = false)
	{
		if (IsMultiselectInstance)
		{
			return defaultValue;
		}
		checkPlayerPrefsForHighestValue = false;
		float num = dict.ContainsKey(key) ? Convert.ToSingle(dict[key]) : defaultValue;
		if (checkPlayerPrefsForHighestValue)
		{
			float num2 = Math.Max(num, PlayerPrefs.GetFloat(key, defaultValue));
			if (num2 != num)
			{
				SetFloat(key, num2);
				num = num2;
			}
		}
		return num;
	}

	public string GetString(string key, string defaultValue = "")
	{
		return Get(key, defaultValue);
	}

	public bool GetBool(string key, bool defaultValue = false)
	{
		return Get(key, defaultValue);
	}

	public Dictionary<string, object> GetDict(string key)
	{
		if (!IsMultiselectInstance && dict.ContainsKey(key))
		{
			return dict[key] as Dictionary<string, object>;
		}
		return new Dictionary<string, object>();
	}

	public List<object> GetList(string key)
	{
		if (!IsMultiselectInstance && dict.ContainsKey(key))
		{
			return dict[key] as List<object>;
		}
		return new List<object>();
	}

	public T Get<T>(string key, T defaultValue)
	{
		object value;
		if (!IsMultiselectInstance && dict.TryGetValue(key, out value) && value is T)
		{
			return (T)value;
		}
		return defaultValue;
	}

	public void SetList(string key, List<object> values)
	{
		if (!IsMultiselectInstance)
		{
			dict[key] = values;
		}
	}

	public void SetDict(string key, Dictionary<string, object> value)
	{
		if (!IsMultiselectInstance)
		{
			dict[key] = value;
		}
	}

	public void SetInt(string key, int value)
	{
		if (!IsMultiselectInstance)
		{
			PlayerPrefs.SetInt(key, value);
			dict[key] = value;
		}
	}

	public void SetFloat(string key, float value)
	{
		if (!IsMultiselectInstance)
		{
			PlayerPrefs.SetFloat(key, value);
			dict[key] = value;
		}
	}

	public void SetString(string key, string value)
	{
		if (!IsMultiselectInstance)
		{
			PlayerPrefs.SetString(key, value);
			dict[key] = value;
		}
	}

	public void SetBool(string key, bool value)
	{
		if (!IsMultiselectInstance)
		{
			PlayerPrefs.SetInt(key, value ? 1 : 0);
			dict[key] = value;
		}
	}

	public void RemoveKey(string key)
	{
		if (!IsMultiselectInstance && dict.ContainsKey(key))
		{
			dict.Remove(key);
		}
	}
}
