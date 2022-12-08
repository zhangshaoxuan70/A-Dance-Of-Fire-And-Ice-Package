using RDTools;

public class ffxSetOptionsIcons : ffxPlusBase
{
	private scrOptionsWindows opWinRef;

	public OptionsShape shape;

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
			opWinRef.SetIcons(shape);
		}
	}

	public override void ScrubToTime(float t)
	{
	}
}
