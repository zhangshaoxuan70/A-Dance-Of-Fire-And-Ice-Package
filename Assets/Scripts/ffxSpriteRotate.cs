using DG.Tweening;
using System;
using UnityEngine;

public class ffxSpriteRotate : ffxBase
{
	public GameObject spriteObject;

	public Vector3 angleDegrees;

	public float time;

	[NonSerialized]
	public RotateMode rotateMode;

	public override void doEffect()
	{
		if (!(spriteObject == null) && ADOBase.controller.visualQuality != VisualQuality.Low)
		{
			if (time < 0f)
			{
				time *= -60f / (cond.bpm * GetComponent<scrFloor>().speed) / cond.song.pitch;
			}
			spriteObject.SetActive(value: true);
			spriteObject.transform.DOLocalRotate(angleDegrees, time, rotateMode);
		}
	}
}
