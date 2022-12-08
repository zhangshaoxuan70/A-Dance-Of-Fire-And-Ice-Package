using UnityEngine;

public class ffxChangeTrack : ffxBase
{
	public Color color1 = Color.clear;

	public Color color2 = Color.clear;

	public TrackColorType colorType;

	public float colorAnimDuration;

	public TrackColorPulse pulseType;

	public int pulseLength = 10;

	public TrackAnimationType animationType;

	public TrackAnimationType2 animationType2;

	public float tilesAhead = 3f;

	public float tilesBehind = 4f;

	public Texture2D texture;

	private float animDuration;

	public override void doEffect()
	{
	}

	public void PrepFloor()
	{
		floor.ColorFloor(colorType, color1, color2, colorAnimDuration / scrConductor.instance.song.pitch, pulseType, pulseLength);
		if (ADOBase.controller.usingInitialTrackStyles)
		{
			floor.SetTrackStyle(floor.initialTrackStyle);
		}
		floor.customTexture = texture;
		if (animationType != 0)
		{
			ffxFloorAppearPlus ffxFloorAppearPlus = base.gameObject.AddComponent<ffxFloorAppearPlus>();
			if (floor.seqID != 0)
			{
				ffxFloorAppearPlus.prevFloor = scrLevelMaker.instance.listFloors[floor.seqID - 1];
			}
			ffxFloorAppearPlus.tilesAhead = tilesAhead;
			ffxFloorAppearPlus.animType = animationType;
			ffxFloorAppearPlus.SetStartTime(ADOBase.conductor.bpm);
			floor.plusEffects.Add(ffxFloorAppearPlus);
		}
		if (animationType2 != 0)
		{
			ffxFloorDisappearPlus ffxFloorDisappearPlus = base.gameObject.AddComponent<ffxFloorDisappearPlus>();
			ffxFloorDisappearPlus.tilesBehind = tilesBehind;
			ffxFloorDisappearPlus.animType = animationType2;
			ffxFloorDisappearPlus.SetStartTime(ADOBase.conductor.bpm);
			floor.plusEffects.Add(ffxFloorDisappearPlus);
		}
	}
}
