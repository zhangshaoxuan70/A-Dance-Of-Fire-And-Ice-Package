using DG.Tweening;
using UnityEngine;

public class ffxBgColor : ffxBase
{
	public Color color;

	public float fadeTime;

	public override void doEffect()
	{
		if (fadeTime == 0f)
		{
			cam.Bgcamstatic.GetComponent<Camera>().backgroundColor = color;
		}
		else
		{
			cam.Bgcamstatic.GetComponent<Camera>().DOColor(color, fadeTime);
		}
	}
}
