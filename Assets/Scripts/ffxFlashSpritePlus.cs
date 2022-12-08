using UnityEngine;

public class ffxFlashSpritePlus : ffxPlusBase
{
	public GameObject objFade;

	public float time;

	private ffxFlashSprite flashComp;

	public override void Awake()
	{
		base.Awake();
		flashComp = base.gameObject.AddComponent<ffxFlashSprite>();
		flashComp.objFade = objFade;
		flashComp.usedByFfxPlus = true;
	}

	public override void StartEffect()
	{
		flashComp.time = time / cond.song.pitch;
		flashComp.doEffect();
	}

	public override void ScrubToTime(float t)
	{
	}
}
