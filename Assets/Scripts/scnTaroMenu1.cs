using TMPro;
using UnityEngine;

public class scnTaroMenu1 : ADOBase
{
	private scrCamera camera;

	public TextMeshPro exitText;

	private void Start()
	{
		camera = scrCamera.instance;
		exitText.text = RDString.Get("neoCosmos.exitWorld");
		exitText.SetLocalizedFont();
	}

	private void Update()
	{
		int num = Mathf.RoundToInt(ADOBase.controller.chosenplanet.transform.position.y);
		if (!ADOBase.controller.isCutscene)
		{
			if (num == 12)
			{
				camera.positionState = PositionState.TaroMenu1TopLane;
			}
			else
			{
				camera.positionState = PositionState.None;
			}
		}
		else
		{
			camera.timer = 0f;
			camera.positionState = PositionState.None;
		}
	}
}
