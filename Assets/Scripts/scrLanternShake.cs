using System;
using UnityEngine;

public class scrLanternShake : ADOBase
{
	private float swingoffsetpct;

	private float c;

	private void Start()
	{
		swingoffsetpct = UnityEngine.Random.value;
		if (ADOBase.controller.levelName == "1-X")
		{
			SpriteRenderer component = GetComponent<SpriteRenderer>();
			string name = component.sprite.name;
			if (ADOBase.IsHalloweenWeek())
			{
				component.sprite = (name.Contains("lanterns_2") ? RDC.data.sprite_halloween_lantern_big : RDC.data.sprite_halloween_lantern_small);
			}
			else if (ADOBase.IsCNY())
			{
				component.sprite = RDC.data.sprite_cny_lantern;
			}
		}
	}

	private void Update()
	{
		if (RDC.debug && UnityEngine.Input.GetKeyDown(KeyCode.LeftShift))
		{
			if (UnityEngine.Input.GetKey(KeyCode.L))
			{
				Vector3 localScale = base.transform.localScale;
				localScale.x += 0.1f;
				localScale.y += 0.1f;
				base.transform.localScale = localScale;
				scrController.instance.txtCaption.text = localScale.x.ToString();
			}
			if (UnityEngine.Input.GetKey(KeyCode.K))
			{
				Vector3 localScale2 = base.transform.localScale;
				localScale2.x -= 0.1f;
				localScale2.y -= 0.1f;
				base.transform.localScale = localScale2;
				scrController.instance.txtCaption.text = localScale2.x.ToString();
			}
		}
		c += (float)(scrConductor.instance.deltaSongPos * (double)ADOBase.d_speed);
		scrMisc.Rotate2DCW(base.transform, 10f * Mathf.Sin(c * 5f + swingoffsetpct * MathF.PI));
	}
}
