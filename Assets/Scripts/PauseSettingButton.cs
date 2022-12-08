using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PauseSettingButton : GeneralPauseButton
{
	[Header("Components")]
	public Image rectangle;

	public Image fill;

	public Text label;

	public Text valueLabel;

	public Button rightArrow;

	public Button leftArrow;

	public Text info;

	public RectTransform infoMask;

	public RectTransform buttonContainer;

	[Header("Info")]
	public string type;

	public bool hasDescription;

	public string descriptionKey;

	public object initialValue;

	public bool restartOnChange;

	public int minInt;

	public int maxInt;

	public int changeBy;

	public int changeBySmall;

	public string unit;

	private Vector2 buttonImageStartSize;

	public bool hasRange => minInt != maxInt;

	private PauseMenu pauseMenu => scrController.instance.pauseMenu;

	private SettingsMenu settingsMenu => pauseMenu.settingsMenu;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		rectangleRT = rectangle.GetComponent<RectTransform>();
		buttonImageStartSize = rightArrow.GetComponentInChildren<RectTransform>().sizeDelta;
		leftArrow.onClick.AddListener(delegate
		{
			settingsMenu.UpdateSelectedSetting(SettingsMenu.Interaction.Decrement);
		});
		rightArrow.onClick.AddListener(delegate
		{
			settingsMenu.UpdateSelectedSetting(SettingsMenu.Interaction.Increment);
		});
		info.GetComponent<Button>().onClick.AddListener(delegate
		{
			settingsMenu.UpdateSelectedSetting(SettingsMenu.Interaction.ActivateInfo);
		});
		info.SetLocalizedFont();
	}

	public override void SetFocus(bool focus)
	{
		label.color = (focus ? pauseMenu.selectedLabelColor : pauseMenu.unselectedLabelColor);
		valueLabel.color = label.color;
		rectangle.color = (focus ? pauseMenu.selectedLabelColor : pauseMenu.unselectedBorderColor);
		fill.color = (focus ? pauseMenu.selectedFillColor : pauseMenu.unselectedFillColor);
		rightArrow.gameObject.SetActive(focus);
		leftArrow.gameObject.SetActive(focus);
		if (hasDescription)
		{
			info.text = RDString.Get(descriptionKey);
		}
		ContentSizeFitter component = pauseMenu.settingsMenu.settingsContainer.GetComponent<ContentSizeFitter>();
		if (!focus || hasDescription)
		{
			component.enabled = true;
			infoMask.SizeDeltaY(focus ? 48 : 24);
			component.SetLayoutVertical();
			component.enabled = false;
			component.enabled = true;
		}
		if (focus)
		{
			buttonContainer.DOKill(complete: true);
			buttonContainer.DOScale(1.02f, pauseMenu.animationTime).SetEase(pauseMenu.animationEase).SetUpdate(isIndependentUpdate: true)
				.OnComplete(delegate
				{
					buttonContainer.DOScale(1f, pauseMenu.animationTime).SetEase(pauseMenu.animationEase).SetUpdate(isIndependentUpdate: true);
				});
		}
	}

	public void PlayArrowAnimation(bool isRight)
	{
		RectTransform componentInChildren = (isRight ? rightArrow : leftArrow).GetComponentInChildren<RectTransform>();
		componentInChildren.DOComplete();
		componentInChildren.DOAnchorPosX(componentInChildren.anchoredPosition.x + pauseMenu.animationDistance * 0.25f * (float)(isRight ? 1 : (-1)), pauseMenu.animationTime).SetEase(pauseMenu.animationEase).SetUpdate(isIndependentUpdate: true)
			.SetLoops(2, LoopType.Yoyo);
	}

	public override void Select()
	{
		settingsMenu.isSelectingTab = false;
		settingsMenu.Select(this);
	}

	public void FlipDescription()
	{
		info.rectTransform.SetAnchorPosY(0f - info.rectTransform.anchoredPosition.y);
		info.rectTransform.pivot = new Vector2(0f, 1f);
		info.alignment = TextAnchor.UpperLeft;
	}
}
