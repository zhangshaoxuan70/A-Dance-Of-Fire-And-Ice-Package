using UnityEngine;

public class scrSfx : ADOBase
{
	[SerializeField]
	private AudioSource src;

	[SerializeField]
	private float baseVolume = 1f;

	public static scrSfx instance
	{
		get;
		private set;
	}

	private void Awake()
	{
		instance = this;
		src.ignoreListenerPause = true;
	}

	public AudioClip PlaySfx(SfxSound sound, float volume = 1f)
	{
		AudioClip[] soundEffects = ADOBase.gc.soundEffects;
		src.volume = baseVolume * volume;
		src.pitch = 1f;
		AudioClip audioClip = soundEffects[(int)sound];
		src.PlayOneShot(audioClip);
		return audioClip;
	}

	public AudioClip PlaySfxPitch(SfxSound sound, float volume = 1f, float pitch = 1f)
	{
		AudioClip[] soundEffects = ADOBase.gc.soundEffects;
		src.volume = baseVolume * volume;
		src.pitch = pitch;
		AudioClip audioClip = soundEffects[(int)sound];
		src.PlayOneShot(audioClip);
		return audioClip;
	}
}
