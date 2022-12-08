using UnityStandardAssets.ImageEffects;

public class ffxCamRestore : ffxBase
{
	public override void doEffect()
	{
		cam.isPulsingOnHit = true;
		cam.isMoveTweening = true;
		scrMisc.Rotate2DCW(cam.transform, 0f);
		cam.torot = 0f;
		cam.fromrot = 0f;
		cam.BGcam.GetComponent<Grayscale>().enabled = false;
		cam.BGcam.GetComponent<Blur>().enabled = false;
		cam.Bgcamstatic.GetComponent<Grayscale>().enabled = false;
	}
}
