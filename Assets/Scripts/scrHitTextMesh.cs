using DG.Tweening;
using System;
using UnityEngine;

public class scrHitTextMesh : ADOBase
{
	[NonSerialized]
	public bool dead;

	private TextMesh text;

	private CanvasGroup sf;

	private DOTweenAnimation anim;

	public HitMargin hitMargin;

	private float timer;

	private int frameShown;

	private bool forceOnScreen;

	private float minBorderDistance;

	private Camera gameCam;

	private Vector3 textPos;

	private Renderer meshRenderer;

	public float startingSize;

	public float sizeUp;

	public float duration;

	public int vibrato;

	public float elasticity;

	public void Init(HitMargin hitMargin)
	{
		this.hitMargin = hitMargin;
		base.gameObject.SetActive(value: false);
		text = GetComponent<TextMesh>();
		meshRenderer = GetComponent<Renderer>();
		text.SetLocalizedFont();
		if (RDString.language != SystemLanguage.Korean && text.font == RDConstants.data.latinFont)
		{
			text.fontSize = Mathf.RoundToInt((float)text.fontSize * 1.1f);
		}
		dead = true;
		ColourSchemeHitMargin hitMarginColours = RDConstants.data.hitMarginColours;
		text.text = RDString.Get("HitMargin." + hitMargin.ToString());
		switch (hitMargin)
		{
		case HitMargin.TooEarly:
			text.color = hitMarginColours.colourTooEarly;
			break;
		case HitMargin.VeryEarly:
			text.color = hitMarginColours.colourVeryEarly;
			break;
		case HitMargin.EarlyPerfect:
			text.color = hitMarginColours.colourLittleEarly;
			break;
		case HitMargin.Perfect:
			text.color = hitMarginColours.colourPerfect;
			break;
		case HitMargin.LatePerfect:
			text.color = hitMarginColours.colourLittleLate;
			break;
		case HitMargin.VeryLate:
			text.color = hitMarginColours.colourVeryLate;
			break;
		case HitMargin.TooLate:
			text.color = hitMarginColours.colourTooLate;
			break;
		case HitMargin.Multipress:
			text.color = hitMarginColours.colourMultipress;
			break;
		case HitMargin.FailMiss:
			text.color = hitMarginColours.colourFail;
			break;
		case HitMargin.FailOverload:
			text.color = hitMarginColours.colourFail;
			break;
		}
		scrController instance = scrController.instance;
		gameCam = instance.camy.GetComponent<Camera>();
		forceOnScreen = instance.forceHitTextOnScreen;
		minBorderDistance = instance.hitTextMinBorderDistance;
		meshRenderer.sortingOrder = 100;
	}

	public void Show(Vector3 position, float angle = 0f)
	{
		frameShown = Time.frameCount;
		timer = 0f;
		base.transform.localPosition = position;
		base.transform.gameObject.SetActive(value: true);
		dead = false;
		Material material = meshRenderer.material;
		material.DOKill();
		material.color = Color.white;
		material.DOFade(0f, 0.7f).SetDelay(0.5f).SetEase(Ease.OutQuad);
		scrMisc.Rotate2D(base.transform, scrController.instance.camy.transform.rotation.eulerAngles.z);
		base.transform.DOKill();
		base.transform.localScale = new Vector3(startingSize, startingSize, 1f);
		base.transform.DOPunchScale(new Vector3(sizeUp, sizeUp, 1f), duration, vibrato, elasticity);
		if (hitMargin != HitMargin.Perfect)
		{
			base.transform.DOLocalRotate(new Vector3(0f, 0f, angle * 20f), 2f, RotateMode.LocalAxisAdd);
		}
		textPos = position;
	}

	private void Update()
	{
		if (!dead)
		{
			if (forceOnScreen)
			{
				float num = gameCam.orthographicSize * 2f;
				float num2 = num * (float)Screen.width / (float)Screen.height;
				Vector3 position = gameCam.transform.position;
				Vector3 vector = textPos - position;
				Vector3 localPosition = textPos;
				localPosition.x = position.x + Mathf.Clamp(vector.x, (0f - num2) / 2f + minBorderDistance, num2 / 2f - minBorderDistance);
				localPosition.y = position.y + Mathf.Clamp(vector.y, (0f - num) / 2f + minBorderDistance, num / 2f - minBorderDistance);
				base.transform.localPosition = localPosition;
			}
			timer += Time.deltaTime;
			if (timer > 1.25f)
			{
				dead = true;
				base.transform.DOKill();
				text.DOKill();
				base.gameObject.SetActive(value: false);
			}
		}
	}
}
