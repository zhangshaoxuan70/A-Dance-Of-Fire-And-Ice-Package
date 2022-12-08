using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SettingsTabButton : GeneralPauseButton
{
	public Image rectangle;

	public Image fill;

	public Text label;

	public Image icon;

	private PauseMenu pauseMenu => scrController.instance.pauseMenu;

	private SettingsMenu settingsMenu => pauseMenu.settingsMenu;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		rectangleRT = rectangle.GetComponent<RectTransform>();
	}

	public override void SetFocus(bool focus)
	{
		SetFocus(focus, dontChangeSize: false);
	}

	public void SetFocus(bool focus, bool dontChangeSize)
	{
		label.color = (focus ? pauseMenu.selectedLabelColor : pauseMenu.unselectedLabelColor);
		icon.color = label.color;
		rectangle.color = (focus ? pauseMenu.selectedLabelColor : pauseMenu.unselectedBorderColor);
		fill.color = (focus ? pauseMenu.selectedFillColor : pauseMenu.unselectedFillColor);
		if (!dontChangeSize)
		{
			Vector2 endValue = rectTransform.sizeDelta.WithX(focus ? 80f : rectTransform.sizeDelta.y);
			rectTransform.DOKill(complete: true);
			rectTransform.DOSizeDelta(endValue, pauseMenu.animationTime).SetEase(pauseMenu.animationEase).SetUpdate(isIndependentUpdate: true);
		}
	}

	public override void Select()
	{
		settingsMenu.StopBrowsingTab();
		settingsMenu.SelectTab(this);
	}
}
