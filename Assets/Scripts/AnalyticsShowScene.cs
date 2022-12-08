using Steamworks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnalyticsShowScene : ADOBase
{
	public TextMeshProUGUI textHolderPersonal;

	public TextMeshProUGUI textHolderGlobal;

	public TMP_InputField field;

	public UIGridRenderer grid;

	public UILineRenderer lineRenderer;

	public GameObject DrawButtonsPanel;

	public GameObject DebugPanel;

	private string steamName;

	private int cls_entered;

	private int editor_entered;

	private float hoursOfficialLevels;

	private float hoursEditorMaking;

	private float hoursEditorPlaying;

	private float hoursCustomLevelsPlaying;

	private int levelselect_entered;

	[SerializeField]
	public List<StatsData> globalStats = new List<StatsData>();

	private int daysPeriodCache;

	private void Awake()
	{
		SteamIntegration instance = SteamIntegration.Instance;
		SteamUserStats.RequestCurrentStats();
	}

	private void Start()
	{
		GetPersonalStats();
		DrawButtonsPanel.SetActive(value: false);
	}

	public void UploadValues()
	{
		Analytics.UploadStatsToSteam(hoursOfficialLevels, hoursEditorMaking, hoursEditorPlaying, hoursCustomLevelsPlaying);
	}

	public void GetGlobalStats()
	{
		if (SteamIntegration.Instance.initialized)
		{
			UnityEngine.Debug.Log("getting global values");
			if (int.TryParse(field.text, out int result) && field.text != null)
			{
				RetrieveGlobalStats(result);
				DrawButtonsPanel.SetActive(value: true);
			}
		}
	}

	public void AnalyticsValueUp()
	{
		if (SteamIntegration.Instance.initialized)
		{
			hoursOfficialLevels += 1f;
			RefreshPersonalStatsText();
		}
	}

	public void AnalyticsValueDown()
	{
		if (SteamIntegration.Instance.initialized)
		{
			hoursOfficialLevels -= 1f;
			RefreshPersonalStatsText();
		}
	}

	private void GetPersonalStats()
	{
		if (SteamIntegration.Instance.initialized)
		{
			steamName = SteamFriends.GetPersonaName();
			SteamUserStats.GetStat("cls_entered", out cls_entered);
			SteamUserStats.GetStat("editor_entered", out editor_entered);
			SteamUserStats.GetStat(globalStats[0].globalStatField, out hoursOfficialLevels);
			SteamUserStats.GetStat(globalStats[1].globalStatField, out hoursEditorMaking);
			SteamUserStats.GetStat(globalStats[2].globalStatField, out hoursEditorPlaying);
			SteamUserStats.GetStat(globalStats[3].globalStatField, out hoursCustomLevelsPlaying);
			SteamUserStats.GetStat("levelselect_entered", out levelselect_entered);
			RefreshPersonalStatsText();
		}
	}

	private void RefreshPersonalStatsText()
	{
		string text = "";
		text = "Personal Stats:\n\n";
		text = text + steamName + "\n";
		text = text + "cls_entered: " + cls_entered.ToString() + "\n";
		text = text + "editor_entered: " + editor_entered.ToString() + "\n";
		text = text + "hoursOfficialLevels: " + hoursOfficialLevels.ToString() + "\n";
		text = text + "hoursEditorMaking: " + hoursEditorMaking.ToString() + "\n";
		text = text + "hoursEditorPlaying: " + hoursEditorPlaying.ToString() + "\n";
		text = text + "hoursCustomLevelsPlaying: " + hoursCustomLevelsPlaying.ToString() + "\n";
		text = text + "levelselect_entered: " + levelselect_entered.ToString() + "\n";
		textHolderPersonal.text = text;
	}

	private void RetrieveGlobalStats(int _daysPeriod)
	{
		UnityEngine.Debug.Log("here " + _daysPeriod.ToString());
		textHolderGlobal.text = "";
		daysPeriodCache = _daysPeriod;
		SteamAPICall_t hAPICall = SteamUserStats.RequestGlobalStats(_daysPeriod);
		CallResult<GlobalStatsReceived_t>.Create().Set(hAPICall, delegate(GlobalStatsReceived_t pCallback, bool bIOFailure)
		{
			printe("callback: " + pCallback.m_eResult.ToString());
			if ((pCallback.m_eResult != EResult.k_EResultOK) | bIOFailure)
			{
				UnityEngine.Debug.Log("There was an error retrieving global stats.");
			}
			else
			{
				UnityEngine.Debug.Log($"game id: {pCallback.m_nGameID}, daysPeriodCache {daysPeriodCache}");
				for (int i = 0; i < globalStats.Count; i++)
				{
					double[] array = new double[daysPeriodCache];
					int globalStatHistory = SteamUserStats.GetGlobalStatHistory(globalStats[i].globalStatField, array, (uint)(daysPeriodCache * 8));
					printe($"days: {globalStatHistory} data.Count {array.Length}");
					globalStats[i].value = array;
				}
			}
		});
	}

	public void DrawButton(int _index)
	{
		DrawGraph(daysPeriodCache, _index);
	}

	private void DrawGraph(int _days, int _statIndex)
	{
		double num = 0.0;
		for (int i = 0; i < globalStats[_statIndex].value.Length; i++)
		{
			if (globalStats[_statIndex].value[i] > num)
			{
				num = globalStats[_statIndex].value[i];
			}
		}
		UnityEngine.Debug.Log("MAX VALUE: " + num.ToString());
		grid.gridSize = new Vector2Int(_days, Mathf.FloorToInt((float)num + 1f));
		grid.SetAllDirty();
		lineRenderer.points.Clear();
		int num2 = 0;
		double[] value = globalStats[_statIndex].value;
		foreach (double num3 in value)
		{
			lineRenderer.points.Add(new Vector2(num2, (float)num3));
			num2++;
		}
		textHolderGlobal.text = globalStats[_statIndex].globalStatField + "\n";
		int num4 = 0;
		value = globalStats[_statIndex].value;
		foreach (double num5 in value)
		{
			textHolderGlobal.text += $"Day {num4 + 1}: {Mathf.Round((float)num5)}\n";
			num4++;
		}
	}

	private void Update()
	{
		if (SteamIntegration.Instance.initialized)
		{
			SteamAPI.RunCallbacks();
			if (UnityEngine.Input.GetKeyDown(KeyCode.Tab))
			{
				DebugPanel.SetActive(!DebugPanel.activeSelf);
			}
		}
	}

	private void OnGlobalStatsReceived(GlobalStatsReceived_t pCallback, bool bIOFailure)
	{
	}
}
