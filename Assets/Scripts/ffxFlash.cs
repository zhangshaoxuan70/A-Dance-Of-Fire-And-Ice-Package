using UnityEngine;

public class ffxFlash : ffxBase
{
	public ffxFlashStyle flashstyle;

	public bool reverse;

	public bool kill;

	public float timeInCrotchets;

	public Color color = Color.white;

	public Color secondaryColor = Color.white;

	public override void Awake()
	{
		base.Awake();
		disableIfMinFx = true;
	}

	public override void doEffect()
	{
		if (!Mathf.Approximately(timeoffset, 0f))
		{
			doEffectDelay();
			return;
		}
		if (flashstyle == ffxFlashStyle.Flash)
		{
			scrFlash.Flash(color);
		}
		if (flashstyle == ffxFlashStyle.Reverse)
		{
			scrFlash.FlashReverse(color, timeInCrotchets);
		}
		if (flashstyle == ffxFlashStyle.StayBlack)
		{
			scrFlash.FlashBlackStay();
		}
		if (flashstyle == ffxFlashStyle.Kill)
		{
			scrFlash.FlashKill();
		}
		if (flashstyle == ffxFlashStyle.FlashEx)
		{
			float num = cond.bpm * cond.song.pitch * (float)ctrl.speed;
			scrFlash.FlashEx(color, secondaryColor, timeInCrotchets * 60f / num);
		}
	}
}
