using UnityEngine;

public class ffxSetHitsound : ffxBase
{
	public GameSound gameSound;

	public HitSound hitSound = HitSound.ReverbClack;

	[Range(0f, 1f)]
	public float volume = 1f;

	public override void doEffect()
	{
	}
}
