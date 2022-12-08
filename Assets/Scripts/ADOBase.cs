using ADOFAI;
using DG.Tweening;
using GDMiniJSON;
using RDTools;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class ADOBase : RDBaseDll
{
	public static Scene[] levelScenes;

	public static bool appIsInSteamLibrary = false;

	public static readonly Color ClearWhite = new Color(1f, 1f, 1f, 0f);

	public static bool ownsTaroDLC = false;

	public static bool hasTaroDLC = false;

	public static Platform platform = Platform.None;

	public static AudioManager audioManager => AudioManager.Instance;

	public static scrConductor conductor => scrConductor.instance;

	public static scrController controller => scrController.instance;

	public static scrLevelMaker lm => scrLevelMaker.instance;

	public static scrUIController uiController => scrUIController.instance;

	public static scnCLS cls => scnCLS.instance;

	public static scnEditor editor => scnEditor.instance;

	public static CustomLevel customLevel => CustomLevel.instance;

	public static string levelPath => CustomLevel.instance.levelPath;

	public static RDConstants gc => RDConstants.data;

	public static Dictionary<string, GCNS.WorldData> worldData => GCNS.worldData;

	public static scnLevelSelect levelSelect => scnLevelSelect.instance;

	public static bool isLevelEditor => scnEditor.instance != null;

	public static bool isEditingLevel
	{
		get
		{
			if (isLevelEditor)
			{
				return !GCS.standaloneLevelMode;
			}
			return false;
		}
	}

	public static bool isUnityEditor => Application.isEditor;

	public static bool isCLS => cls != null;

	public static bool isLevelSelect
	{
		get
		{
			if (!isCLS && controller != null && !controller.gameworld)
			{
				return !isLevelEditor;
			}
			return false;
		}
	}

	public static bool isOfficialLevel
	{
		get
		{
			if (controller != null)
			{
				return !isLevelEditor;
			}
			return false;
		}
	}

	public static bool isBossLevel => currentLevel.Contains("-X");

	public bool practiceAvailable
	{
		get
		{
			if (controller.isPuzzleRoom)
			{
				return false;
			}
			if (GCS.standaloneLevelMode)
			{
				return true;
			}
			if (controller.isbosslevel)
			{
				return true;
			}
			return false;
		}
	}

	public static string currentLevel => scrController.instance.levelName;

	public static bool playerIsOnIntroScene => sceneName == "scnIntro";

	public bool bb => GCS.bb;

	public float randomFloat => UnityEngine.Random.value;

	public float xGlobal
	{
		get
		{
			return base.transform.position.x;
		}
		set
		{
			base.transform.position = base.transform.position.WithX(value);
		}
	}

	public float yGlobal
	{
		get
		{
			return base.transform.position.y;
		}
		set
		{
			base.transform.position = base.transform.position.WithY(value);
		}
	}

	public float x
	{
		get
		{
			return base.transform.localPosition.x;
		}
		set
		{
			base.transform.localPosition = base.transform.localPosition.WithX(value);
		}
	}

	public float y
	{
		get
		{
			return base.transform.localPosition.y;
		}
		set
		{
			base.transform.localPosition = base.transform.localPosition.WithY(value);
		}
	}

	public static float d_speed
	{
		get
		{
			return scrController.instance.d_speed;
		}
		set
		{
			scrController.instance.d_speed = value;
		}
	}

	public static string sceneName
	{
		get
		{
			int sceneCount = SceneManager.sceneCount;
			for (int i = 0; i < sceneCount; i++)
			{
				Scene sceneAt = SceneManager.GetSceneAt(i);
				if (sceneAt.name != "scnLoading")
				{
					return sceneAt.name;
				}
			}
			return "";
		}
	}

	public static bool isGamepad => RDC.runningOnSteamDeck;

	public static bool isSwitch => platform == Platform.Switch;

	public static bool isMobile
	{
		get
		{
			if (platform != Platform.Android)
			{
				return platform == Platform.iOS;
			}
			return true;
		}
	}

	public static bool isSteamworks => true;

	public static void Startup()
	{
		ADOStartup.Startup();
	}

	public static int GetLevelNumber(string levelName = null)
	{
		if (levelName == null)
		{
			levelName = scrController.instance.levelName;
		}
		int num = levelName.IndexOf('-');
		int num2 = num + 1;
		string text = levelName.Substring(num2, levelName.Length - num2);
		if (text == "X")
		{
			string key = levelName.Substring(0, num);
			return GCNS.worldData[key].levelCount;
		}
		return int.Parse(text);
	}

	public static string GetPreviousLevelName(string levelName = null)
	{
		if (levelName == null)
		{
			levelName = scrController.instance.levelName;
		}
		int levelNumber = GetLevelNumber(levelName);
		RDBaseDll.printes($"levelname: {levelName} number: {levelNumber}");
		return string.Concat(str2: Mathf.Max(levelNumber - 1, 1).ToString(), str0: scrController.currentWorldString, str1: "-");
	}

	public static string GetNextLevelName(string levelName = null)
	{
		if (levelName == null)
		{
			levelName = scrController.instance.levelName;
		}
		int levelCount = GCNS.worldData[scrController.currentWorldString].levelCount;
		int levelNumber = GetLevelNumber(levelName);
		string str = (levelNumber + 1 == levelCount) ? "X" : Mathf.Min(levelNumber + 1, levelCount).ToString();
		return scrController.currentWorldString + "-" + str;
	}

	public static string GetCustomLevelName(string path)
	{
		string result = null;
		Dictionary<string, object> dictionary = Json.Deserialize(RDFile.ReadAllText(path)) as Dictionary<string, object>;
		LevelData levelData = new LevelData();
		levelData.Setup();
		if (dictionary != null)
		{
			levelData.Decode(dictionary, out LoadResult _);
			result = levelData.fullCaption;
		}
		return result;
	}

	public static void LoadScene(string scene)
	{
		if (scene.StartsWith("scnTaroMenu") || scene.IsTaro())
		{
			Addressables.LoadSceneAsync(scene);
		}
		else
		{
			SceneManager.LoadScene(scene);
		}
	}

	public static void GoToLevelSelect()
	{
		RDBaseDll.printes("killing all tweens");
		DOTween.KillAll();
		LoadScene(GCNS.sceneLevelSelect);
	}

	public static void GoToCalibration()
	{
		RDBaseDll.printes("killing all tweens");
		DOTween.KillAll();
		Time.timeScale = 1f;
		AudioListener.pause = false;
		GCS.currentSpeedTrial = 1f;
		GCS.nextSpeedRun = 1f;
		GCS.savedCheckpointNum = GCS.checkpointNum;
		GCS.checkpointNum = 0;
		GCS.wasInCalibration = true;
		if (GCS.savedCheckpointNum > 0)
		{
			scrController.checkpointsUsed++;
		}
		GCS.lastVisitedScene = sceneName;
		RDBaseDll.printes("going to scnCalibration from " + sceneName);
		LoadScene("scnCalibration");
	}

	public static void RestartScene()
	{
		RDBaseDll.printes("killing all tweens");
		DOTween.KillAll();
		LoadScene(SceneManager.GetActiveScene().name);
	}

	public void GoToLevelEditor()
	{
		RDBaseDll.printes("killing all tweens");
		DOTween.KillAll();
		LoadScene("scnEditor");
	}

	public static string GetLocalizedLevelName(string sceneName)
	{
		return sceneName + " " + RDString.Get(sceneName + ".title");
	}

	public static bool IsAprilFools()
	{
		int month = DateTime.Now.Month;
		int day = DateTime.Now.Day;
		if (month == 4 && day == 1)
		{
			return Persistence.IsWorldComplete(0);
		}
		return false;
	}

	public static bool IsHalloweenWeek()
	{
		int month = DateTime.Now.Month;
		int day = DateTime.Now.Day;
		if ((month == 10 && day >= 24) || (month == 11 && day <= 2))
		{
			return Persistence.IsWorldComplete(0);
		}
		return false;
	}

	public static bool IsCNY()
	{
		(int year, int month, int day) dateOfChineseNewYear = RDUtils.GetDateOfChineseNewYear();
		int item = dateOfChineseNewYear.year;
		int item2 = dateOfChineseNewYear.month;
		int item3 = dateOfChineseNewYear.day;
		DateTime dateTime = new DateTime(item, item2, item3);
		DateTime dateTime2 = dateTime.AddDays(15.0);
		DateTime now = DateTime.Now;
		long ticks = DateTime.Now.Ticks;
		long ticks2 = dateTime.Ticks;
		long ticks3 = dateTime2.Ticks;
		if (ticks > ticks2)
		{
			return ticks < ticks3;
		}
		return false;
	}

	public static int GetDisplayWidth()
	{
		return Display.main.systemWidth;
	}

	public static int GetDisplayHeight()
	{
		return Display.main.systemHeight;
	}

	public void FlushUnusedMemory()
	{
		if (audioManager != null)
		{
			audioManager.FlushData();
		}
		GC.Collect();
	}

	public virtual void OnBeat()
	{
	}
}
