using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scrShowOnlyInPhase : ADOBase
{
	private const float FadeDuration = 0.3f;

	public int activephase;

	public bool alsoActivateObjWhenFadeIn;

	public List<SpriteRenderer> spriteList;

	public List<Text> textList;

	public List<GameObject> goList;

	public List<Image> imageList;

	private List<float> imageAlphas = new List<float>();

	private bool isVisible = true;

	private void Awake()
	{
		foreach (Image image in imageList)
		{
			imageAlphas.Add(image.color.a);
		}
	}

	private void Update()
	{
		bool flag = ADOBase.controller.menuPhase == activephase;
		if (flag != isVisible)
		{
			Fade(flag);
		}
	}

	private void Fade(bool visible)
	{
		SpriteRenderer component = GetComponent<SpriteRenderer>();
		Text component2 = GetComponent<Text>();
		isVisible = visible;
		float endValue = visible ? 1f : 0f;
		component?.DOFade(endValue, 0.3f);
		component2?.DOFade(endValue, 0.3f);
		SpriteRenderer[] componentsInChildren = GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer spriteRenderer in componentsInChildren)
		{
			spriteRenderer.DOFade(endValue, 0.3f);
			if (visible && alsoActivateObjWhenFadeIn)
			{
				spriteRenderer.gameObject.SetActive(value: true);
			}
		}
		foreach (SpriteRenderer sprite in spriteList)
		{
			sprite?.DOFade(visible ? 1f : 0f, 0.3f);
		}
		foreach (Text text in textList)
		{
			text?.DOFade(visible ? 1f : 0f, 0.3f);
		}
		int num = 0;
		foreach (Image image in imageList)
		{
			image?.DOFade(visible ? imageAlphas[num] : 0f, 0.3f);
			num++;
		}
		foreach (GameObject go in goList)
		{
			if (visible && alsoActivateObjWhenFadeIn)
			{
				go.SetActive(value: true);
			}
			componentsInChildren = go.GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer obj in componentsInChildren)
			{
				float num2 = 1f;
				if (obj.transform.parent.TryGetComponent(out scrFloor component3) && component3.dontChangeMySprite)
				{
					num2 = 0.3f;
				}
				obj.DOFade(visible ? num2 : 0f, 0.3f);
			}
			Text[] componentsInChildren2 = go.GetComponentsInChildren<Text>();
			for (int i = 0; i < componentsInChildren2.Length; i++)
			{
				componentsInChildren2[i].DOFade(visible ? 1f : 0f, 0.3f);
			}
		}
	}
}
