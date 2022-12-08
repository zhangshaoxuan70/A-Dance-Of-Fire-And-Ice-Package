using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class ffxCameraPlus : ffxPlusBase
{
	public Vector2 targetPos;

	public bool positionUsed = true;

	public float targetRot;

	public bool rotationUsed = true;

	public float targetZoom;

	public bool zoomUsed = true;

	private Transform camParent;

	public CamMovementType movementType;

	public bool movementTypeUsed = true;

	public Vector3 floorPos;

	public bool dontDisable;

	private static Tween moveTween;

	private static Tween rotationTween;

	private static Tween zoomTween;

	public static bool legacyRelativeTo;

	public bool movementTypeIsLastPosition
	{
		get
		{
			if (movementType != CamMovementType.LastPosition)
			{
				return movementType == CamMovementType.LastPositionNoRotation;
			}
			return true;
		}
	}

	protected override IEnumerable<Tween> eventTweens => new Tween[3]
	{
		moveTween,
		rotationTween,
		zoomTween
	};

	public override void Awake()
	{
		base.Awake();
		disableIfMinFx = !dontDisable;
		if (disableIfMaxFx)
		{
			disableIfMinFx = false;
		}
		if (disableIfMaxFx && ADOBase.controller.visualEffects == VisualEffects.Minimum)
		{
			dontDisable = true;
		}
		if (ADOBase.customLevel != null)
		{
			camParent = ADOBase.customLevel.camParent;
		}
		else
		{
			camParent = scrCamera.instance.transform.parent;
		}
		floorPos = floor.transform.position;
	}

	public void ForceUpdateCamParent()
	{
		cam = scrCamera.instance;
		camParent = scrCamera.instance.transform.parent;
		ctrl = scrController.instance;
		vfx = scrVfxPlus.instance;
	}

	public override void StartEffect()
	{
		if (positionUsed || movementTypeUsed)
		{
			moveTween?.Kill(complete: true);
		}
		if (rotationUsed || (movementTypeUsed && movementTypeIsLastPosition))
		{
			rotationTween?.Kill(complete: true);
		}
		if (zoomUsed)
		{
			zoomTween?.Kill(complete: true);
		}
		AdjustDurationForHardbake();
		float num = 0f;
		Vector2 v = Vector2.zero;
		CamMovementType camMovementType = movementTypeUsed ? movementType : cam.lastUsedMovementType;
		Vector2 vector = positionUsed ? targetPos : (cam.lastEventRelativePosition - (Vector2)camParent.position);
		if (legacyRelativeTo && !movementTypeUsed)
		{
			Vector2 vector2 = camParent.position;
			v = ((!positionUsed) ? vector2 : (targetPos + vector2));
		}
		else
		{
			switch (camMovementType)
			{
			case CamMovementType.Player:
				if (!cam.followMode)
				{
					Vector3 position = camParent.position;
					camParent.position = position - ctrl.chosenplanet.transform.position;
					cam.transform.position = position + Vector3.back * 10f;
					cam.frompos = cam.transform.localPosition;
					cam.topos = ctrl.chosenplanet.transform.position + Vector3.back * 10f;
					cam.followMode = true;
				}
				v = vector;
				break;
			case CamMovementType.Tile:
				if (cam.followMode)
				{
					camParent.position = cam.transform.position - Vector3.back * 10f;
					cam.SetToFreeMode();
				}
				cam.lastEventRelativePosition = floorPos;
				v = vector + (Vector2)floorPos;
				break;
			case CamMovementType.Global:
				if (cam.followMode)
				{
					camParent.position = cam.transform.position - Vector3.back * 10f;
					cam.SetToFreeMode();
				}
				cam.lastEventRelativePosition = Vector2.zero;
				v = vector;
				break;
			case CamMovementType.LastPosition:
			case CamMovementType.LastPositionNoRotation:
			{
				Vector2 vector3 = camParent.position;
				if (camMovementType == CamMovementType.LastPosition)
				{
					num = vfx.camAngle;
				}
				v = ((!positionUsed) ? vector3 : (targetPos + vector3));
				break;
			}
			}
		}
		if (movementTypeUsed)
		{
			cam.lastUsedMovementType = movementType;
		}
		if (duration == 0f)
		{
			if (positionUsed || movementTypeUsed)
			{
				camParent.position = v;
				vfx.camAngle = targetRot + num;
				moveTween = null;
			}
			if (rotationUsed || (movementTypeUsed && movementTypeIsLastPosition))
			{
				rotationTween = null;
			}
			if (zoomUsed)
			{
				cam.zoomSize = targetZoom;
				zoomTween = null;
			}
		}
		else
		{
			if (positionUsed || movementTypeUsed)
			{
				moveTween = DOTween.To(() => camParent.position, delegate(Vector3 x)
				{
					camParent.position = x;
				}, v, duration).SetEase(ease);
			}
			if (rotationUsed || (movementTypeUsed && movementTypeIsLastPosition))
			{
				rotationTween = DOTween.To(() => vfx.camAngle, delegate(float x)
				{
					vfx.camAngle = x;
				}, targetRot + num, duration).SetEase(ease);
			}
			if (zoomUsed)
			{
				zoomTween = DOTween.To(() => cam.zoomSize, delegate(float x)
				{
					cam.zoomSize = x;
				}, targetZoom, duration).SetEase(ease);
			}
		}
	}

	public override void ScrubToTime(float t)
	{
		if (!dontDisable && ADOBase.controller.visualEffects == VisualEffects.Minimum)
		{
			triggered = true;
			return;
		}
		if (disableIfMaxFx && ADOBase.controller.visualEffects == VisualEffects.Full)
		{
			triggered = true;
			return;
		}
		base.ScrubToTime(t);
		bool flag = movementTypeUsed ? (movementType == CamMovementType.Player) : (cam.lastUsedMovementType == CamMovementType.Player);
		if ((double)t > startTime + (double)duration && flag)
		{
			cam.ViewObjectInstant(ctrl.chosenplanet.transform);
		}
	}
}
