public class ffxFloorDisappear_ZeroBehind : ffxBase
{
	public ffxFloorDisappearPlus parent;

	public override void doEffect()
	{
		if (ADOBase.controller.visualQuality == VisualQuality.Low && parent.hifiEffect)
		{
			parent.StartEffect();
		}
	}
}
