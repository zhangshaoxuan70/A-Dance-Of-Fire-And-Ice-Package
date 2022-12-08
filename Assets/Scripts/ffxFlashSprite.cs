using DG.Tweening;
using UnityEngine;

public class ffxFlashSprite : ffxBase
{
	public GameObject objFade;

	public float time;

	private SpriteRenderer sprFade;

	public override void doEffect()
	{
		if (objFade == null)
		{
			printe("ObjFade is null");
			return;
		}
		objFade.SetActive(value: true);
		sprFade = objFade.GetComponent<SpriteRenderer>();
		if (sprFade != null)
		{
			sprFade.color = sprFade.color.WithAlpha(1f);
			sprFade.DOFade(0f, time);
		}
		SpriteRenderer[] componentsInChildren = objFade.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer obj in componentsInChildren)
		{
			obj.color = obj.color.WithAlpha(1f);
			obj.DOFade(0f, time);
			obj.gameObject.SetActive(value: true);
		}
		for (int j = 0; j < objFade.transform.childCount; j++)
		{
			objFade.transform.GetChild(j).gameObject.SetActive(value: true);
		}
	}
}
