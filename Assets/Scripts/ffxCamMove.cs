public class ffxCamMove : ffxBase
{
	public bool follow;

	public override void doEffect()
	{
		cam.isMoveTweening = follow;
	}
}
