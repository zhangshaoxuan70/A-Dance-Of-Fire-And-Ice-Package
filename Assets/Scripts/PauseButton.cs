using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PauseButton : GeneralPauseButton
{
	public PauseMenu.ButtonType buttonType;

	public Button button;

	public Image fill;

	public Image rectangle;

	public Image icon;

	public Text label;

	public string rdString;

	private float initialY;

	private bool focused;

	private Tween fillTween;

	private Tween labelTween;

	private Tween iconTween;

	private Tween borderTween;

	private scrController wrldGame => scrController.instance;

	private void Awake()
	{
		rectangleRT = rectangle.GetComponent<RectTransform>();
		initialY = rectangleRT.anchoredPosition.y;
	}

	public override void SetFocus(bool focus)
	{
		if (fillTween != null)
		{
			fillTween.Kill();
			labelTween.Kill();
			iconTween.Kill();
			borderTween.Kill();
		}
		PauseMenu pauseMenu = wrldGame.pauseMenu;
		label.color = (focus ? pauseMenu.selectedLabelColor : pauseMenu.unselectedLabelColor);
		icon.color = (focus ? pauseMenu.selectedIconColor : pauseMenu.unselectedIconColor);
		rectangle.color = (focus ? pauseMenu.selectedIconColor : pauseMenu.unselectedBorderColor);
		fill.color = (focus ? pauseMenu.selectedFillColor : pauseMenu.unselectedFillColor);
		if (focus)
		{
			rectangleRT.DOKill();
			rectangleRT.anchoredPosition = rectangleRT.anchoredPosition.WithY(initialY);
			rectangleRT.DOLocalMoveY(initialY + pauseMenu.animationDistance, pauseMenu.animationTime).SetEase(pauseMenu.animationEase).SetUpdate(isIndependentUpdate: true)
				.SetLoops(2, LoopType.Yoyo);
			RectTransform rectTransform = label.rectTransform;
			rectTransform.DOKill();
			rectTransform.anchoredPosition = rectTransform.anchoredPosition.WithY(-48f);
			rectTransform.DOAnchorPosY(-48f - pauseMenu.animationDistance * 0.25f, pauseMenu.animationTime).SetEase(pauseMenu.animationEase).SetUpdate(isIndependentUpdate: true)
				.SetLoops(2, LoopType.Yoyo);
		}
		focused = focus;
	}

	public override void Select()
	{
		PauseMenu pauseMenu = wrldGame.pauseMenu;
		if (pauseMenu.enabled)
		{
			pauseMenu.Select(index);
			pauseMenu.Choose();
		}
	}

	public void ShowAsSelected()
	{
		if (fillTween != null)
		{
			fillTween.Kill();
			labelTween.Kill();
			iconTween.Kill();
			borderTween.Kill();
		}
		label.color = Color.white;
		icon.color = Color.white;
		rectangle.color = Color.white;
		fill.color = Color.Lerp(wrldGame.pauseMenu.selectedFillColor, Color.white, 0.5f);
		Ease ease = Ease.OutExpo;
		float duration = 0.6f;
		float delay = 0.05f;
		fillTween = fill.DOColor(wrldGame.pauseMenu.selectedFillColor, duration).SetEase(ease).SetUpdate(isIndependentUpdate: true)
			.SetDelay(delay);
		iconTween = icon.DOColor(wrldGame.pauseMenu.selectedIconColor, duration).SetEase(ease).SetUpdate(isIndependentUpdate: true)
			.SetDelay(delay);
		borderTween = rectangle.DOColor(wrldGame.pauseMenu.selectedIconColor, duration).SetEase(ease).SetUpdate(isIndependentUpdate: true)
			.SetDelay(delay);
		labelTween = label.DOColor(wrldGame.pauseMenu.selectedLabelColor, duration).SetEase(ease).SetUpdate(isIndependentUpdate: true)
			.SetDelay(delay);
		focused = false;
	}

	private void Update()
	{
		PauseMenu pauseMenu = wrldGame.pauseMenu;
		if (focused)
		{
			float value = 0.5f + Mathf.Sin(Time.realtimeSinceStartup * pauseMenu.selectedButtonTintSpeed) * 0.6f;
			value = Mathf.Clamp01(value);
			label.color = Color.Lerp(pauseMenu.otherTintColor, pauseMenu.selectedLabelColor, value);
			icon.color = Color.Lerp(pauseMenu.otherTintColor, pauseMenu.selectedIconColor, value);
			rectangle.color = Color.Lerp(pauseMenu.otherTintColor, pauseMenu.selectedIconColor, value);
		}
	}
}
