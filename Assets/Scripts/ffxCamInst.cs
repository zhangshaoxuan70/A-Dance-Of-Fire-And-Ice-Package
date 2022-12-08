public class ffxCamInst : ffxBase
{
	public override void doEffect()
	{
		cam.ViewObjectInstant(ctrl.chosenplanet.transform);
	}
}
