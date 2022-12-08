using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class scrOptionsWindows : ADOBase
{
	private static scrOptionsWindows _opWinRef;

	public float windowSpeed = 1f;

	public float bounceHeight = 0.5f;

	public float bounceWidth = 0.5f;

	public float bounceLength = 1f;

	public Ease ease = Ease.OutCubic;

	private Sprite circleSprite;

	private Sprite squareSprite;

	private Sprite crossSprite;

	private Sprite plusSprite;

	private Tween bounceTween;

	private List<Transform> windows = new List<Transform>();

	private static List<SpriteRenderer> icons = new List<SpriteRenderer>();

	private scrConductor cond;

	private float farthestX;

	public static scrOptionsWindows opWinRef
	{
		get
		{
			if (_opWinRef == null)
			{
				_opWinRef = Object.FindObjectsOfType<scrOptionsWindows>()[0];
			}
			return _opWinRef;
		}
	}

	private void Awake()
	{
		if (ADOBase.controller.visualQuality == VisualQuality.Low)
		{
			base.gameObject.SetActive(value: false);
		}
		circleSprite = ADOBase.gc.sprite_options_circle;
		squareSprite = ADOBase.gc.sprite_options_square;
		crossSprite = ADOBase.gc.sprite_options_cross;
		plusSprite = ADOBase.gc.sprite_options_plus;
		if (icons.Count != 0 && icons[0] == null)
		{
			icons.Clear();
		}
		foreach (Transform item in base.transform)
		{
			windows.Add(item);
			icons.Add(item.GetComponentsInChildren<SpriteRenderer>()[1]);
			float x = item.localPosition.x;
			if (x > farthestX)
			{
				farthestX = x;
			}
		}
	}

	private void Start()
	{
		if (bounceHeight != 1f || bounceLength != 1f)
		{
			cond = scrConductor.instance;
			cond.onBeats.Add(this);
		}
	}

	public override void OnBeat()
	{
		if (ADOBase.conductor.hasSongStarted && ADOBase.conductor.beatNumber >= 0 && ADOBase.controller.currentState == States.PlayerControl)
		{
			if (bounceTween != null)
			{
				bounceTween.Kill();
			}
			bool num = ADOBase.conductor.beatNumber % 2 == 0;
			float y = num ? bounceHeight : 1f;
			float x = num ? 1f : bounceWidth;
			float duration = bounceLength * (float)cond.crotchetAtStart / cond.song.pitch;
			base.transform.localScale = new Vector3(x, y, 1f);
			bounceTween = base.transform.DOScale(Vector3.one, duration).SetEase(ease);
		}
	}

	private void Update()
	{
		foreach (Transform window in windows)
		{
			window.localPosition += Vector3.right * (Time.deltaTime * windowSpeed) * cond.song.pitch;
			if (windowSpeed < 0f && window.localPosition.x <= 0f - farthestX)
			{
				window.localPosition += Vector3.right * (farthestX * 2f);
			}
			else if (windowSpeed > 0f && window.localPosition.x >= farthestX)
			{
				window.localPosition += Vector3.left * (farthestX * 2f);
			}
		}
	}

	public void SetIcons(OptionsShape shape, bool staticTime = false)
	{
		float duration = 0.5f;
		scrFloor currFloor = ADOBase.controller.currFloor;
		if (!staticTime && currFloor != null && currFloor.seqID <= ADOBase.lm.listFloors.Count - 3)
		{
			duration = (float)(currFloor.nextfloor.nextfloor.entryTime - currFloor.nextfloor.entryTime) / cond.song.pitch;
		}
		Sprite sprite = circleSprite;
		switch (shape)
		{
		case OptionsShape.Square:
			sprite = squareSprite;
			break;
		case OptionsShape.Cross:
			sprite = crossSprite;
			break;
		case OptionsShape.Plus:
			sprite = plusSprite;
			break;
		}
		foreach (SpriteRenderer icon in icons)
		{
			if (icon.isVisible)
			{
				icon.sprite = sprite;
				icon.DOKill();
				icon.color = Color.black.WithAlpha(1f);
				icon.DOBlendableColor(Color.black.WithAlpha(0f), duration);
			}
		}
	}
}
