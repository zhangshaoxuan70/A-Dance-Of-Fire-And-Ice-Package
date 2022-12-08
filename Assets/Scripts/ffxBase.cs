using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class ffxBase : ADOBase, IEventSystemHandler
{
	public bool hifiEffect;

	public bool disableIfMinFx;

	public float timeoffset;

	[NonSerialized]
	public scrCamera cam;

	[NonSerialized]
	public scrController ctrl;

	[NonSerialized]
	public scrConductor cond;

	public const string gizmoIconFilename = "star.png";

	[NonSerialized]
	public float beatDelay;

	[NonSerialized]
	public scrFloor floor;

	[NonSerialized]
	public bool usedByFfxPlus;

	private Sequence delayTimer;

	public virtual void Awake()
	{
		floor = GetComponent<scrFloor>();
		cond = scrConductor.instance;
		cam = scrCamera.instance;
		ctrl = scrController.instance;
	}

	public abstract void doEffect();

	public void doEffectDelay()
	{
		float delay = timeoffset;
		timeoffset = 0f;
		DOVirtual.DelayedCall(delay, delegate
		{
			doEffect();
		});
	}

	public void beginEffect()
	{
		if (cond == null)
		{
			cond = scrConductor.instance;
		}
		DOVirtual.DelayedCall(beatDelay * (60f / cond.bpm), delegate
		{
			doEffect();
		});
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "star.png", allowScaling: true);
	}
}
