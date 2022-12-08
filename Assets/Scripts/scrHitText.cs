using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class scrHitText : MonoBehaviour
{
	public Outline outline;

	public HitMargin hitMargin;

	public float startingSize;

	public float sizeUp;

	public float duration;

	public int vibrato;

	public float elasticity;

	[NonSerialized]
	public bool dead;

	private Text text;

	private CanvasGroup sf;

	private DOTweenAnimation anim;

	private float timer;

	private int frameShown;

	private Transform canvas;

	private bool forceOnScreen;

	private float minBorderDistance;

	private Camera gameCam;

	private Vector3 textPos;

	public void Init(HitMargin hitMargin)
	{
		this.hitMargin = hitMargin;
		base.transform.parent.gameObject.SetActive(value: false);
		text = GetComponent<Text>();
		text.SetLocalizedFont();
		if (RDString.language != SystemLanguage.Korean && text.font == RDConstants.data.latinFont)
		{
			text.fontSize = Mathf.RoundToInt((float)text.fontSize * 1.1f);
		}
		dead = true;
		canvas = base.transform.parent;
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
			text.color = hitMarginColours.colourTooLate;
			break;
		}
		if (GCS.bb)
		{
			text.fontSize = Mathf.RoundToInt((float)text.fontSize * 0.65f);
			if (hitMargin == HitMargin.EarlyPerfect || hitMargin == HitMargin.LatePerfect || hitMargin == HitMargin.Perfect)
			{
				text.color = hitMarginColours.colourPerfect;
			}
			else
			{
				text.color = hitMarginColours.colourTooLate;
			}
			outline.effectColor = Color.black;
			outline.useGraphicAlpha = false;
			outline.effectDistance = new Vector2(1f, 1f);
		}
		else
		{
			outline.useGraphicAlpha = false;
			outline.effectDistance = new Vector2(1f, 1f);
		}
		scrController instance = scrController.instance;
		gameCam = instance.camy.GetComponent<Camera>();
		forceOnScreen = instance.forceHitTextOnScreen;
		minBorderDistance = instance.hitTextMinBorderDistance;
	}

	public void Show(Vector3 position, float angle = 0f)
	{
		frameShown = Time.frameCount;
		timer = 0f;
		canvas.localPosition = position;
		canvas.gameObject.SetActive(value: true);
		dead = false;
		text.DOKill();
		text.color = text.color.WithAlpha(1f);
		text.DOFade(0f, 0.7f).SetDelay(0.5f).SetEase(Ease.OutQuad);
		scrMisc.Rotate2D(base.transform, scrController.instance.camy.transform.rotation.eulerAngles.z);
		base.transform.DOKill();
		base.transform.localScale = new Vector3(startingSize, startingSize, 1f);
		base.transform.DOPunchScale(new Vector3(sizeUp, sizeUp, 1f), duration, vibrato, elasticity);
		if (hitMargin != HitMargin.Perfect)
		{
			base.transform.DOLocalRotate(new Vector3(0f, 0f, angle * 20f), 2f, RotateMode.LocalAxisAdd);
		}
		if (GCS.bb)
		{
			canvas.localPosition = canvas.localPosition.WithY(1f);
			canvas.transform.eulerAngles = new Vector3(90f, 0f, 25f);
			text.GetComponent<Outline>().effectColor = Color.black;
			text.DOKill();
			text.color = text.color.WithAlpha(1f);
			text.DOFade(0f, 0.8f).SetDelay(0.25f).SetEase(Ease.OutQuad);
		}
		else
		{
			outline.effectColor = new Color(text.color.r * 0.3f, text.color.g * 0.3f, text.color.b * 0.3f, 1f);
			outline.DOFade(0f, 0.5f).SetDelay(0.25f).SetEase(Ease.OutQuad);
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
				canvas.localPosition = localPosition;
			}
			timer += Time.deltaTime;
			if (timer > 1.25f)
			{
				dead = true;
				base.transform.DOKill();
				text.DOKill();
				base.transform.parent.gameObject.SetActive(value: false);
			}
		}
	}
}
