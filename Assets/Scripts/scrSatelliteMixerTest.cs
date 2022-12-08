using RDTools;
using UnityEngine.UI;

public class scrSatelliteMixerTest : ADOBase
{
	public void Start()
	{
	}

	public void SetMixerMasterVolume(Slider slider)
	{
		RDUtils.SetMixerVolume("MasterVolume", slider.value);
		RDBaseDll.printem(slider.value);
	}

	public void SetMixerConductorMusicVolume(Slider slider)
	{
		RDUtils.SetMixerVolume("MusicVolume", slider.value);
		RDBaseDll.printem(slider.value);
	}

	public void SetMixerConductorHitsoundsVolume(Slider slider)
	{
		RDUtils.SetMixerVolume("HitsoundsVolume", slider.value);
		RDBaseDll.printem(slider.value);
	}
}
