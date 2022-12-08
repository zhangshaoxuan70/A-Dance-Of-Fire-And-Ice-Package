using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ffxFadeIn : ffxBase
{
	public GameObject objFade;

	public float time;

	public float value;

	public bool includeChildren = true;

	private SpriteRenderer sprFade;

	public override void doEffect()
	{
		if (objFade == null)
		{
			return;
		}
		objFade.SetActive(value: true);
		sprFade = objFade.GetComponent<SpriteRenderer>();
		Text component = objFade.GetComponent<Text>();
		Image component2 = objFade.GetComponent<Image>();
		if (sprFade != null)
		{
			sprFade.DOFade(value, time);
		}
		else if (component != null)
		{
			component.DOFade(value, time);
		}
		else if (component2 != null)
		{
			component2.DOFade(value, time);
		}
		Transform[] array = (!includeChildren) ? new Transform[1]
		{
			objFade.GetComponent<Transform>()
		} : objFade.GetComponentsInChildren<Transform>();
		Transform[] array2 = array;
		foreach (Transform obj in array2)
		{
			SpriteRenderer component3 = obj.GetComponent<SpriteRenderer>();
			Text component4 = obj.GetComponent<Text>();
			Image component5 = obj.GetComponent<Image>();
			if (component3 != null)
			{
				component3.DOFade(value, time);
			}
			else if (component4 != null)
			{
				component4.DOFade(value, time);
			}
			else if (component5 != null)
			{
				component5.DOFade(value, time);
			}
			obj.gameObject.SetActive(value: true);
		}
		for (int j = 0; j < objFade.transform.childCount; j++)
		{
			objFade.transform.GetChild(j).gameObject.SetActive(value: true);
		}
	}
}
