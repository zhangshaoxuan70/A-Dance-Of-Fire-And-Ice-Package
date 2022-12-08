using ByteSheep.Events;

public class ffxCallFunction : ffxBase
{
	public QuickEvent ue;

	public override void doEffect()
	{
		if (base.enabled)
		{
			ue.Invoke();
		}
	}
}
