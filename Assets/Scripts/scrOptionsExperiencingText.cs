using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scrOptionsExperiencingText : ADOBase
{
	public CanvasScaler canvasScaler;

	public List<Text> experiencingTexts;

	public float speed = -150f;

	public float distance = 200f;

	public float width;

	public float fullDistance;

	private string baseString;

	private bool init;

	private bool generatedWidth;

	private void Awake()
	{
		if (ADOBase.controller.visualQuality != VisualQuality.Low && baseString == null)
		{
			Init(0f);
		}
	}

	private void Update()
	{
		if (!init)
		{
			return;
		}
		if (!generatedWidth)
		{
			StartCoroutine(GetWidthCo());
		}
		if (generatedWidth)
		{
			for (int i = 0; i < experiencingTexts.Count; i++)
			{
				Text text = experiencingTexts[i];
				scrMisc.AnchorPosX(x: (fullDistance * (float)i + Time.unscaledTime * speed) % ((0f - fullDistance) * (float)experiencingTexts.Count), rt: text.rectTransform);
			}
		}
	}

	private IEnumerator GetWidthCo()
	{
		Text text = experiencingTexts[0];
		text.text = baseString.Replace("[angle]", "90");
		yield return null;
		float num = new TextGenerator().GetPreferredWidth(settings: text.GetGenerationSettings(new Vector2(100000f, 100000f)), str: text.text);
		width = num * canvasScaler.referenceResolution.y / (float)Screen.height;
		fullDistance = distance + width;
		generatedWidth = true;
		(base.transform as RectTransform).AnchorPosY(0f);
		base.gameObject.SetActive(value: false);
	}

	private void Init(float angle)
	{
		if (!init)
		{
			init = true;
			baseString = RDString.Get("frumsOptions.experiencing");
			if (baseString == null)
			{
				baseString = "[angle]";
			}
			SetAngleText(angle);
		}
	}

	public void SetAngleText(float angle)
	{
		if (!init)
		{
			Init(angle);
		}
		foreach (Text experiencingText in experiencingTexts)
		{
			experiencingText.text = baseString.Replace("[angle]", angle.ToString());
		}
	}
}
