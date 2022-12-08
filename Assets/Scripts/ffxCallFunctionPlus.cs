using ByteSheep.Events;

public class ffxCallFunctionPlus : ffxPlusBase
{
	public QuickEvent ue;

	private ffxCallFunction callComp;

	public override void Awake()
	{
		base.Awake();
		callComp = base.gameObject.AddComponent<ffxCallFunction>();
		callComp.ue = ue;
		callComp.usedByFfxPlus = true;
	}

	public override void StartEffect()
	{
		callComp.doEffect();
	}

	public override void ScrubToTime(float t)
	{
	}
}
