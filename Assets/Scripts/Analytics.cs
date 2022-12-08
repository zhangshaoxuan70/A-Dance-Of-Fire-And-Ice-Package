using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Analytics;

public static class Analytics
{
	public static float OfficialLevelsTime;

	public static float editorMakingTime;

	public static float editorPlayingTime;

	public static float customLevelsTime;

	public static void UploadStatsToSteam()
	{
		if (SteamIntegration.Instance.initialized)
		{
			SteamUserStats.SetStat("hoursOfficialLevels", OfficialLevelsTime);
			SteamUserStats.SetStat("hoursEditorMaking", editorMakingTime);
			SteamUserStats.SetStat("hoursEditorPlaying", editorPlayingTime);
			SteamUserStats.SetStat("hoursCustomLevelsPlaying", customLevelsTime);
			SteamUserStats.StoreStats();
			OfficialLevelsTime = 0f;
			editorMakingTime = 0f;
			editorPlayingTime = 0f;
			customLevelsTime = 0f;
		}
	}

	public static void UploadStatsToSteam(float _hoursOff = 0f, float _hoursEdMaking = 0f, float _hoursEdPlaying = 0f, float _hoursCustomPlaying = 0f)
	{
		if (SteamIntegration.Instance.initialized)
		{
			SteamUserStats.SetStat("hoursOfficialLevels", _hoursOff);
			SteamUserStats.SetStat("hoursEditorMaking", _hoursEdMaking);
			SteamUserStats.SetStat("hoursEditorPlaying", _hoursEdPlaying);
			SteamUserStats.SetStat("hoursCustomLevelsPlaying", _hoursCustomPlaying);
			SteamUserStats.StoreStats();
		}
	}

	public static void UploadBranchToUnity()
	{
		int dayOfYear = DateTime.Now.DayOfYear;
		if (Persistence.GetLastAnalyticsUpdate() != dayOfYear)
		{
			Persistence.SetLastAnalyticsUpdate(dayOfYear);
			string pchName = "unknown";
			if (SteamIntegration.Instance.initialized)
			{
				SteamApps.GetCurrentBetaName(out pchName, 20);
			}
			else if (Application.dataPath.Contains(Path.Combine("steamapps", "common", "A Dance of Fire and Ice")))
			{
				pchName = "steam-offline";
			}
			else if (ADOBase.platform == Platform.Android)
			{
				pchName = "android";
			}
			else if (ADOBase.platform == Platform.iOS)
			{
				pchName = "ios";
			}
			if (!pchName.IsNullOrEmpty())
			{
				UnityEngine.Analytics.Analytics.CustomEvent("VersionInfo", new Dictionary<string, object>
				{
					{
						"branch",
						pchName
					}
				});
			}
		}
	}
}
