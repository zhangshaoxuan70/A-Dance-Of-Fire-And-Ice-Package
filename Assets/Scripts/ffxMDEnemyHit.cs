using System;

public class ffxMDEnemyHit : ffxBase
{
	public Action onHit;

	public override void doEffect()
	{
		onHit();
	}
}
