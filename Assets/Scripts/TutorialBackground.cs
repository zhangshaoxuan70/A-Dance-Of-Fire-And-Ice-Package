using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBackground : ADOBase
{
	public List<Transform> elements;

	private Vector3[] startScales;

	private bool on;

	private int lastFloorID;

	private void Awake()
	{
		elements = new List<Transform>();
		foreach (Transform item in base.transform)
		{
			if (item.gameObject.activeSelf && item.name.Contains("opt_tutorial"))
			{
				elements.Add(item);
			}
		}
		startScales = new Vector3[elements.Count];
		int num = 0;
		foreach (Transform element in elements)
		{
			startScales[num] = element.localScale;
			num++;
		}
	}

	private void LateUpdate()
	{
		bool flag = ADOBase.controller.state == States.Countdown && Time.frameCount == ADOBase.conductor.onBeatFrame;
		if ((ADOBase.controller.currentFloorID > lastFloorID) | flag)
		{
			int num = 0;
			foreach (Transform element in elements)
			{
				if (Random.value < 0.3f)
				{
					element.DOPunchScale(startScales[num] * 0.1f, (float)ADOBase.conductor.crotchet / 2f, 5);
				}
				num++;
			}
			lastFloorID = ADOBase.controller.currentFloorID;
		}
	}
}
