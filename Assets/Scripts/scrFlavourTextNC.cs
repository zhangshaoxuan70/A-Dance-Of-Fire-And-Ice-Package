using System;
using UnityEngine;
using UnityEngine.UI;

public class scrFlavourTextNC : ADOBase
{
	private const int startX = 13;

	private const int spacing = 7;

	private const int bonusSpeedTrialX = 52;

	[NonSerialized]
	public Text text;

	private float ballX;

	private float ballY;

	private int lastRoundedX = -9999;

	private int lastRoundedY = -9999;

	private bool unlockedWorldT3;

	private bool beatWorldT4;

	private bool bottomRow;

	private void Awake()
	{
		text = GetComponent<Text>();
		text.SetLocalizedFont();
		unlockedWorldT3 = (Persistence.IsWorldComplete("T1") && Persistence.IsWorldComplete("T2"));
		beatWorldT4 = Persistence.IsWorldComplete("T4");
	}

	private void Update()
	{
		Vector3 position = ADOBase.controller.chosenplanet.transform.position;
		int num = Mathf.RoundToInt(position.x);
		int num2 = Mathf.RoundToInt(position.y);
		Vector2Int vector2Int = new Vector2Int(num, num2);
		if (lastRoundedX == num && lastRoundedY == num2)
		{
			return;
		}
		lastRoundedX = num;
		lastRoundedY = num2;
		bottomRow = (scrCamera.instance.positionState == PositionState.TaroMenu0BottomLane || scrCamera.instance.positionState == PositionState.TaroMenu2BottomLane || scrCamera.instance.positionState == PositionState.TaroMenu3BottomLane);
		UpdateAnchors(bottomRow);
		string[] allWorlds = GCNS.allWorlds;
		foreach (string text in allWorlds)
		{
			if (!text.IsTaro())
			{
				continue;
			}
			if (ADOBase.sceneName == "scnTaroMenu0")
			{
				if (ADOBase.worldData[text].jumpPortalPosition == vector2Int)
				{
					UpdateAnchors(text.EndsWith("EX"));
					ShowText(text);
				}
			}
			else if (ADOBase.sceneName == "scnTaroMenu1")
			{
				if (GCNS.activePositionTaroMenu1[text] == vector2Int && ((text == "T3" && unlockedWorldT3) || text == "T1" || text == "T2"))
				{
					ShowText(text);
					return;
				}
			}
			else if (ADOBase.sceneName == "scnTaroMenu2")
			{
				if (GCNS.activePositionTaroMenu2[text] == vector2Int)
				{
					if ((text == "T4" && beatWorldT4) || text == "T3" || text == "T1" || text == "T2")
					{
						UpdateAnchors(atTop: true);
						ShowText(text);
						return;
					}
					if (text == "T4" && !beatWorldT4)
					{
						UpdateAnchors(atTop: false);
						ShowTextRaw("???");
						return;
					}
				}
			}
			else if (ADOBase.sceneName == "scnTaroMenu3" && GCNS.activePositionTaroMenu3[text] == vector2Int && (text == "T5" || text == "T4" || text == "T3" || text == "T1" || text == "T2"))
			{
				UpdateAnchors(text != "T5");
				ShowText(text);
				return;
			}
		}
		ShowText(null);
	}

	private void UpdateAnchors(bool atTop)
	{
		RectTransform component = GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0f, atTop ? 1 : 0);
		component.anchorMax = new Vector2(1f, atTop ? 1 : 0);
		component.pivot = new Vector2(0.5f, atTop ? 0.85f : 0f);
	}

	public void ShowTextRaw(string t)
	{
		if (text == null)
		{
			text = GetComponent<Text>();
		}
		text.text = t;
	}

	public void ShowText(string world)
	{
		if (!(world == "B") || Persistence.GetOverallProgressStage() >= 7)
		{
			if (text == null)
			{
				text = GetComponent<Text>();
			}
			text.text = ((world == null) ? "" : RDString.Get("world" + world + ".description"));
		}
	}
}
