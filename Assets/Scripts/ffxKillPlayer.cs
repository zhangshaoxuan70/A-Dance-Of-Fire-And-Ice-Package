public class ffxKillPlayer : ffxPlusBase
{
	public bool instant;

	public string failMessage;

	public override void StartEffect()
	{
		if (RDC.auto || ctrl.currentState >= States.Fail || ctrl.noFail)
		{
			return;
		}
		bool flag = false;
		bool[] conditionalInfo = base.conditionalInfo;
		for (int i = 0; i < conditionalInfo.Length; i++)
		{
			if (conditionalInfo[i])
			{
				flag = true;
			}
		}
		if (flag)
		{
			ctrl.instantExplode = instant;
			ctrl.FailAction(overload: false, multipress: false, failMessage);
		}
	}
}
