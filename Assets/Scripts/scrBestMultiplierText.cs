using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scrBestMultiplierText : ADOBase
{
	private const int startX = 13;

	private const int spacing = 7;

	private const int bonusSpeedTrialX = 52;

	public string world;

	public Color color1;

	public Color color2;

	[NonSerialized]
	public bool planetIsIn;

	private Text text;

	private bool trialComplete;

	private Vector2Int prevPlanetPosition = new Vector2Int(-1000, -1000);

	private Vector2Int speedTrialPosition;

	private new scrController controller;

	private new string sceneName;

	private void Start()
	{
		sceneName = ADOBase.sceneName;
		controller = scrController.instance;
		text = GetComponent<Text>();
		text.SetLocalizedFont();
		UpdateText(showLongVersion: false);
		Vector2 a = Vector2.zero;
		if (world.IsTaro())
		{
			if (sceneName == "scnTaroMenu0")
			{
				a = GCNS.jumpPositionTaroMenu0[world];
			}
			else if (sceneName == "scnTaroMenu1")
			{
				a = GCNS.activePositionTaroMenu1[world];
			}
			else if (sceneName == "scnTaroMenu2")
			{
				a = GCNS.activePositionTaroMenu2[world];
			}
			else if (sceneName == "scnTaroMenu3")
			{
				a = GCNS.activePositionTaroMenu3[world];
			}
			a += Vector2.right;
		}
		else
		{
			a = ADOBase.worldData[world].trialPortalPosition;
		}
		speedTrialPosition = Vector2Int.RoundToInt(a);
	}

	private void Update()
	{
		if (!controller.setupComplete || world.IsXtra() || world.IsMuseDashWorld())
		{
			return;
		}
		bool flag = false;
		Vector2Int vector2Int = Vector2Int.RoundToInt(controller.chosenplanet.transform.position);
		if (vector2Int != prevPlanetPosition)
		{
			prevPlanetPosition = vector2Int;
			flag = (speedTrialPosition == vector2Int);
			if (flag != planetIsIn)
			{
				planetIsIn = flag;
				UpdateText(planetIsIn);
			}
		}
	}

	public void UpdateText(bool showLongVersion)
	{
		if (!ADOBase.worldData.ContainsKey(world))
		{
			printe("doesn't contain: " + world);
		}
		int index = ADOBase.worldData[world].index;
		float b = ADOBase.worldData[world].hasCheckpoints ? 0f : 1f;
		float num = Mathf.Max(Persistence.GetBestSpeedMultiplier(index), b);
		if (!showLongVersion)
		{
			this.text.text = RDString.Get("levelSelect.multiplier", new Dictionary<string, object>
			{
				{
					"multiplier",
					num.ToString("0.0")
				}
			});
			if (Persistence.IsSpeedTrialComplete(index))
			{
				this.text.color = ADOBase.gc.goldTextColor;
			}
			if (world.IsXtra() || world.IsMuseDashWorld())
			{
				this.text.color = this.text.color.WithAlpha(0f);
			}
			return;
		}
		this.text.text = RDString.Get("levelSelect.SpeedTrialBest", new Dictionary<string, object>
		{
			{
				"bestMultiplier",
				num.ToString("0.0")
			}
		});
		if (world != "B")
		{
			if (Persistence.IsSpeedTrialComplete(index))
			{
				this.text.color = ADOBase.gc.goldTextColor;
			}
			else
			{
				string text = "0";
				string key = (world == "XO") ? "levelSelect.SpeedTrialAimNoCheckpoints" : "levelSelect.SpeedTrialAim";
				text = Persistence.GetSpeedTrialAimForWorld(world).ToString("0.0");
				Text obj = this.text;
				obj.text = obj.text + "\n" + RDString.Get(key, new Dictionary<string, object>
				{
					{
						"aimMultiplier",
						text
					}
				});
			}
			if (world.IsXtra() || world.IsMuseDashWorld())
			{
				this.text.color = this.text.color.WithAlpha(1f);
			}
		}
	}
}
