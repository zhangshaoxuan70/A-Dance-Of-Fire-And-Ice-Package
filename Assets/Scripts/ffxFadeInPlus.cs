using UnityEngine;

public class ffxFadeInPlus : ffxPlusBase
{
	public GameObject objFade;

	public float time;

	public float value;

	private ffxFadeIn fadeComp;

	public override void Awake()
	{
		base.Awake();
		fadeComp = base.gameObject.AddComponent<ffxFadeIn>();
		fadeComp.objFade = objFade;
		fadeComp.time = time;
		fadeComp.value = value;
		fadeComp.usedByFfxPlus = true;
	}

	public override void StartEffect()
	{
		AdjustDurationForHardbake();
		if (duration != 0f)
		{
			fadeComp.time = duration;
		}
		fadeComp.doEffect();
	}

	public override void ScrubToTime(float t)
	{
	}
}
