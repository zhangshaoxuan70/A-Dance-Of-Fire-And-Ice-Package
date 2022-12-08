using UnityEngine;

public class ChineseDLCLogo : MonoBehaviour
{
	public Mawaru_Sprite spr;

	private void Start()
	{
		if (RDString.language == SystemLanguage.ChineseSimplified || RDString.language == SystemLanguage.ChineseTraditional)
		{
			spr.SetState(1);
		}
		else
		{
			spr.SetState(0);
		}
	}
}
