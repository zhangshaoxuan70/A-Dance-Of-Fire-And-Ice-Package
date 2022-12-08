using System;
using UnityEngine;

public class scrParallax : ADOBase
{
	public int d_id;

	public Vector3 posCamAtStart;

	public bool relativeToCamera;

	[NonSerialized]
	public Transform cameraTransform;

	public float multiplier_x;

	public float multiplier_y;

	private Vector3 startpos;

	public bool dontalter_x;

	public bool dontalter_y;

	public bool clampToScreen;

	public Vector2 screenRelativePos;

	private void Awake()
	{
		startpos = base.gameObject.transform.position;
		cameraTransform = ADOBase.controller.camy.transform;
	}

	public void SetNewStartPosition(Vector3 startpos)
	{
		this.startpos = startpos;
	}

	public void SetTrans()
	{
		Vector3 position = base.transform.position;
		if (clampToScreen)
		{
			Vector2 v = new Vector2((float)Screen.width * screenRelativePos.x, (float)Screen.height * screenRelativePos.y);
			Vector3 position2 = ADOBase.controller.camy.camobj.ScreenToWorldPoint(v).WithZ(position.z);
			base.transform.position = position2;
			return;
		}
		Vector3 position3 = new Vector3(position.x, position.y, startpos.z);
		Vector3 position4 = cameraTransform.position;
		if (!dontalter_x)
		{
			position3.x = startpos.x + (position4.x - posCamAtStart.x) * multiplier_x + (relativeToCamera ? posCamAtStart.x : 0f);
		}
		if (!dontalter_y)
		{
			position3.y = startpos.y + (position4.y - posCamAtStart.y) * multiplier_y + (relativeToCamera ? posCamAtStart.y : 0f);
		}
		base.transform.position = position3;
	}

	public Vector2 ToVector()
	{
		return new Vector2(multiplier_x, multiplier_y);
	}

	private void LateUpdate()
	{
		SetTrans();
	}
}
