using UnityEngine;
using UnityEngine.UI;

public class scrHUDText : MonoBehaviour
{
	public bool isTitle;

	private Graphic gui;

	private void Awake()
	{
		gui = GetComponent<Graphic>();
	}

	private void Update()
	{
		bool flag = !RDC.noHud && (!GCS.d_dontShowTitles || !isTitle);
		if (gui.enabled != flag)
		{
			gui.enabled = flag;
		}
		if (flag)
		{
			gui.color = scrVfx.instance.currentColourScheme.colourText;
		}
	}
}
