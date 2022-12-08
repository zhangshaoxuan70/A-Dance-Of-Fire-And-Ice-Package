using DG.Tweening;
using UnityEngine;

public class ffxCamShake : ffxBase
{
	private Tween tween;

	public float strength;

	public float duration;

	public override void doEffect()
	{
		if (tween != null)
		{
			tween.Kill();
			tween = null;
		}
		tween = DOTween.Shake(() => cam.shake, delegate(Vector3 x)
		{
			cam.shake = x;
		}, duration, strength);
	}
}
