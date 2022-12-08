using DG.Tweening;
using RDTools;

public class ffxOptionsShapeBlink : ffxPlusBase
{
	private scrOptionsWindows opWinRef;

	public override void Awake()
	{
		base.Awake();
		hifiEffect = true;
	}

	public void Start()
	{
		if (ADOBase.controller.visualQuality != VisualQuality.Low)
		{
			opWinRef = scrOptionsWindows.opWinRef;
		}
	}

	public override void StartEffect()
	{
		if (ADOBase.controller.visualQuality != VisualQuality.Low)
		{
			if (opWinRef == null)
			{
				opWinRef = scrOptionsWindows.opWinRef;
				RDBaseDll.printem("is null!");
			}
			opWinRef.SetIcons(OptionsShape.Cross, staticTime: true);
			DOVirtual.DelayedCall(0.5f, delegate
			{
				opWinRef.SetIcons(OptionsShape.Cross, staticTime: true);
			}).SetLoops(-1);
		}
	}

	public override void ScrubToTime(float t)
	{
	}
}
