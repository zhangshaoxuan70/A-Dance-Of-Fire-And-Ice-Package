using System.Collections.Generic;
using UnityEngine;

public class GCNS
{
	public struct WorldData
	{
		public int index;

		public int levelCount;

		public bool hasCheckpoints;

		public float trialAim;

		public Vector2Int jumpPortalPosition;

		public Vector2Int trialPortalPosition;

		public WorldData(int index, int levelCount, float trialAim, bool hasCheckpoints, Vector2Int jumpPortalPosition, Vector2Int trialPortalPosition)
		{
			this.index = index;
			this.levelCount = levelCount;
			this.trialAim = trialAim;
			this.hasCheckpoints = hasCheckpoints;
			this.jumpPortalPosition = jumpPortalPosition;
			this.trialPortalPosition = trialPortalPosition;
		}
	}

	public const int releaseNumber = 97;

	public static string buildDate;

	public static readonly Vector2Int XtraPortal = new Vector2Int(0, 11);

	public static readonly Vector2Int CrownPortal = new Vector2Int(0, 23);

	public static readonly Vector2Int MuseDashPortal = new Vector2Int(-25, 23);

	public static Dictionary<string, WorldData> worldData = new Dictionary<string, WorldData>
	{
		["1"] = new WorldData(0, 7, 1.5f, hasCheckpoints: false, new Vector2Int(13, -2), new Vector2Int(14, -2)),
		["2"] = new WorldData(1, 4, 1.5f, hasCheckpoints: false, new Vector2Int(20, -2), new Vector2Int(21, -2)),
		["3"] = new WorldData(2, 7, 1.5f, hasCheckpoints: false, new Vector2Int(27, -2), new Vector2Int(28, -2)),
		["4"] = new WorldData(3, 6, 1.5f, hasCheckpoints: false, new Vector2Int(34, -2), new Vector2Int(35, -2)),
		["5"] = new WorldData(4, 5, 1.5f, hasCheckpoints: false, new Vector2Int(41, -2), new Vector2Int(42, -2)),
		["6"] = new WorldData(5, 5, 1.1f, hasCheckpoints: false, new Vector2Int(48, -2), new Vector2Int(49, -2)),
		["7"] = new WorldData(7, 6, 1.3f, hasCheckpoints: false, new Vector2Int(13, -6), new Vector2Int(14, -6)),
		["8"] = new WorldData(8, 9, 1.3f, hasCheckpoints: false, new Vector2Int(20, -6), new Vector2Int(21, -6)),
		["9"] = new WorldData(9, 6, 1.3f, hasCheckpoints: false, new Vector2Int(27, -6), new Vector2Int(28, -6)),
		["10"] = new WorldData(10, 9, 1.2f, hasCheckpoints: false, new Vector2Int(34, -6), new Vector2Int(35, -6)),
		["11"] = new WorldData(11, 7, 1.2f, hasCheckpoints: true, new Vector2Int(41, -6), new Vector2Int(42, -6)),
		["12"] = new WorldData(12, 7, 1.2f, hasCheckpoints: false, new Vector2Int(48, -6), new Vector2Int(49, -6)),
		["B"] = new WorldData(6, 2, 0f, hasCheckpoints: true, new Vector2Int(55, -2), new Vector2Int(56, -2)),
		["XF"] = new WorldData(50, 4, 1.3f, hasCheckpoints: false, XtraPortal, XtraPortal),
		["XO"] = new WorldData(51, 3, 1f, hasCheckpoints: true, CrownPortal, CrownPortal),
		["XC"] = new WorldData(52, 6, 1.2f, hasCheckpoints: false, XtraPortal, XtraPortal),
		["XH"] = new WorldData(53, 4, 1.2f, hasCheckpoints: true, XtraPortal, XtraPortal),
		["PA"] = new WorldData(54, 2, 1.3f, hasCheckpoints: true, XtraPortal, XtraPortal),
		["XT"] = new WorldData(55, 9, 0.9f, hasCheckpoints: true, CrownPortal, CrownPortal),
		["XR"] = new WorldData(56, 4, 1.3f, hasCheckpoints: true, XtraPortal, XtraPortal),
		["XP"] = new WorldData(57, 1, 1f, hasCheckpoints: true, CrownPortal, CrownPortal),
		["MN"] = new WorldData(58, 5, 1.4f, hasCheckpoints: false, MuseDashPortal, MuseDashPortal),
		["ML"] = new WorldData(59, 8, 1.1f, hasCheckpoints: false, MuseDashPortal, MuseDashPortal),
		["MO"] = new WorldData(60, 4, 1f, hasCheckpoints: false, MuseDashPortal, MuseDashPortal),
		["RJ"] = new WorldData(61, 8, 1.2f, hasCheckpoints: false, XtraPortal, XtraPortal),
		["Template"] = new WorldData(100, 1, 1f, hasCheckpoints: true, new Vector2Int(0, 0), new Vector2Int(0, 0)),
		["T1"] = new WorldData(101, 5, 1.3f, hasCheckpoints: true, new Vector2Int(0, 0), new Vector2Int(1, 0)),
		["T1EX"] = new WorldData(111, 5, 1.3f, hasCheckpoints: true, new Vector2Int(0, -7), new Vector2Int(1, -7)),
		["T2"] = new WorldData(102, 10, 1.3f, hasCheckpoints: true, new Vector2Int(7, 0), new Vector2Int(8, 0)),
		["T2EX"] = new WorldData(112, 5, 1.3f, hasCheckpoints: true, new Vector2Int(7, -7), new Vector2Int(8, -7)),
		["T3"] = new WorldData(103, 2, 1f, hasCheckpoints: true, new Vector2Int(14, 0), new Vector2Int(15, 0)),
		["T3EX"] = new WorldData(113, 1, 1f, hasCheckpoints: true, new Vector2Int(14, -7), new Vector2Int(15, -7)),
		["T4"] = new WorldData(104, 13, 1.1f, hasCheckpoints: true, new Vector2Int(21, 0), new Vector2Int(22, 0)),
		["T4EX"] = new WorldData(114, 5, 1.1f, hasCheckpoints: true, new Vector2Int(21, -7), new Vector2Int(22, -7)),
		["T5"] = new WorldData(105, 6, 1f, hasCheckpoints: true, new Vector2Int(28, 0), new Vector2Int(29, 0)),
		["T6"] = new WorldData(106, 1, 1f, hasCheckpoints: true, new Vector2Int(35, 0), new Vector2Int(36, 0)),
		["TP"] = new WorldData(130, 3, 1f, hasCheckpoints: true, new Vector2Int(25, 0), new Vector2Int(25, 0))
	};

	public static Dictionary<string, Vector2> jumpPositionTaroMenu0 = new Dictionary<string, Vector2>
	{
		["T1"] = new Vector2Int(0, 0),
		["T2"] = new Vector2Int(7, 0),
		["T3"] = new Vector2Int(14, 0),
		["T4"] = new Vector2Int(21, 0),
		["T5"] = new Vector2Int(28, 0),
		["T1EX"] = new Vector2Int(0, -7),
		["T2EX"] = new Vector2Int(7, -7),
		["T3EX"] = new Vector2Int(14, -7),
		["T4EX"] = new Vector2Int(21, -7),
		["T6"] = new Vector2Int(99, 0),
		["TP"] = new Vector2Int(27, -3),
		["Template"] = new Vector2Int(99, 0)
	};

	public static Dictionary<string, Vector2> jumpPositionTaroMenu1 = new Dictionary<string, Vector2>
	{
		["T1"] = new Vector2Int(0, 0),
		["T2"] = new Vector2Int(0, 0),
		["T3"] = new Vector2Int(0, 0),
		["T4"] = new Vector2Int(0, 0),
		["T5"] = new Vector2Int(0, 0),
		["T1EX"] = new Vector2Int(0, 0),
		["T2EX"] = new Vector2Int(0, 0),
		["T3EX"] = new Vector2Int(0, 0),
		["T4EX"] = new Vector2Int(0, 0),
		["T6"] = new Vector2Int(0, 0),
		["TP"] = new Vector2Int(0, 0),
		["Template"] = new Vector2Int(99, 0)
	};

	public static Dictionary<string, Vector2> jumpPositionTaroMenu2 = new Dictionary<string, Vector2>
	{
		["T1"] = new Vector2Int(7, -8),
		["T2"] = new Vector2Int(14, -8),
		["T3"] = new Vector2Int(21, -8),
		["T4"] = new Vector2Int(0, 0),
		["T5"] = new Vector2Int(0, 0),
		["T1EX"] = new Vector2Int(0, 0),
		["T2EX"] = new Vector2Int(0, 0),
		["T3EX"] = new Vector2Int(0, 0),
		["T4EX"] = new Vector2Int(0, 0),
		["T6"] = new Vector2Int(0, 0),
		["TP"] = new Vector2Int(0, 0),
		["Template"] = new Vector2Int(99, 0)
	};

	public static Dictionary<string, Vector2> jumpPositionTaroMenu3 = new Dictionary<string, Vector2>
	{
		["T1"] = new Vector2Int(7, -2),
		["T2"] = new Vector2Int(14, -2),
		["T3"] = new Vector2Int(21, -2),
		["T4"] = new Vector2Int(28, -2),
		["T5"] = new Vector2Int(0, 0),
		["T1EX"] = new Vector2Int(0, 0),
		["T2EX"] = new Vector2Int(0, 0),
		["T3EX"] = new Vector2Int(0, 0),
		["T4EX"] = new Vector2Int(0, 0),
		["T6"] = new Vector2Int(0, 0),
		["TP"] = new Vector2Int(0, 0),
		["Template"] = new Vector2Int(99, 0)
	};

	public static Dictionary<string, Vector2> activePositionTaroMenu1 = new Dictionary<string, Vector2>
	{
		["T1"] = new Vector2Int(2, 12),
		["T2"] = new Vector2Int(16, 12),
		["T3"] = new Vector2Int(9, 12),
		["T4"] = new Vector2Int(99, 0),
		["T5"] = new Vector2Int(99, 0),
		["T1EX"] = new Vector2Int(99, 0),
		["T2EX"] = new Vector2Int(99, 0),
		["T3EX"] = new Vector2Int(99, 0),
		["T4EX"] = new Vector2Int(99, 0),
		["T6"] = new Vector2Int(99, 0),
		["TP"] = new Vector2Int(99, 0),
		["Template"] = new Vector2Int(99, 0)
	};

	public static Dictionary<string, Vector2> activePositionTaroMenu2 = new Dictionary<string, Vector2>
	{
		["T1"] = new Vector2Int(7, -8),
		["T2"] = new Vector2Int(14, -8),
		["T3"] = new Vector2Int(21, -8),
		["T4"] = new Vector2Int(0, 2),
		["T5"] = new Vector2Int(99, 0),
		["T1EX"] = new Vector2Int(99, 0),
		["T2EX"] = new Vector2Int(99, 0),
		["T3EX"] = new Vector2Int(99, 0),
		["T4EX"] = new Vector2Int(99, 0),
		["T6"] = new Vector2Int(99, 0),
		["TP"] = new Vector2Int(99, 0),
		["Template"] = new Vector2Int(99, 0)
	};

	public static Dictionary<string, Vector2> activePositionTaroMenu3 = new Dictionary<string, Vector2>
	{
		["T1"] = new Vector2Int(7, -2),
		["T2"] = new Vector2Int(14, -2),
		["T3"] = new Vector2Int(21, -2),
		["T4"] = new Vector2Int(28, -2),
		["T5"] = new Vector2Int(0, 2),
		["T1EX"] = new Vector2Int(99, 0),
		["T2EX"] = new Vector2Int(99, 0),
		["T3EX"] = new Vector2Int(99, 0),
		["T4EX"] = new Vector2Int(99, 0),
		["T6"] = new Vector2Int(99, 0),
		["TP"] = new Vector2Int(99, 0),
		["Template"] = new Vector2Int(99, 0)
	};

	public static readonly string[] allWorlds = new string[34]
	{
		"1",
		"2",
		"3",
		"4",
		"5",
		"6",
		"7",
		"8",
		"9",
		"10",
		"11",
		"12",
		"B",
		"XF",
		"XO",
		"XC",
		"XH",
		"PA",
		"XT",
		"XR",
		"XP",
		"MN",
		"ML",
		"MO",
		"RJ",
		"T1",
		"T2",
		"T3",
		"T4",
		"T5",
		"T1EX",
		"T2EX",
		"T3EX",
		"T4EX"
	};

	public static readonly string[] dlcWorlds = new string[9]
	{
		"T1",
		"T2",
		"T3",
		"T4",
		"T5",
		"T1EX",
		"T2EX",
		"T3EX",
		"T4EX"
	};

	public static readonly string[] xtraWorlds = new string[6]
	{
		"XF",
		"XC",
		"XH",
		"XR",
		"PA",
		"RJ"
	};

	public static readonly string[] crownWorlds = new string[3]
	{
		"XO",
		"XT",
		"XP"
	};

	public static readonly string[] taroMenus = new string[7]
	{
		"scnTaroMenu0",
		"scnTaroMenu1",
		"scnTaroMenu2",
		"scnTaroMenu3",
		"TP-1",
		"TP-2",
		"TP-X"
	};

	public static readonly string[] museDashWorlds = new string[3]
	{
		"MN",
		"ML",
		"MO"
	};

	public static readonly string[] doNotBuildWorld = new string[1]
	{
		"XP"
	};

	public static readonly Dictionary<string, int> dlcMedalsCount = new Dictionary<string, int>
	{
		["T1"] = 6,
		["T2"] = 7,
		["T3"] = 20,
		["T4"] = 8,
		["T5"] = 10,
		["T1EX"] = 8,
		["T2EX"] = 7,
		["T3EX"] = 20,
		["T4EX"] = 8
	};

	public static readonly Dictionary<string, int> dlcMedalsRequired = new Dictionary<string, int>
	{
		["T1EX"] = 3,
		["T2EX"] = 5,
		["T3EX"] = 12,
		["T4EX"] = 5
	};

	public const float LongTileRadius = 1.5f;

	public const int newBestAfterLongTimeValue = 10;

	public const float practiceSpeedMultiplier = 0.9f;

	public const int defaultPracticeLength = 20;

	public static readonly uint[] ExtraLevelsIds = new uint[17]
	{
		2731825935u,
		2731817282u,
		2731815380u,
		2776211226u,
		2779312583u,
		2779318434u,
		2787254380u,
		2801793853u,
		2801798771u,
		2802386409u,
		2802387483u,
		2804476936u,
		2804919454u,
		2833657749u,
		2833658365u,
		2833658039u,
		2833657910u
	};

	public static readonly uint[] FeralNormalLevelsIds = new uint[2]
	{
		2802386409u,
		2802387483u
	};

	public static readonly uint[] FeralTechLevelsIds = new uint[2]
	{
		2802388539u,
		2802389102u
	};

	public static readonly uint[] SkyscapeLevelsIds = new uint[2]
	{
		2804476936u,
		2804919454u
	};

	public const uint NeoCosmosDLCSteamID = 1977570u;

	public const string NeoCosmosWorkshopTag = "Neo Cosmos";

	public static string sceneLevelSelect = "scnLevelSelect";

	public const string sceneSplash = "scnSplash";

	public const string sceneSplashMobile = "scnSplashMobile";

	public const string sceneLevelSelectDesktop = "scnLevelSelect";

	public const string sceneLevelSelectMobile = "scnMobileMenu";

	public const string sceneLoading = "scnLoading";

	public const string sceneCalibration = "scnCalibration";

	public const string sceneEditor = "scnEditor";

	public const string sceneCustomLevelSelect = "scnCLS";

	public const string sceneTaroMenu0 = "scnTaroMenu0";

	public const string sceneTaroMenu1 = "scnTaroMenu1";

	public const string sceneTaroMenu2 = "scnTaroMenu2";

	public const string sceneTaroMenu3 = "scnTaroMenu3";

	public const string sceneTPTest = "TP-Test";

	public const string sceneTP1 = "TP-1";

	public const string sceneTP2 = "TP-2";

	public const string sceneTP3 = "TP-X";

	public const string sceneMinesweeper = "scnMinesweeper";

	public static readonly string[] betaBranches = new string[5]
	{
		"beta",
		"alpha",
		"stardust",
		"deepspace",
		"frontline"
	};

	public static readonly string[] devBranches = new string[3]
	{
		"stardust",
		"deepspace",
		"frontline"
	};

	public static int numOfWorlds => allWorlds.Length;
}
