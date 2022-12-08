public class ffxHallOfMirrorsPlus : ffxPlusBase
{
	public bool enableHOM;

	public override void StartEffect()
	{
		AdjustDurationForHardbake();
		cam.Bgcamstatic.enabled = !enableHOM;
	}
}
