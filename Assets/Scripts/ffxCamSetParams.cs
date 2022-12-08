public class ffxCamSetParams : ffxBase
{
	public float size;

	public float tweenDuration;

	public bool isPulsing;

	public bool isMoving;

	public override void doEffect()
	{
		cam.isPulsingOnHit = isPulsing;
		cam.isMoveTweening = isMoving;
		if (tweenDuration == 0f)
		{
			cam.camsizenormal = size;
		}
		else
		{
			cam.setCamSizeSmooth(size, tweenDuration);
		}
	}
}
