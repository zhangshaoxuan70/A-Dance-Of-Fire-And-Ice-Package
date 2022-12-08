using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class ffxShakeScreenPlus : ffxPlusBase
{
	public float strength;

	public float intensity;

	public bool fadeOut;

	private static Tween shakeTween;

	protected override IEnumerable<Tween> eventTweens => new Tween[1]
	{
		shakeTween
	};

	public override void StartEffect()
	{
		AdjustDurationForHardbake();
		if (ADOBase.controller.visualEffects != 0 && duration != 0f)
		{
			shakeTween?.Kill(complete: true);
			shakeTween = DOTween.Shake(() => cam.shake, delegate(Vector3 x)
			{
				cam.shake = x;
			}, duration, strength, Mathf.RoundToInt(20f * intensity), 90f, ignoreZAxis: false, fadeOut);
			shakeTween.OnComplete(delegate
			{
				cam.shake = Vector3.zero;
			});
		}
	}
}
