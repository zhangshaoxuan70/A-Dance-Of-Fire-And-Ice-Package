using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ErrorCanvas : MonoBehaviour
{
	public GameObject mainPanel;

	public Text txtTitle;

	public Text txtErrorMessage;

	public Text txtSubmit;

	public Text txtSupportPages;

	public TextMeshProUGUI txtFaq;

	public TextMeshProUGUI txtDiscord;

	public TextMeshProUGUI txtSteam;

	public Text txtGoBack;

	public Button btnSupport;

	public Button btnLog;

	public Button btnSubmit;

	public Button btnIgnore;

	public GameObject supportPagesPanel;

	public Button btnBack;

	private string errorMessage;

	private bool initiated;

	public void ShowError(string errorMessage)
	{
		if (!initiated)
		{
			initiated = true;
			Object.DontDestroyOnLoad(base.gameObject);
			btnSupport.onClick.AddListener(delegate
			{
				GoToSupportPages();
			});
			btnLog.onClick.AddListener(delegate
			{
				GoToLog();
			});
			btnSubmit.onClick.AddListener(delegate
			{
				Submit();
			});
			btnIgnore.onClick.AddListener(delegate
			{
				Ignore();
			});
			btnBack.onClick.AddListener(delegate
			{
				GoBack();
			});
			btnLog.gameObject.SetActive(!ADOBase.isMobile);
			if (RDString.initialized)
			{
				SetRDString(txtTitle, "error.somethingWentWrong");
				SetRDString(txtSubmit, "error.submit");
				SetRDString(txtSupportPages, "error.supportPages");
				SetRDString(txtFaq, "error.faq");
				SetRDString(txtDiscord, "error.discord");
				SetRDString(txtSteam, "error.steam");
				SetRDString(txtGoBack, "error.goBack");
			}
		}
		txtErrorMessage.text = errorMessage;
		this.errorMessage = errorMessage;
	}

	private void SetRDString(Text text, string token)
	{
		string text2 = RDString.Get(token);
		if (!string.IsNullOrWhiteSpace(text2))
		{
			text.text = text2;
		}
	}

	private void SetRDString(TextMeshProUGUI text, string token)
	{
		string text2 = RDString.Get(token);
		if (!string.IsNullOrWhiteSpace(text2))
		{
			text.text = text2;
		}
	}

	private void GoToSupportPages()
	{
		mainPanel.SetActive(value: false);
		supportPagesPanel.SetActive(value: true);
	}

	private void GoToLog()
	{
		RDEditorUtils.OpenLogDirectory();
	}

	private void Submit()
	{
		string body = "There was an error on A Dance of Fire and Ice. Details:\n" + $"- Version: {Application.version} (r{97}, {GCNS.buildDate}) " + "\n- Device Model: " + SystemInfo.deviceModel + "\n- Operating System: " + SystemInfo.operatingSystem + "\n- Stacktrace: \n" + errorMessage + "\nIf you wish, you can add details and/or a screenshot below.\n";
		SendEmail("giacomo@7thbe.at", "A Dance of Fire and Ice - Error report ", body);
	}

	private void Ignore()
	{
		base.gameObject.SetActive(value: false);
	}

	private void GoBack()
	{
		mainPanel.SetActive(value: true);
		supportPagesPanel.SetActive(value: false);
	}

	public static void SendEmail(string email, string subject, string body)
	{
		subject = EscapeURL(subject);
		body = EscapeURL(body);
		Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
	}

	public static string EscapeURL(string url)
	{
		return UnityWebRequest.EscapeURL(url).Replace("+", "%20");
	}
}
