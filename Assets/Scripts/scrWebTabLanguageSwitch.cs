using UnityEngine;

public class scrWebTabLanguageSwitch : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Tab))
		{
			RDConstants.data.forceLanguage = true;
			if (RDString.language == SystemLanguage.English)
			{
				RDString.ChangeLanguage(SystemLanguage.ChineseSimplified);
			}
			else
			{
				RDString.ChangeLanguage(SystemLanguage.English);
			}
			scrController.instance.Restart();
		}
	}
}
