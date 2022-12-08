using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notification : ADOBase
{
	private static Notification _instance;

	public RectTransform bar;

	public Text text;

	private bool showCalibrationOnTap;

	public static Notification instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.Instantiate(RDConstants.data.prefab_notification).GetComponent<Notification>();
			}
			return _instance;
		}
	}

	private void Awake()
	{
		text.SetLocalizedFont();
	}

	public void Show()
	{
		CalibrationPreset preset = scrConductor.currentPreset;
		bar.DOKill();
		bar.AnchorPosX(0f - bar.sizeDelta.x);
		bar.DOAnchorPosX(0f, 0.5f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true)
			.SetDelay(0.5f)
			.OnComplete(delegate
			{
				float delay = preset.confident ? 3f : 5f;
				bar.DOAnchorPosX(0f - bar.sizeDelta.x, 0.3f).SetEase(Ease.InExpo).SetUpdate(isIndependentUpdate: true)
					.SetDelay(delay);
			});
		Dictionary<string, object> parameters = new Dictionary<string, object>
		{
			{
				"device",
				"<b>" + preset.ReadableOutputName() + "</b>"
			}
		};
		if (preset.confident)
		{
			text.text = RDString.Get("calibration.notification.confidentPreset", parameters).Replace("[[", "<color=red>").Replace("]]", "</color>");
			showCalibrationOnTap = false;
		}
		else
		{
			text.text = RDString.Get("calibration.notification.notConfidentPreset", parameters).Replace("[[", "<color=red>").Replace("]]", "</color>");
			showCalibrationOnTap = true;
		}
	}

	public void MoreInfo()
	{
		if (showCalibrationOnTap)
		{
			ADOBase.GoToCalibration();
		}
		else
		{
			ADOBase.controller.takeScreenshot.ShowPauseMenu(goToSettings: true);
		}
	}
}
