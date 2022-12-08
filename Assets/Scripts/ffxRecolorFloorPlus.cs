using UnityEngine;

public class ffxRecolorFloorPlus : ffxPlusBase
{
	private scrLevelMaker levelMaker;

	public int start;

	public int end;

	public Color color1 = Color.clear;

	public Color color2 = Color.clear;

	public TrackColorType colorType;

	public float colorAnimDuration;

	public TrackColorPulse pulseType;

	public int pulseLength = 10;

	public TrackStyle style;

	public override void Awake()
	{
		base.Awake();
		levelMaker = ADOBase.lm;
	}

	public override void StartEffect()
	{
		AdjustDurationForHardbake();
		if (end < start)
		{
			int num = end;
			end = start;
			start = num;
		}
		for (int i = start; i <= end; i++)
		{
			scrFloor scrFloor = levelMaker.listFloors[i];
			base.enabled = false;
			scrFloor.styleNum = (int)style;
			scrFloor.UpdateAngle(rotate: false);
			scrFloor.SetTrackStyle(style);
			scrFloor.ColorFloor(colorType, color1, color2, colorAnimDuration / cond.song.pitch, pulseType, pulseLength, start);
		}
	}
}
