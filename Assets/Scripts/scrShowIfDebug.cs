using UnityEngine;
using UnityEngine.UI;

public class scrShowIfDebug : ADOBase
{
	private Text txt;

	private Color defaultColor = new Color(0.106f, 1f, 0f, 1f);

	private Color orangeColor = new Color(1f, 0.5f, 0f, 1f);

	private void Awake()
	{
		txt = GetComponent<Text>();
		txt.SetLocalizedFont();
	}

	private void Update()
	{
		if (RDC.noHud)
		{
			txt.enabled = false;
			return;
		}
		if (GCS.d_recording)
		{
			printe("recording is false");
			txt.enabled = false;
			return;
		}
		txt.color = defaultColor;
		if (RDC.auto && RDC.debug)
		{
			txt.enabled = true;
			txt.text = string.Empty;
		}
		else if (RDC.auto)
		{
			txt.enabled = true;
			if (RDC.useOldAuto)
			{
				txt.text = RDString.Get("status.autoplay") + " (old)";
			}
			else
			{
				txt.text = RDString.Get("status.autoplay");
			}
		}
		else if (!RDC.auto && (bool)scrController.instance.currFloor && scrController.instance.currFloor.auto && !ADOBase.sceneName.IsTaro())
		{
			txt.enabled = true;
			txt.text = (RDString.Get("status.autoTile") ?? "");
			txt.color = orangeColor;
		}
		else if (RDC.debug)
		{
			txt.enabled = true;
			txt.text = "Debug Mode";
		}
		else if (txt.enabled)
		{
			txt.enabled = false;
			txt.text = string.Empty;
		}
	}
}
