using UnityEngine;

public class ffxLowQualityBlizzard : ffxBase
{
	private SpriteRenderer sprFade;

	public float crotchets;

	public Color color;

	public override void doEffect()
	{
		if (ADOBase.controller.visualQuality == VisualQuality.Low)
		{
			scrFlash.FlashReverse(color, crotchets);
		}
	}
}
