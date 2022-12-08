using UnityEngine;

public class scrRing : ADOBase
{
	public scrPlanet planet;

	public Vector3 scaleStart;

	public Vector3 scaleEnd;

	public float scaletimer;

	public float scaleduration = 0.1f;

	public bool modeflag = true;

	public bool scaleHasResetOnDeath;

	private void Update()
	{
		float num = ADOBase.controller.isCW ? 1f : (-1f);
		base.transform.Rotate(Vector3.back, num * Time.deltaTime * 30f);
		if (planet.onlyRing)
		{
			base.transform.localScale = Vector3.one * 0.4f;
			return;
		}
		if (planet.dead && !scaleHasResetOnDeath)
		{
			scaleStart = Vector3.one;
			scaleEnd = Vector3.zero;
			scaletimer = 0f;
			scaleHasResetOnDeath = true;
		}
		if (planet.isChosen && !modeflag)
		{
			scaleHasResetOnDeath = false;
			modeflag = true;
			scaleStart = Vector3.zero;
			float num2 = 2f - planet.transform.localScale.x;
			if (scrController.instance.isbigtiles)
			{
				num2 = 1.6f;
			}
			if (planet.currfloor != null && !planet.dead && planet.currfloor.nextfloor != null)
			{
				num2 *= planet.currfloor.nextfloor.radiusScale;
			}
			if (planet.currfloor != null && !planet.dead && planet.currfloor.nextfloor == null)
			{
				num2 *= planet.currfloor.radiusScale;
			}
			scaleEnd = Vector3.one * num2;
			scaletimer = 0f;
		}
		if (!planet.isChosen && modeflag)
		{
			scaleHasResetOnDeath = false;
			modeflag = false;
			scaleStart = Vector3.one;
			scaleEnd = Vector3.zero;
			scaletimer = 0f;
		}
		scaletimer += Time.deltaTime;
		base.transform.localScale = Vector3.Slerp(scaleStart, scaleEnd, scaletimer / scaleduration);
	}
}
