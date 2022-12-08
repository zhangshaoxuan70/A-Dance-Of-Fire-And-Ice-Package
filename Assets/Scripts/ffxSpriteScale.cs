using DG.Tweening;
using UnityEngine;

public class ffxSpriteScale : ffxBase
{
	public GameObject spriteObject;

	public Vector3 scale;

	public float time;

	public Ease ease = Ease.Linear;

	public override void doEffect()
	{
		if (!(spriteObject == null))
		{
			spriteObject.SetActive(value: true);
			spriteObject.transform.DOScale(scale, time).SetEase(ease);
		}
	}
}
