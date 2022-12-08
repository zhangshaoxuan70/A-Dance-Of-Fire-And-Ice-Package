public class ffxSetMenuPhase : ffxBase
{
	public int phase;

	public override void doEffect()
	{
		ADOBase.controller.menuPhase = phase;
	}
}
