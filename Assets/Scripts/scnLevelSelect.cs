using DG.Tweening;
using RDTools;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scnLevelSelect : ADOBase
{
	public const int CrownEntrance = 50;

	public const int CrownExit = 51;

	public const int MuseDashEntrance = 52;

	public const int MuseDashExit = 53;

	private readonly Vector2 originPosition = new Vector2(0f, 0f);

	private readonly Vector2 xtraIslandPosition = new Vector2(0f, 9f);

	private readonly Vector2 crownIslandPosition = new Vector2(0f, 23f);

	private readonly Vector2 changingRoomPosition = new Vector2(0f, -6f);

	private readonly Vector2 changingRoom2Position = new Vector2(0f, -19f);

	private readonly Vector2 museDashIslandPosition = new Vector2(-25f, 27f);

	private readonly RDCheatCode cheatIslandCheatCode = new RDCheatCode("imacheater");

	private readonly RDCheatCode qureeCheatCode = new RDCheatCode("quree");

	private readonly RDCheatCode star4thCheatCode = new RDCheatCode("star4th");

	private readonly RDCheatCode paradiddleCheatCode = new RDCheatCode("→←→→←→←←→←→→←→←←");

	private readonly RDCheatCode bonusLevelCheatCode = new RDCheatCode("thanks");

	private readonly RDCheatCode unlockAllCheatCode = new RDCheatCode("idkfa");

	private readonly RDCheatCode transCheatCode = new RDCheatCode("transrights");

	private readonly RDCheatCode minesweeperCheatCode = new RDCheatCode("kaboom");

	private readonly RDCheatCode nbCheatCode = new RDCheatCode("nbrights");

	private readonly RDCheatCode rainbowCheatCode = new RDCheatCode("↑↑↓↓←→←→ba⏎");

	private readonly RDCheatCode faceCheatCode = new RDCheatCode("thonk");

	private readonly RDCheatCode samuraiCheatCode = new RDCheatCode("samurai");

	public static scnLevelSelect instance;

	public scrCamera mainCamera;

	public SpriteRenderer tv;

	public SpriteRenderer tvChick;

	public Sprite tvChickSprite0;

	public Sprite tvChickSprite1;

	public CanvasGroup textGroup;

	public Text rdOfferText;

	public Text rdOfferDismiss;

	public scrFloor rdFloor;

	public LevelSelectCutsceneController cutsceneController;

	public scrFloor crownEntranceGem;

	public scrFloor crownExitGem;

	public List<scrSetPortalToLevel> portalToLevelComponents = new List<scrSetPortalToLevel>();

	public GameObject editorFloor;

	public GameObject editorText;

	[NonSerialized]
	public string lastVisitedWorld;

	private bool showRDOffer;

	private bool showingRDOfferText;

	private uint frameUsed;

	private float lastPlanetX;

	private int verticalDirection;

	public bool playingCutscene;

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

	private void Awake()
	{
		instance = this;
		Application.logMessageReceived -= ADOStartup.LogMessageReceived;
		if (ADOBase.IsAprilFools())
		{
			CameraFilterPack_FlipScreen component = scrCamera.instance.GetComponent<CameraFilterPack_FlipScreen>();
			if (component == null)
			{
				printe("is null!");
			}
			else
			{
				component.enabled = true;
				component.flipX = true;
				component.flipY = true;
			}
		}
		if (RDC.runningOnSteamDeck)
		{
			editorFloor.SetActive(value: false);
			editorText.SetActive(value: false);
		}
		GCS.useNoFail = false;
	}

	private void Start()
	{
		RDInput.SetMapping("LevelSelect");
		showRDOffer = false;
		ShowRDOffer(showRDOffer);
		ShowRDOfferText(show: false);
		UpdateCameraOrthoSize();
		mainCamera.setCamSizeInstant(mainCamera.camsizenormal);
		if (GCS.worldEntrance != null)
		{
			JumpToWorldPortal(GCS.worldEntrance, instant: true);
		}
		playingCutscene = (cutsceneController.CheckForCutscene() != null);
		if (crownEntranceGem != null)
		{
			float animDuration = 8f;
			float s = 0.5f;
			Color color = Color.HSVToRGB(0f, s, 1f);
			crownEntranceGem.ColorFloor(TrackColorType.Rainbow, color, Color.white, animDuration, TrackColorPulse.None, 1f);
			crownExitGem.ColorFloor(TrackColorType.Rainbow, color, Color.white, animDuration, TrackColorPulse.None, 1f);
		}
	}

	private void Update()
	{
		if (showRDOffer)
		{
			float x = ADOBase.controller.chosenplanet.transform.position.x;
			if (x != lastPlanetX)
			{
				if (x - float.Epsilon > 6f && x + float.Epsilon < 10f)
				{
					if (!showingRDOfferText)
					{
						ShowRDOfferText(show: true);
					}
					printe("entered rd portal");
				}
				else if (showingRDOfferText)
				{
					ShowRDOfferText(show: false);
				}
				lastPlanetX = x;
			}
		}
		if (cheatIslandCheatCode.CheckCheatCode())
		{
			if (ADOBase.sceneName != GCNS.sceneLevelSelect || chosenplanet.currfloor.GetComponent<scrMenuMovingFloor>() != null)
			{
				return;
			}
			if (chosenplanet.transform.position.x < -5f)
			{
				JumpAndWipeWithKey(52);
			}
			else if (chosenplanet.transform.position.y > 20f)
			{
				JumpAndWipeWithKey(50);
			}
			else
			{
				JumpToWorldPortal("XO", instant: false, wipeFirst: true);
			}
		}
		if (qureeCheatCode.CheckCheatCode())
		{
			ADOBase.controller.GoToSpecificLevel("XO-1");
		}
		if (star4thCheatCode.CheckCheatCode())
		{
			ADOBase.controller.GoToSpecificLevel("XO-X", speedTrial: true);
		}
		if (RDC.debug && bonusLevelCheatCode.CheckCheatCode())
		{
			ADOBase.controller.GoToSpecificLevel("B-X");
		}
		if (unlockAllCheatCode.CheckCheatCode() && GCS.IsDev())
		{
			Persistence.Complete100();
			ADOBase.GoToLevelSelect();
			scrFlash.Flash(Color.white);
		}
		if (transCheatCode.CheckCheatCode())
		{
			TransMode();
			scrFlash.Flash(Color.white);
		}
		if (nbCheatCode.CheckCheatCode())
		{
			EnbyMode();
			scrFlash.Flash(Color.white);
		}
		if (rainbowCheatCode.CheckCheatCode())
		{
			RainbowMode();
			scrFlash.Flash(Color.white);
		}
		if (faceCheatCode.CheckCheatCode())
		{
			ToggleFaceMode();
			scrFlash.Flash(Color.white);
		}
		if (minesweeperCheatCode.CheckCheatCode())
		{
			scnMinesweeper.EnterScene();
		}
		if (samuraiCheatCode.CheckCheatCode())
		{
			SamuraiMode();
			scrFlash.Flash(Color.white);
		}
		if (RDEditorUtils.CheckPlayerLogKeyCombo())
		{
			RDEditorUtils.OpenLogDirectory();
		}
		if (!ADOBase.controller.paused)
		{
			if (GCS.d_drumcontroller && UnityEngine.Input.GetKeyDown(KeyCode.End))
			{
				JumpAndWipeWithKey(0);
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.BackQuote) || UnityEngine.Input.GetKeyDown(KeyCode.Alpha0))
			{
				JumpAndWipeWithKey(0);
			}
			else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
			{
				JumpAndWipeWithKey(1);
			}
			else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2))
			{
				JumpAndWipeWithKey(2);
			}
			else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3))
			{
				JumpAndWipeWithKey(3);
			}
			else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha4))
			{
				JumpAndWipeWithKey(4);
			}
			else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha5))
			{
				JumpAndWipeWithKey(5);
			}
			else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha6))
			{
				JumpAndWipeWithKey(6);
			}
			else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha7))
			{
				JumpAndWipeWithKey(7);
			}
			foreach (RDInputType input in RDInput.inputs)
			{
				if (input is RDInputType_Joystick)
				{
					RDInputType_Joystick rDInputType_Joystick = input as RDInputType_Joystick;
					if (rDInputType_Joystick.Left(ButtonState.WentDown))
					{
						JumpHorizontal(-1);
					}
					else if (rDInputType_Joystick.Right(ButtonState.WentDown))
					{
						JumpHorizontal(1);
					}
					else if (rDInputType_Joystick.Up(ButtonState.WentDown))
					{
						JumpVertical(1);
					}
					else if (rDInputType_Joystick.Down(ButtonState.WentDown))
					{
						JumpVertical(-1);
					}
					break;
				}
			}
			if (RDEditorUtils.CheckForKeyCombo(control: true, shift: true, KeyCode.C))
			{
				ADOBase.controller.PortalTravelAction(-5);
			}
			else if (RDEditorUtils.CheckForKeyCombo(control: true, shift: true, KeyCode.E))
			{
				ADOBase.controller.PortalTravelAction(-4);
			}
			else if (RDEditorUtils.CheckForKeyCombo(control: true, shift: true, KeyCode.N) && ADOBase.hasTaroDLC)
			{
				ADOBase.controller.PortalTravelAction(-60);
			}
		}
	}

	private void LateUpdate()
	{
		UpdateCameraOrthoSize();
	}

	private void UpdateCameraOrthoSize(bool print = false)
	{
		float value = (float)Screen.width * 1f / (float)Screen.height;
		float num = Mathf.InverseLerp(1.77777779f, 1.33333337f, value);
		mainCamera.tosize = Mathf.Lerp(5f, 6.7f, num);
		float tosize = mainCamera.tosize;
		mainCamera.camsizenormal = tosize;
		mainCamera.Bgcamstatic.orthographicSize = tosize;
		mainCamera.BGcam.orthographicSize = tosize;
		if (print)
		{
			printe($"t {num} toSize {mainCamera.tosize} camsizeNormal {mainCamera.camsizenormal}");
		}
	}

	public void DismissRDOffer()
	{
		Persistence.SetShowRDOffer(offer: false);
		ShowRDOffer(show: false);
	}

	public void ShowRDOffer(bool show)
	{
		tv.gameObject.SetActive(show);
		rdFloor.gameObject.SetActive(show);
	}

	public void ShowRDOfferText(bool show)
	{
		showingRDOfferText = show;
		float endValue = show ? 1f : 0f;
		float endValue2 = show ? 0.3f : 1f;
		tv.DOFade(endValue2, 0.3f);
		tvChick.DOFade(endValue2, 0.3f);
		textGroup.DOFade(endValue, 0.3f);
	}

	public override void OnBeat()
	{
		frameUsed++;
		tvChick.sprite = ((frameUsed % 2u == 0) ? tvChickSprite0 : tvChickSprite1);
	}

	public void JumpVertical(int direction)
	{
		int result = 0;
		if (lastVisitedWorld == "B")
		{
			result = 7;
		}
		else if (int.TryParse(lastVisitedWorld, out result) && result > 6)
		{
			result -= 6;
		}
		if (result == 0)
		{
			verticalDirection = direction;
			JumpToWorldPortal(null);
		}
		else
		{
			JumpAndWipeWithKey(result);
		}
	}

	public void JumpHorizontal(int direction)
	{
		int result = 0;
		if (lastVisitedWorld == "B")
		{
			result = 7;
		}
		else if (!int.TryParse(lastVisitedWorld, out result))
		{
			result = 0;
		}
		bool flag = chosenplanet.transform.position.y < -3.5f && chosenplanet.transform.position.x > 2f;
		result += direction;
		int num = (Persistence.GetOverallProgressStage() >= 3) ? 7 : 5;
		int num2 = 12;
		bool wipeFirst = false;
		if ((flag && result < num) || (!flag && result < 0))
		{
			wipeFirst = true;
			result = (flag ? num2 : num);
		}
		else if ((flag && result > num2) || (!flag && result > num))
		{
			wipeFirst = true;
			result = (flag ? num : 0);
		}
		if (flag)
		{
			result -= 6;
		}
		JumpAndWipeWithKey(result, wipeFirst);
	}

	public void JumpWithKey(int key)
	{
		JumpAndWipeWithKey(key, wipeFirst: true);
	}

	public void JumpAndWipeWithKey(int key, bool wipeFirst = false)
	{
		if (chosenplanet.currfloor.GetComponent<scrMenuMovingFloor>() != null && key < 50)
		{
			RDBaseDll.printem("failsafe??");
			return;
		}
		bool flag = chosenplanet.transform.position.y > 20f;
		bool wipeFirst2 = wipeFirst | flag;
		switch (key)
		{
		case 50:
			JumpToWorldPortal(50.ToString(), instant: false, wipeFirst: true);
			return;
		case 51:
			JumpToWorldPortal(51.ToString(), instant: false, wipeFirst: true);
			return;
		case 52:
			JumpToWorldPortal(52.ToString(), instant: false, wipeFirst: true);
			return;
		case 53:
			JumpToWorldPortal(53.ToString(), instant: false, wipeFirst: true);
			return;
		case 0:
			verticalDirection = -1;
			JumpToWorldPortal(null, instant: false, wipeFirst);
			return;
		}
		if (chosenplanet.transform.position.y < -3.5f && chosenplanet.transform.position.x > 2f)
		{
			switch (key)
			{
			case 1:
				JumpToWorldPortal("7", instant: false, wipeFirst2);
				break;
			case 2:
				JumpToWorldPortal("8", instant: false, wipeFirst2);
				break;
			case 3:
				JumpToWorldPortal("9", instant: false, wipeFirst2);
				break;
			case 4:
				JumpToWorldPortal("10", instant: false, wipeFirst2);
				break;
			case 5:
				JumpToWorldPortal("11", instant: false, wipeFirst2);
				break;
			case 6:
				JumpToWorldPortal("12", instant: false, wipeFirst2);
				break;
			}
			return;
		}
		switch (key)
		{
		case 1:
			JumpToWorldPortal("1", instant: false, wipeFirst2);
			break;
		case 2:
			JumpToWorldPortal("2", instant: false, wipeFirst2);
			break;
		case 3:
			JumpToWorldPortal("3", instant: false, wipeFirst2);
			break;
		case 4:
			JumpToWorldPortal("4", instant: false, wipeFirst2);
			break;
		case 5:
			JumpToWorldPortal("5", instant: false, wipeFirst2);
			break;
		case 6:
			JumpToWorldPortal("6", instant: false, wipeFirst2);
			break;
		case 7:
			JumpToWorldPortal("B", instant: false, wipeFirst2);
			break;
		}
	}

	public void JumpToWorldPortal(string world, bool instant = false, bool wipeFirst = false)
	{
		if (!responsive || (world != null && world.IsTaro()))
		{
			return;
		}
		float planetX = chosenplanet.transform.position.x;
		float planetY = chosenplanet.transform.position.y;
		Transform transform = chosenplanet.transform;
		if (wipeFirst)
		{
			responsive = false;
			scrUIController.instance.WipeToBlack(WipeDirection.StartsFromRight, delegate
			{
				responsive = true;
				JumpToWorldPortal(world, instant: true);
				scrUIController.instance.WipeFromBlack();
				if (world == 50.ToString() || world == 51.ToString() || world == 52.ToString() || world == 53.ToString())
				{
					Collider2D[] array = Physics2D.OverlapPointAll(new Vector2(planetX, planetY), 1 << LayerMask.NameToLayer("Floor"));
					if (array.Length != 0)
					{
						for (int i = 0; i < array.Length; i++)
						{
							scrFloor component = array[i].GetComponent<scrFloor>();
							if (component.isLandable)
							{
								chosenplanet.currfloor = component;
							}
						}
					}
				}
			});
			return;
		}
		bool flag = world == "7" || world == "8" || world == "9" || world == "10" || world == "11" || world == "12" || world == "B";
		bool flag2 = world != null && world.IsCrownWorld();
		bool flag3 = world == 50.ToString();
		bool flag4 = world == 51.ToString();
		bool flag5 = world != null && world.IsMuseDashWorld();
		bool flag6 = world == 52.ToString();
		bool flag7 = world == 53.ToString();
		int overallProgressStage = Persistence.GetOverallProgressStage();
		if ((overallProgressStage < 3 && world == "6") || (overallProgressStage < 5 && flag))
		{
			return;
		}
		bool flag8 = world != null && (world.IsXtra() | flag5);
		if (overallProgressStage < 5 && flag8 && !flag2 && !flag5)
		{
			return;
		}
		if (world != null && !flag8 && !flag2 && !flag3 && !flag4 && !flag5 && !flag6 && !flag7 && ADOBase.worldData[world].jumpPortalPosition == new Vector2Int((int)planetX, (int)planetY) && int.TryParse(world, out int result))
		{
			if (result >= 1 && result <= 6)
			{
				JumpToWorldPortal((result + 6).ToString());
			}
			else if (result >= 7 && result <= 12)
			{
				JumpToWorldPortal((result - 6).ToString());
			}
			return;
		}
		camy.SetXOffset(0f);
		camy.SetYOffset(((world == null) | flag8 | flag4 | flag7) ? 0f : 3f);
		if (world == null)
		{
			bool flag9 = transform.position == new Vector3(crownIslandPosition.x, crownIslandPosition.y, transform.position.z);
			bool flag10 = transform.position == new Vector3(xtraIslandPosition.x, xtraIslandPosition.y, transform.position.z);
			bool flag11 = transform.position == new Vector3(museDashIslandPosition.x, museDashIslandPosition.y, transform.position.z);
			bool flag12 = transform.position == new Vector3(originPosition.x, originPosition.y, transform.position.z);
			bool flag13 = transform.position == new Vector3(changingRoomPosition.x, changingRoomPosition.y, transform.position.z);
			bool flag14 = transform.position == new Vector3(changingRoom2Position.x, changingRoom2Position.y, transform.position.z);
			if (verticalDirection == 1)
			{
				if (flag12 && overallProgressStage >= 1)
				{
					if (overallProgressStage >= 3)
					{
						JumpTo(PositionState.XtraIsland, instant: false);
					}
					else
					{
						JumpTo(PositionState.ChangingRoom2, instant: false);
					}
				}
				else if (flag10)
				{
					if (overallProgressStage >= 3)
					{
						JumpTo(PositionState.MuseDashIsland, instant: true, wipeFirst: true);
					}
					else if (overallProgressStage >= 1)
					{
						JumpTo(PositionState.ChangingRoom2, instant: false);
					}
					else
					{
						JumpTo(PositionState.Origin, instant: false);
					}
				}
				else if (flag11)
				{
					if (overallProgressStage >= 8)
					{
						JumpTo(PositionState.CrownIsland, instant: true, wipeFirst: true);
					}
					else if (overallProgressStage >= 1)
					{
						JumpTo(PositionState.ChangingRoom2, instant: true, wipeFirst: true);
					}
					else
					{
						JumpTo(PositionState.Origin, instant: true, wipeFirst: true);
					}
				}
				else if (flag9)
				{
					if (overallProgressStage >= 1)
					{
						JumpTo(PositionState.ChangingRoom2, instant: true, wipeFirst: true);
					}
					else
					{
						JumpTo(PositionState.Origin, instant: true, wipeFirst: true);
					}
				}
				else if (flag14)
				{
					if (overallProgressStage >= 1)
					{
						JumpTo(PositionState.ChangingRoom, instant: false);
					}
					else
					{
						JumpTo(PositionState.Origin, instant: false);
					}
				}
				else if (flag13)
				{
					JumpTo(PositionState.Origin, instant: false);
				}
				else if (!flag12)
				{
					JumpTo(PositionState.Origin, instant);
				}
			}
			else if (flag12 && overallProgressStage >= 1)
			{
				JumpTo(PositionState.ChangingRoom, instant: false);
			}
			else if (flag13)
			{
				JumpTo(PositionState.ChangingRoom2, instant: false);
			}
			else if (flag14)
			{
				if (overallProgressStage >= 8)
				{
					JumpTo(PositionState.CrownIsland, instant: true, wipeFirst: true);
				}
				else if (overallProgressStage >= 3)
				{
					JumpTo(PositionState.MuseDashIsland, instant: true, wipeFirst: true);
				}
				else
				{
					JumpTo(PositionState.Origin, instant: false);
				}
			}
			else if (flag9)
			{
				if (overallProgressStage >= 3)
				{
					JumpTo(PositionState.MuseDashIsland, instant: true, wipeFirst: true);
				}
				else
				{
					JumpTo(PositionState.Origin, instant: true, wipeFirst: true);
				}
			}
			else if (flag11)
			{
				if (overallProgressStage >= 3)
				{
					JumpTo(PositionState.XtraIsland, instant: true, wipeFirst: true);
				}
				else
				{
					JumpTo(PositionState.Origin, instant: true, wipeFirst: true);
				}
			}
			else if (flag10)
			{
				JumpTo(PositionState.Origin, instant: false);
			}
			else if (!flag12)
			{
				JumpTo(PositionState.Origin, instant);
			}
		}
		else
		{
			Vector2Int vector2Int = (world == 50.ToString()) ? new Vector2Int(10, -2) : ((world == 51.ToString()) ? new Vector2Int(0, 21) : ((world == 52.ToString()) ? new Vector2Int(8, -2) : ((world == 53.ToString()) ? new Vector2Int(-25, 27) : ADOBase.worldData[world].jumpPortalPosition)));
			if (world.IsTaro())
			{
				vector2Int = new Vector2Int(0, 0);
			}
			transform.LocalMoveXY(vector2Int.x, vector2Int.y);
			menuPhase = 1;
			if (instant)
			{
				camy.ViewObjectInstant(transform, includeOffset: true);
			}
			else
			{
				camy.Refocus(transform);
			}
			bool num = (float)vector2Int.y <= -5f;
			camy.isMoveTweening = true;
			if (num)
			{
				camy.positionState = PositionState.DLC;
			}
			else if (flag2 | flag4)
			{
				camy.positionState = PositionState.CrownIsland;
				camy.ViewVectorInstant(new Vector2(0f, 21f), includeOffset: true);
			}
			else if (flag5 | flag7)
			{
				camy.positionState = PositionState.MuseDashIsland;
				camy.ViewVectorInstant(new Vector2(-25f, 23f), includeOffset: true);
			}
			else if (flag8)
			{
				camy.positionState = PositionState.XtraIsland;
			}
			else
			{
				camy.positionState = PositionState.Levels;
			}
		}
	}

	private void JumpTo(PositionState positionState, bool instant, bool wipeFirst = false)
	{
		if (wipeFirst)
		{
			responsive = false;
			scrUIController.instance.WipeToBlack(WipeDirection.StartsFromRight, delegate
			{
				responsive = true;
				JumpTo(positionState, instant);
				scrUIController.instance.WipeFromBlack();
			});
		}
		else
		{
			JumpTo(positionState, instant);
		}
	}

	private void JumpTo(PositionState positionState, bool instant)
	{
		Transform transform = chosenplanet.transform;
		switch (positionState)
		{
		case PositionState.Origin:
			transform.LocalMoveXY(originPosition);
			lastVisitedWorld = "0";
			break;
		case PositionState.XtraIsland:
			transform.LocalMoveXY(xtraIslandPosition);
			break;
		case PositionState.CrownIsland:
			transform.LocalMoveXY(crownIslandPosition);
			break;
		case PositionState.ChangingRoom:
			transform.LocalMoveXY(changingRoomPosition);
			break;
		case PositionState.ChangingRoom2:
			transform.LocalMoveXY(changingRoom2Position);
			break;
		case PositionState.MuseDashIsland:
			transform.LocalMoveXY(museDashIslandPosition);
			break;
		}
		menuPhase = ((positionState != PositionState.Origin) ? 1 : 0);
		camy.positionState = positionState;
		if (instant)
		{
			camy.ViewObjectInstant(transform);
			return;
		}
		camy.Refocus(transform);
		camy.isMoveTweening = true;
	}

	private void TransMode()
	{
		scrPlanet redPlanet = ADOBase.controller.redPlanet;
		scrPlanet bluePlanet = ADOBase.controller.bluePlanet;
		Color planetColor = new Color(0.3607843f, 67f / 85f, 0.9294118f);
		Color planetColor2 = new Color(0.9568627f, 164f / 255f, 0.7098039f);
		redPlanet.EnableCustomColor();
		bluePlanet.EnableCustomColor();
		redPlanet.SetPlanetColor(planetColor);
		bluePlanet.SetPlanetColor(planetColor2);
		redPlanet.SetTailColor(Color.white);
		bluePlanet.SetTailColor(Color.white);
		Persistence.SetPlayerColor(scrPlanet.transBlueColor, red: true);
		Persistence.SetPlayerColor(scrPlanet.transPinkColor, red: false);
		scrLogoText.instance.UpdateColors();
	}

	private void EnbyMode()
	{
		scrPlanet redPlanet = ADOBase.controller.redPlanet;
		scrPlanet bluePlanet = ADOBase.controller.bluePlanet;
		Color planetColor = new Color(0.996f, 0.953f, 0.18f);
		Color planetColor2 = new Color(0.612f, 0.345f, 0.82f);
		redPlanet.EnableCustomColor();
		bluePlanet.EnableCustomColor();
		redPlanet.SetPlanetColor(planetColor);
		bluePlanet.SetPlanetColor(planetColor2);
		redPlanet.SetTailColor(Color.white);
		bluePlanet.SetTailColor(Color.black);
		Persistence.SetPlayerColor(scrPlanet.nbYellowColor, red: true);
		Persistence.SetPlayerColor(scrPlanet.nbPurpleColor, red: false);
		scrLogoText.instance.UpdateColors();
	}

	private void RainbowMode()
	{
		scrPlanet redPlanet = ADOBase.controller.redPlanet;
		scrPlanet bluePlanet = ADOBase.controller.bluePlanet;
		redPlanet.EnableCustomColor();
		bluePlanet.EnableCustomColor();
		redPlanet.SetRainbow(enabled: true);
		bluePlanet.SetRainbow(enabled: true);
		Persistence.SetPlayerColor(scrPlanet.rainbowColor, red: true);
		Persistence.SetPlayerColor(scrPlanet.rainbowColor, red: false);
		scrLogoText.instance.UpdateColors();
	}

	public void GoToCheatIsland()
	{
		instance.JumpAndWipeWithKey(51);
	}

	public void GoToMuseDashIsland()
	{
		instance.JumpAndWipeWithKey(53);
	}

	private void SamuraiMode()
	{
		scrPlanet redPlanet = ADOBase.controller.redPlanet;
		scrPlanet bluePlanet = ADOBase.controller.bluePlanet;
		bool samuraiMode = Persistence.GetSamuraiMode(red: true);
		bool samuraiMode2 = Persistence.GetSamuraiMode(red: false);
		Persistence.SetSamuraiMode(!samuraiMode, red: true);
		Persistence.SetSamuraiMode(!samuraiMode2, red: false);
		redPlanet.ToggleSamurai(!samuraiMode);
		bluePlanet.ToggleSamurai(!samuraiMode2);
	}

	private void ToggleFaceMode()
	{
		scrPlanet redPlanet = ADOBase.controller.redPlanet;
		scrPlanet bluePlanet = ADOBase.controller.bluePlanet;
		bool faceMode = Persistence.GetFaceMode(red: true);
		bool faceMode2 = Persistence.GetFaceMode(red: false);
		Persistence.SetFaceMode(!faceMode, red: true);
		Persistence.SetFaceMode(!faceMode2, red: false);
		redPlanet.SetFaceMode(!faceMode);
		bluePlanet.SetFaceMode(!faceMode2);
	}
}
