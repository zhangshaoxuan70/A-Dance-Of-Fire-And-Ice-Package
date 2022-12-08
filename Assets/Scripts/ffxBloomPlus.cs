using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class ffxBloomPlus : ffxPlusBase
{
	public bool enableBloom;

	public float threshold;

	public float intensity;

	public Color color;

	private static VideoBloom videoBloom;

	private static Tween thresholdTween;

	private static Tween intensityTween;

	private static Tween colorTween;

	protected override IEnumerable<Tween> eventTweens => new Tween[3]
	{
		thresholdTween,
		intensityTween,
		colorTween
	};

	public override void Awake()
	{
		base.Awake();
		if (videoBloom == null)
		{
			videoBloom = cam.GetComponent<VideoBloom>();
		}
	}

	public override void StartEffect()
	{
		thresholdTween?.Kill(complete: true);
		intensityTween?.Kill(complete: true);
		colorTween?.Kill(complete: true);
		if (duration == 0f)
		{
			SetBloom(enableBloom, threshold, intensity, color);
			thresholdTween = null;
			intensityTween = null;
			colorTween = null;
		}
		else
		{
			videoBloom.enabled = enableBloom;
			thresholdTween = DOTween.To(() => videoBloom.Threshold, delegate(float t)
			{
				videoBloom.Threshold = t;
			}, threshold, duration).SetEase(ease);
			intensityTween = DOTween.To(() => videoBloom.MasterAmount, delegate(float i)
			{
				videoBloom.MasterAmount = i;
			}, intensity, duration).SetEase(ease);
			colorTween = DOTween.To(() => videoBloom.Tint, delegate(Color c)
			{
				videoBloom.Tint = c;
			}, color, duration).SetEase(ease);
		}
	}

	private void SetBloom(bool bEnable, float thresh, float inten, Color col)
	{
		videoBloom.enabled = bEnable;
		if (bEnable)
		{
			videoBloom.Threshold = thresh;
			videoBloom.MasterAmount = inten;
			videoBloom.Tint = col;
		}
	}
}
