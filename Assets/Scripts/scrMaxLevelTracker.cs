using UnityEngine.UI;

public class scrMaxLevelTracker : ADOBase
{
	private Text maxLevel;

	private void Start()
	{
		maxLevel = GetComponent<Text>();
		maxLevel.text = "jjj";
		maxLevel.SetLocalizedFont();
	}

	private void Update()
	{
		if (ADOBase.isMobile)
		{
			maxLevel.text = "Highest Level Reached: " + GCS.maxLevel.ToString();
		}
		else
		{
			maxLevel.text = RDString.Get("webgl.maxLevel") + GCS.maxLevel.ToString() + "\n" + RDString.Get("webgl.reset");
		}
	}
}
