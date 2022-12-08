using TMPro;
using UnityEngine;

public class scnTaroMenu0 : ADOBase
{
	private scrCamera camera;

	public TextMeshPro exitText;

	public TextMeshPro showSceneText;

	public TextMeshPro puzzleRoomText;

	private void Start()
	{
		camera = scrCamera.instance;
		exitText.text = RDString.Get("neoCosmos.exitWorld");
		exitText.SetLocalizedFont();
		showSceneText.text = RDString.Get("neoCosmos.showCutscene");
		showSceneText.SetLocalizedFont();
		showSceneText.gameObject.SetActive(value: false);
		puzzleRoomText.text = RDString.Get("TP.title");
		puzzleRoomText.SetLocalizedFont();
		GCS.enableCutsceneT5 = false;
	}

	public void EnableT5Cutscene()
	{
		printe("enable t5 cutscene");
		GCS.enableCutsceneT5 = true;
	}

	public void ShowSceneText()
	{
		showSceneText.gameObject.SetActive(value: true);
	}

	public void HideSceneText()
	{
		showSceneText.gameObject.SetActive(value: false);
	}

	private void Update()
	{
		Vector3 position = ADOBase.controller.chosenplanet.transform.position;
		int num = Mathf.RoundToInt(position.y);
		int num2 = Mathf.RoundToInt(position.x);
		if (!ADOBase.controller.isCutscene)
		{
			if (num2 == 36)
			{
				camera.positionState = PositionState.NeoCosmosCredits;
			}
			else if ((num == 0 || num == 1) && num2 != 35)
			{
				camera.positionState = PositionState.TaroMenu0TopLane;
			}
			else if (num == -7)
			{
				camera.positionState = PositionState.TaroMenu0BottomLane;
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
