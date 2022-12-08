using DG.Tweening;
using GDMiniJSON;
using SA.GoogleDoc;
using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : ADOBase
{
	public enum ButtonType
	{
		Continue,
		Previous,
		Next,
		LevelEditor,
		SteamWorkshop,
		Refresh,
		Restart,
		Practice,
		Settings,
		Calibration,
		Quit,
		LoadLevel
	}

	public enum Feature
	{
		Pause,
		NoPrevious,
		NoNext,
		NoPreviousOrNext,
		Practice
	}

	private enum PauseButtonsLayout
	{
		ShowPreviousAndNextButton,
		HidePreviousButton,
		HideNextButton,
		HidePreviousAndNextButton
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Predicate<GeneralPauseButton> _003C_003E9__105_0;

		public static Predicate<GeneralPauseButton> _003C_003E9__105_1;

		internal bool _003CUpdateButtonsContainer_003Eb__105_0(GeneralPauseButton a)
		{
			return a.gameObject.activeSelf;
		}

		internal bool _003CUpdateButtonsContainer_003Eb__105_1(GeneralPauseButton a)
		{
			return a.gameObject.activeSelf;
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003C_003Ec__DisplayClass114_0
	{
		public PauseMenu _003C_003E4__this;

		public int direction;
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003C_003Ec__DisplayClass114_1
	{
		public bool selectingLeftBar;

		public bool selectingRightBar;
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass121_0
	{
		public ButtonType buttonType;

		public PauseMenu _003C_003E4__this;

		public scrController controller;

		internal void _003CChoose_003Eb__0()
		{
			switch (buttonType)
			{
			case ButtonType.Continue:
				_003C_003E4__this.Unpause();
				break;
			case ButtonType.Settings:
				_003C_003E4__this.enabled = true;
				_003C_003E4__this.ShowSettingsMenu();
				break;
			case ButtonType.Calibration:
				ADOBase.GoToCalibration();
				break;
			case ButtonType.LoadLevel:
				_003C_003E4__this.StartCoroutine(_003C_003E4__this.OpenLevelCo());
				_003C_003E4__this.enabled = true;
				break;
			case ButtonType.LevelEditor:
				scnEditor.instance.SwitchToEditMode(clsToEditor: true);
				DiscordController.instance?.UpdatePresence();
				Persistence.UpdateLastOpenedLevel(ADOBase.levelPath);
				_003C_003E4__this.Hide();
				break;
			case ButtonType.Quit:
				if (ADOBase.cls != null)
				{
					GCS.customLevelPaths = null;
					_003C_003E4__this.wrldGame.QuitToMainMenu();
				}
				else if (controller.gameworld || (!controller.gameworld && (((bool)controller.currFloor && controller.currFloor.freeroamGenerated) || controller.isPuzzleRoom)))
				{
					controller.SaveProgress(save: true);
					_003C_003E4__this.wrldGame.QuitToMainMenu();
				}
				else if (ADOBase.sceneName.StartsWith("scnTaro"))
				{
					_003C_003E4__this.wrldGame.QuitToMainMenu();
				}
				else
				{
					Application.Quit();
				}
				break;
			case ButtonType.SteamWorkshop:
				SteamWorkshop.OpenWorkshop();
				_003C_003E4__this.enabled = true;
				break;
			case ButtonType.Refresh:
				_003C_003E4__this.Unpause();
				scnCLS.instance.Refresh();
				break;
			case ButtonType.Restart:
				controller.Restart(fromBeginning: true);
				break;
			case ButtonType.Practice:
				if (GCS.standaloneLevelMode)
				{
					_003C_003E4__this.Unpause();
				}
				controller.SetPracticeMode(!GCS.practiceMode);
				break;
			default:
				_003C_003E4__this.enabled = true;
				_003C_003E4__this.printe("option not found");
				break;
			}
			_003C_003E4__this.delayedAction = null;
		}
	}

	[CompilerGenerated]
	private sealed class _003COpenLevelCo_003Ed__122 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		private string[] _003ClevelPaths_003E5__2;

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
		public _003COpenLevelCo_003Ed__122(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			switch (_003C_003E1__state)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003ClevelPaths_003E5__2 = StandaloneFileBrowser.OpenFilePanel(RDString.Get("editor.dialog.openFile"), Persistence.GetLastUsedFolder(), "adofai", multiselect: false);
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				if (_003ClevelPaths_003E5__2.Length == 0 || string.IsNullOrEmpty(_003ClevelPaths_003E5__2[0]))
				{
					UnityEngine.Debug.Log("Level was not selected");
					return false;
				}
				ADOBase.controller.LoadCustomLevel(_003ClevelPaths_003E5__2[0]);
				return false;
			}
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

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass123_0
	{
		public PauseMenu _003C_003E4__this;

		public string sceneName;

		internal void _003CUpdateLevelDescriptionAndReload_003Eb__0()
		{
			_003C_003E4__this.enabled = false;
			if (GCS.standaloneLevelMode)
			{
				GCS.customLevelIndex = _003C_003E4__this.currentCustomPauseLevel;
				GCS.checkpointNum = 0;
			}
			else
			{
				GCS.sceneToLoad = sceneName;
			}
			_003C_003E4__this.wrldGame.StartLoadingScene(WipeDirection.StartsFromRight);
		}
	}

	[Header("General UI")]
	public RawImage background;

	public GameObject pauseMenuContainer;

	public GameObject pauseButton;

	public Image overlay;

	public SettingsMenu settingsMenu;

	public PracticeTimeline practiceTimeline;

	public PauseMedals pauseMedals;

	[Header("Sprites")]
	public Sprite selectedButtonSprite;

	public Sprite unselectedButtonSprite;

	[Header("Button groups")]
	public Dictionary<ButtonType, PauseButton> buttonDictionary = new Dictionary<ButtonType, PauseButton>();

	public RectTransform buttonsContainer;

	public List<GeneralPauseButton> currentButtons;

	[Header("Individual buttons")]
	public PauseButton previousButton;

	public PauseButton nextButton;

	public PauseButton openInEditorButton;

	public PauseButton steamWorkshopButton;

	public PauseButton restartButton;

	public PauseButton practiceButton;

	public PauseButton settingsButton;

	public PauseButton quitButton;

	public Text subtitle;

	[Header("Social network buttons")]
	public Button discordButton;

	public Button youtubeButton;

	public Button qqButton;

	[Header("Nintendo Switch Indicators")]
	public GameObject back;

	public GameObject select;

	public GameObject backSettings;

	[Header("Variables")]
	public float animationDistance;

	public float settingsButtonAnimationDistance;

	public float arrowButtonAnimationDistance;

	public float volumeTimeToFastforward;

	public float volumeChangeTime;

	public float animationTime = 2f;

	public Ease animationEase = Ease.InOutQuad;

	[Header("Settings")]
	public float settings_mobile_y0;

	public float settings_mobile_yDelta;

	public float settings_mobile_height;

	public float settings_desktop_y0;

	public float settings_desktop_yDelta;

	public float settings_desktop_height;

	public Text inputOffsetLabel;

	[Header("Colors")]
	public Color arrowButtonBackgroundColor;

	public Color selectedIconColor;

	public Color unselectedIconColor;

	public Color selectedLabelColor;

	public Color unselectedLabelColor;

	public Color selectedFillColor;

	public Color unselectedFillColor;

	public Color unselectedBorderColor;

	public Color otherTintColor;

	public float selectedButtonTintSpeed;

	[Header("Other")]
	public int lastInputSelected;

	public bool requireRestart;

	private bool isOnSettingsMenu;

	private int lastFrameVolume;

	private int selectedIndex;

	private int selectedVerticalIndex;

	private PauseButtonsLayout buttonsLayout;

	private GeneralPauseButton[] pauseButtons;

	private string currentPauseLevel;

	private int currentCustomPauseLevel;

	private List<int> framerates = new List<int>();

	private bool lastGCEnabled;

	private bool anyButtonPressed;

	private float timelineInputTimer;

	private float lastScreenWidth = Screen.width;

	private const float pauseButtonOffset = 58f;

	private const float pauseButtonWidth = 52f;

	private const float buttonSideMargin = 0.1f;

	private const float leastTotalPauseButtons = 8f;

	private const float leastTotalContainerWidth = 822f;

	private const float blinkAnimDuration = 0.5f;

	private const float timelineFirstInputDelay = 0.4f;

	private const float timelineInputUpdateTime = 0.02f;

	private static bool addedSceneChangeEvent;

	private List<PauseButton> allPauseButtons = new List<PauseButton>();

	private List<PauseButton> menuPauseButtons = new List<PauseButton>();

	private List<PauseButton> clsPauseButtons = new List<PauseButton>();

	private List<PauseButton> gamePauseButtons = new List<PauseButton>();

	private List<PauseButton> gamePauseButtonsNoPrevious = new List<PauseButton>();

	private List<PauseButton> gamePauseButtonsNoNext = new List<PauseButton>();

	private List<PauseButton> gamePauseButtonsNoPreviousOrNext = new List<PauseButton>();

	private List<PauseButton> gamePauseButtonsPractice = new List<PauseButton>();

	private const int areEqual = 0;

	private const int isHigher = 1;

	private const int isLower = -1;

	private const int NotYetKnown = -100;

	private Tween delayedAction;

	private Tween delayedActionLevelSkip;

	private int currentVolume
	{
		get
		{
			return scrController.volume;
		}
		set
		{
			scrController.volume = value;
		}
	}

	private scrController wrldGame => scrController.instance;

	private void Awake()
	{
		GenerateButtons();
		base.gameObject.SetActive(value: false);
		bool flag = RDString.language.ToString().Contains("Chinese");
		bool active = !ADOBase.controller.gameworld && !flag;
		discordButton.gameObject.SetActive(active);
		youtubeButton.gameObject.SetActive(active);
		discordButton.onClick.AddListener(OpenDiscord);
		youtubeButton.onClick.AddListener(OpenYoutube);
		qqButton.gameObject.SetActive(!ADOBase.controller.gameworld && flag);
		qqButton.onClick.AddListener(OpenQQ);
		if (!addedSceneChangeEvent)
		{
			SceneManager.activeSceneChanged += OnSceneChanged;
			addedSceneChangeEvent = true;
		}
	}

	private void GenerateButtons()
	{
		buttonDictionary.Clear();
		allPauseButtons.Clear();
		menuPauseButtons.Clear();
		clsPauseButtons.Clear();
		gamePauseButtons.Clear();
		gamePauseButtonsNoPrevious.Clear();
		gamePauseButtonsNoNext.Clear();
		gamePauseButtonsNoPreviousOrNext.Clear();
		gamePauseButtonsPractice.Clear();
		Dictionary<string, object> obj = Json.Deserialize(Resources.Load<TextAsset>("PauseMenuButtons").text) as Dictionary<string, object>;
		Dictionary<string, object> dictionary = obj["Buttons"] as Dictionary<string, object>;
		List<object> list = obj["CLSPause"] as List<object>;
		int num = 0;
		foreach (KeyValuePair<string, object> item in dictionary)
		{
			ButtonType buttonType = item.Key.ToEnum(ButtonType.Continue);
			Dictionary<string, object> dictionary2 = item.Value as Dictionary<string, object>;
			List<object> list2 = dictionary2["scale"] as List<object>;
			string rdString = dictionary2["rdString"] as string;
			Sprite sprite = Resources.Load<Sprite>(dictionary2["icon"] as string);
			GameObject gameObject = UnityEngine.Object.Instantiate(pauseButton, Vector3.zero, Quaternion.identity);
			PauseButton component = gameObject.GetComponent<PauseButton>();
			gameObject.transform.SetParent(buttonsContainer);
			component.icon.sprite = sprite;
			component.buttonType = buttonType;
			component.rdString = rdString;
			gameObject.name = item.Key;
			component.transform.ScaleXY(1f, 1f);
			component.icon.transform.ScaleX(Convert.ToSingle(list2[0]));
			component.icon.transform.ScaleY(Convert.ToSingle(list2[1]));
			gameObject.GetComponent<RectTransform>().AnchorPosX(58 * num);
			gameObject.GetComponent<RectTransform>().AnchorPosY(0f);
			buttonDictionary.Add(buttonType, component);
			allPauseButtons.Add(component);
			switch (buttonType)
			{
			case ButtonType.Previous:
				previousButton = component;
				break;
			case ButtonType.Next:
				nextButton = component;
				break;
			case ButtonType.LevelEditor:
				openInEditorButton = component;
				break;
			case ButtonType.Restart:
				restartButton = component;
				break;
			case ButtonType.SteamWorkshop:
				steamWorkshopButton = component;
				break;
			case ButtonType.Practice:
				practiceButton = component;
				break;
			case ButtonType.Settings:
				settingsButton = component;
				break;
			case ButtonType.Quit:
				quitButton = component;
				break;
			case ButtonType.LoadLevel:
				if (ADOBase.isUnityEditor)
				{
					menuPauseButtons.Add(component);
				}
				break;
			}
			if (dictionary2.ContainsKey("platforms"))
			{
				foreach (object item2 in dictionary2["platforms"] as List<object>)
				{
					if ((item2 as string).ToEnum(Platform.None) == ADOBase.platform)
					{
						menuPauseButtons.Add(component);
					}
				}
			}
			if (dictionary2.ContainsKey("features"))
			{
				using (List<object>.Enumerator enumerator2 = (dictionary2["features"] as List<object>).GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						switch ((enumerator2.Current as string).ToEnum(Feature.Pause))
						{
						case Feature.Pause:
							gamePauseButtons.Add(component);
							break;
						case Feature.NoPrevious:
							gamePauseButtonsNoPrevious.Add(component);
							break;
						case Feature.NoNext:
							gamePauseButtonsNoNext.Add(component);
							break;
						case Feature.NoPreviousOrNext:
							gamePauseButtonsNoPreviousOrNext.Add(component);
							break;
						case Feature.Practice:
							gamePauseButtonsPractice.Add(component);
							break;
						}
					}
				}
			}
			num++;
		}
		foreach (object item3 in list)
		{
			ButtonType buttonType2 = (item3 as string).ToEnum(ButtonType.Continue);
			if (buttonType2 == ButtonType.SteamWorkshop)
			{
				if (ADOBase.isSteamworks && !ADOBase.isSwitch)
				{
					clsPauseButtons.Add(buttonDictionary[buttonType2]);
				}
			}
			else
			{
				clsPauseButtons.Add(buttonDictionary[buttonType2]);
			}
		}
	}

	private void OnEnable()
	{
		AudioManager.pauseGameSounds = true;
		if (!Cursor.visible)
		{
			Cursor.visible = true;
		}
	}

	private void OnDisable()
	{
		UpdateCursorVisibility();
	}

	private static void OnSceneChanged(Scene _, Scene __)
	{
		UpdateCursorVisibility();
	}

	private static void UpdateCursorVisibility()
	{
		if (Persistence.GetHideCursorWhilePlaying())
		{
			scrController instance = scrController.instance;
			if ((bool)instance)
			{
				Cursor.visible = (!instance.gameworld || ADOBase.isEditingLevel || (bool)scnCLS.instance);
			}
		}
	}

	private int CompareSpeedRunValues(float a, float b)
	{
		if (Mathf.Abs(a - b) < 0.01f)
		{
			return 0;
		}
		if (!(a > b))
		{
			return -1;
		}
		return 1;
	}

	private void RefreshLayout()
	{
		foreach (PauseButton allPauseButton in allPauseButtons)
		{
			allPauseButton.label.text = RDString.Get(allPauseButton.rdString);
		}
		if (GCS.speedTrialMode)
		{
			previousButton.label.text = RDString.Get("pauseMenu.slower");
			nextButton.label.text = RDString.Get("pauseMenu.faster");
		}
		if (GCS.practiceMode)
		{
			practiceButton.label.text = RDString.Get("pauseMenu.endPractice");
		}
		if (GCS.standaloneLevelMode)
		{
			currentCustomPauseLevel = GCS.customLevelIndex;
		}
		else
		{
			currentPauseLevel = ADOBase.currentLevel;
		}
		if ((ADOBase.controller.gameworld && !ADOBase.isEditingLevel) || (!ADOBase.controller.gameworld && (((bool)ADOBase.controller.currFloor && ADOBase.controller.currFloor.freeroamGenerated) || ADOBase.controller.isPuzzleRoom)))
		{
			PauseButtonsLayout pauseButtonsLayout = GetPauseButtonsLayout();
			printe("layout selected: " + pauseButtonsLayout.ToString());
			buttonsLayout = pauseButtonsLayout;
			if (GCS.practiceMode)
			{
				GeneralPauseButton[] array = pauseButtons = gamePauseButtonsPractice.ToArray();
			}
			else
			{
				switch (pauseButtonsLayout)
				{
				case PauseButtonsLayout.HidePreviousAndNextButton:
				{
					GeneralPauseButton[] array = pauseButtons = gamePauseButtonsNoPreviousOrNext.ToArray();
					break;
				}
				case PauseButtonsLayout.HidePreviousButton:
				{
					GeneralPauseButton[] array = pauseButtons = gamePauseButtonsNoPrevious.ToArray();
					break;
				}
				case PauseButtonsLayout.HideNextButton:
				{
					GeneralPauseButton[] array = pauseButtons = gamePauseButtonsNoNext.ToArray();
					break;
				}
				default:
				{
					GeneralPauseButton[] array = pauseButtons = gamePauseButtons.ToArray();
					break;
				}
				}
			}
			List<GeneralPauseButton> list = pauseButtons.ToList();
			if (!GCS.practiceMode)
			{
				if (GCS.standaloneLevelMode)
				{
					list.Insert(list.Count - 1, openInEditorButton);
				}
				if (base.practiceAvailable)
				{
					list.Insert(2, practiceButton);
				}
			}
			pauseButtons = list.ToArray();
		}
		else
		{
			List<PauseButton> list2 = (ADOBase.cls != null) ? clsPauseButtons : menuPauseButtons;
			GeneralPauseButton[] array = pauseButtons = list2.ToArray();
		}
	}

	private PauseButtonsLayout GetPauseButtonsLayout()
	{
		if (currentPauseLevel == null && !GCS.standaloneLevelMode)
		{
			return PauseButtonsLayout.HidePreviousAndNextButton;
		}
		if (currentPauseLevel == "scnMinesweeper")
		{
			return PauseButtonsLayout.HidePreviousAndNextButton;
		}
		bool flag = true;
		bool flag2 = true;
		if (GCS.standaloneLevelMode)
		{
			if (GCS.speedTrialMode)
			{
				flag2 = (GCS.nextSpeedRun <= 0.5f);
				flag = false;
			}
			else
			{
				flag2 = (currentCustomPauseLevel <= 0);
				flag = (currentCustomPauseLevel >= GCS.customLevelPaths.Length - 1);
			}
		}
		else if (GCS.speedTrialMode)
		{
			if (CompareSpeedRunValues(GCS.nextSpeedRun, 0.5f) != 0)
			{
				flag2 = false;
			}
			flag = false;
		}
		else
		{
			if (scrController.currentWorldString == null || !ADOBase.worldData.ContainsKey(scrController.currentWorldString))
			{
				scrController.currentWorldString = "Template";
			}
			if (ADOBase.sceneName.StartsWith("TP"))
			{
				scrController.currentWorldString = "TP";
			}
			bool num = currentPauseLevel.EndsWith("-1");
			bool flag3 = ADOBase.worldData[scrController.currentWorldString].levelCount == 1;
			flag2 = (num | flag3);
			flag = currentPauseLevel.Contains("-X");
		}
		if (flag && flag2)
		{
			return PauseButtonsLayout.HidePreviousAndNextButton;
		}
		if (flag)
		{
			return PauseButtonsLayout.HideNextButton;
		}
		if (flag2)
		{
			return PauseButtonsLayout.HidePreviousButton;
		}
		return PauseButtonsLayout.ShowPreviousAndNextButton;
	}

	public void Show(int selectedItem = 0)
	{
		RefreshLayout();
		GCS.nextSpeedRun = GCS.currentSpeedTrial;
		base.enabled = true;
		base.gameObject.SetActive(value: true);
		currentButtons = new List<GeneralPauseButton>(pauseButtons);
		float x = (float)(pauseButtons.Length - 1) * 58f + 52f;
		buttonsContainer.sizeDelta = buttonsContainer.sizeDelta.WithX(x);
		foreach (PauseButton allPauseButton in allPauseButtons)
		{
			allPauseButton.gameObject.SetActive(value: false);
		}
		int num = 0;
		GeneralPauseButton[] array = pauseButtons;
		foreach (GeneralPauseButton obj in array)
		{
			obj.gameObject.SetActive(value: true);
			RectTransform component = obj.GetComponent<RectTransform>();
			component.anchoredPosition = component.anchoredPosition.WithX((float)num * 58f);
			num++;
		}
		isOnSettingsMenu = false;
		pauseMenuContainer.SetActive(value: true);
		settingsMenu.gameObject.SetActive(value: false);
		num = 0;
		foreach (GeneralPauseButton currentButton in currentButtons)
		{
			currentButton.SetFocus(value: false);
			currentButton.index = num;
			num++;
		}
		selectedIndex = ((lastInputSelected != -1) ? lastInputSelected : selectedItem);
		lastInputSelected = -1;
		selectedIndex = selectedItem;
		currentButtons[selectedIndex].SetFocus(value: true);
		UpdateButtonsContainer();
		Persistence.Save();
		practiceTimeline.Init();
		pauseMedals.Init();
		lastGCEnabled = RDUtils.GetGarbageCollectionEnabled();
		RDUtils.SetGarbageCollectionEnabled(enabled: true);
	}

	public void Hide()
	{
		if (delayedAction != null)
		{
			delayedAction.Kill();
		}
		base.gameObject.SetActive(value: false);
		Persistence.Save();
		RDUtils.SetGarbageCollectionEnabled(lastGCEnabled);
	}

	private void UpdateButtonsContainer()
	{
		buttonsContainer.ScaleXY(1f);
		float x = currentButtons.Find((GeneralPauseButton a) => a.gameObject.activeSelf).rectangleRT.position.x;
		float num = 52f + currentButtons.FindLast((GeneralPauseButton a) => a.gameObject.activeSelf).rectangleRT.position.x;
		float num2 = lastScreenWidth = Screen.width;
		float num3 = Math.Max(num - x, 822f);
		float num4 = num2 * 0.8f;
		float xy = Mathf.Min(1f, num4 / num3);
		buttonsContainer.ScaleXY(xy);
	}

	private string GetNativeLanguageName(SystemLanguage language)
	{
		LangCode lang = RDUtils.ParseEnum(language.ToString(), LangCode.English);
		return Localization.GetLocalizedString("pauseMenu.settings.myLanguage", Localization.DefaultSection, lang);
	}

	public void RemoveRestartAndQuitButton()
	{
		buttonsContainer.sizeDelta = buttonsContainer.sizeDelta.WithX(131f);
		nextButton.gameObject.SetActive(value: false);
		previousButton.gameObject.SetActive(value: false);
		quitButton.gameObject.SetActive(value: false);
		RectTransform component = settingsButton.GetComponent<RectTransform>();
		component.anchoredPosition = component.anchoredPosition.WithX(70f);
	}

	private void OnGUI()
	{
		if (RDC.debug)
		{
			string text = "";
			text = text + "currentResolution Width: " + Screen.width.ToString() + " height: " + Screen.height.ToString() + " @ " + Screen.currentResolution.refreshRate.ToString() + "hz\n";
			Resolution[] resolutions = Screen.resolutions;
			foreach (Resolution r in resolutions)
			{
				text = text + ResolutionToString(r) + "\n";
			}
			text = text + "fullscreen: " + Screen.fullScreen.ToString() + "\n";
			text = text + "fullscreenMode: " + Screen.fullScreenMode.ToString() + "\n";
			GUI.Label(new Rect(0f, 20f, 300f, 200f), text);
		}
	}

	private string ResolutionToString(Resolution r)
	{
		return r.width + " x " + r.height + " @ " + r.refreshRate + "hz";
	}

	public void SettingsMenu_BackButton()
	{
		if (!isOnSettingsMenu)
		{
			return;
		}
		if (ADOBase.isEditingLevel)
		{
			Hide();
			return;
		}
		if (settingsMenu.isSelectingTab)
		{
			settingsMenu.StopBrowsingTab();
		}
		HideSettingsMenu();
		PlaySfx(SfxSound.MenuSquelch, 1.5f);
	}

	private void Update()
	{
		if (!wrldGame.paused)
		{
			return;
		}
		if (RDEditorUtils.CheckForKeyCombo(control: true, shift: true, KeyCode.L))
		{
			RDEditorUtils.RevealInExplorer(RDEditorUtils.LogPath());
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.C) && (!GCS.d_judges || RDC.debug) && (!GCS.d_boothDisablePossibleMessUpButtons || RDEditorUtils.CheckForKeyCombo(control: false, shift: true, KeyCode.C)))
		{
			ADOBase.GoToCalibration();
		}
		anyButtonPressed = false;
		if (isOnSettingsMenu)
		{
			if (RDInput.cancelPress)
			{
				if (!settingsMenu.isSelectingTab)
				{
					settingsMenu.StopBrowsingTab();
				}
				else if (ADOBase.isEditingLevel)
				{
					Hide();
				}
				else
				{
					HideSettingsMenu();
					PlaySfx(SfxSound.MenuSquelch, 1.5f);
				}
			}
		}
		else if (RDInput.rightIsPressed)
		{
			SelectHorizontal(1, RDInput.rightPress);
		}
		else if (RDInput.leftIsPressed)
		{
			SelectHorizontal(-1, RDInput.leftPress);
		}
		else if (RDInput.upPress && GCS.practiceMode)
		{
			SelectVertical(-1);
		}
		else if (RDInput.downPress && GCS.practiceMode)
		{
			SelectVertical(1);
		}
		else if (RDInput.quitPress)
		{
			quitButton.Select();
			anyButtonPressed = true;
		}
		else if (RDInput.restartPress && restartButton.isActiveAndEnabled)
		{
			restartButton.Select();
			anyButtonPressed = true;
		}
		else if (RDInput.cancelPress)
		{
			Unpause();
			anyButtonPressed = true;
		}
		GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
		InputField component;
		bool flag = currentSelectedGameObject != null && currentSelectedGameObject.TryGetComponent(out component);
		if (RDInput.mainPress && !anyButtonPressed && !flag)
		{
			Select(selectedIndex);
			Choose();
		}
	}

	private void LateUpdate()
	{
		if (lastFrameVolume != currentVolume)
		{
			scrController.volume = currentVolume;
			Persistence.SetGlobalVolume(currentVolume);
		}
		lastFrameVolume = currentVolume;
		if (lastScreenWidth != (float)Screen.width)
		{
			UpdateButtonsContainer();
		}
	}

	public void SelectHorizontal(int direction, bool firstPress)
	{
		_003C_003Ec__DisplayClass114_0 _003C_003Ec__DisplayClass114_ = default(_003C_003Ec__DisplayClass114_0);
		_003C_003Ec__DisplayClass114_._003C_003E4__this = this;
		_003C_003Ec__DisplayClass114_.direction = direction;
		anyButtonPressed = true;
		if (selectedVerticalIndex == 0)
		{
			if (firstPress)
			{
				int index = (_003C_003Ec__DisplayClass114_.direction == 1) ? ((selectedIndex + 1) % currentButtons.Count) : ((selectedIndex + currentButtons.Count - 1) % currentButtons.Count);
				Select(index);
			}
			return;
		}
		_003C_003Ec__DisplayClass114_1 _003C_003Ec__DisplayClass114_2 = default(_003C_003Ec__DisplayClass114_1);
		_003C_003Ec__DisplayClass114_2.selectingLeftBar = (selectedVerticalIndex == 1);
		_003C_003Ec__DisplayClass114_2.selectingRightBar = (selectedVerticalIndex == 2);
		if (_003C_003Ec__DisplayClass114_2.selectingLeftBar | _003C_003Ec__DisplayClass114_2.selectingRightBar)
		{
			if (firstPress)
			{
				_003CSelectHorizontal_003Eg__UpdateTimelineBar_007C114_0(ref _003C_003Ec__DisplayClass114_, ref _003C_003Ec__DisplayClass114_2);
				timelineInputTimer = Time.unscaledTime + 0.4f;
			}
			else if (Time.unscaledTime > timelineInputTimer)
			{
				_003CSelectHorizontal_003Eg__UpdateTimelineBar_007C114_0(ref _003C_003Ec__DisplayClass114_, ref _003C_003Ec__DisplayClass114_2);
				timelineInputTimer = Time.unscaledTime + 0.02f;
			}
		}
		else if (firstPress)
		{
			practiceTimeline.ChangeSpeed(_003C_003Ec__DisplayClass114_.direction == 1);
		}
	}

	public void SelectVertical(int direction)
	{
		anyButtonPressed = true;
		selectedVerticalIndex += direction;
		if (selectedVerticalIndex > 3)
		{
			selectedVerticalIndex = 0;
		}
		else if (selectedVerticalIndex < 0)
		{
			selectedVerticalIndex = 3;
		}
		EventSystem.current.SetSelectedGameObject(null);
		currentButtons[selectedIndex].SetFocus(value: false);
		if (selectedVerticalIndex == 0)
		{
			currentButtons[selectedIndex].SetFocus(value: true);
		}
		practiceTimeline.Select(selectedVerticalIndex, direction);
		PlaySfx(SfxSound.MenuSquelch, 1.5f);
	}

	public void SelectVerticalFixed(int index, int direction)
	{
		anyButtonPressed = true;
		selectedVerticalIndex = index;
		EventSystem.current.SetSelectedGameObject(null);
		currentButtons[selectedIndex].SetFocus(value: false);
		if (selectedVerticalIndex == 0)
		{
			currentButtons[selectedIndex].SetFocus(value: true);
		}
		practiceTimeline.Select(selectedVerticalIndex, direction);
		PlaySfx(SfxSound.MenuSquelch, 1.5f);
	}

	public void Select(int index)
	{
		EventSystem.current.SetSelectedGameObject(null);
		if (selectedIndex != index)
		{
			currentButtons[selectedIndex].SetFocus(value: false);
			selectedIndex = index;
			currentButtons[index].SetFocus(value: true);
			PlaySfx(SfxSound.MenuSquelch, 1.5f);
		}
	}

	private void ChangeLevel(bool next)
	{
		PauseButtonsLayout pauseButtonsLayout = (!next) ? PauseButtonsLayout.HidePreviousButton : PauseButtonsLayout.HideNextButton;
		if (GetPauseButtonsLayout() != pauseButtonsLayout)
		{
			if (GCS.speedTrialMode)
			{
				GCS.nextSpeedRun += (next ? 0.1f : (-0.1f));
			}
			else if (GCS.standaloneLevelMode)
			{
				currentCustomPauseLevel += (next ? 1 : (-1));
			}
			else
			{
				currentPauseLevel = (next ? ADOBase.GetNextLevelName(currentPauseLevel) : ADOBase.GetPreviousLevelName(currentPauseLevel));
			}
			UpdateLevelDescriptionAndReload();
		}
	}

	public void Choose()
	{
		if (isOnSettingsMenu)
		{
			return;
		}
		PauseButton pauseButton = currentButtons[selectedIndex] as PauseButton;
		ButtonType buttonType = pauseButton.buttonType;
		pauseButton.ShowAsSelected();
		if (delayedAction == null)
		{
			switch (buttonType)
			{
			case ButtonType.Previous:
				ChangeLevel(next: false);
				break;
			case ButtonType.Next:
				ChangeLevel(next: true);
				break;
			default:
			{
				base.enabled = false;
				scrController controller = scrController.instance;
				delayedAction = DOVirtual.DelayedCall(0.15f, delegate
				{
					switch (buttonType)
					{
					case ButtonType.Continue:
						Unpause();
						break;
					case ButtonType.Settings:
						base.enabled = true;
						ShowSettingsMenu();
						break;
					case ButtonType.Calibration:
						ADOBase.GoToCalibration();
						break;
					case ButtonType.LoadLevel:
						StartCoroutine(OpenLevelCo());
						base.enabled = true;
						break;
					case ButtonType.LevelEditor:
						scnEditor.instance.SwitchToEditMode(clsToEditor: true);
						DiscordController.instance?.UpdatePresence();
						Persistence.UpdateLastOpenedLevel(ADOBase.levelPath);
						Hide();
						break;
					case ButtonType.Quit:
						if (ADOBase.cls != null)
						{
							GCS.customLevelPaths = null;
							wrldGame.QuitToMainMenu();
						}
						else if (controller.gameworld || (!controller.gameworld && (((bool)controller.currFloor && controller.currFloor.freeroamGenerated) || controller.isPuzzleRoom)))
						{
							controller.SaveProgress(save: true);
							wrldGame.QuitToMainMenu();
						}
						else if (ADOBase.sceneName.StartsWith("scnTaro"))
						{
							wrldGame.QuitToMainMenu();
						}
						else
						{
							Application.Quit();
						}
						break;
					case ButtonType.SteamWorkshop:
						SteamWorkshop.OpenWorkshop();
						base.enabled = true;
						break;
					case ButtonType.Refresh:
						Unpause();
						scnCLS.instance.Refresh();
						break;
					case ButtonType.Restart:
						controller.Restart(fromBeginning: true);
						break;
					case ButtonType.Practice:
						if (GCS.standaloneLevelMode)
						{
							Unpause();
						}
						controller.SetPracticeMode(!GCS.practiceMode);
						break;
					default:
						base.enabled = true;
						printe("option not found");
						break;
					}
					delayedAction = null;
				});
				break;
			}
			}
			PlaySfx(SfxSound.MenuSquelch, 1.5f);
		}
		else
		{
			printe("delayedAction is not null");
		}
	}

	private IEnumerator OpenLevelCo()
	{
		string[] levelPaths = StandaloneFileBrowser.OpenFilePanel(RDString.Get("editor.dialog.openFile"), Persistence.GetLastUsedFolder(), "adofai", multiselect: false);
		yield return null;
		if (levelPaths.Length == 0 || string.IsNullOrEmpty(levelPaths[0]))
		{
			UnityEngine.Debug.Log("Level was not selected");
		}
		else
		{
			ADOBase.controller.LoadCustomLevel(levelPaths[0]);
		}
	}

	private void UpdateLevelDescriptionAndReload()
	{
		string sceneName = "";
		string text = "";
		pauseMedals.gameObject.SetActive(value: false);
		if (GCS.standaloneLevelMode)
		{
			text = ADOBase.GetCustomLevelName(GCS.customLevelPaths[currentCustomPauseLevel]);
		}
		else
		{
			sceneName = currentPauseLevel;
			text = ADOBase.GetLocalizedLevelName(sceneName);
			if (GCS.speedTrialMode && sceneName.Contains("EX") && sceneName.StartsWith("T"))
			{
				text = ADOBase.GetLocalizedLevelName(sceneName.Replace("EX", "")).Replace("-X", "-EX");
			}
		}
		if (GCS.speedTrialMode)
		{
			string str = RDString.Get("levelSelect.multiplier", new Dictionary<string, object>
			{
				{
					"multiplier",
					GCS.nextSpeedRun.ToString("0.0")
				}
			});
			text = text + " (" + str + ")";
		}
		subtitle.text = text;
		subtitle.enabled = true;
		if (delayedActionLevelSkip != null)
		{
			delayedActionLevelSkip.Kill();
			delayedActionLevelSkip = null;
		}
		delayedActionLevelSkip = DOVirtual.DelayedCall(0.3f, delegate
		{
			base.enabled = false;
			if (GCS.standaloneLevelMode)
			{
				GCS.customLevelIndex = currentCustomPauseLevel;
				GCS.checkpointNum = 0;
			}
			else
			{
				GCS.sceneToLoad = sceneName;
			}
			wrldGame.StartLoadingScene(WipeDirection.StartsFromRight);
		});
	}

	private void Unpause()
	{
		practiceTimeline.SetPositions();
		if (requireRestart)
		{
			ADOBase.controller.Restart();
		}
		else
		{
			wrldGame.TogglePauseGame();
		}
	}

	public void ShowSettingsMenu()
	{
		settingsMenu.Show();
		pauseMenuContainer.SetActive(value: false);
		isOnSettingsMenu = true;
	}

	public void HideSettingsMenu()
	{
		if (settingsMenu.settingsThatRequireRestart > 0)
		{
			wrldGame.Restart();
		}
		int num = Array.IndexOf(pauseButtons, settingsButton);
		Show((!scrController.isGameWorld) ? 1 : num);
		isOnSettingsMenu = false;
		settingsMenu.gameObject.SetActive(value: false);
		pauseMenuContainer.transform.localScale = Vector3.one;
		pauseMenuContainer.SetActive(value: true);
		Persistence.Save();
	}

	public void GoToCalibrationScreen()
	{
		ADOBase.GoToCalibration();
	}

	public void OpenDiscord()
	{
		Application.OpenURL("https://7thbe.at/discord");
	}

	public void OpenYoutube()
	{
		Application.OpenURL("https://www.youtube.com/c/7thBeatGames?sub_confirmation=1");
	}

	public void OpenQQ()
	{
		Application.OpenURL("https://jq.qq.com/?_wv=1027&k=5xO9aF4");
	}

	public void PlaySfx(SfxSound sound, float volume = 1f)
	{
		scrSfx.instance.PlaySfx(sound, volume);
	}

	[CompilerGenerated]
	private void _003CSelectHorizontal_003Eg__UpdateTimelineBar_007C114_0(ref _003C_003Ec__DisplayClass114_0 P_0, ref _003C_003Ec__DisplayClass114_1 P_1)
	{
		if (P_1.selectingLeftBar)
		{
			practiceTimeline.practiceStart += P_0.direction;
		}
		else
		{
			practiceTimeline.practiceEnd += P_0.direction;
		}
		practiceTimeline.UpdatePositions(P_1.selectingRightBar);
	}
}
