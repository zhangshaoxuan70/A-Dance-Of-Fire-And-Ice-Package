using DG.Tweening;
using UnityEngine;

public class ffxFearGrowsLightning : ffxPlusBase
{
	public float durationBeats = 1.5f;

	public SpriteRenderer[] lightningBolts;

	public override void Awake()
	{
		base.Awake();
		SetStartTime(cond.bpm);
		hifiEffect = true;
	}

	public override void StartEffect()
	{
		if (ADOBase.controller.visualQuality != VisualQuality.Low)
		{
			AdjustDurationForHardbake();
			float duration = durationBeats * (float)cond.crotchetAtStart / (floor.speed * cond.song.pitch);
			SpriteRenderer[] array = lightningBolts;
			foreach (SpriteRenderer obj in array)
			{
				obj.DOKill(complete: true);
				obj.color = obj.color.WithAlpha(1f);
				obj.DOColor(obj.color.WithAlpha(0f), duration);
			}
		}
	}

	public override void ScrubToTime(float t)
	{
	}
}
