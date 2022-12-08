using ADOFAI;
using RDTools;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Persistence : RDClassDll
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Func<string, bool> _003C_003E9__265_1;

		internal bool _003CLoad_003Eb__265_1(string k)
		{
			if (!customPrefs.dict.ContainsKey(k))
			{
				return _003CLoad_003Eg__StartsWithAnyOf_007C265_0(k, new string[3]
				{
					"CLSTotalPlays",
					"CustomWorld_",
					"SubscribedToFeatured_"
				});
			}
			return false;
		}
	}

	private const int trueValue = 1;

	private const int falseValue = 0;

	private const char intType = 'i';

	private const char floatType = 'f';

	private const char boolType = 'b';

	private const char stringType = 's';

	public const float inputOffsetNotSet = 999f;

	private const string keyPercentCompletion = "percentCompletion";

	private const string keyBestPercentAccuracy = "bestPercentAccuracy";

	private const string keyBestPercentXAccuracy = "bestPercentXAccuracy";

	private const string keyBestSpeedMultiplier = "bestSpeedMultiplier";

	private const string keyWorldAttempts = "worldAttempts";

	private const string keyWorldAttemptsWithoutNewBest = "worldAttemptsWithoutNewBest";

	private const string keyTutorialProgressWorld = "tutorialProgress";

	private const string keyIsHighestPossibleAccuracy = "isHighestPossibleAcc";

	private const string keyCurrentLevel = "currentLevel";

	private const string keyPassedTutorial = "passedTutorial";

	private const string keyMaxLevel = "maxlevel";

	private const string keyReleaseVersion = "version";

	private const string keyCalibrationPresets = "calibrationPresets";

	private const string keyUnlockedXtra = "unlockedXtra";

	private const string keyUnlockedXC = "unlockedXC";

	private const string keyUnlockedXH = "unlockedXH";

	private const string keyUnlockedXR = "unlockedXR";

	private const string keyUnlockedMD = "unlockedMD";

	private const string keyPlayedFirst5WorldsCutscene = "playedFirst5WorldsCutscene";

	private const string keyPlayedWorld6Cutscene = "keyPlayedWorld6Cutscene";

	private const string keyColorRed = "colorRed";

	private const string keyColorBlue = "colorBlue";

	private const string keyLastUsedFolder = "lastUsedFolder";

	private const string keyLastOpenedLevel = "lastOpenedLevel";

	private const string keyAcceptedAgreement = "acceptedAgreement";

	private const string keyTargetFramerate = "targetFramerate";

	private const string keyNextRatingPromptDay = "nextRatingromptDay";

	private const string keyRatedGame = "ratedGame";

	private const string keyDisplayedCLSIntro = "displayedCLSIntro";

	private const string keyCLSSortingParameter = "CLSSortingParameter";

	private const string keyCLSSortingReversed = "CLSSortingReversed";

	private const string keyEditorScaleY = "editorScaleY";

	private const string keyEditorFavoriteEvents = "editorFavoriteEvents";

	private const string keyShowRDOffer = "showRDOffer";

	private const string keyShowXAccuracy = "showXAccuracy";

	private const string keyHitErrorMeterSize = "hitErrorMeterSize";

	private const string keyHitErrorMeterShape = "hitErrorMeterShape";

	private const string keyMultitapTileBehavior = "multitapTileBehavior";

	private const string keyHoldBehavior = "holdBehavior";

	private const string keyFreeroamInvuln = "freeroamInvuln";

	private const string keyShowDetailedResults = "showDetailedResults";

	private const string keySkipIntroAfterFirstTry = "skipIntroAfterFirstTry";

	private const string keyLastAnalyticsUpdate = "lastAnalyticsUpdate";

	private const string keyFXAA = "antiAliasing";

	private const string keySavedProgress = "savedProgress";

	private const string keySamuraiRed = "samuraiRed";

	private const string keySamuraiBlue = "samuraiBlue";

	private const string keyFaceRed = "faceRed";

	private const string keyFaceBlue = "faceBlue";

	private const string keyInputOffset = "offset";

	private const string keyVisualOffset = "offset_v";

	private const string keyGlobalVolume = "globalVolume";

	private const string keyVibrate = "vibrate";

	private const string keyVisualQuality = "visualQuality";

	private const string keyVisualEffects = "visualEffects";

	private const string keyLanguage = "language";

	private const string keyVsync = "vSyncKey";

	private const string keyPerfectsOnlyMode = "perfectsOnlyMode";

	private const string keyMarkFloorWithComment = "markFloorWithComment";

	private const string keyDisableRewindButton = "disableRewindButton";

	private const string keyShortcutPlaySpeed = "shortcutPlaySpeed";

	private const string keyQuickScrubbedPlay = "quickScrubbedPlay";

	private const string keyAnimateSpeedChanges = "animateSpeedChanges";

	private const string keyHideCursorWhilePlaying = "hideCursorWhilePlaying";

	private const string keyUseAsynchronousInput = "useAsynchronousInput";

	private const string keyTaroStoryProgress = "dlcStoryProgress";

	private const string keyTaroEXProgress = "dlcEXProgress";

	private const string keyTaroMedalsPrefix = "dlcMedals";

	private const string keyTaroT5Time = "dlcT5Time";

	public static int bonusWorldIndex => GCNS.worldData["B"].index;

	public static string DataPath => Application.persistentDataPath;

	public static PlayerPrefsJson generalPrefs => PlayerPrefsJson.Select(SaveFileType.General);

	public static PlayerPrefsJson customPrefs => PlayerPrefsJson.Select(SaveFileType.CustomWorld);

	public static float GetPercentCompletion(int worldZeroIndex)
	{
		return generalPrefs.GetFloat("percentCompletion" + dd(worldZeroIndex), 0f, checkPlayerPrefsForHighestValue: true);
	}

	public static float GetBestPercentAccuracy(int worldZeroIndex)
	{
		return generalPrefs.GetFloat("bestPercentAccuracy" + dd(worldZeroIndex), 0f, checkPlayerPrefsForHighestValue: true);
	}

	public static float GetBestPercentXAccuracy(int worldZeroIndex)
	{
		return generalPrefs.GetFloat("bestPercentXAccuracy" + dd(worldZeroIndex), 0f, checkPlayerPrefsForHighestValue: true);
	}

	public static float GetBestSpeedMultiplier(int worldZeroIndex)
	{
		return generalPrefs.GetFloat("bestSpeedMultiplier" + dd(worldZeroIndex), 0f, checkPlayerPrefsForHighestValue: true);
	}

	public static int GetWorldAttempts(int worldZeroIndex)
	{
		return generalPrefs.GetInt("worldAttempts" + dd(worldZeroIndex), 0, checkPlayerPrefsForHighestValue: true);
	}

	public static int GetWorldAttemptsWithoutNewBest(int worldZeroIndex)
	{
		return generalPrefs.GetInt("worldAttemptsWithoutNewBest" + dd(worldZeroIndex), 0, checkPlayerPrefsForHighestValue: true);
	}

	public static int GetLevelTutorialProgress(string world)
	{
		return GetLevelTutorialProgress(GCNS.worldData[world].index);
	}

	public static int GetLevelTutorialProgress(int worldZeroIndexed)
	{
		return generalPrefs.GetInt("tutorialProgress" + dd(worldZeroIndexed), 0, checkPlayerPrefsForHighestValue: true);
	}

	public static bool GetIsHighestPossibleAcc(int worldZeroIndex)
	{
		return generalPrefs.GetBool("isHighestPossibleAcc" + dd(worldZeroIndex));
	}

	public static string GetSavedCurrentLevel()
	{
		return generalPrefs.GetString("currentLevel", "0-0");
	}

	public static bool GetShowRDOffer()
	{
		return generalPrefs.GetBool("showRDOffer", defaultValue: true);
	}

	public static bool GetVibration()
	{
		return generalPrefs.GetBool("vibrate");
	}

	public static VisualQuality GetVisualQuality()
	{
		return (VisualQuality)generalPrefs.GetInt("visualQuality", 20);
	}

	public static VisualEffects GetVisualEffects(bool getReal = false)
	{
		if (!getReal && scnEditor.instance != null)
		{
			return VisualEffects.Full;
		}
		return (VisualEffects)generalPrefs.GetInt("visualEffects", 1);
	}

	public static int GetTargetFramerate()
	{
		int num = 60;
		Resolution[] resolutions = Screen.resolutions;
		for (int i = 0; i < resolutions.Length; i++)
		{
			Resolution resolution = resolutions[i];
			num = Math.Max(resolution.refreshRate, num);
		}
		if (num < 120)
		{
			num = 120;
		}
		return generalPrefs.GetInt("targetFramerate", num);
	}

	public static bool GetShowDetailedResults()
	{
		return generalPrefs.GetBool("showDetailedResults");
	}

	public static bool GetPassedMobileTutorial()
	{
		return generalPrefs.GetBool("passedTutorial");
	}

	public static int GetGlobalVolume()
	{
		return generalPrefs.GetInt("globalVolume", 10);
	}

	public static string GetLanguage()
	{
		SystemLanguage systemLanguage = Application.systemLanguage;
		return generalPrefs.GetString("language", systemLanguage.ToString());
	}

	public static bool GetPerfectsOnlyMode()
	{
		return generalPrefs.GetBool("perfectsOnlyMode");
	}

	public static int GetVSync()
	{
		return generalPrefs.GetInt("vSyncKey", QualitySettings.vSyncCount);
	}

	public static bool GetUnlockedXF()
	{
		if (GCS.d_booth)
		{
			return false;
		}
		return generalPrefs.GetBool("unlockedXtra");
	}

	public static bool GetUnlockedXC()
	{
		return generalPrefs.GetBool("unlockedXC");
	}

	public static bool GetUnlockedXH()
	{
		return generalPrefs.GetBool("unlockedXH");
	}

	public static bool GetUnlockedXR()
	{
		return generalPrefs.GetBool("unlockedXR");
	}

	public static bool GetUnlockedMD()
	{
		return generalPrefs.GetBool("unlockedMD");
	}

	public static bool GetPlayedFirs5WorldsCutscene()
	{
		return generalPrefs.GetBool("playedFirst5WorldsCutscene");
	}

	public static bool GetPlayedWorld6Cutscene()
	{
		return generalPrefs.GetBool("keyPlayedWorld6Cutscene");
	}

	public static Color GetPlayerColor(bool red)
	{
		string @string = generalPrefs.GetString(red ? "colorRed" : "colorBlue", null);
		if (@string == "gold")
		{
			return scrPlanet.goldColor;
		}
		if (@string == "rainbow")
		{
			return scrPlanet.rainbowColor;
		}
		if (@string == "transPink")
		{
			return scrPlanet.transPinkColor;
		}
		if (@string == "transBlue")
		{
			return scrPlanet.transBlueColor;
		}
		if (@string == "nbYellow")
		{
			return scrPlanet.nbYellowColor;
		}
		if (@string == "nbPurple")
		{
			return scrPlanet.nbPurpleColor;
		}
		if (@string == "overseer")
		{
			return scrPlanet.overseerColor;
		}
		if (!RDUtils.TryHexToColor(@string, out Color color))
		{
			return red ? Color.red : Color.blue;
		}
		return color;
	}

	public static List<LevelEventType> GetFavoriteEditorEvents()
	{
		List<object> list = generalPrefs.GetList("editorFavoriteEvents");
		List<LevelEventType> list2 = new List<LevelEventType>();
		foreach (object item in list)
		{
			list2.Add((LevelEventType)Enum.Parse(typeof(LevelEventType), item.ToString()));
		}
		return list2;
	}

	public static bool GetSamuraiMode(bool red)
	{
		return generalPrefs.GetBool(red ? "samuraiRed" : "samuraiBlue");
	}

	public static bool GetFaceMode(bool red)
	{
		return generalPrefs.GetBool(red ? "faceRed" : "faceBlue");
	}

	public static float GetInputOffset()
	{
		return PlayerPrefs.GetFloat("offset", 999f);
	}

	public static float GetVisualOffset()
	{
		return generalPrefs.GetFloat("offset_v");
	}

	public static bool HasAcceptedAgreement()
	{
		return generalPrefs.GetBool("acceptedAgreement");
	}

	public static bool GetDisplayedCLSIntro()
	{
		return generalPrefs.GetBool("displayedCLSIntro");
	}

	public static OptionsPanelsCLS.OptionName GetCLSSortingParameter()
	{
		return (OptionsPanelsCLS.OptionName)generalPrefs.GetInt("CLSSortingParameter", 1);
	}

	public static bool GetCLSSortingReversed()
	{
		return generalPrefs.GetBool("CLSSortingReversed");
	}

	public static int GetNextRatingPromptDay()
	{
		return generalPrefs.GetInt("nextRatingromptDay", -1);
	}

	public static bool GetShowXAccuracy()
	{
		return generalPrefs.GetBool("showXAccuracy");
	}

	public static ErrorMeterSize GetHitErrorMeterSize()
	{
		return (ErrorMeterSize)generalPrefs.GetInt("hitErrorMeterSize");
	}

	public static ErrorMeterShape GetHitErrorMeterShape()
	{
		return (ErrorMeterShape)generalPrefs.GetInt("hitErrorMeterShape");
	}

	public static bool GetSkipIntroAfterFirstTry()
	{
		return generalPrefs.GetBool("skipIntroAfterFirstTry", defaultValue: true);
	}

	public static MultitapTileBehavior GetMultitapTileBehavior()
	{
		return (MultitapTileBehavior)generalPrefs.GetInt("multitapTileBehavior");
	}

	public static HoldBehavior GetHoldBehavior()
	{
		return (HoldBehavior)generalPrefs.GetInt("holdBehavior");
	}

	public static bool GetFreeroamInvulnerability()
	{
		return generalPrefs.GetBool("freeroamInvuln");
	}

	public static bool GetMarkFloorWithComment()
	{
		return generalPrefs.GetBool("markFloorWithComment", defaultValue: true);
	}

	public static bool GetDisableRewindButton()
	{
		return generalPrefs.GetBool("disableRewindButton");
	}

	public static int GetShortcutPlaySpeed()
	{
		return generalPrefs.GetInt("shortcutPlaySpeed", 50);
	}

	public static int GetQuickScrubbedPlay()
	{
		return generalPrefs.GetInt("quickScrubbedPlay", 1000);
	}

	public static bool HasRatedGame()
	{
		return generalPrefs.GetBool("ratedGame");
	}

	public static bool HasSubscribedToFeatured(ulong levelId)
	{
		return customPrefs.GetBool("SubscribedToFeatured_" + levelId.ToString());
	}

	public static bool GetAnimateSpeedChange()
	{
		return generalPrefs.GetBool("animateSpeedChanges", defaultValue: true);
	}

	public static int GetLastAnalyticsUpdate()
	{
		return generalPrefs.GetInt("lastAnalyticsUpdate", -1);
	}

	public static int GetFXAA()
	{
		return generalPrefs.GetInt("antiAliasing", 1);
	}

	public static bool GetHideCursorWhilePlaying()
	{
		return generalPrefs.GetBool("hideCursorWhilePlaying");
	}

	public static bool GetChosenAsynchronousInput()
	{
		return generalPrefs.GetBool("useAsynchronousInput");
	}

	public static void SetPercentCompletion(int worldZeroIndexed, float pct)
	{
		generalPrefs.SetFloat("percentCompletion" + dd(worldZeroIndexed), pct);
	}

	public static void SetBestPercentAccuracy(int worldZeroIndex, float pct)
	{
		generalPrefs.SetFloat("bestPercentAccuracy" + dd(worldZeroIndex), pct);
	}

	public static void SetBestPercentXAccuracy(int worldZeroIndex, float pct)
	{
		generalPrefs.SetFloat("bestPercentXAccuracy" + dd(worldZeroIndex), pct);
	}

	public static void SetBestSpeedTrial(int worldZeroIndex, float spd)
	{
		string key = "bestSpeedMultiplier" + dd(worldZeroIndex);
		generalPrefs.SetFloat(key, spd);
	}

	public static void SetLevelTutorialProgress(int worldZeroIndexed, int level)
	{
		generalPrefs.SetInt("tutorialProgress" + dd(worldZeroIndexed), level);
	}

	public static void SetLevelTutorialProgress(string world, int level)
	{
		SetLevelTutorialProgress(GCNS.worldData[world].index, level);
	}

	public static void SetWorldAttempts(int worldZeroIndex, int attempts)
	{
		generalPrefs.SetInt("worldAttempts" + dd(worldZeroIndex), attempts);
	}

	public static void IncrementWorldAttempts(int worldZeroIndex)
	{
		int @int = generalPrefs.GetInt("worldAttempts" + dd(worldZeroIndex));
		@int++;
		generalPrefs.SetInt("worldAttempts" + dd(worldZeroIndex), @int);
		Save();
	}

	public static void SetNextRatingPromptDay(int nextDay)
	{
		generalPrefs.SetInt("nextRatingromptDay", nextDay);
	}

	public static void SetIsHighestPossibleAcc(int worldZeroIndex, bool isHighest)
	{
		generalPrefs.SetBool("isHighestPossibleAcc" + dd(worldZeroIndex), isHighest);
		Save();
	}

	public static void SetWorldAttemptsWithoutNewBest(int worldZeroIndex, int attempts)
	{
		generalPrefs.SetInt("worldAttemptsWithoutNewBest" + dd(worldZeroIndex), attempts);
	}

	public static void IncrementWorldAttemptsWithoutNewBest(int worldZeroIndex)
	{
		int @int = generalPrefs.GetInt("worldAttemptsWithoutNewBest" + dd(worldZeroIndex));
		@int++;
		generalPrefs.SetInt("worldAttemptsWithoutNewBest" + dd(worldZeroIndex), @int);
		Save();
	}

	public static void SetPassedMobileTutorial(bool passed)
	{
		generalPrefs.SetBool("passedTutorial", passed);
	}

	public static void SetGlobalVolume(int globalVolume)
	{
		generalPrefs.SetInt("globalVolume", globalVolume);
	}

	public static void SetLanguage(SystemLanguage language)
	{
		generalPrefs.SetString("language", language.ToString());
	}

	public static void SetInputOfset(float inputOffset)
	{
		generalPrefs.SetFloat("offset", inputOffset);
	}

	public static void SetPerfectsOnlyMode(bool perfectsOnlyMode)
	{
		generalPrefs.SetBool("perfectsOnlyMode", perfectsOnlyMode);
	}

	public static void SetVSync(int vs)
	{
		generalPrefs.SetInt("vSyncKey", vs);
	}

	public static void SetVisualQuality(VisualQuality quality)
	{
		RDBaseDll.printes($"setting visual quality to: {quality} ({(int)quality})");
		generalPrefs.SetInt("visualQuality", (int)quality);
	}

	public static void SetVisualEffects(VisualEffects effects)
	{
		generalPrefs.SetInt("visualEffects", (int)effects);
	}

	public static void SetTargetFramerate(int framerate)
	{
		RDClassDll.printem("updating target framerate to: " + framerate.ToString());
		Application.targetFrameRate = framerate;
		generalPrefs.SetInt("targetFramerate", framerate);
	}

	public static void SetShowDetailedResults(bool show)
	{
		generalPrefs.SetBool("showDetailedResults", show);
	}

	public static void SetVibration(bool enabled)
	{
		generalPrefs.SetBool("vibrate", enabled);
	}

	public static void SetShowRDOffer(bool offer)
	{
		generalPrefs.SetBool("showRDOffer", offer);
	}

	public static void SetSavedCurrentLevel(string currentLevel)
	{
		generalPrefs.SetString("currentLevel", currentLevel);
		Save();
	}

	public static void SetUnlockedXF(bool unlocked)
	{
		generalPrefs.SetBool("unlockedXtra", unlocked);
	}

	public static void SetUnlockedXC(bool unlocked)
	{
		generalPrefs.SetBool("unlockedXC", unlocked);
	}

	public static void SetUnlockedXH(bool unlocked)
	{
		generalPrefs.SetBool("unlockedXH", unlocked);
	}

	public static void SetUnlockedXR(bool unlocked)
	{
		generalPrefs.SetBool("unlockedXR", unlocked);
	}

	public static void SetUnlockedMD(bool unlocked)
	{
		generalPrefs.SetBool("unlockedMD", unlocked);
	}

	public static void SetPlayedFirst5WorldsCutscene(bool played)
	{
		generalPrefs.SetBool("playedFirst5WorldsCutscene", played);
	}

	public static void SetPlayedWorld6Cutscene(bool played)
	{
		generalPrefs.SetBool("keyPlayedWorld6Cutscene", played);
	}

	public static void SetPlayerColor(Color planetColor, bool red)
	{
		string text = "";
		text = ((planetColor == scrPlanet.goldColor) ? "gold" : ((planetColor == scrPlanet.rainbowColor) ? "rainbow" : ((planetColor == scrPlanet.transPinkColor) ? "transPink" : ((planetColor == scrPlanet.transBlueColor) ? "transBlue" : ((planetColor == scrPlanet.nbYellowColor) ? "nbYellow" : ((planetColor == scrPlanet.nbPurpleColor) ? "nbPurple" : ((!(planetColor == scrPlanet.overseerColor)) ? ColorUtility.ToHtmlStringRGB(planetColor) : "overseer")))))));
		generalPrefs.SetString(red ? "colorRed" : "colorBlue", text);
	}

	public static void SetFavoriteEditorEvents(List<LevelEventType> events)
	{
		List<object> list = new List<object>();
		foreach (LevelEventType @event in events)
		{
			list.Add(@event.ToString());
		}
		generalPrefs.SetList("editorFavoriteEvents", list);
	}

	public static void SetSamuraiMode(bool enabled, bool red)
	{
		generalPrefs.SetBool(red ? "samuraiRed" : "samuraiBlue", enabled);
	}

	public static int GetTaroStoryProgress()
	{
		return generalPrefs.GetInt("dlcStoryProgress");
	}

	public static void SetTaroStoryProgress(int value)
	{
		generalPrefs.SetInt("dlcStoryProgress", value);
	}

	public static int GetTaroEXProgress()
	{
		return generalPrefs.GetInt("dlcEXProgress");
	}

	public static void SetTaroEXProgress(int value)
	{
		generalPrefs.SetInt("dlcEXProgress", value);
	}

	public static void ResetMedalsForDLCLevel(string taroWorld)
	{
		generalPrefs.SetString("dlcMedals" + taroWorld, "");
	}

	public static int[] GetMedalsForDLCLevel(string taroWorld)
	{
		string @string = generalPrefs.GetString("dlcMedals" + taroWorld);
		int num = GCNS.dlcMedalsCount[taroWorld];
		int[] array = new int[num];
		if (!string.IsNullOrEmpty(@string) && @string.Length == num)
		{
			for (int i = 0; i < num; i++)
			{
				array[i] = (int)char.GetNumericValue(@string[i]);
			}
		}
		return array;
	}

	public static void SetMedalsForDLCLevel(string taroWorld, int[] medals)
	{
		int num = GCNS.dlcMedalsCount[taroWorld];
		if (medals.Length != num)
		{
			UnityEngine.Debug.LogError("medals count should be " + num.ToString() + "!");
			return;
		}
		char[] array = new char[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = medals[i].ToString()[0];
		}
		string text = new string(array);
		RDClassDll.printes("set medal string for " + taroWorld + " to " + text);
		generalPrefs.SetString("dlcMedals" + taroWorld, text);
	}

	public static void SetMedalsForDLCLevel(string taroWorld, string medalsString)
	{
		int num = GCNS.dlcMedalsCount[taroWorld];
		if (medalsString.Length != num)
		{
			UnityEngine.Debug.LogError("medals count should be " + num.ToString() + "!");
			return;
		}
		RDClassDll.printes("set medal string for " + taroWorld + " to " + medalsString);
		generalPrefs.SetString("dlcMedals" + taroWorld, medalsString);
	}

	public static float GetT5BestTime()
	{
		return generalPrefs.GetFloat("dlcT5Time", 60f);
	}

	public static void SetT5BestTime(float value)
	{
		generalPrefs.SetFloat("dlcT5Time", value);
	}

	public static void SetFaceMode(bool enabled, bool red)
	{
		generalPrefs.SetBool(red ? "faceRed" : "faceBlue", enabled);
	}

	public static void SetInputOffset(float offset)
	{
		generalPrefs.SetFloat("offset", offset);
	}

	public static void SetVisualOffset(float offset)
	{
		PlayerPrefs.SetFloat("offset_v", offset);
	}

	public static void SetEditorScale(float scale)
	{
		PlayerPrefs.SetFloat("editorScaleY", scale);
	}

	public static float GetEditorScale()
	{
		return PlayerPrefs.GetFloat("editorScaleY", 800f);
	}

	public static void SetShowXAccuracy(bool enabled)
	{
		generalPrefs.SetBool("showXAccuracy", enabled);
	}

	public static void SetHitErrorMeterSize(ErrorMeterSize size)
	{
		generalPrefs.SetInt("hitErrorMeterSize", (int)size);
	}

	public static void SetHitErrorMeterShape(ErrorMeterShape shape)
	{
		generalPrefs.SetInt("hitErrorMeterShape", (int)shape);
	}

	public static void SetSkipIntroAfterFirstTry(bool enabled)
	{
		generalPrefs.SetBool("skipIntroAfterFirstTry", enabled);
	}

	public static void SetMultitapTileBehavior(MultitapTileBehavior behavior)
	{
		generalPrefs.SetInt("multitapTileBehavior", (int)behavior);
	}

	public static void SetHoldBehavior(HoldBehavior behavior)
	{
		generalPrefs.SetInt("holdBehavior", (int)behavior);
	}

	public static void SetFreeroamInvulnerability(bool enabled)
	{
		generalPrefs.SetBool("freeroamInvuln", enabled);
	}

	public static void SetMarkFloorWithComment(bool enabled)
	{
		generalPrefs.SetBool("markFloorWithComment", enabled);
	}

	public static void SetDisableRewindButton(bool enabled)
	{
		generalPrefs.SetBool("disableRewindButton", enabled);
	}

	public static int SetShortcutPlaySpeed(int pitch)
	{
		pitch = Mathf.Clamp(pitch, 1, 1000);
		generalPrefs.SetInt("shortcutPlaySpeed", pitch);
		return pitch;
	}

	public static void SetQuickScrubbedPlay(int timing)
	{
		generalPrefs.SetInt("quickScrubbedPlay", timing);
	}

	public static void SetLastAnalyticsUpdate(int day)
	{
		generalPrefs.SetInt("lastAnalyticsUpdate", day);
	}

	public static bool IsWorldComplete(int worldZeroIndex)
	{
		return GetPercentCompletion(worldZeroIndex) >= 1f;
	}

	public static bool IsWorldComplete(string world)
	{
		return GetPercentCompletion(GCNS.worldData[world].index) >= 1f;
	}

	public static void SetPercentCompletion(string world, float pct)
	{
		int index = GCNS.worldData[world].index;
		generalPrefs.SetFloat("percentCompletion" + dd(index), pct);
	}

	public static bool IsWorldPerfect(int worldZeroIndex)
	{
		return GetBestPercentAccuracy(worldZeroIndex) >= 1f;
	}

	public static bool IsSpeedTrialComplete(int worldZeroIndex)
	{
		string world = null;
		foreach (KeyValuePair<string, GCNS.WorldData> worldDatum in GCNS.worldData)
		{
			GCNS.WorldData value = worldDatum.Value;
			if (worldDatum.Value.index == worldZeroIndex)
			{
				world = worldDatum.Key;
				break;
			}
		}
		float num = GetSpeedTrialAimForWorld(world) - 0.01f;
		return GetBestSpeedMultiplier(worldZeroIndex) > num;
	}

	public static float GetSpeedTrialAimForWorld(string world)
	{
		return GCNS.worldData[world].trialAim;
	}

	public static bool ShouldShowSpeedTrials()
	{
		return GetOverallProgressStage() >= 5;
	}

	public static bool hasBeatBonusLevel()
	{
		return IsWorldComplete(bonusWorldIndex);
	}

	public static void SetAcceptedAgreement(bool accepted)
	{
		generalPrefs.SetBool("acceptedAgreement", accepted);
	}

	public static void SetDisplayedCLSIntro(bool displayed)
	{
		generalPrefs.SetBool("displayedCLSIntro", displayed);
	}

	public static void SetCLSSortingParameter(OptionsPanelsCLS.OptionName sortingParameter)
	{
		generalPrefs.SetInt("CLSSortingParameter", (int)sortingParameter);
	}

	public static void SetCLSSortingReversed(bool sortingReversed)
	{
		generalPrefs.SetBool("CLSSortingReversed", sortingReversed);
	}

	public static void SetRatedGame(bool rated)
	{
		generalPrefs.SetBool("ratedGame", rated);
	}

	public static void SetSubscribedToFeatured(ulong levelId, bool subscribed)
	{
		customPrefs.SetBool("SubscribedToFeatured_" + levelId.ToString(), subscribed);
	}

	public static void SetAnimateSpeedChanges(bool animate)
	{
		generalPrefs.SetBool("animateSpeedChanges", animate);
	}

	public static void SetFXAA(int samples)
	{
		generalPrefs.SetInt("antiAliasing", samples);
	}

	public static void SetHideCursorWhilePlaying(bool hide)
	{
		generalPrefs.SetBool("hideCursorWhilePlaying", hide);
	}

	public static void SetChosenAsynchronousInput(bool enabled)
	{
		generalPrefs.SetBool("useAsynchronousInput", enabled);
	}

	public static Dictionary<string, object> GetSavedProgress()
	{
		return generalPrefs.GetDict("savedProgress");
	}

	public static void SetSavedProgress(Dictionary<string, object> dict)
	{
		generalPrefs.SetDict("savedProgress", dict);
	}

	public static void DeleteSavedProgress()
	{
		generalPrefs.RemoveKey("savedProgress");
	}

	public static float GetCustomWorldCompletion(string hash)
	{
		return customPrefs.GetFloat("CustomWorld_" + hash + "_Completion");
	}

	public static int GetCustomWorldAttempts(string hash)
	{
		return customPrefs.GetInt("CustomWorld_" + hash + "_Attempts");
	}

	public static float GetCustomWorldAccuracy(string hash)
	{
		return customPrefs.GetFloat("CustomWorld_" + hash + "_Accuracy");
	}

	public static float GetCustomWorldXAccuracy(string hash)
	{
		return customPrefs.GetFloat("CustomWorld_" + hash + "_XAccuracy");
	}

	public static float GetCustomWorldSpeedTrial(string hash)
	{
		return customPrefs.GetFloat("CustomWorld_" + hash + "_SpeedTrial", 1f);
	}

	public static int GetCustomWorldMinDeaths(string hash)
	{
		return customPrefs.GetInt("CustomWorld_" + hash + "_MinDeaths", -1);
	}

	public static int GetCustomWorldPlayIndex(string hash)
	{
		return customPrefs.GetInt("CustomWorld_" + hash + "_PlayIndex", -1);
	}

	public static bool GetCustomWorldIsHighestPossibleAcc(string hash)
	{
		return customPrefs.GetBool("CustomWorld_" + hash + "_isHighestPossibleAcc");
	}

	public static int GetCLSTotalPlays()
	{
		return customPrefs.GetInt("CLSTotalPlays");
	}

	public static void SetCustomWorldCompletion(string hash, float completion)
	{
		customPrefs.SetFloat("CustomWorld_" + hash + "_Completion", completion);
	}

	public static void SetCustomWorldAttempts(string hash, int attempts)
	{
		customPrefs.SetInt("CustomWorld_" + hash + "_Attempts", attempts);
	}

	public static void SetCustomWorldAccuracy(string hash, float accuracy)
	{
		customPrefs.SetFloat("CustomWorld_" + hash + "_Accuracy", accuracy);
	}

	public static void SetCustomWorldXAccuracy(string hash, float accuracy)
	{
		customPrefs.SetFloat("CustomWorld_" + hash + "_XAccuracy", accuracy);
	}

	public static void SetCustomWorldSpeedTrial(string hash, float multiplier)
	{
		customPrefs.SetFloat("CustomWorld_" + hash + "_SpeedTrial", multiplier);
	}

	public static void SetCustomWorldMinDeaths(string hash, int deaths)
	{
		customPrefs.SetInt("CustomWorld_" + hash + "_MinDeaths", deaths);
	}

	public static void SetCustomWorldPlayIndex(string hash, int playIndex)
	{
		customPrefs.SetInt("CustomWorld_" + hash + "_PlayIndex", playIndex);
	}

	public static void SetCustomWorldIsHighestPossibleAcc(string hash, bool isHighest)
	{
		customPrefs.SetBool("CustomWorld_" + hash + "_isHighestPossibleAcc", isHighest);
	}

	public static void SetCLSTotalPlays(int totalPlays)
	{
		customPrefs.SetInt("CLSTotalPlays", totalPlays);
	}

	public static void IncrementCustomWorldAttempts(string hash)
	{
		int attempts = GetCustomWorldAttempts(hash) + 1;
		SetCustomWorldAttempts(hash, attempts);
		Save();
	}

	public static void IncrementCLSTotalPlays()
	{
		SetCLSTotalPlays(GetCLSTotalPlays() + 1);
		Save();
	}

	public static string GetWorldAchievementPrefix(string world)
	{
		int index = GCNS.worldData[world].index;
		string str = (index > 12) ? world : index.ToString();
		return "World" + str;
	}

	public static void GiveAchievements()
	{
		SteamIntegration instance = SteamIntegration.Instance;
		if (!instance.initialized)
		{
			return;
		}
		string[] allWorlds = GCNS.allWorlds;
		foreach (string text in allWorlds)
		{
			if (!(text == "B"))
			{
				int index = GCNS.worldData[text].index;
				string worldAchievementPrefix = GetWorldAchievementPrefix(text);
				if (IsWorldComplete(index))
				{
					instance.UnlockAchievementWithName(worldAchievementPrefix + "Complete");
				}
				if (IsWorldPerfect(index))
				{
					instance.UnlockAchievementWithName(worldAchievementPrefix + "Perfect");
				}
				if (IsSpeedTrialComplete(index))
				{
					instance.UnlockAchievementWithName(worldAchievementPrefix + "Trial");
				}
			}
		}
		bool flag = true;
		allWorlds = GCNS.dlcWorlds;
		foreach (string text2 in allWorlds)
		{
			if (text2.EndsWith("EX") && !IsWorldComplete(GCNS.worldData[text2].index))
			{
				flag = false;
				break;
			}
		}
		bool flag2 = true;
		bool flag3 = true;
		allWorlds = GCNS.xtraWorlds;
		foreach (string key in allWorlds)
		{
			int index2 = GCNS.worldData[key].index;
			if (!IsWorldComplete(index2))
			{
				flag3 = false;
			}
			if (!IsSpeedTrialComplete(index2))
			{
				flag2 = false;
			}
		}
		bool flag4 = true;
		allWorlds = GCNS.museDashWorlds;
		foreach (string text3 in allWorlds)
		{
			if (!(text3 == "MO") && !IsWorldComplete(text3))
			{
				flag4 = false;
				break;
			}
		}
		if (flag3)
		{
			instance.UnlockAchievementWithName("XtraComplete");
		}
		if (flag2)
		{
			instance.UnlockAchievementWithName("XtraTrial");
		}
		if (flag4)
		{
			instance.UnlockAchievementWithName("MuseDashComplete");
		}
		if (flag)
		{
			instance.UnlockAchievementWithName("NeoCosmosEXComplete");
		}
		if (IsWorldComplete(6))
		{
			instance.UnlockAchievementWithName("BonusComplete");
		}
		if (GetOverallProgressStage() >= 9)
		{
			instance.UnlockAchievementWithName("Game100PercentComplete");
		}
		SteamUserStats.StoreStats();
	}

	public static void ClearData()
	{
		RDBaseDll.printes("cleared data");
		GCS.maxLevel = 0;
		generalPrefs.SetInt("maxlevel", 0);
		generalPrefs.SetInt("currentLevel", 0);
		SetPassedMobileTutorial(passed: false);
		SetSavedCurrentLevel("0-0");
		SetUnlockedXF(unlocked: false);
		SetUnlockedXC(unlocked: false);
		SetUnlockedXH(unlocked: false);
		SetUnlockedXR(unlocked: false);
		SetUnlockedMD(unlocked: false);
		SetPlayedFirst5WorldsCutscene(played: false);
		SetPlayedWorld6Cutscene(played: false);
		SetAcceptedAgreement(accepted: false);
		generalPrefs.dict.Remove("colorRed");
		generalPrefs.dict.Remove("colorBlue");
		string[] allWorlds = GCNS.allWorlds;
		for (int i = 0; i < allWorlds.Length; i++)
		{
			ResetWorldProgress(allWorlds[i]);
		}
		GCS.worldEntrance = null;
		GCS.lastLevelPlayed = 0;
		GCS.checkpointNum = 0;
		scrController.currentWorldString = null;
		ResetTaroStoryProgress();
		Save();
	}

	public static void ClearDataAll()
	{
		SaveFileType[] loadedSaveFileTypes = PlayerPrefsJson.LoadedSaveFileTypes;
		for (int i = 0; i < loadedSaveFileTypes.Length; i++)
		{
			PlayerPrefsJson.Select(loadedSaveFileTypes[i])?.dict.Clear();
		}
		Save();
	}

	public static void ResetTaroStoryProgress()
	{
		RDClassDll.printem("RESET TARO STORY PROGRESS");
		SetTaroStoryProgress(0);
		SetTaroEXProgress(0);
		string[] dlcWorlds = GCNS.dlcWorlds;
		for (int i = 0; i < dlcWorlds.Length; i++)
		{
			ResetWorldProgress(dlcWorlds[i]);
		}
		Save();
		RDClassDll.printem($"Taro Story Progress: {GetTaroStoryProgress()}, Taro EX Progress: {GetTaroEXProgress()}");
	}

	public static string GetLastUsedFolder()
	{
		return PlayerPrefs.GetString("lastUsedFolder", DataPath);
	}

	public static void UpdateLastUsedFolder(string levelPath)
	{
		PlayerPrefs.SetString("lastUsedFolder", Path.GetDirectoryName(levelPath));
	}

	public static string GetLastOpenedLevel()
	{
		return PlayerPrefs.GetString("lastOpenedLevel", "");
	}

	public static void UpdateLastOpenedLevel(string levelPath)
	{
		PlayerPrefs.SetString("lastOpenedLevel", levelPath);
	}

	public static int GetOverallProgressStage()
	{
		int num = -1;
		if (GetSavedCurrentLevel() != "1-1" && GetSavedCurrentLevel() != "0-0")
		{
			num = 0;
		}
		if (IsWorldComplete(0))
		{
			num = 1;
		}
		if (IsWorldComplete(0) && IsWorldComplete(1) && IsWorldComplete(2) && IsWorldComplete(3) && IsWorldComplete(4))
		{
			num = 3;
		}
		if (IsWorldComplete(5))
		{
			num = 5;
		}
		bool num2 = IsSpeedTrialComplete(0) && IsSpeedTrialComplete(1) && IsSpeedTrialComplete(2) && IsSpeedTrialComplete(3) && IsSpeedTrialComplete(4) && IsSpeedTrialComplete(5);
		if (num2)
		{
			num = 7;
		}
		if (num2 && IsWorldComplete(bonusWorldIndex))
		{
			num = 8;
		}
		if (IsWorldPerfect(0) && IsWorldPerfect(1) && IsWorldPerfect(2) && IsWorldPerfect(3) && IsWorldPerfect(4) && IsWorldPerfect(5) && IsWorldComplete(bonusWorldIndex))
		{
			num = 9;
		}
		if (num == 9 && GetIsHighestPossibleAcc(0) && GetIsHighestPossibleAcc(1) && GetIsHighestPossibleAcc(2) && GetIsHighestPossibleAcc(3) && GetIsHighestPossibleAcc(4) && GetIsHighestPossibleAcc(5) && GetIsHighestPossibleAcc(bonusWorldIndex))
		{
			num = 10;
		}
		return num;
	}

	public static void CompleteFirst()
	{
		ResetAllWorldsProgress();
		CompleteWorld(0);
		GiveAchievementsAndSave();
	}

	public static void SaveMaxLevel(int currentLevel)
	{
		GCS.maxLevel = Mathf.Max(GCS.maxLevel, currentLevel);
		generalPrefs.SetInt("maxlevel", GCS.maxLevel);
		Save();
	}

	public static void CompleteWorld(int worldZeroIndexed, bool includingSpeedTrials = false, bool goldenLantern = false)
	{
		SetPercentCompletion(worldZeroIndexed, 1f);
		SetBestSpeedTrial(worldZeroIndexed, 1f);
		SetBestPercentAccuracy(worldZeroIndexed, goldenLantern ? 1.1f : 0.9f);
		SetBestPercentXAccuracy(worldZeroIndexed, goldenLantern ? 1f : 0.85f);
		if (goldenLantern)
		{
			SetIsHighestPossibleAcc(worldZeroIndexed, isHighest: true);
		}
		SetWorldAttempts(worldZeroIndexed, 1);
		SetWorldAttemptsWithoutNewBest(worldZeroIndexed, 0);
		SetLevelTutorialProgress(worldZeroIndexed, 99);
		if (includingSpeedTrials)
		{
			SetBestSpeedTrial(worldZeroIndexed, 2f);
		}
	}

	public static void ResetWorldProgress(string world)
	{
		int index = GCNS.worldData[world].index;
		SetPercentCompletion(index, 0f);
		SetBestSpeedTrial(index, 0f);
		SetBestPercentAccuracy(index, 0f);
		SetBestPercentXAccuracy(index, 0f);
		SetIsHighestPossibleAcc(index, isHighest: false);
		SetWorldAttempts(index, 0);
		SetWorldAttemptsWithoutNewBest(index, 0);
		SetLevelTutorialProgress(index, 0);
		if (world.IsTaro())
		{
			ResetMedalsForDLCLevel(world);
		}
	}

	public static void ResetAllWorldsProgress()
	{
		string[] allWorlds = GCNS.allWorlds;
		for (int i = 0; i < allWorlds.Length; i++)
		{
			ResetWorldProgress(allWorlds[i]);
		}
	}

	public static void CompleteAllMainLevels()
	{
		ResetAllWorldsProgress();
		for (int i = 0; i < 6; i++)
		{
			CompleteWorld(i);
		}
		GiveAchievementsAndSave();
	}

	public static void CompleteAllMainLevelsAndSpeedTrials()
	{
		ResetAllWorldsProgress();
		for (int i = 0; i < 6; i++)
		{
			CompleteWorld(i, includingSpeedTrials: true);
		}
		GiveAchievementsAndSave();
	}

	public static void CompleteFirst5()
	{
		ResetAllWorldsProgress();
		for (int i = 0; i < 5; i++)
		{
			CompleteWorld(i, includingSpeedTrials: true);
		}
		GiveAchievementsAndSave();
	}

	public static void CompleteAllWorlds()
	{
		string[] allWorlds = GCNS.allWorlds;
		foreach (string key in allWorlds)
		{
			CompleteWorld(GCNS.worldData[key].index, includingSpeedTrials: true);
		}
		GiveAchievementsAndSave();
	}

	public static void Complete100()
	{
		string[] allWorlds = GCNS.allWorlds;
		foreach (string key in allWorlds)
		{
			CompleteWorld(GCNS.worldData[key].index, includingSpeedTrials: true);
		}
		SetUnlockedXF(unlocked: true);
		SetUnlockedXC(unlocked: true);
		SetUnlockedXH(unlocked: true);
		SetUnlockedXR(unlocked: true);
		GiveAchievementsAndSave();
	}

	public static void UnlockXtra()
	{
		SetUnlockedXF(unlocked: true);
		GiveAchievementsAndSave();
	}

	public static void Load()
	{
		if (!PlayerPrefsJson.SaveFileExists(SaveFileType.General))
		{
			MigrateToJSON();
			UnityEngine.Debug.Log("Migrated save data to JSON.");
		}
		bool flag = true;
		SaveFileType[] allSaveFileTypes = PlayerPrefsJson.AllSaveFileTypes;
		foreach (SaveFileType saveFileType in allSaveFileTypes)
		{
			Dictionary<string, object> fileContent2;
			if (PlayerPrefsJson.LoadFile(saveFileType, loadBackup: false, out Dictionary<string, object> fileContent))
			{
				PlayerPrefsJson playerPrefsJson = new PlayerPrefsJson(saveFileType, fileContent);
				PlayerPrefsJson.AddSaveFile(saveFileType, playerPrefsJson);
				playerPrefsJson.SaveBackup();
			}
			else if (PlayerPrefsJson.LoadFile(saveFileType, loadBackup: true, out fileContent2))
			{
				PlayerPrefsJson.MarkCorruptFile(saveFileType, isBackup: false);
				RDClassDll.printem("Main save file seems corrupted, attempting to load backup.");
				PlayerPrefsJson playerPrefsJson = new PlayerPrefsJson(saveFileType, fileContent2);
				PlayerPrefsJson.AddSaveFile(saveFileType, playerPrefsJson);
			}
			else
			{
				PlayerPrefsJson.MarkCorruptFile(saveFileType, isBackup: false);
				PlayerPrefsJson.MarkCorruptFile(saveFileType, isBackup: true);
				flag = false;
				PlayerPrefsJson data = PlayerPrefsJson.CreateSaveFile(saveFileType);
				PlayerPrefsJson.AddSaveFile(saveFileType, data);
			}
		}
		if (flag && generalPrefs != null && customPrefs != null)
		{
			string[] array = (from k in generalPrefs.dict.Keys
				where !customPrefs.dict.ContainsKey(k) && _003CLoad_003Eg__StartsWithAnyOf_007C265_0(k, new string[3]
				{
					"CLSTotalPlays",
					"CustomWorld_",
					"SubscribedToFeatured_"
				})
				select k).ToArray();
			foreach (string key in array)
			{
				customPrefs.dict.Add(key, generalPrefs.dict[key]);
				generalPrefs.dict.Remove(key);
			}
		}
		int @int = generalPrefs.GetInt("version");
		RDClassDll.printes($"release version is {@int}");
		if (generalPrefs.dict.ContainsKey("currentLevel") && (@int < 35 || generalPrefs.dict["currentLevel"] is int))
		{
			RDClassDll.printes("doing this thing");
			SetSavedCurrentLevel("0-0");
		}
		if (@int < 48)
		{
			RDClassDll.printes($"Reset world progress because release version is {@int}");
			SetUnlockedXF(unlocked: false);
			string[] array = GCNS.allWorlds;
			foreach (string text in array)
			{
				if (text.IsXtra())
				{
					ResetWorldProgress(text);
				}
			}
		}
		if (@int < 53 && flag)
		{
			Dictionary<string, object> dict2 = generalPrefs.dict;
			if (generalPrefs.GetList("calibrationPresets").Count == 0)
			{
				CalibrationPreset item = default(CalibrationPreset);
				item.confident = true;
				item.inputOffset = Mathf.RoundToInt(GetInputOffset() * 1000f);
				item.outputName = "*";
				item.outputType = AudioOutputType.Speaker;
				scrConductor.defaultPresets.Add(item);
				item.outputType = AudioOutputType.Wired;
				scrConductor.defaultPresets.Add(item);
			}
		}
		Dictionary<string, object> dict = generalPrefs.dict;
		if (dict.TryGetValue("faceBlue", out object value) && value is string)
		{
			dict["faceBlue"] = bool.Parse(value as string);
		}
		if (dict.TryGetValue("faceRed", out object value2) && value2 is string)
		{
			dict["faceRed"] = bool.Parse(value2 as string);
		}
		scrConductor.userPresets = new List<CalibrationPreset>();
		foreach (object item2 in generalPrefs.GetList("calibrationPresets"))
		{
			CalibrationPreset calibrationPreset = default(CalibrationPreset);
			calibrationPreset.confident = true;
			calibrationPreset.FromDict(item2 as Dictionary<string, object>);
			RDBaseDll.printes(calibrationPreset);
			scrConductor.userPresets.Add(calibrationPreset);
		}
	}

	public static void RecoverSaveDataFromSteamAchievements()
	{
		SteamIntegration instance = SteamIntegration.Instance;
		if (!SteamIntegration.Instance.initialized)
		{
			return;
		}
		bool flag = false;
		string[] allWorlds = GCNS.allWorlds;
		foreach (string text in allWorlds)
		{
			if (text == "B")
			{
				continue;
			}
			int index = GCNS.worldData[text].index;
			string worldAchievementPrefix = GetWorldAchievementPrefix(text);
			if (_003CRecoverSaveDataFromSteamAchievements_003Eg__GetAchieved_007C266_1(worldAchievementPrefix + "Complete") && GetPercentCompletion(index) < 1f)
			{
				_003CRecoverSaveDataFromSteamAchievements_003Eg__SetComplete_007C266_0(index);
			}
			if (_003CRecoverSaveDataFromSteamAchievements_003Eg__GetAchieved_007C266_1(worldAchievementPrefix + "Perfect") && flag && GetBestPercentAccuracy(index) < 1f)
			{
				SetBestPercentAccuracy(index, 1.1f);
				SetBestPercentXAccuracy(index, 1f);
			}
			if (_003CRecoverSaveDataFromSteamAchievements_003Eg__GetAchieved_007C266_1(worldAchievementPrefix + "Trial"))
			{
				float speedTrialAimForWorld = GetSpeedTrialAimForWorld(text);
				if (GetBestSpeedMultiplier(index) < speedTrialAimForWorld)
				{
					SetBestSpeedTrial(index, speedTrialAimForWorld);
				}
			}
		}
		if (_003CRecoverSaveDataFromSteamAchievements_003Eg__GetAchieved_007C266_1("BonusComplete"))
		{
			SetPercentCompletion(GCNS.worldData["B"].index, 1f);
		}
		if (_003CRecoverSaveDataFromSteamAchievements_003Eg__GetAchieved_007C266_1("Game100PercentComplete"))
		{
			for (int j = 0; j < 5; j++)
			{
				SetIsHighestPossibleAcc(j, isHighest: true);
			}
		}
		if (_003CRecoverSaveDataFromSteamAchievements_003Eg__GetAchieved_007C266_1("NeoCosmosEXComplete"))
		{
			SetTaroStoryProgress(7);
			SetTaroEXProgress(4);
			allWorlds = GCNS.dlcWorlds;
			foreach (string key in allWorlds)
			{
				int index2 = GCNS.worldData[key].index;
				if (GetPercentCompletion(index2) < 1f)
				{
					_003CRecoverSaveDataFromSteamAchievements_003Eg__SetComplete_007C266_0(index2);
				}
			}
		}
		else if (_003CRecoverSaveDataFromSteamAchievements_003Eg__GetAchieved_007C266_1("WorldT5Complete"))
		{
			SetTaroStoryProgress(7);
		}
		else if (_003CRecoverSaveDataFromSteamAchievements_003Eg__GetAchieved_007C266_1("WorldT4Complete"))
		{
			SetTaroStoryProgress(4);
		}
		else if (_003CRecoverSaveDataFromSteamAchievements_003Eg__GetAchieved_007C266_1("WorldT3Complete"))
		{
			SetTaroStoryProgress(3);
		}
		else if (_003CRecoverSaveDataFromSteamAchievements_003Eg__GetAchieved_007C266_1("WorldT1Complete") && _003CRecoverSaveDataFromSteamAchievements_003Eg__GetAchieved_007C266_1("WorldT2Complete"))
		{
			SetTaroStoryProgress(2);
		}
	}

	public static void GiveAchievementsAndSave()
	{
		if (!Application.isEditor)
		{
			GiveAchievements();
		}
		Save();
	}

	public static void Save()
	{
		List<object> list = new List<object>();
		foreach (CalibrationPreset userPreset in scrConductor.userPresets)
		{
			list.Add(userPreset.ToDict());
		}
		generalPrefs.SetList("calibrationPresets", list);
		PlayerPrefsJson playerPrefsJson = PlayerPrefsJson.SelectAll();
		playerPrefsJson.deltaDict.Add("version", 97);
		playerPrefsJson.ApplyDeltaDict();
		PlayerPrefsJson.SaveAllFiles();
	}

	public static string dd(int num)
	{
		return num.ToString("00");
	}

	public static void MigrateToJSON()
	{
		string[] array = new string[7]
		{
			"percentCompletion",
			"bestPercentAccuracy",
			"bestSpeedMultiplier",
			"worldAttempts",
			"worldAttemptsWithoutNewBest",
			"tutorialProgress",
			"isHighestPossibleAcc"
		};
		char[] array2 = new char[7]
		{
			'f',
			'f',
			'f',
			'i',
			'i',
			'i',
			'b'
		};
		string[] array3 = new string[9]
		{
			"currentLevel",
			"passedTutorial",
			"globalVolume",
			"vibrate",
			"visualQuality",
			"language",
			"vSyncKey",
			"offset",
			"offset_v"
		};
		char[] array4 = new char[9]
		{
			'i',
			'b',
			'i',
			'b',
			'i',
			's',
			'i',
			'f',
			'f'
		};
		int num = 0;
		string[] array5 = array;
		foreach (string str in array5)
		{
			for (int j = 0; j < 10; j++)
			{
				string fullKey = str + dd(j);
				char type = array2[num];
				_003CMigrateToJSON_003Eg__ConvertPlayerPrefsJsonEntryToDict_007C270_0(fullKey, type);
			}
			num++;
		}
		num = 0;
		array5 = array3;
		foreach (string fullKey2 in array5)
		{
			char type2 = array4[num];
			_003CMigrateToJSON_003Eg__ConvertPlayerPrefsJsonEntryToDict_007C270_0(fullKey2, type2);
			num++;
		}
		Save();
	}

	public static void SetOverallProgressStage(int i)
	{
	}

	[CompilerGenerated]
	private static bool _003CLoad_003Eg__StartsWithAnyOf_007C265_0(string str, string[] strings)
	{
		bool result = false;
		foreach (string value in strings)
		{
			if (str.StartsWith(value))
			{
				return true;
			}
		}
		return result;
	}

	[CompilerGenerated]
	private static void _003CRecoverSaveDataFromSteamAchievements_003Eg__SetComplete_007C266_0(int worldIndex)
	{
		SetWorldAttempts(worldIndex, 1);
		SetWorldAttemptsWithoutNewBest(worldIndex, 0);
		SetBestPercentAccuracy(worldIndex, 0.95f);
		SetBestPercentXAccuracy(worldIndex, 0.9f);
		SetLevelTutorialProgress(worldIndex, 99);
		SetPercentCompletion(worldIndex, 1f);
	}

	[CompilerGenerated]
	private static bool _003CRecoverSaveDataFromSteamAchievements_003Eg__GetAchieved_007C266_1(string achievementName)
	{
		SteamUserStats.GetAchievement(achievementName, out bool pbAchieved);
		return pbAchieved;
	}

	[CompilerGenerated]
	private static void _003CMigrateToJSON_003Eg__ConvertPlayerPrefsJsonEntryToDict_007C270_0(string fullKey, char type)
	{
		if (PlayerPrefs.HasKey(fullKey))
		{
			switch (type)
			{
			case 'f':
			{
				float @float = PlayerPrefs.GetFloat(fullKey);
				generalPrefs.SetFloat(fullKey, @float);
				break;
			}
			case 'i':
			{
				int @int = PlayerPrefs.GetInt(fullKey);
				generalPrefs.SetInt(fullKey, @int);
				break;
			}
			case 'b':
			{
				bool value = PlayerPrefs.GetInt(fullKey) == 1;
				generalPrefs.SetBool(fullKey, value);
				break;
			}
			case 's':
			{
				string @string = PlayerPrefs.GetString(fullKey);
				generalPrefs.SetString(fullKey, @string);
				break;
			}
			default:
				UnityEngine.Debug.LogWarning("wtf");
				break;
			}
		}
		else
		{
			RDClassDll.printes("key " + fullKey + " doesn't exist.");
		}
	}
}
