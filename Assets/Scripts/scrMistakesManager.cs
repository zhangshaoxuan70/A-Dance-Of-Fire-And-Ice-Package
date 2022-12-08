using RDTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class scrMistakesManager
{
	public scrController controller;

	public scrLevelMaker lm;

	public static List<HitMargin> hitMargins = new List<HitMargin>();

	public static int[] hitMarginsCount = new int[Enum.GetValues(typeof(HitMargin)).Cast<int>().Max() + 1];

	public static Difficulty hardestDifficulty;

	public static int lastHitMarginsSize = 0;

	private bool newBestCompletion;

	public float percentComplete;

	public float percentAcc;

	public float percentXAcc;

	public CustomLevel customLevel => CustomLevel.instance;

	public scrMistakesManager(scrController controller, scrLevelMaker levelMaker)
	{
		this.controller = controller;
		lm = levelMaker;
	}

	public void RevertToLastCheckpoint()
	{
		while (hitMargins.Count > lastHitMarginsSize)
		{
			HitMargin hitMargin = hitMargins.Pop();
			hitMarginsCount[(int)hitMargin]--;
		}
	}

	public void MarkCheckpoint(int checkpointTileOffset)
	{
		lastHitMarginsSize = hitMargins.Count;
		int num = 0;
		while (lastHitMarginsSize > 0)
		{
			if (hitMargins[lastHitMarginsSize - 1] != 0)
			{
				if (num >= -checkpointTileOffset)
				{
					break;
				}
				num++;
			}
			lastHitMarginsSize--;
		}
	}

	public void Reset()
	{
		hitMargins = new List<HitMargin>();
		lastHitMarginsSize = 0;
		for (int i = 0; i < hitMarginsCount.Length; i++)
		{
			hitMarginsCount[i] = 0;
		}
		hardestDifficulty = Difficulty.Strict;
		CalculatePercentAcc();
	}

	public void AddHit(HitMargin hit)
	{
		hitMargins.Add(hit);
		hitMarginsCount[(int)hit]++;
		CalculatePercentAcc();
		hardestDifficulty = (Difficulty)Mathf.Min((int)GCS.difficulty, (int)hardestDifficulty);
	}

	public int GetHits(HitMargin hit)
	{
		return hitMarginsCount[(int)hit];
	}

	public float GetTotalHits()
	{
		return hitMargins.Count;
	}

	public void CalculatePercentAcc()
	{
		double num = GetHits(HitMargin.Perfect) + GetHits(HitMargin.EarlyPerfect) + GetHits(HitMargin.LatePerfect) + GetHits(HitMargin.Auto);
		double num2 = hitMargins.Count + GetHits(HitMargin.FailMiss) + GetHits(HitMargin.FailOverload);
		double num3 = (num == num2) ? 1.0 : (num / num2);
		double num4 = (double)(GetHits(HitMargin.Perfect) + GetHits(HitMargin.Auto)) * 0.0001;
		percentAcc = (float)(num3 + num4);
		if ((bool)lm && (bool)scrController.instance)
		{
			percentComplete = (float)(scrController.instance.currentSeqID + 1) / (float)lm.listFloors.Count;
		}
		double num5 = (1.0 * (double)GetHits(HitMargin.Perfect) + 1.0 * (double)GetHits(HitMargin.Auto) + 0.75 * (double)GetHits(HitMargin.EarlyPerfect) + 0.75 * (double)GetHits(HitMargin.LatePerfect) + 0.4 * (double)GetHits(HitMargin.VeryEarly) + 0.4 * (double)GetHits(HitMargin.VeryLate) + 0.2 * (double)GetHits(HitMargin.TooEarly) + 0.2 * (double)GetHits(HitMargin.TooLate)) / (double)hitMargins.Count;
		percentXAcc = (float)(num5 * Math.Pow(0.9875, scrController.checkpointsUsed));
	}

	public bool IsAllPurePerfect()
	{
		if (controller.startedFromCheckpoint)
		{
			return false;
		}
		foreach (HitMargin hitMargin in hitMargins)
		{
			if (hitMargin != HitMargin.Perfect)
			{
				return false;
			}
		}
		return true;
	}

	public bool IsNewBest()
	{
		if (controller.currentFloorID == 0)
		{
			return newBestCompletion = false;
		}
		if (controller.isbosslevel && !controller.isPuzzleRoom)
		{
			float percentCompletion = Persistence.GetPercentCompletion(scrController.currentWorld);
			if (newBestCompletion || percentCompletion < percentComplete)
			{
				newBestCompletion = false;
				return true;
			}
		}
		else if ((bool)customLevel && GCS.standaloneLevelMode && GCS.customLevelIndex >= GCS.customLevelPaths.Length - 1 && !GCS.practiceMode && newBestCompletion)
		{
			newBestCompletion = false;
			return true;
		}
		return false;
	}

	public EndLevelType SaveCustom(string hash, bool wonLevel, float multiplier)
	{
		if (controller.noFail)
		{
			return EndLevelType.None;
		}
		EndLevelType result = EndLevelType.None;
		float customWorldAccuracy = Persistence.GetCustomWorldAccuracy(hash);
		float customWorldXAccuracy = Persistence.GetCustomWorldXAccuracy(hash);
		float customWorldCompletion = Persistence.GetCustomWorldCompletion(hash);
		bool customWorldIsHighestPossibleAcc = Persistence.GetCustomWorldIsHighestPossibleAcc(hash);
		bool flag = IsAllPurePerfect();
		if (multiplier >= 1f)
		{
			if (!wonLevel)
			{
				if (customWorldCompletion < percentComplete)
				{
					newBestCompletion = true;
					result = EndLevelType.NewBest;
					Persistence.SetCustomWorldCompletion(hash, percentComplete);
				}
			}
			else
			{
				float customWorldSpeedTrial = Persistence.GetCustomWorldSpeedTrial(hash);
				int customWorldMinDeaths = Persistence.GetCustomWorldMinDeaths(hash);
				Persistence.SetCustomWorldCompletion(hash, 1f);
				if (customWorldCompletion < 1f && multiplier == 1f)
				{
					result = EndLevelType.FirstWin;
				}
				else if (GCS.speedTrialMode && multiplier >= 1f && multiplier > customWorldSpeedTrial)
				{
					result = EndLevelType.FirstWinSpeedTrial;
					Persistence.SetCustomWorldSpeedTrial(hash, multiplier);
				}
				if (percentAcc > customWorldAccuracy)
				{
					Persistence.SetCustomWorldAccuracy(hash, percentAcc);
					if (flag)
					{
						Persistence.SetCustomWorldIsHighestPossibleAcc(hash, isHighest: true);
					}
				}
				if (percentXAcc > customWorldXAccuracy)
				{
					Persistence.SetCustomWorldXAccuracy(hash, percentXAcc);
				}
				if (customLevel.checkpointsUsed < customWorldMinDeaths || customWorldMinDeaths == -1)
				{
					Persistence.SetCustomWorldMinDeaths(hash, customLevel.checkpointsUsed);
				}
				if (!customWorldIsHighestPossibleAcc && flag)
				{
					Persistence.SetCustomWorldIsHighestPossibleAcc(hash, isHighest: true);
				}
			}
			Persistence.Save();
		}
		return result;
	}

	public EndLevelType Save(int worldZeroIndexed, bool wonLevel, float multiplier)
	{
		if (controller.noFail)
		{
			return EndLevelType.None;
		}
		EndLevelType result = EndLevelType.None;
		float percentCompletion = Persistence.GetPercentCompletion(worldZeroIndexed);
		float bestPercentAccuracy = Persistence.GetBestPercentAccuracy(worldZeroIndexed);
		float bestPercentXAccuracy = Persistence.GetBestPercentXAccuracy(worldZeroIndexed);
		float bestSpeedMultiplier = Persistence.GetBestSpeedMultiplier(worldZeroIndexed);
		string currentWorldString = scrController.currentWorldString;
		if (!wonLevel)
		{
			if (percentCompletion < percentComplete)
			{
				newBestCompletion = true;
				result = EndLevelType.NewBest;
				if (Persistence.GetWorldAttemptsWithoutNewBest(worldZeroIndexed) > 10)
				{
					result = EndLevelType.NewBestAfterLongTime;
				}
				Persistence.SetWorldAttemptsWithoutNewBest(worldZeroIndexed, 0);
				Persistence.SetPercentCompletion(worldZeroIndexed, percentComplete);
			}
		}
		else
		{
			Persistence.SetPercentCompletion(worldZeroIndexed, 1f);
			if (percentCompletion < 1f && multiplier == 1f)
			{
				result = EndLevelType.FirstWin;
				Persistence.SetPercentCompletion(worldZeroIndexed, 1f);
			}
			else if (GCS.speedTrialMode && multiplier > bestSpeedMultiplier)
			{
				bool flag = Persistence.IsSpeedTrialComplete(worldZeroIndexed);
				Persistence.SetBestSpeedTrial(worldZeroIndexed, multiplier);
				if (multiplier >= GCNS.worldData[currentWorldString].trialAim)
				{
					bool flag2 = Persistence.IsSpeedTrialComplete(worldZeroIndexed);
					result = ((flag || !flag2) ? EndLevelType.NewBestMult : EndLevelType.FirstWinSpeedTrial);
				}
			}
			if (percentAcc > bestPercentAccuracy)
			{
				Persistence.SetBestPercentAccuracy(worldZeroIndexed, percentAcc);
			}
			if (percentXAcc > bestPercentXAccuracy)
			{
				Persistence.SetBestPercentXAccuracy(worldZeroIndexed, percentXAcc);
			}
			if (IsAllPurePerfect())
			{
				Persistence.SetIsHighestPossibleAcc(worldZeroIndexed, isHighest: true);
			}
		}
		Persistence.GiveAchievements();
		Persistence.Save();
		return result;
	}

	public static void SaveProgress(bool save)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		if (GCS.checkpointNum > 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (hitMargins.Count > 0)
			{
				foreach (HitMargin hitMargin in hitMargins)
				{
					stringBuilder.Append(((int)hitMargin).ToString() + ",");
				}
				stringBuilder.Length--;
			}
			dictionary["hitMargins"] = stringBuilder.ToString();
			dictionary["lastHitMarginsSize"] = lastHitMarginsSize;
			dictionary["checkpointNum"] = GCS.checkpointNum;
			dictionary["level"] = scrController.instance.levelName;
			dictionary["hardestDifficulty"] = hardestDifficulty.ToString();
			Persistence.SetSavedProgress(dictionary);
			if (save)
			{
				Persistence.generalPrefs.Save();
			}
		}
	}

	public static void LoadProgress()
	{
		Dictionary<string, object> savedProgress = Persistence.GetSavedProgress();
		string text = savedProgress["hitMargins"] as string;
		if (text == null || text.Length == 0 || !text.Contains(','))
		{
			UnityEngine.Debug.Log("Error with hitMarginsString value: " + text);
			return;
		}
		hitMargins.Clear();
		for (int i = 0; i < hitMarginsCount.Length; i++)
		{
			hitMarginsCount[i] = 0;
		}
		string[] array = text?.Split(',');
		for (int j = 0; j < array.Length; j++)
		{
			int num = Convert.ToInt32(array[j]);
			hitMargins.Add((HitMargin)num);
			hitMarginsCount[num]++;
		}
		GCS.checkpointNum = (int)savedProgress["checkpointNum"];
		lastHitMarginsSize = (int)savedProgress["lastHitMarginsSize"];
		if (savedProgress.TryGetValue("hardestDifficulty", out object value))
		{
			hardestDifficulty = RDUtils.ParseEnum((string)value, Difficulty.Normal);
		}
		else
		{
			hardestDifficulty = Difficulty.Normal;
		}
		RDBaseDll.printem($"<color=red>Decoded progress:</color> checkpointNum {GCS.checkpointNum}, lastHitMarginsSize {lastHitMarginsSize}");
	}
}
