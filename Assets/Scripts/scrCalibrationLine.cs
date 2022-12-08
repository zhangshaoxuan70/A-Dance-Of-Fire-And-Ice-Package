using DG.Tweening;
using System;
using UnityEngine;

public class scrCalibrationLine : MonoBehaviour
{
	public scrCalibrationPlanet cplanet;

	private SpriteRenderer sprite;

	public static scrCalibrationLine instance;

	private void Awake()
	{
		instance = this;
		sprite = GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		float rotation = (0f - (float)cplanet.averageAngleOffset) * 180f / MathF.PI;
		scrMisc.Rotate2D(base.transform, rotation);
	}

	public void FadeIn()
	{
		if (!sprite.enabled)
		{
			sprite.enabled = true;
			sprite.color = new Color(1f, 1f, 1f, 0f);
			sprite.DOFade(1f, 0.6f).SetEase(Ease.Linear);
		}
	}
}
