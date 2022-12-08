using UnityEngine;

public class scrTempEscToQuit : ADOBase
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void LateUpdate()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Q) || UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		{
			Quit();
		}
	}

	private void Quit()
	{
		if (GCS.webVersion)
		{
			ADOBase.LoadScene("scnIntro");
		}
	}
}
