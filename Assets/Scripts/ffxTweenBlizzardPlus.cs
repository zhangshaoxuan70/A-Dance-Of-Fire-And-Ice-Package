using DG.Tweening;

public class ffxTweenBlizzardPlus : ffxPlusBase
{
	public float durationBeats = 1f;

	public float startOpacity;

	public float endOpacity;

	private CameraFilterPack_Blizzard blizzFilter;

	public override void Awake()
	{
		base.Awake();
		SetStartTime(cond.bpm);
		blizzFilter = cam.GetComponent<CameraFilterPack_Blizzard>();
	}

	public override void StartEffect()
	{
		AdjustDurationForHardbake();
		float duration = durationBeats * (float)cond.crotchetAtStart / (floor.speed * cond.song.pitch);
		blizzFilter._Fade = startOpacity;
		DOTween.To(() => blizzFilter._Fade, delegate(float b)
		{
			blizzFilter._Fade = b;
		}, endOpacity, duration);
	}

	public override void ScrubToTime(float t)
	{
	}
}
