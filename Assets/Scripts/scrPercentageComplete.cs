using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scrPercentageComplete : MonoBehaviour
{
	private float percent;

	public bool custommessage;

	private Text text;

	private void Awake()
	{
	}

	public void UpdatePercent()
	{
		text = GetComponent<Text>();
		text.SetLocalizedFont();
		if (!custommessage)
		{
			percent = scrController.instance.percentComplete * 100f;
			int num = Mathf.FloorToInt(percent);
			if (scrController.instance.IsPercentCompleteBest())
			{
				text.text = RDString.Get("status.newbest", new Dictionary<string, object>
				{
					{
						"pctComplete",
						num
					}
				});
			}
			else
			{
				text.text = RDString.Get("status.complete", new Dictionary<string, object>
				{
					{
						"pctComplete",
						num
					}
				});
			}
		}
	}

	public void ShowMessage(string msg)
	{
		text = GetComponent<Text>();
		text.SetLocalizedFont();
		GetComponent<Text>().text = msg;
		custommessage = true;
	}
}
