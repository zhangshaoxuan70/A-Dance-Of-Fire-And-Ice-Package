using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class scrImageFader : ADOBase
{
	public int activephase;

	private Color oricolor;

	private Image image;

	private bool fadeToggle = true;

	private void Start()
	{
		image = GetComponent<Image>();
		oricolor = image.color;
		if (ADOBase.controller.menuPhase != activephase)
		{
			image.enabled = false;
		}
	}

	private void Update()
	{
		if (ADOBase.controller.menuPhase == activephase)
		{
			if (!fadeToggle)
			{
				fadeToggle = true;
				image.DOColor(oricolor, 0.4f);
			}
		}
		else if (fadeToggle)
		{
			fadeToggle = false;
			image.DOColor(image.color.WithAlpha(0f), 0.4f);
		}
	}
}
