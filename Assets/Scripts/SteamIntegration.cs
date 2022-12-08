using RDTools;
using Steamworks;
using System;
using UnityEngine;

public class SteamIntegration
{
	public struct SteamAchievement
	{
		public Achievement id;

		public string name;

		public string description;

		public bool achieved;

		public SteamAchievement(Achievement id)
		{
			this.id = id;
			name = string.Empty;
			description = string.Empty;
			achieved = false;
		}
	}

	public struct SteamStat
	{
		public Stat name;

		public float value;

		public bool isValueInt;

		public SteamStat(Stat name, bool valueType)
		{
			this.name = name;
			value = 0f;
			isValueInt = valueType;
		}
	}

	public enum Stat
	{
		stat_test
	}

	public enum Achievement
	{
		World0Complete,
		World1Complete,
		World2Complete,
		World3Complete,
		World4Complete,
		World5Complete,
		World7Complete,
		World8Complete,
		World9Complete,
		World0Perfect,
		World1Perfect,
		World2Perfect,
		World3Perfect,
		World4Perfect,
		World5Perfect,
		World7Perfect,
		World8Perfect,
		World9Perfect,
		World0Trial,
		World1Trial,
		World2Trial,
		World3Trial,
		World4Trial,
		World5Trial,
		World7Trial,
		World8Trial,
		World9Trial,
		BonusComplete,
		Game100PercentComplete
	}

	public static bool isSteamBuildInitialized = false;

	public static bool isSteamBuild = true;

	private static SteamIntegration instance;

	public CGameID gameID;

	private SteamStat[] steamStatArray = new SteamStat[1]
	{
		new SteamStat(Stat.stat_test, valueType: false)
	};

	private SteamAchievement[] steamAchievementArray = new SteamAchievement[29]
	{
		new SteamAchievement(Achievement.World0Complete),
		new SteamAchievement(Achievement.World1Complete),
		new SteamAchievement(Achievement.World2Complete),
		new SteamAchievement(Achievement.World3Complete),
		new SteamAchievement(Achievement.World4Complete),
		new SteamAchievement(Achievement.World5Complete),
		new SteamAchievement(Achievement.World7Complete),
		new SteamAchievement(Achievement.World8Complete),
		new SteamAchievement(Achievement.World9Complete),
		new SteamAchievement(Achievement.World0Perfect),
		new SteamAchievement(Achievement.World1Perfect),
		new SteamAchievement(Achievement.World2Perfect),
		new SteamAchievement(Achievement.World3Perfect),
		new SteamAchievement(Achievement.World4Perfect),
		new SteamAchievement(Achievement.World5Perfect),
		new SteamAchievement(Achievement.World7Perfect),
		new SteamAchievement(Achievement.World8Perfect),
		new SteamAchievement(Achievement.World9Perfect),
		new SteamAchievement(Achievement.World0Trial),
		new SteamAchievement(Achievement.World1Trial),
		new SteamAchievement(Achievement.World2Trial),
		new SteamAchievement(Achievement.World3Trial),
		new SteamAchievement(Achievement.World4Trial),
		new SteamAchievement(Achievement.World5Trial),
		new SteamAchievement(Achievement.World7Trial),
		new SteamAchievement(Achievement.World8Trial),
		new SteamAchievement(Achievement.World9Trial),
		new SteamAchievement(Achievement.BonusComplete),
		new SteamAchievement(Achievement.Game100PercentComplete)
	};

	protected Callback<UserStatsReceived_t> userStatsReceived;

	protected Callback<UserStatsStored_t> userStatsStored;

	protected Callback<UserAchievementStored_t> userAchievementStored;

	private static bool EverInitialized;

	public bool initialized;

	private const string CLS_Entered = "cls_entered";

	private const string Editor_Entered = "editor_entered";

	private const string LevelSelect_Entered = "levelselect_entered";

	public static SteamIntegration Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new SteamIntegration();
				instance.OpenConnection();
				if (instance.initialized)
				{
					SteamWorkshop.Setup();
				}
			}
			return instance;
		}
	}

	public SteamIntegration()
	{
		RDC.forceNoSteamworks = false;
		if (instance == null && !RDC.forceNoSteamworks)
		{
			instance = this;
			if (EverInitialized)
			{
				throw new Exception("Tried to Initialize the SteamAPI twice in one session");
			}
			if (!Packsize.Test())
			{
				UnityEngine.Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.");
			}
			if (!DllCheck.Test())
			{
				UnityEngine.Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.");
			}
			initialized = SteamAPI.Init();
			if (!initialized)
			{
				UnityEngine.Debug.LogWarning("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.");
			}
			else
			{
				EverInitialized = true;
			}
		}
	}

	public void CheckCallbacks()
	{
		if (initialized)
		{
			SteamAPI.RunCallbacks();
		}
	}

	public bool OpenConnection()
	{
		if (!initialized)
		{
			return false;
		}
		gameID = new CGameID(SteamUtils.GetAppID());
		userStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
		userStatsStored = Callback<UserStatsStored_t>.Create(OnUserStatsStored);
		userAchievementStored = Callback<UserAchievementStored_t>.Create(OnAchievementStored);
		SteamUser.GetSteamID();
		ADOBase.ownsTaroDLC = SteamApps.BIsDlcInstalled(new AppId_t(1977570u));
		UnityEngine.Debug.Log($"Has license for {1977570u}: {ADOBase.ownsTaroDLC}");
		if (SteamUserStats.RequestCurrentStats())
		{
			for (int i = 0; i < steamStatArray.Length; i++)
			{
				SteamUserStats.GetStat(steamStatArray[i].name.ToString(), out steamStatArray[i].value);
			}
		}
		return true;
	}

	public void CloseConnection()
	{
		if (instance == this)
		{
			instance = null;
			if (initialized)
			{
				SteamAPI.Shutdown();
				EverInitialized = false;
				initialized = false;
				instance = null;
			}
		}
	}

	public bool GetStatValue(Stat statName, ref int value)
	{
		if (!initialized)
		{
			return false;
		}
		for (int i = 0; i < steamStatArray.Length; i++)
		{
			if (statName == steamStatArray[i].name)
			{
				value = (int)steamStatArray[i].value;
				return true;
			}
		}
		return false;
	}

	public bool GetStatValue(Stat statName, ref float value)
	{
		if (!initialized)
		{
			return false;
		}
		for (int i = 0; i < steamStatArray.Length; i++)
		{
			if (statName == steamStatArray[i].name)
			{
				value = steamStatArray[i].value;
				return true;
			}
		}
		return false;
	}

	public bool SetStatValue(Stat statName, int addedValue)
	{
		if (!initialized)
		{
			return false;
		}
		for (int i = 0; i < steamStatArray.Length; i++)
		{
			if (statName == steamStatArray[i].name)
			{
				steamStatArray[i].value += addedValue;
				UnityEngine.Debug.Log((int)steamStatArray[i].value);
				SteamUserStats.SetStat(statName.ToString(), (int)steamStatArray[i].value);
				SteamUserStats.StoreStats();
				return true;
			}
		}
		return false;
	}

	public bool SetStatValue(Stat statName, float addedValue)
	{
		if (!initialized)
		{
			return false;
		}
		for (int i = 0; i < steamStatArray.Length; i++)
		{
			if (statName == steamStatArray[i].name)
			{
				steamStatArray[i].value += addedValue;
				SteamUserStats.SetStat(statName.ToString(), steamStatArray[i].value);
				SteamUserStats.StoreStats();
				return true;
			}
		}
		return false;
	}

	public string[] GetSteamFriends()
	{
		if (initialized)
		{
			string[] array = new string[SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate)];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = SteamFriends.GetFriendPersonaName(SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate));
			}
			return array;
		}
		return new string[0];
	}

	public string GetPlayersName()
	{
		if (!initialized)
		{
			return string.Empty;
		}
		return SteamFriends.GetPersonaName();
	}

	public bool GetAchievementByName(Achievement achievementID, ref SteamAchievement steamAchievement)
	{
		if (!initialized)
		{
			return false;
		}
		for (int i = 0; i < steamAchievementArray.Length; i++)
		{
			if (steamAchievementArray[i].id == achievementID)
			{
				steamAchievement = steamAchievementArray[i];
				return true;
			}
		}
		return false;
	}

	public void UnlockAchievement(SteamAchievement achievement)
	{
		if (initialized)
		{
			SteamUserStats.SetAchievement(achievement.ToString());
		}
	}

	public void UnlockAchievementWithName(string achievement)
	{
		if (initialized)
		{
			SteamUserStats.SetAchievement(achievement);
		}
	}

	private void OnUserStatsReceived(UserStatsReceived_t pCallback)
	{
		if (!initialized || (ulong)gameID != pCallback.m_nGameID)
		{
			return;
		}
		if (EResult.k_EResultOK == pCallback.m_eResult)
		{
			for (int i = 0; i < steamAchievementArray.Length; i++)
			{
				if (SteamUserStats.GetAchievement(steamAchievementArray[i].id.ToString(), out steamAchievementArray[i].achieved))
				{
					steamAchievementArray[i].name = SteamUserStats.GetAchievementDisplayAttribute(steamAchievementArray[i].id.ToString(), "name");
					steamAchievementArray[i].description = SteamUserStats.GetAchievementDisplayAttribute(steamAchievementArray[i].id.ToString(), "desc");
				}
				else
				{
					UnityEngine.Debug.LogWarning("SteamUserStats.GetAchievement failed for Achievement " + steamAchievementArray[i].id.ToString() + "\nIs it registered in the Steam Partner site?");
				}
			}
			for (int j = 0; j < steamStatArray.Length; j++)
			{
				SteamUserStats.GetStat(steamStatArray[j].name.ToString(), out steamStatArray[j].value);
			}
		}
		else
		{
			UnityEngine.Debug.Log("RequestStats - failed, " + pCallback.m_eResult.ToString());
		}
	}

	private void OnUserStatsStored(UserStatsStored_t pCallback)
	{
		if ((ulong)gameID == pCallback.m_nGameID && EResult.k_EResultOK != pCallback.m_eResult)
		{
			if (EResult.k_EResultInvalidParam == pCallback.m_eResult)
			{
				UnityEngine.Debug.Log("StoreStats - some failed to validate");
				UserStatsReceived_t pCallback2 = default(UserStatsReceived_t);
				pCallback2.m_eResult = EResult.k_EResultOK;
				pCallback2.m_nGameID = (ulong)gameID;
				OnUserStatsReceived(pCallback2);
			}
			else
			{
				UnityEngine.Debug.Log("StoreStats - failed, " + pCallback.m_eResult.ToString());
			}
		}
	}

	private void OnAchievementStored(UserAchievementStored_t pCallback)
	{
		if ((ulong)gameID == pCallback.m_nGameID)
		{
			if (pCallback.m_nMaxProgress == 0)
			{
				UnityEngine.Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' unlocked!");
			}
			else
			{
				UnityEngine.Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' progress callback, (" + pCallback.m_nCurProgress.ToString() + "," + pCallback.m_nMaxProgress.ToString() + ")");
			}
		}
	}

	public void ClearAllAchievements()
	{
		if (initialized)
		{
			RDBaseDll.printes("SteamIntegration.ClearAllAchievements()");
			for (int i = 0; i < steamAchievementArray.Length; i++)
			{
				SteamUserStats.ClearAchievement(steamAchievementArray[i].id.ToString());
			}
		}
	}

	public void ResetAllData()
	{
		if (initialized)
		{
			SteamUserStats.ResetAllStats(bAchievementsToo: true);
		}
	}

	public static void IncrementCLSEnteredStat()
	{
		if (instance.initialized)
		{
			SteamUserStats.GetStat("cls_entered", out int pData);
			pData++;
			SteamUserStats.SetStat("cls_entered", pData);
		}
	}

	public static void CLSEntered()
	{
		if (instance.initialized)
		{
			SteamUserStats.SetStat("cls_entered", 1);
		}
	}

	public static void EditorEntered()
	{
		if (instance.initialized)
		{
			SteamUserStats.SetStat("editor_entered", 1);
		}
	}

	public static void LevelSelectEntered()
	{
		if (instance.initialized)
		{
			SteamUserStats.SetStat("levelselect_entered", 1);
		}
	}
}
