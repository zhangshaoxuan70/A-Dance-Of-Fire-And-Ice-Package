using Discord;
using RDTools;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DiscordController : ADOBase
{
	public static DiscordController instance;

	public static bool shouldUpdatePresence = true;

	public static string currentUsername = null;

	public static long currentUserID = -1L;

	public static bool isBirthday = false;

	private Discord.Discord discord;

	public void UpdatePresence()
	{
		if (discord == null)
		{
			return;
		}
		string s = string.Empty;
		string s2 = string.Empty;
		string s3 = string.Empty;
		scrController scrController = scrController.instance;
		if (ADOBase.sceneName == GCNS.sceneLevelSelect)
		{
			s2 = RDString.Get("discord.inLevelSelect");
			int overallProgressStage = Persistence.GetOverallProgressStage();
			if (overallProgressStage >= 9)
			{
				s3 = RDString.Get("levelSelect.GameCompleteFull");
			}
			else if (overallProgressStage >= 5)
			{
				s3 = RDString.Get("levelSelect.GameComplete");
			}
		}
		else if (scrController != null && ADOBase.isLevelEditor)
		{
			string text = ADOBase.editor.levelData.fullCaption;
			if (GCS.standaloneLevelMode)
			{
				s2 = RDString.Get("discord.playing");
				if (!scrMisc.ApproximatelyFloor(GCS.speedTrialMode ? GCS.currentSpeedTrial : (ADOBase.isEditingLevel ? ADOBase.editor.playbackSpeed : 1f), 1.0))
				{
					string str = RDString.Get("levelSelect.multiplier", new Dictionary<string, object>
					{
						{
							"multiplier",
							ADOBase.conductor.song.pitch.ToString("0.0#")
						}
					});
					text = text + " (" + str + ")";
				}
				s3 = text;
			}
			else
			{
				s2 = RDString.Get("discord.inLevelEditor");
				if (!ADOBase.editor.customLevel.levelPath.IsNullOrEmpty())
				{
					s3 = RDString.Get("discord.editedLevel", new Dictionary<string, object>
					{
						{
							"level",
							text
						}
					});
				}
			}
		}
		else if (ADOBase.sceneName == "scnCLS")
		{
			s2 = RDString.Get("discord.inCustomLevelSelect");
		}
		else if (scrController != null && scrController.gameworld)
		{
			string text2 = ADOBase.GetLocalizedLevelName(ADOBase.sceneName).RemoveRichTags();
			if (!scrMisc.ApproximatelyFloor(GCS.speedTrialMode ? GCS.currentSpeedTrial : (ADOBase.isEditingLevel ? ADOBase.editor.playbackSpeed : 1f), 1.0))
			{
				string str2 = RDString.Get("levelSelect.multiplier", new Dictionary<string, object>
				{
					{
						"multiplier",
						ADOBase.conductor.song.pitch.ToString("0.0#")
					}
				});
				text2 = text2 + " (" + str2 + ")";
			}
			s2 = RDString.Get("discord.playing");
			s3 = text2;
			s = text2;
		}
		s = Validate(s);
		s3 = Validate(s3);
		s2 = Validate(s2);
		Activity activity = default(Activity);
		activity.State = s3;
		activity.Details = s2;
		activity.Assets.LargeImage = "planets_icon_stars";
		activity.Assets.LargeText = s;
		Activity activity2 = activity;
		discord.GetActivityManager().UpdateActivity(activity2, delegate(Result result)
		{
			if (result != 0)
			{
				RDBaseDll.printem(result.ToString());
			}
		});
		shouldUpdatePresence = false;
	}

	private string Validate(string s)
	{
		if (s.Length <= 60)
		{
			return s;
		}
		return s.Substring(0, 57) + "...";
	}

	private void CheckForBirthday()
	{
		string[] array = Resources.Load<TextAsset>("birthdays").text.Split('\n');
		isBirthday = false;
		string[] array2 = array;
		int num = 0;
		while (true)
		{
			if (num >= array2.Length)
			{
				return;
			}
			string[] array3 = array2[num].Split(',');
			string[] array4 = array3[0].Split('/');
			if (long.TryParse(array3[1], out long result) && result == currentUserID)
			{
				DateTime now = DateTime.Now;
				int num2 = int.Parse(array4[0]);
				int num3 = int.Parse(array4[1]);
				int month = now.Month;
				int day = now.Day;
				if (num2 == month && num3 == day)
				{
					break;
				}
			}
			num++;
		}
		isBirthday = true;
	}

	private void Update()
	{
		if (discord != null)
		{
			try
			{
				discord.RunCallbacks();
			}
			catch (ResultException ex)
			{
				if (ex.Result != 0)
				{
					RDBaseDll.printem($"Discord result exception: {ex.Result}");
				}
			}
			if (shouldUpdatePresence)
			{
				UpdatePresence();
			}
		}
	}

	private void OnEnable()
	{
		if (!(instance != null))
		{
			instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			try
			{
				discord = new Discord.Discord(537047684993777686L, 1uL);
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log("Discord was not initialized: " + ex.ToString());
			}
			if (discord != null)
			{
				discord.GetActivityManager().RegisterSteam(977950u);
				discord.GetUserManager().OnCurrentUserUpdate += delegate
				{
					User currentUser = discord.GetUserManager().GetCurrentUser();
					currentUsername = currentUser.Username;
					currentUserID = currentUser.Id;
					CheckForBirthday();
				};
			}
		}
	}

	private void OnDisable()
	{
		if (discord != null)
		{
			discord.Dispose();
		}
	}
}
