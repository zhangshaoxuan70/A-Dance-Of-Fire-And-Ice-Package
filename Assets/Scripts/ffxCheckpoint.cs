using UnityEngine;

public class ffxCheckpoint : ffxBase
{
	public int checkpointTileOffset;

	public bool scrubFourBack;

	public override void Awake()
	{
		base.Awake();
		if (floor.floorIcon == FloorIcon.None)
		{
			floor.floorIcon = ((!GCS.speedTrialMode && !GCS.practiceMode) ? FloorIcon.Checkpoint : FloorIcon.None);
		}
	}

	public override void doEffect()
	{
		if (!GCS.speedTrialMode && !GCS.practiceMode)
		{
			scrFlash.Flash(Color.white.WithAlpha(0.3f));
			GCS.checkpointNum = GetComponent<scrFloor>().seqID + checkpointTileOffset;
			ADOBase.controller.mistakesManager.MarkCheckpoint(checkpointTileOffset);
			floor.floorIcon = FloorIcon.Checkpoint;
			floor.UpdateIconSprite();
		}
	}
}
