using ADOFAI;
using DG.Tweening;
using GDMiniJSON;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class ADOStartup
{
	public static ErrorCanvas errorCanvas;

	private static bool startup;

	public static List<string> addedMods = new List<string>();

	public static void Startup()
	{
		if (!startup)
		{
			startup = true;
			Application.logMessageReceived += LogMessageReceived;
			_003CStartup_003Eg__GetPlatform_007C2_1();
			DetermineAppLocation();
			_003CStartup_003Eg__SetBuildDate_007C2_6();
			_003CStartup_003Eg__SetLocale_007C2_0();
			_003CStartup_003Eg__LoadSaveData_007C2_2();
			_003CStartup_003Eg__FixResolution_007C2_3();
			SetupLevelEventsInfo();
			_003CStartup_003Eg__SetSettings_007C2_4();
			_003CStartup_003Eg__ForceResolutionOnLofi_007C2_5();
			_003CStartup_003Eg__LoadCalibration_007C2_7();
			_003CStartup_003Eg__LoadLevelEventSprites_007C2_8();
			_003CStartup_003Eg__LoadLevelCategorySprites_007C2_9();
			_003CStartup_003Eg__LoadAssetBundle_007C2_11();
			_003CStartup_003Eg__SetupSfxHandler_007C2_12();
			_003CStartup_003Eg__GetSteamBranch_007C2_10();
			_003CStartup_003Eg__CheckIfTaroDLCIsInstalled_007C2_13();
			UnityEngine.Debug.Log($"Neo Cosmos: owns {ADOBase.ownsTaroDLC}, has {ADOBase.hasTaroDLC}");
			scrFloor.ShaderProperty_Color = Shader.PropertyToID("_Color");
			scrFloor.ShaderProperty_Alpha = Shader.PropertyToID("_Alpha");
			FloorRenderer.ShaderProperty_Flash = Shader.PropertyToID("_Flash");
			if (!ADOBase.isMobile)
			{
				new GameObject("DiscordController").AddComponent<DiscordController>();
			}
			else
			{
				GCNS.sceneLevelSelect = "scnMobileMenu";
			}
			Persistence.GiveAchievements();
			UnityEngine.Object.Instantiate(RDConstants.data.prefab_rewiredManager);
			RDInput.Setup();
			Analytics.UploadBranchToUnity();
			QualitySettings.antiAliasing = Persistence.GetFXAA();
			UnityEngine.Debug.Log("runtime path: " + Addressables.RuntimePath);
		}
	}

	public static void SetupLevelEventsInfo()
	{
		Dictionary<string, object> obj = Json.Deserialize(Resources.Load<TextAsset>("LevelEditorProperties").text) as Dictionary<string, object>;
		GCS.levelEventsInfo = DecodeLevelEventInfoList(obj["levelEvents"] as List<object>);
		GCS.settingsInfo = DecodeLevelEventInfoList(obj["settings"] as List<object>);
		DecodeLevelEventCategoryList(obj["categories"] as List<object>);
		LevelEventType[] obj2 = (LevelEventType[])Enum.GetValues(typeof(LevelEventType));
		GCS.levelEventTypeString = new Dictionary<LevelEventType, string>();
		LevelEventType[] array = obj2;
		for (int i = 0; i < array.Length; i++)
		{
			LevelEventType key = array[i];
			GCS.levelEventTypeString.Add(key, key.ToString());
		}
	}

	public static Dictionary<string, LevelEventInfo> DecodeLevelEventInfoList(List<object> eventInfoList)
	{
		Dictionary<string, LevelEventInfo> dictionary = new Dictionary<string, LevelEventInfo>();
		foreach (Dictionary<string, object> eventInfo in eventInfoList)
		{
			if (!eventInfo.TryGetValue("enabled", out object value) || (bool)value)
			{
				LevelEventInfo levelEventInfo = new LevelEventInfo();
				levelEventInfo.name = (eventInfo["name"] as string);
				levelEventInfo.type = RDUtils.ParseEnum(levelEventInfo.name, LevelEventType.None);
				levelEventInfo.pro = (eventInfo.TryGetValue("pro", out object value2) && (bool)value2);
				levelEventInfo.taroDLC = (eventInfo.TryGetValue("taroDLC", out object value3) && (bool)value3);
				levelEventInfo.categories = new List<LevelEventCategory>();
				levelEventInfo.executionTime = RDUtils.ParseEnum(eventInfo["executionTime"] as string, LevelEventExecutionTime.OnBar);
				levelEventInfo.propertiesInfo = new Dictionary<string, PropertyInfo>();
				List<object> obj = eventInfo["properties"] as List<object>;
				int order = 0;
				foreach (Dictionary<string, object> item in obj)
				{
					if (!item.ContainsKey("enabled") || (bool)item["enabled"])
					{
						PropertyInfo propertyInfo = new PropertyInfo(item, levelEventInfo);
						propertyInfo.order = order;
						levelEventInfo.propertiesInfo.Add(propertyInfo.name, propertyInfo);
					}
				}
				dictionary.Add(levelEventInfo.name, levelEventInfo);
			}
		}
		return dictionary;
	}

	public static void DecodeLevelEventCategoryList(List<object> categoryInfoList)
	{
		foreach (Dictionary<string, object> categoryInfo in categoryInfoList)
		{
			List<object> list = categoryInfo["events"] as List<object>;
			LevelEventCategory item = RDUtils.ParseEnum(categoryInfo["name"] as string, LevelEventCategory.Gameplay);
			foreach (string item2 in list)
			{
				if (GCS.levelEventsInfo.ContainsKey(item2))
				{
					GCS.levelEventsInfo[item2].categories.Add(item);
				}
			}
		}
	}

	public static void LogMessageReceived(string logString, string stackTrace, LogType type)
	{
		if (type != 0 && type != LogType.Exception && type != LogType.Assert)
		{
			return;
		}
		string text = $"{type}:\n{logString}\n{stackTrace}";
		if (!text.Contains("GLSL link error"))
		{
			if (errorCanvas == null)
			{
				errorCanvas = UnityEngine.Object.Instantiate(RDConstants.data.prefab_errorCanvas).GetComponent<ErrorCanvas>();
			}
			errorCanvas.ShowError(text);
		}
	}

	public static void ModWasAdded(string modName)
	{
		if (!string.IsNullOrEmpty(modName))
		{
			addedMods.Add(modName);
		}
		UnityEngine.Debug.Log("Mod was added: " + modName);
	}

	public static void DetermineAppLocation()
	{
		bool appIsInSteamLibrary = false;
		UnityEngine.Debug.Log("Platform: " + ADOBase.platform.ToString());
		DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath)?.Parent;
		if (ADOBase.platform == Platform.Mac)
		{
			directoryInfo = directoryInfo?.Parent;
		}
		if (directoryInfo != null && directoryInfo.Parent?.Name == "common" && directoryInfo.Parent?.Parent?.Name == "steamapps")
		{
			appIsInSteamLibrary = true;
		}
		UnityEngine.Debug.Log("isInSteamLibrary: " + appIsInSteamLibrary.ToString());
		ADOBase.appIsInSteamLibrary = appIsInSteamLibrary;
	}

	[CompilerGenerated]
	private static void _003CStartup_003Eg__SetLocale_007C2_0()
	{
		Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
	}

	[CompilerGenerated]
	private static void _003CStartup_003Eg__GetPlatform_007C2_1()
	{
		Platform platform = Platform.Windows;
		switch (Application.platform)
		{
		case RuntimePlatform.Android:
			platform = Platform.Android;
			break;
		case RuntimePlatform.IPhonePlayer:
			platform = Platform.iOS;
			break;
		case RuntimePlatform.LinuxPlayer:
		case RuntimePlatform.LinuxEditor:
			platform = Platform.Linux;
			break;
		case RuntimePlatform.OSXEditor:
		case RuntimePlatform.OSXPlayer:
			platform = Platform.Mac;
			break;
		case RuntimePlatform.WindowsPlayer:
		case RuntimePlatform.WindowsEditor:
			platform = Platform.Windows;
			break;
		case RuntimePlatform.Switch:
			platform = Platform.Switch;
			break;
		case RuntimePlatform.WebGLPlayer:
			platform = Platform.WebGL;
			break;
		}
		ADOBase.platform = platform;
		UnityEngine.Debug.Log($"Setting platform to {platform}.");
	}

	[CompilerGenerated]
	private static void _003CStartup_003Eg__LoadSaveData_007C2_2()
	{
		Persistence.Load();
	}

	[CompilerGenerated]
	private static void _003CStartup_003Eg__FixResolution_007C2_3()
	{
		Vector2Int vector2Int = new Vector2Int(640, 360);
		if (Screen.width < vector2Int.x || Screen.height < vector2Int.y)
		{
			Screen.SetResolution(vector2Int.x, vector2Int.y, FullScreenMode.Windowed, 60);
		}
		if (!RDC.runningOnSteamDeck)
		{
			return;
		}
		Resolution[] resolutions = Screen.resolutions;
		Resolution resolution = resolutions[0];
		int num = resolution.width * resolution.height;
		Resolution[] array = resolutions;
		for (int i = 0; i < array.Length; i++)
		{
			Resolution resolution2 = array[i];
			int num2 = resolution2.width * resolution2.height;
			if (num2 > num)
			{
				resolution = resolution2;
				num = num2;
			}
			else if (num2 == num && resolution2.refreshRate > resolution.refreshRate)
			{
				resolution = resolution2;
			}
		}
		if (resolution.width == 1280 && resolution.height == 800)
		{
			Screen.SetResolution(1280, 800, FullScreenMode.MaximizedWindow);
			UnityEngine.Debug.Log($"Setting game resolution to {1280}x{800} (Steam Deck)");
		}
	}

	[CompilerGenerated]
	private static void _003CStartup_003Eg__SetSettings_007C2_4()
	{
		DOTween.SetTweensCapacity(500, 50);
		GCS.d_vibrate = Persistence.GetVibration();
		scrController.volume = Persistence.GetGlobalVolume();
		Application.targetFrameRate = Persistence.GetTargetFramerate();
		QualitySettings.vSyncCount = Persistence.GetVSync();
		RDC.forceNoSteamworks = false;
		GCS.perfectOnlyMode = Persistence.GetPerfectsOnlyMode();
		scrController.showDetailedResults = Persistence.GetShowDetailedResults();
	}

	[CompilerGenerated]
	private static void _003CStartup_003Eg__ForceResolutionOnLofi_007C2_5()
	{
		if (GCS.lofiVersion && !ADOBase.isMobile)
		{
			Screen.fullScreen = false;
			if (ADOBase.playerIsOnIntroScene && !GCS.webHasSetResolution)
			{
				Screen.SetResolution(800, 400, FullScreenMode.Windowed);
				GCS.webHasSetResolution = true;
			}
		}
	}

	[CompilerGenerated]
	private static void _003CStartup_003Eg__SetBuildDate_007C2_6()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("buildDate");
		if (textAsset != null)
		{
			GCNS.buildDate = textAsset.text;
			UnityEngine.Debug.Log($"Version r{97}, ({GCNS.buildDate})");
		}
	}

	[CompilerGenerated]
	private static void _003CStartup_003Eg__LoadCalibration_007C2_7()
	{
		scrConductor.defaultPresets = CalibrationPreset.LoadDefaults();
		scrConductor.UpdateCurrentAudioOutput();
	}

	[CompilerGenerated]
	private static void _003CStartup_003Eg__LoadLevelEventSprites_007C2_8()
	{
		GCS.levelEventIcons = new Dictionary<LevelEventType, Sprite>();
		foreach (object value in Enum.GetValues(typeof(LevelEventType)))
		{
			Sprite sprite = Resources.Load<Sprite>("LevelEditor/LevelEvents/" + value.ToString());
			if (sprite != null)
			{
				GCS.levelEventIcons.Add((LevelEventType)value, sprite);
			}
		}
	}

	[CompilerGenerated]
	private static void _003CStartup_003Eg__LoadLevelCategorySprites_007C2_9()
	{
		GCS.eventCategoryIcons = new Dictionary<LevelEventCategory, Sprite>();
		foreach (object value in Enum.GetValues(typeof(LevelEventCategory)))
		{
			Sprite sprite = Resources.Load<Sprite>("LevelEditor/EventCategories/" + value.ToString());
			if (sprite != null)
			{
				GCS.eventCategoryIcons.Add((LevelEventCategory)value, sprite);
			}
		}
	}

	[CompilerGenerated]
	private static void _003CStartup_003Eg__GetSteamBranch_007C2_10()
	{
		if (SteamIntegration.Instance.initialized && SteamApps.GetCurrentBetaName(out string pchName, 20))
		{
			GCS.steamBranchName = pchName;
		}
	}

	[CompilerGenerated]
	private static void _003CStartup_003Eg__LoadAssetBundle_007C2_11()
	{
	}

	[CompilerGenerated]
	private static void _003CStartup_003Eg__SetupSfxHandler_007C2_12()
	{
		UnityEngine.Object.DontDestroyOnLoad(UnityEngine.Object.Instantiate(RDConstants.data.prefab_sfxHandler));
	}

	[CompilerGenerated]
	private static void _003CStartup_003Eg__CheckIfTaroDLCIsInstalled_007C2_13()
	{
		string runtimePath = Addressables.RuntimePath;
		string path = "StandaloneWindows64";
		ADOBase.hasTaroDLC = File.Exists(Path.Combine(runtimePath, path, "neocosmos_scenes_all.bundle"));
	}
}
