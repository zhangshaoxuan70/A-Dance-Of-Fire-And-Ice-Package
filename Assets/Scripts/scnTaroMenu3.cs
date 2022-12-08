using TMPro;
using UnityEngine;

public class scnTaroMenu3 : ADOBase
{
	private scrCamera camera;

	public TextMeshPro exitText;

	public float fontScale = 1f;

	public MenuArrow toStage;

	public scrCamera mainCamera;

	private scrPlanet chosenplanet => ADOBase.controller.chosenplanet;

	private bool responsive
	{
		get
		{
			return ADOBase.controller.responsive;
		}
		set
		{
			ADOBase.controller.responsive = value;
		}
	}

	private int menuPhase
	{
		get
		{
			return ADOBase.controller.menuPhase;
		}
		set
		{
			ADOBase.controller.menuPhase = value;
		}
	}

	private scrCamera camy => ADOBase.controller.camy;

	private void Start()
	{
		camera = scrCamera.instance;
		if (Persistence.GetTaroStoryProgress() == 4)
		{
			Persistence.SetTaroStoryProgress(5);
			Persistence.Save();
		}
		toStage.FadeIn();
		exitText.text = RDString.Get("neoCosmos.exitWorld");
		exitText.SetLocalizedFont();
		if (GCS.worldEntrance != null)
		{
			JumpToWorldPortal(GCS.worldEntrance, instant: true);
		}
	}

	private void Update()
	{
		Vector3 position = ADOBase.controller.chosenplanet.transform.position;
		int num = Mathf.RoundToInt(position.y);
		int num2 = Mathf.RoundToInt(position.x);
		if (!ADOBase.controller.isCutscene)
		{
			if (num == 2)
			{
				camera.positionState = PositionState.TaroMenu3TopLane;
			}
			else if (num == -2 && num2 > 2)
			{
				camera.positionState = PositionState.TaroMenu3BottomLane;
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
		if (UnityEngine.Input.GetKeyDown(KeyCode.BackQuote) || UnityEngine.Input.GetKeyDown(KeyCode.Alpha0))
		{
			JumpWithKey(0);
		}
		else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
		{
			JumpWithKey(1);
		}
		else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2))
		{
			JumpWithKey(2);
		}
		else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3))
		{
			JumpWithKey(3);
		}
		else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha4))
		{
			JumpWithKey(4);
		}
		else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha5))
		{
			JumpWithKey(5);
		}
	}

	public void JumpWithKey(int key)
	{
		if (!ADOBase.isMobile && !ADOBase.controller.holding && !ADOBase.controller.isCutscene)
		{
			switch (key)
			{
			case 1:
				JumpToWorldPortal("T1");
				break;
			case 2:
				JumpToWorldPortal("T2");
				break;
			case 3:
				JumpToWorldPortal("T3");
				break;
			case 4:
				JumpToWorldPortal("T4");
				break;
			default:
				JumpToWorldPortal("T5");
				break;
			}
		}
	}

	public void JumpToWorldPortal(string world, bool instant = false, bool wipeFirst = false)
	{
		if (!responsive)
		{
			return;
		}
		if (wipeFirst)
		{
			responsive = false;
			scrUIController.instance.WipeToBlack(WipeDirection.StartsFromRight, delegate
			{
				responsive = true;
				JumpToWorldPortal(world, instant: true);
				scrUIController.instance.WipeFromBlack();
			});
		}
		else if (world != null)
		{
			Vector2 vector = new Vector2(0f, 0f);
			if (!GCS.puzzle)
			{
				vector = (world.IsTaro() ? GCNS.jumpPositionTaroMenu3[world] : new Vector2(0f, 0f));
			}
			else
			{
				GCS.puzzle = false;
			}
			chosenplanet.transform.position = new Vector3(vector.x, vector.y, chosenplanet.transform.position.z);
			menuPhase = 1;
			if (instant)
			{
				camy.ViewObjectInstant(chosenplanet.transform, includeOffset: true);
			}
			else
			{
				camy.Refocus(chosenplanet.transform);
			}
			camera.timer = 0f;
			camera.positionState = PositionState.None;
			camy.isMoveTweening = true;
		}
	}
}
