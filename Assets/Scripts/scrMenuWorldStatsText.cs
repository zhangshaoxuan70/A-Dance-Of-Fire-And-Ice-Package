using MobileMenu;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scrMenuWorldStatsText : ADOBase
{
	public scrPortal portal;

	public Text text;

	private bool didUpdateText;

	private bool lastSpeedTrialState;

	private void Start()
	{
		text.SetLocalizedFont();
		UpdateText(portal.locked);
	}

	public void UpdateText(bool locked, bool speedTrial = false)
	{
		if (!scrController.instance.setupComplete)
		{
			return;
		}
		this.text.color = Color.white;
		MobileMenuController instance = MobileMenuController.instance;
		string newValue = "<color=#00aaff>";
		string newValue2 = "<color=#cccc00>";
		string str = "<color=#a8f0ff>";
		string text = "</color>";
		string text2 = instance ? "    " : "\n";
		string world = portal.world;
		int index = ADOBase.worldData[world].index;
		int worldAttempts = Persistence.GetWorldAttempts(index);
		if (portal.locked)
		{
			bool exists = false;
			string text3 = "";
			text3 = ((!speedTrial) ? RDString.GetWithCheck("world" + world + ".requirement", out exists) : RDString.GetWithCheck("levelSelect.completeForTrial", out exists));
			if (exists)
			{
				this.text.text = text3;
			}
			else
			{
				this.text.text = "";
			}
		}
		else if (Persistence.GetLevelTutorialProgress(index) <= 0 && worldAttempts == 0)
		{
			string text4 = RDString.Get("levelSelect.neverPlayed");
			this.text.text = (instance ? (str + text4 + text) : text4);
		}
		else if (worldAttempts <= 0)
		{
			this.text.text = "";
		}
		else if (!Persistence.IsWorldComplete(index))
		{
			string value = (100f * Persistence.GetPercentCompletion(index)).ToString("0.00");
			string[] array = RDString.Get("levelSelect.worldStatsIncomplete", new Dictionary<string, object>
			{
				{
					"pctComplete",
					value
				},
				{
					"numAttempts",
					worldAttempts
				}
			}).Split('\n');
			string str2 = array[0].Replace("[[", newValue).Replace("]]", text);
			string str3 = array[1].Replace("[[", newValue2).Replace("]]", text);
			this.text.text = str2 + text2 + str3;
		}
		else if (Persistence.GetBestPercentAccuracy(index) == 0f)
		{
			this.text.text = RDString.Get("levelSelect.worldStatsCompleteCheckpoint").Replace("[[", newValue).Replace("]]", text);
		}
		else
		{
			double num = Persistence.GetBestPercentAccuracy(index);
			double num2 = Persistence.GetBestPercentXAccuracy(index);
			bool num3 = Persistence.GetShowXAccuracy() && num2 != 0.0;
			bool flag = num3 ? (num2 == 1.0) : Persistence.GetIsHighestPossibleAcc(index);
			string value2 = (!num3) ? ((num > 1.0) ? ("100% + " + (100.0 * (num - 1.0)).ToString("0.00")) : (100.0 * num).ToString("0.00")) : (100.0 * num2).ToString("0.00");
			Dictionary<string, object> parameters = new Dictionary<string, object>
			{
				{
					"numAttempts",
					worldAttempts
				},
				{
					"pctAccuracy",
					value2
				}
			};
			string[] array2 = (num3 ? RDString.Get("levelSelect.worldStatsCompleteXAccuracy", parameters) : RDString.Get("levelSelect.worldStatsComplete", parameters)).Split('\n');
			string text5 = array2[0].Replace("[[", newValue).Replace("]]", text);
			string text6 = array2[1].Replace("[[", newValue2).Replace("]]", text);
			string text7 = array2[2].Replace("[[", newValue2).Replace("]]", text);
			string text8 = array2[3];
			if (flag)
			{
				text8 = "<color=#FFDA00>" + text8 + "</color>";
			}
			string text9 = instance ? (text5 + text2 + text7 + " " + text8) : (text5 + text2 + text6 + text2 + text2 + text7 + text2 + text8);
			this.text.text = text9;
		}
		if (!ADOBase.isMobile || !Persistence.ShouldShowSpeedTrials() || !Persistence.IsWorldComplete(index) || !speedTrial)
		{
			return;
		}
		float num4 = Mathf.Max(Persistence.GetBestSpeedMultiplier(index), 1f);
		this.text.text = RDString.Get("levelSelect.SpeedTrialBest", new Dictionary<string, object>
		{
			{
				"bestMultiplier",
				num4.ToString("0.0")
			}
		});
		if (world != "B")
		{
			if (Persistence.IsSpeedTrialComplete(index))
			{
				this.text.color = ADOBase.gc.goldTextColor;
				return;
			}
			string value3 = Persistence.GetSpeedTrialAimForWorld(world).ToString("0.0");
			Text obj = this.text;
			obj.text = obj.text + " ~ " + RDString.Get("levelSelect.SpeedTrialAim", new Dictionary<string, object>
			{
				{
					"aimMultiplier",
					value3
				}
			});
		}
	}
}
