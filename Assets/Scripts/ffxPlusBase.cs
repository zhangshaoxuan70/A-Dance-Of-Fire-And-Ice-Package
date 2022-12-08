using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ffxPlusBase : ADOBase
{
	public bool hifiEffect;

	public bool disableIfMinFx;

	public bool disableIfMaxFx;

	[NonSerialized]
	public scrCamera cam;

	[NonSerialized]
	public scrController ctrl;

	[NonSerialized]
	public scrConductor cond;

	[NonSerialized]
	public scrVfxPlus vfx;

	[NonSerialized]
	public scrFloor floor;

	public double startTime;

	public double startEffectOffset;

	public float duration;

	public Ease ease = Ease.Linear;

	public bool triggered;

	public float degreeOffset;

	[Tooltip("if true, it sets duration /= conductor.pitch ")]
	public bool hardbakedInScene;

	public bool[] conditionalInfo = new bool[5];

	public const string gizmoIconFilename = "star.png";

	protected virtual IEnumerable<Tween> eventTweens => null;

	public virtual void Awake()
	{
		cam = scrCamera.instance;
		ctrl = scrController.instance;
		cond = scrConductor.instance;
		vfx = scrVfxPlus.instance;
		floor = GetComponent<scrFloor>();
	}

	public abstract void StartEffect();

	public virtual void SetStartTime(float bpm, float degreeOffset = 0f)
	{
		scrFloor component = base.gameObject.GetComponent<scrFloor>();
		double startTime2 = startTime;
		startTime = component.entryTime + (double)(degreeOffset * (MathF.PI / 180f) / MathF.PI * (60f / (bpm * component.speed)));
		this.degreeOffset = degreeOffset;
	}

	public virtual void ScrubToTime(float t)
	{
		if ((double)t < startTime)
		{
			return;
		}
		StartEffect();
		triggered = true;
		if ((double)t >= startTime + (double)duration)
		{
			if (eventTweens != null)
			{
				foreach (Tween eventTween in eventTweens)
				{
					eventTween?.Kill(complete: true);
				}
			}
			return;
		}
		float to = (float)((double)t - startTime);
		if (eventTweens != null)
		{
			foreach (Tween eventTween2 in eventTweens)
			{
				eventTween2?.Goto(to, andPlay: true);
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "star.png", allowScaling: true);
	}

	public void AdjustDurationForHardbake()
	{
		if (!ADOBase.editor)
		{
			duration /= scrConductor.instance.song.pitch;
		}
	}
}
