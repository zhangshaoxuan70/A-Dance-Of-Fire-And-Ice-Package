using UnityEngine.Audio;

public class ffxPlaySound : ffxPlusBase
{
	public HitSound hitSound;

	public float volume = 0.5f;

	private AudioMixerGroup group;

	public override void Awake()
	{
		base.Awake();
		startEffectOffset = 1.0;
		group = RDUtils.GetMixerGroup("ConductorPlaySound");
	}

	public override void StartEffect()
	{
		double num = ADOBase.conductor.dspTimeSongPosZero + startTime / (double)ADOBase.conductor.song.pitch;
		double value = 0.0;
		ADOBase.gc.hitSoundOffsets.TryGetValue(hitSound, out value);
		AudioManager.Play("snd" + hitSound.ToString(), num - value, group, volume);
	}

	public override void ScrubToTime(float t)
	{
		if ((double)t > startTime - 1.0)
		{
			triggered = true;
		}
	}
}
