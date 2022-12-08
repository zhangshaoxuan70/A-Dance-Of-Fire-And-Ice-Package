using System;
using UnityEngine;
using UnityEngine.UI;

public class scrFlavourText : ADOBase
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

	private void Awake()
	{
		text = GetComponent<Text>();
		text.SetLocalizedFont();
	}

	private void Update()
	{
		if (scnLevelSelect.instance.playingCutscene)
		{
			return;
		}
		Vector3 position = ADOBase.controller.chosenplanet.transform.position;
		int num = Mathf.RoundToInt(position.x);
		int num2 = Mathf.RoundToInt(position.y);
		Vector2Int rhs = new Vector2Int(num, num2);
		if (lastRoundedX == num && lastRoundedY == num2)
		{
			return;
		}
		lastRoundedX = num;
		lastRoundedY = num2;
		UpdateAnchors(scrCamera.instance.positionState == PositionState.DLC);
		string[] allWorlds = GCNS.allWorlds;
		foreach (string text in allWorlds)
		{
			if (text.IsXtra())
			{
				break;
			}
			if (ADOBase.worldData[text].jumpPortalPosition == rhs)
			{
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
