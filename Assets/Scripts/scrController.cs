using DG.Tweening;
using MonsterLove.StateMachine;
using RDTools;
using SharpHook.Native;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class scrController : StateBehaviour
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass176_0
	{
		public scrController _003C_003E4__this;

		public GameObject planetContainer;

		private scrPlanet _003CAwake_003Eg__InitPlanet_007C0(string name, Color c)
		{
			scrPlanet scrPlanet = UnityEngine.Object.Instantiate(_003C_003E4__this.bluePlanet, planetContainer.transform);
			scrPlanet.name = name;
			scrPlanet.EnableCustomColor();
			scrPlanet.SetPlanetColor(c);
			scrPlanet.SetTailColor(c);
			scrPlanet.isExtra = true;
			return scrPlanet;
		}

		internal void _003CAwake_003Eb__1()
		{
			_003C_003E4__this.TogglePauseGame();
		}
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Predicate<scrFloor> _003C_003E9__186_0;

		public static TweenCallback _003C_003E9__197_0;

		public static TweenCallback _003C_003E9__197_1;

		public static TweenCallback _003C_003E9__287_0;

		internal bool _003CWaitForStartCo_003Eb__186_0(scrFloor x)
		{
			return x.seqID <= GCS.checkpointNum;
		}

		internal void _003COnLandOnPortal_003Eb__197_0()
		{
			RDUtils.SetGarbageCollectionEnabled(enabled: true);
		}

		internal void _003COnLandOnPortal_003Eb__197_1()
		{
			ADOBase.conductor.song.Stop();
		}

		internal void _003CFail2Action_003Eb__287_0()
		{
			RDUtils.SetGarbageCollectionEnabled(enabled: true);
		}
	}

	[CompilerGenerated]
	private sealed class _003CWaitForStartCo_003Ed__186 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public scrController _003C_003E4__this;

		public int seqID;

		private scrPressToStart _003CpressToStart_003E5__2;

		private bool _003CprevIsEditingLevel_003E5__3;

		private HashSet<Filter> _003Cfilters_003E5__4;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CWaitForStartCo_003Ed__186(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			scrController scrController = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				scrUIController.instance.canvas.enabled = true;
				scrController.txtCaption.text = scrController.caption;
				if (GCS.speedTrialMode)
				{
					string str = RDString.Get("levelSelect.multiplier", new Dictionary<string, object>
					{
						{
							"multiplier",
							GCS.currentSpeedTrial.ToString("0.0#")
						}
					});
					scrController.txtCaption.text = scrController.caption + " (" + str + ")";
				}
				else if (GCS.practiceMode)
				{
					string str2 = RDString.Get("status.practiceMode");
					scrController.txtCaption.text = scrController.caption + "\n(" + str2 + ")";
				}
				scrController.txtCaption.SetLocalizedFont();
				if (!GCS.standaloneLevelMode)
				{
					ADOBase.lm.CalculateFloorEntryTimes();
				}
				scrController.chosenplanet.FirstFloorAngleSetup();
				_003Cfilters_003E5__4 = new HashSet<Filter>();
				foreach (scrFloor listFloor in ADOBase.lm.listFloors)
				{
					Component[] components = listFloor.GetComponents(typeof(ffxSetFilterPlus));
					for (int i = 0; i < components.Length; i++)
					{
						ffxSetFilterPlus ffxSetFilterPlus = ((ffxPlusBase)components[i]) as ffxSetFilterPlus;
						if (ffxSetFilterPlus.enableFilter)
						{
							_003Cfilters_003E5__4.Add(ffxSetFilterPlus.filter);
						}
					}
				}
				foreach (Filter item in _003Cfilters_003E5__4)
				{
					scrVfxPlus.instance.filterToComp[item].enabled = true;
				}
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				_003C_003E2__current = null;
				_003C_003E1__state = 2;
				return true;
			case 2:
				_003C_003E1__state = -1;
				foreach (Filter item2 in _003Cfilters_003E5__4)
				{
					scrVfxPlus.instance.filterToComp[item2].enabled = false;
				}
				_003Cfilters_003E5__4 = null;
				if (!GCS.standaloneLevelMode)
				{
					CustomLevel.SetFxPlusFromComponents(ADOBase.lm.listFloors, scrController.useComponentNotationForFx);
					CustomLevel.PrepVfx(ADOBase.lm.listFloors, GCS.checkpointNum == 0);
					ADOBase.lm.ColorFreeroam();
					ADOBase.lm.DrawHolds();
					ADOBase.lm.DrawMultiPlanet();
				}
				else
				{
					ADOBase.customLevel.PrepVfx(seqID == 0);
				}
				if (GCS.standaloneLevelMode)
				{
					ADOBase.customLevel.UpdateDecorationObjects();
				}
				if (GCS.checkpointNum != 0)
				{
					foreach (scrFloor item3 in ADOBase.lm.listFloors.FindAll((scrFloor x) => x.seqID <= GCS.checkpointNum))
					{
						scrController.camy.torot += item3.rotatecamera;
						scrController.camy.fromrot = scrController.camy.torot;
						ffxBase[] componentsInChildren = item3.GetComponentsInChildren<ffxBase>();
						foreach (ffxBase ffxBase in componentsInChildren)
						{
							if (!(ffxBase is ffxCheckpoint))
							{
								ffxBase.doEffect();
							}
							else
							{
								item3.floorIcon = FloorIcon.Checkpoint;
								item3.UpdateIconSprite();
							}
						}
						DOTween.CompleteAll(withCallbacks: true);
					}
					scrController.chosenplanet.ScrubToFloorNumber(GCS.checkpointNum);
					scrController.camy.ViewObjectInstant(instance.chosenplanet.transform);
					scrController.speed = scrController.currFloor.speed;
					if (scrVfxPlus.instance != null)
					{
						int index = GCS.checkpointNum;
						ffxCheckpoint component = ADOBase.lm.listFloors[GCS.checkpointNum].GetComponent<ffxCheckpoint>();
						if (component != null && component.scrubFourBack)
						{
							index = scrController.FindScrubStart(GCS.checkpointNum);
						}
						scrVfxPlus.instance.ScrubToTime((float)ADOBase.lm.listFloors[index].entryTime);
						scrController.printe("complete all");
						DOTween.CompleteAll(withCallbacks: true);
					}
					if (scrController.stickToFloor)
					{
						scrController.chosenplanet.transform.position = ADOBase.lm.listFloors[GCS.checkpointNum].transform.position;
					}
				}
				_003CpressToStart_003E5__2 = scrUIController.instance.txtPressToStart.GetComponent<scrPressToStart>();
				_003CpressToStart_003E5__2.ShowText();
				if (GCS.standaloneLevelMode)
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 3;
					return true;
				}
				goto IL_04f9;
			case 3:
				_003C_003E1__state = -1;
				ADOBase.customLevel.isLoading = false;
				goto IL_04f9;
			case 4:
				{
					_003C_003E1__state = -1;
					break;
				}
				IL_04f9:
				_003CprevIsEditingLevel_003E5__3 = ADOBase.isEditingLevel;
				break;
			}
			if (!scrController.ValidInputWasTriggered() || scrController.isCutscene)
			{
				if (ADOBase.isEditingLevel != _003CprevIsEditingLevel_003E5__3)
				{
					_003CpressToStart_003E5__2.HideText();
					return false;
				}
				if (!ADOBase.isEditingLevel || scrController.exitingToMainMenu)
				{
					if (!scrController.paused && ADOBase.uiController.difficultyUIMode != 0 && !scrController.isCutscene)
					{
						if (RDInput.leftPress)
						{
							ADOBase.uiController.DifficultyArrowPressed(rightPressed: false);
						}
						else if (RDInput.rightPress)
						{
							ADOBase.uiController.DifficultyArrowPressed(rightPressed: true);
						}
					}
					scrController.holdKeys.Clear();
					_003C_003E2__current = null;
					_003C_003E1__state = 4;
					return true;
				}
			}
			_003CpressToStart_003E5__2.HideText();
			scrUIController.instance.txtCountdown.GetComponent<scrCountdown>().ShowGetReady();
			ADOBase.conductor.Rewind();
			ADOBase.conductor.Start();
			scrController.Start_Rewind();
			if (scrController.gameworld && scrController.levelNameShouldHide && scrController.currFloor != null)
			{
				DOVirtual.DelayedCall((float)(ADOBase.conductor.crotchetAtStart / (double)scrController.currFloor.speed / (double)ADOBase.conductor.song.pitch * (double)ADOBase.conductor.adjustedCountdownTicks), scrController.LevelNameTextAway);
			}
			if (GCS.standaloneLevelMode)
			{
				ADOBase.customLevel.FinishCustomLevelLoading(seqID, scrController.bluePlanet, scrController.redPlanet);
			}
			RDUtils.SetGarbageCollectionEnabled(enabled: false);
			return false;
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003C_003Ec__DisplayClass197_0
	{
		public bool isPurePerfect;
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003C_003Ec__DisplayClass216_0
	{
		public bool mouseOverAButton;
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass275_0
	{
		public scrController _003C_003E4__this;

		public long? targetTick;

		internal void _003CSimulated_PlayerControl_Update_003Eb__0()
		{
			_003C_003E4__this.CheckPostHoldFail(targetTick);
		}

		internal void _003CSimulated_PlayerControl_Update_003Eb__1()
		{
			_003C_003E4__this.OttoHoldHit(targetTick);
		}

		internal void _003CSimulated_PlayerControl_Update_003Eb__2()
		{
			_003C_003E4__this.HitHoldFloorsIfStartedAtHold(targetTick);
		}

		internal void _003CSimulated_PlayerControl_Update_003Eb__3()
		{
			_003C_003E4__this.CheckPreHoldFail(targetTick);
		}

		internal void _003CSimulated_PlayerControl_Update_003Eb__4()
		{
			_003C_003E4__this.UpdateHoldKeys(targetTick);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass289_0
	{
		public bool complete;

		internal void _003CResetCustomLevel_003Eb__0()
		{
			complete = true;
		}
	}

	[CompilerGenerated]
	private sealed class _003CResetCustomLevel_003Ed__289 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		private _003C_003Ec__DisplayClass289_0 _003C_003E8__1;

		public scrController _003C_003E4__this;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CResetCustomLevel_003Ed__289(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			scrController scrController = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				if (GCS.standaloneLevelMode)
				{
					_003C_003E8__1 = new _003C_003Ec__DisplayClass289_0();
					_003C_003E8__1.complete = false;
					scrUIController.instance.WipeToBlack(WipeDirection.StartsFromRight, delegate
					{
						_003C_003E8__1.complete = true;
					});
					goto IL_007c;
				}
				RDUtils.SetGarbageCollectionEnabled(enabled: true);
				goto IL_0098;
			case 1:
				_003C_003E1__state = -1;
				goto IL_007c;
			case 2:
				{
					_003C_003E1__state = -1;
					scrUIController.instance.WipeFromBlack();
					break;
				}
				IL_007c:
				if (!_003C_003E8__1.complete)
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				_003C_003E8__1 = null;
				goto IL_0098;
				IL_0098:
				foreach (scrFloor listFloor in ADOBase.lm.listFloors)
				{
					if ((bool)listFloor.bottomGlow)
					{
						listFloor.bottomGlow.enabled = false;
					}
					listFloor.topGlow.enabled = false;
				}
				if (!GCS.practiceMode && GCS.standaloneLevelMode)
				{
					GCS.currentSpeedTrial = GCS.nextSpeedRun;
				}
				scrController.printe($"GCS.currentSpeedTrial {GCS.currentSpeedTrial} GCS.nextSpeedRun {GCS.nextSpeedRun}");
				ADOBase.customLevel.ResetScene();
				ADOBase.customLevel.Play(GCS.checkpointNum);
				scrController.transitioningLevel = false;
				if (GCS.standaloneLevelMode)
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
				break;
			}
			return false;
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	public const int EndOfLevel = -1;

	public const int LastLevelPlayed = -2;

	public const int CalibrationScene = -3;

	public const int EditorScene = -4;

	public const int CustomLevelsScene = -5;

	public const int RDSteamPage = -77777;

	public const int PreviousLevel = -10;

	public const int NextLevel = -11;

	public const int LowerSpeed = -12;

	public const int HigherSpeed = -13;

	public const int GoToLevel = -14;

	public const int GoToLevelSpeedTrial = -15;

	public const int GoToWorldBossIfReached = -16;

	public const int TaroDLCMapExit = -59;

	public const int TaroDLCMap = -60;

	public const int PuzzleTest = -573;

	public const int Puzzle1 = -574;

	public const int Puzzle2 = -575;

	public const int Puzzle3 = -576;

	public const int TaroDLCMap3 = -577;

	public const float boothModeDebounceCooldownTime = 0.1f;

	private const float MaxTimeToTypeDebugCheat = 8f;

	private readonly RDCheatCode debugModeCheatCode = new RDCheatCode("despacit0");

	private readonly RDCheatCode typingModeCheatCode = new RDCheatCode("typing");

	public static int volume;

	public static bool showDetailedResults;

	public static string currentWorldString;

	public static int deaths;

	public static int checkpointsUsed;

	private static scrController _instance;

	public static bool displayedMultiFingerHint = false;

	private static float lastTimeStatsUploaded;

	private const float IntervalToUpdateSteamStats = 120f;

	[Header("GameObjects and MonoBehaviours")]
	public GameObject background;

	public GameObject lofiBackground;

	public scrMistakesManager mistakesManager;

	public scrDecorationManager decorationManager;

	public scrPlanet chosenplanet;

	public scrCamera camy;

	public scrFailBar failbar;

	[Header("Toggles")]
	public bool forceNoCountdown;

	public bool gameworld;

	public bool stickToFloor;

	public bool instantExplode;

	public bool forceHitTextOnScreen;

	public bool isbigtiles = true;

	public bool usingOutlines;

	public bool usingInitialTrackStyles;

	[Header("Variables")]
	public new float d_speed;

	public float hitTextMinBorderDistance;

	public bool isbosslevel;

	public bool useComponentNotationForFx = true;

	public int customCheckpoint;

	public int planetsUsed = 2;

	public bool safetyTilesArePresent;

	public int numFastPresses;

	public bool recommendsTwoFingers;

	private Dictionary<HitMargin, scrHitTextMesh[]> cachedHitTexts;

	public Dictionary<FontName, Font> nameToFont = new Dictionary<FontName, Font>();

	[NonSerialized]
	public string caption;

	[NonSerialized]
	public string levelName;

	[NonSerialized]
	public bool goShown;

	[NonSerialized]
	public bool moving;

	[NonSerialized]
	public int curCountdown;

	[NonSerialized]
	public List<ffxPlusBase> perfectEffects = new List<ffxPlusBase>();

	[NonSerialized]
	public List<ffxPlusBase> hitEffects = new List<ffxPlusBase>();

	[NonSerialized]
	public List<ffxPlusBase> barelyEffects = new List<ffxPlusBase>();

	[NonSerialized]
	public List<ffxPlusBase> missEffects = new List<ffxPlusBase>();

	[NonSerialized]
	public List<ffxPlusBase> lossEffects = new List<ffxPlusBase>();

	[NonSerialized]
	public States currentState;

	[NonSerialized]
	public int currentSeqID;

	[NonSerialized]
	public bool setupComplete;

	[NonSerialized]
	public List<double> keyTimes = new List<double>();

	[NonSerialized]
	public List<object> holdKeys = new List<object>();

	[NonSerialized]
	public bool startedFromCheckpoint;

	[NonSerialized]
	public int consecMultipressCounter;

	[NonSerialized]
	public float averageFrameTime;

	[NonSerialized]
	public EndLevelType endLevelType;

	[NonSerialized]
	public int menuPhase;

	[NonSerialized]
	public float boothModeDebounceCounter;

	[NonSerialized]
	public bool midspinInfiniteMargin;

	[NonSerialized]
	public bool responsive = true;

	[NonSerialized]
	public Ease rotationEase = Ease.Linear;

	[NonSerialized]
	public int rotationEaseParts = 1;

	[NonSerialized]
	public EasePartBehavior rotationEasePartBehavior;

	[NonSerialized]
	public bool multipressPenalty;

	[NonSerialized]
	public bool multipressAndHasPressedFirstPress;

	[NonSerialized]
	public bool noFail;

	[NonSerialized]
	public bool noFailInfiniteMargin;

	[NonSerialized]
	public double speed = 1.0;

	[NonSerialized]
	public bool isCW = true;

	[NonSerialized]
	public int portalDestination = -1;

	[NonSerialized]
	public string portalArguments = "";

	[NonSerialized]
	public Text txtCongrats;

	[NonSerialized]
	public Text txtAllStrictClear;

	[NonSerialized]
	public Text txtResults;

	[NonSerialized]
	public Text txtPercent;

	[NonSerialized]
	public Text txtTryCalibrating;

	[NonSerialized]
	public Text txtCaption;

	[NonSerialized]
	public PauseMenu pauseMenu;

	[NonSerialized]
	public scrPlanet bluePlanet;

	[NonSerialized]
	public scrPlanet redPlanet;

	[NonSerialized]
	public scrHitErrorMeter errorMeter;

	[NonSerialized]
	public GameObject hitTextContainer;

	[NonSerialized]
	public TakeScreenshot takeScreenshot;

	[NonSerialized]
	public List<scrMissIndicator> missesOnCurrFloor = new List<scrMissIndicator>();

	[NonSerialized]
	public List<scrLetterPress> typingLetters = new List<scrLetterPress>();

	[NonSerialized]
	public VisualQuality visualQuality;

	[NonSerialized]
	public VisualEffects visualEffects;

	public List<scrPlanet> dummyPlanets;

	public List<LineRenderer> multiPlanetLines;

	public Material lineMaterial;

	public Color lineColor;

	public Level level;

	public int curFreeRoamSection;

	public float freeroamUpTime;

	public bool controllerUpdate;

	public float freeroamAngleInterval = 90f;

	public float freeroamAngleOffset;

	public bool levelNameShouldHide;

	private bool _paused;

	private bool transitioningLevel;

	private bool benchmarkMode;

	private float oldPercentComplete;

	private bool disableCongratsMessage;

	private float debugTileTime = -100f;

	private int numTimesSfxToggled;

	[NonSerialized]
	public List<scrPlanet> planetList;

	[NonSerialized]
	public List<scrPlanet> allPlanets;

	public bool strictHolds;

	public bool strictHoldsSaved;

	public bool requireHolding;

	public bool freeroamInvulnerability;

	private scrDpadInputChecker dpadInputChecker;

	[NonSerialized]
	public int currentFloorID;

	private int lastTogglePauseFrame;

	[NonSerialized]
	public scrPlanet planetGreen;

	[NonSerialized]
	public scrPlanet planetYellow;

	[NonSerialized]
	public scrPlanet planetPurple;

	[NonSerialized]
	public scrPlanet planetPink;

	[NonSerialized]
	public scrPlanet planetOrange;

	[NonSerialized]
	public scrPlanet planetCyan;

	private List<scrPlanet> availablePlanets;

	public float lockInput;

	public bool isCutscene;

	public bool isPuzzleRoom;

	public bool canExitLevel = true;

	[NonSerialized]
	public List<Tuple<double, double>> listBPM = new List<Tuple<double, double>>();

	[NonSerialized]
	public bool forceOK;

	private bool validKeyWasReleased;

	public Dictionary<object, int> keyFrequency = new Dictionary<object, int>();

	public int keyTotal;

	private int frameStart;

	private const int DontScrub = -1;

	[NonSerialized]
	public float popuptime = 0.5f;

	public int lastCamPulseFloor = -1;

	private bool exitingToMainMenu;

	private scrPlanet scrubchosen;

	private static readonly FieldInfo DestinationStateField = typeof(StateEngine).GetField("destinationState", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty);

	private bool levelNameTextPresent = true;

	private RectTransform lvlname;

	private Vector2 lvlnameAnchorPos;

	[NonSerialized]
	public float startVolume;

	private double startTime;

	private float winTime;

	public static readonly States[] deathStates = new States[2]
	{
		States.Fail,
		States.Fail2
	};

	private bool validInputWasReleasedThisFrame;

	public static bool shouldReplaceCamyToPos;

	public static Vector3 overrideCamyToPos;

	private Vector3 cachedCamyToPos;

	private bool __nextTileIsHoldCached;

	public static int currentWorld => GCNS.worldData[currentWorldString].index;

	public bool holding => holdKeys.Count != 0;

	public static scrController instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType<scrController>();
			}
			return _instance;
		}
	}

	public bool audioPaused
	{
		get
		{
			return AudioListener.pause;
		}
		set
		{
			AudioListener.pause = value;
		}
	}

	public bool paused
	{
		get
		{
			return _paused;
		}
		set
		{
			lastTogglePauseFrame = Time.frameCount;
			_paused = value;
			if ((bool)errorMeter && gameworld && Persistence.GetHitErrorMeterSize() != 0)
			{
				errorMeter.gameObject.SetActive(!value);
			}
			if (paused)
			{
				RDInput.SetMapping("Pause");
			}
			else if (ADOBase.isLevelSelect)
			{
				RDInput.SetMapping("LevelSelect");
			}
			else if (ADOBase.isCLS)
			{
				RDInput.SetMapping("CLS");
			}
			else
			{
				RDInput.SetMapping("Gameplay");
			}
		}
	}

	public float startRadius
	{
		get
		{
			if (!isbigtiles)
			{
				return 1f;
			}
			return 1.5f;
		}
	}

	public static bool isGameWorld => instance.gameworld;

	public scrFloor currFloor => chosenplanet.currfloor;

	public float percentComplete => (float)(instance.currentSeqID + 1) / (float)ADOBase.lm.listFloors.Count;

	public bool toggledPausedThisFrame => lastTogglePauseFrame == Time.frameCount;

	public bool saveProgressConditions
	{
		get
		{
			if (ADOBase.isOfficialLevel && ADOBase.isBossLevel && !GCS.speedTrialMode)
			{
				return !GCS.practiceMode;
			}
			return false;
		}
	}

	private double _minAngleMargin => scrMisc.GetAdjustedAngleBoundaryInDeg(HitMarginGeneral.Counted, (float)((double)ADOBase.conductor.bpm * speed), ADOBase.conductor.song.pitch) * 0.01745329238474369;

	private float _marginScale
	{
		get
		{
			if (!(currFloor.nextfloor != null))
			{
				return 1f;
			}
			return (float)currFloor.nextfloor.marginScale;
		}
	}

	private double _holdMargin => 1.0 - _minAngleMargin * (double)_marginScale / currFloor.angleLength;

	private bool _nextTileIsAuto
	{
		get
		{
			if (currFloor.nextfloor != null)
			{
				return currFloor.nextfloor.auto;
			}
			return false;
		}
	}

	private bool _nextTileIsHold
	{
		get
		{
			if (!__nextTileIsHoldCached)
			{
				if ((bool)currFloor.nextfloor)
				{
					return currFloor.nextfloor.holdLength > -1;
				}
				return false;
			}
			return true;
		}
	}

	private void Awake()
	{
		ADOBase.Startup();
		RDInput.SetMapping("Gameplay");
		RDUtils.SetGarbageCollectionEnabled(enabled: true);
		if (gameworld)
		{
			MakeNewFontDictionary();
		}
		if (!gameworld || GCS.standaloneLevelMode)
		{
			FlushUnusedMemory();
		}
		if (RDC.deleteSavedProgress)
		{
			RDC.deleteSavedProgress = false;
			Persistence.DeleteSavedProgress();
		}
		dpadInputChecker = base.gameObject.GetOrAddComponent<scrDpadInputChecker>();
		redPlanet = GameObject.Find("PlanetRed").GetComponent<scrPlanet>();
		bluePlanet = redPlanet.other;
		planetList = new List<scrPlanet>
		{
			redPlanet,
			bluePlanet
		};
		GameObject planetContainer = redPlanet.transform.parent.gameObject;
		for (int i = 0; i < planetList.Count; i++)
		{
			planetList[i].planetIndex = i;
			planetList[i].next = GetMultiPlanet(i, 1);
			planetList[i].prev = GetMultiPlanet(i, -1);
		}
		canExitLevel = true;
		_003C_003Ec__DisplayClass176_0 CS_0024_003C_003E8__locals0;
		availablePlanets = new List<scrPlanet>
		{
			planetGreen = CS_0024_003C_003E8__locals0._003CAwake_003Eg__InitPlanet_007C0("PlanetGreen", new Color(0.3f, 0.7f, 0f, 1f)),
			planetYellow = CS_0024_003C_003E8__locals0._003CAwake_003Eg__InitPlanet_007C0("PlanetYellow", new Color(1f, 0.8f, 0f, 1f)),
			planetPurple = CS_0024_003C_003E8__locals0._003CAwake_003Eg__InitPlanet_007C0("PlanetPurple", new Color(0.7f, 0.1f, 1f, 1f)),
			planetPink = CS_0024_003C_003E8__locals0._003CAwake_003Eg__InitPlanet_007C0("PlanetPink", new Color(1f, 0.1f, 0.7f, 1f)),
			planetOrange = CS_0024_003C_003E8__locals0._003CAwake_003Eg__InitPlanet_007C0("PlanetOrange", new Color(1f, 0.4f, 0.1f, 1f)),
			planetCyan = CS_0024_003C_003E8__locals0._003CAwake_003Eg__InitPlanet_007C0("PlanetCyan", new Color(0.1f, 0.8f, 0.9f, 1f))
		};
		foreach (scrPlanet availablePlanet in availablePlanets)
		{
			availablePlanet.Destroy();
		}
		allPlanets = new List<scrPlanet>
		{
			redPlanet,
			bluePlanet
		};
		allPlanets.AddRange(availablePlanets);
		dummyPlanets = new List<scrPlanet>();
		multiPlanetLines = new List<LineRenderer>();
		lineMaterial = new Material(Shader.Find("ADOFAI/ScrollingSprite"));
		lineMaterial.SetTexture("_MainTex", ADOBase.gc.planetPolygonTex);
		lineMaterial.SetVector("_ScrollSpeed", new Vector2(-0.4f, 0f));
		lineMaterial.SetFloat("_Time0", 0f);
		lineColor = new Color(1f, 1f, 1f, 0.5f);
		Application.runInBackground = true;
		if (GCS.wasInCalibration)
		{
			GCS.checkpointNum = GCS.savedCheckpointNum;
			GCS.savedCheckpointNum = 0;
			GCS.wasInCalibration = false;
		}
		if (RDC.customCheckpoint && ADOBase.isUnityEditor)
		{
			RDC.customCheckpoint = false;
			GCS.checkpointNum = RDC.customCheckpointPos;
		}
		else
		{
			bool practiceMode = GCS.practiceMode;
		}
		if (GCS.previousScene == null)
		{
			GCS.previousScene = base.gameObject.scene.name;
		}
		if (GCS.previousScene != base.gameObject.scene.name)
		{
			GCS.checkpointNum = 0;
			GCS.previousScene = GCS.sceneToLoad;
		}
		levelName = base.gameObject.scene.name;
		if (saveProgressConditions)
		{
			Dictionary<string, object> savedProgress = Persistence.GetSavedProgress();
			if (savedProgress.Keys.Count > 2 && (string)savedProgress["level"] == levelName)
			{
				scrMistakesManager.LoadProgress();
			}
		}
		responsive = true;
		lockInput = 0f;
		if (GCS.turnOnBenchmarkMode)
		{
			benchmarkMode = true;
			GCS.turnOnBenchmarkMode = false;
		}
		RDC.auto = false;
		Initialize<States>();
		if (gameworld)
		{
			if (background == null)
			{
				background = GameObject.Find("BG");
				if (background == null)
				{
					background = GameObject.Find("Tutorial BG");
				}
			}
			if (ADOBase.isBossLevel)
			{
				if (visualQuality == VisualQuality.High)
				{
					background.SetActive(value: true);
				}
				else if (lofiBackground != null)
				{
					Sprite sprite = lofiBackground.GetComponent<SpriteRenderer>().sprite;
					if ((bool)sprite)
					{
						Rect rect = sprite.rect;
						float num = 1f / rect.height * 10f;
						float x = (float)Screen.width * 1f / (float)Screen.height / (rect.width * 1f / rect.height) * 1.0005f * num;
						printe($"texture [{rect.width} x {rect.height}] height: {num}, Screen {Screen.width} x {Screen.height}");
						lofiBackground.transform.localScale = new Vector3(x, num, 1f);
						lofiBackground.SetActive(value: true);
					}
					else
					{
						MonoBehaviour.print("oh no sprite is null, lets just ignore the sprite entirely");
					}
				}
			}
		}
		if (ADOBase.isCLS || ADOBase.isLevelSelect || ADOBase.isLevelEditor)
		{
			GCS.practiceMode = false;
			GCS.checkpointBeforePractice = 0;
			GCS.speedTrialModeBeforePractice = false;
			GCS.speedRunBeforePractice = 1f;
		}
		if (!gameworld || GCS.speedTrialMode)
		{
			GCS.checkpointNum = 0;
		}
		if (levelName.Contains("-"))
		{
			scrUIController.instance.canvas.enabled = true;
			txtCaption = scrUIController.instance.txtLevelName;
			txtCaption.SetLocalizedFont();
			caption = ADOBase.GetLocalizedLevelName(levelName);
			string text = levelName.Substring(0, levelName.IndexOf('-'));
			bool num2 = ADOBase.worldData.ContainsKey(text);
			if (!num2)
			{
				printe(currentWorldString + " is not present in WorldData...");
			}
			currentWorldString = (num2 ? text : "Template");
		}
		if (ADOBase.isMobile)
		{
			Button pauseButton = scrUIController.instance.pauseButton;
			pauseButton.gameObject.SetActive(value: true);
			pauseButton.onClick.AddListener(delegate
			{
				TogglePauseGame();
			});
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(ADOBase.gc.canvasPrefab);
		if (!GCS.lofiVersion || ADOBase.isMobile)
		{
			pauseMenu = gameObject.transform.Find("RDPauseMenu").GetComponent<PauseMenu>();
			if ((float)Screen.width / (float)Screen.height < 1.5f)
			{
				gameObject.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.2f;
				RectTransform component = pauseMenu.settingsMenu.GetComponent<RectTransform>();
				component.offsetMax = component.offsetMax.WithY(-20f);
			}
			takeScreenshot = camy.GetOrAddComponent<TakeScreenshot>();
		}
		mistakesManager = new scrMistakesManager(ADOBase.controller, ADOBase.lm);
		if (GCS.checkpointNum == 0)
		{
			mistakesManager.Reset();
			checkpointsUsed = 0;
		}
		mistakesManager.RevertToLastCheckpoint();
		if (gameworld)
		{
			errorMeter = UnityEngine.Object.Instantiate(ADOBase.gc.errorMeterPrefab).GetComponent<scrHitErrorMeter>();
			errorMeter.gameObject.SetActive(value: false);
			errorMeter.UpdateLayout(Persistence.GetHitErrorMeterSize(), Persistence.GetHitErrorMeterShape());
		}
		SaveDataSetup();
		Awake_Rewind();
		if (gameworld)
		{
			scrUIController.instance.PrepareWipeFromBlack();
			hitTextContainer = new GameObject("HitTexts");
			Transform transform = hitTextContainer.transform;
			HitMargin[] obj = (HitMargin[])Enum.GetValues(typeof(HitMargin));
			cachedHitTexts = new Dictionary<HitMargin, scrHitTextMesh[]>();
			HitMargin[] array = obj;
			foreach (HitMargin hitMargin in array)
			{
				if (hitMargin != HitMargin.Auto)
				{
					scrHitTextMesh[] array2 = new scrHitTextMesh[100];
					for (int k = 0; k < 100; k++)
					{
						scrHitTextMesh componentInChildren = UnityEngine.Object.Instantiate(ADOBase.gc.hitTextPrefab, transform).GetComponentInChildren<scrHitTextMesh>();
						componentInChildren.Init(hitMargin);
						array2[k] = componentInChildren;
					}
					cachedHitTexts[hitMargin] = array2;
				}
			}
		}
		if (benchmarkMode)
		{
			base.gameObject.AddComponent<scrBenchmark>();
		}
		if (ADOBase.IsHalloweenWeek() && ADOBase.conductor != null && ADOBase.conductor.song != null && ADOBase.conductor.song.clip != null && ADOBase.conductor.song.clip.name.StartsWith("1-X"))
		{
			ADOBase.conductor.song.clip = ADOBase.gc.halloweenMusic;
			RDBaseDll.printem("it's Halloween! conductor song is now: " + ADOBase.conductor.song.clip.name);
		}
		lastTimeStatsUploaded = Time.unscaledTime;
		if (!gameworld)
		{
			SteamIntegration.EditorEntered();
		}
		noFail = GCS.useNoFail;
		if (noFail)
		{
			freeroamInvulnerability = true;
		}
	}

	public void LoadCustomWorld(string levelPath, bool skipToMain = false)
	{
		GCS.sceneToLoad = "scnEditor";
		GCS.customLevelPaths = CustomLevel.GetWorldPaths(levelPath, excludeMain: false, renamed: true);
		GCS.standaloneLevelMode = true;
		GCS.customLevelIndex = (skipToMain ? (GCS.customLevelPaths.Length - 1) : 0);
		StartLoadingScene(WipeDirection.StartsFromRight);
	}

	public void LoadCustomLevel(string levelPath)
	{
		GCS.sceneToLoad = "scnEditor";
		GCS.customLevelPaths = new string[1];
		GCS.customLevelPaths[0] = levelPath;
		GCS.standaloneLevelMode = true;
		StartLoadingScene(WipeDirection.StartsFromRight);
	}

	public void SetupImportantVariables()
	{
		visualQuality = Persistence.GetVisualQuality();
		visualEffects = Persistence.GetVisualEffects();
		strictHoldsSaved = (Persistence.GetHoldBehavior() == HoldBehavior.Normal);
		strictHolds = strictHoldsSaved;
		requireHolding = (Persistence.GetHoldBehavior() < HoldBehavior.NoHoldNeeded);
		freeroamInvulnerability = Persistence.GetFreeroamInvulnerability();
	}

	public void Awake_Rewind()
	{
		if (ADOBase.sceneName == "MO-X")
		{
			visualQuality = VisualQuality.High;
			visualEffects = VisualEffects.Full;
		}
		if (ADOBase.sceneName == "ML-X")
		{
			visualEffects = VisualEffects.Full;
			level = new LevelML();
		}
		if (ADOBase.sceneName == "MN-X")
		{
			visualEffects = VisualEffects.Full;
		}
		if (ADOBase.sceneName.StartsWith("T"))
		{
			visualQuality = VisualQuality.High;
		}
		keyFrequency.Clear();
		keyTotal = 0;
		failbar.Rewind();
		if (scrVfxPlus.instance != null)
		{
			scrVfxPlus.instance.enabled = true;
		}
		if (chosenplanet == null)
		{
			chosenplanet = redPlanet;
		}
		frameStart = Time.frameCount;
		paused = false;
		Time.timeScale = 1f;
		AudioListener.pause = false;
		audioPaused = false;
		goShown = false;
		isCW = true;
		multipressAndHasPressedFirstPress = false;
		multipressPenalty = false;
		consecMultipressCounter = 0;
		forceOK = false;
		curFreeRoamSection = 0;
		freeroamUpTime = 0f;
		curCountdown = 0;
		controllerUpdate = false;
		levelNameShouldHide = false;
		ChangeState(States.Start);
		if (gameworld)
		{
			if (!ADOBase.isLevelEditor)
			{
				Persistence.SetSavedCurrentLevel(levelName);
			}
			txtCongrats = scrUIController.instance.txtCongrats;
			txtCongrats.SetLocalizedFont();
			txtAllStrictClear = scrUIController.instance.txtAllStrictClear;
			txtAllStrictClear.SetLocalizedFont();
			txtResults = scrUIController.instance.txtResults;
			txtResults.SetLocalizedFont();
			txtResults.text = "";
			txtTryCalibrating = scrUIController.instance.txtTryCalibrating;
			txtTryCalibrating.SetLocalizedFont();
			txtTryCalibrating.text = "";
			txtPercent = scrUIController.instance.txtPercent;
			txtCaption = scrUIController.instance.txtLevelName;
			float a = GCS.speedTrialMode ? GCS.currentSpeedTrial : (ADOBase.isEditingLevel ? ADOBase.editor.playbackSpeed : 1f);
			if (Mathf.Approximately(a, 1f))
			{
				txtCaption.text = caption;
			}
			else
			{
				string str = RDString.Get("levelSelect.multiplier", new Dictionary<string, object>
				{
					{
						"multiplier",
						a.ToString("0.0#")
					}
				});
				txtCaption.text = caption + " (" + str + ")";
			}
			txtCaption.SetLocalizedFont();
		}
		GameObject gameObject = GameObject.Find("BGMovingCam");
		if (gameObject != null)
		{
			CameraMotionBlur component = gameObject.GetComponent<CameraMotionBlur>();
			if (component != null)
			{
				component.enabled = false;
			}
		}
		if (!gameworld && scrUIController.instance != null)
		{
			scrUIController.instance.SetToBlack();
		}
		camy.GetComponent<Grayscale>().enabled = false;
		if (gameworld)
		{
			foreach (scrFloor listFloor in scrLevelMaker.instance.listFloors)
			{
				if (listFloor == null)
				{
					return;
				}
				if (GCS.practiceMode)
				{
					int num = Math.Min(GCS.checkpointNum + GCS.practiceLength, scrLevelMaker.instance.listFloors.Count - 1);
					bool flag = listFloor.seqID == num;
					if (flag || listFloor.isportal)
					{
						listFloor.isportal = flag;
						listFloor.UpdateIconSprite();
					}
				}
				if (listFloor.isportal)
				{
					if (listFloor.GetComponentsInChildren<scrPortalParticles>().Length != 0)
					{
						break;
					}
					listFloor.SpawnPortalParticles();
				}
			}
		}
		if ((bool)errorMeter && gameworld)
		{
			errorMeter.Reset();
		}
	}

	private void PostSong()
	{
		if ((!gameworld && !currFloor.freeroamGenerated) || isPuzzleRoom)
		{
			ADOBase.conductor.StartMusic(PostSong);
		}
	}

	private void SaveDataSetup()
	{
		if (!ADOBase.isLevelEditor)
		{
			if (gameworld || (!gameworld && isPuzzleRoom))
			{
				GCS.worldEntrance = currentWorldString;
			}
			else
			{
				GCS.maxLevel = PlayerPrefs.GetInt("maxlevel", 0);
			}
			GCS.maxCalibrationRank = PlayerPrefs.GetInt("maxcalibrationrank", 0);
			if (GCS.d_kong)
			{
				scrKongAPI.Submit("Highest Level", GCS.maxLevel - 1);
				scrKongAPI.Submit("Calibration Rank", GCS.maxCalibrationRank);
			}
		}
		if (GCS.d_customhitmargins)
		{
			GCS.HITMARGIN_COUNTED = PlayerPrefs.GetFloat("difficulty", 60f);
		}
		setupComplete = true;
		if (!gameworld)
		{
			currentWorldString = null;
		}
	}

	private void Start()
	{
		bool debug = RDC.debug;
		if (GCS.d_recording)
		{
			RDC.auto = true;
			RDC.noHud = true;
		}
		responsive = true;
		lockInput = 0f;
		if (ADOBase.isEditingLevel)
		{
			base.enabled = false;
			return;
		}
		if (gameworld)
		{
			if (!GCS.practiceMode)
			{
				GCS.currentSpeedTrial = GCS.nextSpeedRun;
			}
			ADOBase.conductor.song.pitch *= GCS.currentSpeedTrial;
			if (isbosslevel)
			{
				oldPercentComplete = Persistence.GetPercentCompletion(currentWorld);
				Persistence.IncrementWorldAttempts(currentWorld);
				Persistence.IncrementWorldAttemptsWithoutNewBest(currentWorld);
			}
			if (!GCS.standaloneLevelMode)
			{
				StartCoroutine(WaitForStartCo());
			}
		}
		else
		{
			Start_Rewind();
		}
		CheckForAudioOutputChange();
		if (!GCS.standaloneLevelMode)
		{
			DiscordController.instance?.UpdatePresence();
		}
		Shader.WarmupAllShaders();
	}

	private void CheckForAudioOutputChange()
	{
		if (scrConductor.HasAudioOutputChanged())
		{
			scrConductor.UpdateCurrentAudioOutput();
			Notification.instance.Show();
		}
	}

	public IEnumerator WaitForStartCo(int seqID = 0)
	{
		scrUIController.instance.canvas.enabled = true;
		txtCaption.text = caption;
		if (GCS.speedTrialMode)
		{
			string str = RDString.Get("levelSelect.multiplier", new Dictionary<string, object>
			{
				{
					"multiplier",
					GCS.currentSpeedTrial.ToString("0.0#")
				}
			});
			txtCaption.text = caption + " (" + str + ")";
		}
		else if (GCS.practiceMode)
		{
			string str2 = RDString.Get("status.practiceMode");
			txtCaption.text = caption + "\n(" + str2 + ")";
		}
		txtCaption.SetLocalizedFont();
		if (!GCS.standaloneLevelMode)
		{
			ADOBase.lm.CalculateFloorEntryTimes();
		}
		chosenplanet.FirstFloorAngleSetup();
		HashSet<Filter> filters = new HashSet<Filter>();
		foreach (scrFloor listFloor in ADOBase.lm.listFloors)
		{
			Component[] components = listFloor.GetComponents(typeof(ffxSetFilterPlus));
			for (int i = 0; i < components.Length; i++)
			{
				ffxSetFilterPlus ffxSetFilterPlus = ((ffxPlusBase)components[i]) as ffxSetFilterPlus;
				if (ffxSetFilterPlus.enableFilter)
				{
					filters.Add(ffxSetFilterPlus.filter);
				}
			}
		}
		foreach (Filter item in filters)
		{
			scrVfxPlus.instance.filterToComp[item].enabled = true;
		}
		yield return null;
		yield return null;
		foreach (Filter item2 in filters)
		{
			scrVfxPlus.instance.filterToComp[item2].enabled = false;
		}
		if (!GCS.standaloneLevelMode)
		{
			CustomLevel.SetFxPlusFromComponents(ADOBase.lm.listFloors, useComponentNotationForFx);
			CustomLevel.PrepVfx(ADOBase.lm.listFloors, GCS.checkpointNum == 0);
			ADOBase.lm.ColorFreeroam();
			ADOBase.lm.DrawHolds();
			ADOBase.lm.DrawMultiPlanet();
		}
		else
		{
			ADOBase.customLevel.PrepVfx(seqID == 0);
		}
		if (GCS.standaloneLevelMode)
		{
			ADOBase.customLevel.UpdateDecorationObjects();
		}
		if (GCS.checkpointNum != 0)
		{
			foreach (scrFloor item3 in ADOBase.lm.listFloors.FindAll((scrFloor x) => x.seqID <= GCS.checkpointNum))
			{
				camy.torot += item3.rotatecamera;
				camy.fromrot = camy.torot;
				ffxBase[] componentsInChildren = item3.GetComponentsInChildren<ffxBase>();
				foreach (ffxBase ffxBase in componentsInChildren)
				{
					if (!(ffxBase is ffxCheckpoint))
					{
						ffxBase.doEffect();
					}
					else
					{
						item3.floorIcon = FloorIcon.Checkpoint;
						item3.UpdateIconSprite();
					}
				}
				DOTween.CompleteAll(withCallbacks: true);
			}
			chosenplanet.ScrubToFloorNumber(GCS.checkpointNum);
			camy.ViewObjectInstant(instance.chosenplanet.transform);
			speed = currFloor.speed;
			if (scrVfxPlus.instance != null)
			{
				int index = GCS.checkpointNum;
				ffxCheckpoint component = ADOBase.lm.listFloors[GCS.checkpointNum].GetComponent<ffxCheckpoint>();
				if (component != null && component.scrubFourBack)
				{
					index = FindScrubStart(GCS.checkpointNum);
				}
				scrVfxPlus.instance.ScrubToTime((float)ADOBase.lm.listFloors[index].entryTime);
				printe("complete all");
				DOTween.CompleteAll(withCallbacks: true);
			}
			if (stickToFloor)
			{
				chosenplanet.transform.position = ADOBase.lm.listFloors[GCS.checkpointNum].transform.position;
			}
		}
		scrPressToStart pressToStart = scrUIController.instance.txtPressToStart.GetComponent<scrPressToStart>();
		pressToStart.ShowText();
		if (GCS.standaloneLevelMode)
		{
			yield return null;
			ADOBase.customLevel.isLoading = false;
		}
		bool prevIsEditingLevel = ADOBase.isEditingLevel;
		while (!ValidInputWasTriggered() || isCutscene)
		{
			if (ADOBase.isEditingLevel != prevIsEditingLevel)
			{
				pressToStart.HideText();
				yield break;
			}
			if (ADOBase.isEditingLevel && !exitingToMainMenu)
			{
				break;
			}
			if (!paused && ADOBase.uiController.difficultyUIMode != 0 && !isCutscene)
			{
				if (RDInput.leftPress)
				{
					ADOBase.uiController.DifficultyArrowPressed(rightPressed: false);
				}
				else if (RDInput.rightPress)
				{
					ADOBase.uiController.DifficultyArrowPressed(rightPressed: true);
				}
			}
			holdKeys.Clear();
			yield return null;
		}
		pressToStart.HideText();
		scrUIController.instance.txtCountdown.GetComponent<scrCountdown>().ShowGetReady();
		ADOBase.conductor.Rewind();
		ADOBase.conductor.Start();
		Start_Rewind();
		if (gameworld && levelNameShouldHide && currFloor != null)
		{
			DOVirtual.DelayedCall((float)(ADOBase.conductor.crotchetAtStart / (double)currFloor.speed / (double)ADOBase.conductor.song.pitch * (double)ADOBase.conductor.adjustedCountdownTicks), LevelNameTextAway);
		}
		if (GCS.standaloneLevelMode)
		{
			ADOBase.customLevel.FinishCustomLevelLoading(seqID, bluePlanet, redPlanet);
		}
		RDUtils.SetGarbageCollectionEnabled(enabled: false);
	}

	public void Start_Rewind(int _currentSeqID = -1)
	{
		if (isGameWorld)
		{
			while (_currentSeqID < ADOBase.lm.listFloors.Count - 1 && _currentSeqID > 1 && ADOBase.lm.listFloors[_currentSeqID].freeroam)
			{
				_currentSeqID--;
			}
		}
		safetyTilesArePresent = false;
		if (Persistence.GetFreeroamInvulnerability())
		{
			safetyTilesArePresent = true;
		}
		else if (ADOBase.lm != null)
		{
			foreach (scrFloor listFloor in ADOBase.lm.listFloors)
			{
				if (listFloor != null && listFloor.isSafe)
				{
					safetyTilesArePresent = true;
					break;
				}
			}
		}
		if (ADOBase.isEditingLevel)
		{
			printe("killing all tweens");
			DOTween.KillAll(complete: true);
			failbar.Rewind();
		}
		if (_currentSeqID != -1)
		{
			GCS.checkpointNum = _currentSeqID;
		}
		if (GCS.checkpointNum != 0 && ADOBase.isLevelEditor && !GCS.standaloneLevelMode)
		{
			scrUIController.instance.SetToBlack();
		}
		if (GCS.d_oldConductor)
		{
			ChangeState(States.Countdown);
		}
		if (GCS.checkpointNum != 0)
		{
			ADOBase.conductor.song.volume = 0f;
		}
		ADOBase.conductor.LoadOnBeats();
		ADOBase.conductor.StartMusic(PostSong, OnMusicScheduled);
		if (gameworld)
		{
			if (!GCS.standaloneLevelMode)
			{
				ADOBase.lm.CalculateFloorEntryTimes();
			}
			ADOBase.lm.CalculateFloorAngleLengths();
			if (gameworld)
			{
				listBPM.Add(new Tuple<double, double>(0.0, ADOBase.conductor.bpm));
				float num = 1f;
				foreach (scrFloor listFloor2 in scrLevelMaker.instance.listFloors)
				{
					if (listFloor2 == null)
					{
						return;
					}
					if (listFloor2.speed != num)
					{
						listBPM.Add(new Tuple<double, double>(listFloor2.entryBeat, (double)ADOBase.conductor.bpm * (double)listFloor2.speed));
					}
				}
			}
		}
		if (gameworld)
		{
			double num2 = -100.0;
			recommendsTwoFingers = false;
			foreach (scrFloor listFloor3 in ADOBase.lm.listFloors)
			{
				double num3 = listFloor3.entryTime - num2;
				if (num3 > 0.001 && num3 <= 0.125)
				{
					numFastPresses++;
				}
				num2 = listFloor3.entryTime;
			}
			if (numFastPresses > 20 || (float)numFastPresses > (float)ADOBase.lm.listFloors.Count / 10f)
			{
				recommendsTwoFingers = true;
			}
			printe($"Level has {numFastPresses} fast presses (threshold based on tile count is {(float)ADOBase.lm.listFloors.Count / 10f}) Multi finger recommended: {recommendsTwoFingers}");
		}
		chosenplanet.FirstFloorAngleSetup();
		ADOBase.conductor.hasSongStarted = false;
		if (ADOBase.isLevelEditor)
		{
			hitTextContainer.SetActive(value: true);
		}
		ColorPlanets();
	}

	private void ColorPlanets()
	{
		GCS.staticPlanetColors = false;
		redPlanet.LoadPlanetColor();
		bluePlanet.LoadPlanetColor();
		for (int i = 0; i < planetList.Count; i++)
		{
			planetList[i].planetIndex = i;
			planetList[i].next = GetMultiPlanet(i, 1);
			planetList[i].prev = GetMultiPlanet(i, -1);
		}
		Color color = scrMisc.PlayerColorToRealColor(Persistence.GetPlayerColor(red: true));
		Color color2 = scrMisc.PlayerColorToRealColor(Persistence.GetPlayerColor(red: false));
		if (color == Color.red && color2 == Color.blue)
		{
			planetGreen.SetPlanetColor(new Color(0.3f, 0.7f, 0f, 1f));
			planetGreen.SetTailColor(new Color(0.3f, 0.7f, 0f, 1f));
			planetYellow.SetPlanetColor(new Color(1f, 0.8f, 0f, 1f));
			planetYellow.SetTailColor(new Color(1f, 0.8f, 0f, 1f));
			planetPurple.SetPlanetColor(new Color(0.7f, 0.1f, 1f, 1f));
			planetPurple.SetTailColor(new Color(0.7f, 0.1f, 1f, 1f));
			planetPink.SetPlanetColor(new Color(1f, 0.1f, 0.7f, 1f));
			planetPink.SetTailColor(new Color(1f, 0.1f, 0.7f, 1f));
			planetOrange.SetPlanetColor(new Color(1f, 0.4f, 0.1f, 1f));
			planetOrange.SetTailColor(new Color(1f, 0.4f, 0.1f, 1f));
			planetCyan.SetPlanetColor(new Color(0.1f, 0.8f, 0.9f, 1f));
			planetCyan.SetTailColor(new Color(0.1f, 0.8f, 0.9f, 1f));
		}
		else if (color == color2)
		{
			for (int j = 2; j <= planetsUsed - 1; j++)
			{
				if (Persistence.GetPlayerColor(red: true) != scrPlanet.goldColor)
				{
					allPlanets[j].SetPlanetColor(color);
					allPlanets[j].SetTailColor(color);
				}
				if (Persistence.GetPlayerColor(red: true) == scrPlanet.rainbowColor)
				{
					allPlanets[j].SetRainbow(enabled: true);
				}
				if (Persistence.GetPlayerColor(red: true) == scrPlanet.transPinkColor)
				{
					allPlanets[j].SetTailColor(Color.white);
				}
				if (Persistence.GetPlayerColor(red: true) == scrPlanet.transBlueColor)
				{
					allPlanets[j].SetTailColor(Color.white);
				}
				if (Persistence.GetPlayerColor(red: true) == scrPlanet.nbYellowColor)
				{
					allPlanets[j].SetTailColor(Color.white);
				}
				if (Persistence.GetPlayerColor(red: true) == scrPlanet.nbPurpleColor)
				{
					allPlanets[j].SetTailColor(Color.black);
				}
			}
		}
		else if ((Persistence.GetPlayerColor(red: true) == scrPlanet.transPinkColor && Persistence.GetPlayerColor(red: false) == scrPlanet.transBlueColor) || (Persistence.GetPlayerColor(red: false) == scrPlanet.transPinkColor && Persistence.GetPlayerColor(red: true) == scrPlanet.transBlueColor) || (Persistence.GetPlayerColor(red: true) == scrPlanet.nbYellowColor && Persistence.GetPlayerColor(red: false) == scrPlanet.nbPurpleColor) || (Persistence.GetPlayerColor(red: false) == scrPlanet.nbYellowColor && Persistence.GetPlayerColor(red: true) == scrPlanet.nbPurpleColor))
		{
			for (int k = 2; k <= planetsUsed - 1; k++)
			{
				allPlanets[k].SetPlanetColor(Color.white);
				allPlanets[k].SetTailColor(Color.white);
			}
		}
		else if (planetsUsed > 2)
		{
			Color.RGBToHSV(color, out float H, out float S, out float V);
			Color.RGBToHSV(color2, out float H2, out float S2, out float V2);
			Mathf.Max(Mathf.Abs(H2 - H), 1f - Mathf.Abs(H2 - H));
			float num = (1f - Mathf.Abs(H2 - H) > Mathf.Abs(H2 - H)) ? Mathf.Max(H, H2) : Mathf.Min(H, H2);
			float num2 = (1f - Mathf.Abs(H2 - H) > Mathf.Abs(H2 - H)) ? Mathf.Min(H, H2) : Mathf.Max(H, H2);
			float num3 = (num == H) ? S : S2;
			float num4 = (num == H) ? V : V2;
			float num5 = (num == H) ? S2 : S;
			float num6 = (num == H) ? V2 : V;
			if (num2 < num)
			{
				num2 += 1f;
			}
			for (int l = 2; l <= planetsUsed - 1; l++)
			{
				float num7 = (float)(l - 2 + 1) / (float)(planetsUsed - 1);
				float h = (num + (num2 - num) * num7) % 1f;
				float s = num3 + (num5 - num3) * num7;
				float v = num4 + (num6 - num4) * num7;
				allPlanets[l].SetPlanetColor(Color.HSVToRGB(h, s, v));
				allPlanets[l].SetTailColor(Color.HSVToRGB(h, s, v));
			}
		}
	}

	private void OnMusicScheduled()
	{
		if (GCS.checkpointNum != 0)
		{
			ADOBase.conductor.hasSongStarted = true;
			Scrub(GCS.checkpointNum, RDC.auto && ADOBase.isLevelEditor);
			ChangeState(States.Checkpoint);
		}
		else if (!GCS.d_oldConductor)
		{
			States states = (gameworld && !forceNoCountdown) ? States.Countdown : States.PlayerControl;
			ChangeState(states);
		}
		ADOBase.uiController.MinimizeDifficultyContainer();
		if (GCS.checkpointNum != 0)
		{
			scrDebugHUDMessage.Log("OnMusicStart");
			if (ADOBase.isLevelEditor && !GCS.standaloneLevelMode)
			{
				scrUIController.instance.FadeFromBlack();
			}
		}
		if (!gameworld)
		{
			scrUIController.instance.FadeFromBlack(0.3f);
		}
		popuptime = Mathf.Min(50f / (ADOBase.conductor.bpm * ADOBase.conductor.song.pitch), 0.5f);
		float b = popuptime;
		if (chosenplanet.currfloor != null && chosenplanet.currfloor.nextfloor != null)
		{
			b = (float)chosenplanet.currfloor.nextfloor.entryTimePitchAdj - (float)chosenplanet.currfloor.entryTimePitchAdj;
		}
		popuptime = Mathf.Min(popuptime, b);
		if (chosenplanet != null && currFloor != null)
		{
			DOTween.To(() => chosenplanet.cosmeticRadius, delegate(float x)
			{
				chosenplanet.cosmeticRadius = x;
			}, startRadius * currFloor.radiusScale, popuptime);
		}
		foreach (scrPlanet planet in planetList)
		{
			if (!planet.isChosen && currFloor != null)
			{
				planet.cosmeticRadius = startRadius * currFloor.radiusScale;
			}
		}
	}

	public void FollowMovingPlatform(scrFloor platform, bool goingDown)
	{
		chosenplanet.transform.position = platform.transform.position;
		responsive = false;
	}

	private void Update()
	{
		controllerUpdate = true;
		if (GCS.d_drumcontroller && ADOBase.controller.boothModeDebounceCounter > 0f)
		{
			ADOBase.controller.boothModeDebounceCounter -= Time.deltaTime;
		}
		moving = false;
		if ((ADOBase.isLevelSelect || ADOBase.isCLS) && currFloor != null && currFloor.tag == "MovingFloor")
		{
			if (currFloor.GetComponent<scrMenuMovingFloor>().moving)
			{
				moving = true;
				FollowMovingPlatform(currFloor, goingDown: true);
			}
			else
			{
				responsive = true;
			}
		}
		currentState = (States)(object)base.stateMachine.GetState();
		if ((debugModeCheatCode.CheckCheatCode() && Time.unscaledTime - debugTileTime < 8f) || (UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftControl) && UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Home)) || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.F2))
		{
			RDC.debug = !RDC.debug;
			printe("debug mode is: " + RDC.debug.ToString());
		}
		if (typingModeCheatCode.CheckCheatCode())
		{
			GCS.typingMode = !GCS.typingMode;
			scrFlash.Flash(Color.white);
			printe("typing mode is: " + GCS.typingMode.ToString());
		}
		if (RDInput.cancelPress && !paused)
		{
			if (GCS.lofiVersion && !gameworld && ADOBase.platform == Platform.Windows)
			{
				Application.Quit();
			}
			else if (ADOBase.cls == null || !ADOBase.cls.optionsPanels.justHidPanels)
			{
				scnCLS instance = scnCLS.instance;
				if (instance != null && instance.lastFrameSearchModeAvailable == Time.frameCount)
				{
					instance.ToggleSearchMode(search: false);
				}
				else
				{
					TogglePauseGame();
				}
			}
		}
		if (GCS.d_drumcontroller && gameworld && UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Tab))
		{
			disableCongratsMessage = true;
			OnLandOnPortal(-1, null);
			PortalTravelAction(portalDestination);
		}
		if (RDEditorUtils.CheckForKeyCombo(control: true, shift: true, UnityEngine.KeyCode.D))
		{
			numTimesSfxToggled++;
			int num = 25;
			if (numTimesSfxToggled != num)
			{
				GCS.playWilhelm = false;
				if (GCS.playDeathSound)
				{
					GCS.playDeathSound = false;
					scrSfx.instance.PlaySfx(SfxSound.OttoDeactivate);
				}
				else
				{
					GCS.playDeathSound = true;
					scrSfx.instance.PlaySfx(SfxSound.OttoActivate);
				}
			}
			else
			{
				GCS.playDeathSound = true;
				GCS.playWilhelm = true;
				scrSfx.instance.PlaySfx(SfxSound.Wilhelm);
			}
		}
		int num2 = currentFloorID;
		currentFloorID = Math.Max(currentFloorID, currFloor?.seqID ?? 0);
		if (currentFloorID > num2 && level != null)
		{
			level.Hit(currentFloorID);
		}
		bool holdingShift = RDInput.holdingShift;
		if ((RDC.debug || (ADOBase.isUnityEditor && !ADOBase.isLevelEditor)) & holdingShift)
		{
			if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.A))
			{
				RDC.auto = !RDC.auto;
			}
			else if ((UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftControl) || UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightControl)) && UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.H))
			{
				ClearAllAchievements();
			}
			else if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.H))
			{
				Persistence.GiveAchievements();
			}
			else if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.M))
			{
				GCS.d_hitsounds = !GCS.d_hitsounds;
				if (!GCS.d_hitsounds)
				{
					AudioManager.Instance.StopAllSounds();
				}
				scrDebugHUDMessage.LogBool(GCS.d_hitsounds, "Hit Sounds");
			}
			else if (gameworld && UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.N))
			{
				OnLandOnPortal(-1, null);
				PortalTravelAction(portalDestination);
				if (currentWorldString.IsTaro() && isbosslevel && !isPuzzleRoom)
				{
					int num3 = GCNS.dlcMedalsCount[currentWorldString];
					int[] array = new int[num3];
					for (int i = 0; i < num3; i++)
					{
						array[i] = 3;
					}
					Persistence.SetMedalsForDLCLevel(currentWorldString, array);
				}
			}
			else if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.R) && !GCS.lofiVersion)
			{
				if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftControl))
				{
					GCS.checkpointNum = 0;
				}
				Restart();
			}
			else if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.U) && !GCS.lofiVersion)
			{
				GCS.checkpointNum = GCS.customInternalUseCheckpoint;
				Restart();
			}
			else if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.Comma))
			{
				ScrubAdjacent(forward: false);
			}
			else if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.Period))
			{
				ScrubAdjacent(forward: true);
			}
			else if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Slash))
			{
				Scrub(customCheckpoint);
			}
			else if (gameworld && UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.P))
			{
				printe("killing all tweens");
				DOTween.KillAll();
				ADOBase.LoadScene(ADOBase.GetPreviousLevelName());
			}
			else if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Z))
			{
				GCS.d_stationary = !GCS.d_stationary;
				scrDebugHUDMessage.LogBool(GCS.d_stationary, "Stationary");
			}
			else if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Slash))
			{
				RDC.noHud = !RDC.noHud;
				scrDebugHUDMessage.LogBool(RDC.noHud, "No HUD");
			}
			else if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Backslash) && background != null)
			{
				background.SetActive(!background.activeSelf);
			}
		}
		if (currentState == States.PlayerControl)
		{
			UpdateLockInput();
			UpdateFreeroam();
		}
		if ((gameworld || (!gameworld && currFloor != null && currFloor.freeroamGenerated)) && !ADOBase.customLevel)
		{
			Analytics.OfficialLevelsTime += Time.unscaledDeltaTime;
		}
		if (GCS.standaloneLevelMode)
		{
			Analytics.customLevelsTime += Time.unscaledDeltaTime;
		}
		if (Time.unscaledTime > lastTimeStatsUploaded + 120f)
		{
			Analytics.UploadStatsToSteam();
			lastTimeStatsUploaded = Time.unscaledTime;
		}
	}

	public bool Hit()
	{
		if (!responsive)
		{
			return false;
		}
		if (ADOBase.isLevelEditor && ADOBase.controller.paused)
		{
			return false;
		}
		bool flag = chosenplanet.currfloor.nextfloor != null && chosenplanet.currfloor.nextfloor.auto;
		chosenplanet.cachedAngle = chosenplanet.angle;
		if ((bool)errorMeter && gameworld && Persistence.GetHitErrorMeterSize() != 0)
		{
			float num = (float)(chosenplanet.cachedAngle - chosenplanet.targetExitAngle);
			if (!isCW)
			{
				num *= -1f;
			}
			if (!midspinInfiniteMargin)
			{
				if ((RDC.auto | flag) && !RDC.useOldAuto)
				{
					errorMeter.AddHit(0f);
				}
				else
				{
					errorMeter.AddHit(num, (float)currFloor.nextfloor.marginScale);
				}
			}
		}
		scrMisc.Vibrate(50L);
		chosenplanet.next.ChangeFace(pulse: true);
		scrPlanet x = chosenplanet;
		chosenplanet = chosenplanet.SwitchChosen();
		bool result = x != chosenplanet;
		if (ADOBase.playerIsOnIntroScene)
		{
			return result;
		}
		bool flag2 = chosenplanet.currfloor.holdLength == -1 || (chosenplanet.currfloor.holdLength > -1 && lastCamPulseFloor < chosenplanet.currfloor.seqID);
		lastCamPulseFloor = chosenplanet.currfloor.seqID;
		if (camy.followMode && flag2)
		{
			camy.frompos = camy.transform.localPosition;
			camy.topos = new Vector3(chosenplanet.transform.position.x, chosenplanet.transform.position.y, camy.transform.position.z);
			camy.timer = 0f;
		}
		if (camy.isPulsingOnHit && flag2)
		{
			camy.Pulse();
		}
		if (ADOBase.isEditingLevel)
		{
			bool flag3 = true;
			if (currFloor.midSpin || (currFloor.seqID > 0 && ADOBase.lm.listFloors[currFloor.seqID - 1].holdLength > -1))
			{
				flag3 = false;
			}
			if (currFloor.seqID > 1 && ADOBase.lm.listFloors[currFloor.seqID - 1].midSpin && ADOBase.lm.listFloors[currFloor.seqID - 2].holdLength > -1)
			{
				flag3 = false;
			}
			if (flag3)
			{
				scnEditor.instance.OttoBlink();
			}
		}
		if (currFloor.midSpin)
		{
			int count = planetList.Count;
			midspinInfiniteMargin = true;
			keyTimes.Add(Time.timeAsDouble);
		}
		else
		{
			midspinInfiniteMargin = false;
		}
		chosenplanet.Update_RefreshAngles();
		return result;
	}

	public void ClearMisses()
	{
		foreach (scrMissIndicator item in missesOnCurrFloor)
		{
			item.FadeOut();
		}
		missesOnCurrFloor.Clear();
	}

	public void OnLandOnPortal(int portalDestination, string portalArguments)
	{
		DOVirtual.DelayedCall(3f, delegate
		{
			RDUtils.SetGarbageCollectionEnabled(enabled: true);
		});
		bool flag = false;
		if (GCS.d_newgrounds)
		{
			scrNewgroundsAPIManager.StaticCheckMedals();
		}
		this.portalArguments = portalArguments;
		this.portalDestination = portalDestination;
		scrFlash.Flash(Color.white.WithAlpha(0.4f));
		if (GCS.practiceMode)
		{
			DOTween.KillAll();
			ADOBase.conductor.song.DOFade(0f, 1f).OnKill(delegate
			{
				ADOBase.conductor.song.Stop();
			});
		}
		if (levelNameShouldHide)
		{
			LevelNameTextRestore();
		}
		if (ADOBase.sceneName == "TP-X")
		{
			txtCongrats = scrUIController.instance.txtCongrats;
			txtCongrats.gameObject.SetActive(value: true);
			txtCongrats.text = RDString.Get("status.congratulations");
			scrSfx.instance.PlaySfx(SfxSound.Applause);
		}
		if (gameworld || (!gameworld && !isPuzzleRoom && currFloor.freeroamGenerated))
		{
			bool flag2 = mistakesManager.IsAllPurePerfect();
			bool flag3 = false;
			if (GCS.practiceMode)
			{
				txtCongrats.text = string.Empty;
			}
			else if (ADOBase.isLevelEditor)
			{
				string key = flag2 ? "status.allPurePerfect" : "status.congratulations";
				txtCongrats.text = RDString.Get(key);
				if (GCS.standaloneLevelMode)
				{
					mistakesManager.SaveCustom(ADOBase.customLevel.levelData.Hash, wonLevel: true, GCS.currentSpeedTrial);
				}
				flag = flag2;
			}
			else if (isbosslevel)
			{
				mistakesManager.CalculatePercentAcc();
				SystemLanguage language = RDString.language;
				string text = null;
				string text2 = null;
				if (currentWorldString == "7" && flag2)
				{
					text = RDString.Get("status.world7Purrfect");
				}
				else if (currentWorldString == "11" && !flag2)
				{
					text2 = RDString.Get("status.world11Congratulations");
				}
				string text3 = flag2 ? text : text2;
				if (text3 == null || text3.Contains("[Don't translate]"))
				{
					text3 = RDString.Get(flag2 ? "status.allPurePerfect" : "status.congratulations");
				}
				txtCongrats.text = text3;
				flag = flag2;
				if (!GCS.practiceMode && !GCS.d_booth)
				{
					endLevelType = mistakesManager.Save(scrController.currentWorld, wonLevel: true, GCS.currentSpeedTrial);
				}
				else
				{
					endLevelType = EndLevelType.WinInPracticeMode;
				}
				if (GCS.speedTrialMode)
				{
					GCS.nextSpeedRun = GCS.currentSpeedTrial + 0.1f;
				}
				if (saveProgressConditions)
				{
					Persistence.DeleteSavedProgress();
				}
			}
			else
			{
				flag3 = true;
				printe("levelName: " + levelName);
				string text4 = levelName;
				int index = text4.Length - 1;
				int num = int.Parse(text4[index].ToString());
				int currentWorld = scrController.currentWorld;
				if (Persistence.GetLevelTutorialProgress(currentWorld) < num)
				{
					Persistence.SetLevelTutorialProgress(currentWorld, num);
				}
			}
			if (disableCongratsMessage | flag3)
			{
				txtCongrats.text = "";
			}
			ADOBase.controller.txtCongrats.gameObject.SetActive(value: true);
			if (!isPuzzleRoom && showDetailedResults && !flag3)
			{
				_003C_003Ec__DisplayClass197_0 _003C_003Ec__DisplayClass197_ = default(_003C_003Ec__DisplayClass197_0);
				_003C_003Ec__DisplayClass197_.isPurePerfect = mistakesManager.IsAllPurePerfect();
				int resultCount = _003COnLandOnPortal_003Eg__GetHits_007C197_6(HitMargin.Perfect);
				int resultCount2 = _003COnLandOnPortal_003Eg__GetHits_007C197_6(HitMargin.EarlyPerfect);
				int resultCount3 = _003COnLandOnPortal_003Eg__GetHits_007C197_6(HitMargin.LatePerfect);
				int resultCount4 = _003COnLandOnPortal_003Eg__GetHits_007C197_6(HitMargin.VeryEarly);
				int resultCount5 = _003COnLandOnPortal_003Eg__GetHits_007C197_6(HitMargin.VeryLate);
				int resultCount6 = _003COnLandOnPortal_003Eg__GetHits_007C197_6(HitMargin.TooEarly);
				int resultCount7 = _003COnLandOnPortal_003Eg__GetHits_007C197_6(HitMargin.FailMiss);
				int resultCount8 = _003COnLandOnPortal_003Eg__GetHits_007C197_6(HitMargin.FailOverload);
				int num2 = _003COnLandOnPortal_003Eg__GetHits_007C197_6(HitMargin.Auto);
				float num3 = Persistence.GetShowXAccuracy() ? mistakesManager.percentXAcc : mistakesManager.percentAcc;
				ColourSchemeHitMargin hitMarginColoursUI = RDConstants.data.hitMarginColoursUI;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(_003COnLandOnPortal_003Eg__Localized_007C197_2("ePerfect")).Append(_003COnLandOnPortal_003Eg__Result_007C197_3(resultCount2, hitMarginColoursUI.colourLittleEarly.ToHex())).Append("     ");
				if (num2 > 0)
				{
					stringBuilder.Append(_003COnLandOnPortal_003Eg__Localized_007C197_2("perfect")).Append(_003COnLandOnPortal_003Eg__ResultWithAuto_007C197_4(resultCount, num2, hitMarginColoursUI.colourPerfect.ToHex())).Append("     ");
				}
				else
				{
					stringBuilder.Append(_003COnLandOnPortal_003Eg__Localized_007C197_2("perfect")).Append(_003COnLandOnPortal_003Eg__Result_007C197_3(resultCount, hitMarginColoursUI.colourPerfect.ToHex())).Append("     ");
				}
				stringBuilder.Append(_003COnLandOnPortal_003Eg__Localized_007C197_2("lPerfect")).Append(_003COnLandOnPortal_003Eg__Result_007C197_3(resultCount3, hitMarginColoursUI.colourLittleLate.ToHex())).Append("\n");
				stringBuilder.Append(_003COnLandOnPortal_003Eg__Localized_007C197_2("tooEarly")).Append(_003COnLandOnPortal_003Eg__Result_007C197_3(resultCount6, hitMarginColoursUI.colourTooEarly.ToHex())).Append("     ");
				stringBuilder.Append(_003COnLandOnPortal_003Eg__Localized_007C197_2("early")).Append(_003COnLandOnPortal_003Eg__Result_007C197_3(resultCount4, hitMarginColoursUI.colourVeryEarly.ToHex())).Append("     ");
				stringBuilder.Append(_003COnLandOnPortal_003Eg__Localized_007C197_2("late")).Append(_003COnLandOnPortal_003Eg__Result_007C197_3(resultCount5, hitMarginColoursUI.colourVeryLate.ToHex())).Append("\n");
				if (ADOBase.controller.noFail || ADOBase.controller.safetyTilesArePresent)
				{
					stringBuilder.Append(_003COnLandOnPortal_003Eg__Localized_007C197_2("missFails")).Append(_003COnLandOnPortal_003Eg__Result_007C197_3(resultCount7, hitMarginColoursUI.colourFail.ToHex())).Append("     ");
					stringBuilder.Append(_003COnLandOnPortal_003Eg__Localized_007C197_2("overloadFails")).Append(_003COnLandOnPortal_003Eg__Result_007C197_3(resultCount8, hitMarginColoursUI.colourFail.ToHex())).Append("\n");
				}
				stringBuilder.Append(_003COnLandOnPortal_003Eg__Localized_007C197_2(Persistence.GetShowXAccuracy() ? "xAccuracy" : "accuracy")).Append(_003COnLandOnPortal_003Eg__GoldAccuracy_007C197_5($"{num3 * 100f:0.00}%", ref _003C_003Ec__DisplayClass197_)).Append("     ");
				stringBuilder.Append(_003COnLandOnPortal_003Eg__Localized_007C197_2("checkpoints")).Append(checkpointsUsed.ToString()).Append("\n");
				ADOBase.controller.txtResults.text = stringBuilder.ToString();
				ADOBase.controller.txtResults.gameObject.SetActive(value: true);
			}
		}
		if (!RDC.auto && flag)
		{
			scrSfx.instance.PlaySfx(SfxSound.PurePerfect);
		}
		if (gameworld && scrMistakesManager.hardestDifficulty == Difficulty.Strict && txtAllStrictClear != null)
		{
			txtAllStrictClear.text = RDString.Get("status.allStrictClear");
			txtAllStrictClear.gameObject.SetActive(value: true);
		}
		if (!ADOBase.isEditingLevel)
		{
			winTime = Time.unscaledTime;
			ChangeState(States.Won);
		}
	}

	public void PortalTravelAction(int destination)
	{
		if (transitioningLevel)
		{
			return;
		}
		portalDestination = destination;
		bool flag = false;
		WipeDirection wipeDirection = WipeDirection.StartsFromRight;
		switch (portalDestination)
		{
		case -1:
			if (currentWorldString == "T4" && isbosslevel && Persistence.GetTaroStoryProgress() < 4)
			{
				Persistence.SetTaroStoryProgress(4);
				Persistence.Save();
			}
			if (currentWorldString == "T5" && isbosslevel && Persistence.GetTaroStoryProgress() < 6)
			{
				Persistence.SetTaroStoryProgress(6);
				Persistence.Save();
			}
			if (GCS.standaloneLevelMode)
			{
				if (GCS.speedTrialMode || GCS.practiceMode)
				{
					if (GCS.speedTrialMode)
					{
						GCS.nextSpeedRun = GCS.currentSpeedTrial + 0.1f;
					}
					printe($"incremented speed trial to {GCS.nextSpeedRun}");
					StartCoroutine(ResetCustomLevel());
				}
				else if (GCS.customLevelIndex >= GCS.customLevelPaths.Length - 1)
				{
					QuitToMainMenu();
					flag = true;
				}
				else
				{
					GCS.customLevelIndex++;
					StartLoadingScene(WipeDirection.StartsFromRight);
				}
			}
			else if (GCS.speedTrialMode || GCS.practiceMode)
			{
				if (endLevelType == EndLevelType.FirstWinSpeedTrial)
				{
					QuitToMainMenu();
					flag = true;
					break;
				}
				if (GCS.speedTrialMode)
				{
					GCS.nextSpeedRun = GCS.currentSpeedTrial + 0.1f;
				}
				GCS.sceneToLoad = levelName;
			}
			else if (isbosslevel)
			{
				if (currentWorldString == "6" && endLevelType == EndLevelType.FirstWin)
				{
					GCS.worldEntrance = null;
				}
				QuitToMainMenu();
				flag = true;
			}
			else
			{
				GCS.sceneToLoad = ADOBase.GetNextLevelName();
			}
			break;
		case -2:
			GCS.sceneToLoad = Persistence.GetSavedCurrentLevel();
			if (!ADOBase.ownsTaroDLC && GCS.sceneToLoad.IsTaro())
			{
				GCS.sceneToLoad = "1-1";
			}
			break;
		case -3:
			GCS.sceneToLoad = "scnCalibration";
			break;
		case -4:
			if (!GCS.standaloneLevelMode)
			{
				GCS.speedTrialMode = false;
				GCS.practiceMode = false;
			}
			GCS.sceneToLoad = "scnEditor";
			GCS.worldEntrance = null;
			SteamIntegration.EditorEntered();
			break;
		case -60:
			GCS.sceneToLoad = GetTaroMenuToGoTo();
			break;
		case -573:
			GCS.sceneToLoad = "TP-Test";
			break;
		case -574:
			GCS.sceneToLoad = "TP-1";
			break;
		case -575:
			GCS.sceneToLoad = "TP-2";
			break;
		case -576:
			GCS.sceneToLoad = "TP-X";
			break;
		case -577:
			Persistence.SetTaroStoryProgress(5);
			Persistence.Save();
			GCS.sceneToLoad = "scnTaroMenu3";
			break;
		case -59:
			QuitToMainMenu();
			break;
		case -5:
			GCS.speedTrialMode = false;
			GCS.practiceMode = false;
			GCS.currentSpeedTrial = 1f;
			GCS.nextSpeedRun = 1f;
			printe("we reset speed trial value...");
			GCS.sceneToLoad = "scnCLS";
			GCS.worldEntrance = null;
			SteamIntegration.IncrementCLSEnteredStat();
			break;
		case -77777:
			SteamFriends.ActivateGameOverlayToWebPage("https://store.steampowered.com/app/774181/Rhythm_Doctor/");
			GCS.sceneToLoad = GCNS.sceneLevelSelect;
			break;
		case -10:
			wipeDirection = WipeDirection.StartsFromLeft;
			GCS.sceneToLoad = ADOBase.GetPreviousLevelName();
			break;
		case -11:
			GCS.sceneToLoad = ADOBase.GetNextLevelName();
			break;
		case -12:
			wipeDirection = WipeDirection.StartsFromLeft;
			GCS.nextSpeedRun = GCS.currentSpeedTrial - 0.1f;
			GCS.sceneToLoad = levelName;
			break;
		case -13:
			GCS.nextSpeedRun = GCS.currentSpeedTrial + 0.1f;
			GCS.sceneToLoad = levelName;
			break;
		case -14:
			GCS.speedTrialMode = false;
			GCS.practiceMode = false;
			GCS.nextSpeedRun = 1f;
			GCS.standaloneLevelMode = false;
			if (portalArguments == "?")
			{
				GCS.sceneToLoad = "oldtrumpet-X";
			}
			else
			{
				GCS.sceneToLoad = portalArguments;
			}
			break;
		case -15:
			GCS.speedTrialMode = true;
			GCS.practiceMode = false;
			GCS.standaloneLevelMode = false;
			if (portalArguments == "XO-X" || portalArguments == "T3-X" || portalArguments == "T3EX-X" || portalArguments == "T5-X")
			{
				GCS.nextSpeedRun = 1f;
			}
			else
			{
				GCS.nextSpeedRun = 1.1f;
			}
			GCS.sceneToLoad = portalArguments;
			break;
		case -16:
		{
			GCS.speedTrialMode = false;
			GCS.practiceMode = false;
			GCS.nextSpeedRun = 1f;
			string key = portalArguments;
			int worldAttempts = Persistence.GetWorldAttempts(ADOBase.worldData[key].index);
			int levelCount = ADOBase.worldData[key].levelCount;
			GCS.sceneToLoad = ((!GCS.d_booth && (worldAttempts > 0 || levelCount == 1)) ? (portalArguments + "-X") : (portalArguments + "-1"));
			break;
		}
		}
		if (!flag)
		{
			StartLoadingScene(wipeDirection);
		}
		transitioningLevel = true;
	}

	public void StartLoadingScene(WipeDirection wipeDirection)
	{
		RDUtils.SetGarbageCollectionEnabled(enabled: true);
		if (!GCS.lofiVersion)
		{
			scrUIController.instance.WipeToBlack(wipeDirection);
		}
		else
		{
			printe("killing all tweens");
			DOTween.KillAll();
			printe("trying to load scene name");
		}
		deaths = 0;
	}

	public string GetTaroMenuToGoTo()
	{
		int taroStoryProgress = Persistence.GetTaroStoryProgress();
		string text = (taroStoryProgress < 3) ? "scnTaroMenu1" : ((taroStoryProgress < 5) ? "scnTaroMenu2" : ((taroStoryProgress < 6) ? "scnTaroMenu3" : "scnTaroMenu0"));
		if (currentWorldString == "T4" && taroStoryProgress == 4)
		{
			text = "TP-1";
		}
		if (ADOBase.sceneName == "TP-X" && taroStoryProgress == 4)
		{
			text = "scnTaroMenu3";
		}
		printe($"dlcProgress: {taroStoryProgress} -> {text}");
		return text;
	}

	public void GoToSpecificLevel(string levelName, bool speedTrial = false)
	{
		portalArguments = levelName;
		PortalTravelAction(speedTrial ? (-15) : (-14));
	}

	public void GoToNextLevel()
	{
		PortalTravelAction(GCS.speedTrialMode ? (-13) : (-11));
	}

	public void GoToPrevLevel()
	{
		PortalTravelAction(GCS.speedTrialMode ? (-12) : (-10));
	}

	public bool OnDamage(bool multipress = false, bool applyMultipressDamage = false, bool skipDamage = false)
	{
		bool flag = false;
		double bpm = (double)ADOBase.conductor.bpm * ADOBase.controller.speed * (double)ADOBase.conductor.song.pitch;
		double num = scrMisc.AngleToTime(scrMisc.GetAngleMoved(currFloor.entryangle, currFloor.exitangle, !currFloor.isCCW), bpm);
		if (multipress && !applyMultipressDamage && num > 0.019999999552965164)
		{
			consecMultipressCounter++;
		}
		if (consecMultipressCounter > 8)
		{
			consecMultipressCounter = 4;
			FailAction(overload: true, multipress: true);
			flag = true;
		}
		if (!multipress || (multipress && applyMultipressDamage))
		{
			if (!skipDamage)
			{
				flag |= failbar.Damage(multipress);
			}
			if (gameworld && (!ADOBase.controller.noFail || !flag) && !currFloor.hideJudgment)
			{
				missesOnCurrFloor.Add(chosenplanet.MarkMiss());
			}
		}
		return flag;
	}

	public void ApplyDamage(float dmg)
	{
		failbar.Damage(dmg);
	}

	public void QuitToMainMenu()
	{
		RDUtils.SetGarbageCollectionEnabled(enabled: true);
		ADOBase.audioManager.StopLoadingMP3File();
		if (GCS.webVersion)
		{
			ADOBase.LoadScene("scnIntro");
		}
		else
		{
			exitingToMainMenu = true;
			scrUIController.instance.WipeToBlack(WipeDirection.StartsFromRight);
			if (GCS.customLevelPaths == null)
			{
				string text = currentWorldString;
				if (text != null && text.IsTaro() && !base.gameObject.scene.name.StartsWith("scnTaroMenu"))
				{
					GCS.sceneToLoad = GetTaroMenuToGoTo();
				}
				else
				{
					GCS.sceneToLoad = GCNS.sceneLevelSelect;
				}
			}
			else
			{
				GCS.sceneToLoad = "scnCLS";
			}
			GCS.standaloneLevelMode = false;
		}
		deaths = 0;
		GCS.currentSpeedTrial = 1f;
	}

	public int FindScrubStart(int floorNum, bool forceDontStartMusicFourTilesBefore = false)
	{
		int result = floorNum;
		if (!forceDontStartMusicFourTilesBefore)
		{
			double num = ADOBase.conductor.crotchetAtStart / (double)ADOBase.lm.listFloors[floorNum].speed;
			for (int num2 = floorNum - 1; num2 >= 1; num2--)
			{
				if (ADOBase.lm.listFloors[num2].entryTime <= ADOBase.lm.listFloors[floorNum].entryTime - (double)ADOBase.conductor.adjustedCountdownTicks * num)
				{
					result = num2;
					break;
				}
			}
			if (ADOBase.lm.listFloors[1].entryTime > ADOBase.lm.listFloors[floorNum].entryTime - (double)ADOBase.conductor.adjustedCountdownTicks * num)
			{
				result = 1;
			}
		}
		return result;
	}

	public void Scrub(int floorNum, bool forceDontStartMusicFourTilesBefore = false)
	{
		if (floorNum > scrLevelMaker.instance.listFloors.Count - 1 || floorNum < 0)
		{
			scrDebugHUDMessage.Log("Past the limit");
			return;
		}
		if (scrLevelMaker.instance.listFloors[floorNum].midSpin && (bool)scrLevelMaker.instance.listFloors[floorNum].nextfloor && scrLevelMaker.instance.listFloors[floorNum].numPlanets > 2)
		{
			floorNum++;
		}
		while (floorNum > 1 && scrLevelMaker.instance.listFloors[floorNum].freeroam)
		{
			floorNum--;
		}
		curFreeRoamSection = 0;
		for (int i = 0; i < scrLevelMaker.instance.listFreeroamStartTiles.Count; i++)
		{
			if (floorNum > scrLevelMaker.instance.listFreeroamStartTiles[i].seqID)
			{
				curFreeRoamSection++;
			}
		}
		scrubchosen = planetList[0];
		SetNumPlanets(2, whileScrubbing: true, 0);
		int num = 2;
		for (int j = 1; j <= floorNum; j++)
		{
			scrFloor scrFloor = scrLevelMaker.instance.listFloors[j];
			if (scrFloor.numPlanets != num)
			{
				num = scrFloor.numPlanets;
				SetNumPlanets(num, whileScrubbing: true, j);
			}
			if (scrFloor.midSpin)
			{
				scrubchosen = scrubchosen.prev;
			}
			else
			{
				scrubchosen = scrubchosen.next;
			}
		}
		chosenplanet = scrubchosen;
		double num2 = ADOBase.conductor.crotchetAtStart / (double)ADOBase.lm.listFloors[floorNum].speed;
		double num3 = Math.Max(ADOBase.lm.listFloors[floorNum].entryTime - num2 * (double)((!forceDontStartMusicFourTilesBefore) ? 4 : 0), ADOBase.conductor.separateCountdownTime ? (ADOBase.conductor.crotchetAtStart * (double)ADOBase.conductor.adjustedCountdownTicks) : 0.0);
		chosenplanet.ScrubToFloorNumber(floorNum, (float)(ADOBase.lm.listFloors[floorNum].entryTime - num3) / ADOBase.conductor.song.pitch, ADOBase.isLevelEditor || RDC.debug);
		if (RDC.debug)
		{
			camy.ViewObjectInstant(chosenplanet.transform);
		}
		ADOBase.conductor.ScrubMusicToTime(num3);
		GameObject gameObject = GameObject.Find("Vfx");
		if (!(gameObject == null))
		{
			scrVfxPlus component = gameObject.GetComponent<scrVfxPlus>();
			if (!(component == null) && ADOBase.isLevelEditor)
			{
				component.ScrubToTime((float)num3);
			}
		}
	}

	public void ScrubAdjacent(bool forward)
	{
		int floorNum = currFloor.seqID + (forward ? 1 : (-1));
		Scrub(floorNum, forceDontStartMusicFourTilesBefore: true);
	}

	public bool TogglePauseGame()
	{
		AudioManager.pauseGameSounds = false;
		AsyncInputManager.offsetTickUpdated = false;
		if (GCS.standaloneLevelMode && (Time.frameCount - frameStart < 4 || (ADOBase.customLevel != null && ADOBase.customLevel.isLoading)))
		{
			return paused;
		}
		if (!ADOBase.isEditingLevel && scrUIController.instance.transitionPanel.gameObject.activeSelf)
		{
			return paused;
		}
		if (GCS.d_boothDisablePossibleMessUpButtons || GCS.webVersion)
		{
			QuitToMainMenu();
		}
		paused = !paused;
		audioPaused = paused;
		base.enabled = !paused;
		Time.timeScale = (paused ? 0f : 1f);
		if (scnEditor.instance == null || GCS.standaloneLevelMode)
		{
			if (paused)
			{
				if (!GCS.lofiVersion || ADOBase.isMobile)
				{
					takeScreenshot.ShowPauseMenu(goToSettings: false);
				}
			}
			else
			{
				CheckForAudioOutputChange();
				pauseMenu.Hide();
			}
		}
		return paused;
	}

	public void Restart(bool fromBeginning = false)
	{
		RDUtils.SetGarbageCollectionEnabled(enabled: true);
		if (fromBeginning)
		{
			GCS.checkpointNum = 0;
			Persistence.DeleteSavedProgress();
		}
		GCS.sceneToLoad = levelName;
		scrUIController.instance.WipeToBlack(WipeDirection.StartsFromRight);
	}

	public void SetPracticeMode(bool practice)
	{
		GCS.practiceMode = practice;
		if (practice)
		{
			GCS.practiceLength = 20;
			GCS.checkpointBeforePractice = GCS.checkpointNum;
			GCS.checkpointNum = Math.Max(0, currentSeqID - GCS.practiceLength / 2);
			if (GCS.speedTrialMode)
			{
				GCS.speedTrialModeBeforePractice = GCS.speedTrialMode;
				GCS.speedRunBeforePractice = GCS.currentSpeedTrial;
			}
			GCS.currentSpeedTrial = 0.9f;
			GCS.nextSpeedRun = 0.9f;
			GCS.speedTrialMode = false;
		}
		else
		{
			GCS.checkpointNum = GCS.checkpointBeforePractice;
			if (GCS.speedTrialModeBeforePractice)
			{
				GCS.nextSpeedRun = GCS.speedRunBeforePractice;
				GCS.speedTrialMode = true;
			}
			else
			{
				GCS.nextSpeedRun = 1f;
			}
		}
		if (scnEditor.instance != null)
		{
			StartCoroutine(ResetCustomLevel());
		}
		else
		{
			Restart();
		}
	}

	public void SkipLevel()
	{
		PortalTravelAction(portalDestination);
	}

	public bool ValidInputWasTriggered()
	{
		if (exitingToMainMenu)
		{
			return false;
		}
		bool flag = false;
		if (ADOBase.isMobile)
		{
			Touch[] touches = Input.touches;
			for (int i = 0; i < touches.Length; i++)
			{
				Touch touch = touches[i];
				if (touch.phase == TouchPhase.Began && !IsScreenPointInsideUIElements(touch.position))
				{
					flag = true;
					break;
				}
			}
		}
		bool flag2;
		if (ADOBase.isMobile)
		{
			flag2 = ((Input.anyKeyDown && !Input.GetKeyDown(UnityEngine.KeyCode.Mouse0) && !Input.GetKeyDown(UnityEngine.KeyCode.Mouse1)) | flag);
		}
		else
		{
			_003C_003Ec__DisplayClass216_0 _003C_003Ec__DisplayClass216_ = default(_003C_003Ec__DisplayClass216_0);
			_003C_003Ec__DisplayClass216_.mouseOverAButton = false;
			if (_003CValidInputWasTriggered_003Eg__GetMouseDown_007C216_0())
			{
				_003C_003Ec__DisplayClass216_.mouseOverAButton = EventSystem.current.IsPointerOverGameObject();
			}
			if (isCutscene)
			{
				_003C_003Ec__DisplayClass216_.mouseOverAButton = false;
			}
			flag2 = _003CValidInputWasTriggered_003Eg__GetInputHappened_007C216_1(ref _003C_003Ec__DisplayClass216_);
		}
		if (!flag2)
		{
			return false;
		}
		return CountValidKeysPressed() > 0;
	}

	public bool ValidInputWasReleased()
	{
		bool flag = false;
		if (ADOBase.isMobile)
		{
			if (holdKeys.Count != 0 && UnityEngine.Input.touchCount == 0 && !Input.anyKey)
			{
				holdKeys.Clear();
				return true;
			}
		}
		else
		{
			bool holding = this.holding;
			int count = holdKeys.Count;
			if (holding && count == 0)
			{
				RDBaseDll.printem("stopped holding");
				return true;
			}
			List<object> mainHeldKeys = RDInput.GetMainHeldKeys();
			for (int num = holdKeys.Count - 1; num >= 0; num--)
			{
				object obj = holdKeys[num];
				if (obj == null)
				{
					holdKeys.RemoveAt(num);
				}
				else if (mainHeldKeys.IndexOf(obj) == -1)
				{
					holdKeys.RemoveAt(num);
					flag = true;
				}
			}
		}
		if (!flag)
		{
			return validKeyWasReleased;
		}
		return true;
	}

	private bool IsScreenPointInsideUIElements(Vector2 position)
	{
		float num = (float)Screen.height / 1000f;
		if (currentState != States.PlayerControl && ADOBase.uiController.difficultyUIMode != 0)
		{
			float num2 = num * 260f;
			float num3 = num * 555f;
			if (position.x > (float)Screen.width - num3 && position.y < num2)
			{
				return true;
			}
		}
		float num4 = num * 200f;
		float num5 = num4;
		if (position.x > (float)Screen.width - num5 && position.y > (float)Screen.height - num4)
		{
			return true;
		}
		return false;
	}

	public int CountValidKeysPressed()
	{
		int num = 0;
		if (ADOBase.isMobile)
		{
			Touch[] touches = Input.touches;
			for (int i = 0; i < touches.Length; i++)
			{
				Touch touch = touches[i];
				if (touch.phase == TouchPhase.Began && !IsScreenPointInsideUIElements(touch.position))
				{
					num++;
				}
			}
		}
		num += RDInput.mainPressCount;
		if (currentState == States.PlayerControl)
		{
			foreach (object mainPressKey in RDInput.GetMainPressKeys())
			{
				if (mainPressKey is UnityEngine.KeyCode)
				{
					UnityEngine.KeyCode keyCode = (UnityEngine.KeyCode)mainPressKey;
					keyFrequency[keyCode] = (keyFrequency.ContainsKey(keyCode) ? (keyFrequency[keyCode] + 1) : 0);
					keyTotal++;
				}
				if (mainPressKey is SharpHook.Native.KeyCode)
				{
					SharpHook.Native.KeyCode keyCode2 = (SharpHook.Native.KeyCode)mainPressKey;
					keyFrequency[keyCode2] = (keyFrequency.ContainsKey(keyCode2) ? (keyFrequency[keyCode2] + 1) : 0);
					keyTotal++;
				}
			}
		}
		return Math.Max(0, num);
	}

	private void IterateValidKeysHeld(Action<MutualKeyCode> foundHeld, Action<MutualKeyCode> foundSpecial, bool onlyPressedKeys = false)
	{
		if (ADOBase.isMobile)
		{
			Touch[] touches = Input.touches;
			for (int i = 0; i < touches.Length; i++)
			{
				Touch touch = touches[i];
				if (!IsScreenPointInsideUIElements(touch.position))
				{
					foundHeld(new MutualKeyCode(null));
				}
			}
		}
		foreach (object item in onlyPressedKeys ? RDInput.GetMainPressKeys() : RDInput.GetMainHeldKeys())
		{
			bool flag = AsyncInputManager.isActive && item is ushort;
			foundHeld(new MutualKeyCode(flag, flag ? ((object)(ushort)item) : item));
		}
	}

	private void LateUpdate()
	{
		float num = (float)volume * 0.1f;
		AudioListener.volume = 0.5f * num * num;
		ADOBase.uiController.mutedImage.gameObject.SetActive(volume == 0);
	}

	public void ChangeToStartState()
	{
		ChangeState(States.Start);
	}

	public bool IsPercentCompleteBest()
	{
		return percentComplete > oldPercentComplete;
	}

	public void ShowHitText(HitMargin hitMargin, Vector3 position, float angle)
	{
		scrHitTextMesh[] array = cachedHitTexts[hitMargin];
		int num = 0;
		scrHitTextMesh scrHitTextMesh;
		while (true)
		{
			if (num < array.Length)
			{
				scrHitTextMesh = array[num];
				if (scrHitTextMesh.dead)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		scrHitTextMesh.Show(position, angle);
	}

	private void OnApplicationQuit()
	{
		SteamIntegration.Instance.CloseConnection();
	}

	public void ClearAllAchievements()
	{
		SteamIntegration.Instance.ClearAllAchievements();
	}

	public void MakeNewFontDictionary()
	{
		RDString.Setup();
		nameToFont = new Dictionary<FontName, Font>();
		nameToFont.Add(FontName.Default, RDString.fontData.font);
		nameToFont.Add(FontName.Arial, RDConstants.data.arialFont);
		nameToFont.Add(FontName.ComicSansMS, RDConstants.data.comicSansMSFont);
		nameToFont.Add(FontName.CourierNew, RDConstants.data.courierNewFont);
		nameToFont.Add(FontName.Georgia, RDConstants.data.georgiaFont);
		nameToFont.Add(FontName.Impact, RDConstants.data.impactFont);
		nameToFont.Add(FontName.TimesNewRoman, RDConstants.data.timesNewRomanFont);
	}

	public void DebugTileEnter()
	{
		printe("debug tile enter");
		debugTileTime = Time.unscaledTime;
	}

	public scrPlanet GetMultiPlanet(int index, int dir)
	{
		return planetList[scrMisc.ModInt(index + dir, planetList.Count)];
	}

	public void ResetNumPlanets()
	{
		planetList.Clear();
		availablePlanets.Clear();
		planetList.Add(redPlanet);
		planetList.Add(bluePlanet);
		availablePlanets.Add(planetGreen);
		availablePlanets.Add(planetYellow);
		availablePlanets.Add(planetPurple);
		availablePlanets.Add(planetPink);
		availablePlanets.Add(planetOrange);
		availablePlanets.Add(planetCyan);
		for (int i = 0; i < planetList.Count; i++)
		{
			planetList[i].planetIndex = i;
			planetList[i].next = GetMultiPlanet(i, 1);
			planetList[i].prev = GetMultiPlanet(i, -1);
		}
		for (int j = 0; j < availablePlanets.Count; j++)
		{
			availablePlanets[j].transform.position = planetList[0].transform.position;
			availablePlanets[j].Destroy();
		}
	}

	public void SetNumPlanets(int numPlanets, bool whileScrubbing = false, int scrubbingfloor = -1)
	{
		int count = planetList.Count;
		if (numPlanets < 2 || numPlanets > 8)
		{
			return;
		}
		if (numPlanets < count)
		{
			for (int i = 0; i < planetList.Count - numPlanets; i++)
			{
				int planetIndex = GetMultiPlanet(whileScrubbing ? scrubchosen.planetIndex : chosenplanet.planetIndex, -(i + 1)).planetIndex;
				scrPlanet scrPlanet = planetList[planetIndex];
				if (!whileScrubbing)
				{
					scrPlanet.Die(0.3f);
				}
				else
				{
					scrPlanet.Destroy();
				}
				scrPlanet.toDelete = true;
			}
			int num = 0;
			for (int num2 = planetList.Count - 1; num2 >= 0; num2--)
			{
				if (planetList[num2].toDelete)
				{
					availablePlanets.Insert(num, planetList[num2]);
					planetList[num2].toDelete = false;
					planetList.RemoveAt(num2);
					num++;
				}
			}
		}
		if (numPlanets > count)
		{
			int num3 = 0;
			int num4 = whileScrubbing ? scrubchosen.planetIndex : chosenplanet.planetIndex;
			int num5 = num4 + num3;
			for (int j = count; j < numPlanets; j++)
			{
				num5 = num4 + num3;
				planetList.Insert(num5, availablePlanets[0]);
				planetList[num5].Rewind();
				planetList[num5].transform.position = chosenplanet.transform.position;
				planetList[num5].ClearParticles();
				if (!GCS.staticPlanetColors)
				{
					if (Persistence.GetFaceMode(red: true) && planetList[num5] == redPlanet)
					{
						redPlanet.SetFaceMode(enabled: true);
					}
					else if (Persistence.GetFaceMode(red: false) && planetList[num5] == bluePlanet)
					{
						bluePlanet.SetFaceMode(enabled: true);
					}
					else if (Persistence.GetFaceMode(red: true) && Persistence.GetFaceMode(red: false))
					{
						planetList[num5].SetFaceMode(enabled: true);
					}
					if (Persistence.GetSamuraiMode(red: true) && planetList[num5] == redPlanet)
					{
						redPlanet.ToggleSamurai(enabled: true);
					}
					else if (Persistence.GetSamuraiMode(red: false) && planetList[num5] == bluePlanet)
					{
						bluePlanet.ToggleSamurai(enabled: true);
					}
					else if (Persistence.GetSamuraiMode(red: true) && Persistence.GetSamuraiMode(red: false))
					{
						planetList[num5].ToggleSamurai(enabled: true);
					}
				}
				availablePlanets.RemoveAt(0);
				num3++;
			}
		}
		for (int k = 0; k < planetList.Count; k++)
		{
			planetList[k].planetIndex = k;
			planetList[k].next = GetMultiPlanet(k, 1);
			planetList[k].prev = GetMultiPlanet(k, -1);
		}
		if (GCS.staticPlanetColors)
		{
			return;
		}
		if ((Persistence.GetPlayerColor(red: true) == scrPlanet.transPinkColor && Persistence.GetPlayerColor(red: false) == scrPlanet.transBlueColor) || (Persistence.GetPlayerColor(red: false) == scrPlanet.transPinkColor && Persistence.GetPlayerColor(red: true) == scrPlanet.transBlueColor) || (Persistence.GetPlayerColor(red: true) == scrPlanet.nbYellowColor && Persistence.GetPlayerColor(red: false) == scrPlanet.nbPurpleColor) || (Persistence.GetPlayerColor(red: false) == scrPlanet.nbYellowColor && Persistence.GetPlayerColor(red: true) == scrPlanet.nbPurpleColor))
		{
			Color item = new Color(0.9568627f, 164f / 255f, 0.7098039f);
			Color item2 = new Color(0.3607843f, 67f / 85f, 0.9294118f);
			Color white = Color.white;
			Color item3 = Color.white;
			Color white2 = Color.white;
			Color white3 = Color.white;
			if ((Persistence.GetPlayerColor(red: true) == scrPlanet.nbYellowColor && Persistence.GetPlayerColor(red: false) == scrPlanet.nbPurpleColor) || (Persistence.GetPlayerColor(red: false) == scrPlanet.nbYellowColor && Persistence.GetPlayerColor(red: true) == scrPlanet.nbPurpleColor))
			{
				item = new Color(0.612f, 0.345f, 0.82f);
				item2 = new Color(0.996f, 0.953f, 0.18f);
				item3 = Color.black;
				white2 = Color.white;
			}
			List<Color> list = new List<Color>
			{
				item,
				item2,
				white
			};
			List<Color> list2 = new List<Color>
			{
				item3,
				white2,
				white3
			};
			List<List<int>> list3 = new List<List<int>>();
			list3.Add(new List<int>
			{
				0,
				1
			});
			list3.Add(new List<int>
			{
				0,
				1,
				2
			});
			list3.Add(new List<int>
			{
				0,
				1,
				0,
				1
			});
			list3.Add(new List<int>
			{
				0,
				1,
				1,
				0,
				2
			});
			list3.Add(new List<int>
			{
				0,
				1,
				1,
				0,
				2,
				2
			});
			list3.Add(new List<int>
			{
				0,
				1,
				0,
				2,
				0,
				1,
				0
			});
			list3.Add(new List<int>
			{
				0,
				1,
				0,
				2,
				2,
				0,
				1,
				0
			});
			for (int l = 0; l < planetList.Count; l++)
			{
				planetList[l].SetPlanetColor(list[list3[planetList.Count - 2][l]]);
				planetList[l].SetTailColor(list2[list3[planetList.Count - 2][l]]);
			}
		}
		else
		{
			if (!(Persistence.GetPlayerColor(red: true) == Persistence.GetPlayerColor(red: false)))
			{
				return;
			}
			for (int m = 0; m < planetList.Count; m++)
			{
				if (Persistence.GetPlayerColor(red: true) == scrPlanet.goldColor)
				{
					planetList[m].DisableAllSpecialPlanets();
					planetList[m].DisableCustomColor();
					planetList[m].SwitchToGold();
				}
				if (Persistence.GetPlayerColor(red: true) == scrPlanet.overseerColor)
				{
					planetList[m].DisableAllSpecialPlanets();
					planetList[m].DisableCustomColor();
					planetList[m].SwitchToOverseer();
				}
				if (Persistence.GetPlayerColor(red: true) == scrPlanet.rainbowColor)
				{
					planetList[m].SetRainbow(enabled: true);
				}
			}
		}
	}

	public void UnlockInput()
	{
		lockInput = 0f;
		responsive = true;
	}

	public void LockInput(float fSecs)
	{
		if (fSecs > 0f)
		{
			lockInput = fSecs;
			responsive = false;
		}
	}

	public void UpdateLockInput()
	{
		if (lockInput > 0f && !responsive)
		{
			lockInput -= Time.deltaTime * ADOBase.conductor.song.pitch;
			if (lockInput <= 0f)
			{
				responsive = true;
				lockInput = 0f;
			}
		}
	}

	public void UpdateFreeroam()
	{
		if (ADOBase.lm == null)
		{
			return;
		}
		List<scrFloor> listFreeroamStartTiles = ADOBase.lm.listFreeroamStartTiles;
		if (listFreeroamStartTiles == null || listFreeroamStartTiles.Count == 0 || curFreeRoamSection >= listFreeroamStartTiles.Count)
		{
			return;
		}
		freeroamUpTime += Time.deltaTime;
		scrFloor scrFloor = listFreeroamStartTiles[curFreeRoamSection];
		if (freeroamUpTime > 0.1f && ADOBase.conductor.songposition_minusi > scrFloor.nextfloor.entryTime - ADOBase.conductor.crotchet / (double)scrFloor.speed * (double)scrFloor.freeroamEndEarlyBeats)
		{
			if (currentState == States.PlayerControl || scnEditor.instance != null)
			{
				foreach (scrFloor item in ADOBase.lm.listFreeroam[curFreeRoamSection])
				{
					if (item != chosenplanet.currfloor)
					{
						item.ToggleCollider(collEn: false);
						item.isLandable = false;
						item.TweenOpacity(0f, (float)ADOBase.conductor.crotchet);
					}
					else
					{
						item.ToggleCollider(collEn: false);
						item.isLandable = false;
						int freeroamEndEarlyBeats = scrFloor.freeroamEndEarlyBeats;
						float num = (float)ADOBase.conductor.crotchet * ((float)freeroamEndEarlyBeats - 0.8f) / scrFloor.speed;
						if (ADOBase.conductor.crotchet / (double)scrFloor.speed * (double)scrFloor.freeroamEndEarlyBeats - (double)num < 0.05000000074505806)
						{
							num = 0f;
						}
						if (num > 0f)
						{
							LockInput(num);
						}
						for (int i = 0; i < planetList.Count; i++)
						{
							planetList[i].iFrames = (float)ADOBase.conductor.crotchet * ((float)freeroamEndEarlyBeats - 0.8f) / scrFloor.speed;
						}
						MoveCameraToTile(scrFloor.nextfloor, item, (float)(ADOBase.conductor.crotchet / (double)scrFloor.nextfloor.speed) * (float)freeroamEndEarlyBeats * 0.5f / ADOBase.conductor.song.pitch, scrFloor.freeroamEndEase, 1f);
						Vector3 b = startRadius * scrFloor.nextfloor.radiusScale * new Vector3(Mathf.Sin((float)scrFloor.exitangle), Mathf.Cos((float)scrFloor.exitangle), 0f);
						Vector3 endValue = scrFloor.nextfloor.transform.position - b;
						float duration = (float)(ADOBase.conductor.crotchet / (double)scrFloor.speed) * (float)freeroamEndEarlyBeats * 0.5f / ADOBase.conductor.song.pitch;
						item.transform.DOMove(endValue, duration).SetEase(scrFloor.freeroamEndEase);
						if (!stickToFloor)
						{
							chosenplanet.transform.DOMove(endValue, duration).SetEase(scrFloor.freeroamEndEase);
						}
						DOTween.Sequence().AppendInterval((float)(ADOBase.conductor.crotchet / (double)scrFloor.speed) * (float)freeroamEndEarlyBeats / ADOBase.conductor.song.pitch).Append(item.TweenOpacity(0f, (float)ADOBase.conductor.crotchet));
						double num2 = scrMisc.mod(chosenplanet.angle, 6.2831854820251465);
						ADOBase.conductor.lastHit = scrFloor.nextfloor.entryTime - ADOBase.conductor.crotchet / (double)scrFloor.speed * (double)freeroamEndEarlyBeats;
						item.isCCW = item.nextfloor.isCCW;
						isCW = !item.isCCW;
						int num3 = (!item.isCCW) ? 1 : (-1);
						bool isCCW = item.isCCW;
						double num4 = scrFloor.nextfloor.entryangle - (double)(MathF.PI * (float)num3);
						double num5 = num4 - (double)(MathF.PI * (float)freeroamEndEarlyBeats * (float)num3);
						chosenplanet.SetTargetExitAngle(num4);
						chosenplanet.SetSnappedLastAngle(num5);
						chosenplanet.Update_RefreshAngles();
						double num6 = scrMisc.mod(chosenplanet.angle, 6.2831854820251465);
						float angleCorrectionType = item.angleCorrectionType;
						bool isCCW2 = item.isCCW;
						chosenplanet.TweenSnappedLastAngle(num5 - (num6 - num2), num5);
						gameworld = true;
					}
				}
			}
			curFreeRoamSection++;
		}
	}

	public void UpdateInput()
	{
		validKeyWasReleased = false;
		long num = 0L;
		List<UnityEngine.KeyCode> list = new List<UnityEngine.KeyCode>();
		AsyncInputManager.keyDownMask.Clear();
		AsyncInputManager.keyUpMask.Clear();
		AsyncInputManager.frameDependentKeyDownMask.Clear();
		AsyncInputManager.frameDependentKeyUpMask.Clear();
		KeyEvent result;
		while (AsyncInputManager.keyQueue.TryDequeue(out result))
		{
			if (result.tick != num)
			{
				if (num != 0L)
				{
					ProcessKeyInputs(list, num);
				}
				num = result.tick;
				list.Clear();
				AsyncInputManager.keyDownMask.Clear();
				AsyncInputManager.keyUpMask.Clear();
			}
			if (result.press)
			{
				if (!AsyncInputManager.keyMask.Contains(result.keyCode))
				{
					AsyncInputManager.keyMask.Add(result.keyCode);
					AsyncInputManager.keyDownMask.Add(result.keyCode);
					AsyncInputManager.frameDependentKeyMask.Add(result.keyCode);
					AsyncInputManager.frameDependentKeyDownMask.Add(result.keyCode);
					list.Add((UnityEngine.KeyCode)result.keyCode);
					RDBaseDll.printem($"[Frame {Time.frameCount}] Async Input>> Handle press key {result.keyCode}");
				}
			}
			else
			{
				AsyncInputManager.keyMask.Remove(result.keyCode);
				AsyncInputManager.keyUpMask.Add(result.keyCode);
				AsyncInputManager.frameDependentKeyMask.Remove(result.keyCode);
				AsyncInputManager.frameDependentKeyUpMask.Add(result.keyCode);
				RDBaseDll.printem($"[Frame {Time.frameCount}] Async Input>> Handle release key {result.keyCode}");
			}
		}
		LinkedList<ushort> linkedList = new LinkedList<ushort>();
		LinkedList<ushort> linkedList2 = new LinkedList<ushort>();
		foreach (Tuple<UnityEngine.KeyCode, ushort> unityNativeKeyCode in KeyCodeConverter.UnityNativeKeyCodeList)
		{
			unityNativeKeyCode.Deconstruct(out UnityEngine.KeyCode item, out ushort item2);
			UnityEngine.KeyCode key = item;
			ushort num2 = item2;
			if (UnityEngine.Input.GetKeyUp(key))
			{
				if (AsyncInputManager.keyMask.Contains(num2))
				{
					linkedList2.AddLast(num2);
				}
			}
			else if (UnityEngine.Input.GetKeyDown(key) && !AsyncInputManager.keyMask.Contains(num2))
			{
				linkedList.AddLast(num2);
			}
		}
		if (linkedList.Count > 0 || linkedList2.Count > 0)
		{
			if (num != AsyncInputManager.currFrameTick)
			{
				if (num != 0L)
				{
					ProcessKeyInputs(list, num);
				}
				validKeyWasReleased = false;
				num = AsyncInputManager.currFrameTick;
				list.Clear();
				AsyncInputManager.keyDownMask.Clear();
				AsyncInputManager.keyUpMask.Clear();
			}
			validKeyWasReleased = (linkedList2.Count > 0);
			foreach (ushort item3 in linkedList)
			{
				list.Add((UnityEngine.KeyCode)item3);
				AsyncInputManager.keyMask.Add(item3);
				AsyncInputManager.keyDownMask.Add(item3);
				AsyncInputManager.frameDependentKeyMask.Add(item3);
				AsyncInputManager.frameDependentKeyDownMask.Add(item3);
				RDBaseDll.printesw($"[Frame {Time.frameCount}] Async Input>> Handle dropped release key {item3}");
			}
			foreach (ushort item4 in linkedList2)
			{
				AsyncInputManager.keyMask.Remove(item4);
				AsyncInputManager.keyUpMask.Add(item4);
				AsyncInputManager.frameDependentKeyMask.Remove(item4);
				AsyncInputManager.frameDependentKeyUpMask.Add(item4);
				RDBaseDll.printesw($"[Frame {Time.frameCount}] Async Input>> Handle dropped press key {item4}");
			}
		}
		ProcessKeyInputs(list, num);
	}

	private States GetDestinationState(StateEngine stateMachine)
	{
		return (States)(object)((StateMapping)DestinationStateField.GetValue(stateMachine)).state;
	}

	private void ProcessKeyInputs(IReadOnlyList<UnityEngine.KeyCode> keyCodes, long eventTick)
	{
		long value = (eventTick != 0L) ? eventTick : AsyncInputManager.currFrameTick;
		if (base.state == States.PlayerControl && GetDestinationState(base.stateMachine) == States.PlayerControl)
		{
			Simulated_PlayerControl_Update(value);
		}
	}

	public void ScreenShake(float duration, float strength)
	{
		DOTween.Shake(() => camy.shake, delegate(Vector3 x)
		{
			camy.shake = x;
		}, duration, strength, 100);
	}

	public void MoveCameraToTile(scrFloor floor, scrFloor from, float fSecs, Ease ease, float zoom = -1f)
	{
		ffxCameraPlus ffxCameraPlus = chosenplanet.currfloor.gameObject.AddComponent<ffxCameraPlus>();
		ffxCameraPlus.duration = fSecs;
		ffxCameraPlus.targetPos = new Vector2(floor.transform.position.x - from.transform.position.x, floor.transform.position.y - from.transform.position.y);
		ffxCameraPlus.targetRot = camy.transform.eulerAngles.z;
		if (zoom <= 0f)
		{
			ffxCameraPlus.targetZoom = camy.zoomSize;
		}
		else
		{
			ffxCameraPlus.targetZoom = zoom;
		}
		ffxCameraPlus.ease = ease;
		ffxCameraPlus.movementType = CamMovementType.Tile;
		ffxCameraPlus.StartEffect();
	}

	public void MoveCameraToObject(GameObject o, float fSecs, Ease ease, float zoom = -1f)
	{
		ffxCameraPlus ffxCameraPlus = new ffxCameraPlus();
		ffxCameraPlus.ForceUpdateCamParent();
		ffxCameraPlus.duration = fSecs;
		ffxCameraPlus.targetPos = new Vector2(o.transform.position.x, o.transform.position.y);
		ffxCameraPlus.targetRot = camy.transform.eulerAngles.z;
		if (zoom <= 0f)
		{
			ffxCameraPlus.targetZoom = camy.zoomSize;
		}
		else
		{
			ffxCameraPlus.targetZoom = zoom;
		}
		ffxCameraPlus.ease = ease;
		ffxCameraPlus.movementType = CamMovementType.Tile;
		ffxCameraPlus.StartEffect();
	}

	public void MoveCameraToPlayer(float fSecs, Ease ease, float zoom = -1f)
	{
		ffxCameraPlus ffxCameraPlus = new ffxCameraPlus();
		ffxCameraPlus.ForceUpdateCamParent();
		ffxCameraPlus.duration = fSecs;
		ffxCameraPlus.targetPos = Vector2.zero;
		ffxCameraPlus.targetRot = camy.transform.eulerAngles.z;
		if (zoom <= 0f)
		{
			ffxCameraPlus.targetZoom = camy.zoomSize;
		}
		else
		{
			ffxCameraPlus.targetZoom = zoom;
		}
		ffxCameraPlus.ease = ease;
		ffxCameraPlus.movementType = CamMovementType.Player;
		ffxCameraPlus.StartEffect();
	}

	public void LevelNameTextAway()
	{
		levelNameTextPresent = false;
		lvlname = scrUIController.instance.txtLevelName.transform.GetComponent<RectTransform>();
		lvlnameAnchorPos = lvlname.anchoredPosition;
		lvlname.DOAnchorPosY(200f, 1f).SetEase(Ease.InBack);
	}

	public void LevelNameTextRestore()
	{
		if (!levelNameTextPresent)
		{
			lvlname.DOAnchorPosY(lvlnameAnchorPos.y, 1f).SetEase(Ease.OutBack);
		}
	}

	public void SaveProgress(bool save)
	{
		if (saveProgressConditions)
		{
			scrMistakesManager.SaveProgress(save);
		}
	}

	private void Countdown_Update()
	{
		if ((float)ADOBase.conductor.beatNumber >= ADOBase.conductor.adjustedCountdownTicks || !isGameWorld || forceNoCountdown)
		{
			ChangeState(States.PlayerControl);
		}
		if (camy.followMode)
		{
			camy.topos = new Vector3(chosenplanet.transform.position.x, chosenplanet.transform.position.y, camy.transform.position.z);
		}
	}

	private void Checkpoint_Enter()
	{
		startTime = ADOBase.conductor.songposition_minusi;
	}

	private void Checkpoint_Update()
	{
		if (!(ADOBase.editor?.inStrictlyEditingMode ?? false))
		{
			double num = (ADOBase.conductor.songposition_minusi - startTime) / (ADOBase.lm.listFloors[GCS.checkpointNum].entryTime - startTime);
			ADOBase.conductor.song.volume = Mathf.Lerp(0f, startVolume, (float)num);
			if (chosenplanet.AutoShouldHitNow() && ADOBase.conductor.hasSongStarted)
			{
				Hit();
			}
			if (ADOBase.conductor.songposition_minusi >= ADOBase.lm.listFloors[GCS.checkpointNum].entryTime)
			{
				ChangeState(States.PlayerControl);
			}
			if (camy.followMode)
			{
				camy.topos = new Vector3(chosenplanet.transform.position.x, chosenplanet.transform.position.y, camy.transform.position.z);
			}
		}
	}

	private void Checkpoint_Exit()
	{
		camy.GetComponent<Grayscale>().enabled = false;
		ADOBase.conductor.song.volume = startVolume;
	}

	private void PlayerControl_Enter()
	{
		if (gameworld)
		{
			camy.GetComponent<Grayscale>().enabled = false;
		}
	}

	private void PlayerControl_Update()
	{
		if (!AsyncInputManager.isActive)
		{
			Simulated_PlayerControl_Update();
		}
		averageFrameTime = 0.5f * averageFrameTime + 0.5f * Time.deltaTime;
		if (camy.followMode && camy.followMovingPlatforms)
		{
			Vector3 position = chosenplanet.transform.position;
			camy.topos = new Vector3(position.x, position.y, camy.transform.position.z);
		}
	}

	public void Simulated_PlayerControl_Update(long? targetTick = default(long?))
	{
		if (paused || currFloor == null || isCutscene)
		{
			return;
		}
		__nextTileIsHoldCached = false;
		validInputWasReleasedThisFrame = ValidInputWasReleased();
		cachedCamyToPos = ADOBase.controller.camy.topos;
		if ((bool)currFloor.nextfloor)
		{
			scrFloor nextfloor = currFloor.nextfloor;
			while (nextfloor.midSpin && (bool)nextfloor.nextfloor)
			{
				nextfloor = nextfloor.nextfloor;
			}
			__nextTileIsHoldCached = (nextfloor.holdLength > -1);
		}
		AsyncInputUtils.WhileFloorNotChangeOrOnceIfAsyncInputDisabled(this, delegate
		{
			CheckPostHoldFail(targetTick);
		});
		AsyncInputUtils.WhileFloorNotChangeOrOnceIfAsyncInputDisabled(this, delegate
		{
			OttoHoldHit(targetTick);
		});
		HitAutoFloors(targetTick);
		UpdateHoldBehavior(targetTick);
		AsyncInputUtils.WhileFloorNotChangeOrOnceIfAsyncInputDisabled(this, delegate
		{
			HitHoldFloorsIfStartedAtHold(targetTick);
		});
		AsyncInputUtils.WhileFloorNotChangeOrOnceIfAsyncInputDisabled(this, delegate
		{
			CheckPreHoldFail(targetTick);
		});
		AsyncInputUtils.WhileFloorNotChangeOrOnceIfAsyncInputDisabled(this, delegate
		{
			UpdateHoldKeys(targetTick);
		});
		Vector3 topos = camy.topos;
		if (cachedCamyToPos != topos)
		{
			shouldReplaceCamyToPos = true;
			overrideCamyToPos = topos;
		}
	}

	private void CheckPostHoldFail(long? targetTick)
	{
		if (targetTick.HasValue)
		{
			long valueOrDefault = targetTick.GetValueOrDefault();
			AsyncInputUtils.AdjustAngle(this, valueOrDefault);
		}
		double minAngleMargin = _minAngleMargin;
		double num = Math.Max(3.1415927410125732, minAngleMargin * 2.0);
		if (noFail || currFloor.isSafe)
		{
			num = minAngleMargin * 1.01;
		}
		double num2 = chosenplanet.angle - chosenplanet.targetExitAngle;
		if (!isCW)
		{
			num2 *= -1.0;
		}
		if (isGameWorld && ADOBase.lm.listFloors.Count > currFloor.seqID + 1 && num2 > num)
		{
			FailAction();
		}
	}

	private void OttoHoldHit(long? targetTick = default(long?))
	{
		if (targetTick.HasValue)
		{
			long valueOrDefault = targetTick.GetValueOrDefault();
			AsyncInputUtils.AdjustAngle(this, valueOrDefault);
		}
		bool nextTileIsAuto = _nextTileIsAuto;
		if ((!((RDC.auto || benchmarkMode) | nextTileIsAuto) && (currFloor.holdLength <= -1 || !currFloor.auto)) || !isGameWorld)
		{
			return;
		}
		int num = RDC.useOldAuto ? 1 : 5;
		while (num > 0 && chosenplanet.AutoShouldHitNow())
		{
			bool auto = RDC.auto;
			RDC.auto = true;
			if (currFloor.holdLength > -1)
			{
				currFloor.holdRenderer.Hit();
			}
			keyTimes.Clear();
			Hit();
			RDC.auto = auto;
			num--;
		}
	}

	private void HitAutoFloors(long? targetTick = default(long?))
	{
		if (targetTick.HasValue)
		{
			long valueOrDefault = targetTick.GetValueOrDefault();
			AsyncInputUtils.AdjustAngle(this, valueOrDefault);
		}
		bool nextTileIsAuto = _nextTileIsAuto;
		if (ValidInputWasTriggered() && (!nextTileIsAuto || (nextTileIsAuto && currFloor.freeroam)))
		{
			int num = CountValidKeysPressed();
			for (int i = 0; i < num; i++)
			{
				double timeAsDouble = Time.timeAsDouble;
				keyTimes.Add(timeAsDouble);
			}
			if (num == 1)
			{
				consecMultipressCounter = 0;
			}
		}
	}

	private void UpdateHoldBehavior(long? targetTick = default(long?))
	{
		if (targetTick.HasValue)
		{
			long valueOrDefault = targetTick.GetValueOrDefault();
			AsyncInputUtils.AdjustAngle(this, valueOrDefault);
		}
		double minAngleMargin = _minAngleMargin;
		double num = _holdMargin;
		if (!gameworld && currFloor.holdLength > -1)
		{
			float num2 = MathF.PI * (float)(currFloor.holdLength * 2 + 1);
			num = 1.0 - minAngleMargin * 1.0 / (double)num2;
			if (validInputWasReleasedThisFrame || (ValidInputWasTriggered() && !strictHolds))
			{
				if ((double)currFloor.holdCompletion > num)
				{
					currFloor.holdRenderer.Unfill();
					currFloor.holdCompletion = 0f;
					Hit();
					if (AsyncInputManager.isActive)
					{
						RDBaseDll.printem($"[Frame {Time.frameCount}] Async Input>> hold Hit from update {ADOBase.controller.currFloor.seqID}th tile");
					}
				}
				else
				{
					currFloor.holdRenderer.Unfill();
					currFloor.holdCompletion = 0f;
					camy.Refocus(currFloor.prevfloor.transform);
					DOTween.To(() => camy.holdOffset, delegate(Vector3 x)
					{
						camy.holdOffset = x;
					}, Vector3.zero, 0.3f).SetEase(Ease.OutCubic);
					chosenplanet.transform.DOMove(currFloor.prevfloor.transform.position, 0.4f).SetEase(Ease.OutCubic);
					chosenplanet.currfloor = currFloor.prevfloor;
					scrFlash.OnDamage();
					LockInput(0.4f);
				}
			}
			if ((double)currFloor.holdCompletion > 2.0 - num || currFloor.holdCompletion < -0.3f)
			{
				currFloor.holdRenderer.Unfill(withHoldTiming: false);
				currFloor.holdCompletion = 0f;
				camy.Refocus(currFloor.prevfloor.transform);
				DOTween.To(() => camy.holdOffset, delegate(Vector3 x)
				{
					camy.holdOffset = x;
				}, Vector3.zero, 0.3f).SetEase(Ease.OutCubic);
				chosenplanet.transform.DOMove(currFloor.prevfloor.transform.position, 0.4f).SetEase(Ease.OutCubic);
				chosenplanet.currfloor = currFloor.prevfloor;
				scrFlash.OnDamage();
				LockInput(0.4f);
			}
		}
		bool nextTileIsAuto = _nextTileIsAuto;
		bool nextTileIsHold = _nextTileIsHold;
		if (!gameworld || !validInputWasReleasedThisFrame || nextTileIsAuto || currFloor.auto || RDC.auto || benchmarkMode || currFloor.holdLength <= -1)
		{
			return;
		}
		if ((double)currFloor.holdCompletion < num)
		{
			if (GCS.checkpointNum != currFloor.seqID && requireHolding)
			{
				FailAction();
			}
		}
		else if (!nextTileIsHold)
		{
			currFloor.holdRenderer.Hit();
			Hit();
			if (AsyncInputManager.isActive)
			{
				RDBaseDll.printem($"[Frame {Time.frameCount}] Async Input>> nextTileIsHold Hit from update {ADOBase.controller.currFloor.seqID}th tile");
			}
		}
	}

	private void HitHoldFloorsIfStartedAtHold(long? targetTick = default(long?))
	{
		if (targetTick.HasValue)
		{
			long valueOrDefault = targetTick.GetValueOrDefault();
			AsyncInputUtils.AdjustAngle(this, valueOrDefault);
		}
		if (!RDC.auto && !benchmarkMode && chosenplanet.AutoShouldHitNow() && currFloor.holdLength > -1 && GCS.checkpointNum == currFloor.seqID && ADOBase.controller.state != States.Fail && ADOBase.controller.state != States.Fail2)
		{
			Hit();
			if (AsyncInputManager.isActive)
			{
				RDBaseDll.printem($"[Frame {Time.frameCount}] Async Input>> Taro Hit from update {ADOBase.controller.currFloor.seqID}th tile");
			}
		}
	}

	private void CheckPreHoldFail(long? targetTick = default(long?))
	{
		if (targetTick.HasValue)
		{
			long valueOrDefault = targetTick.GetValueOrDefault();
			AsyncInputUtils.AdjustAngle(this, valueOrDefault);
		}
		bool nextTileIsAuto = _nextTileIsAuto;
		double holdMargin = _holdMargin;
		if (gameworld && !nextTileIsAuto && !currFloor.auto && (double)currFloor.holdCompletion > holdMargin && (double)currFloor.holdCompletion < 1.0 - holdMargin && !RDC.auto && !holding && requireHolding && GCS.checkpointNum != currFloor.seqID && !benchmarkMode && currFloor.holdLength > -1)
		{
			FailAction();
		}
	}

	private void UpdateHoldKeys(long? targetTick = default(long?))
	{
		if (targetTick.HasValue)
		{
			long valueOrDefault = targetTick.GetValueOrDefault();
			AsyncInputUtils.AdjustAngle(this, valueOrDefault);
		}
		bool nextTileIsHold = _nextTileIsHold;
		double holdMargin = _holdMargin;
		if (keyTimes.Count <= 0 || GCS.d_stationary || GCS.d_freeroam || (!((currFloor.holdLength > -1 && !strictHolds) | nextTileIsHold) && currFloor.holdLength != -1 && !((double)currFloor.holdCompletion < holdMargin)) || (RDC.auto && isGameWorld) || (isGameWorld && currFloor.seqID >= ADOBase.lm.listFloors.Count - 1) || benchmarkMode)
		{
			return;
		}
		keyTimes.RemoveAt(0);
		bool num = Hit();
		if (AsyncInputManager.isActive)
		{
			RDBaseDll.printem($"[Frame {Time.frameCount}] Async Input>> Hit from update {ADOBase.controller.currFloor.seqID}th tile");
		}
		if (num && currFloor.holdLength > -1)
		{
			holdKeys.Clear();
			IterateValidKeysHeld(delegate(MutualKeyCode k)
			{
				object obj2 = k.isAsync ? ((object)k.asyncKeyCode) : k.keyCodeObject;
				holdKeys.Add(obj2);
				printe($"(1) adding as holdKey {obj2}");
			}, delegate(MutualKeyCode k)
			{
				holdKeys.Remove(k.isAsync ? ((object)k.asyncKeyCode) : k.keyCodeObject);
			}, onlyPressedKeys: true);
		}
		if (midspinInfiniteMargin)
		{
			keyTimes.RemoveAt(0);
			if (Hit() && currFloor.holdLength > -1)
			{
				holdKeys.Clear();
				IterateValidKeysHeld(delegate(MutualKeyCode k)
				{
					object obj = k.isAsync ? ((object)k.asyncKeyCode) : k.keyCodeObject;
					holdKeys.Add(obj);
					printe($"(2) adding as holdKey {obj}");
				}, delegate(MutualKeyCode k)
				{
					holdKeys.Remove(k.isAsync ? ((object)k.asyncKeyCode) : k.keyCodeObject);
				}, onlyPressedKeys: true);
			}
		}
	}

	private void Won_Enter()
	{
		if (!gameworld && !currFloor.freeroamGenerated)
		{
			PortalTravelAction(portalDestination);
		}
	}

	private void Won_Update()
	{
		if (ValidInputWasTriggered() && Time.unscaledTime - winTime > 1f && canExitLevel)
		{
			PortalTravelAction(portalDestination);
		}
	}

	public void FailByHitbox(string failMessage = "")
	{
		FailAction(overload: false, multipress: false, failMessage, hitbox: true);
	}

	public void FailAction(bool overload = false, bool multipress = false, string failMessage = "", bool hitbox = false)
	{
		ADOBase.controller.SaveProgress(save: false);
		bool flag = currFloor.nextfloor != null && currFloor.nextfloor.auto;
		if (!hitbox)
		{
			if (((RDC.auto | flag) && !RDC.useOldAuto) || (RDC.debug && !overload) || (!gameworld && !currFloor.freeroam))
			{
				return;
			}
			if (noFail || currFloor.isSafe)
			{
				if (overload)
				{
					if (!currFloor.hideJudgment)
					{
						chosenplanet.MarkFail()?.BlinkForSeconds(3f);
					}
					return;
				}
				scrFloor nextfloor = currFloor.nextfloor;
				if ((!nextfloor || nextfloor.tapsSoFar + 1 == nextfloor.tapsNeeded) && !currFloor.hideJudgment)
				{
					chosenplanet.MarkFail()?.BlinkForSeconds(3f);
				}
				noFailInfiniteMargin = true;
				Hit();
				noFailInfiniteMargin = false;
				return;
			}
		}
		ChangeState(States.Fail);
		if (scrVfxPlus.instance != null)
		{
			scrVfxPlus.instance.enabled = false;
		}
		if (overload)
		{
			scrUIController.instance.txtCountdown.GetComponent<scrCountdown>().ShowOverload();
			AdjustTryCalibratingTextPosition();
			if (failMessage.IsNullOrEmpty())
			{
				if (multipress)
				{
					txtTryCalibrating.text = RDString.Get("status.multipressExplainer");
				}
				else
				{
					txtTryCalibrating.text = RDString.Get("status.overloadExplainer");
				}
			}
		}
		if (!failMessage.IsNullOrEmpty())
		{
			txtTryCalibrating.text = failMessage;
		}
		foreach (scrMissIndicator item in missesOnCurrFloor)
		{
			item.StartBlinking();
		}
		if (!GCS.lofiVersion)
		{
			scrFlash.OnDamage();
		}
		ADOBase.conductor.song.Stop();
		if (ADOBase.controller.isPuzzleRoom || ((bool)ADOBase.controller.currFloor && ADOBase.controller.currFloor.freeroamGenerated))
		{
			ADOBase.conductor.song.volume = 0f;
		}
		AudioManager.Instance.StopAllSounds();
		if (!instantExplode)
		{
			printe("die");
			chosenplanet.PreDie();
			DOTween.To(() => chosenplanet.cosmeticRadius, delegate(float x)
			{
				chosenplanet.cosmeticRadius = x;
			}, 0f, 0.5f).OnComplete(Fail2Action);
		}
		else
		{
			scrFlash.Flash(Color.white.WithAlpha(0.3f));
			chosenplanet.Die();
			chosenplanet.other.Die();
			DOVirtual.DelayedCall(0.5f, Fail2Action);
		}
		if (ADOBase.isLevelEditor && RDC.auto)
		{
			ADOBase.editor.blinkTimer.Kill();
			ADOBase.editor.autoFailed = true;
		}
		foreach (ffxPlusBase lossEffect in lossEffects)
		{
			lossEffect.StartEffect();
		}
		mistakesManager.RevertToLastCheckpoint();
		if (GCS.checkpointNum > 0)
		{
			checkpointsUsed++;
			startedFromCheckpoint = true;
		}
	}

	private void Fail2Action()
	{
		if (!gameworld && !currFloor.freeroam)
		{
			return;
		}
		ChangeState(States.Fail2);
		DOVirtual.DelayedCall(1f, delegate
		{
			RDUtils.SetGarbageCollectionEnabled(enabled: true);
		});
		if (GCS.practiceMode)
		{
			return;
		}
		mistakesManager.CalculatePercentAcc();
		if (isbosslevel && !isPuzzleRoom)
		{
			endLevelType = mistakesManager.Save(currentWorld, wonLevel: false, GCS.currentSpeedTrial);
			if (endLevelType == EndLevelType.NewBestAfterLongTime && !instantExplode)
			{
				scrSfx.instance.PlaySfx(SfxSound.ApplauseQuiet, 3f);
			}
		}
		else if (GCS.standaloneLevelMode && GCS.customLevelIndex >= GCS.customLevelPaths.Length - 1 && !isPuzzleRoom && !GCS.practiceMode)
		{
			endLevelType = mistakesManager.SaveCustom(ADOBase.customLevel.levelData.Hash, wonLevel: false, GCS.currentSpeedTrial);
		}
		for (int i = 0; i < planetList.Count; i++)
		{
			planetList[i].Die();
		}
		if (!instantExplode)
		{
			scrFlash.Flash(Color.white.WithAlpha(0.3f));
			camy.ZoomOut();
			chosenplanet.Die();
			chosenplanet.other.Die();
		}
		deaths++;
		scrUIController.deathCounterToFixDotweenBug++;
		if ((isbosslevel || ADOBase.isLevelEditor) && !isPuzzleRoom)
		{
			txtPercent.GetComponent<scrPercentageComplete>().UpdatePercent();
			txtPercent.gameObject.SetActive(value: true);
		}
		if (!isbosslevel && !gameworld && !currFloor.freeroam)
		{
			txtPercent.GetComponent<scrPercentageComplete>().UpdatePercent();
			txtPercent.gameObject.SetActive(value: true);
			string msg = (!GCS.d_chinese) ? ((!GCS.d_booth) ? "" : (GCS.d_drumcontroller ? "Drum To Restart" : "Any Key To Restart")) : ((!GCS.d_booth) ? "" : (GCS.d_drumcontroller ? "拍下按钮, 重新挑战" : "拍下按钮, 重新挑战"));
			txtPercent.GetComponent<scrPercentageComplete>().ShowMessage(msg);
		}
		if (GCS.d_booth)
		{
			return;
		}
		string text = "";
		AdjustTryCalibratingTextPosition();
		if (deaths == 1000)
		{
			text = "status.1000Attempts";
		}
		else if (deaths == 100)
		{
			text = "status.100Attempts";
		}
		else if (!ADOBase.isEditingLevel)
		{
			bool flag = false;
			if (keyTotal > 5)
			{
				foreach (KeyValuePair<object, int> item in keyFrequency)
				{
					printe($"{item.Key} = {item.Value}");
					object key = item.Key;
					if ((key is UnityEngine.KeyCode || key is SharpHook.Native.KeyCode) && (float)item.Value / (float)keyTotal > 0.95f)
					{
						printe($"Key {item.Key} pressed with over 95% frequency. Caught single tapping in 4k");
						flag = true;
						break;
					}
				}
			}
			printe($"Level recommends two fingers {recommendsTwoFingers}, hint displayed: {displayedMultiFingerHint}, isSingleTapping: {flag}");
			if (deaths == 5 && Persistence.GetOverallProgressStage() < 3)
			{
				text = "status.tryCalibrating";
			}
			else if (deaths == 5 || deaths % 10 == 0)
			{
				if (currFloor.freeroamGenerated || isPuzzleRoom)
				{
					text = "status.tryFreeroamInvulnerability";
				}
				else if (recommendsTwoFingers && flag && !displayedMultiFingerHint)
				{
					text = "status.tryTwoFingers";
					displayedMultiFingerHint = true;
				}
				else
				{
					displayedMultiFingerHint = false;
					if (base.practiceAvailable)
					{
						text = "status.tryPractice";
					}
				}
			}
		}
		if (!text.IsNullOrEmpty() && txtTryCalibrating.text.IsNullOrEmpty())
		{
			txtTryCalibrating.text = RDString.Get(text);
		}
	}

	private void Fail2_Update()
	{
		if (!ValidInputWasTriggered())
		{
			return;
		}
		if (scnEditor.instance != null)
		{
			if (!ADOBase.editor.inStrictlyEditingMode)
			{
				StartCoroutine(ResetCustomLevel());
			}
		}
		else
		{
			Restart();
		}
	}

	public IEnumerator ResetCustomLevel()
	{
		if (GCS.standaloneLevelMode)
		{
			bool complete = false;
			scrUIController.instance.WipeToBlack(WipeDirection.StartsFromRight, delegate
			{
				complete = true;
			});
			while (!complete)
			{
				yield return null;
			}
		}
		else
		{
			RDUtils.SetGarbageCollectionEnabled(enabled: true);
		}
		foreach (scrFloor listFloor in ADOBase.lm.listFloors)
		{
			if ((bool)listFloor.bottomGlow)
			{
				listFloor.bottomGlow.enabled = false;
			}
			listFloor.topGlow.enabled = false;
		}
		if (!GCS.practiceMode && GCS.standaloneLevelMode)
		{
			GCS.currentSpeedTrial = GCS.nextSpeedRun;
		}
		printe($"GCS.currentSpeedTrial {GCS.currentSpeedTrial} GCS.nextSpeedRun {GCS.nextSpeedRun}");
		ADOBase.customLevel.ResetScene();
		ADOBase.customLevel.Play(GCS.checkpointNum);
		transitioningLevel = false;
		if (GCS.standaloneLevelMode)
		{
			yield return null;
			scrUIController.instance.WipeFromBlack();
		}
	}

	private void AdjustTryCalibratingTextPosition()
	{
		if ((instance?.errorMeter?.gameObject.activeInHierarchy).GetValueOrDefault())
		{
			txtTryCalibrating.alignment = TextAnchor.UpperCenter;
		}
		else
		{
			txtTryCalibrating.alignment = TextAnchor.MiddleCenter;
		}
	}

	[CompilerGenerated]
	private static string _003COnLandOnPortal_003Eg__Localized_007C197_2(string s)
	{
		return RDString.Get("status.results." + s) + ": ";
	}

	[CompilerGenerated]
	private static string _003COnLandOnPortal_003Eg__Result_007C197_3(int resultCount, string color)
	{
		return $"<color={color}>{resultCount}</color>";
	}

	[CompilerGenerated]
	private static string _003COnLandOnPortal_003Eg__ResultWithAuto_007C197_4(int resultCount, int resultCount2, string color)
	{
		return $"<color={color}>{resultCount} ({resultCount2})</color>";
	}

	[CompilerGenerated]
	private static string _003COnLandOnPortal_003Eg__GoldAccuracy_007C197_5(string accText, ref _003C_003Ec__DisplayClass197_0 P_1)
	{
		if (!P_1.isPurePerfect)
		{
			return accText;
		}
		return "<color=#FFDA00>" + accText + "</color>";
	}

	[CompilerGenerated]
	private int _003COnLandOnPortal_003Eg__GetHits_007C197_6(HitMargin hitMargin)
	{
		return mistakesManager.GetHits(hitMargin);
	}

	[CompilerGenerated]
	private static bool _003CValidInputWasTriggered_003Eg__GetMouseDown_007C216_0()
	{
		if (AsyncInputManager.isActive)
		{
			if (!AsyncInput.GetKeyDown(MouseButton.Button1, frameDependent: false))
			{
				return AsyncInput.GetKeyDown(MouseButton.Button2, frameDependent: false);
			}
			return true;
		}
		if (!Input.GetKeyDown(UnityEngine.KeyCode.Mouse0))
		{
			return UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Mouse1);
		}
		return true;
	}

	[CompilerGenerated]
	private bool _003CValidInputWasTriggered_003Eg__GetInputHappened_007C216_1(ref _003C_003Ec__DisplayClass216_0 P_0)
	{
		if ((Input.anyKeyDown || dpadInputChecker.anyDirDown || RDInput.GetMain(ButtonState.IsDown) > 0) && !P_0.mouseOverAButton)
		{
			return !paused;
		}
		return false;
	}
}
