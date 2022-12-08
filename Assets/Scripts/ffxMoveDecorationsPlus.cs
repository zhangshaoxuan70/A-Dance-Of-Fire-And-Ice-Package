using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class ffxMoveDecorationsPlus : ffxPlusBase
{
	public scrDecorationManager decManager;

	public Vector2 targetPos;

	public bool positionUsed = true;

	public float targetRot;

	public bool rotationUsed = true;

	public float targetScale = float.NaN;

	public Vector2 targetScaleV2;

	public bool scaleUsed = true;

	public Color targetColor;

	public bool colorUsed = true;

	public float targetOpacity = 1f;

	public bool opacityUsed = true;

	public Vector2 targetParallax;

	public bool parallaxUsed;

	public string targetImageFilename;

	public bool imageFilenameUsed;

	public int targetDepth;

	public bool depthUsed;

	public List<string> targetTags = new List<string>();

	public bool forceDontTweenMovement;

	protected override IEnumerable<Tween> eventTweens
	{
		get
		{
			List<Tween> list = new List<Tween>();
			foreach (string targetTag in targetTags)
			{
				if (decManager.taggedDecorations.TryGetValue(targetTag, out List<scrDecoration> value))
				{
					foreach (scrDecoration item in value)
					{
						list.AddRange(item.eventTweens.Values);
					}
				}
			}
			return list;
		}
	}

	public override void Awake()
	{
		base.Awake();
		hifiEffect = true;
	}

	public override void StartEffect()
	{
		if (ADOBase.controller.visualQuality != VisualQuality.Low)
		{
			if (!float.IsNaN(targetScale))
			{
				targetScaleV2 = new Vector2(targetScale, targetScale);
			}
			AdjustDurationForHardbake();
			List<scrDecoration> list = new List<scrDecoration>();
			Vector2 targetScaleVec = new Vector3(targetScaleV2.x, targetScaleV2.y);
			foreach (string targetTag in targetTags)
			{
				if (decManager.taggedDecorations.ContainsKey(targetTag))
				{
					foreach (scrDecoration dec in decManager.taggedDecorations[targetTag])
					{
						if (!list.Contains(dec))
						{
							Dictionary<TweenType, Tween> eventTweens = dec.eventTweens;
							if (!forceDontTweenMovement)
							{
								if (positionUsed)
								{
									if (eventTweens.ContainsKey(TweenType.Position))
									{
										eventTweens[TweenType.Position].Kill(complete: true);
									}
									if (duration != 0f)
									{
										Vector2 newPos = dec.pivotPosVec;
										eventTweens[TweenType.Position] = DOTween.To(() => newPos, delegate(Vector2 v)
										{
											newPos = v;
										}, dec.startPos + targetPos, duration).SetEase(ease).OnUpdate(delegate
										{
											dec.SetPosition(newPos, dec.pivotOffsetVec);
										})
											.OnComplete(delegate
											{
												dec.SetPosition(dec.startPos + targetPos, dec.pivotOffsetVec);
											});
									}
									else
									{
										dec.SetPosition(dec.startPos + targetPos, dec.pivotOffsetVec);
									}
								}
								if (rotationUsed)
								{
									if (eventTweens.ContainsKey(TweenType.Rotation))
									{
										eventTweens[TweenType.Rotation].Kill(complete: true);
									}
									if (duration != 0f)
									{
										float newRot = dec.rotAngle;
										eventTweens[TweenType.Rotation] = DOTween.To(() => newRot, delegate(float r)
										{
											newRot = r;
										}, targetRot, duration).SetEase(ease).OnUpdate(delegate
										{
											dec.SetRotation(newRot);
										})
											.OnComplete(delegate
											{
												dec.SetRotation(targetRot);
											});
									}
									else
									{
										dec.SetRotation(targetRot);
									}
								}
								if (scaleUsed)
								{
									if (eventTweens.ContainsKey(TweenType.Scale))
									{
										eventTweens[TweenType.Scale].Kill(complete: true);
									}
									if (duration != 0f)
									{
										eventTweens[TweenType.Scale] = dec.pivotTrans.DOScale(targetScaleVec, duration).SetEase(ease).OnComplete(delegate
										{
											dec.SetScale(targetScaleVec);
										});
									}
									else
									{
										dec.SetScale(targetScaleVec);
									}
								}
							}
							if (colorUsed)
							{
								if (eventTweens.ContainsKey(TweenType.Color))
								{
									eventTweens[TweenType.Color].Kill(complete: true);
								}
								if (duration != 0f)
								{
									Color newColor = dec.color;
									eventTweens[TweenType.Color] = DOTween.To(() => newColor, delegate(Color c)
									{
										newColor = c;
									}, targetColor, duration).SetEase(ease).OnUpdate(delegate
									{
										dec.SetColor(newColor);
									})
										.OnComplete(delegate
										{
											dec.SetColor(targetColor);
										});
								}
								else
								{
									dec.SetColor(targetColor);
								}
							}
							if (opacityUsed)
							{
								if (eventTweens.ContainsKey(TweenType.Opacity))
								{
									eventTweens[TweenType.Opacity].Kill(complete: true);
								}
								if (duration != 0f)
								{
									float newOpacity = dec.opacity;
									eventTweens[TweenType.Opacity] = DOTween.To(() => newOpacity, delegate(float a)
									{
										newOpacity = a;
									}, targetOpacity, duration).SetEase(ease).OnUpdate(delegate
									{
										dec.SetOpacity(newOpacity);
									})
										.OnComplete(delegate
										{
											dec.SetOpacity(targetOpacity);
										});
								}
								else
								{
									dec.SetOpacity(targetOpacity);
								}
							}
							if (imageFilenameUsed)
							{
								scrVisualDecoration scrVisualDecoration = dec as scrVisualDecoration;
								if ((object)scrVisualDecoration != null)
								{
									bool flag = !string.IsNullOrEmpty(targetImageFilename);
									Dictionary<string, scrExtImgHolder.CustomSprite> customSprites = scrDecorationManager.instance.imageHolder.customSprites;
									scrVisualDecoration.SetSprite(flag ? customSprites[targetImageFilename].sprite : null);
								}
							}
							if (parallaxUsed)
							{
								if (eventTweens.ContainsKey(TweenType.Parallax))
								{
									eventTweens[TweenType.Parallax].Kill(complete: true);
								}
								if (duration != 0f)
								{
									Vector2 newParallax = dec.parallax.ToVector();
									eventTweens[TweenType.Position] = DOTween.To(() => newParallax, delegate(Vector2 p)
									{
										newParallax = p;
									}, targetParallax, duration).SetEase(ease).OnUpdate(delegate
									{
										dec.SetParallax(targetParallax - newParallax, dec.placementType);
									})
										.OnComplete(delegate
										{
											dec.SetParallax(targetParallax, dec.placementType);
										});
								}
								else
								{
									dec.SetParallax(targetParallax, dec.placementType);
								}
							}
							if (depthUsed)
							{
								dec.SetDepth(targetDepth);
							}
							list.Add(dec);
						}
					}
				}
			}
		}
	}
}
