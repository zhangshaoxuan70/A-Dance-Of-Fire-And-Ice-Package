using UnityEngine;

public class scrEnableOnlyForLanguage : MonoBehaviour
{
	public bool english;

	public bool chinese;

	private void Start()
	{
		SystemLanguage language = RDString.language;
		bool num = language == SystemLanguage.Chinese || language == SystemLanguage.ChineseSimplified || language == SystemLanguage.ChineseTraditional;
		if (num && !chinese)
		{
			base.gameObject.SetActive(value: false);
		}
		if (!num && !english)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
