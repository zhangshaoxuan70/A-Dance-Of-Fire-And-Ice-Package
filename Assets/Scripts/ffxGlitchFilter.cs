public class ffxGlitchFilter : ffxBase
{
	public bool enable;

	public override void doEffect()
	{
		cam.GetComponent<CameraFilterPack_FX_Glitch1>().enabled = enable;
	}
}
