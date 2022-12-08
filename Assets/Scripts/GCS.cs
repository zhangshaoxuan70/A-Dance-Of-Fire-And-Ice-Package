using ADOFAI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GCS
{
	public const bool usingCheckpoints = true;

	public static int _checkpointNum;

	public static int checkpointBeforePractice;

	public static int savedCheckpointNum = 0;

	public static string worldEntrance = null;

	public static int lastLevelPlayed = 0;

	public static string lastVisitedScene = "";

	public static bool wasInCalibration = false;

	public static double longIntroThresholdSec = 2.0;

	public static int customInternalUseCheckpoint = 0;

	public static bool hasVisitedCalibration = false;

	public static bool perfectOnlyMode = false;

	public static bool speedTrialMode = false;

	public static bool speedTrialModeBeforePractice = false;

	public static float currentSpeedTrial = 1f;

	public static float nextSpeedRun = 1f;

	public static float speedRunBeforePractice = 1f;

	public static Difficulty difficulty = Difficulty.Normal;

	public static bool typingMode = false;

	public static bool practiceMode = false;

	public static int practiceLength = 20;

	public static float practiceSpeed = 0.75f;

	public static bool transMode = false;

	public static string steamBranchName;

	public const bool RandomValuesEnabled = false;

	public static string[] customLevelPaths = null;

	public static int customLevelIndex = 0;

	public static bool standaloneLevelMode = false;

	public static bool TARODEBUG = false;

	public static bool seenCutscene2_1a = false;

	public static bool seenCutscene2_1b = false;

	public static bool seenCutscene3_1 = false;

	public static bool seenCutscene4_2 = false;

	public static bool seenCutscene4_4 = false;

	public static bool seenCutscene4_6 = false;

	public static bool seenCutscene4_12 = false;

	public static bool seenCutsceneP_1 = false;

	public static bool seenCutscene5_1 = false;

	public static bool seenCutscene5_3 = false;

	public static bool seenCutscene5_4 = false;

	public static bool seenCutscene5_x = false;

	public static bool seenCutscene4b_1 = false;

	public static bool enableCutsceneT5 = false;

	public static bool banished = false;

	public static bool puzzle = false;

	public static bool staticPlanetColors = false;

	public static List<int> pauseMedalStatsCurrent;

	public static List<int> pauseMedalFloors;

	public static bool playDeathSound = true;

	public static bool playWilhelm = false;

	public static bool playHitSounds = true;

	public static bool playCountdownHatsAndCymbal = true;

	public static bool editorQuickPitchedPlaying = false;

	public static bool DisableAllZooming = true;

	public static bool turnOnBenchmarkMode = false;

	public static Dictionary<string, LevelEventInfo> levelEventsInfo;

	public static Dictionary<string, LevelEventInfo> settingsInfo;

	public static Dictionary<LevelEventType, Sprite> levelEventIcons;

	public static Dictionary<LevelEventCategory, Sprite> eventCategoryIcons;

	public static Dictionary<LevelEventType, string> levelEventTypeString = null;

	public static LevelEventType filteredEvent = LevelEventType.None;

	public static bool useNoFail;

	public static bool lofiVersion = false;

	public static bool webVersion = false;

	public static bool bb = false;

	public static int maxLevel;

	public static int maxCalibrationRank;

	public static string sceneToLoad;

	public static string previousScene;

	public static bool d_vibrate = true;

	public static bool d_dontShowTitles = false;

	public static bool d_recording = false;

	public static bool d_forceGoldPlanets = false;

	public static bool d_bars = true;

	public static bool d_customlevel = false;

	public static bool d_stationary = false;

	public static bool d_calibration = true;

	public static bool d_candie;

	public static bool d_skiptolevel = true;

	public static bool d_freeroam = false;

	public static bool d_flash = true;

	public static bool d_loop = true;

	public static bool d_kong = false;

	public static bool d_newgrounds = false;

	public static bool d_attractmode = true;

	public static bool d_booth = false;

	public static bool d_drumcontroller = false;

	public static bool d_boothDisablePossibleMessUpButtons = false;

	public static bool d_judges = false;

	public static bool d_backgrounds = true;

	public static bool d_checkpoints = true;

	public static bool d_hitsounds = true;

	public static bool d_chinese = false;

	public static bool d_oldConductor = false;

	public static bool d_webglConductor = false;

	public static bool webHasSetResolution = false;

	public static bool webGL = false;

	public const float minSpeedrunSpeed = 0.5f;

	public static bool streamMP3Loading = true;

	public static bool d_customhitmargins = false;

	public static float HITMARGIN_COUNTED = 60f;

	public const float HITMARGIN_PERFECT = 45f;

	public const float HITMARGIN_PURE = 30f;

	public const float HITMARGIN_MINIMUM_SECONDS_HARD = 0.04f;

	public const float HITMARGIN_MINIMUM_SECONDS_NORMAL = 0.065f;

	public const float HITMARGIN_MINIMUM_SECONDS_EASY = 0.091f;

	public const float LenientOptionMinimumBPM_Official = 340f;

	public const float LenientOptionMinimumBPM_Custom = 220f;

	public const float StrictOptionMinimumBPM_Custom = 310f;

	public const float HITMARGIN_PERFECT_MINIMUM_SECONDS = 0.03f;

	public const float HITMARGIN_PURE_MINIMUM_SECONDS = 0.02f;

	public const float HITMARGIN_ABSOLUTE_MINIMUM_SECONDS = 0.025f;

	public const float HITMARGIN_MINIMUM_SECONDS_MOBILE = 0.09f;

	public const float HITMARGIN_PERFECT_MINIMUM_SECONDS_MOBILE = 0.07f;

	public const float HITMARGIN_PURE_MINIMUM_SECONDS_MOBILE = 0.05f;

	public const float multiplierIncrement = 0.1f;

	public const int multipressOverload = 8;

	public const float frameTimeSensitivity = 0.5f;

	public const float framesPerTileLimit = 3.5f;

	public const float msPerTileLimit = 40f;

	public const float msPerTileConsecLimit = 20f;

	public const float semitone = 1.05946314f;

	public static readonly string[] SupportedAudioFiles = new string[5]
	{
		"mp3",
		"wav",
		"ogg",
		"aiff",
		"aif"
	};

	public const string ExternalSoundSuffix = "*external";

	public const string OggVorbisFileExtension = "ogg";

	public const string MP3FileExtension = "mp3";

	public const string WavFileExtension = "wav";

	public const string AiffFileExtension = "aiff";

	public const string AifFileExtension = "aif";

	public static readonly string[] SupportedImageFiles = new string[4]
	{
		"jpeg",
		"jpg",
		"png",
		"gif"
	};

	public const string JPEGFileExtension = "jpeg";

	public const string JPGFileExtension = "jpg";

	public const string PNGFileExtension = "png";

	public const string GIFFileExtension = "gif";

	public static readonly string[] SupportedVideoFiles = new string[5]
	{
		"mp4",
		"avi",
		"mov",
		"wmv",
		"flv"
	};

	public const string MP4FileExtension = "mp4";

	public const string AVIFileExtension = "avi";

	public const string MOVFileExtension = "mov";

	public const string WMVFileExtension = "wmv";

	public const string FLVFileExtension = "flv";

	public const string zipExtension = "zip";

	public const string levelExtension = "adofai";

	public const string levelZipExtension = "adofaizip";

	public static readonly string[] levelZipExtensions = new string[2]
	{
		"adofaizip",
		"zip"
	};

	public const string TapjoyEventCategorySceneOpened = "SceneOpened";

	public static readonly KeyCode[] SpecialKeys = new KeyCode[22]
	{
		KeyCode.Print,
		KeyCode.SysReq,
		KeyCode.LeftAlt,
		KeyCode.RightAlt,
		KeyCode.LeftWindows,
		KeyCode.RightWindows,
		KeyCode.LeftMeta,
		KeyCode.RightMeta,
		KeyCode.Tab,
		KeyCode.End,
		KeyCode.F1,
		KeyCode.F2,
		KeyCode.F3,
		KeyCode.F4,
		KeyCode.F5,
		KeyCode.F6,
		KeyCode.F7,
		KeyCode.F8,
		KeyCode.F9,
		KeyCode.F10,
		KeyCode.F11,
		KeyCode.F12
	};

	public static readonly KeyCode[] LevelSelectKeys = new KeyCode[10]
	{
		KeyCode.BackQuote,
		KeyCode.Alpha0,
		KeyCode.Alpha1,
		KeyCode.Alpha2,
		KeyCode.Alpha3,
		KeyCode.Alpha4,
		KeyCode.Alpha5,
		KeyCode.Alpha6,
		KeyCode.Alpha7,
		KeyCode.AltGr
	};

	public static readonly KeyCode[] CLSKeys = new KeyCode[9]
	{
		KeyCode.UpArrow,
		KeyCode.DownArrow,
		KeyCode.R,
		KeyCode.S,
		KeyCode.Delete,
		KeyCode.I,
		KeyCode.F,
		KeyCode.O,
		KeyCode.N
	};

	public static readonly KeyCode[] joystickButtons = new KeyCode[20]
	{
		KeyCode.JoystickButton0,
		KeyCode.JoystickButton1,
		KeyCode.JoystickButton2,
		KeyCode.JoystickButton3,
		KeyCode.JoystickButton4,
		KeyCode.JoystickButton5,
		KeyCode.JoystickButton6,
		KeyCode.JoystickButton7,
		KeyCode.JoystickButton8,
		KeyCode.JoystickButton9,
		KeyCode.JoystickButton10,
		KeyCode.JoystickButton11,
		KeyCode.JoystickButton12,
		KeyCode.JoystickButton13,
		KeyCode.JoystickButton14,
		KeyCode.JoystickButton15,
		KeyCode.JoystickButton16,
		KeyCode.JoystickButton17,
		KeyCode.JoystickButton18,
		KeyCode.JoystickButton19
	};

	public static int checkpointNum
	{
		get
		{
			return _checkpointNum;
		}
		set
		{
			_checkpointNum = value;
		}
	}

	public static bool IsDev()
	{
		if (Application.isEditor || GCNS.devBranches.Contains(steamBranchName))
		{
			return true;
		}
		return new long[38]
		{
			235383080884371456L,
			213313185544011786L,
			129108330026237952L,
			214416049230184448L,
			164425061128863745L,
			133207411203768321L,
			191321415914356738L,
			260047393934934016L,
			301985049140789258L,
			207567174351454208L,
			159404252651978753L,
			273645812184121344L,
			137002336542392320L,
			166302763200937994L,
			158318575017394177L,
			309104296362901505L,
			192083644062367745L,
			331846965530853378L,
			201091631795929089L,
			197664924363653121L,
			157972666790182912L,
			300050030923087872L,
			125330806557114369L,
			292673382816677908L,
			110858211615064064L,
			97281704443539456L,
			280232033261715466L,
			294492604123316224L,
			290582109750427648L,
			328571392972947458L,
			243713342487789578L,
			185549848672600064L,
			365948766991155202L,
			574607911486226442L,
			69464262786953216L,
			199485066684923904L,
			543672901585469441L,
			305547529192472577L
		}.Contains(DiscordController.currentUserID);
	}
}
