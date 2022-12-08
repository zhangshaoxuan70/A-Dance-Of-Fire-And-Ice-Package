using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class ffxMoveFloorPlus : ffxPlusBase
{
	private scrLevelMaker levelMaker;

	public int start;

	public int end;

	public Vector2 targetPos;

	public bool positionUsed = true;

	public float targetRot;

	public bool rotationUsed = true;

	public float targetScale = float.NaN;

	public Vector2 targetScaleV2;

	public bool scaleUsed = true;

	public float targetOpacity;

	public bool opacityUsed = true;

	public int gapLength;

	protected override IEnumerable<Tween> eventTweens
	{
		get
		{
			List<Tween> list = new List<Tween>();
			List<scrFloor> listFloors = levelMaker.listFloors;
			for (int i = start; i <= end; i += 1 + gapLength)
			{
				scrFloor scrFloor = listFloors[i];
				list.AddRange(scrFloor.moveTweens.Values);
			}
			return list;
		}
	}

	public override void Awake()
	{
		base.Awake();
		levelMaker = ADOBase.lm;
	}

	private void Start()
	{
		if (!float.IsNaN(targetScale))
		{
			targetScaleV2 = new Vector2(targetScale, targetScale);
		}
	}

	public override void StartEffect()
	{
		AdjustDurationForHardbake();
		if (end < start)
		{
			int num = end;
			end = start;
			start = num;
		}
		Vector3 b = new Vector3(targetPos.x, targetPos.y, 0f);
		Vector3 b2 = new Vector3(0f, 0f, targetRot);
		Vector3 vector = new Vector3(targetScaleV2.x, targetScaleV2.y, 1f);
		List<scrFloor> listFloors = levelMaker.listFloors;
		for (int i = start; i <= end; i += 1 + gapLength)
		{
			scrFloor target = listFloors[i];
			Transform targetTransform = target.transform;
			Material material = target.floorRenderer.material;
			Dictionary<TweenType, Tween> moveTweens = target.moveTweens;
			Vector3 vector2 = target.startPos + b;
			float z = (target.startRot + b2).z;
			if (positionUsed)
			{
				if (moveTweens.ContainsKey(TweenType.Position))
				{
					moveTweens[TweenType.Position].Kill(complete: true);
				}
				if (!targetTransform.position.ApproximatelyXY(vector2))
				{
					if (duration != 0f)
					{
						moveTweens[TweenType.Position] = targetTransform.DOMove(vector2, duration).SetEase(ease);
					}
					else
					{
						targetTransform.position = vector2;
					}
				}
			}
			if (rotationUsed)
			{
				if (moveTweens.ContainsKey(TweenType.Rotation))
				{
					moveTweens[TweenType.Rotation].Kill(complete: true);
				}
				if (!Mathf.Approximately(targetTransform.eulerAngles.z, z))
				{
					if (duration != 0f)
					{
						moveTweens[TweenType.Rotation] = DOTween.To(() => target.tweenRot.z, delegate(float r)
						{
							target.tweenRot.z = r;
						}, (target.startRot + b2).z, duration).SetEase(ease).OnUpdate(delegate
						{
							targetTransform.eulerAngles = target.tweenRot;
						});
					}
					else
					{
						target.tweenRot.z = (target.startRot + b2).z;
						targetTransform.eulerAngles = target.tweenRot;
					}
				}
			}
			if (scaleUsed)
			{
				if (moveTweens.ContainsKey(TweenType.Scale))
				{
					moveTweens[TweenType.Scale].Kill(complete: true);
				}
				if (!targetTransform.localScale.ApproximatelyXY(vector))
				{
					if (duration != 0f)
					{
						moveTweens[TweenType.Scale] = targetTransform.DOScale(vector, duration).SetEase(ease);
					}
					else
					{
						targetTransform.localScale = vector;
					}
				}
			}
			if (!opacityUsed)
			{
				continue;
			}
			if (moveTweens.ContainsKey(TweenType.Opacity))
			{
				moveTweens[TweenType.Opacity].Kill(complete: true);
			}
			if (!Mathf.Approximately(target.opacity, targetOpacity))
			{
				Tween tween = target.TweenOpacity(targetOpacity, duration, ease);
				if (tween != null)
				{
					moveTweens[TweenType.Opacity] = tween;
				}
			}
		}
	}
}
