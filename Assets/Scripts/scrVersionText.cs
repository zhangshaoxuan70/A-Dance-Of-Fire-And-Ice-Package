using UnityEngine;
using UnityEngine.UI;

public class scrVersionText : MonoBehaviour
{
	private int page;

	public Text text;

	private string page0 => $"v{Application.version.ToString()} (r{97}) {ADOBase.platform}";

	private void Awake()
	{
		text.text = page0;
	}

	public void UpdatePage()
	{
		page = ((page == 0) ? 1 : 0);
		text.text = ((page == 0) ? page0 : GCNS.buildDate);
	}
}
