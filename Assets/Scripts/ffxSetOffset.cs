public class ffxSetOffset : ffxBase
{
	public float xoffset;

	public float yoffset;

	public override void doEffect()
	{
		cam.SetYOffset(yoffset);
		cam.SetXOffset(xoffset);
	}
}
