using DG.Tweening;

public class ffxScriptEnable : ffxBase
{
	public DOTweenPath path;

	public bool enable;

	public override void doEffect()
	{
		if (path != null)
		{
			if (enable)
			{
				path.DOPlay();
			}
			else
			{
				path.DOPause();
			}
		}
	}
}
