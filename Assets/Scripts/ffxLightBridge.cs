using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ffxLightBridge : ffxBase
{
	public int tilesAhead;

	public int tileRange;

	public float brightness = 0.5f;

	public Color color = Color.white;

	private List<scrFloor> floors = new List<scrFloor>();

	public bool turnOff;

	public void Start()
	{
		List<scrFloor> listFloors = ADOBase.lm.listFloors;
		int seqID = GetComponent<scrFloor>().seqID;
		int num = Math.Max(0, seqID + tilesAhead);
		int num2 = Math.Min(seqID + tilesAhead + tileRange + 1, listFloors.Count);
		for (int i = num; i < num2; i++)
		{
			scrFloor scrFloor = ADOBase.lm.listFloors[i];
			floors.Add(scrFloor);
			scrFloor.floorRenderer.material.SetFloat("_Flash", brightness);
			if (!turnOff)
			{
				scrFloor.opacity = 0f;
			}
		}
	}

	public override void doEffect()
	{
		float duration = 60f / (cond.bpm * GetComponent<scrFloor>().speed);
		foreach (scrFloor floor in floors)
		{
			floor.floorRenderer.color = color;
			float endValue = turnOff ? 0f : 1f;
			DOTween.To(() => floor.opacity, delegate(float o)
			{
				floor.opacity = o;
			}, endValue, duration).SetEase(Ease.Flash, 5f, 0f);
			if (turnOff)
			{
				floor.topGlow.DOColor(floor.topGlow.color * new Color(1f, 1f, 1f, 0f), duration).SetEase(Ease.Flash, 5f, 0f);
				floor.dontChangeMySprite = true;
			}
		}
	}
}
