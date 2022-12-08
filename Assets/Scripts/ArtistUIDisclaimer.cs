using ADOFAI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtistUIDisclaimer : ADOBase
{
	public const string SelectedArtistKey = "editor.artistDisclaimer.selected";

	public const string ApprovalLevelToken = "editor.artistDisclaimer.";

	public const string ConditionKey = "editor.artistDisclaimer.condition";

	public const string ConditionDescKey = "editor.artistDisclaimer.conditionDescription";

	public const string ConditionDeclinedDescKey = "editor.artistDisclaimer.conditionDeclinedDescription";

	public const string AcceptConditionKey = "editor.artistDisclaimer.acceptCondition";

	public const string TitleKey = "editor.artistDisclaimer.title";

	public const string ConfirmKey = "editor.artistDisclaimer.confirm";

	public const string CancelKey = "editor.artistDisclaimer.cancel";

	public const string CloseKey = "editor.artistDisclaimer.close";

	public Text selectedArtistText;

	public Text approvalLevelText;

	public Text conditionText;

	public Text conditionDescriptionText;

	public Text conditionCheckText;

	public GameObject confirm;

	public GameObject cancel;

	public GameObject close;

	public RectTransform selectedArtistRect;

	public RectTransform approvalLevelRect;

	public RectTransform conditionDescRect;

	public RectTransform conditionCheckRect;

	public Toggle conditionCheckToggle;

	public Text titleText;

	public Text confirmText;

	public Text cancelText;

	public Text closeText;

	public RectTransform rectTransform;

	public VerticalLayoutGroup verticalLayoutGroup;

	public GameObject fader;

	private PropertyControl_Text artistPropertyControl;

	private ArtistData currentArtistData;

	private bool onlyChecking;

	private ApprovalLevelBadge ApprovalLevelBadge
	{
		get
		{
			return ADOBase.editor.settingsPanel.approvalLevelBadge;
		}
		set
		{
			ADOBase.editor.settingsPanel.approvalLevelBadge = value;
		}
	}

	private void Awake()
	{
		titleText.text = RDString.Get("editor.artistDisclaimer.title");
		confirmText.text = RDString.Get("editor.artistDisclaimer.confirm");
		cancelText.text = RDString.Get("editor.artistDisclaimer.cancel");
		closeText.text = RDString.Get("editor.artistDisclaimer.close");
	}

	public void SetData(ArtistData data, PropertyControl_Text artistPC, bool onlyCheckingMode)
	{
		currentArtistData = data;
		artistPropertyControl = artistPC;
		onlyChecking = onlyCheckingMode;
		selectedArtistText.text = RDString.Get("editor.artistDisclaimer.selected", new Dictionary<string, object>
		{
			{
				"artist",
				data.name
			}
		});
		selectedArtistRect.sizeDelta = selectedArtistRect.sizeDelta.WithY(selectedArtistText.preferredHeight + 10f);
		string key = "editor.artistDisclaimer." + data.approvalLevel.ToString();
		approvalLevelText.text = RDString.Get(key);
		approvalLevelRect.sizeDelta = approvalLevelRect.sizeDelta.WithY(approvalLevelText.preferredHeight + 10f);
		conditionText.text = RDString.Get("editor.artistDisclaimer.condition", new Dictionary<string, object>
		{
			{
				"artist",
				data.name
			}
		});
		string key2 = (data.approvalLevel == ApprovalLevel.Declined) ? "editor.artistDisclaimer.conditionDeclinedDescription" : "editor.artistDisclaimer.conditionDescription";
		conditionDescriptionText.text = RDString.Get(key2);
		conditionDescRect.sizeDelta = conditionDescRect.sizeDelta.WithY(conditionDescriptionText.preferredHeight + 10f);
		if (data.approvalLevel == ApprovalLevel.Declined)
		{
			confirm.SetActive(value: false);
			cancel.SetActive(value: false);
			close.SetActive(value: true);
			conditionCheckRect.gameObject.SetActive(value: false);
		}
		else
		{
			cancel.SetActive(value: true);
			close.SetActive(value: false);
			conditionCheckRect.gameObject.SetActive(!onlyChecking);
			confirm.SetActive(!onlyChecking);
			if (!onlyChecking)
			{
				confirm.GetComponent<Button>().interactable = false;
				conditionCheckToggle.isOn = false;
				conditionCheckRect.sizeDelta = conditionCheckRect.sizeDelta.WithY(conditionCheckText.preferredHeight + 10f);
			}
		}
		float num = 0f;
		for (int i = 0; i < rectTransform.childCount; i++)
		{
			if (rectTransform.GetChild(i).gameObject.activeSelf)
			{
				num += rectTransform.GetChild(i).GetComponent<RectTransform>().sizeDelta.y;
			}
		}
		num += verticalLayoutGroup.spacing * (float)(rectTransform.childCount - 1);
		num += (float)(verticalLayoutGroup.padding.top + verticalLayoutGroup.padding.bottom);
		rectTransform.sizeDelta = rectTransform.sizeDelta.WithY(num);
		fader.SetActive(value: true);
		base.gameObject.SetActive(value: true);
		ADOBase.editor.settingsPanel.HideArtistDropdown();
		if (!onlyChecking)
		{
			ADOBase.editor.levelData.artist = currentArtistData.name;
			artistPropertyControl.inputField.text = currentArtistData.name;
			ApprovalLevelBadge.UpdateUI(currentArtistData.approvalLevel, onlyColor: true);
		}
	}

	public void ShowArtistDisclaimer()
	{
		Application.OpenURL("https://7thbe.at/verified-artists/adofai/artist/" + currentArtistData.id.ToString());
	}

	public void Confirm()
	{
		if (!onlyChecking)
		{
			ADOBase.editor.levelData.artist = currentArtistData.name;
			artistPropertyControl.inputField.text = currentArtistData.name;
			ApprovalLevelBadge.UpdateUI(currentArtistData.approvalLevel, onlyColor: true);
		}
		Hide();
	}

	public void Cancel()
	{
		if (!onlyChecking)
		{
			ADOBase.editor.levelData.artist = string.Empty;
			artistPropertyControl.inputField.text = string.Empty;
			ApprovalLevelBadge.UpdateUI(ApprovalLevel.Pending, onlyColor: true);
		}
		Hide();
	}

	private void Hide()
	{
		if (!onlyChecking)
		{
			ADOBase.editor.settingsPanel.HideArtistDropdown();
		}
		fader.SetActive(value: false);
		base.gameObject.SetActive(value: false);
		ADOBase.editor.popupBlocker.gameObject.SetActive(value: false);
	}
}
