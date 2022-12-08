public class ffxPulseMag : ffxBase
{
	public float pulsemag;

	public bool justOnce;

	private float oldPulseMag;

	public override void doEffect()
	{
		if (ADOBase.controller.visualEffects != 0)
		{
			cam.isPulsingOnHit = true;
			oldPulseMag = cam.pulsemagnitude;
			cam.pulsemagnitude = pulsemag;
			if (justOnce)
			{
				Timer.Add(delegate
				{
					cam.pulsemagnitude = oldPulseMag;
				}, 0.03f);
			}
		}
	}
}
