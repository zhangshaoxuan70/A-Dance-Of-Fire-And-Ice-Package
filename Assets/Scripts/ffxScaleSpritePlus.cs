using UnityEngine;

public class ffxScaleSpritePlus : ffxPlusBase
{
	public GameObject objScale;

	public Vector3 scale;

	public float time;

	private ffxSpriteScale scaleComp;

	public override void Awake()
	{
		base.Awake();
		scaleComp = base.gameObject.AddComponent<ffxSpriteScale>();
		scaleComp.spriteObject = objScale;
		scaleComp.scale = scale;
		scaleComp.ease = ease;
		scaleComp.usedByFfxPlus = true;
	}

	public override void StartEffect()
	{
		scaleComp.time = time / cond.song.pitch;
		scaleComp.doEffect();
	}

	public override void ScrubToTime(float t)
	{
	}
}
