// scnEditor
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ADOFAI;
using DG.Tweening;
using GDMiniJSON;
using RDTools;
using SFB;
using Steamworks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityStandardAssets.ImageEffects;

public class scnEditor : ADOBase
{
	private enum SettingsTabType
	{
		None,
		Level,
		Song
	}

	public enum PopupType
	{
		SaveBeforeSongImport,
		SaveBeforeImageImport,
		SaveBeforeVideoImport,
		SaveBeforeLevelExport,
		ExportLevel,
		MissingExportParams,
		MissingFiles,
		OpenURL,
		CopyrightWarning,
		OggEncode,
		ConversionSuccessful,
		ConversionError,
		UnsavedChanges,
		MacAppStoreFolderRestriction,
		MacAppStoreFileOutsideDownloads,
		MultiPlanet,
		GenericText
	}

	[Serializable]
	public class LevelState
	{
		public LevelData data;

		public List<int> selectedFloors = new List<int>();

		public int[] selectedDecorationIndices;

		public int selectedDecorationItemIndex = -1;

		public LevelEventType settingsEventType;

		public LevelEventType floorEventType;

		public int floorEventTypeIndex;

		public LevelState(LevelData data, List<int> selectedFloors, int[] currentDecorationItemIndices)
		{
			this.data = data;
			this.selectedFloors = selectedFloors;
			selectedDecorationIndices = currentDecorationItemIndices;
		}
	}

	public struct NotificationAction
	{
		public string text;

		public Action action;

		public NotificationAction(string text, Action action)
		{
			this.text = text;
			this.action = action;
		}
	}

	public enum ClipboardContent
	{
		None,
		Floors,
		Decorations
	}

	public struct FloorData
	{
		public char stringDirection;

		public float floatDirection;

		public List<LevelEvent> levelEventData;

		public FloorData(char stringDir, float floatDir, List<LevelEvent> events)
		{
			stringDirection = stringDir;
			floatDirection = floatDir;
			levelEventData = events;
		}
	}

	private const float CameraZPosition = -10f;

	private const int PrimaryMouseButton = 0;

	private const int SecondaryMouseButton = 1;

	private const int AutoOff = 0;

	private const int AutoOn = 1;

	private const int AutoLeft = 2;

	private const int AutoRight = 3;

	private const int AutoOffLeft = 4;

	private const int AutoOffRight = 5;

	private const int AutoMiss = 6;

	private const int AutoNervousOn = 7;

	private const int AutoNervousOff = 8;

	private const int AutoPet = 9;

	private const string shortcutCol = "<color=#00000077>";

	private const string selectedColorTweenId = "selectedColorTween";

	public const int MaxUndoSteps = 100;

	private const int EventsInBar = 11;

	private readonly Color grayColor = new Color(36f / 85f, 36f / 85f, 36f / 85f);

	private readonly Color lineGreen = new Color(0.4f, 1f, 0.4f, 1f);

	private readonly Color lineYellow = new Color(1f, 1f, 0.4f, 1f);

	private readonly Color linePurple = new Color(0.75f, 0.5f, 1f, 1f);

	private readonly Color lineBlue = new Color(0.4f, 0.4f, 1f, 1f);

	public static string savedLevelString;

	[NonSerialized]
	[Header("Components")]
	public int version;

	public GameObject levelEditorScene;

	public GameObject levelStringPanel;

	public GameObject shortcutsPanel;

	public GameObject popupPanel;

	public GameObject popupWindow;

	public GameObject savePopupContainer;

	public GameObject urlPopupContainer;

	public GameObject copyrightPopupContainer;

	public GameObject paramsPopupContainer;

	public GameObject missingFilesPopupContainer;

	public GameObject oggPopupContainer;

	public GameObject okPopupContainer;

	public GameObject multiplanetPopupContainer;

	public GameObject unsavedChangesPopupContainer;

	public Text savePopupText;

	public Text paramsPopupText;

	public Text missingFilesPopupText;

	public Text copyrightText;

	public Slider oggConversionBar;

	public Text oggConversionBarText;

	public Text okPopupText;

	public Text multiplanetPopupText;

	public Text eventPickerText;

	public Text categoryText;

	public Button playPause;

	public Image playPauseIcon;

	public Button rewind;

	public EventSystem eventSystem;

	public PublishWindow publishWindow;

	public WorkshopThumbnailMaker thumbnailMaker;

	public RDColorPickerPopup colorPickerPopup;

	public EditorDifficultySelector editorDifficultySelector;

	public EditorSpeedIndicator speedIndicator;

	public FloorDirectionButton[] floorDirectionButtons;

	[Header("Level/song configuration panel")]
	public Transform settingsPanelsContainer;

	[Header("Buttons")]
	public Button buttonFileActionDropdown;

	public Button buttonNew;

	public Button buttonOpen;

	public Button buttonOpenRecent;

	public Button buttonOpenURL;

	public Button buttonSave;

	public Button buttonSaveAs;

	public Button buttonExit;

	public Button buttonSettings;

	public Button buttonHelp;

	public Button buttonCloseHelp;

	public Button buttonDiscord;

	public Button buttonAuto;

	public Button buttonNextPage;

	public Button buttonPrevPage;

	public Button buttonNoFail;

	[Header("Popup buttons")]
	public Button popupSaveOk;

	public Button popupSaveSaveAs;

	public Button popupURLDownload;

	public Button popupURLCancel;

	public Button popupCopyrightAccept;

	public Button popupCopyrightReturn;

	public Button popupParamsCancel;

	public Button popupMissingFilesCancel;

	public Button popupOggCancel;

	public Button popupOggConvert;

	public Button popupOkOk;

	public Button popupMultiplanetOk;

	private TweenCallback popupOkCallback;

	public Button popupUnsavedChangesCancel;

	public Button popupUnsavedChangesDiscard;

	public Button popupUnsavedChangesSave;

	public Button popupBlocker;

	[Header("Floor stuff")]
	public Button buttonD;

	public Button buttonW;

	public Button buttonA;

	public Button buttonS;

	public Button buttonE;

	public Button buttonQ;

	public Button buttonZ;

	public Button buttonC;

	public Button buttonT;

	public Button buttonG;

	public Button buttonF;

	public Button buttonB;

	public Button buttonH;

	public Button buttonJ;

	public Button buttonM;

	public Button buttonN;

	public Button buttonBackQuoteT;

	public Button buttonBackQuoteY;

	public Button buttonBackQuoteV;

	public Button buttonBackQuoteB;

	public Button buttonBackQuoteH;

	public Button buttonBackQuoteJ;

	public Button buttonBackQuoteM;

	public Button buttonBackQuoteN;

	public Button buttonSpace;

	public Button buttonTab;

	public Button buttonToggleAngleInput;

	public Image EventCircle;

	public Canvas floorButtonCanvas;

	public Canvas floorButtonPrimaryCanvas;

	public Canvas floorButtonExtraCanvas;

	public Canvas floorButtonExtraBackQuoteCanvas;

	public Canvas floorButtonLeftRightCanvas;

	public GameObject floorButtonContainer;

	public GameObject floorButtonArbitraryContainer;

	public InputField floorButtonArbitrary;

	public KeyIndicator tabIndicator;

	[Header("Colliders")]
	public GameObject Collider15;

	public GameObject Collider30;

	public GameObject Collider45;

	public GameObject Collider60;

	public GameObject Collider75;

	public GameObject Collider90;

	public GameObject Collider105;

	public GameObject Collider120;

	public GameObject Collider135;

	public GameObject Collider165;

	public GameObject Collider180;

	[Header("Inspector")]
	public InspectorPanel levelEventsPanel;

	public InspectorPanel settingsPanel;

	public GameObject inspectorTabs;

	public GameObject inspectorPanels;

	public RectTransform propertyHelpContainer;

	public RectTransform propertyHelpImage;

	[Header("Help Button Popup")]
	public Text propertyHelpText;

	public Button propertyHelpURLButton;

	public Text propertyHelpURLButtonText;

	[Header("Find Floor Popup")]
	public GameObject findFloorPanel;

	public Text findFloorPanelTitle;

	public Image findArrow;

	public Property findType;

	public Property findValue;

	public Text findFloorSelectedInfo;

	public Button findButton;

	[Header("Notification Popup")]
	public GameObject notificationPopupContainer;

	public RectTransform notificationPopupScrollview;

	public RectTransform notificationPopupScrollviewContent;

	public RectTransform notificationPopupActionsContainer;

	public GameObject notificationPopupScrollviewVertical;

	public GameObject notificationPopupScrollviewHorizontal;

	public RectTransform notificationPopupWindow;

	public Text notificationPopupTitle;

	public Text notificationPopupContent;

	public Button notificationOkButton;

	private List<Button> notificationPopupActions = new List<Button>();

	[Header("Shortcut")]
	public RectTransform shortcutTabsContainer;

	public RectTransform shortcutContentContainer;

	public GameObject shortcutTabPrefab;

	public GameObject shortcutContentPrefab;

	public GameObject shortcutTextPrefab;

	public Text shortcutTitle;

	private List<GameObject> shortcutTabs = new List<GameObject>();

	private List<GameObject> shortcutContent = new List<GameObject>();

	[Header("Others")]
	public GameObject fileActionsPanel;

	public Canvas levelEditorCanvas;

	public Canvas gameCanvas;

	public Canvas ottoCanvas;

	public Text filenameText;

	public GameObject notificationText;

	public Image fileIcon;

	public Image fileArrow;

	public InputField levelLinkInput;

	public EditorWebServices webServices;

	public Image lockIcon;

	public Image lockBackground;

	[Header("Level Events Bar")]
	public RectTransform levelEventsBar;

	public RectTransform levelEventsBarButtons;

	public RectTransform levelEventsBarCategories;

	public HorizontalLayoutGroup levelEventsBarCategoriesLayout;

	[NonSerialized]
	public LevelEventCategory currentCategory;

	[NonSerialized]
	public List<LevelEventType> favoriteEvents = new List<LevelEventType>();

	private Dictionary<LevelEventCategory, List<LevelEventButton>> eventButtons = new Dictionary<LevelEventCategory, List<LevelEventButton>>();

	private List<CategoryTab> categoryTabs = new List<CategoryTab>();

	private int currentPage;

	private int maxPage = 1;

	[NonSerialized]
	public List<scrFloor> selectedFloors = new List<scrFloor>();

	[NonSerialized]
	public int selectedFloorCached;

	[NonSerialized]
	public List<LevelEvent> selectedDecorations = new List<LevelEvent>();

	[NonSerialized]
	public scrFloor multiSelectPoint;

	[NonSerialized]
	public List<object> clipboard = new List<object>();

	public ClipboardContent clipboardContent;

	[NonSerialized]
	private bool refreshBgSprites;

	[NonSerialized]
	private bool refreshDecSprites;

	[NonSerialized]
	public PropertyControl_List propertyControlList;

	[Header("Tweakables")]
	public float scrollSpeed;

	public Color selectedColor0;

	public Color selectedColor1;

	public Color defaultColor;

	private Color currentSecondaryTrackColor;

	public float shortcutsPanelOpenHeight;

	public float shortcutsPanelCloseHeight;

	public float filePanelOpenWidth;

	public float filePanelCloseWidth;

	public float filePanelMoveDuration;

	public float findFloorPanelOpenHeight;

	public float findFloorPanelCloseHeight;

	public float floorButtonPulseSize;

	public float floorButtonPulseDuration;

	public float cameraSelectDuration;

	public Ease panelShowEase;

	public Ease panelHideEase;

	public Ease UIPanelEaseMode;

	public float UIPanelEaseDur;

	public float backupInterval = 30f;

	public float maxShadowDistance;

	public float maxOverlapRadius;

	public Color vfxIconColor;

	public Color shortcutsLockColor;

	public Color shortcutsLockIconColor;

	public Color notificationErrorColor;

	[Header("Prefabs")]
	public GameObject prefab_levelEventButton;

	public GameObject prefab_eventCategoryTab;

	public GameObject prefab_tileFlash;

	public GameObject prefab_editorNum;

	[Header("Data")]
	public Dictionary<string, AudioClip> preparedAudioClips;

	public new CustomLevel customLevel;

	[NonSerialized]
	[Header("Undo Redo")]
	public bool initialized;

	[NonSerialized]
	public int changingState;

	[NonSerialized]
	public List<LevelState> undoStates = new List<LevelState>();

	[NonSerialized]
	public List<LevelState> redoStates = new List<LevelState>();

	[NonSerialized]
	public LevelEventType settingsEventType;

	[NonSerialized]
	public LevelEventType filteredEventType;

	private int saveStateLastFrame;

	[Header("Sprites")]
	public Sprite pauseButtonIcon;

	public Sprite playButtonIcon;

	public Sprite[] autoSprites;

	public Image autoImage;

	public Sprite fileSpriteUp;

	public Sprite fileSpriteDown;

	public Sprite lockSpriteOff;

	public Sprite lockSpriteOn;

	public Sprite show8DirectionSprite;

	public Sprite showArbitraryAngleSprite;

	private List<GameObject> floorConnectorGOs = new List<GameObject>();

	private GameObject floorConnectors;

	public Texture2D floorConnectorTex;

	private Dictionary<scrFloor, Vector3> floorPositionsAtDragStart = new Dictionary<scrFloor, Vector3>();

	private Dictionary<scrDecoration, Vector3> decorationPositionsAtDragStart = new Dictionary<scrDecoration, Vector3>();

	private bool isDraggingTiles;

	private bool isDraggingDecorations;

	private TransformGizmo draggingGizmo;

	public static scnEditor instance;

	private float dragGridSize = 0.5f;

	private Shader spritesDefaultShader;

	private float autoPetTime;

	[Header("Audio Sources")]
	[SerializeField]
	private AudioSource ottoAudioSrc;

	[SerializeField]
	private AudioSource interfaceAudioSrc;

	[NonSerialized]
	[Header("Runtime")]
	public string songToConvert;

	private CanvasScaler canvasScaler;

	[NonSerialized]
	public RectTransform decorationsListContent;

	private SettingsTabType selectedSettingsTab;

	private Vector3 mousePosition0;

	private Vector3 cameraPositionAtDragStart;

	private Vector3 evIndPosAtDragStart;

	private LayerMask floorLayerMask;

	private LayerMask handlesLayerMask;

	private LayerMask textDecoLayerMask;

	private scrFloor lastSelectedFloor;

	private int lastSelectedFloorsCount;

	private Tween copiedColorTween;

	private Sequence notificationSeq;

	[NonSerialized]
	public Sequence blinkTimer;

	public bool autoFailed;

	[NonSerialized]
	public Camera camera;

	private UnityWebRequest www;

	private Coroutine downloadCo;

	private GameObject[] previouslyFoundObjects;

	private int selectedObjectIndexOfBunch;

	public Texture2D workshopThumbnail;

	[NonSerialized]
	public float speedMultiplier;

	private bool dragging;

	private bool cancelDrag;

	private EventIndicator draggedEvIndicator;

	private float freeAngle;

	private bool freeAngleMode;

	private bool showingFileActions;

	private bool showingShortcuts;

	private bool showingPopup;

	private bool showingFindFloorPanel;

	private bool hoveringFileActions;

	private bool hoveringFindFloorPanel;

	private bool isOttoBlinking;

	private int ottoBlinkCounter;

	private bool downloadingLevel;

	private bool animatingPropertyHelp;

	private bool showingPropertyHelp;

	private float backupTimer;

	private bool _unsavedChanges;

	private bool lockPathEditing;

	private bool decorationWasSelected;

	private List<LevelEvent> lastSelectedDecorations;

	private Material lineMaterial;

	private Color defaultLockColor;

	private Color defaultLockIconColor;

	private Dictionary<string, string> errorImageResult = new Dictionary<string, string>();

	private bool isUnauthorizedAccess;

	[NonSerialized]
	public bool showFloorNums;

	private static float lastTimeStatsUploaded;

	private const float IntervalToUpdateSteamStats = 120f;

	private Action quitLevelAction;

	private bool forceQuit;

	private LevelEventType[] whitelistedEvents = new LevelEventType[2]
	{
		LevelEventType.Checkpoint,
		LevelEventType.Bookmark
	};

	private GameObject[] foundObjects;

	private int lastFrameUpdated = -1;

	private bool useAbsoluteArbitraryAngle;

	private bool mpWarned;

	private LevelEvent copiedTrackColor;

	private LevelEvent previousTrackColor;

	private LevelEvent copiedHitsound;

	private LevelEvent previousHitsound;

	private bool popupIsAnimating;

	private bool stallFileDialog;

	private int saveBackupLastFrame;

	public float playbackSpeed = 1f;

	public static bool applyEventsToFloorsOnPlay = true;

	private bool paused => scrController.instance.paused;

	public List<scrFloor> floors => customLevel.levelMaker.listFloors;

	public LevelData levelData => customLevel.levelData;

	public List<LevelEvent> events => levelData.levelEvents;

	public List<LevelEvent> decorations => levelData.decorations;

	public List<scrDecoration> scrDecorations => scrDecorationManager.instance.allDecorations;

	public bool playMode => !paused;

	public bool inStrictlyEditingMode { get; set; }

	public bool isLoading { get; set; }

	private bool highBPM => customLevel.highestBPM >= 300f;

	private bool isOldLevel => levelData.isOldLevel;

	private EditorSelectTarget currentSelectTarget => (EditorSelectTarget)levelData.miscSettings["selectTarget"];

	private bool userIsEditingAnInputField
	{
		get
		{
			GameObject currentSelectedGameObject = eventSystem.currentSelectedGameObject;
			if (currentSelectedGameObject != null)
			{
				return currentSelectedGameObject.GetComponent<InputField>() != null;
			}
			return false;
		}
	}

	private bool unsavedChanges
	{
		get
		{
			return _unsavedChanges;
		}
		set
		{
			_unsavedChanges = value;
			RefreshFilenameText();
		}
	}

	private void UpdateCanvasScalerResolution(float height)
	{
		height = Mathf.Clamp(height, 900f, Screen.height * 2);
		float num = (float)Screen.width * 1f / (float)Screen.height * height;
		canvasScaler.referenceResolution = new Vector2(num, height);
		ottoCanvas.GetComponent<CanvasScaler>().referenceResolution = canvasScaler.referenceResolution;
		Persistence.SetEditorScale(height);
	}

	private void Awake()
	{
		ottoBlinkCounter = 0;
		canvasScaler = GetComponent<CanvasScaler>();
		UpdateCanvasScalerResolution(Persistence.GetEditorScale());
		spritesDefaultShader = Shader.Find("Sprites/Default");
		defaultLockColor = lockBackground.color;
		defaultLockIconColor = lockIcon.color;
		lineMaterial = new Material(Shader.Find("ADOFAI/ScrollingSprite"));
		lineMaterial.SetTexture("_MainTex", floorConnectorTex);
		lineMaterial.SetVector("_ScrollSpeed", new Vector2(-0.4f, 0f));
		lineMaterial.SetFloat("_Time0", 0f);
		GCS.filteredEvent = LevelEventType.None;
		floorLayerMask = LayerMask.GetMask("Floor");
		handlesLayerMask = LayerMask.GetMask("Handles");
		camera = ADOBase.controller.camy.GetComponent<Camera>();
		preparedAudioClips = new Dictionary<string, AudioClip>();
		LoadEditorProperties();
		customLevel.levelData = new LevelData();
		levelData.Setup();
		ottoAudioSrc.ignoreListenerPause = true;
		interfaceAudioSrc.ignoreListenerPause = true;
		floorButtonCanvas.gameObject.SetActive(value: false);
		backupTimer = Time.unscaledTime;
		if (Persistence.GetDisableRewindButton())
		{
			rewind.gameObject.SetActive(value: false);
			playPause.transform.position -= new Vector3(38.5f, 0f);
		}
		lastTimeStatsUploaded = Time.unscaledTime;
	}

	private bool TryApplicationQuit()
	{
		if (unsavedChanges && !forceQuit)
		{
			if (playMode)
			{
				TogglePause();
			}
			CheckUnsavedChanges(delegate
			{
				ApplicationQuit();
			});
			return false;
		}
		return true;
	}

	private void ApplicationQuit()
	{
		forceQuit = true;
		Application.Quit();
	}

	private void CheckUnsavedChanges(Action quitLevelAction)
	{
		if (unsavedChanges)
		{
			ShowPopup(show: true, PopupType.UnsavedChanges);
			this.quitLevelAction = quitLevelAction;
		}
		else
		{
			quitLevelAction();
		}
	}

	private void SaveAndQuit()
	{
		SaveLevel();
		if (!string.IsNullOrEmpty(ADOBase.levelPath))
		{
			DoQuitAction();
		}
	}

	private void DoQuitAction()
	{
		ShowPopup(show: false);
		quitLevelAction();
	}

	private void TryQuitToMenu()
	{
		CheckUnsavedChanges(delegate
		{
			QuitToMenu();
		});
	}

	private void QuitToMenu()
	{
		Application.wantsToQuit -= TryApplicationQuit;
		if (GCS.customLevelPaths != null)
		{
			GCS.sceneToLoad = "scnCLS";
			scrUIController.instance.WipeToBlack(WipeDirection.StartsFromRight);
		}
		else
		{
			ADOBase.controller.QuitToMainMenu();
		}
	}

	private void RefreshFilenameText()
	{
		string text = (string.IsNullOrEmpty(ADOBase.levelPath) ? RDString.Get("editor.levelNotSaved") : Path.GetFileName(ADOBase.levelPath));
		if (unsavedChanges)
		{
			text += "*";
		}
		filenameText.text = text;
		filenameText.fontStyle = FontStyle.Bold;
	}

	private void Start()
	{
		Application.wantsToQuit += TryApplicationQuit;
		CloseAllPanels();
		TogglePause();
		if (!GCS.standaloneLevelMode)
		{
			UpdateSongAndLevelSettings();
		}
		levelEventsPanel.HideAllInspectorTabs();
		levelEventsPanel.ShowInspector(show: false);
		propertyControlList.OnItemSelected = OnDecorationSelected;
		propertyControlList.OnAllItemsDeselected = OnDecorationAllItemsDeselected;
		propertyHelpImage.ScaleXY(0f, 0f);
		popupPanel.SetActive(value: false);
		buttonNew.GetComponentInChildren<Text>().text = RDString.Get("editor.newLevel") + "<color=#00000077> (" + RDEditorUtils.KeyComboToString(control: true, shift: false, KeyCode.N) + ")</color>";
		buttonOpen.GetComponentInChildren<Text>().text = RDString.Get("editor.open") + "<color=#00000077> (" + RDEditorUtils.KeyComboToString(control: true, shift: false, KeyCode.O) + ")</color>";
		buttonOpenURL.GetComponentInChildren<Text>().text = RDString.Get("editor.openURL") + "<color=#00000077> (" + RDEditorUtils.KeyComboToString(control: true, shift: false, KeyCode.U) + ")</color>";
		buttonSave.GetComponentInChildren<Text>().text = RDString.Get("editor.save") + "<color=#00000077> (" + RDEditorUtils.KeyComboToString(control: true, shift: false, KeyCode.S) + ")</color>";
		buttonSaveAs.GetComponentInChildren<Text>().text = RDString.Get("editor.saveAs") + "<color=#00000077> (" + RDEditorUtils.KeyComboToString(control: true, shift: true, KeyCode.S) + ")</color>";
		buttonHelp.GetComponentInChildren<Text>().text = RDString.Get("editor.help") + "<color=#00000077> (" + RDEditorUtils.KeyComboToString(control: true, shift: false, KeyCode.H) + ")</color>";
		buttonDiscord.GetComponentInChildren<Text>().text = RDString.Get("editor.shareLevels");
		notificationOkButton.GetComponentInChildren<Text>().text = RDString.Get("editor.ok");
		shortcutTitle.text = RDString.Get("editor.shortcuts.KeyboardShortcuts");
		int num = 0;
		foreach (KeyValuePair<string, List<EditorConstants.EditorKeyShortcut>> editorShortcut in EditorConstants.editorShortcuts)
		{
			string key = editorShortcut.Key;
			List<EditorConstants.EditorKeyShortcut> value = editorShortcut.Value;
			GameObject gameObject = UnityEngine.Object.Instantiate(shortcutTabPrefab);
			gameObject.transform.SetParent(shortcutTabsContainer, worldPositionStays: false);
			gameObject.name = key;
			gameObject.GetComponentInChildren<Text>().text = RDString.Get("editor.shortcuts." + key);
			int copyIndex = num;
			gameObject.GetComponent<Button>().onClick.AddListener(delegate
			{
				ShowShortcutTab(copyIndex);
			});
			shortcutTabs.Add(gameObject);
			GameObject gameObject2 = UnityEngine.Object.Instantiate(shortcutContentPrefab);
			gameObject2.transform.SetParent(shortcutContentContainer, worldPositionStays: false);
			gameObject2.name = key;
			shortcutContent.Add(gameObject2);
			Transform child = gameObject2.transform.GetChild(0).GetChild(0);
			if (RDString.isCJK)
			{
				child.GetComponent<VerticalLayoutGroup>().spacing = 10f;
			}
			foreach (EditorConstants.EditorKeyShortcut item in value)
			{
				GameObject obj = UnityEngine.Object.Instantiate(shortcutTextPrefab);
				obj.transform.SetParent(child.transform, worldPositionStays: false);
				obj.name = item.key;
				string text = RDEditorUtils.KeyComboToString(item.usingCtrl, item.usingShift, item.usingAlt, item.keyCode);
				string text2 = string.Empty;
				if (item.otherKeyCode != 0)
				{
					EditorConstants.EditorKeyShortcut editorKeyShortcut = item;
					if (editorKeyShortcut.otherKeyDoesntUseModifierKeys)
					{
						editorKeyShortcut.usingCtrl = (editorKeyShortcut.usingAlt = (editorKeyShortcut.usingShift = false));
					}
					text2 = " " + RDString.Get("editor.shortcuts.or") + " " + RDEditorUtils.KeyComboToString(editorKeyShortcut.usingCtrl, editorKeyShortcut.usingShift, editorKeyShortcut.usingAlt, editorKeyShortcut.otherKeyCode);
				}
				obj.GetComponent<Text>().text = "<b>" + text + text2 + ":</b> " + RDString.Get("editor.shortcuts." + item.key);
			}
			Canvas.ForceUpdateCanvases();
			num++;
		}
		foreach (GameObject item2 in shortcutContent)
		{
			item2.gameObject.SetActive(value: false);
		}
		playPause.onClick.AddListener(delegate
		{
			Play();
		});
		rewind.onClick.AddListener(delegate
		{
			SelectFirstFloor();
			DeselectAnyUIGameObject();
		});
		buttonExit.onClick.AddListener(delegate
		{
			TryQuitToMenu();
		});
		buttonSettings.onClick.AddListener(delegate
		{
			ADOBase.controller.takeScreenshot.ShowPauseMenu(goToSettings: true);
		});
		SetupFindFloorPanel();
		buttonFileActionDropdown.onClick.AddListener(delegate
		{
			if (!OpenDirectory(customLevel.levelPath))
			{
				ToggleFileActionsPanel();
			}
		});
		buttonNew.onClick.AddListener(delegate
		{
			NewLevel();
		});
		buttonOpen.onClick.AddListener(delegate
		{
			OpenLevel();
		});
		buttonOpenRecent.onClick.AddListener(delegate
		{
			OpenRecent(checkCtrl: true);
		});
		buttonOpenURL.onClick.AddListener(delegate
		{
			ShowPopup(show: true, PopupType.OpenURL);
		});
		buttonSave.onClick.AddListener(delegate
		{
			SaveLevel();
		});
		buttonSaveAs.onClick.AddListener(delegate
		{
			SaveLevelAs();
		});
		buttonHelp.onClick.AddListener(delegate
		{
			ShowShortcutsPanel(show: true);
		});
		buttonCloseHelp.onClick.AddListener(delegate
		{
			ShowShortcutsPanel(show: false);
		});
		buttonDiscord.onClick.AddListener(delegate
		{
			OpenDiscord();
		});
		buttonD.onClick.AddListener(delegate
		{
			CreateFloorWithCharOrAngle(0f, 'R');
		});
		buttonE.onClick.AddListener(delegate
		{
			CreateFloorWithCharOrAngle(0f, 'E');
		});
		buttonW.onClick.AddListener(delegate
		{
			CreateFloorWithCharOrAngle(0f, 'U');
		});
		buttonQ.onClick.AddListener(delegate
		{
			CreateFloorWithCharOrAngle(0f, 'Q');
		});
		buttonA.onClick.AddListener(delegate
		{
			CreateFloorWithCharOrAngle(0f, 'L');
		});
		buttonZ.onClick.AddListener(delegate
		{
			CreateFloorWithCharOrAngle(0f, 'Z');
		});
		buttonS.onClick.AddListener(delegate
		{
			CreateFloorWithCharOrAngle(0f, 'D');
		});
		buttonC.onClick.AddListener(delegate
		{
			CreateFloorWithCharOrAngle(0f, 'C');
		});
		buttonT.onClick.AddListener(delegate
		{
			CreateFloorWithCharOrAngle(0f, 'T');
		});
		buttonG.onClick.AddListener(delegate
		{
			CreateFloorWithCharOrAngle(0f, 'G');
		});
		buttonF.onClick.AddListener(delegate
		{
			CreateFloorWithCharOrAngle(0f, 'F');
		});
		buttonB.onClick.AddListener(delegate
		{
			CreateFloorWithCharOrAngle(0f, 'B');
		});
		buttonBackQuoteT.onClick.AddListener(delegate
		{
			CreateFloorWithCharOrAngle(0f, 'o');
		});
		buttonBackQuoteY.onClick.AddListener(delegate
		{
			CreateFloorWithCharOrAngle(0f, 'q');
		});
		buttonBackQuoteV.onClick.AddListener(delegate
		{
			CreateFloorWithCharOrAngle(0f, 'V');
		});
		buttonBackQuoteB.onClick.AddListener(delegate
		{
			CreateFloorWithCharOrAngle(0f, 'Y');
		});
		buttonJ.onClick.AddListener(delegate
		{
			CreateFloorWithCharOrAngle(0f, 'J');
		});
		buttonH.onClick.AddListener(delegate
		{
			CreateFloorWithCharOrAngle(0f, 'H');
		});
		buttonN.onClick.AddListener(delegate
		{
			CreateFloorWithCharOrAngle(0f, 'N');
		});
		buttonM.onClick.AddListener(delegate
		{
			CreateFloorWithCharOrAngle(0f, 'M');
		});
		buttonBackQuoteJ.onClick.AddListener(delegate
		{
			CreateFloorWithCharOrAngle(0f, 'p');
		});
		buttonBackQuoteH.onClick.AddListener(delegate
		{
			CreateFloorWithCharOrAngle(0f, 'W');
		});
		buttonBackQuoteN.onClick.AddListener(delegate
		{
			CreateFloorWithCharOrAngle(0f, 'x');
		});
		buttonBackQuoteM.onClick.AddListener(delegate
		{
			CreateFloorWithCharOrAngle(0f, 'A');
		});
		buttonSpace.onClick.AddListener(delegate
		{
			CreateFloorWithCharOrAngle(ADOBase.lm.GetRotDirection(ADOBase.lm.GetRotDirection(selectedFloors[0].floatDirection, CW: true), CW: true), ADOBase.lm.GetRotDirection(ADOBase.lm.GetRotDirection(selectedFloors[0].stringDirection, CW: true), CW: true), pulseFloorButtons: true, fullSpin: true);
		});
		buttonTab.onClick.AddListener(delegate
		{
			CreateFloorWithCharOrAngle(999f, '!', pulseFloorButtons: true, fullSpin: true);
		});
		buttonAuto.onClick.AddListener(delegate
		{
			ToggleAuto();
		});
		buttonNextPage.onClick.AddListener(delegate
		{
			ShowNextPage();
		});
		buttonPrevPage.onClick.AddListener(delegate
		{
			ShowPrevPage();
		});
		buttonNoFail.onClick.AddListener(delegate
		{
			ToggleNoFail();
		});
		floorButtonArbitrary.onEndEdit.AddListener(delegate
		{
			bool successful;
			float arbitraryAngleFromField = GetArbitraryAngleFromField(out successful);
			floorButtonArbitrary.text = (successful ? arbitraryAngleFromField.ToString() : "");
		});
		popupSaveOk.onClick.AddListener(delegate
		{
			ShowPopup(show: false);
		});
		popupSaveSaveAs.onClick.AddListener(delegate
		{
			SaveLevelAs();
			ShowPopup(show: false);
		});
		popupURLDownload.onClick.AddListener(delegate
		{
			StartLevelDownload();
		});
		popupURLCancel.onClick.AddListener(delegate
		{
			CancelDownload();
		});
		popupCopyrightAccept.onClick.AddListener(delegate
		{
			AcceptAgreement();
		});
		popupCopyrightReturn.onClick.AddListener(delegate
		{
			DeclineAgreement();
		});
		popupParamsCancel.onClick.AddListener(delegate
		{
			ShowPopup(show: false, PopupType.MissingExportParams);
		});
		popupMissingFilesCancel.onClick.AddListener(delegate
		{
			ShowPopup(show: false, PopupType.MissingFiles);
		});
		popupOggCancel.onClick.AddListener(delegate
		{
			ShowPopup(show: false);
		});
		popupOggConvert.onClick.AddListener(delegate
		{
			StartCoroutine(ConvertSongToOggCo());
		});
		popupOkOk.onClick.AddListener(delegate
		{
			if (popupOkCallback == null)
			{
				ShowPopup(show: false);
			}
			else
			{
				popupOkCallback();
			}
		});
		popupUnsavedChangesCancel.onClick.AddListener(delegate
		{
			ShowPopup(show: false);
		});
		popupUnsavedChangesDiscard.onClick.AddListener(delegate
		{
			DoQuitAction();
		});
		popupUnsavedChangesSave.onClick.AddListener(delegate
		{
			SaveAndQuit();
		});
		floorConnectors = new GameObject();
		floorConnectors.name = "Floor Connector Lines";
		lineMaterial.DOFloat(lineMaterial.GetFloat("_Time0") + 10f, "_Time0", 10f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental)
			.SetUpdate(isIndependentUpdate: true);
		RefreshFilenameText();
		if (!Persistence.HasAcceptedAgreement())
		{
			ShowPopup(show: true, PopupType.CopyrightWarning, skipAnim: true);
		}
		if (!GCS.standaloneLevelMode)
		{
			webServices.LoadAllArtists();
			DiscordController.instance?.UpdatePresence();
			GCS.difficulty = Difficulty.Strict;
			ADOBase.uiController.difficultyContainer.gameObject.SetActive(value: false);
			editorDifficultySelector.gameObject.SetActive(value: true);
			autoImage.gameObject.SetActive(value: true);
			buttonNoFail.gameObject.SetActive(value: true);
		}
		initialized = true;
		ShowEventPicker(show: false);
	}

	private void UpdateSelectedFloor()
	{
		if (selectedFloors.Count != lastSelectedFloorsCount || (SelectionIsSingle() && lastSelectedFloor != selectedFloors[0]))
		{
			if (SelectionIsSingle())
			{
				lastSelectedFloor = selectedFloors[0];
			}
			lastSelectedFloorsCount = selectedFloors.Count;
			OnSelectedFloorChange();
		}
	}

	public void UpdateDecorationObjects()
	{
		customLevel.UpdateDecorationObjects();
		refreshDecSprites = false;
		propertyControlList.OnDecorationUpdate();
	}

	public void UpdateBackgroundSprites()
	{
		customLevel.UpdateBackgroundSprites();
		refreshBgSprites = false;
	}

	public void UpdateDecorationObject(LevelEvent e)
	{
		scrDecoration scrDecoration2 = scrDecorationManager.instance.allDecorations.Find((scrDecoration d) => d.sourceLevelEvent == e);
		if (scrDecoration2 != null)
		{
			scrDecoration2.Setup(e, out var _);
			scrDecoration2.UpdateHitbox();
			propertyControlList.OnDecorationUpdate();
		}
		else
		{
			printe("decoration not found: " + e);
		}
	}

	public void GoToDecoration(LevelEvent levelEvent)
	{
		scrDecoration decoration = scrDecorationManager.GetDecoration(levelEvent);
		DoCameraJump(decoration.transform.position.WithZ(-10f));
	}

	private void UpdateSteamCallbacks()
	{
		if (SteamIntegration.Instance.initialized)
		{
			SteamIntegration.Instance.CheckCallbacks();
			if (publishWindow.uploadInProgress)
			{
				SteamWorkshop.CheckUploadInfo();
			}
		}
	}

	public void UpdateImageLoadResult(string name, LoadResult loadResult)
	{
		if (isLoading)
		{
			if (!isUnauthorizedAccess && loadResult == LoadResult.UnauthorizedAccess)
			{
				isUnauthorizedAccess = true;
			}
			if (loadResult == LoadResult.UnauthorizedAccess || loadResult == LoadResult.MissingFile || loadResult == LoadResult.Error)
			{
				errorImageResult.Add(name, loadResult.ToString());
			}
		}
	}

	private void ShowImageLoadResult()
	{
		if (errorImageResult.Count == 0)
		{
			return;
		}
		string text = RDString.Get("editor.dialog.imageMessage") + " \n";
		string text2 = "";
		foreach (KeyValuePair<string, string> item in errorImageResult)
		{
			text2 = text2 + RDString.Get("editor.dialog.image" + item.Value) + ": " + item.Key + " \n";
		}
		text += text2;
		if (isUnauthorizedAccess)
		{
			text += RDString.Get("editor.dialog.imageUnauthorizedMessage");
		}
		ShowNotificationPopup(text);
	}

	private (PropertyControl_Toggle, PropertyControl_Text) GetFindPanelProps()
	{
		return ((PropertyControl_Toggle)findType.control, (PropertyControl_Text)findValue.control);
	}

	private void SetupFindFloorPanel()
	{
		var (typeControl, valueControl) = GetFindPanelProps();
		if (typeControl.buttons != null)
		{
			return;
		}
		findFloorPanelTitle.text = RDString.Get("editor.findFloor.title");
		Property[] array = new Property[2] { findType, findValue };
		foreach (Property property in array)
		{
			property.label.text = RDString.Get("editor.findFloor." + property.key);
		}
		typeControl.buttons = new Dictionary<string, Button>();
		Button[] componentsInChildren = findType.controlContainer.GetComponentsInChildren<Button>();
		for (int j = 0; j < componentsInChildren.Length; j++)
		{
			FindFloorType findFloorType = (FindFloorType)j;
			string curEnumStr = findFloorType.ToString();
			componentsInChildren[j].GetComponentInChildren<Text>().text = RDString.Get("enum.FindFloorType." + curEnumStr);
			typeControl.buttons.Add(curEnumStr, componentsInChildren[j]);
			componentsInChildren[j].onClick.AddListener(delegate
			{
				typeControl.text = curEnumStr;
				typeControl.selected = curEnumStr;
			});
		}
		valueControl.onEndEdit.AddListener(delegate (string str)
		{
			int result = 0;
			int.TryParse(str, out result);
			valueControl.text = result.ToString();
		});
		findButton.GetComponentInChildren<Text>().text = RDString.Get("editor.findFloor");
		findButton.onClick.AddListener(delegate
		{
			SelectBookmarkOrFloor();
		});
	}

	private void LateUpdate()
	{
		levelEditorCanvas.enabled = !playMode && !GCS.standaloneLevelMode && !ADOBase.controller.pauseMenu.gameObject.activeSelf;
		gameCanvas.enabled = playMode;
		playPause.interactable = floors.Count != 1;
		playPauseIcon.sprite = (paused ? playButtonIcon : pauseButtonIcon);
		buttonToggleAngleInput.gameObject.SetActive(!isOldLevel);
	}

	private void Update()
	{
		if (GCS.standaloneLevelMode || ADOBase.controller.pauseMenu.gameObject.activeSelf)
		{
			return;
		}
		thumbnailMaker.gameObject.SetActive(value: true);
		if (StandaloneFileBrowser.lastFrameCount == Time.frameCount)
		{
			return;
		}
		UpdateSteamCallbacks();
		UpdateSelectedFloor();
		OttoUpdate();
		if (refreshBgSprites)
		{
			UpdateBackgroundSprites();
		}
		if (refreshDecSprites)
		{
			UpdateDecorationObjects();
		}
		if (Input.GetKeyDown(KeyCode.Escape) && !GCS.standaloneLevelMode)
		{
			if (!paused)
			{
				SwitchToEditMode();
				return;
			}
			if (!SelectionIsEmpty() || !SelectionDecorationIsEmpty())
			{
				DeselectFloors();
				DeselectAllDecorations();
			}
			else
			{
				ToggleFileActionsPanel();
			}
		}
		if (Time.unscaledTime > backupTimer + backupInterval)
		{
			backupTimer = Time.unscaledTime;
			SaveBackup();
		}
		PointerEventData pointerData = ((CustomStandaloneInputModule)eventSystem.currentInputModule).GetPointerData();
		bool flag = false;
		if (pointerData != null && pointerData.pointerCurrentRaycast.module != null)
		{
			GameObject gameObject = pointerData.pointerCurrentRaycast.gameObject;
			if (gameObject != null)
			{
				Transform parent = gameObject.transform;
				do
				{
					if (parent.TryGetComponent<ScrollRect>(out var _))
					{
						flag = true;
						break;
					}
					parent = parent.parent;
				}
				while (parent != null);
			}
		}
		if (!flag)
		{
			Vector2 mouseScrollDelta = Input.mouseScrollDelta;
			scrCamera scrCamera2 = scrCamera.instance;
			if (Mathf.Abs(mouseScrollDelta.y) > 0.05f)
			{
				float value = scrCamera2.userSizeMultiplier - mouseScrollDelta.y * scrollSpeed;
				scrCamera2.userSizeMultiplier = Mathf.Clamp(value, 0.5f, 15f);
			}
		}
		if (playMode)
		{
			return;
		}
		bool flag2 = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		bool flag3 = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftMeta) || Input.GetKey(KeyCode.RightMeta);
		bool key = Input.GetKey(KeyCode.BackQuote);
		bool flag4 = flag2 || flag3;
		bool flag5 = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
		if (!userIsEditingAnInputField && !showingPopup && !Input.GetMouseButton(0))
		{
			if (Input.GetKeyDown(KeyCode.P) || (Input.GetKeyDown(KeyCode.Space) && !flag2))
			{
				Play();
			}
			else if (flag5)
			{
				if (Input.GetKeyDown(KeyCode.LeftArrow))
				{
					SelectBookmark(-1, selectRelative: true);
				}
				else if (Input.GetKeyDown(KeyCode.RightArrow))
				{
					SelectBookmark(1, selectRelative: true);
				}
				else if (Input.GetKeyDown(KeyCode.Semicolon))
				{
					ToggleFindFloorPanel();
				}
				else if (Input.GetKeyDown(KeyCode.B))
				{
					if (!SelectionIsEmpty())
					{
						if (levelData.levelEvents.Exists((LevelEvent e) => e.floor == selectedFloors[0].seqID && e.eventType == LevelEventType.Bookmark))
						{
							RemoveEventAtSelected(LevelEventType.Bookmark);
						}
						else
						{
							AddEventAtSelected(LevelEventType.Bookmark);
						}
						return;
					}
				}
				else if (flag3 && Input.GetKeyDown(KeyCode.L))
				{
					RDEditorUtils.OpenLogDirectory();
				}
			}
			else if (!flag4)
			{
				if (Input.GetKeyDown(KeyCode.LeftBracket))
				{
					ShowPrevPage();
				}
				else if (Input.GetKeyDown(KeyCode.RightBracket))
				{
					ShowNextPage();
				}
				else if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadPlus))
				{
					scrCamera obj = scrCamera.instance;
					float value2 = obj.userSizeMultiplier - 0.5f;
					obj.userSizeMultiplier = Mathf.Clamp(value2, 0.5f, 15f);
				}
				else if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
				{
					scrCamera obj2 = scrCamera.instance;
					float value3 = obj2.userSizeMultiplier + 0.5f;
					obj2.userSizeMultiplier = Mathf.Clamp(value3, 0.5f, 15f);
				}
				else if (Input.GetKeyDown(KeyCode.Home))
				{
					SelectFirstFloor();
				}
				else if (Input.GetKeyDown(KeyCode.End))
				{
					SelectFloor(floors[floors.Count() - 1]);
				}
				else if (Input.GetKeyDown(KeyCode.F1))
				{
					ShowPopup(show: true, PopupType.CopyrightWarning, skipAnim: true);
				}
				bool keyDown = Input.GetKeyDown(KeyCode.Backspace);
				bool keyDown2 = Input.GetKeyDown(KeyCode.Delete);
				if (!SelectionDecorationIsEmpty() && (keyDown || keyDown2))
				{
					DeleteMultiSelectionDecorations();
				}
				else if (SelectionIsSingle())
				{
					if (Input.GetKeyDown(KeyCode.LeftArrow))
					{
						SelectFloor(PreviousFloor(selectedFloors[0]));
					}
					else if (Input.GetKeyDown(KeyCode.RightArrow))
					{
						SelectFloor(NextFloor(selectedFloors[0]));
					}
					else if (Input.GetKeyDown(KeyCode.UpArrow))
					{
						levelEventsPanel.CycleTabs(next: false);
					}
					else if (Input.GetKeyDown(KeyCode.DownArrow))
					{
						levelEventsPanel.CycleTabs(next: true);
					}
					else if (keyDown || keyDown2)
					{
						DeleteSingleSelection(keyDown);
					}
					else if (Input.GetKeyDown(KeyCode.F5))
					{
						char chara = '5';
						for (int i = 0; i < 5; i++)
						{
							CreateFloorWithCharOrAngle(selectedFloors[0].floatDirection + 72f, chara);
						}
					}
					else if (Input.GetKeyDown(KeyCode.F7))
					{
						char chara2 = '7';
						for (int j = 0; j < 7; j++)
						{
							CreateFloorWithCharOrAngle(selectedFloors[0].floatDirection + 51.42857f, chara2);
						}
					}
					else
					{
						string inputString = Input.inputString;
						if (inputString != null && inputString.Length == 1 && inputString[0] >= '1' && inputString[0] <= '9')
						{
							if (!Application.isEditor)
							{
								int num = inputString[0] - 48;
								printe("number: " + num);
								foreach (LevelEventButton item in eventButtons[currentCategory])
								{
									if (item.keyCode == num && item.page == currentPage)
									{
										AddEventAtSelected(item.type);
										break;
									}
								}
							}
							else
							{
								RDBaseDll.printesw("Disabled keys 0-9 in editor because it's hard to debug with these shortcuts enabled");
							}
						}
					}
					if (lockPathEditing)
					{
						if (Input.GetKeyDown(KeyCode.A))
						{
							ToggleAuto();
						}
						else if (Input.GetKeyDown(KeyCode.N))
						{
							ToggleNoFail();
						}
					}
				}
				else if (!SelectionIsEmpty())
				{
					if (Input.GetKeyDown(KeyCode.LeftArrow))
					{
						SelectFloor(PreviousFloor(selectedFloors.First()));
					}
					if (Input.GetKeyDown(KeyCode.RightArrow))
					{
						SelectFloor(NextFloor(selectedFloors.Last()));
					}
					if (Input.GetKeyDown(KeyCode.Backspace))
					{
						DeleteMultiSelection();
					}
					else if (Input.GetKeyDown(KeyCode.Delete))
					{
						DeleteMultiSelection(backspace: false);
					}
					if (lockPathEditing)
					{
						if (Input.GetKeyDown(KeyCode.A))
						{
							ToggleAuto();
						}
						else if (Input.GetKeyDown(KeyCode.N))
						{
							ToggleNoFail();
						}
					}
				}
				else if (Input.GetKeyDown(KeyCode.A))
				{
					ToggleAuto();
				}
				else if (Input.GetKeyDown(KeyCode.N))
				{
					ToggleNoFail();
				}
				if (Input.GetKeyDown(KeyCode.Escape))
				{
					DeselectFloors();
					DeselectAllDecorations();
				}
				bool flag6 = RDEditorUtils.CheckPointerInObject(fileActionsPanel);
				if (!showingFileActions && hoveringFileActions != flag6)
				{
					if (hoveringFileActions = flag6)
					{
						fileIcon.color = Color.white.WithAlpha(0.5f);
						fileIcon.DOColor(Color.white.WithAlpha(1f), 0.25f).SetUpdate(isIndependentUpdate: true);
						fileArrow.color = Color.white.WithAlpha(0.5f);
						fileArrow.DOColor(Color.white.WithAlpha(1f), 0.25f).SetUpdate(isIndependentUpdate: true);
					}
					else
					{
						fileIcon.color = Color.white.WithAlpha(1f);
						fileIcon.DOColor(Color.white.WithAlpha(0.5f), 0.25f).SetUpdate(isIndependentUpdate: true);
						fileArrow.color = Color.white.WithAlpha(1f);
						fileArrow.DOColor(Color.white.WithAlpha(0.5f), 0.25f).SetUpdate(isIndependentUpdate: true);
					}
				}
				flag6 = RDEditorUtils.CheckPointerInObject(findFloorPanel);
				if (hoveringFindFloorPanel != flag6)
				{
					if (hoveringFindFloorPanel = flag6)
					{
						findArrow.color = Color.white.WithAlpha(0.5f);
						findArrow.DOColor(Color.white.WithAlpha(1f), 0.25f).SetUpdate(isIndependentUpdate: true);
					}
					else
					{
						findArrow.color = Color.white.WithAlpha(1f);
						findArrow.DOColor(Color.white.WithAlpha(0.5f), 0.25f).SetUpdate(isIndependentUpdate: true);
					}
				}
			}
			else if (flag2)
			{
				if (flag3)
				{
					if (Input.GetKeyDown(KeyCode.S))
					{
						DeselectAnyUIGameObject();
						SaveLevelAs();
					}
					else if (Input.GetKeyDown(KeyCode.O))
					{
						DeselectAnyUIGameObject();
						OpenRecent();
					}
					else if (Input.GetKeyDown(KeyCode.L))
					{
						if (!SelectionIsEmpty())
						{
							if (SelectionIsSingle())
							{
								FlipFloor(selectedFloors[0], horizontal: false);
							}
							else
							{
								FlipSelection(horizontal: false);
							}
						}
					}
					else if (Input.GetKeyDown(KeyCode.E))
					{
						if (SelectionIsSingle())
						{
							CopyTrackColor(selectedFloors[0].seqID);
						}
					}
					else if (Input.GetKeyDown(KeyCode.R))
					{
						if (SelectionIsSingle())
						{
							PasteTrackColor(selectedFloors[0].seqID);
						}
					}
					else if (Input.GetKeyDown(KeyCode.T))
					{
						if (SelectionIsSingle())
						{
							PasteTrackColorSingleTile(selectedFloors[0].seqID);
						}
					}
					else if (Input.GetKeyDown(KeyCode.Alpha1))
					{
						if (SelectionIsSingle())
						{
							CopyHitSound(selectedFloors[0].seqID);
						}
					}
					else if (Input.GetKeyDown(KeyCode.Alpha2))
					{
						if (SelectionIsSingle())
						{
							PasteHitsoundSingleTile(selectedFloors[0].seqID);
						}
					}
					else if (Input.GetKeyDown(KeyCode.F))
					{
						if (SelectionIsSingle())
						{
							ShowNotification("Currently selected floor is " + selectedFloors[0].seqID + "!");
							Debug.Log($"THIS IS FLOOR {selectedFloors[0].seqID} - entry time {selectedFloors[0].entryTime}");
						}
					}
					else if (Input.GetKeyDown(KeyCode.LeftArrow))
					{
						if (!SelectionIsEmpty())
						{
							if (SelectionIsSingle())
							{
								MultiSelectFloors(selectedFloors[0], floors[0], setSelectPoint: true);
							}
							else
							{
								MultiSelectFloors(floors[0], multiSelectPoint);
							}
						}
					}
					else if (Input.GetKeyDown(KeyCode.RightArrow))
					{
						if (!SelectionIsEmpty())
						{
							if (SelectionIsSingle())
							{
								MultiSelectFloors(selectedFloors[0], floors[floors.Count - 1], setSelectPoint: true);
							}
							else
							{
								MultiSelectFloors(floors[floors.Count - 1], multiSelectPoint);
							}
						}
					}
					else if (Input.GetKeyDown(KeyCode.C))
					{
						if (SelectionIsSingle())
						{
							CopyFloor(selectedFloors[0], clearClipboard: true, cut: false, selectedEventOnly: true);
						}
					}
					else if (Input.GetKeyDown(KeyCode.X))
					{
						if (SelectionIsSingle() && selectedFloors[0].seqID != 0)
						{
							CutFloor(selectedFloors[0], clearClipboard: true, selectedEventOnly: true);
						}
					}
					else if (Input.GetKeyDown(KeyCode.V))
					{
						if (clipboard.Any() && !SelectionIsEmpty())
						{
							if (SelectionIsSingle())
							{
								if (clipboard.Count() == 1)
								{
									if (clipboardContent == ClipboardContent.Floors)
									{
										List<LevelEvent> levelEventData = ((FloorData)clipboard[0]).levelEventData;
										PasteEvents(selectedFloors[0], levelEventData, overwrite: false);
									}
								}
								else if (clipboardContent == ClipboardContent.Floors)
								{
									int seqID = selectedFloors[0].seqID;
									PasteEvents(selectedFloors[0], ((FloorData)clipboard[0]).levelEventData, overwrite: false);
									for (int k = 1; k < clipboard.Count && seqID + k < floors.Count; k++)
									{
										PasteEvents(floors[seqID + k], ((FloorData)clipboard[k]).levelEventData, overwrite: false, selectAfterward: false);
									}
								}
							}
							else if (clipboard.Count() == 1 && selectedFloors[0].seqID != 0 && clipboardContent == ClipboardContent.Floors)
							{
								List<LevelEvent> levelEventData2 = ((FloorData)clipboard[0]).levelEventData;
								PasteEvents(selectedFloors[0], levelEventData2, overwrite: false);
							}
						}
					}
					else if (Input.GetKeyDown(KeyCode.Z))
					{
						Redo();
					}
				}
				else if (Input.GetKeyDown(KeyCode.LeftArrow))
				{
					if (!SelectionIsEmpty())
					{
						if (SelectionIsSingle() && selectedFloors[0].seqID != 0)
						{
							MultiSelectFloors(selectedFloors[0], floors[selectedFloors[0].seqID - 1], setSelectPoint: true);
						}
						else
						{
							MultiSelectFloors((selectedFloors[0].seqID < multiSelectPoint.seqID) ? floors[selectedFloors[0].seqID - ((selectedFloors[0].seqID != 0) ? 1 : 0)] : floors[selectedFloors.Last().seqID - 1], multiSelectPoint);
						}
					}
				}
				else if (Input.GetKeyDown(KeyCode.RightArrow))
				{
					if (!SelectionIsEmpty())
					{
						if (SelectionIsSingle() && selectedFloors[0].seqID != floors.Count - 1)
						{
							MultiSelectFloors(selectedFloors[0], floors[selectedFloors[0].seqID + 1], setSelectPoint: true);
						}
						else
						{
							MultiSelectFloors((selectedFloors.Last().seqID > multiSelectPoint.seqID) ? floors[selectedFloors.Last().seqID + ((selectedFloors.Last().seqID != floors.Count - 1) ? 1 : 0)] : floors[selectedFloors[0].seqID + 1], multiSelectPoint);
						}
					}
				}
				else if (Input.GetKeyDown(KeyCode.UpArrow))
				{
					if (SelectionIsSingle())
					{
						levelEventsPanel.CycleSelectedEventTab(next: true);
					}
				}
				else if (Input.GetKeyDown(KeyCode.DownArrow))
				{
					if (SelectionIsSingle())
					{
						levelEventsPanel.CycleSelectedEventTab(next: false);
					}
				}
				else if (Input.GetKeyDown(KeyCode.F5))
				{
					if (SelectionIsSingle())
					{
						char chara3 = '6';
						for (int l = 0; l < 5; l++)
						{
							CreateFloorWithCharOrAngle(selectedFloors[0].floatDirection - 72f, chara3);
						}
					}
				}
				else if (Input.GetKeyDown(KeyCode.F7))
				{
					if (SelectionIsSingle())
					{
						char chara4 = '8';
						for (int m = 0; m < 7; m++)
						{
							CreateFloorWithCharOrAngle(selectedFloors[0].floatDirection - 51.42857f, chara4);
						}
					}
				}
				else if (Input.GetKeyDown(KeyCode.Space) && SelectionIsSingle())
				{
					CreateFloorWithCharOrAngle(ADOBase.lm.GetRotDirection(ADOBase.lm.GetRotDirection(selectedFloors[0].floatDirection, CW: true), CW: true), ADOBase.lm.GetRotDirection(ADOBase.lm.GetRotDirection(selectedFloors[0].stringDirection, CW: true), CW: true), pulseFloorButtons: true, fullSpin: true);
				}
				else if (Input.GetKeyDown(KeyCode.LeftBracket))
				{
					ShowPrevPage(moveToFirst: true);
				}
				else if (Input.GetKeyDown(KeyCode.RightBracket))
				{
					ShowNextPage(moveToLast: true);
				}
			}
			else if (Input.GetKeyDown(KeyCode.C))
			{
				if (!SelectionIsEmpty())
				{
					if (SelectionIsSingle())
					{
						CopyFloor(selectedFloors[0]);
					}
					else
					{
						MultiCopyFloors();
					}
				}
				else if (!SelectionDecorationIsEmpty())
				{
					if (SelectionDecorationIsSingle())
					{
						CopyDecoration(selectedDecorations[0]);
					}
					else
					{
						MultiCopyDecorations();
					}
				}
			}
			else if (Input.GetKeyDown(KeyCode.X))
			{
				if (!SelectionIsEmpty())
				{
					if (SelectionIsSingle() && selectedFloors[0].seqID != 0)
					{
						CutFloor(selectedFloors[0]);
					}
					else
					{
						MultiCutFloors();
					}
				}
				else if (!SelectionDecorationIsEmpty())
				{
					if (SelectionDecorationIsSingle())
					{
						CutDecoration(selectedDecorations[0]);
					}
					else
					{
						MultiCutDecorations();
					}
				}
			}
			else if (Input.GetKeyDown(KeyCode.V))
			{
				if (clipboard.Any())
				{
					if (clipboardContent == ClipboardContent.Floors && !SelectionIsEmpty())
					{
						if (SelectionIsSingle())
						{
							if (clipboard.Count == 1)
							{
								List<LevelEvent> levelEventData3 = ((FloorData)clipboard[0]).levelEventData;
								PasteEvents(selectedFloors[0], levelEventData3);
							}
							else
							{
								PasteFloors();
							}
						}
						else if (clipboard.Count == 1)
						{
							if (selectedFloors[0].seqID != 0)
							{
								List<LevelEvent> levelEventData4 = ((FloorData)clipboard[0]).levelEventData;
								PasteEvents(selectedFloors[0], levelEventData4);
							}
						}
						else
						{
							DeleteMultiSelection();
							PasteFloors();
						}
					}
					else if (clipboardContent == ClipboardContent.Decorations)
					{
						PasteDecorations();
					}
				}
			}
			else if (Input.GetKeyDown(KeyCode.D))
			{
				if (!SelectionDecorationIsEmpty())
				{
					DuplicateDecorations();
				}
			}
			else if (Input.GetKeyDown(KeyCode.L))
			{
				if (!SelectionIsEmpty())
				{
					if (SelectionIsSingle())
					{
						FlipFloor(selectedFloors[0]);
					}
					else
					{
						FlipSelection();
					}
				}
			}
			else if (Input.GetKeyDown(KeyCode.Comma))
			{
				if (!SelectionIsEmpty())
				{
					if (SelectionIsSingle())
					{
						RotateFloor(selectedFloors[0], CW: false);
					}
					else
					{
						RotateSelection(CW: false);
					}
				}
			}
			else if (Input.GetKeyDown(KeyCode.Period))
			{
				if (!SelectionIsEmpty())
				{
					if (SelectionIsSingle())
					{
						RotateFloor(selectedFloors[0]);
					}
					else
					{
						RotateSelection();
					}
				}
			}
			else if (Input.GetKeyDown(KeyCode.Slash))
			{
				if (!SelectionIsEmpty())
				{
					if (SelectionIsSingle())
					{
						RotateFloor180(selectedFloors[0]);
					}
					else
					{
						RotateSelection180();
					}
				}
			}
			else if (Input.GetKeyDown(KeyCode.H))
			{
				ToggleShortcutsPanel();
			}
			else if (Input.GetKeyDown(KeyCode.F))
			{
				int num2 = (SelectionIsSingle() ? selectedFloors[0].seqID : (-1));
				showFloorNums = !showFloorNums;
				RemakePath();
				if (num2 != -1)
				{
					SelectFloor(floors[num2], cameraJump: false);
				}
			}
			else if (Input.GetKeyDown(KeyCode.U))
			{
				DeselectAnyUIGameObject();
				ShowPopup(show: true, PopupType.OpenURL);
			}
			else if (Input.GetKeyDown(KeyCode.O))
			{
				DeselectAnyUIGameObject();
				OpenLevel();
			}
			else if (Input.GetKeyDown(KeyCode.N))
			{
				DeselectAnyUIGameObject();
				NewLevel();
			}
			else if (Input.GetKeyDown(KeyCode.S))
			{
				DeselectAnyUIGameObject();
				SaveLevel();
			}
			else if (Input.GetKeyDown(KeyCode.Z))
			{
				Undo();
			}
			else if (Input.GetKeyDown(KeyCode.Y))
			{
				Redo();
			}
			else if (Input.GetKeyDown(KeyCode.Delete))
			{
				DeleteSubsequentFloors();
			}
			else if (Input.GetKeyDown(KeyCode.Backspace))
			{
				DeletePrecedingFloors();
			}
			else if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				SelectFirstFloor();
			}
			else if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				SelectFloor(floors[floors.Count() - 1]);
			}
			else if (Input.GetKeyDown(KeyCode.Minus))
			{
				ZoomOutUI();
			}
			else if (Input.GetKeyDown(KeyCode.Equals))
			{
				ZoomInUI();
			}
			if (!flag3)
			{
				if (Input.GetKeyDown(KeyCode.D))
				{
					CreateFloorWithCharOrAngle(0f, 'R');
				}
				else if (Input.GetKeyDown(KeyCode.W))
				{
					CreateFloorWithCharOrAngle(0f, 'U');
				}
				else if (Input.GetKeyDown(KeyCode.A))
				{
					CreateFloorWithCharOrAngle(0f, 'L');
				}
				else if (Input.GetKeyDown(KeyCode.S))
				{
					CreateFloorWithCharOrAngle(0f, 'D');
				}
				else if (Input.GetKeyDown(KeyCode.X))
				{
					CreateFloorWithCharOrAngle(0f, 'D');
				}
				else if (Input.GetKeyDown(KeyCode.E))
				{
					CreateFloorWithCharOrAngle(0f, 'E');
				}
				else if (Input.GetKeyDown(KeyCode.Q))
				{
					CreateFloorWithCharOrAngle(0f, 'Q');
				}
				else if (Input.GetKeyDown(KeyCode.Z))
				{
					CreateFloorWithCharOrAngle(0f, 'Z');
				}
				else if (Input.GetKeyDown(KeyCode.C))
				{
					CreateFloorWithCharOrAngle(0f, 'C');
				}
				else if (Input.GetKeyDown(KeyCode.Y))
				{
					CreateFloorWithCharOrAngle(0f, key ? 'o' : 'T');
				}
				else if (Input.GetKeyDown(KeyCode.T))
				{
					CreateFloorWithCharOrAngle(0f, key ? 'q' : 'G');
				}
				else if (Input.GetKeyDown(KeyCode.V))
				{
					CreateFloorWithCharOrAngle(0f, key ? 'V' : 'F');
				}
				else if (Input.GetKeyDown(KeyCode.B))
				{
					CreateFloorWithCharOrAngle(0f, key ? 'Y' : 'B');
				}
				else if (Input.GetKeyDown(KeyCode.J))
				{
					CreateFloorWithCharOrAngle(0f, key ? 'p' : 'J');
				}
				else if (Input.GetKeyDown(KeyCode.H))
				{
					CreateFloorWithCharOrAngle(0f, key ? 'W' : 'H');
				}
				else if (Input.GetKeyDown(KeyCode.N))
				{
					CreateFloorWithCharOrAngle(0f, key ? 'x' : 'N');
				}
				else if (Input.GetKeyDown(KeyCode.M))
				{
					CreateFloorWithCharOrAngle(0f, key ? 'A' : 'M');
				}
				else if (Input.GetKeyDown(KeyCode.Tab))
				{
					CreateFloorWithCharOrAngle(999f, '!', pulseFloorButtons: true, fullSpin: true);
				}
			}
		}
		if (Input.mouseScrollDelta != Vector2.zero)
		{
			ShowPropertyHelp(show: false);
		}
		floorButtonLeftRightCanvas.gameObject.SetActive((!flag2 || !key) && !freeAngleMode);
		floorButtonExtraCanvas.gameObject.SetActive(flag2 && !key && !freeAngleMode);
		floorButtonExtraBackQuoteCanvas.gameObject.SetActive(flag2 && key && !freeAngleMode);
		floorButtonPrimaryCanvas.gameObject.SetActive(!flag2 && !freeAngleMode);
		tabIndicator.SetKeyCode(flag2 ? KeyCode.Space : KeyCode.Tab);
		bool flag7 = EventSystem.current.IsPointerOverGameObject();
		if (!userIsEditingAnInputField && !showingPopup && !flag5 && !flag2)
		{
			speedIndicator.gameObject.SetActive(flag3);
		}
		if (Input.GetMouseButtonUp(0))
		{
			if (dragging)
			{
				if (draggedEvIndicator != null && draggedEvIndicator.editable)
				{
					float num3 = (float)draggedEvIndicator.floor.entryangle;
					float num4 = (0f - draggedEvIndicator.transform.rotation.eulerAngles.z) * (MathF.PI / 180f);
					double angleMoved = scrMisc.GetAngleMoved(num3, num4, !draggedEvIndicator.floor.isCCW);
					draggedEvIndicator.evnt.data["angleOffset"] = Mathf.Round((float)angleMoved * 57.29578f);
					ApplyEventsToFloors();
					levelEventsPanel.ShowPanelOfEvent(draggedEvIndicator.evnt);
					draggedEvIndicator.circle.color = Color.white;
					draggedEvIndicator = null;
				}
				if (isDraggingTiles)
				{
					_ = Vector3.zero;
					if (!SelectionIsEmpty())
					{
						if (SelectionIsSingle())
						{
							bool num5 = selectedFloors[0].seqID == 0;
							bool flag8 = selectedFloors[0].seqID == floors.Count - 1;
							LevelEvent levelEvent = new LevelEvent(selectedFloors[0].seqID, LevelEventType.PositionTrack);
							LevelEvent levelEvent2 = new LevelEvent(selectedFloors[0].seqID + 1, LevelEventType.PositionTrack);
							events.RemoveAll((LevelEvent x) => x.floor == selectedFloors[0].seqID && x.eventType == LevelEventType.PositionTrack);
							events.RemoveAll((LevelEvent x) => x.floor == selectedFloors[0].seqID + 1 && x.eventType == LevelEventType.PositionTrack);
							Vector3 vector = Vector3.zero;
							if (!num5)
							{
								vector = floors[selectedFloors[0].seqID - 1].transform.position - floors[selectedFloors[0].seqID - 1].startPos;
							}
							Vector3 vector2 = floors[selectedFloors[0].seqID].transform.position - floors[selectedFloors[0].seqID].startPos;
							Vector3 vector3 = Vector3.zero;
							Vector3 vector4 = Vector3.zero;
							if (!flag8)
							{
								vector3 = floors[selectedFloors[0].seqID].transform.position - floors[selectedFloors[0].seqID].startPos;
								vector4 = floors[selectedFloors[0].seqID + 1].transform.position - floors[selectedFloors[0].seqID + 1].startPos;
							}
							Vector3 vector5 = vector2 - vector;
							if (!flag2)
							{
								dragGridSize = (flag3 ? 0.707f : 0.5f);
								vector5 /= dragGridSize * customLevel.GetTileSize();
								vector5 = new Vector3(Mathf.Round(vector5.x), Mathf.Round(vector5.y), Mathf.Round(vector5.z));
								vector5 *= dragGridSize * customLevel.GetTileSize();
							}
							levelEvent.data["positionOffset"] = new Vector2(vector5.x / customLevel.GetTileSize(), vector5.y / customLevel.GetTileSize());
							levelEvent.data["rotation"] = selectedFloors[0].rotationOffset;
							levelEvent.data["scale"] = (int)(100f * selectedFloors[0].transform.localScale.x);
							levelEvent.data["opacity"] = (int)(100f * selectedFloors[0].floorRenderer.material.GetFloat("_Alpha"));
							if (!flag8)
							{
								Vector3 vector6 = vector4 - vector3;
								if (!flag2)
								{
									dragGridSize = (flag3 ? 0.707f : 0.5f);
									vector6 /= dragGridSize * customLevel.GetTileSize();
									vector6 = new Vector3(Mathf.Round(vector6.x), Mathf.Round(vector6.y), Mathf.Round(vector6.z));
									vector6 *= dragGridSize * customLevel.GetTileSize();
								}
								levelEvent2.data["positionOffset"] = new Vector2(vector6.x / customLevel.GetTileSize(), vector6.y / customLevel.GetTileSize());
								levelEvent2.data["rotation"] = floors[selectedFloors.Last().seqID + 1].rotationOffset;
								levelEvent2.data["scale"] = (int)(100f * floors[selectedFloors.Last().seqID + 1].transform.localScale.x);
								levelEvent2.data["opacity"] = (int)(100f * floors[selectedFloors.Last().seqID + 1].floorRenderer.material.GetFloat("_Alpha"));
							}
							events.Add(levelEvent);
							if (!flag8)
							{
								events.Add(levelEvent2);
							}
							int seqID2 = selectedFloors[0].seqID;
							RemakePath();
							Vector3 position = selectedFloors[0].transform.position;
							floorButtonCanvas.transform.position = new Vector2(position.x, position.y);
							SelectFloor(floors[seqID2]);
						}
						else
						{
							new Vector2(0f, 0f);
							new Vector2(0f, 0f);
							bool num6 = selectedFloors.First().seqID == 0;
							bool flag9 = selectedFloors.Last().seqID == floors.Count - 1;
							LevelEvent levelEvent3 = new LevelEvent(selectedFloors.First().seqID, LevelEventType.PositionTrack);
							LevelEvent levelEvent4 = new LevelEvent(selectedFloors.Last().seqID + 1, LevelEventType.PositionTrack);
							events.RemoveAll((LevelEvent x) => x.floor == selectedFloors.First().seqID && x.eventType == LevelEventType.PositionTrack);
							events.RemoveAll((LevelEvent x) => x.floor == selectedFloors.Last().seqID + 1 && x.eventType == LevelEventType.PositionTrack);
							Vector3 vector7 = Vector3.zero;
							if (!num6)
							{
								vector7 = floors[selectedFloors.First().seqID - 1].transform.position - floors[selectedFloors.First().seqID - 1].startPos;
							}
							Vector3 vector8 = floors[selectedFloors.First().seqID].transform.position - floors[selectedFloors.First().seqID].startPos;
							Vector3 vector9 = Vector3.zero;
							Vector3 vector10 = Vector3.zero;
							if (!flag9)
							{
								vector9 = floors[selectedFloors.Last().seqID].transform.position - floors[selectedFloors.Last().seqID].startPos;
								vector10 = floors[selectedFloors.Last().seqID + 1].transform.position - floors[selectedFloors.Last().seqID + 1].startPos;
							}
							Vector3 vector11 = vector8 - vector7;
							if (!flag2)
							{
								dragGridSize = (flag3 ? 0.707f : 0.5f);
								vector11 /= dragGridSize * customLevel.GetTileSize();
								vector11 = new Vector3(Mathf.Round(vector11.x), Mathf.Round(vector11.y), Mathf.Round(vector11.z));
								vector11 *= dragGridSize * customLevel.GetTileSize();
							}
							levelEvent3.data["positionOffset"] = new Vector2(vector11.x / customLevel.GetTileSize(), vector11.y / customLevel.GetTileSize());
							levelEvent3.data["rotation"] = floors[selectedFloors[0].seqID].rotationOffset;
							levelEvent3.data["scale"] = (int)(100f * floors[selectedFloors[0].seqID].transform.localScale.x);
							levelEvent3.data["opacity"] = (int)(100f * floors[selectedFloors[0].seqID].floorRenderer.material.GetFloat("_Alpha"));
							levelEvent3.data["editorOnly"] = ((!flag2) ? ToggleBool.Disabled : ToggleBool.Enabled);
							if (!flag9)
							{
								Vector3 vector12 = vector10 - vector9;
								if (!flag2)
								{
									dragGridSize = (flag3 ? 0.707f : 0.5f);
									vector12 /= dragGridSize * customLevel.GetTileSize();
									vector12 = new Vector3(Mathf.Round(vector12.x), Mathf.Round(vector12.y), Mathf.Round(vector12.z));
									vector12 *= dragGridSize * customLevel.GetTileSize();
								}
								levelEvent4.data["positionOffset"] = new Vector2(vector12.x / customLevel.GetTileSize(), vector12.y / customLevel.GetTileSize());
								levelEvent4.data["rotation"] = floors[selectedFloors[0].seqID + 1].rotationOffset;
								levelEvent4.data["scale"] = (int)(100f * floors[selectedFloors[0].seqID + 1].transform.localScale.x);
								levelEvent4.data["opacity"] = (int)(100f * floors[selectedFloors[0].seqID + 1].floorRenderer.material.GetFloat("_Alpha"));
								levelEvent4.data["editorOnly"] = ((!flag2) ? ToggleBool.Disabled : ToggleBool.Enabled);
							}
							events.Add(levelEvent3);
							if (!flag9)
							{
								events.Add(levelEvent4);
							}
							int seqID3 = selectedFloors.First().seqID;
							int seqID4 = selectedFloors.Last().seqID;
							RemakePath();
							MultiSelectFloors(floors[seqID3], floors[seqID4]);
							multiSelectPoint = floors[seqID3];
						}
					}
					isDraggingTiles = false;
				}
			}
			else if (!flag7)
			{
				if (freeAngleMode)
				{
					CreateFloor(freeAngle * 57.29578f);
				}
				else
				{
					Transform transform = ((ObjectsAtMouse() != null) ? SmartObjectSelect(backwardsCycling: true).transform : null);
					if (transform != null)
					{
						scrVisualDecoration component2;
						scrTextDecoration component3;
						if (transform.TryGetComponent<scrFloor>(out var floor))
						{
							if (flag2 && !SelectionIsEmpty())
							{
								if (SelectionIsSingle())
								{
									MultiSelectFloors(selectedFloors[0], floor, setSelectPoint: true);
								}
								else
								{
									_ = selectedFloors[0];
									selectedFloors.Last();
									if (floor == multiSelectPoint)
									{
										SelectFloor(floor);
									}
									else
									{
										MultiSelectFloors(floor, multiSelectPoint);
									}
								}
							}
							else
							{
								SelectFloor(floor);
								events.Count((LevelEvent x) => x.floor == floor.seqID);
							}
						}
						else if (transform.parent.TryGetComponent<scrVisualDecoration>(out component2))
						{
							SelectDecoration(component2.sourceLevelEvent);
						}
						else if (transform.parent.parent.TryGetComponent<scrTextDecoration>(out component3))
						{
							SelectDecoration(component3.sourceLevelEvent);
						}
					}
					else
					{
						DeselectFloors();
						DeselectAllDecorations();
					}
				}
			}
			if (ADOBase.isEditingLevel)
			{
				Analytics.editorMakingTime += Time.unscaledDeltaTime;
			}
			else
			{
				Analytics.editorPlayingTime += Time.unscaledDeltaTime;
			}
			if (Time.unscaledTime > lastTimeStatsUploaded + 120f)
			{
				Analytics.UploadStatsToSteam();
				lastTimeStatsUploaded = Time.unscaledTime;
			}
		}
		bool flag10 = !SelectionIsEmpty() && SelectionIsSingle() && !isOldLevel;
		freeAngleMode = Input.GetMouseButton(1) && flag10;
		if (freeAngleMode)
		{
			scrFloor scrFloor2 = selectedFloors[0];
			float orthographicSize = camera.orthographicSize;
			float num7 = camera.aspect * orthographicSize;
			Vector3 to = Input.mousePosition / Screen.height * camera.orthographicSize * 2f + camera.transform.position.WithZ(0f) - new Vector3(num7, orthographicSize) - scrFloor2.transform.position;
			freeAngle = Vector3.Angle(Vector3.up, to);
			if (to.x < 0f)
			{
				freeAngle = 360f - freeAngle;
			}
			if (!flag2)
			{
				freeAngle = Mathf.Round(freeAngle / 15f) * 15f;
			}
			freeAngle = (MathF.PI / 2f - freeAngle * (MathF.PI / 180f)) % (MathF.PI * 2f);
			float entryAngle = (MathF.PI / 2f - (float)scrFloor2.entryangle) % (MathF.PI * 2f);
			scrFloor2.floorRenderer.SetAngle(entryAngle, freeAngle);
			if (scrFloor2.seqID < floors.Count - 1)
			{
				scrFloor scrFloor3 = floors[scrFloor2.seqID + 1];
				float entryAngle2 = (freeAngle + MathF.PI) % (MathF.PI * 2f);
				float exitAngle = (MathF.PI / 2f - (float)scrFloor3.exitangle) % (MathF.PI * 2f);
				scrFloor3.floorRenderer.SetAngle(entryAngle2, exitAngle);
				Vector3 vector13 = new Vector3(1.5f * Mathf.Cos(freeAngle), 1.5f * Mathf.Sin(freeAngle)) + scrFloor2.startPos - scrFloor3.startPos;
				for (int n = scrFloor2.seqID + 1; n < floors.Count; n++)
				{
					scrFloor scrFloor4 = floors[n];
					scrFloor4.transform.position = scrFloor4.startPos + scrFloor4.offsetPos + vector13;
				}
			}
		}
		if (Input.GetMouseButtonUp(1) && flag10)
		{
			ADOBase.lm.RefreshAngles();
			ApplyEventsToFloors();
		}
		if (!paused)
		{
			return;
		}
		if (Input.GetMouseButtonUp(0))
		{
			ShowPropertyHelp(show: false);
		}
		if (Input.GetMouseButtonDown(0))
		{
			isDraggingTiles = false;
			isDraggingDecorations = false;
			draggingGizmo = GizmoAtMouse();
			if (draggingGizmo == null)
			{
				GameObject[] array = ObjectsAtMouse();
				Transform transform2 = ((array != null) ? SmartObjectSelect(backwardsCycling: true, dontIncrement: true).transform : null);
				if (!flag7 && transform2 != null)
				{
					if (flag5)
					{
						scrFloor component4 = (levelData.isOldLevel ? transform2.parent : transform2).GetComponent<scrFloor>();
						if (component4 != null && !SelectionIsEmpty())
						{
							if (SelectionIsSingle())
							{
								if (component4.seqID == selectedFloors[0].seqID)
								{
									isDraggingTiles = true;
								}
							}
							else if (component4.seqID >= selectedFloors.First().seqID && component4.seqID <= selectedFloors.Last().seqID)
							{
								isDraggingTiles = true;
							}
						}
					}
					else if (transform2.parent.GetComponent<scrDecoration>() != null && !SelectionDecorationIsEmpty())
					{
						isDraggingDecorations = true;
						printe("Mouse down on selected decoration");
					}
				}
				if (isDraggingTiles)
				{
					floorPositionsAtDragStart.Clear();
					if (SelectionIsSingle())
					{
						floorPositionsAtDragStart[selectedFloors[0]] = selectedFloors[0].transform.position;
					}
					else
					{
						foreach (scrFloor selectedFloor in selectedFloors)
						{
							floorPositionsAtDragStart[selectedFloor] = selectedFloor.transform.position;
						}
					}
				}
				else if (isDraggingDecorations)
				{
					decorationPositionsAtDragStart.Clear();
					foreach (LevelEvent selectedDecoration in selectedDecorations)
					{
						scrDecoration decoration = scrDecorationManager.GetDecoration(selectedDecoration);
						decorationPositionsAtDragStart[decoration] = decoration.transform.position;
					}
				}
				mousePosition0 = Input.mousePosition;
				cameraPositionAtDragStart = ADOBase.controller.camy.transform.position;
				bool flag11 = false;
				if (array != null)
				{
					GameObject[] array2 = array;
					for (int num8 = 0; num8 < array2.Length; num8++)
					{
						EventIndicator component5 = array2[num8].transform.parent.GetComponent<EventIndicator>();
						if (component5 != null)
						{
							draggedEvIndicator = component5;
							flag11 = true;
							break;
						}
					}
				}
				if (!flag11)
				{
					draggedEvIndicator = null;
				}
				if (draggedEvIndicator != null)
				{
					if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
					{
						LevelEvent evnt = draggedEvIndicator.evnt;
						EnableEvent(evnt, !evnt.active);
						cancelDrag = true;
						printe("event is now enabled: " + base.enabled);
					}
					else
					{
						evIndPosAtDragStart = draggedEvIndicator.circle.transform.position;
						levelEventsPanel.ShowPanelOfEvent(draggedEvIndicator.evnt);
						if (draggedEvIndicator.editable)
						{
							draggedEvIndicator.circle.color = new Color(0.9f, 0.9f, 0.9f);
						}
					}
				}
			}
			if (flag7 || freeAngleMode)
			{
				cancelDrag = true;
			}
			if (!RDEditorUtils.CheckPointerInObject(fileActionsPanel))
			{
				ShowFileActionsPanel(show: false);
			}
			if (!RDEditorUtils.CheckPointerInObject(shortcutsPanel))
			{
				ShowShortcutsPanel(show: false);
			}
			if (!RDEditorUtils.CheckPointerInObject(findFloorPanel) || RDEditorUtils.CheckPointerInObject(findArrow))
			{
				ShowFindFloorPanel(show: false);
			}
		}
		else if (Input.GetMouseButton(0))
		{
			Vector3 vector14 = (Input.mousePosition - mousePosition0) / Screen.height * camera.orthographicSize * 2f;
			Vector3 vector15 = Vector3.zero;
			if (!cancelDrag)
			{
				if (draggedEvIndicator == null)
				{
					if (!isDraggingTiles)
					{
						vector15 = cameraPositionAtDragStart - vector14;
						camera.transform.position = new Vector3(vector15.x, vector15.y, -10f);
					}
					else if (isDraggingTiles)
					{
						if (SelectionIsSingle())
						{
							scrFloor scrFloor5 = selectedFloors[0];
							Vector3 vector16 = floorPositionsAtDragStart[scrFloor5] + vector14;
							scrFloor5.transform.position = new Vector3(vector16.x, vector16.y, scrFloor5.transform.position.z);
						}
						else
						{
							foreach (scrFloor selectedFloor2 in selectedFloors)
							{
								Vector3 vector17 = floorPositionsAtDragStart[selectedFloor2] + vector14;
								selectedFloor2.transform.position = new Vector3(vector17.x, vector17.y, selectedFloor2.transform.position.z);
							}
						}
					}
				}
				else if (draggedEvIndicator.editable)
				{
					vector15 = evIndPosAtDragStart + vector14;
					Vector3 to2 = vector15 - draggedEvIndicator.gameObject.transform.position;
					float num9 = Vector3.Angle(Vector3.up, to2);
					if (to2.x < 0f)
					{
						num9 = 360f - num9;
					}
					num9 *= MathF.PI / 180f;
					scrFloor floor2 = draggedEvIndicator.floor;
					double num10 = scrMisc.GetAngleMoved((float)floor2.entryangle, num9, !floor2.isCCW);
					double num11 = scrMisc.GetAngleMoved((float)floor2.entryangle, (float)floor2.exitangle, !floor2.isCCW);
					if (Mathf.Abs((float)num11) <= Mathf.Pow(10f, -6f))
					{
						num11 = 6.2831854820251465;
						if (floor2.midSpin)
						{
							num10 = 0.0;
						}
					}
					double num12 = num10 / num11;
					if (num12 > 1.0)
					{
						double num13 = (Math.PI * 2.0 - num11) / 2.0;
						double num14 = (num11 + num13 + Math.PI) % (Math.PI * 2.0);
						double num15 = num11 + Math.PI;
						double num16 = (num10 + Math.PI) % (Math.PI * 2.0);
						num12 = ((!(num16 > num14) || !(num16 < num15)) ? 1.0 : 0.0);
					}
					double entryangle = floor2.entryangle;
					double num17 = floor2.entryangle + num11 * (double)(floor2.isCCW ? 1 : (-1));
					float num18 = Mathf.Lerp((float)entryangle, (float)num17, (float)num12);
					num18 = (num18 - 2f * (float)floor2.entryangle) * 57.29578f;
					if (!flag2)
					{
						num18 = (float)Mathf.RoundToInt(num18 / 15f) * 15f;
					}
					draggedEvIndicator.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, num18);
				}
			}
			if (vector15 != cameraPositionAtDragStart && !freeAngleMode)
			{
				dragging = true;
			}
		}
		else
		{
			dragging = false;
			cancelDrag = false;
		}
	}

	private int GetBookmarkInDirection(int seqID, bool isDirectionLeft = false, int moveAmount = 1)
	{
		List<int> list = (from e in levelData.levelEvents
						  where e.eventType == LevelEventType.Bookmark && e.active
						  select e.floor).ToList();
		list.Sort();
		if (!list.Contains(0))
		{
			list.Insert(0, 0);
		}
		if (!list.Contains(floors.Count - 1))
		{
			list.Add(floors.Count - 1);
		}
		if (!list.Contains(seqID))
		{
			int index = Math.Max(list.FindLastIndex((int b) => b < seqID) + 1, 0);
			list.Insert(index, seqID);
		}
		int index2 = Math.Max(Math.Min(list.FindIndex((int b) => b == seqID) + (isDirectionLeft ? (-moveAmount) : moveAmount), list.Count - 1), 0);
		return list[index2];
	}

	private void SelectBookmark(int bookmarkIndex, bool selectRelative)
	{
		int index = ((bookmarkIndex >= 0) ? (floors.Count - 1) : 0);
		if (selectRelative)
		{
			if (!SelectionIsEmpty())
			{
				int moveAmount = Math.Abs(bookmarkIndex);
				int seqID = selectedFloors[0].seqID;
				if (!SelectionIsSingle())
				{
					seqID = ((bookmarkIndex < 0) ? selectedFloors[0] : selectedFloors.Last()).seqID;
				}
				index = GetBookmarkInDirection(seqID, bookmarkIndex < 0, moveAmount);
			}
		}
		else
		{
			index = GetBookmarkInDirection(0, isDirectionLeft: false, bookmarkIndex);
		}
		SelectFloor(floors[index]);
	}

	private void ShowFindFloorPanel(bool show)
	{
		showingFindFloorPanel = show;
		RectTransform rt = findFloorPanel.GetComponent<RectTransform>();
		float endValue = (show ? findFloorPanelOpenHeight : findFloorPanelCloseHeight);
		rt.DOKill();
		rt.DOAnchorPosY(endValue, UIPanelEaseDur).SetUpdate(isIndependentUpdate: true).SetEase(UIPanelEaseMode)
			.OnComplete(delegate
			{
				if (!show)
				{
					rt.gameObject.SetActive(value: false);
				}
			});
		if (show)
		{
			rt.gameObject.SetActive(value: true);
			ShowShortcutsPanel(show: false);
		}
	}

	private void ToggleFindFloorPanel()
	{
		ShowFindFloorPanel(!showingFindFloorPanel);
	}

	public void CloseAllPanels(GameObject excludedPanel = null)
	{
		if (excludedPanel != fileActionsPanel)
		{
			ShowFileActionsPanel(show: false);
		}
		if (excludedPanel != shortcutsPanel)
		{
			ShowShortcutsPanel(show: false);
		}
		if (excludedPanel != findFloorPanel)
		{
			ShowFindFloorPanel(show: false);
		}
	}

	public void CloseAllInspectors()
	{
		settingsPanel.ShowInspector(show: false);
		levelEventsPanel.ShowInspector(show: false);
	}

	private void SelectBookmarkOrFloor()
	{
		(PropertyControl_Toggle, PropertyControl_Text) findPanelProps = GetFindPanelProps();
		PropertyControl_Toggle item = findPanelProps.Item1;
		PropertyControl_Text item2 = findPanelProps.Item2;
		int num = 0;
		if (int.TryParse(item2.text, out var result))
		{
			num = Math.Max(0, Math.Min(result, floors.Count - 1));
		}
		if (item.text.Equals(FindFloorType.Floor.ToString()))
		{
			SelectFloor(floors[num]);
		}
		else
		{
			SelectBookmark(num, selectRelative: false);
		}
	}

	public void EnableEvent(LevelEvent e, bool enabled)
	{
		e.active = enabled;
		ApplyEventsToFloors();
	}

	public void ShowEvent(LevelEvent e, bool visible)
	{
		e.visible = visible;
		UpdateEventVisibility(e);
	}

	public void ForceHideEvent(LevelEvent e, bool forceHide)
	{
		e.forceHide = forceHide;
		UpdateEventVisibility(e);
	}

	private void UpdateEventVisibility(LevelEvent e)
	{
		scrDecorationManager.instance.allDecorations.Find((scrDecoration d) => d.sourceLevelEvent == e).SetVisible(e.visible && !e.forceHide);
	}

	public void SelectDecoration(int itemIndex, bool jumpToDecoration = true, bool showPanel = true, bool ignoreDeselection = false, bool ignoreAdjustRect = false)
	{
		LevelEvent sourceLevelEvent = scrDecorationManager.GetDecoration(itemIndex).sourceLevelEvent;
		if (sourceLevelEvent != null)
		{
			SelectDecoration(sourceLevelEvent, jumpToDecoration, showPanel, ignoreDeselection, ignoreAdjustRect);
		}
	}

	public void SelectDecoration(LevelEvent levelEvent, bool jumpToDecoration = true, bool showPanel = true, bool ignoreDeselection = false, bool ignoreAdjustRect = false)
	{
		using (new SaveStateScope(ADOBase.editor))
		{
			bool flag = selectedDecorations.Contains(levelEvent);
			if (flag && RDInput.holdingControl && !ignoreDeselection)
			{
				DeselectDecoration(levelEvent);
				return;
			}
			if (!RDInput.holdingShift && !RDInput.holdingControl && !ignoreDeselection)
			{
				DeselectAllDecorations();
				DeselectFloors();
				flag = false;
			}
			if (!(scrDecorationManager.GetDecoration(levelEvent) == null))
			{
				if (!flag)
				{
					selectedDecorations.Add(levelEvent);
				}
				if (jumpToDecoration)
				{
					ADOBase.editor.GoToDecoration(levelEvent);
				}
				scrDecorationManager.instance.ShowSelectionBorders(levelEvent);
				int decorationIndex = scrDecorationManager.GetDecorationIndex(levelEvent);
				if (showPanel)
				{
					levelEventsPanel.ShowInspector(show: true, forceAction: true);
					levelEventsPanel.ShowPanel(levelEvent.eventType, decorationIndex);
				}
				propertyControlList.lastSelectedIndex = decorationIndex;
				propertyControlList.UpdateItemList();
				if (!ignoreAdjustRect)
				{
					propertyControlList.AdjustItemListScrollRect(levelEvent);
				}
				if (propertyControlList.OnItemSelected != null)
				{
					propertyControlList.OnItemSelected(levelEvent);
				}
			}
		}
	}

	public void DeselectDecoration(LevelEvent levelEvent)
	{
		using (new SaveStateScope(ADOBase.editor))
		{
			if (selectedDecorations.Count <= 1)
			{
				DeselectAllDecorations();
			}
			else if (!(scrDecorationManager.GetDecoration(levelEvent) == null))
			{
				scrDecorationManager.instance.ShowSelectionBorders(levelEvent, show: false);
				selectedDecorations.Remove(levelEvent);
				LevelEvent levelEvent2 = selectedDecorations[selectedDecorations.Count - 1];
				SelectDecoration(levelEvent2, jumpToDecoration: false, showPanel: true, ignoreDeselection: true);
			}
		}
	}

	public void DeselectAllDecorations()
	{
		using (new SaveStateScope(ADOBase.editor))
		{
			levelEventsPanel.ShowInspector(show: false);
			scrDecorationManager.instance.ClearDecorationBorders();
			int count = selectedDecorations.Count;
			selectedDecorations.Clear();
			propertyControlList.UpdateItemList();
			if (count > 0)
			{
				levelEventsPanel.HideAllInspectorTabs();
				if (propertyControlList.OnAllItemsDeselected != null)
				{
					propertyControlList.OnAllItemsDeselected();
				}
			}
		}
	}

	public void SwitchToEditMode(bool clsToEditor = false)
	{
		GCS.standaloneLevelMode = false;
		GCS.speedTrialMode = false;
		GCS.editorQuickPitchedPlaying = false;
		GCS.practiceMode = false;
		GCS.currentSpeedTrial = 1f;
		inStrictlyEditingMode = true;
		if (EditorWebServices.artists == null)
		{
			webServices.LoadAllArtists();
		}
		mpWarned = false;
		scrFlash.FlashKill();
		ADOBase.conductor.song.Stop();
		ADOBase.conductor.KillAllSounds();
		ADOBase.conductor.gameObject.SetActive(value: false);
		ADOBase.conductor.song.pitch = (int)levelData.songSettings["pitch"] / 100;
		int num = Math.Min(ADOBase.controller.currentSeqID, floors.Count - 1);
		ADOBase.controller.camy.GetComponent<Grayscale>().enabled = false;
		scrUIController.instance.SetToTransparent();
		ADOBase.uiController.difficultyContainer.gameObject.SetActive(value: false);
		ADOBase.uiController.modifiersContainer.gameObject.SetActive(value: false);
		TogglePause(clsToEditor);
		ClearFloorGlows();
		int index = (RDInput.holdingShift ? num : selectedFloorCached);
		if (decorationWasSelected)
		{
			for (int i = 0; i < lastSelectedDecorations.Count; i++)
			{
				SelectDecoration(lastSelectedDecorations[i], i == lastSelectedDecorations.Count - 1);
			}
		}
		else
		{
			SelectFloor(floors[index], cameraJump: false);
		}
		ADOBase.controller.gameworld = true;
		DrawHolds(unfillHolds: true);
		DrawFloorOffsetLines();
		DrawFloorNums();
		DrawMultiPlanet();
		lineMaterial.DOFloat(lineMaterial.GetFloat("_Time0") + 10f, "_Time0", 10f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental)
			.SetUpdate(isIndependentUpdate: true);
		foreach (scrPlanet dummyPlanet in scrController.instance.dummyPlanets)
		{
			dummyPlanet.DisableParticles();
		}
		scrFloor scrFloor2 = floors[num];
		if (Vector2.Distance(camera.transform.position, scrFloor2.transform.position) > 3f)
		{
			Vector3 endValue = scrFloor2.transform.position.WithZ(-10f);
			camera.transform.DOKill();
			camera.transform.DOMove(endValue, 0.6f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutExpo);
		}
		editorDifficultySelector.gameObject.SetActive(value: true);
		editorDifficultySelector.SetChangeable(changeable: true);
		autoImage.gameObject.SetActive(value: true);
		buttonNoFail.gameObject.SetActive(value: true);
		buttonNoFail.interactable = true;
		buttonNoFail.GetComponent<RectTransform>().DOScale(Vector3.one, 0.25f).SetEase(Ease.OutQuad)
			.SetUpdate(isIndependentUpdate: true);
		buttonNoFail.GetComponent<Image>().color = (ADOBase.controller.noFail ? Color.white : grayColor);
		scrDecorationManager.instance.ShowEmptyDecorations(show: true);
		scrController.checkpointsUsed = 0;
		ADOBase.controller.startedFromCheckpoint = false;
		ADOBase.controller.mistakesManager.Reset();
		if (!Cursor.visible)
		{
			Cursor.visible = true;
		}
	}

	private GameObject[] ObjectsAtMouse()
	{
		if (Time.frameCount != lastFrameUpdated)
		{
			lastFrameUpdated = Time.frameCount;
			Vector3 mousePosition = Input.mousePosition;
			Vector2 vector = camera.ScreenToWorldPoint(mousePosition).xy();
			float magnitude = new Vector2(0.75f, 0.4125f).magnitude;
			List<Collider2D> list = new List<Collider2D>();
			List<GameObject> list2 = new List<GameObject>();
			foreach (scrFloor floor in floors)
			{
				if (Vector2.Distance(floor.transform.position.xy(), vector) <= magnitude)
				{
					FloorMesh component = floor.GetComponent<FloorMesh>();
					if (component != null)
					{
						component.GenerateCollider();
						component.polygonCollider.enabled = true;
						list.Add(component.polygonCollider);
					}
					if (floor.GetComponent<FloorSpriteRenderer>() != null)
					{
						GameObject gameObject = floor.GenerateCollider();
						gameObject.name = "Parent";
						list.Add(gameObject.GetComponent<Collider2D>());
						list2.Add(gameObject);
					}
				}
			}
			RaycastHit2D[] array = Physics2D.RaycastAll(vector, Vector2.zero, 0f, floorLayerMask);
			if (array.Length == 0 || array == null)
			{
				foundObjects = null;
			}
			else
			{
				foundObjects = new GameObject[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					Collider2D collider = array[i].collider;
					foundObjects[i] = ((collider.name == "Parent") ? collider.transform.parent.gameObject : collider.gameObject);
				}
				Array.Sort(foundObjects, delegate (GameObject x, GameObject y)
				{
					Renderer componentInParent = x.GetComponentInParent<Renderer>();
					Renderer componentInParent2 = y.GetComponentInParent<Renderer>();
					Canvas componentInParent3 = x.GetComponentInParent<Canvas>();
					Canvas componentInParent4 = y.GetComponentInParent<Canvas>();
					int value = ((componentInParent != null) ? componentInParent.sortingOrder : componentInParent3.sortingOrder);
					return ((componentInParent2 != null) ? componentInParent2.sortingOrder : componentInParent4.sortingOrder).CompareTo(value);
				});
			}
			foreach (Collider2D item in list)
			{
				item.enabled = false;
			}
			foreach (GameObject item2 in list2)
			{
				UnityEngine.Object.DestroyImmediate(item2);
			}
		}
		return foundObjects;
	}

	private TransformGizmo GizmoAtMouse()
	{
		Vector3 mousePosition = Input.mousePosition;
		RaycastHit2D[] array = Physics2D.RaycastAll(camera.ScreenToWorldPoint(mousePosition).xy(), Vector2.zero, 0f, handlesLayerMask);
		TransformGizmo component = null;
		if (array.Length != 0)
		{
			array?[0].collider.TryGetComponent<TransformGizmo>(out component);
		}
		return component;
	}

	private GameObject SmartObjectSelect(bool backwardsCycling, bool dontIncrement = false)
	{
		GameObject[] array = ObjectsAtMouse();
		if (dontIncrement)
		{
			if (array.Length == 0)
			{
				return null;
			}
			return array[0];
		}
		bool flag = false;
		if (previouslyFoundObjects != null)
		{
			if (previouslyFoundObjects.Length == array.Length)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (previouslyFoundObjects[i] != array[i])
					{
						flag = true;
						break;
					}
				}
			}
			else
			{
				flag = true;
			}
		}
		if (flag)
		{
			selectedObjectIndexOfBunch = 0;
			previouslyFoundObjects = array;
		}
		previouslyFoundObjects = array;
		GameObject result = array[selectedObjectIndexOfBunch];
		selectedObjectIndexOfBunch = (selectedObjectIndexOfBunch + 1) % array.Length;
		return result;
	}

	public void FilterEventType(LevelEventType eventType)
	{
		foreach (KeyValuePair<LevelEventCategory, List<LevelEventButton>> eventButton in eventButtons)
		{
			foreach (LevelEventButton item in eventButton.Value)
			{
				if (item.type != eventType)
				{
					item.ShowAsFiltered(filtered: false);
				}
			}
		}
		if (filteredEventType != eventType)
		{
			filteredEventType = eventType;
		}
		else
		{
			filteredEventType = LevelEventType.None;
		}
		GCS.filteredEvent = filteredEventType;
		ApplyEventsToFloors();
	}

	public void OnSelectedFloorChange()
	{
		if (SelectionIsSingle())
		{
			scrFloor scrFloor2 = selectedFloors[0];
			levelEventsPanel.ShowTabsForFloor(scrFloor2.seqID);
			UpdateFloorDirectionButtons(active: true);
			ShowEventPicker(scrFloor2.seqID != 0);
			ShowEventIndicators(scrFloor2);
		}
		else
		{
			if (SelectionIsEmpty() && paused)
			{
				DeselectFloors();
			}
			levelEventsPanel.ShowInspector(show: false);
			levelEventsPanel.HideAllInspectorTabs();
			ShowEventPicker(show: false);
			UpdateFloorDirectionButtons(active: false);
			DestroyEventIndicators();
			selectedObjectIndexOfBunch = 0;
		}
		HidePopupBlocker();
	}

	public void ShowEventIndicators(scrFloor floor)
	{
		if (floor == null)
		{
			return;
		}
		DestroyEventIndicators();
		bool flag = false;
		int num = 0;
		foreach (LevelEvent ev in events.FindAll((LevelEvent x) => x.floor == floor.seqID))
		{
			if (ev.data.Keys.Contains("angleOffset"))
			{
				if (!Array.Exists(EditorConstants.soloTypes, (LevelEventType element) => element == ev.eventType))
				{
					UnityEngine.Object.Instantiate(ADOBase.gc.prefab_eventIndicator, floor.transform).GetComponent<EventIndicator>().Init(ev, floor, num);
					flag = true;
				}
				num++;
			}
		}
		if (flag)
		{
			EventCircle.gameObject.SetActive(value: true);
			EventCircle.fillClockwise = !floor.isCCW;
			EventCircle.transform.rotation = Quaternion.Euler(0f, 0f, (0f - (float)floor.entryangle) * 57.29578f);
			double angleMoved = scrMisc.GetAngleMoved((float)floor.entryangle, (float)floor.exitangle, !floor.isCCW);
			if (Mathf.Abs((float)angleMoved) <= Mathf.Pow(10f, -6f))
			{
				EventCircle.fillAmount = 1f;
			}
			else
			{
				EventCircle.fillAmount = (float)angleMoved / (MathF.PI * 2f);
			}
		}
	}

	private void DestroyEventIndicators()
	{
		EventCircle.gameObject.SetActive(value: false);
		GameObject[] array = GameObject.FindGameObjectsWithTag("EventIndicator");
		for (int i = 0; i < array.Length; i++)
		{
			UnityEngine.Object.Destroy(array[i]);
		}
	}

	public void ShowFileActionsPanel(bool show)
	{
		showingFileActions = show;
		float alpha = (show ? 1f : 0.6f);
		fileIcon.DOColor(Color.white.WithAlpha(alpha), UIPanelEaseDur).SetUpdate(isIndependentUpdate: true);
		fileArrow.DOColor(Color.white.WithAlpha(alpha), UIPanelEaseDur).SetUpdate(isIndependentUpdate: true);
		RectTransform component = fileActionsPanel.GetComponent<RectTransform>();
		float endValue = (show ? filePanelOpenWidth : filePanelCloseWidth);
		component.DOKill();
		component.DOAnchorPosY(endValue, UIPanelEaseDur).SetUpdate(isIndependentUpdate: true).SetEase(UIPanelEaseMode);
		fileArrow.sprite = (show ? fileSpriteUp : fileSpriteDown);
		if (show)
		{
			string lastOpenedLevel = Persistence.GetLastOpenedLevel();
			string text = "";
			if (File.Exists(lastOpenedLevel))
			{
				string value = "<color=#6495ED>" + Path.GetFileNameWithoutExtension(lastOpenedLevel) + "</color>";
				text = RDString.Get("editor.openRecent", new Dictionary<string, object> { { "file", value } });
			}
			else
			{
				text = RDString.Get("editor.noRecentFile");
			}
			buttonOpenRecent.GetComponentInChildren<Text>().text = text + "<color=#00000077> (" + RDEditorUtils.KeyComboToString(control: true, shift: true, KeyCode.O) + ")</color>";
		}
	}

	public void ShowShortcutsPanel(bool show)
	{
		showingShortcuts = show;
		RectTransform component = shortcutsPanel.GetComponent<RectTransform>();
		float endValue = (show ? shortcutsPanelOpenHeight : shortcutsPanelCloseHeight);
		component.DOKill();
		component.DOAnchorPosY(endValue, UIPanelEaseDur).SetUpdate(isIndependentUpdate: true).SetEase(UIPanelEaseMode);
		if (show)
		{
			CloseAllPanels(shortcutsPanel);
			ShowEventPicker(show: false);
			CloseAllInspectors();
			ShowShortcutTab(0);
		}
	}

	public void ShowShortcutTab(int index)
	{
		for (int i = 0; i < shortcutTabs.Count; i++)
		{
			bool flag = i == index;
			GameObject obj = shortcutTabs[i];
			Image component = obj.GetComponent<Image>();
			Text componentInChildren = obj.GetComponentInChildren<Text>();
			component.color = (flag ? Color.white : Color.white.WithAlpha(0.13f));
			componentInChildren.color = (flag ? Color.black : Color.white);
			GameObject obj2 = shortcutContent[i];
			obj2.SetActive(flag);
			obj2.transform.GetChild(1).GetChild(0).GetComponent<Scrollbar>()
				.value = 1f;
		}
	}

	public void ToggleFileActionsPanel()
	{
		ShowFileActionsPanel(!showingFileActions);
	}

	public void ToggleShortcutsPanel()
	{
		ShowShortcutsPanel(!showingShortcuts);
	}

	private void ShowSelectedFloorsAsDeselected()
	{
		foreach (scrFloor selectedFloor in selectedFloors)
		{
			if (selectedFloor != null)
			{
				ShowDeselectedColor(selectedFloor);
			}
		}
	}

	public void DeselectFloors(bool skipSaving = false)
	{
		if (SelectionIsEmpty())
		{
			return;
		}
		using (new SaveStateScope(this, clearRedo: true, onlySelectionChanged: true, skipSaving))
		{
			DOTween.Kill("selectedColorTween");
			ShowSelectedFloorsAsDeselected();
			levelEventsPanel.HideAllInspectorTabs();
			selectedFloors.Clear();
			SelectFloorInfo();
			UpdateSelectedFloor();
		}
	}

	private bool DeleteFloor(int sequenceIndex, bool remakePath = true)
	{
		if (lockPathEditing)
		{
			return false;
		}
		using (new SaveStateScope(this))
		{
			int num = sequenceIndex - 1;
			int sequenceID = sequenceIndex;
			bool result = false;
			bool flag = num < levelData.pathData.Length;
			bool flag2 = num < levelData.angleData.Count;
			if (num >= 0 && ((isOldLevel && flag) || (!isOldLevel && flag2)))
			{
				foreach (LevelEvent item in events.FindAll((LevelEvent x) => x.floor == sequenceID))
				{
					if (EventHasBackgroundSprite(item))
					{
						refreshBgSprites = true;
					}
					if (item.eventType == LevelEventType.AddDecoration || item.eventType == LevelEventType.AddText)
					{
						refreshDecSprites = true;
					}
					if (item.data.ContainsKey("startTile"))
					{
						Tuple<int, TileRelativeTo> tuple = CustomLevel.StringToTile(item.data["startTile"].ToString());
						if (tuple.Item2 == TileRelativeTo.Start && tuple.Item1 > sequenceID)
						{
							item.data["startTile"] = new Tuple<int, TileRelativeTo>(tuple.Item1 - 1, TileRelativeTo.Start);
						}
						else if (tuple.Item2 == TileRelativeTo.End && tuple.Item1 <= sequenceID)
						{
							item.data["startTile"] = new Tuple<int, TileRelativeTo>(tuple.Item1 + 1, TileRelativeTo.End);
						}
					}
					if (item.data.ContainsKey("endTile"))
					{
						Tuple<int, TileRelativeTo> tuple2 = CustomLevel.StringToTile(item.data["endTile"].ToString());
						if (tuple2.Item2 == TileRelativeTo.Start && tuple2.Item1 > sequenceID)
						{
							item.data["endTile"] = new Tuple<int, TileRelativeTo>(tuple2.Item1 - 1, TileRelativeTo.Start);
						}
						else if (tuple2.Item2 == TileRelativeTo.End && tuple2.Item1 <= sequenceID)
						{
							item.data["endTile"] = new Tuple<int, TileRelativeTo>(tuple2.Item1 + 1, TileRelativeTo.End);
						}
					}
				}
				events.RemoveAll((LevelEvent x) => x.floor == sequenceID);
				OffsetFloorIDsInEvents(sequenceID, -1);
				if (isOldLevel)
				{
					RemoveCharFloor(num);
				}
				else
				{
					RemoveFloatFloor(num);
				}
				if (remakePath)
				{
					RemakePath();
				}
				result = true;
			}
			UpdateSelectedFloor();
			return result;
		}
	}

	private void RemoveCharFloor(int charIndex)
	{
		levelData.pathData = levelData.pathData.Remove(charIndex, 1);
	}

	private void RemoveFloatFloor(int floatIndex)
	{
		levelData.angleData.RemoveAt(floatIndex);
	}

	private void UpdateFloorDirectionButtons(bool active)
	{
		if (active)
		{
			PreviousFloor(selectedFloors[0]);
			double num = selectedFloors[0].entryangle * 57.295780181884766;
			float oppositeAngle = Mathf.Abs(450f - (float)num) % 360f;
			FloorDirectionButton[] array = floorDirectionButtons;
			foreach (FloorDirectionButton btn in array)
			{
				UpdateDirectionButton(btn, oppositeAngle);
			}
			floorButtonCanvas.transform.position = selectedFloors[0].transform.position;
		}
		floorButtonCanvas.gameObject.SetActive(active);
	}

	private void OffsetFloorIDsInEvents(int startFloorID, int offset)
	{
		List<LevelEvent>[] array = new List<LevelEvent>[2] { events, decorations };
		for (int i = 0; i < array.Length; i++)
		{
			foreach (LevelEvent item in array[i])
			{
				if (item.floor > startFloorID)
				{
					item.floor += offset;
				}
			}
		}
		refreshDecSprites = true;
	}

	private void UpdateDirectionButton(FloorDirectionButton btn, float oppositeAngle)
	{
		if (!(btn == null))
		{
			int num = 0;
			bool flag = false;
			switch (btn.btnType)
			{
				case FloorDirectionButtonType.D:
					num = 0;
					break;
				case FloorDirectionButtonType.BackQuoteJ:
					num = 15;
					break;
				case FloorDirectionButtonType.J:
					num = 30;
					break;
				case FloorDirectionButtonType.E:
					num = 45;
					break;
				case FloorDirectionButtonType.Y:
					num = 60;
					break;
				case FloorDirectionButtonType.BackQuoteY:
					num = 75;
					break;
				case FloorDirectionButtonType.W:
					num = 90;
					break;
				case FloorDirectionButtonType.BackQuoteT:
					num = 105;
					break;
				case FloorDirectionButtonType.T:
					num = 120;
					break;
				case FloorDirectionButtonType.Q:
					num = 135;
					break;
				case FloorDirectionButtonType.H:
					num = 150;
					break;
				case FloorDirectionButtonType.BackQuoteH:
					num = 165;
					break;
				case FloorDirectionButtonType.A:
					num = 180;
					break;
				case FloorDirectionButtonType.BackQuoteN:
					num = 195;
					break;
				case FloorDirectionButtonType.N:
					num = 210;
					break;
				case FloorDirectionButtonType.Z:
					num = 225;
					break;
				case FloorDirectionButtonType.V:
					num = 240;
					break;
				case FloorDirectionButtonType.BackQuoteV:
					num = 255;
					break;
				case FloorDirectionButtonType.S:
					num = 270;
					break;
				case FloorDirectionButtonType.BackQuoteB:
					num = 285;
					break;
				case FloorDirectionButtonType.B:
					num = 300;
					break;
				case FloorDirectionButtonType.C:
					num = 315;
					break;
				case FloorDirectionButtonType.M:
					num = 330;
					break;
				case FloorDirectionButtonType.BackQuoteM:
					num = 345;
					break;
				case FloorDirectionButtonType.Space:
					flag = true;
					break;
				case FloorDirectionButtonType.Tab:
					flag = true;
					break;
			}
			btn.delete = Mathf.Approximately(oppositeAngle, num) && !flag && !FloorIsMidspinOr360(selectedFloors[0]);
			btn.gameObject.SetActive(!btn.delete || selectedFloors[0].seqID != 0);
			btn.Init();
		}
	}

	private bool FloorPointsBackwards(char floorType)
	{
		if (floorType == '6' || floorType == '5' || floorType == '8' || floorType == '7')
		{
			return false;
		}
		PreviousFloor(selectedFloors[0]);
		float b = scrLevelMaker.GetAngleFromFloorCharDirection(floorType) % 360f;
		double num = selectedFloors[0].entryangle * 57.295780181884766;
		return Mathf.Approximately(Mathf.Abs(450f - (float)num) % 360f, b);
	}

	private bool FloorPointsBackwards(float floorAngle)
	{
		PreviousFloor(selectedFloors[0]);
		float b = floorAngle % 360f;
		double num = selectedFloors[0].entryangle * 57.295780181884766;
		return Mathf.Approximately(Mathf.Abs(450f - (float)num) % 360f, b);
	}

	private bool FloorIsMidspinOr360(scrFloor floor)
	{
		if (!scrMisc.ApproximatelyFloor(scrMisc.GetAngleMoved(floor.entryangle, floor.exitangle, !floor.isCCW), 6.2831854820251465))
		{
			return scrMisc.ApproximatelyFloor(floor.entryangle, floor.exitangle);
		}
		return true;
	}

	private void MoveCameraToFloor(scrFloor floor)
	{
		ShortcutExtensions.DOMove(endValue: new Vector3(floor.x, floor.y, -10f), target: camera.transform, duration: cameraSelectDuration).SetUpdate(isIndependentUpdate: true);
	}

	private void CreateFloor(char floorType, bool pulseFloorButtons = true, bool fullSpin = false)
	{
		if (!SelectionIsSingle())
		{
			return;
		}
		scrFloor scrFloor2 = selectedFloors[0];
		if (fullSpin && scrFloor2.seqID == 0)
		{
			return;
		}
		using (new SaveStateScope(this))
		{
			int seqID = scrFloor2.seqID;
			scrFloor scrFloor3 = PreviousFloor(scrFloor2);
			_ = scrLevelMaker.GetAngleFromFloorCharDirection(floorType) % 360f;
			double num = scrFloor2.entryangle * 57.295780181884766;
			_ = Mathf.Abs(450f - (float)num) % 360f;
			if (FloorPointsBackwards(floorType) && !fullSpin && !FloorIsMidspinOr360(scrFloor2))
			{
				if (scrFloor3 != null)
				{
					int seqID2 = scrFloor2.seqID;
					if (DeleteFloor(seqID2))
					{
						SelectFloor(floors[seqID2 - 1]);
					}
					scrFloor floor = floors[seqID2 - 1];
					MoveCameraToFloor(floor);
				}
				return;
			}
			foreach (LevelEvent @event in events)
			{
				if (@event.data.ContainsKey("startTile"))
				{
					Tuple<int, TileRelativeTo> tuple = CustomLevel.StringToTile(@event.data["startTile"].ToString());
					if (tuple.Item2 == TileRelativeTo.Start && tuple.Item1 > seqID)
					{
						@event.data["startTile"] = new Tuple<int, TileRelativeTo>(tuple.Item1 + 1, TileRelativeTo.Start);
					}
					else if (tuple.Item2 == TileRelativeTo.End && tuple.Item1 <= seqID)
					{
						@event.data["startTile"] = new Tuple<int, TileRelativeTo>(tuple.Item1 - 1, TileRelativeTo.End);
					}
				}
				if (@event.data.ContainsKey("endTile"))
				{
					Tuple<int, TileRelativeTo> tuple2 = CustomLevel.StringToTile(@event.data["endTile"].ToString());
					if (tuple2.Item2 == TileRelativeTo.Start && tuple2.Item1 > seqID)
					{
						@event.data["endTile"] = new Tuple<int, TileRelativeTo>(tuple2.Item1 + 1, TileRelativeTo.Start);
					}
					else if (tuple2.Item2 == TileRelativeTo.End && tuple2.Item1 <= seqID)
					{
						@event.data["endTile"] = new Tuple<int, TileRelativeTo>(tuple2.Item1 - 1, TileRelativeTo.End);
					}
				}
			}
			OffsetFloorIDsInEvents(seqID, 1);
			InsertCharFloor(seqID, floorType);
			scrFloor scrFloor4 = floors[seqID + 1];
			SelectFloor(scrFloor4);
			MoveCameraToFloor(scrFloor4);
			if (pulseFloorButtons)
			{
				Button button = null;
				switch (floorType)
				{
					case 'R':
						button = buttonD;
						break;
					case 'U':
						button = buttonW;
						break;
					case 'L':
						button = buttonA;
						break;
					case 'D':
						button = buttonS;
						break;
					case 'E':
						button = buttonE;
						break;
					case 'Q':
						button = buttonQ;
						break;
					case 'Z':
						button = buttonZ;
						break;
					case 'C':
						button = buttonC;
						break;
					case 'Y':
						button = buttonG;
						break;
					case 'T':
						button = buttonT;
						break;
					case 'V':
						button = buttonF;
						break;
					case 'B':
						button = buttonB;
						break;
					case 'H':
						button = buttonH;
						break;
					case 'J':
						button = buttonJ;
						break;
					case 'M':
						button = buttonM;
						break;
					case 'N':
						button = buttonN;
						break;
				}
				if (button != null)
				{
					Vector3 endValue = new Vector3(1f, 1f);
					button.transform.DOKill();
					button.transform.ScaleXY(floorButtonPulseSize);
					button.transform.DOScale(endValue, floorButtonPulseDuration).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutQuad);
				}
			}
		}
	}

	private void CreateFloor(float floorAngle, bool pulseFloorButtons = true, bool fullSpin = false)
	{
		if (!SelectionIsSingle())
		{
			return;
		}
		scrFloor scrFloor2 = selectedFloors[0];
		if (fullSpin && scrFloor2.seqID == 0)
		{
			return;
		}
		using (new SaveStateScope(this))
		{
			int seqID = scrFloor2.seqID;
			scrFloor scrFloor3 = PreviousFloor(scrFloor2);
			_ = floorAngle % 360f;
			double num = scrFloor2.entryangle * 57.295780181884766;
			_ = Mathf.Abs(450f - (float)num) % 360f;
			if (FloorPointsBackwards(floorAngle) && !fullSpin && !FloorIsMidspinOr360(scrFloor2))
			{
				if (scrFloor3 != null)
				{
					int seqID2 = scrFloor2.seqID;
					if (DeleteFloor(seqID2))
					{
						SelectFloor(floors[seqID2 - 1]);
					}
					scrFloor floor = floors[seqID2 - 1];
					MoveCameraToFloor(floor);
				}
			}
			else
			{
				OffsetFloorIDsInEvents(seqID, 1);
				InsertFloatFloor(seqID, floorAngle);
				scrFloor scrFloor4 = floors[seqID + 1];
				SelectFloor(scrFloor4);
				MoveCameraToFloor(scrFloor4);
				if (pulseFloorButtons)
				{
					Button button = null;
					if (floorAngle <= 180f)
					{
						if (floorAngle <= 60f)
						{
							if (floorAngle <= 30f)
							{
								if (floorAngle != 0f)
								{
									if (floorAngle == 30f)
									{
										button = buttonJ;
									}
								}
								else
								{
									button = buttonD;
								}
							}
							else if (floorAngle != 45f)
							{
								if (floorAngle == 60f)
								{
									button = buttonT;
								}
							}
							else
							{
								button = buttonE;
							}
						}
						else if (floorAngle <= 135f)
						{
							if (floorAngle != 90f)
							{
								if (floorAngle == 135f)
								{
									button = buttonQ;
								}
							}
							else
							{
								button = buttonW;
							}
						}
						else if (floorAngle != 150f)
						{
							if (floorAngle == 180f)
							{
								button = buttonA;
							}
						}
						else
						{
							button = buttonH;
						}
					}
					else if (floorAngle <= 270f)
					{
						if (floorAngle <= 225f)
						{
							if (floorAngle != 210f)
							{
								if (floorAngle == 225f)
								{
									button = buttonZ;
								}
							}
							else
							{
								button = buttonN;
							}
						}
						else if (floorAngle != 255f)
						{
							if (floorAngle == 270f)
							{
								button = buttonS;
							}
						}
						else
						{
							button = buttonF;
						}
					}
					else if (floorAngle <= 300f)
					{
						if (floorAngle != 285f)
						{
							if (floorAngle == 300f)
							{
								button = buttonB;
							}
						}
						else
						{
							button = buttonG;
						}
					}
					else if (floorAngle != 315f)
					{
						if (floorAngle == 330f)
						{
							button = buttonM;
						}
					}
					else
					{
						button = buttonC;
					}
					if (button != null)
					{
						Vector3 endValue = new Vector3(1f, 1f);
						button.transform.DOKill();
						button.transform.ScaleXY(floorButtonPulseSize);
						button.transform.DOScale(endValue, floorButtonPulseDuration).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutQuad);
					}
				}
			}
		}
		UpdateDecorationObjects();
	}

	private void CreateFloorWithCharOrAngle(float angle, char chara, bool pulseFloorButtons = true, bool fullSpin = false)
	{
		if (lockPathEditing)
		{
			return;
		}
		if (isOldLevel && chara != '?')
		{
			CreateFloor(chara, pulseFloorButtons, fullSpin);
			return;
		}
		bool exists;
		float angleFromFloorCharDirectionWithCheck = scrLevelMaker.GetAngleFromFloorCharDirectionWithCheck(chara, out exists);
		if (exists)
		{
			CreateFloor(angleFromFloorCharDirectionWithCheck);
		}
		else
		{
			CreateFloor(angle, pulseFloorButtons, fullSpin);
		}
	}

	public void CreateArbitraryFloor()
	{
		if (isOldLevel)
		{
			return;
		}
		bool successful;
		float num = GetArbitraryAngleFromField(out successful);
		floorButtonArbitrary.text = (successful ? num.ToString() : "");
		if (successful && (!successful || !(Mathf.Abs(num) < 0.01f) || num == 0f))
		{
			char chara = '£';
			if (!useAbsoluteArbitraryAngle)
			{
				num += selectedFloors[0].floatDirection;
			}
			CreateFloorWithCharOrAngle(num, chara);
		}
	}

	private float GetArbitraryAngleFromField(out bool successful)
	{
		float result = 1f;
		if (!float.TryParse(floorButtonArbitrary.text, out result))
		{
			DataTable dataTable = new DataTable();
			try
			{
				result = RDEditorUtils.DecodeFloat(dataTable.Compute(floorButtonArbitrary.text, ""));
			}
			catch
			{
				successful = false;
				return 0f;
			}
		}
		result %= 360f;
		successful = true;
		return result;
	}

	public void ToggleAngleInputMode()
	{
		if (floorButtonContainer.activeSelf)
		{
			floorButtonContainer.SetActive(value: false);
			floorButtonArbitraryContainer.SetActive(value: true);
			buttonToggleAngleInput.GetComponent<Image>().sprite = show8DirectionSprite;
		}
		else
		{
			floorButtonContainer.SetActive(value: true);
			floorButtonArbitraryContainer.SetActive(value: false);
			buttonToggleAngleInput.GetComponent<Image>().sprite = showArbitraryAngleSprite;
		}
	}

	public void SwitchArbitraryMode()
	{
		useAbsoluteArbitraryAngle = !useAbsoluteArbitraryAngle;
	}

	public void CreateArbitraryMidspin()
	{
		if (isOldLevel)
		{
			return;
		}
		bool successful;
		float num = GetArbitraryAngleFromField(out successful);
		floorButtonArbitrary.text = (successful ? num.ToString() : "");
		if (successful && (!successful || !(Mathf.Abs(num) < 0.01f) || num == 0f))
		{
			char chara = '£';
			if (!useAbsoluteArbitraryAngle)
			{
				num = ((selectedFloors[0].floatDirection != 999f) ? (num + selectedFloors[0].floatDirection) : (num + floors[Math.Max(0, selectedFloors[0].seqID - 1)].floatDirection));
			}
			if (Mathf.Abs(num) > 0.01f || num == 0f)
			{
				CreateFloorWithCharOrAngle(num, chara);
				CreateFloorWithCharOrAngle(999f, '!', pulseFloorButtons: true, fullSpin: true);
			}
		}
	}

	public void InsertCharFloor(int sequenceID, char floorType)
	{
		levelData.pathData = levelData.pathData.Insert(sequenceID, floorType.ToString());
		RemakePath();
	}

	public void InsertFloatFloor(int sequenceID, float floorAngle)
	{
		levelData.angleData.Insert(sequenceID, floorAngle);
		RemakePath();
	}

	public void RemakePath(bool applyEventsToFloors = true)
	{
		customLevel.RemakePath(applyEventsToFloors);
		if (!GCS.standaloneLevelMode)
		{
			DrawFloorOffsetLines();
		}
		DrawHolds();
		DrawFloorNums();
		DrawMultiPlanet();
	}

	private void DrawFloorNums()
	{
		foreach (scrFloor floor in floors)
		{
			if (floor.enabled)
			{
				floor.editorNumText.gameObject.SetActive(showFloorNums && !playMode);
			}
		}
	}

	private void DrawHolds(bool unfillHolds = false)
	{
		customLevel.levelMaker.DrawHolds(unfillHolds);
	}

	private void DrawMultiPlanet()
	{
		if (customLevel.levelMaker.DrawMultiPlanet() > 3 && !mpWarned)
		{
			ShowPopup(show: true, PopupType.MultiPlanet, skipAnim: true);
			mpWarned = true;
		}
	}

	private void DrawFloorOffsetLines()
	{
		foreach (GameObject floorConnectorGO in floorConnectorGOs)
		{
			UnityEngine.Object.Destroy(floorConnectorGO);
		}
		floorConnectorGOs.Clear();
		int num = -1;
		int num2 = -2;
		Vector3 vector = Vector3.zero;
		foreach (LevelEvent @event in events)
		{
			if (@event.eventType != LevelEventType.PositionTrack || @event.floor <= 0)
			{
				continue;
			}
			num = @event.floor;
			if ((floors[num].prevfloor != null && floors[num].prevfloor.holdLength > -1) || (@event.data.Keys.Contains("justThisTile") && (ToggleBool)@event.data["justThisTile"] == ToggleBool.Enabled))
			{
				continue;
			}
			if (num != num2)
			{
				vector = new Vector2(0f, 0f);
			}
			Vector3 vector2 = (Vector2)@event.data["positionOffset"] * customLevel.GetTileSize();
			Vector2 vector3 = new Vector2(-0.75f * Mathf.Cos((float)floors[num - 1].exitangle + MathF.PI / 2f), 0.75f * Mathf.Sin((float)floors[num - 1].exitangle + MathF.PI / 2f));
			Vector3 vector4 = new Vector3(vector.x + floors[num - 1].transform.position.x + vector3.x, vector.y + floors[num - 1].transform.position.y + vector3.y, floors[num - 1].transform.position.z);
			Vector3 vector5 = new Vector3(vector4.x + vector2.x, vector4.y + vector2.y, floors[num].transform.position.z);
			if (!(Vector3.Distance(vector4, vector5) < 0.05f))
			{
				GameObject gameObject = new GameObject();
				LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
				lineRenderer.positionCount = 2;
				lineRenderer.material = lineMaterial;
				lineRenderer.textureMode = LineTextureMode.Tile;
				if ((ToggleBool)@event.data["editorOnly"] == ToggleBool.Enabled)
				{
					lineRenderer.startColor = lineGreen;
					lineRenderer.endColor = lineYellow;
				}
				else
				{
					lineRenderer.startColor = linePurple;
					lineRenderer.endColor = lineBlue;
				}
				lineRenderer.SetPosition(0, vector4);
				lineRenderer.SetPosition(1, vector5);
				lineRenderer.startWidth = 0.1f;
				lineRenderer.endWidth = 0.1f;
				lineRenderer.name = "Floor connector";
				lineRenderer.transform.parent = floorConnectors.transform;
				floorConnectorGOs.Add(gameObject);
				Vector2 vector6 = (Vector2)@event.data["positionOffset"] * customLevel.GetTileSize();
				vector += new Vector3(vector6.x, vector6.y, 0f);
				num2 = num;
			}
		}
	}

	private void ClearAllFloorOffsets()
	{
		foreach (GameObject floorConnectorGO in floorConnectorGOs)
		{
			UnityEngine.Object.Destroy(floorConnectorGO);
		}
		floorConnectorGOs.Clear();
	}

	public void SelectFloor(scrFloor floorToSelect, bool cameraJump = true)
	{
		if (floorToSelect == null)
		{
			return;
		}
		using (new SaveStateScope(this, clearRedo: true, onlySelectionChanged: true))
		{
			ClearMultiSelection();
			DOTween.Kill("selectedColorTween");
			ShowSelectedFloorsAsDeselected();
			if ((bool)lastSelectedFloor)
			{
				ShowDeselectedColor(lastSelectedFloor);
			}
			selectedFloors.Clear();
			selectedFloors.Add(floorToSelect);
			SelectFloorInfo(floorToSelect);
			ShowSelectedColor(floorToSelect);
			if (cameraJump && Vector2.Distance(camera.transform.position, floorToSelect.transform.position) > 3f)
			{
				Vector3 targetPos = floorToSelect.transform.position.WithZ(-10f);
				DoCameraJump(targetPos);
			}
			UpdateSelectedFloor();
		}
	}

	private void DoCameraJump(Vector3 targetPos)
	{
		camera.transform.DOKill();
		camera.transform.DOMove(targetPos, 0.6f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutExpo);
	}

	private scrFloor SelectFirstFloor()
	{
		scrFloor scrFloor2 = floors[0];
		SelectFloor(scrFloor2);
		return scrFloor2;
	}

	private scrFloor NextFloor(scrFloor floor)
	{
		List<scrFloor> list = floors;
		int num = list.IndexOf(floor) + 1;
		if (num >= list.Count)
		{
			return null;
		}
		return list[num];
	}

	private scrFloor PreviousFloor(scrFloor floor)
	{
		List<scrFloor> list = floors;
		int num = list.IndexOf(floor) - 1;
		if (num < 0)
		{
			return null;
		}
		return list[num];
	}

	private void MultiSelectFloors(scrFloor startFloor, scrFloor endFloor, bool setSelectPoint = false)
	{
		ClearMultiSelection();
		if (floors.Count == 1)
		{
			return;
		}
		using (new SaveStateScope(this, clearRedo: true, onlySelectionChanged: true))
		{
			int num = floors.IndexOf(startFloor);
			int num2 = floors.IndexOf(endFloor);
			int num3;
			int num4;
			if (num2 - num > 0)
			{
				num3 = num;
				num4 = num2;
			}
			else
			{
				if (num2 - num >= 0)
				{
					SelectFloor(startFloor);
					UpdateSelectedFloor();
					return;
				}
				num3 = num2;
				num4 = num;
			}
			if (setSelectPoint)
			{
				multiSelectPoint = startFloor;
			}
			for (int i = num3; i <= num4; i++)
			{
				selectedFloors.Add(floors[i]);
			}
			SelectFloorInfo(selectedFloors);
			DOTween.Kill("selectedColorTween");
			if (selectedFloors.Count == 1)
			{
				ClearMultiSelection();
			}
			else
			{
				foreach (scrFloor selectedFloor in selectedFloors)
				{
					ShowSelectedColor(selectedFloor);
				}
			}
			UpdateSelectedFloor();
		}
	}

	private void ClearMultiSelection()
	{
		ShowSelectedFloorsAsDeselected();
		selectedFloors.Clear();
	}

	public bool SelectionIsEmpty()
	{
		return selectedFloors.Count == 0;
	}

	public bool SelectionIsSingle()
	{
		if (selectedFloors.Count == 1)
		{
			return selectedFloors[0] != null;
		}
		return false;
	}

	public bool SelectionDecorationIsEmpty()
	{
		return selectedDecorations.Count == 0;
	}

	public bool SelectionDecorationIsSingle()
	{
		if (selectedDecorations.Count == 1)
		{
			return selectedDecorations[0] != null;
		}
		return false;
	}

	private void SelectFloorInfo(scrFloor floor)
	{
		SelectFloorInfo(new List<scrFloor> { floor });
	}

	private void SelectFloorInfo(List<scrFloor> floors = null)
	{
		if (floors == null)
		{
			floors = new List<scrFloor>();
		}
		findFloorSelectedInfo.text = RDString.Get("editor.findFloor.currFloor", new Dictionary<string, object>
		{
			{
				"seqID",
				(floors.Count > 0) ? floors[0].seqID.ToString() : "None"
			},
			{ "floorCount", floors.Count }
		});
	}

	private void FlipFloor(scrFloor floor, bool horizontal = true, bool remakePath = true)
	{
		int seqID = floor.seqID;
		if (seqID != 0)
		{
			if (isOldLevel)
			{
				char c = ((!horizontal) ? ADOBase.lm.GetVFlippedDirection(floor.stringDirection) : ADOBase.lm.GetHFlippedDirection(floor.stringDirection));
				levelData.pathData = levelData.pathData.Remove(floor.seqID - 1, 1).Insert(floor.seqID - 1, c.ToString());
			}
			else
			{
				float item = ((!horizontal) ? ADOBase.lm.GetVFlippedDirection(floor.floatDirection) : ADOBase.lm.GetHFlippedDirection(floor.floatDirection));
				levelData.angleData.RemoveAt(floor.seqID - 1);
				levelData.angleData.Insert(floor.seqID - 1, item);
			}
			if (remakePath)
			{
				RemakePath();
				SelectFloor(floors[seqID]);
			}
		}
	}

	private void FlipSelection(bool horizontal = true)
	{
		int seqID = selectedFloors[0].seqID;
		int seqID2 = selectedFloors.Last().seqID;
		int seqID3 = multiSelectPoint.seqID;
		if (seqID == 0)
		{
			return;
		}
		foreach (scrFloor selectedFloor in selectedFloors)
		{
			FlipFloor(selectedFloor, horizontal, remakePath: false);
		}
		RemakePath();
		MultiSelectFloors(floors[seqID], floors[seqID2]);
		multiSelectPoint = floors[seqID3];
	}

	private void RotateFloor(scrFloor floor, bool CW = true, bool remakePath = true)
	{
		if (lockPathEditing)
		{
			return;
		}
		int seqID = floor.seqID;
		if (seqID != 0)
		{
			if (isOldLevel)
			{
				char rotDirection = ADOBase.lm.GetRotDirection(floor.stringDirection, CW);
				levelData.pathData = levelData.pathData.Remove(floor.seqID - 1, 1);
				levelData.pathData = levelData.pathData.Insert(floor.seqID - 1, rotDirection.ToString());
			}
			else
			{
				float rotDirection2 = ADOBase.lm.GetRotDirection(floor.floatDirection, CW);
				levelData.angleData.RemoveAt(floor.seqID - 1);
				levelData.angleData.Insert(floor.seqID - 1, rotDirection2);
			}
			if (remakePath)
			{
				RemakePath();
				SelectFloor(floors[seqID]);
			}
		}
	}

	private void RotateSelection(bool CW = true)
	{
		int seqID = selectedFloors[0].seqID;
		int seqID2 = selectedFloors.Last().seqID;
		int seqID3 = multiSelectPoint.seqID;
		if (seqID == 0)
		{
			return;
		}
		foreach (scrFloor selectedFloor in selectedFloors)
		{
			RotateFloor(selectedFloor, CW, remakePath: false);
		}
		RemakePath();
		MultiSelectFloors(floors[seqID], floors[seqID2]);
		multiSelectPoint = floors[seqID3];
	}

	private void RotateFloor180(scrFloor floor, bool remakePath = true)
	{
		int seqID = floor.seqID;
		if (seqID != 0)
		{
			if (isOldLevel)
			{
				char rotDirection = ADOBase.lm.GetRotDirection(ADOBase.lm.GetRotDirection(floor.stringDirection, CW: true), CW: true);
				levelData.pathData = levelData.pathData.Remove(floor.seqID - 1, 1);
				levelData.pathData = levelData.pathData.Insert(floor.seqID - 1, rotDirection.ToString());
			}
			else
			{
				float rotDirection2 = ADOBase.lm.GetRotDirection(ADOBase.lm.GetRotDirection(floor.floatDirection, CW: true), CW: true);
				levelData.angleData.RemoveAt(floor.seqID - 1);
				levelData.angleData.Insert(floor.seqID - 1, rotDirection2);
			}
			if (remakePath)
			{
				RemakePath();
				SelectFloor(floors[seqID]);
			}
		}
	}

	private void RotateSelection180()
	{
		int seqID = selectedFloors[0].seqID;
		int seqID2 = selectedFloors.Last().seqID;
		int seqID3 = multiSelectPoint.seqID;
		if (seqID == 0)
		{
			return;
		}
		foreach (scrFloor selectedFloor in selectedFloors)
		{
			RotateFloor180(selectedFloor, remakePath: false);
		}
		RemakePath();
		MultiSelectFloors(floors[seqID], floors[seqID2]);
		multiSelectPoint = floors[seqID3];
	}

	private FloorData CopyOfFloor(scrFloor floor, bool selectedEventOnly = false)
	{
		char stringDir;
		float floatDir;
		if (floor.seqID == 0)
		{
			stringDir = 'R';
			floatDir = 0f;
		}
		else if (isOldLevel)
		{
			stringDir = floor.stringDirection;
			floatDir = scrLevelMaker.GetAngleFromFloorCharDirectionWithCheck(floor.stringDirection, out var _);
		}
		else
		{
			stringDir = 'R';
			floatDir = floor.floatDirection;
		}
		List<LevelEvent> list = new List<LevelEvent>();
		List<LevelEvent> list2 = new List<LevelEvent>();
		if (selectedEventOnly)
		{
			if (levelEventsPanel.selectedEventType != 0)
			{
				list2.Add(levelEventsPanel.selectedEvent);
			}
		}
		else
		{
			list2 = events.FindAll((LevelEvent x) => x.floor == floor.seqID);
		}
		foreach (LevelEvent item in list2)
		{
			list.Add(CopyEvent(item));
		}
		return new FloorData(stringDir, floatDir, list);
	}

	private LevelEvent CopyEvent(LevelEvent eventToCopy, int floor = -1)
	{
		LevelEvent levelEvent = new LevelEvent(floor, eventToCopy.eventType);
		levelEvent.data = new Dictionary<string, object>();
		levelEvent.disabled = new Dictionary<string, bool>();
		foreach (string key in eventToCopy.data.Keys)
		{
			object value = eventToCopy[key];
			levelEvent.data.Add(key, value);
			levelEvent.disabled.Add(key, eventToCopy.disabled[key]);
		}
		return levelEvent;
	}

	private void CopyTrackColor(int floor)
	{
		List<LevelEvent> list = events.FindAll((LevelEvent x) => x.floor == floor && x.eventType == LevelEventType.ColorTrack);
		if (list.Any())
		{
			copiedTrackColor = CopyEvent(list[0]);
			ShowNotification("Copied Track Color");
		}
		else
		{
			ShowNotification("No Track Color To Copy");
		}
	}

	private void PasteTrackColor(int id)
	{
		if (id != 0 && copiedTrackColor != null)
		{
			events.RemoveAll((LevelEvent x) => x.floor == id && x.eventType == LevelEventType.ColorTrack);
			events.Add(CopyEvent(copiedTrackColor, id));
			ApplyEventsToFloors();
			scrFloor scrFloor2 = floors[id];
			SelectFloor(scrFloor2);
			levelEventsPanel.ShowTabsForFloor(selectedFloors[0].seqID);
			levelEventsPanel.selectedEventType = LevelEventType.ColorTrack;
			levelEventsPanel.ShowPanel(LevelEventType.ColorTrack);
			ShowEventIndicators(scrFloor2);
			ShowNotification("Paste Track Color");
		}
	}

	private void PasteTrackColorSingleTile(int id)
	{
		if (id == 0 || copiedTrackColor == null)
		{
			return;
		}
		List<LevelEvent> list = new List<LevelEvent>();
		int i;
		for (i = id; i > 0; i--)
		{
			list = events.FindAll((LevelEvent x) => x.floor == i && x.eventType == LevelEventType.ColorTrack);
			if (list.Any())
			{
				break;
			}
		}
		if (list.Any())
		{
			previousTrackColor = CopyEvent(list[0]);
			events.RemoveAll((LevelEvent x) => x.floor == id && x.eventType == LevelEventType.ColorTrack);
			events.Add(CopyEvent(copiedTrackColor, id));
			if (id < floors.Count - 2 && !events.FindAll((LevelEvent x) => x.floor == id + 1 && x.eventType == LevelEventType.ColorTrack).Any())
			{
				events.Add(CopyEvent(previousTrackColor, id + 1));
			}
			ApplyEventsToFloors();
			scrFloor scrFloor2 = floors[id];
			SelectFloor(scrFloor2);
			levelEventsPanel.ShowTabsForFloor(selectedFloors[0].seqID);
			levelEventsPanel.selectedEventType = LevelEventType.ColorTrack;
			levelEventsPanel.ShowPanel(LevelEventType.ColorTrack);
			ShowEventIndicators(scrFloor2);
			ShowNotification("Paste Track Color (single tile)");
		}
	}

	private void CopyHitSound(int floor)
	{
		List<LevelEvent> list = events.FindAll((LevelEvent x) => x.floor == floor && x.eventType == LevelEventType.SetHitsound);
		if (list.Any())
		{
			copiedHitsound = CopyEvent(list[0]);
			ShowNotification("Copied Hitsound");
		}
		else
		{
			ShowNotification("No Hitsound To Copy");
		}
	}

	private void PasteHitsoundSingleTile(int id)
	{
		if (id == 0 || copiedHitsound == null)
		{
			return;
		}
		List<LevelEvent> list = new List<LevelEvent>();
		int i;
		for (i = id; i > 0; i--)
		{
			list = events.FindAll((LevelEvent x) => x.floor == i && x.eventType == LevelEventType.SetHitsound);
			if (list.Any())
			{
				break;
			}
		}
		if (list.Any())
		{
			previousHitsound = CopyEvent(list[0]);
			events.RemoveAll((LevelEvent x) => x.floor == id && x.eventType == LevelEventType.SetHitsound);
			events.Add(CopyEvent(copiedHitsound, id));
			if (id < floors.Count - 2 && !events.FindAll((LevelEvent x) => x.floor == id + 1 && x.eventType == LevelEventType.SetHitsound).Any())
			{
				events.Add(CopyEvent(previousHitsound, id + 1));
			}
			ApplyEventsToFloors();
			scrFloor scrFloor2 = floors[id];
			SelectFloor(scrFloor2);
			levelEventsPanel.ShowTabsForFloor(selectedFloors[0].seqID);
			levelEventsPanel.selectedEventType = LevelEventType.SetHitsound;
			levelEventsPanel.ShowPanel(LevelEventType.SetHitsound);
			ShowEventIndicators(scrFloor2);
			ShowNotification("Paste Hitsound (single tile)");
		}
	}

	private void CopyFloor(scrFloor toCopy, bool clearClipboard = true, bool cut = false, bool selectedEventOnly = false)
	{
		if (clearClipboard)
		{
			clipboard.Clear();
		}
		clipboard.Add(CopyOfFloor(toCopy, selectedEventOnly));
		clipboardContent = ClipboardContent.Floors;
		if (!cut)
		{
			FlashTile(toCopy);
		}
	}

	private void MultiCopyFloors(bool cut = false)
	{
		clipboard.Clear();
		foreach (scrFloor selectedFloor in selectedFloors)
		{
			if (!cut || selectedFloor.seqID != 0)
			{
				CopyFloor(selectedFloor, clearClipboard: false);
			}
		}
	}

	private void CopyDecoration(LevelEvent toCopy, bool clearClipboard = true, bool cut = false)
	{
		if (clearClipboard)
		{
			clipboard.Clear();
		}
		clipboard.Add(toCopy.Copy());
		clipboardContent = ClipboardContent.Decorations;
	}

	private void MultiCopyDecorations()
	{
		clipboard.Clear();
		foreach (LevelEvent selectedDecoration in selectedDecorations)
		{
			CopyDecoration(selectedDecoration, clearClipboard: false);
		}
	}

	private void DuplicateDecorations()
	{
		MultiCopyDecorations();
		PasteDecorations();
	}

	private void CutFloor(scrFloor toCut, bool clearClipboard = true, bool selectedEventOnly = false)
	{
		int id = toCut.seqID;
		if (id != 0)
		{
			CopyFloor(toCut, clearClipboard, cut: true, selectedEventOnly);
			List<LevelEvent> targetEvents = new List<LevelEvent>();
			if (selectedEventOnly)
			{
				if (levelEventsPanel.selectedEventType != 0)
				{
					targetEvents.Add(levelEventsPanel.selectedEvent);
				}
			}
			else
			{
				targetEvents = events.FindAll((LevelEvent x) => x.floor == id);
			}
			foreach (LevelEvent item in targetEvents)
			{
				if (EventHasBackgroundSprite(item))
				{
					refreshBgSprites = true;
				}
			}
			events.RemoveAll((LevelEvent x) => targetEvents.Contains(x));
			ApplyEventsToFloors();
		}
		levelEventsPanel.ShowTabsForFloor(id);
		ShowEventIndicators(toCut);
	}

	private void MultiCutFloors()
	{
		MultiCopyFloors(cut: true);
		DeleteMultiSelection();
	}

	private void CutDecoration(LevelEvent toCut)
	{
		CopyDecoration(toCut);
		RemoveEvent(toCut);
	}

	private void MultiCutDecorations()
	{
		MultiCopyDecorations();
		DeleteMultiSelectionDecorations();
	}

	private void PasteFloors()
	{
		List<int> list = new List<int>();
		if (!clipboard.Any() || clipboardContent != ClipboardContent.Floors || !SelectionIsSingle())
		{
			return;
		}
		int num = selectedFloors[0].seqID;
		if (isOldLevel)
		{
			char stringDirection = ((FloorData)clipboard[0]).stringDirection;
			if (FloorPointsBackwards(stringDirection))
			{
				return;
			}
		}
		else
		{
			float floatDirection = ((FloorData)clipboard[0]).floatDirection;
			if (FloorPointsBackwards(floatDirection))
			{
				return;
			}
		}
		OffsetFloorIDsInEvents(num, clipboard.Count);
		foreach (LevelEvent @event in events)
		{
			if (@event.data.ContainsKey("startTile"))
			{
				Tuple<int, TileRelativeTo> tuple = CustomLevel.StringToTile(@event.data["startTile"].ToString());
				if (tuple.Item2 == TileRelativeTo.Start && tuple.Item1 > num)
				{
					@event.data["startTile"] = new Tuple<int, TileRelativeTo>(tuple.Item1 + clipboard.Count, TileRelativeTo.Start);
				}
				else if (tuple.Item2 == TileRelativeTo.End && tuple.Item1 <= num)
				{
					@event.data["startTile"] = new Tuple<int, TileRelativeTo>(tuple.Item1 - clipboard.Count, TileRelativeTo.End);
				}
			}
			if (@event.data.ContainsKey("endTile"))
			{
				Tuple<int, TileRelativeTo> tuple2 = CustomLevel.StringToTile(@event.data["endTile"].ToString());
				if (tuple2.Item2 == TileRelativeTo.Start && tuple2.Item1 > num)
				{
					@event.data["endTile"] = new Tuple<int, TileRelativeTo>(tuple2.Item1 + clipboard.Count, TileRelativeTo.Start);
				}
				else if (tuple2.Item2 == TileRelativeTo.End && tuple2.Item1 <= num)
				{
					@event.data["endTile"] = new Tuple<int, TileRelativeTo>(tuple2.Item1 - clipboard.Count, TileRelativeTo.End);
				}
			}
		}
		for (int i = 0; i < clipboard.Count(); i++)
		{
			char stringDirection2 = ((FloorData)clipboard[i]).stringDirection;
			float floatDirection2 = ((FloorData)clipboard[i]).floatDirection;
			List<LevelEvent> levelEventData = ((FloorData)clipboard[i]).levelEventData;
			if (isOldLevel)
			{
				levelData.pathData = levelData.pathData.Insert(num, stringDirection2.ToString());
			}
			else
			{
				levelData.angleData.Insert(num, floatDirection2);
			}
			list.Add(num);
			num++;
			if (!levelEventData.Any())
			{
				continue;
			}
			foreach (LevelEvent item in levelEventData)
			{
				events.Add(CopyEvent(item, num));
				if (EventHasBackgroundSprite(item))
				{
					refreshBgSprites = true;
				}
				if (item.eventType == LevelEventType.AddDecoration || item.eventType == LevelEventType.AddText)
				{
					refreshDecSprites = true;
				}
			}
		}
		RemakePath();
		SelectFloor(floors[num]);
		MoveCameraToFloor(floors[num]);
		foreach (int item2 in list)
		{
			FlashTile(floors[item2]);
		}
		FlashTile(floors[selectedFloors[0].seqID]);
	}

	private void PasteDecorations()
	{
		if (!clipboard.Any() || clipboardContent != ClipboardContent.Decorations)
		{
			return;
		}
		using (new SaveStateScope(this))
		{
			if (!SelectionIsEmpty())
			{
				selectedDecorations.Clear();
				List<LevelEvent> list = new List<LevelEvent>();
				foreach (scrFloor selectedFloor in selectedFloors)
				{
					if (!(selectedFloor != null))
					{
						continue;
					}
					foreach (LevelEvent item in clipboard)
					{
						LevelEvent levelEvent = item.Copy();
						levelEvent.data["relativeTo"] = DecPlacementType.Tile;
						levelEvent.floor = selectedFloor.seqID;
						list.Add(levelEvent);
					}
				}
				{
					foreach (LevelEvent item2 in list)
					{
						AddDecoration(item2);
					}
					return;
				}
			}
			DeselectAllDecorations();
			foreach (LevelEvent item3 in clipboard)
			{
				LevelEvent dec = item3.Copy();
				AddDecoration(dec);
			}
		}
	}

	private void FlashTile(scrFloor floor)
	{
		string text = "copyFlashTween" + floor.seqID;
		DOTween.Kill(text);
		floor.floorRenderer.SetFlash(1f);
		floor.floorRenderer.TweenFlash(0f, 0.3f, text);
	}

	private void FlashDecoration(scrDecoration deco)
	{
	}

	private void ShowSelectedColor(scrFloor floor, float opacity = 1f)
	{
		FloorRenderer floorRenderer = floor.floorRenderer;
		Color[] array = SelectedColors(floorRenderer.deselectedColor);
		Color color = array[0];
		Color color2 = array[1];
		if (opacity != 1f)
		{
			color2.a = opacity;
			floorRenderer.color = GetOverlayColor(floorRenderer.deselectedColor, color);
			color2 = GetOverlayColor(floorRenderer.deselectedColor, color2);
		}
		else
		{
			floorRenderer.color = color;
		}
		DOTween.To(() => floorRenderer.color, delegate (Color x)
		{
			floorRenderer.color = x;
		}, color2, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo)
			.SetUpdate(isIndependentUpdate: true)
			.SetId("selectedColorTween");
		static Color GetOverlayColor(Color a, Color b)
		{
			a.r = GetOverlayValue(a.r, b.r);
			a.g = GetOverlayValue(a.g, b.g);
			a.b = GetOverlayValue(a.b, b.b);
			a.a = GetOverlayValue(a.a, b.a);
			return a;
		}
		static float GetOverlayValue(float a, float b)
		{
			if (a < 0.5f)
			{
				return 2f * a * b;
			}
			return 1f - 2f * (1f - a) * (1f - b);
		}
	}

	private void ShowDeselectedColor(scrFloor floor)
	{
		floor.SetColor(floor.floorRenderer.deselectedColor);
	}

	public void ShowSelectedColorForLastSelectedFloor()
	{
		if ((bool)lastSelectedFloor)
		{
			if (DOTween.TweensById("selectedColorTween") != null)
			{
				DOTween.Kill("selectedColorTween");
			}
			ShowSelectedColor(lastSelectedFloor, 0.5f);
		}
	}

	private void OnDecorationSelected(LevelEvent decorationEvent)
	{
		if (!SelectionIsEmpty())
		{
			DeselectFloors();
		}
		ShowSelectedColorForLastSelectedFloor();
	}

	private void OnDecorationAllItemsDeselected()
	{
		if ((bool)lastSelectedFloor && SelectionIsEmpty())
		{
			DOTween.Kill("selectedColorTween");
			ShowDeselectedColor(lastSelectedFloor);
		}
	}

	private void PasteEvents(scrFloor targetFloor, List<LevelEvent> eventsList, bool overwrite = true, bool selectAfterward = true)
	{
		int id = targetFloor.seqID;
		if (id == 0 || !eventsList.Any())
		{
			return;
		}
		if (overwrite)
		{
			events.RemoveAll((LevelEvent x) => x.floor == id);
		}
		foreach (LevelEvent ev in eventsList)
		{
			if ((!EditorConstants.soloTypes.Contains(ev.eventType) || !events.Exists((LevelEvent x) => x.floor == id && x.eventType == ev.eventType)) && (!EditorConstants.soloTypes.Contains(ev.eventType) || ((ev.eventType != LevelEventType.Hold || !events.Exists((LevelEvent x) => x.floor == id && x.eventType == LevelEventType.Pause)) && (ev.eventType != LevelEventType.Pause || !events.Exists((LevelEvent x) => x.floor == id && x.eventType == LevelEventType.Hold)))) && (!EditorConstants.soloTypes.Contains(ev.eventType) || ((ev.eventType != LevelEventType.FreeRoam || !events.Exists((LevelEvent x) => x.floor == id && x.eventType == LevelEventType.Twirl)) && (ev.eventType != LevelEventType.Twirl || !events.Exists((LevelEvent x) => x.floor == id && x.eventType == LevelEventType.FreeRoam)))))
			{
				if (EventHasBackgroundSprite(ev))
				{
					refreshBgSprites = true;
				}
				if (ev.eventType == LevelEventType.AddDecoration || ev.eventType == LevelEventType.AddText)
				{
					refreshDecSprites = true;
				}
				events.Add(CopyEvent(ev, id));
			}
		}
		ApplyEventsToFloors();
		if (selectAfterward)
		{
			LevelEventType eventType = eventsList[0].eventType;
			SelectFloor(targetFloor);
			levelEventsPanel.ShowTabsForFloor(selectedFloors[0].seqID);
			levelEventsPanel.selectedEventType = eventType;
			levelEventsPanel.ShowPanel(eventType);
			ShowEventIndicators(targetFloor);
		}
	}

	private void DeselectAnyUIGameObject()
	{
		EventSystem.current.SetSelectedGameObject(null);
	}

	private void TogglePause(bool clsToEditor = false)
	{
		if (!GCS.standaloneLevelMode)
		{
			ResetScene(clsToEditor);
		}
	}

	private void ResetScene(bool clsToEditor = false)
	{
		RDUtils.SetGarbageCollectionEnabled(enabled: true);
		autoFailed = false;
		if (clsToEditor)
		{
			levelEventsPanel.HideAllInspectorTabs();
			levelEventsPanel.ShowInspector(show: false);
			UpdateSongAndLevelSettings();
		}
		customLevel.ResetScene();
		refreshDecSprites = false;
		propertyControlList.ClearDragCache();
		propertyControlList.OnDecorationUpdate();
		DeselectAnyUIGameObject();
	}

	private void ClearFloorGlows()
	{
		foreach (scrFloor floor in floors)
		{
			if ((bool)floor.bottomGlow)
			{
				floor.bottomGlow.enabled = false;
			}
			floor.topGlow.enabled = false;
		}
	}

	public void UpdateSongAndLevelSettings()
	{
		int num = 0;
		foreach (PropertiesPanel panels in settingsPanel.panelsList)
		{
			panels.SetProperties(num switch
			{
				0 => levelData.levelSettings,
				1 => levelData.songSettings,
				2 => levelData.trackSettings,
				3 => levelData.backgroundSettings,
				4 => levelData.cameraSettings,
				5 => levelData.miscSettings,
				6 => levelData.decorationSettings,
				_ => levelData.songSettings,
			}, checkIfEnabled: false);
			num++;
		}
		settingsPanel.ShowPanel(LevelEventType.SongSettings);
	}

	public bool EventHasBackgroundSprite(LevelEvent evnt)
	{
		if (evnt.eventType == LevelEventType.CustomBackground)
		{
			return !string.IsNullOrEmpty(evnt.data["bgImage"].ToString());
		}
		return false;
	}

	private void PauseIfUnpaused()
	{
		if (!paused)
		{
			TogglePause();
		}
	}

	private void OpenLevel()
	{
		CheckUnsavedChanges(delegate
		{
			PauseIfUnpaused();
			StartCoroutine(OpenLevelCo());
		});
	}

	private string SanitizeLevelPath(string path)
	{
		return Uri.UnescapeDataString(path.Replace("file:", ""));
	}

	private IEnumerator OpenLevelCo(string definedLevelPath = null)
	{
		while (stallFileDialog)
		{
			yield return null;
		}
		ClearAllFloorOffsets();
		bool num = definedLevelPath == null;
		string lastLevelPath = customLevel.levelPath;
		printe("opening file browser 1");
		if (num)
		{
			printe("opening file browser 2");
			string[] levelPaths = StandaloneFileBrowser.OpenFilePanel(RDString.Get("editor.dialog.openFile"), Persistence.GetLastUsedFolder(), "adofai", multiselect: false);
			yield return null;
			if (levelPaths.Length == 0 || string.IsNullOrEmpty(levelPaths[0]))
			{
				yield break;
			}
			string text = SanitizeLevelPath(levelPaths[0]);
			customLevel.levelPath = text;
		}
		else
		{
			customLevel.levelPath = definedLevelPath;
		}
		scrController.deaths = 0;
		Persistence.UpdateLastUsedFolder(ADOBase.levelPath);
		Persistence.UpdateLastOpenedLevel(ADOBase.levelPath);
		bool flag = false;
		LoadResult status = LoadResult.Error;
		string text2 = "";
		isLoading = true;
		try
		{
			flag = customLevel.LoadLevel(ADOBase.levelPath, out status);
		}
		catch (Exception ex)
		{
			text2 = "Error loading level file at " + ADOBase.levelPath + ": " + ex.Message + ", Stacktrace:\n" + ex.StackTrace;
			Debug.Log(text2);
		}
		if (flag)
		{
			errorImageResult.Clear();
			isUnauthorizedAccess = false;
			RemakePath();
			SelectFirstFloor();
			UpdateSongAndLevelSettings();
			customLevel.ReloadAssets();
			UpdateDecorationObjects();
			DiscordController.instance?.UpdatePresence();
			ShowNotification(RDString.Get("editor.notification.levelLoaded"));
			unsavedChanges = false;
		}
		else
		{
			customLevel.levelPath = lastLevelPath;
			ShowNotificationPopup(text2, new NotificationAction[2]
			{
				new NotificationAction(RDString.Get("editor.notification.copyText"), delegate
				{
					notificationPopupContent.text.CopyToClipboard();
					ShowNotification(RDString.Get("editor.notification.copiedText"));
				}),
				new NotificationAction(RDString.Get("editor.ok"), delegate
				{
					CloseNotificationPopup();
				})
			}, RDString.Get($"editor.notification.loadingFailed.{status}"));
		}
		isLoading = false;
		CloseAllPanels();
		yield return null;
		ShowImageLoadResult();
	}

	private void OpenRecent(bool checkCtrl = false)
	{
		string recentLevel = Persistence.GetLastOpenedLevel();
		if (File.Exists(recentLevel) && (!checkCtrl || !OpenDirectory(recentLevel)))
		{
			CheckUnsavedChanges(delegate
			{
				StartCoroutine(OpenLevelCo(recentLevel));
			});
		}
	}

	private bool OpenDirectory(string path)
	{
		if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
		{
			RDEditorUtils.RevealInExplorer(Path.GetDirectoryName(path));
			return true;
		}
		return false;
	}

	private string GetDataPathFromURL(string url)
	{
		int num = url.LastIndexOf('/') + 1;
		string path = url.Substring(num, url.Length - num);
		string text = Persistence.DataPath + "/Temp";
		if (!Directory.Exists(text))
		{
			RDDirectory.CreateDirectory(text);
		}
		string text2 = Path.Combine(text, path).Replace("?", "").Replace("=", "");
		RDBaseDll.printem(text2);
		return text2;
	}

	private string FindAdofaiLevelOnDirectory(string path)
	{
		string[] files = Directory.GetFiles(path, "*.adofai", SearchOption.AllDirectories);
		if (files.Length == 0)
		{
			printe("level was not found");
			return null;
		}
		string text = null;
		for (int i = 0; i < files.Length; i++)
		{
			if (!(Path.GetFileName(files[i]) == "backup.adofai") && !Path.GetFileName(files[i]).StartsWith("."))
			{
				text = files[i];
				MonoBehaviour.print("selected file: " + text);
				break;
			}
		}
		if (text == null)
		{
			MonoBehaviour.print("was null");
			return null;
		}
		return text;
	}

	private void StartLevelDownload()
	{
		downloadCo = StartCoroutine(OpenLevelFromURL());
	}

	private void CancelDownload()
	{
		if (downloadingLevel)
		{
			downloadingLevel = false;
			StopCoroutine(downloadCo);
			if (www != null)
			{
				www.Dispose();
			}
			popupURLDownload.interactable = true;
			popupURLDownload.GetComponentInChildren<Text>().text = RDString.Get("editor.dialog.download");
		}
		else
		{
			ShowPopup(show: false);
		}
	}

	public IEnumerator OpenLevelFromURL()
	{
		string url = levelLinkInput.text;
		printe("DownloadLevel: " + url);
		www = UnityWebRequest.Get(url);
		downloadingLevel = true;
		printe("Downloading: true");
		popupURLDownload.interactable = false;
		popupURLDownload.GetComponentInChildren<Text>().text = RDString.Get("editor.dialog.downloading");
		yield return www.SendWebRequest();
		if (www.HasConnectionError())
		{
			ShowNotificationPopup(RDString.Get("editor.notification.downloadFailed"));
		}
		printe("Downloading: false");
		popupURLDownload.interactable = true;
		popupURLDownload.GetComponentInChildren<Text>().text = RDString.Get("editor.dialog.download");
		string dataPathFromURL = GetDataPathFromURL(url);
		if (RDFile.Exists(dataPathFromURL))
		{
			RDFile.Delete(dataPathFromURL);
		}
		byte[] data = www.downloadHandler.data;
		RDFile.WriteAllBytes(dataPathFromURL, data);
		www.Dispose();
		string text = dataPathFromURL + "_unzip";
		RDBaseDll.printem(dataPathFromURL + " " + text);
		if (Directory.Exists(text))
		{
			Directory.Delete(text, recursive: true);
		}
		RDDirectory.CreateDirectory(text);
		bool flag = false;
		try
		{
			ZipUtil.Unzip(dataPathFromURL, text);
			flag = true;
		}
		catch (Exception ex)
		{
			ShowNotificationPopup(RDString.Get("editor.notification.unzipFailed"));
			Debug.LogError("Unzip failed: " + ex.ToString());
			printe("there was an exception");
		}
		if (!flag)
		{
			Directory.Delete(text, recursive: true);
			RDFile.Delete(dataPathFromURL);
			yield break;
		}
		string text2 = FindAdofaiLevelOnDirectory(text);
		if (text2 != null)
		{
			PauseIfUnpaused();
			StartCoroutine(OpenLevelCo(text2));
			ShowPopup(show: false);
		}
		else
		{
			ShowNotificationPopup(RDString.Get("editor.notification.levelNotFound"));
			Directory.Delete(text, recursive: true);
			RDFile.Delete(dataPathFromURL);
		}
		downloadingLevel = false;
	}

	private void OnApplicationQuit()
	{
		string path = Persistence.DataPath + "/Temp";
		if (Directory.Exists(path))
		{
			Directory.Delete(path, recursive: true);
		}
	}

	private void ExportInternalLevel()
	{
		if (!levelData.isOldLevel)
		{
			return;
		}
		string text = levelData.pathData;
		int num = 0;
		foreach (LevelEvent @event in events)
		{
			if (@event.eventType == LevelEventType.Twirl)
			{
				text = text.Insert(@event.floor + num, "/");
				num++;
			}
			if (@event.eventType == LevelEventType.SetSpeed)
			{
				string value = ">";
				scrFloor scrFloor2 = floors[@event.floor];
				float num2 = ((scrFloor2.seqID <= 0) ? 1f : floors[scrFloor2.seqID - 1].speed);
				float speed = scrFloor2.speed;
				if (Mathf.Approximately(speed, num2 * 2f))
				{
					value = ">";
				}
				else if (Mathf.Approximately(speed, num2 / 0.75f))
				{
					value = "_";
				}
				else if (Mathf.Approximately(speed, num2 * 4f))
				{
					value = "*";
				}
				else if (Mathf.Approximately(speed, num2 / 2f))
				{
					value = "<";
				}
				else if (Mathf.Approximately(speed, num2 * 0.75f))
				{
					value = "-";
				}
				else if (Mathf.Approximately(speed, num2 / 4f))
				{
					value = "%";
				}
				text = text.Insert(@event.floor + num, value);
				num++;
			}
		}
		printe("Dev Level Data: " + text);
	}

	private void SaveLevel()
	{
		if (!string.IsNullOrEmpty(ADOBase.levelPath))
		{
			try
			{
				string data = levelData.Encode();
				RDFile.WriteAllText(ADOBase.levelPath, data);
				ShowNotification(RDString.Get("editor.notification.levelSaved"));
				unsavedChanges = false;
			}
			catch (Exception ex)
			{
				ShowNotificationPopup(RDString.Get("editor.notification.savingFailed"));
				Debug.Log("Failed saving at path " + ADOBase.levelPath + ": " + ex.Message);
				return;
			}
		}
		else
		{
			SaveLevelAs();
		}
		CloseAllPanels();
		if (Application.isEditor)
		{
			ExportInternalLevel();
		}
	}

	private void SaveLevelAs(bool newLevel = false)
	{
		StartCoroutine(SaveLevelAsCo(newLevel));
	}

	private IEnumerator SaveLevelAsCo(bool newLevel = false)
	{
		while (stallFileDialog)
		{
			yield return null;
		}
		string defaultName = ((newLevel || string.IsNullOrEmpty(customLevel.levelPath)) ? "level" : Path.GetFileNameWithoutExtension(customLevel.levelPath));
		StandaloneFileBrowser.SaveFilePanelAsync(RDString.Get("editor.dialog.saveLevel"), Persistence.GetLastUsedFolder(), defaultName, "adofai", delegate (string levelPath)
		{
			if (!string.IsNullOrEmpty(levelPath))
			{
				string text = SanitizeLevelPath(levelPath);
				customLevel.levelPath = text;
				RDBaseDll.printem("level path is now: " + customLevel.levelPath);
				RefreshFilenameText();
				Persistence.UpdateLastUsedFolder(levelPath);
				Persistence.UpdateLastOpenedLevel(levelPath);
				DiscordController.instance?.UpdatePresence();
				SaveLevel();
			}
		});
	}

	private void NewLevel()
	{
		CheckUnsavedChanges(delegate
		{
			ClearAllFloorOffsets();
			DeselectAnyUIGameObject();
			DeselectFloors();
			DeselectAllDecorations();
			propertyControlList.ClearDragCache();
			SaveLevelAs(newLevel: true);
			string text = ADOBase.levelPath;
			if (text == null || text.Length != 0)
			{
				levelData.Setup();
				events.Clear();
				scrDecorationManager.instance.ClearDecorations();
				levelData.decorations.Clear();
				selectedDecorations.Clear();
				propertyControlList.ClearCache();
				RemakePath();
				customLevel.ReloadSong();
				if (!GCS.standaloneLevelMode)
				{
					UpdateSongAndLevelSettings();
					customLevel.ReloadAssets();
					UpdateDecorationObjects();
				}
				ShowNotification(RDString.Get("editor.notification.levelReset"));
			}
			CloseAllPanels();
		});
	}

	public void ShowPopup(bool show, PopupType popupType = PopupType.SaveBeforeSongImport, bool skipAnim = false)
	{
		if (popupIsAnimating)
		{
			return;
		}
		printe($"show: {show}, type: {popupType}");
		showingPopup = show;
		if (show)
		{
			foreach (Transform item in popupWindow.transform)
			{
				item.gameObject.SetActive(value: false);
			}
			switch (popupType)
			{
				case PopupType.SaveBeforeSongImport:
				case PopupType.SaveBeforeImageImport:
				case PopupType.SaveBeforeVideoImport:
				case PopupType.SaveBeforeLevelExport:
					{
						string key = "";
						switch (popupType)
						{
							case PopupType.SaveBeforeSongImport:
								key = "editor.dialog.saveBeforeImportingSounds";
								break;
							case PopupType.SaveBeforeImageImport:
								key = "editor.dialog.saveBeforeImportingImages";
								break;
							case PopupType.SaveBeforeVideoImport:
								key = "editor.dialog.saveBeforeImportingVideos";
								break;
							case PopupType.SaveBeforeLevelExport:
								key = "editor.dialog.saveBeforeLevelExport";
								break;
						}
						string text2 = RDString.Get(key);
						savePopupContainer.SetActive(value: true);
						savePopupText.text = text2;
						break;
					}
				case PopupType.OpenURL:
					urlPopupContainer.SetActive(value: true);
					break;
				case PopupType.ExportLevel:
					publishWindow.windowContainer.SetActive(value: true);
					publishWindow.Init();
					ShowEventPicker(show: false);
					CloseAllInspectors();
					break;
				case PopupType.CopyrightWarning:
					ShowEventPicker(show: false);
					CloseAllInspectors();
					copyrightPopupContainer.SetActive(value: true);
					copyrightText.text = RDString.Get("editor.agreement");
					break;
				case PopupType.MultiPlanet:
					ShowEventPicker(show: false);
					CloseAllInspectors();
					multiplanetPopupContainer.SetActive(value: true);
					multiplanetPopupText.text = "Planets more than 3 works, but is an unreleased feature right now. If you're reading this, please do not release a mod to enable it or share footage, so we can keep the spoiler!";
					break;
				case PopupType.MissingExportParams:
					{
						paramsPopupContainer.SetActive(value: true);
						string text = "";
						foreach (string missingExportParam in GetMissingExportParams())
						{
							text = text + "- " + RDString.Get(missingExportParam, new Dictionary<string, object> {
					{
						"artist",
						"<b>" + levelData.artist + "</b>"
					} }) + "\n";
						}
						text = text.Replace("[artist]", "<b>" + levelData.artist + "</b>");
						paramsPopupText.text = text;
						break;
					}
				case PopupType.MissingFiles:
					{
						missingFilesPopupContainer.SetActive(value: true);
						List<string> missingFiles = GetMissingFiles();
						StringBuilder stringBuilder = new StringBuilder();
						foreach (string item2 in missingFiles)
						{
							stringBuilder.Append("- ").Append(item2).Append('\n');
						}
						missingFilesPopupText.text = stringBuilder.ToString();
						break;
					}
				case PopupType.OggEncode:
					oggPopupContainer.SetActive(value: true);
					popupOggCancel.interactable = true;
					popupOggConvert.interactable = true;
					oggConversionBar.gameObject.SetActive(value: false);
					oggConversionBarText.text = RDString.Get("editor.dialog.convert");
					break;
				case PopupType.ConversionSuccessful:
					okPopupContainer.SetActive(value: true);
					okPopupText.text = RDString.Get("editor.dialog.conversionSuccessful");
					break;
				case PopupType.ConversionError:
					okPopupContainer.SetActive(value: true);
					okPopupText.text = RDString.Get("editor.dialog.conversionSongNotFound");
					break;
				case PopupType.UnsavedChanges:
					unsavedChangesPopupContainer.SetActive(value: true);
					break;
				case PopupType.MacAppStoreFolderRestriction:
					okPopupContainer.SetActive(value: true);
					stallFileDialog = true;
					okPopupText.text = RDString.Get("editor.open.macAppStoreWarning");
					popupOkCallback = delegate
					{
						stallFileDialog = false;
						popupOkCallback = null;
						ShowPopup(show: false);
						popupIsAnimating = false;
					};
					break;
				case PopupType.MacAppStoreFileOutsideDownloads:
					okPopupContainer.SetActive(value: true);
					okPopupText.text = RDString.Get("editor.open.fileOutsideDownloads");
					break;
			}
		}
		popupPanel.SetActive(value: true);
		Image component = popupPanel.GetComponent<Image>();
		RectTransform component2 = popupWindow.GetComponent<RectTransform>();
		float num = (skipAnim ? 0f : 0.5f);
		float alpha = (show ? 0.5f : 0f);
		float num2 = ((popupType == PopupType.ExportLevel || popupType == PopupType.MissingExportParams) ? 450f : 200f);
		float num3 = 20f;
		float endValue = (show ? num3 : num2);
		component2.DOKill();
		component.DOKill();
		if (show)
		{
			component2.SetAnchorPosY(num2);
			component.color = Color.black.WithAlpha(0f);
		}
		component.DOColor(Color.black.WithAlpha(alpha), num / 2f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.Linear);
		popupIsAnimating = true;
		component2.DOAnchorPosY(endValue, num).SetUpdate(isIndependentUpdate: true).SetEase(show ? panelShowEase : panelHideEase)
			.OnComplete(delegate
			{
				popupIsAnimating = false;
				if (!show)
				{
					popupPanel.SetActive(value: false);
				}
			});
		CloseAllPanels();
	}

	public void ShowNotification(string text, Color? textColor = null, float delayDuration = 1.25f)
	{
		RectTransform rt = notificationText.GetComponent<RectTransform>();
		float duration = 0.5f;
		float hidePos = -135f;
		float endValue = 165f;
		if (notificationSeq != null && notificationSeq.active)
		{
			notificationSeq.Kill();
		}
		Text component = notificationText.GetComponent<Text>();
		component.text = text;
		component.color = textColor ?? Color.white;
		notificationSeq = DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(rt.DOAnchorPosX(hidePos, 0f))
			.Append(rt.DOAnchorPosX(endValue, duration).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutBack))
			.AppendInterval(delayDuration)
			.SetUpdate(isIndependentUpdate: true)
			.Append(rt.DOAnchorPosX(hidePos, duration).SetUpdate(isIndependentUpdate: true).SetEase(Ease.InQuad))
			.OnKill(delegate
			{
				rt.AnchorPosX(hidePos);
			});
	}

	private void ShowNotificationPopupBase(string text, string title = null)
	{
		notificationPopupContent.gameObject.GetComponent<ContentSizeFitter>().enabled = true;
		notificationPopupContent.text = text;
		notificationPopupContainer.SetActive(value: true);
		Canvas.ForceUpdateCanvases();
		int num = 630;
		float height = notificationPopupContent.rectTransform.rect.height;
		float num2 = height + 170f;
		if (!string.IsNullOrEmpty(title))
		{
			notificationPopupScrollview.offsetMax = new Vector2(-40f, -100f);
			notificationPopupTitle.text = title;
			notificationPopupTitle.gameObject.SetActive(value: true);
		}
		else
		{
			num += 60;
			num2 -= 60f;
			notificationPopupScrollview.offsetMax = new Vector2(-40f, -40f);
			notificationPopupTitle.gameObject.SetActive(value: false);
		}
		notificationPopupScrollviewContent.SizeDeltaY(height);
		if (height > (float)num)
		{
			height = num;
			num2 = 800f;
			notificationPopupScrollview.GetComponent<ScrollRect>().enabled = true;
		}
		else
		{
			notificationPopupScrollview.GetComponent<ScrollRect>().enabled = false;
			notificationPopupScrollviewVertical.SetActive(value: false);
			notificationPopupScrollviewHorizontal.SetActive(value: false);
		}
		notificationPopupWindow.SizeDeltaY(num2);
		notificationPopupContainer.SetActive(value: true);
		Image component = notificationPopupContainer.GetComponent<Image>();
		component.color = Color.black.WithAlpha(0f);
		notificationPopupWindow.anchoredPosition = new Vector2(0f, 450f);
		component.DOColor(Color.black.WithAlpha(0.5f), filePanelMoveDuration / 2f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.Linear);
		notificationPopupWindow.DOAnchorPosY(20f, filePanelMoveDuration).SetUpdate(isIndependentUpdate: true).SetEase(panelShowEase);
		foreach (Button notificationPopupAction in notificationPopupActions)
		{
			UnityEngine.Object.Destroy(notificationPopupAction.gameObject);
		}
		notificationPopupActions.Clear();
	}

	public void ShowNotificationPopup(string text, string title = null, Action callbackAction = null)
	{
		ShowNotificationPopupBase(text, title);
		notificationOkButton.gameObject.SetActive(value: true);
		notificationOkButton.onClick.AddListener(delegate
		{
			callbackAction?.Invoke();
			notificationOkButton.onClick.RemoveAllListeners();
			CloseNotificationPopup();
		});
	}

	public void ShowNotificationPopup(string text, NotificationAction[] notificationActions, string title = null)
	{
		ShowNotificationPopupBase(text, title);
		notificationOkButton.gameObject.SetActive(value: false);
		notificationPopupActions.Clear();
		for (int i = 0; i < notificationActions.Length; i++)
		{
			NotificationAction notificationAction = notificationActions[i];
			GameObject obj = UnityEngine.Object.Instantiate(notificationOkButton.gameObject);
			Button component = obj.GetComponent<Button>();
			Text componentInChildren = obj.GetComponentInChildren<Text>();
			obj.name = notificationAction.text;
			componentInChildren.text = notificationAction.text;
			obj.transform.SetParent(notificationPopupActionsContainer);
			obj.transform.ScaleXY(1f);
			obj.SetActive(value: true);
			notificationPopupActions.Add(component);
			component.onClick.AddListener(delegate
			{
				notificationAction.action?.Invoke();
			});
		}
	}

	private void CloseNotificationPopup()
	{
		notificationPopupWindow.DOAnchorPosY(450f, filePanelMoveDuration).SetUpdate(isIndependentUpdate: true).SetEase(panelHideEase)
			.OnComplete(delegate
			{
				notificationPopupContainer.SetActive(value: false);
			});
	}

	public void ShowEventsPage(int pageNum)
	{
		foreach (LevelEventButton item in eventButtons[currentCategory])
		{
			item.gameObject.SetActive(item.page == pageNum);
		}
	}

	public void ShowNextPage(bool moveToLast = false)
	{
		Array values = Enum.GetValues(typeof(LevelEventCategory));
		int num = Array.IndexOf(values, currentCategory);
		int eventCategory = (moveToLast ? (values.Length - 1) : ((num + 1) % values.Length));
		SetCategory((LevelEventCategory)eventCategory);
	}

	public void ShowPrevPage(bool moveToFirst = false)
	{
		Array values = Enum.GetValues(typeof(LevelEventCategory));
		int num = Array.IndexOf(values, currentCategory);
		int eventCategory = ((!moveToFirst) ? ((num - 1 < 0) ? (values.Length - 1) : (num - 1)) : 0);
		SetCategory((LevelEventCategory)eventCategory);
	}

	public void SetCategory(LevelEventCategory eventCategory, bool changedFavorites = false)
	{
		LevelEventButton[] componentsInChildren = levelEventsBar.GetComponentsInChildren<LevelEventButton>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.SetActive(value: false);
		}
		currentCategory = eventCategory;
		maxPage = (eventButtons[eventCategory].Count - 1) / 11;
		if (changedFavorites)
		{
			currentPage = Math.Min(currentPage, maxPage);
		}
		else
		{
			currentPage = 0;
		}
		ShowEventsPage(currentPage);
		ShowEventPicker(show: true);
	}

	public void AddFavoriteEvent(LevelEventType type)
	{
		if (!favoriteEvents.Contains(type))
		{
			favoriteEvents.Add(type);
			Persistence.SetFavoriteEditorEvents(favoriteEvents);
			SetupFavoritesCategory();
		}
	}

	public void RemoveFavoriteEvent(LevelEventType type)
	{
		if (favoriteEvents.Contains(type))
		{
			favoriteEvents.Remove(type);
			Persistence.SetFavoriteEditorEvents(favoriteEvents);
			SetupFavoritesCategory();
		}
	}

	public void SetupFavoritesCategory(bool firstTime = false)
	{
		if (firstTime)
		{
			favoriteEvents = Persistence.GetFavoriteEditorEvents();
		}
		else
		{
			foreach (LevelEventButton item in eventButtons[LevelEventCategory.Favorites])
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
			eventButtons[LevelEventCategory.Favorites].Clear();
		}
		int num = 0;
		foreach (LevelEventInfo value in GCS.levelEventsInfo.Values)
		{
			LevelEventType levelEventType = RDUtils.ParseEnum(value.name, LevelEventType.None);
			if (levelEventType != LevelEventType.ChangeTrack && levelEventType != LevelEventType.AddDecoration && levelEventType != LevelEventType.AddText && favoriteEvents.Contains(levelEventType))
			{
				GameObject obj = UnityEngine.Object.Instantiate(prefab_levelEventButton, levelEventsBarButtons);
				RectTransform component = obj.GetComponent<RectTransform>();
				float num2 = 0f + (0f + component.sizeDelta.x) * (float)(num % 11);
				component.SetAnchorPosX(num2);
				LevelEventButton component2 = obj.GetComponent<LevelEventButton>();
				component2.Init(levelEventType, num / 11, num % 11 + 1);
				eventButtons[LevelEventCategory.Favorites].Add(component2);
				num++;
			}
		}
		if (!firstTime)
		{
			SetCategory(currentCategory, changedFavorites: true);
		}
	}

	public void DecideInspectorTabsAtSelected()
	{
		if (SelectionIsSingle())
		{
			levelEventsPanel.ShowTabsForFloor(selectedFloors[0].seqID);
		}
	}

	public void AddEventAtSelected(LevelEventType eventType)
	{
		if (!SelectionIsSingle() || (lockPathEditing && !whitelistedEvents.Contains(eventType)))
		{
			return;
		}
		int sequenceID = selectedFloors[0].seqID;
		if (sequenceID == 0)
		{
			return;
		}
		if ((eventType == LevelEventType.Hold && events.Exists((LevelEvent x) => x.eventType == LevelEventType.Pause && x.floor == sequenceID)) || (eventType == LevelEventType.Pause && events.Exists((LevelEvent x) => x.eventType == LevelEventType.Hold && x.floor == sequenceID)))
		{
			ShowNotification(RDString.Get("editor.errorHeldBeatPausedBeat"));
			return;
		}
		if ((eventType == LevelEventType.FreeRoam && events.Exists((LevelEvent x) => x.eventType == LevelEventType.Twirl && x.floor == sequenceID)) || (eventType == LevelEventType.Twirl && events.Exists((LevelEvent x) => x.eventType == LevelEventType.FreeRoam && x.floor == sequenceID)))
		{
			ShowNotification(RDString.Get("editor.errorFreeroamTwirl"));
			return;
		}
		if (eventType == LevelEventType.FreeRoam && selectedFloors[0].nextfloor == null)
		{
			ShowNotification(RDString.Get("editor.errorFreeroamAtEnd"));
			return;
		}
		using (new SaveStateScope(this))
		{
			LevelEvent levelEvent = events.Find((LevelEvent x) => x.eventType == eventType && x.floor == sequenceID);
			bool flag = Array.Exists(EditorConstants.toggleableTypes, (LevelEventType element) => element == eventType);
			if (levelEvent != null && Array.Exists(EditorConstants.soloTypes, (LevelEventType element) => element == eventType) && !flag)
			{
				return;
			}
			if (eventType == LevelEventType.FreeRoam && !events.Exists((LevelEvent x) => x.eventType == LevelEventType.PositionTrack && x.floor == sequenceID + 1))
			{
				LevelEvent levelEvent2 = new LevelEvent(selectedFloors[0].seqID + 1, LevelEventType.PositionTrack);
				levelEvent2.data["positionOffset"] = new Vector2(4f * Mathf.Sin((float)selectedFloors[0].exitangle), 4f * Mathf.Cos((float)selectedFloors[0].exitangle));
				levelEvent2.data["rotation"] = 0f;
				levelEvent2.data["scale"] = 100;
				levelEvent2.data["opacity"] = 100;
				events.Add(levelEvent2);
				LevelEvent item = new LevelEvent(selectedFloors[0].seqID + 1, LevelEventType.MoveCamera);
				events.Add(item);
			}
			if (flag && levelEvent != null)
			{
				RemoveEvent(levelEvent);
				DecideInspectorTabsAtSelected();
			}
			else
			{
				AddEvent(sequenceID, eventType);
				levelEventsPanel.selectedEventType = eventType;
				int count = events.FindAll((LevelEvent x) => x.eventType == eventType && x.floor == sequenceID).Count;
				if (count == 1)
				{
					DecideInspectorTabsAtSelected();
					levelEventsPanel.ShowPanel(eventType);
				}
				else
				{
					levelEventsPanel.ShowPanel(eventType, count - 1);
				}
			}
			ApplyEventsToFloors();
			ShowEventIndicators(selectedFloors[0]);
		}
	}

	public void AddEvent(int floorID, LevelEventType eventType)
	{
		if (floorID == 0)
		{
			return;
		}
		LevelEvent newLevelEvent = new LevelEvent(floorID, eventType);
		LevelEvent selectedEvent = levelEventsPanel.selectedEvent;
		if (selectedEvent != null && selectedEvent.data.ContainsKey("angleOffset") && newLevelEvent.data.ContainsKey("angleOffset"))
		{
			newLevelEvent["angleOffset"] = selectedEvent["angleOffset"];
		}
		events.Add(newLevelEvent);
		if (eventType == LevelEventType.SetHitsound)
		{
			List<LevelEvent> list = events.FindAll((LevelEvent e) => e.eventType == LevelEventType.SetHitsound).FindAll((LevelEvent e) => e.data["gameSound"] == newLevelEvent.data["gameSound"]);
			list.Sort((LevelEvent a, LevelEvent b) => a.floor.CompareTo(b.floor));
			LevelEvent levelEvent = list.FindLast((LevelEvent e) => e.floor < floorID);
			newLevelEvent.data["hitsoundVolume"] = ((levelEvent != null) ? levelEvent.data["hitsoundVolume"] : ((object)100));
		}
	}

	public void AddDecoration(LevelEventType eventType)
	{
		LevelEvent dec = CreateDecoration(eventType);
		AddDecoration(dec);
	}

	public void AddDecoration(LevelEvent dec)
	{
		levelData.decorations.Add(dec);
		selectedDecorations.Add(dec);
		refreshDecSprites = true;
		propertyControlList.OnDecorationUpdate();
	}

	private LevelEvent CreateDecoration(LevelEventType eventType)
	{
		LevelEvent levelEvent = new LevelEvent(-1, eventType);
		Vector3 position = Camera.main.transform.position;
		if (selectedFloors.Count == 1)
		{
			levelEvent.data["relativeTo"] = DecPlacementType.Tile;
			levelEvent.floor = selectedFloors[0].seqID;
			levelEvent.data["position"] = Vector2.zero;
		}
		else
		{
			levelEvent.data["position"] = new Vector2(position.x, position.y) / customLevel.GetTileSize();
		}
		return levelEvent;
	}

	public void RemoveEvent(LevelEvent evnt)
	{
		if (evnt == null)
		{
			return;
		}
		using (new SaveStateScope(this))
		{
			if (evnt.eventType == LevelEventType.AddDecoration || evnt.eventType == LevelEventType.AddText)
			{
				levelData.decorations.Remove(evnt);
				selectedDecorations.Remove(evnt);
				UpdateDecorationObjects();
				levelEventsPanel.ShowPanel(LevelEventType.None);
				levelEventsPanel.HideAllInspectorTabs();
				refreshBgSprites = true;
			}
			else
			{
				events.Remove(evnt);
			}
			if (EventHasBackgroundSprite(evnt))
			{
				refreshBgSprites = true;
			}
		}
	}

	public void RemoveEventAtSelected(LevelEventType eventType)
	{
		switch (eventType)
		{
			case LevelEventType.None:
				return;
			case LevelEventType.AddDecoration:
			case LevelEventType.AddText:
				DeleteMultiSelectionDecorations();
				return;
		}
		int num = levelEventsPanel.EventNumOfTab(eventType);
		List<LevelEvent> selectedFloorEvents = GetSelectedFloorEvents(eventType);
		if ((lockPathEditing && !whitelistedEvents.Contains(eventType)) || selectedFloorEvents == null || num >= selectedFloorEvents.Count)
		{
			return;
		}
		using (new SaveStateScope(this))
		{
			RemoveEvent(selectedFloorEvents[num]);
			int num2 = selectedFloorEvents.Count - 1;
			if (num2 > 0)
			{
				num = Mathf.Clamp(num, 0, num2 - 1);
				levelEventsPanel.ShowPanel(eventType, num);
			}
			else
			{
				DecideInspectorTabsAtSelected();
			}
			ApplyEventsToFloors();
			ShowEventIndicators(selectedFloors[0]);
			floorButtonCanvas.transform.position = selectedFloors[0].transform.position;
			if (eventType == LevelEventType.Hold)
			{
				RemakePath();
			}
		}
	}

	private void OnGUI()
	{
		if (RDC.debug)
		{
			VideoPlayer videoBG = scrVfxPlus.instance.videoBG;
			string arg = ((videoBG != null) ? videoBG.time.ToString() : "none");
			string text = $"song.time: {ADOBase.conductor.song.time}\nvideoBg.time: {arg}";
			text += $"\n {videoBG != null} && {videoBG.gameObject.activeSelf} && {!videoBG.isPlaying} && {!scrVfxPlus.instance.hasPlayed} && {videoBG.isPrepared}";
			GUI.Label(new Rect(0f, 300f, 200f, 200f), text);
			Rect position = new Rect(200f, 0f, 1000f, 1000f);
			string text2 = "scnEditor: ";
			text2 = text2 + "\n" + customLevel.imgHolder.ToString();
			GUI.Label(position, text2);
		}
	}

	public void LoadEditorProperties()
	{
		LevelEventType[] source = new LevelEventType[3]
		{
			LevelEventType.ChangeTrack,
			LevelEventType.AddDecoration,
			LevelEventType.AddText
		};
		Dictionary<LevelEventCategory, int> dictionary = new Dictionary<LevelEventCategory, int>();
		foreach (LevelEventCategory value in Enum.GetValues(typeof(LevelEventCategory)))
		{
			dictionary[value] = 0;
			eventButtons[value] = new List<LevelEventButton>();
		}
		foreach (LevelEventInfo value2 in GCS.levelEventsInfo.Values)
		{
			LevelEventType levelEventType = RDUtils.ParseEnum(value2.name, LevelEventType.None);
			if (source.Contains(levelEventType) || !value2.isActive)
			{
				continue;
			}
			foreach (LevelEventCategory category in value2.categories)
			{
				int num = dictionary[category];
				if (levelEventType != LevelEventType.FreeRoamWarning)
				{
					GameObject obj = UnityEngine.Object.Instantiate(prefab_levelEventButton, levelEventsBarButtons);
					RectTransform component = obj.GetComponent<RectTransform>();
					float num2 = 0f + (0f + component.sizeDelta.x) * (float)(num % 11);
					component.SetAnchorPosX(num2);
					LevelEventButton component2 = obj.GetComponent<LevelEventButton>();
					component2.Init(levelEventType, num / 11, num % 11 + 1);
					eventButtons[category].Add(component2);
					dictionary[category]++;
				}
			}
		}
		int num3 = 0;
		foreach (LevelEventCategory value3 in Enum.GetValues(typeof(LevelEventCategory)))
		{
			if (eventButtons[value3].Count == 0 && value3 != LevelEventCategory.Favorites)
			{
				num3++;
				continue;
			}
			GameObject obj2 = UnityEngine.Object.Instantiate(prefab_eventCategoryTab, levelEventsBarCategories);
			obj2.GetComponent<RectTransform>().SetAnchorPosY(55f);
			CategoryTab component3 = obj2.GetComponent<CategoryTab>();
			component3.Init(value3);
			categoryTabs.Add(component3);
			num3++;
		}
		SetupFavoritesCategory(firstTime: true);
		SetCategory(currentCategory);
		settingsPanel.Init(GCS.settingsInfo, floorPanel: false);
		levelEventsPanel.Init(GCS.levelEventsInfo, floorPanel: true);
	}

	private void ToggleAuto()
	{
		RDC.auto = !RDC.auto;
		autoFailed = false;
		if (RDC.auto)
		{
			if (highBPM)
			{
				ottoAudioSrc.clip = ADOBase.gc.soundEffects[5];
			}
			else
			{
				ottoAudioSrc.clip = ADOBase.gc.soundEffects[5];
			}
			ShowNotification(RDString.Get("editor.notification.autoplayEnabled"));
		}
		else
		{
			ottoAudioSrc.clip = ADOBase.gc.soundEffects[7];
			ShowNotification(RDString.Get("editor.notification.autoplayDisabled"));
		}
		ottoAudioSrc.Play();
	}

	private void OttoUpdate()
	{
		if (RDEditorUtils.CheckPointerInObject(buttonAuto))
		{
			OttoPetUpdate();
		}
		else
		{
			autoPetTime = 0f;
		}
		Color color = (highBPM ? Color.red : Color.white);
		if (isOttoBlinking)
		{
			return;
		}
		if (RDC.auto)
		{
			if (!autoFailed)
			{
				if (autoPetTime < 1.5f)
				{
					autoImage.sprite = ((highBPM && paused) ? autoSprites[7] : autoSprites[1]);
				}
				else
				{
					autoImage.sprite = autoSprites[9];
				}
			}
			else
			{
				autoImage.sprite = autoSprites[6];
			}
			autoImage.color = color;
		}
		else
		{
			autoImage.sprite = ((highBPM && paused) ? autoSprites[8] : autoSprites[0]);
			autoImage.color = grayColor * color;
		}
	}

	public void OttoBlink()
	{
		float interval = 60f / (ADOBase.conductor.bpm * ADOBase.conductor.song.pitch * (float)ADOBase.controller.speed) / 2f;
		if (ADOBase.controller.currFloor.holdLength > -1 && ADOBase.controller.currFloor.nextfloor != null)
		{
			interval = (float)ADOBase.controller.currFloor.nextfloor.entryTime - (float)ADOBase.controller.currFloor.entryTime;
		}
		int num = ((!RDC.auto) ? ((ottoBlinkCounter % 2 == 0) ? 5 : 4) : ((ottoBlinkCounter % 2 == 0) ? 3 : 2));
		ottoBlinkCounter++;
		autoImage.sprite = autoSprites[num];
		if (blinkTimer != null && blinkTimer.active)
		{
			blinkTimer.Kill();
		}
		blinkTimer = DOTween.Sequence().AppendInterval(interval).OnComplete(delegate
		{
			isOttoBlinking = false;
		})
			.SetUpdate(isIndependentUpdate: true)
			.OnKill(delegate
			{
				isOttoBlinking = false;
			});
		isOttoBlinking = true;
	}

	private void OttoPetUpdate()
	{
		if (RDC.auto)
		{
			if (Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f)
			{
				autoPetTime += Time.unscaledDeltaTime;
			}
		}
		else
		{
			autoPetTime = 0f;
		}
	}

	private void ToggleNoFail()
	{
		ADOBase.controller.noFail = !ADOBase.controller.noFail;
		if (ADOBase.controller.noFail)
		{
			buttonNoFail.GetComponent<Image>().color = Color.white;
			interfaceAudioSrc.PlayOneShot(ADOBase.gc.soundEffects[10]);
			ShowNotification(RDString.Get("editor.notification.noFailEnabled"));
		}
		else
		{
			buttonNoFail.GetComponent<Image>().color = grayColor;
			interfaceAudioSrc.PlayOneShot(ADOBase.gc.soundEffects[11]);
			ShowNotification(RDString.Get("editor.notification.noFailDisabled"));
		}
	}

	private void DeleteSubsequentFloors()
	{
		if (selectedFloors.Count == 0)
		{
			return;
		}
		using (new SaveStateScope(this))
		{
			int seqID = selectedFloors[0].seqID;
			for (int num = floors.Count() - 2; num > seqID; num--)
			{
				DeleteFloor(seqID + 1, remakePath: false);
			}
			DeleteFloor(seqID + 1);
			SelectFloor(floors[seqID]);
		}
	}

	private void DeletePrecedingFloors()
	{
		if (selectedFloors.Count == 0)
		{
			return;
		}
		using (new SaveStateScope(this))
		{
			int num = selectedFloors[0].seqID;
			if (num != 0)
			{
				while (num > 1)
				{
					DeleteFloor(1, remakePath: false);
					num--;
				}
				DeleteFloor(1);
				SelectFloor(floors[0]);
			}
		}
	}

	private void DeleteSingleSelection(bool backspace = true)
	{
		if (lockPathEditing)
		{
			return;
		}
		int seqID = selectedFloors[0].seqID;
		if ((backspace && seqID == 0) || (!backspace && seqID == floors.Count - 1))
		{
			return;
		}
		using (new SaveStateScope(this))
		{
			if (backspace)
			{
				if (DeleteFloor(seqID))
				{
					SelectFloor(floors[seqID - 1]);
				}
			}
			else if (DeleteFloor(seqID + 1))
			{
				SelectFloor(floors[seqID]);
			}
		}
	}

	private void DeleteMultiSelection(bool backspace = true)
	{
		if (lockPathEditing || selectedFloors[0].seqID == 0)
		{
			return;
		}
		using (new SaveStateScope(this))
		{
			int num = selectedFloors[0].seqID;
			int num2 = selectedFloors.Last().seqID;
			ClearMultiSelection();
			while (num < num2)
			{
				if (DeleteFloor(num, remakePath: false))
				{
					num2--;
				}
				else
				{
					num++;
				}
			}
			DeleteFloor(num);
			if (backspace)
			{
				SelectFloor(floors[num - 1]);
			}
			else if (num == floors.Count())
			{
				SelectFloor(floors[num - 1]);
			}
			else
			{
				SelectFloor(floors[num]);
			}
		}
	}

	public void DeleteMultiSelectionDecorations()
	{
		if (SelectionDecorationIsEmpty())
		{
			return;
		}
		using (new SaveStateScope(this))
		{
			HashSet<LevelEvent> setToRemove = new HashSet<LevelEvent>(selectedDecorations);
			levelData.decorations.RemoveAll((LevelEvent x) => setToRemove.Contains(x));
			selectedDecorations.Clear();
			UpdateDecorationObjects();
			levelEventsPanel.ShowPanel(LevelEventType.None);
			levelEventsPanel.HideAllInspectorTabs();
			refreshBgSprites = true;
		}
	}

	public List<LevelEvent> GetSelectedFloorEvents(LevelEventType eventType)
	{
		return GetFloorEvents(selectedFloors[0].seqID, eventType);
	}

	public List<LevelEvent> GetFloorEvents(int floorID, LevelEventType eventType)
	{
		if (eventType.IsSetting())
		{
			return null;
		}
		List<LevelEvent> list = new List<LevelEvent>();
		foreach (LevelEvent @event in events)
		{
			if (floorID == @event.floor && @event.eventType == eventType)
			{
				list.Add(@event);
			}
		}
		return list;
	}

	private void ShowEventPicker(bool show)
	{
		float endValue = (show ? 0f : (0f - levelEventsBar.sizeDelta.y - levelEventsBarCategories.sizeDelta.y - 5f));
		levelEventsBar.DOAnchorPosY(endValue, 0.25f).SetUpdate(isIndependentUpdate: true).SetEase(UIPanelEaseMode);
		foreach (CategoryTab categoryTab in categoryTabs)
		{
			categoryTab.SetSelected(show && currentCategory == categoryTab.levelEventCategory);
		}
		if (!show)
		{
			eventPickerText.text = "";
			categoryText.text = "";
		}
	}

	private void SaveBackup()
	{
		if (playMode || string.IsNullOrEmpty(ADOBase.levelPath))
		{
			return;
		}
		if (saveBackupLastFrame == saveStateLastFrame)
		{
			printe("not going to saveBackup because state has not changed");
		}
		else if (unsavedChanges)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(ADOBase.levelPath);
			string text = "backup";
			if (fileNameWithoutExtension == text)
			{
				text = "backup2";
			}
			string data = levelData.Encode();
			try
			{
				RDFile.WriteAllText(Path.GetDirectoryName(ADOBase.levelPath) + "/" + text + ".adofai", data);
			}
			catch (Exception)
			{
			}
		}
	}

	private void OpenDiscord()
	{
		Application.OpenURL("https://discord.gg/rhythmdr");
	}

	private Color[] SelectedColors(Color color)
	{
		Color[] array = new Color[2];
		new Color(1f - color.r, 1f - color.g, 1f - color.b);
		Color.RGBToHSV(color, out var H, out var S, out var V);
		H = (H + 0.25f) % 1f;
		V = Mathf.Clamp01(V * 1.2f);
		float s = Mathf.Clamp01(S + 0.5f);
		float v = ((V > 0.75f) ? (V - 0.25f) : (V + 0.25f));
		array[0] = Color.HSVToRGB(H, S, V);
		array[1] = Color.HSVToRGB(H, s, v);
		return array;
	}

	public void Undo()
	{
		UndoOrRedo(redo: false);
	}

	public void Redo()
	{
		UndoOrRedo(redo: true);
	}

	public void UndoOrRedo(bool redo)
	{
		if (changingState != 0)
		{
			return;
		}
		List<LevelState> list = (redo ? redoStates : undoStates);
		if (list.Count <= 0)
		{
			return;
		}
		using (new SaveStateScope(this))
		{
			if (!redo)
			{
				redoStates.Add(undoStates.Pop());
			}
			LevelState levelState = list.Last();
			int[] selectedDecorationIndices = levelState.selectedDecorationIndices;
			if (levelState.data != null)
			{
				customLevel.levelData = levelState.data;
				DeselectFloors();
				DeselectAllDecorations();
				RemakePath();
				int[] array = selectedDecorationIndices;
				foreach (int index in array)
				{
					selectedDecorations.Add(levelState.data.decorations[index]);
				}
			}
			List<int> list2 = levelState.selectedFloors;
			UpdateDecorationObjects();
			propertyControlList.UpdateItemList(forceRefreshAll: true);
			if (list2.Count > 1)
			{
				MultiSelectFloors(floors[list2[0]], floors[list2[list2.Count - 1]]);
			}
			else if (list2.Count == 1)
			{
				int index2 = list2[0];
				SelectFloor(floors[index2]);
				levelEventsPanel.ShowPanel(levelState.floorEventType, levelState.floorEventTypeIndex);
			}
			settingsPanel.ShowPanel(levelState.settingsEventType);
			list.RemoveAt(list.Count - 1);
		}
	}

	public void SaveState(bool clearRedo = true, bool onlySelectionChanged = false)
	{
		onlySelectionChanged = false;
		if (changingState != 0 || !initialized)
		{
			return;
		}
		List<int> list = new List<int>();
		if (!SelectionIsEmpty())
		{
			if (SelectionIsSingle())
			{
				list.Add(selectedFloors[0].seqID);
			}
			else
			{
				foreach (scrFloor selectedFloor in selectedFloors)
				{
					list.Add(selectedFloor.seqID);
				}
			}
		}
		LevelData data = levelData.Copy();
		List<scrDecoration> allDecorations = scrDecorationManager.instance.allDecorations;
		int[] array = new int[selectedDecorations.Count];
		for (int i = 0; i < selectedDecorations.Count; i++)
		{
			using (List<scrDecoration>.Enumerator enumerator2 = allDecorations.GetEnumerator())
			{
				while (enumerator2.MoveNext() && enumerator2.Current.sourceLevelEvent != selectedDecorations[i])
				{
					array[i]++;
				}
			}
			if (array[i] == allDecorations.Count)
			{
				array[i] = -1;
			}
		}
		LevelState levelState = new LevelState(data, list, array);
		levelState.settingsEventType = settingsPanel.selectedEventType;
		levelState.floorEventType = levelEventsPanel.selectedEventType;
		levelState.floorEventTypeIndex = levelEventsPanel.EventNumOfTab(levelState.floorEventType);
		undoStates.Add(levelState);
		if (clearRedo)
		{
			redoStates.Clear();
		}
		if (undoStates.Count > 100)
		{
			undoStates.RemoveAt(0);
		}
		if (!onlySelectionChanged)
		{
			unsavedChanges = true;
		}
		saveStateLastFrame = Time.frameCount;
	}

	public void ApplyEventsToFloors()
	{
		customLevel.ApplyEventsToFloors(floors);
		DrawFloorOffsetLines();
		DrawHolds();
		DrawMultiPlanet();
		refreshDecSprites = true;
	}

	public void Play()
	{
		if (floors.Count == 1)
		{
			return;
		}
		scrController.instance.UnlockInput();
		ottoBlinkCounter = 0;
		playbackSpeed = ((GCS.editorQuickPitchedPlaying = RDInput.holdingControl) ? ((float)Persistence.GetShortcutPlaySpeed() / 100f) : 1f);
		decorationWasSelected = false;
		int num;
		if (SelectionIsSingle())
		{
			num = selectedFloors[0].seqID;
		}
		else if (!SelectionDecorationIsEmpty() && (bool)lastSelectedFloor)
		{
			num = lastSelectedFloor.seqID;
			decorationWasSelected = true;
			lastSelectedDecorations = new List<LevelEvent>(selectedDecorations);
		}
		else
		{
			num = 0;
		}
		selectedFloorCached = ((!decorationWasSelected) ? num : 0);
		DeselectAnyUIGameObject();
		DeselectFloors(skipSaving: true);
		DeselectAllDecorations();
		HidePopupBlocker();
		ClearFloorGlows();
		ADOBase.conductor.gameObject.SetActive(value: true);
		RemakePath(applyEventsToFloors: false);
		customLevel.ApplyCoreEventsToFloors(floors);
		foreach (GameObject floorConnectorGO in floorConnectorGOs)
		{
			UnityEngine.Object.Destroy(floorConnectorGO);
		}
		floorConnectorGOs.Clear();
		foreach (scrFloor floor in floors)
		{
			floor.editorNumText.gameObject.SetActive(value: false);
		}
		ADOBase.controller.currentSeqID = 0;
		GCS.checkpointNum = num;
		customLevel.ReloadAssets();
		customLevel.Play(num);
		DrawHolds();
		DrawMultiPlanet();
		editorDifficultySelector.SetChangeable(changeable: false);
		buttonNoFail.interactable = false;
		scrDecorationManager.instance.ShowEmptyDecorations(show: false);
		if (Persistence.GetHideCursorWhilePlaying())
		{
			Cursor.visible = false;
		}
	}

	public void ShowExportWindow(int state)
	{
		if (ADOBase.levelPath.IsNullOrEmpty())
		{
			ShowPopup(show: true, PopupType.SaveBeforeLevelExport);
		}
		else if (GetMissingExportParams().Count > 0)
		{
			ShowPopup(show: true, PopupType.MissingExportParams);
		}
		else if (!levelData.songFilename.IsNullOrEmpty() && levelData.songFilename.Split('.').Last() == "mp3")
		{
			songToConvert = Path.Combine(Path.GetDirectoryName(ADOBase.levelPath), levelData.songFilename);
			ShowPopup(show: true, PopupType.OggEncode);
		}
		else if (GetMissingFiles().Count > 0)
		{
			ShowPopup(show: true, PopupType.MissingFiles);
		}
		else
		{
			ShowPopup(show: true, PopupType.ExportLevel);
		}
	}

	private string GetExportLevelTempDirectory()
	{
		string path = Path.Combine(Persistence.DataPath, "Temp");
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
		return RDUtils.GetAvailableDirectoryName(Path.Combine(path, fileNameWithoutExtension));
	}

	private List<string> GetExportLevelFiles(string tempDir)
	{
		bool uploadable;
		uint[] requiredDLC;
		return GetExportLevelFiles(tempDir, out uploadable, out requiredDLC);
	}

	private List<string> GetExportLevelFiles(string tempDir, out bool uploadable, out uint[] requiredDLC)
	{
		uploadable = true;
		string data = this.levelData.Encode();
		RDFile.WriteAllText(ADOBase.levelPath, data);
		List<string> includedFiles = new List<string>();
		List<string> list = new List<string>();
		List<uint> list2 = new List<uint>();
		string levelDir = Path.GetDirectoryName(ADOBase.levelPath);
		if (!Directory.Exists(tempDir))
		{
			RDDirectory.CreateDirectory(tempDir);
		}
		string text = Path.Combine(tempDir, "main.adofai");
		File.Copy(ADOBase.levelPath, text, overwrite: true);
		list.Add(text);
		string[] worldPaths = CustomLevel.GetWorldPaths(ADOBase.levelPath, excludeMain: true);
		string[] array = new string[worldPaths.Length];
		int num = 1;
		string[] array2 = worldPaths;
		foreach (string sourceFileName in array2)
		{
			string text2 = Path.Combine(tempDir, "sub" + num + ".adofai");
			File.Copy(sourceFileName, text2, overwrite: true);
			array[num - 1] = text2;
			num++;
		}
		list.AddRange(array);
		includedFiles.AddRange(list);
		AddFile(this.levelData.previewImage);
		AddFile(this.levelData.previewIcon);
		AddFile(this.levelData.artistPermission);
		foreach (string item in list)
		{
			Dictionary<string, object> dictionary = Json.Deserialize(RDFile.ReadAllText(item)) as Dictionary<string, object>;
			LevelData levelData = new LevelData();
			levelData.Setup();
			if (dictionary == null)
			{
				continue;
			}
			levelData.Decode(dictionary, out var status);
			if (status == LoadResult.ModRequired)
			{
				uploadable = false;
			}
			levelData.RefreshRequiredDLC();
			list2.AddMany(levelData.requiredDLC);
			AddFile(levelData.bgImage);
			AddFile(levelData.songFilename);
			AddFile(levelData.bgVideo);
			foreach (LevelEvent levelEvent in levelData.levelEvents)
			{
				switch (levelEvent.eventType)
				{
					case LevelEventType.CustomBackground:
						AddFile(levelEvent.GetString("bgImage"));
						break;
					case LevelEventType.ColorTrack:
						AddFile(levelEvent.GetString("trackTexture"));
						break;
					case LevelEventType.MoveDecorations:
						{
							string @string = levelEvent.GetString("decorationImage");
							if (!levelEvent.disabled["decorationImage"] && !@string.IsNullOrEmpty())
							{
								AddFile(@string);
							}
							break;
						}
				}
			}
			foreach (LevelEvent decoration in levelData.decorations)
			{
				if (decoration.eventType == LevelEventType.AddDecoration)
				{
					AddFile(decoration.GetString("decorationImage"));
				}
			}
		}
		requiredDLC = list2.Distinct().ToArray();
		return includedFiles;
		void AddFile(string filename)
		{
			if (!filename.IsNullOrEmpty())
			{
				includedFiles.Add(Path.Combine(levelDir, filename));
			}
		}
	}

	public string MakeThumbnail(uint[] requiredDLC = null)
	{
		string text = Path.Combine(Path.GetDirectoryName(ADOBase.levelPath), levelData.previewImage);
		Texture2D texture2D = ADOBase.gc.sprite_defaultPortal.texture;
		RDBaseDll.printem("filePath: " + text);
		if (File.Exists(text))
		{
			printe("load file " + text);
			byte[] data = File.ReadAllBytes(text);
			texture2D = new Texture2D(2, 2);
			texture2D.LoadImage(data);
		}
		return thumbnailMaker.MakeThumbnail(texture2D, levelData.previewIconColor, requiredDLC);
	}

	public void ExportLevel(bool uploadToSteam = false)
	{
		RDBaseDll.printem($"uploadToSteam: {uploadToSteam}");
		if (!uploadToSteam)
		{
			string text = StandaloneFileBrowser.SaveFilePanel(RDString.Get("editor.dialog.exportLevel"), Persistence.GetLastUsedFolder(), "level", "adofaizip");
			if (!string.IsNullOrEmpty(text))
			{
				string data = levelData.Encode();
				RDFile.WriteAllText(ADOBase.levelPath, data);
				string exportLevelTempDirectory = GetExportLevelTempDirectory();
				List<string> exportLevelFiles = GetExportLevelFiles(exportLevelTempDirectory);
				ZipUtil.Zip(text, exportLevelFiles.Distinct().ToArray());
				ShowNotification(RDString.Get("editor.notification.levelExported"));
			}
			else
			{
				Debug.Log("Export cancelled!");
			}
		}
		else
		{
			string exportLevelTempDirectory2 = GetExportLevelTempDirectory();
			bool uploadable;
			uint[] requiredDLC;
			List<string> exportLevelFiles2 = GetExportLevelFiles(exportLevelTempDirectory2, out uploadable, out requiredDLC);
			if (uploadable)
			{
				string thumbPath = MakeThumbnail(requiredDLC);
				StartCoroutine(PublishToSteam(exportLevelFiles2, exportLevelTempDirectory2, thumbPath, requiredDLC));
			}
		}
	}

	public void ExportLevelAsUpdate(PublishedFileId_t updateId)
	{
		string exportLevelTempDirectory = GetExportLevelTempDirectory();
		bool uploadable;
		uint[] requiredDLC;
		List<string> exportLevelFiles = GetExportLevelFiles(exportLevelTempDirectory, out uploadable, out requiredDLC);
		if (uploadable)
		{
			string thumbPath = MakeThumbnail(requiredDLC);
			StartCoroutine(PublishToSteam(exportLevelFiles, exportLevelTempDirectory, thumbPath, requiredDLC, updateId));
		}
	}

	public IEnumerator PublishToSteam(List<string> includedFiles, string tempDir, string thumbPath, uint[] requiredDLC, PublishedFileId_t updateId = default(PublishedFileId_t))
	{
		RDBaseDll.printes($"includedFiles = {includedFiles.Count} files to upload at {tempDir}");
		publishWindow.uploadInProgress = true;
		foreach (string includedFile in includedFiles)
		{
			if (!RDFile.Exists(includedFile))
			{
				printe("missing filepath: " + includedFile);
				ShowNotification(RDString.Get("editor.notification.exportFailed"));
				yield break;
			}
			string text = Path.Combine(tempDir, Path.GetFileName(includedFile));
			if (!RDFile.Exists(text))
			{
				RDFile.Copy(includedFile, text);
			}
		}
		List<string> list = new List<string>();
		list.Add(levelData.levelTags.Split(','));
		int difficulty = levelData.difficulty;
		string item = ((difficulty <= 3) ? "Easy" : ((difficulty <= 6) ? "Medium" : ((difficulty <= 9) ? "Tough" : "Very Tough")));
		list.Add(item);
		if (requiredDLC.Contains(1977570u))
		{
			list.Add("Neo Cosmos");
		}
		ApprovalLevel approvalLevel = ApprovalLevelForArtist(levelData.artist);
		SpecialArtistType specialArtistType = levelData.specialArtistType;
		if (levelData.seizureWarning)
		{
			list.Add("Seizure Warning");
		}
		if (CustomLevel.GetWorldPaths(ADOBase.levelPath, excludeMain: true).Length != 0)
		{
			list.Add("World");
		}
		if (approvalLevel != ApprovalLevel.Allowed && approvalLevel != ApprovalLevel.PartiallyDeclined)
		{
			if (specialArtistType == SpecialArtistType.AuthorIsArtist)
			{
				list.Add("Composed by me");
			}
			if (specialArtistType == SpecialArtistType.PublicLicense)
			{
				list.Add("Public License");
			}
		}
		if (approvalLevel == ApprovalLevel.Pending && specialArtistType == SpecialArtistType.None)
		{
			list.Add("New Artist");
		}
		yield return SteamWorkshop.UploadToWorkshop(levelData.fullCaption, levelData.levelDesc, thumbPath, tempDir, list.Distinct().ToArray(), updateId, requiredDLC);
		publishWindow.uploadInProgress = false;
		publishWindow.UpdateProgress();
		publishWindow.ChangePage(2);
		if (SteamWorkshop.OperationSuccess)
		{
			SteamWorkshop.Subscribe(SteamWorkshop.lastPublishedFileId);
			ShowNotification(RDString.Get("editor.notification.levelExported"));
			yield break;
		}
		foreach (SteamWorkshop.WorkshopError error in SteamWorkshop.errors)
		{
			RDBaseDll.printes(error.ToString());
		}
		ShowNotification(RDString.Get("editor.notification.exportFailed"));
	}

	private void AcceptAgreement()
	{
		ShowPopup(show: false, PopupType.CopyrightWarning, skipAnim: true);
		Persistence.SetAcceptedAgreement(accepted: true);
		Persistence.Save();
		ShowEventPicker(show: true);
		settingsPanel.ShowInspector(show: true);
	}

	private void DeclineAgreement()
	{
		ADOBase.controller.QuitToMainMenu();
	}

	public void ShowPropertyHelp(bool show, Transform location = null, string helpText = "", string buttonText = null, string buttonURL = null)
	{
		if (animatingPropertyHelp || show == showingPropertyHelp)
		{
			return;
		}
		animatingPropertyHelp = true;
		showingPropertyHelp = show;
		float duration = 0.15f;
		bool active = !string.IsNullOrEmpty(buttonURL);
		propertyHelpURLButton.gameObject.SetActive(active);
		if (show)
		{
			propertyHelpText.text = helpText;
			propertyHelpContainer.position = location.position;
			propertyHelpContainer.AnchorPosY(propertyHelpContainer.anchoredPosition.y - 27f);
			RectTransform component = propertyHelpImage.GetComponent<RectTransform>();
			RectTransform component2 = propertyHelpText.GetComponent<RectTransform>();
			propertyHelpText.GetComponent<ContentSizeFitter>().enabled = true;
			Canvas.ForceUpdateCanvases();
			float num = component2.rect.height + 20f;
			component.SizeDeltaY(num);
			if (propertyHelpURLButton.gameObject.activeSelf)
			{
				propertyHelpURLButtonText.text = buttonText;
				propertyHelpURLButton.onClick.RemoveAllListeners();
				propertyHelpURLButton.onClick.AddListener(delegate
				{
					Application.OpenURL(buttonURL);
				});
			}
			propertyHelpImage.DOScale(Vector3.one, duration).SetEase(Ease.OutCubic).SetUpdate(isIndependentUpdate: true)
				.OnComplete(delegate
				{
					animatingPropertyHelp = false;
				});
		}
		else
		{
			propertyHelpImage.DOScale(Vector3.zero, duration).SetEase(Ease.InCubic).SetUpdate(isIndependentUpdate: true)
				.OnComplete(delegate
				{
					animatingPropertyHelp = false;
				});
		}
	}

	private List<string> GetMissingExportParams()
	{
		List<string> list = new List<string>();
		string text = levelData.artist.Trim();
		if (text == "")
		{
			list.Add("editor.artist");
		}
		else
		{
			ApprovalLevel approvalLevel = ApprovalLevelForArtist(text);
			if (approvalLevel == ApprovalLevel.Declined)
			{
				list.Add("editor.artistDisclaimer.artistDeclinedRequirement");
			}
			else if (levelData.artistPermission == "" && approvalLevel == ApprovalLevel.Pending && levelData.specialArtistType == SpecialArtistType.None)
			{
				list.Add("editor.artistDisclaimer.artistNewRequirement");
			}
		}
		if (levelData.song == "")
		{
			list.Add("editor.song");
		}
		if (levelData.artist == "")
		{
			list.Add("editor.artist");
		}
		if (levelData.author == "")
		{
			list.Add("editor.author");
		}
		if (levelData.previewImage == "")
		{
			list.Add("editor.previewImage");
		}
		return list;
	}

	private List<string> GetMissingFiles()
	{
		string exportLevelTempDirectory = GetExportLevelTempDirectory();
		bool uploadable;
		uint[] requiredDLC;
		List<string> exportLevelFiles = GetExportLevelFiles(exportLevelTempDirectory, out uploadable, out requiredDLC);
		List<string> list = new List<string>();
		foreach (string item in exportLevelFiles)
		{
			if (!RDFile.Exists(item))
			{
				list.Add(Path.GetFileName(item));
			}
		}
		return list;
	}

	private ApprovalLevel ApprovalLevelForArtist(string artistName)
	{
		artistName = artistName.ToLower().Trim();
		artistName = new Regex("<.*?>").Replace(artistName, "");
		ArtistData[] artists = EditorWebServices.artists;
		foreach (ArtistData artistData in artists)
		{
			if (artistName == artistData.nameLowercase)
			{
				return artistData.approvalLevel;
			}
		}
		return ApprovalLevel.Pending;
	}

	public void oggConCallback(float percent)
	{
		oggConversionBar.value = percent / 100f;
		oggConversionBarText.text = string.Format("{0}  {1:F0}%", RDString.Get("editor.dialog.converting"), percent);
	}

	private IEnumerator ConvertSongToOggCo()
	{
		popupOggCancel.interactable = false;
		popupOggConvert.interactable = false;
		oggConversionBar.gameObject.SetActive(value: true);
		oggConversionBar.value = 0f;
		oggConversionBarText.text = RDString.Get("editor.dialog.converting") + "  0%";
		string directoryName = Path.GetDirectoryName(ADOBase.levelPath);
		string songName = songToConvert;
		string path = Path.Combine(directoryName, songName);
		string resultName = Path.GetFileNameWithoutExtension(songName) + ".ogg";
		string resultPath = Path.Combine(directoryName, resultName);
		if (File.Exists(resultPath))
		{
			Debug.Log(resultPath + " already exists! defaulting to that...");
			levelData.songFilename = resultName;
			UpdateSongAndLevelSettings();
			customLevel.ReloadSong();
			ShowNotificationPopup(RDString.Get("editor.notification.oggAlreadyExists"));
			ShowPopup(show: false);
			yield break;
		}
		yield return ADOBase.audioManager.FindOrLoadAudioClipExternal(path, mp3Streaming: false);
		string songKey = songName + "*external";
		if (ADOBase.audioManager.audioLib.ContainsKey(songKey))
		{
			AudioClip audioClip = ADOBase.audioManager.audioLib[songKey];
			yield return AudioclipToOggEncoder.EncodeToOgg(audioClip, 0f, audioClip.length, resultPath, oggConCallback);
			ADOBase.audioManager.audioLib.Remove(songKey);
			RDBaseDll.printem("song " + songName + " converted to ogg");
			oggConversionBarText.text = RDString.Get("editor.dialog.convert");
			levelData.songFilename = resultName;
			UpdateSongAndLevelSettings();
			customLevel.ReloadSong();
			ShowPopup(show: true, PopupType.ConversionSuccessful, skipAnim: true);
		}
		else
		{
			RDBaseDll.printem("song with key " + songKey + " not found in audiolib!");
			ShowPopup(show: true, PopupType.ConversionError, skipAnim: true);
		}
	}

	public void ShowPopupBlocker(Action onClickAction)
	{
		popupBlocker.gameObject.SetActive(value: true);
		Button component = popupBlocker.GetComponent<Button>();
		component.onClick.RemoveAllListeners();
		component.onClick.AddListener(delegate
		{
			popupBlocker.gameObject.SetActive(value: false);
			onClickAction();
		});
	}

	public void HidePopupBlocker()
	{
		if (popupBlocker.gameObject.activeSelf)
		{
			popupBlocker.gameObject.SetActive(value: false);
			popupBlocker.GetComponent<Button>().onClick.Invoke();
		}
	}

	public void ZoomOutUI()
	{
		float height = Persistence.GetEditorScale() + 50f;
		UpdateCanvasScalerResolution(height);
	}

	public void ZoomInUI()
	{
		float height = Persistence.GetEditorScale() - 50f;
		UpdateCanvasScalerResolution(height);
	}

	public void ToggleShortcutsLock()
	{
		LockPathEditing(!lockPathEditing);
	}

	public void LockPathEditing(bool locked)
	{
		lockPathEditing = locked;
		lockBackground.color = (lockPathEditing ? shortcutsLockColor : defaultLockColor);
		lockIcon.color = (lockPathEditing ? shortcutsLockIconColor : defaultLockIconColor);
		lockIcon.sprite = (lockPathEditing ? lockSpriteOn : lockSpriteOff);
		floorButtonContainer.SetActive(!lockPathEditing);
	}
}
