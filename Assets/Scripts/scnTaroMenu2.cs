using TMPro;
using UnityEngine;

public class scnTaroMenu2 : ADOBase
{
	private scrCamera camera;

	public MenuArrow toPuzzle;

	public MenuArrow toStage;

	public scrFloor puzzle;

	public TextMeshPro exitText;

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
		Persistence.Save();
		if (Persistence.IsWorldComplete("T4"))
		{
			toPuzzle.FadeIn();
		}
		else
		{
			toStage.FadeIn();
			puzzle.isLandable = false;
			puzzle.transform.localScale = Vector3.zero;
		}
		exitText.text = RDString.Get("neoCosmos.exitWorld");
		exitText.SetLocalizedFont();
		if (GCS.worldEntrance != null)
		{
			JumpToWorldPortal(GCS.worldEntrance, instant: true);
		}
		else if (GCS.banished)
		{
			Vector2 vector = new Vector2(-18f, -4f);
			chosenplanet.transform.position = new Vector3(vector.x, vector.y, chosenplanet.transform.position.z);
			menuPhase = 1;
			camy.ViewObjectInstant(chosenplanet.transform, includeOffset: true);
			camera.timer = 0f;
			camera.positionState = PositionState.None;
			camy.isMoveTweening = true;
		}
	}

	private void Update()
	{
		Vector3 position = ADOBase.controller.chosenplanet.transform.position;
		int num = Mathf.RoundToInt(position.y);
		int num2 = Mathf.RoundToInt(position.x);
		if (!ADOBase.controller.isCutscene)
		{
			if (num == 2 && num2 > -5)
			{
				camera.positionState = PositionState.TaroMenu2TopLane;
			}
			else if (num == -8 && num2 > 2)
			{
				camera.positionState = PositionState.TaroMenu2BottomLane;
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
		if (num2 > -5)
		{
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
			default:
				JumpToWorldPortal("T4");
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
			if (!GCS.banished)
			{
				vector = (world.IsTaro() ? GCNS.jumpPositionTaroMenu2[world] : new Vector2(0f, 0f));
			}
			else
			{
				vector = new Vector2(-18f, -4f);
				GCS.banished = false;
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
