using ADOFAI;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsPanelsCLS : ADOBase
{
	public enum OptionName
	{
		Find,
		Difficulty,
		LastPlayed,
		Song,
		Artist,
		Author,
		SpeedTrial,
		NoFail,
		Delete
	}

	[Serializable]
	public class Option
	{
		private const float animDuration = 0.15f;

		private readonly OptionName[] sortings = new OptionName[5]
		{
			OptionName.Difficulty,
			OptionName.LastPlayed,
			OptionName.Song,
			OptionName.Artist,
			OptionName.Author
		};

		public static bool initialized;

		[NonSerialized]
		public bool highlighted;

		[NonSerialized]
		public bool selected;

		public OptionName name;

		public Image image;

		public Text text;

		public bool canBeUnselected;

		private static OptionsPanelsCLS panelsCLS => scnCLS.instance.optionsPanels;

		public void SetState(bool _highlighted, bool _selected)
		{
			if (highlighted != _highlighted)
			{
				highlighted = _highlighted;
				image.DOKill();
				Color endValue = highlighted ? panelsCLS.highlightedColor : panelsCLS.unhighlightedColor;
				image.DOColor(endValue, 0.15f);
			}
			bool flag = selected != _selected;
			bool flag2 = !selected && _selected;
			if (flag)
			{
				selected = _selected;
				text.DOKill();
				Color endValue2 = selected ? panelsCLS.selectedColor : panelsCLS.unselectedColor;
				text.DOColor(endValue2, 0.15f);
			}
			if ((!canBeUnselected && !flag2) || (canBeUnselected && !flag) || !initialized)
			{
				return;
			}
			if (Array.IndexOf(sortings, name) != -1)
			{
				Option[] leftPanelOptions = panelsCLS.leftPanelOptions;
				foreach (Option option in leftPanelOptions)
				{
					if (option.name != name && Array.IndexOf(sortings, option.name) != -1 && option.selected)
					{
						option.SetState(_highlighted: false, _selected: false);
					}
				}
				panelsCLS.sortingMethod = name;
				panelsCLS.UpdateSorting();
			}
			else if (name == OptionName.SpeedTrial)
			{
				panelsCLS.ToggleSpeedTrial();
			}
			else if (name == OptionName.NoFail)
			{
				panelsCLS.ToggleNoFail();
			}
			else if (name == OptionName.Delete)
			{
				scnCLS.instance.DeleteLevel();
			}
			else if (name == OptionName.Find)
			{
				panelsCLS.toggleSearchModeCoroutine = panelsCLS.ToggleSearchMode(selected);
				panelsCLS.StartCoroutine(panelsCLS.toggleSearchModeCoroutine);
			}
		}
	}

	private readonly OptionName[] sortings = new OptionName[5]
	{
		OptionName.Difficulty,
		OptionName.LastPlayed,
		OptionName.Song,
		OptionName.Artist,
		OptionName.Author
	};

	[Header("Panel")]
	public EventSystem eventSystem;

	public CanvasScaler canvas;

	public Image fadeBackground;

	public RectTransform leftPanel;

	public RectTransform rightPanel;

	public Option[] leftPanelOptions;

	public Option[] rightPanelOptions;

	[Header("Search")]
	public InputField searchInputField;

	public SpriteRenderer bgSprite;

	public Color bgColor;

	public Color bgColorSpeedTrial;

	public Text currentOrderText;

	[Header("Color")]
	public Color unhighlightedColor = Color.clear;

	public Color highlightedColor = new Color(1f, 1f, 1f, 0.75f);

	public Color unselectedColor = new Color(1f, 1f, 1f, 0.75f);

	public Color selectedColor = new Color(1f, 0.9921569f, 63f / 85f, 1f);

	[NonSerialized]
	public bool searchMode;

	[NonSerialized]
	public bool speedTrial;

	[NonSerialized]
	public OptionName sortingMethod = OptionName.Difficulty;

	[NonSerialized]
	public bool justHidPanels;

	private IEnumerator toggleSearchModeCoroutine;

	[NonSerialized]
	public bool showingLeftPanel;

	[NonSerialized]
	public bool showingRightPanel;

	private int currentOptionIndex;

	private float leftPanelShowY;

	private float rightPanelShowY;

	private bool searchingUsingShortcut;

	private int searchDeselectedFrame;

	private int showPanelFrame;

	private bool openedPanelUsingShortcut;

	private Option currentOption => (showingLeftPanel ? leftPanelOptions : rightPanelOptions)[currentOptionIndex];

	public bool showingAnyPanel
	{
		get
		{
			if (!showingLeftPanel)
			{
				return showingRightPanel;
			}
			return true;
		}
	}

	private void Awake()
	{
		sortingMethod = Persistence.GetCLSSortingParameter();
		Option[] array = leftPanelOptions;
		foreach (Option option in array)
		{
			if (sortingMethod == option.name)
			{
				option.SetState(_highlighted: false, _selected: true);
			}
		}
		array = rightPanelOptions;
		foreach (Option option2 in array)
		{
			bool num = GCS.speedTrialMode && OptionName.SpeedTrial == option2.name;
			bool flag = GCS.useNoFail && OptionName.NoFail == option2.name;
			if (num | flag)
			{
				option2.SetState(_highlighted: false, _selected: true);
			}
		}
		Option.initialized = true;
		searchInputField.onValueChanged.AddListener(delegate(string sub)
		{
			ADOBase.cls.SearchLevels(sub);
		});
		searchInputField.onEndEdit.AddListener(delegate
		{
			searchDeselectedFrame = Time.frameCount;
			currentOption.SetState(_highlighted: true, _selected: false);
		});
	}

	public bool ChecksInputs()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return false;
		}
		if (justHidPanels)
		{
			justHidPanels = false;
		}
		if (RDInput.leftPress || RDInput.rightPress)
		{
			bool show = RDInput.leftPress ? (!showingLeftPanel) : (!showingRightPanel);
			TogglePanel(RDInput.leftPress, show);
			return true;
		}
		if (showingAnyPanel)
		{
			if (RDInput.cancelPress)
			{
				if (showingLeftPanel)
				{
					TogglePanel(left: true, show: false);
				}
				else if (showingRightPanel)
				{
					TogglePanel(left: false, show: false);
				}
				justHidPanels = true;
				return true;
			}
			if (RDInput.downPress || RDInput.upPress)
			{
				ChangeOption(RDInput.downPress ? 1 : (-1));
				return true;
			}
			if (RDInput.mainPress && searchDeselectedFrame != Time.frameCount && showPanelFrame != Time.frameCount)
			{
				bool selected = !currentOption.canBeUnselected || !currentOption.selected;
				currentOption.SetState(_highlighted: true, selected);
				return true;
			}
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.F) && !searchMode)
		{
			if (!showingLeftPanel)
			{
				TogglePanel(left: true, show: true);
			}
			SelectOption(0);
			openedPanelUsingShortcut = true;
			return true;
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.S))
		{
			SelectOption(OptionName.SpeedTrial, leftOptions: false);
			return true;
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.N))
		{
			SelectOption(OptionName.NoFail, leftOptions: false);
			return true;
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Delete))
		{
			ADOBase.cls.DeleteLevel();
			return true;
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.O))
		{
			int num = Array.IndexOf(sortings, sortingMethod);
			num = ((num != sortings.Length - 1) ? (num + 1) : 0);
			sortingMethod = sortings[num];
			SelectOption(sortingMethod, leftOptions: true);
			return true;
		}
		if (openedPanelUsingShortcut)
		{
			TogglePanel(left: true, show: false);
			openedPanelUsingShortcut = false;
			return true;
		}
		if (showingAnyPanel)
		{
			return true;
		}
		return false;
	}

	public void HideAnyPanel()
	{
		if (showingLeftPanel)
		{
			ToggleLeftPanel();
		}
		else if (showingRightPanel)
		{
			ToggleRightPanel();
		}
	}

	public void ToggleLeftPanel()
	{
		TogglePanel(left: true, !showingLeftPanel);
	}

	public void ToggleRightPanel()
	{
		TogglePanel(left: false, !showingRightPanel);
	}

	public void SelectOption(int option)
	{
		openedPanelUsingShortcut = false;
		currentOption.SetState(_highlighted: false, currentOption.selected);
		currentOptionIndex = option;
		currentOption.SetState(_highlighted: true, currentOption.selected);
		bool selected = currentOption.name == OptionName.Find || !currentOption.canBeUnselected || !currentOption.selected;
		currentOption.SetState(_highlighted: true, selected);
	}

	private void TogglePanel(bool left, bool show)
	{
		if (!left)
		{
			Option[] array = rightPanelOptions;
			foreach (Option option in array)
			{
				if (option.name == OptionName.Delete)
				{
					option.SetState(_highlighted: false, ADOBase.cls.levelDeleted);
				}
			}
		}
		showPanelFrame = Time.frameCount;
		RectTransform target = left ? leftPanel : rightPanel;
		float endValue = (!show) ? 0f : (left ? leftPanel.sizeDelta.y : rightPanel.sizeDelta.y);
		if (!eventSystem.alreadySelecting)
		{
			eventSystem.SetSelectedGameObject(null);
		}
		if (show)
		{
			ADOBase.controller.responsive = false;
			fadeBackground.DOKill();
			fadeBackground.DOFade(0.4f, 0.1f);
		}
		else
		{
			if ((left && !showingRightPanel) || (!left && !showingLeftPanel))
			{
				StartCoroutine(EnableInputCo());
				fadeBackground.DOKill();
				fadeBackground.DOFade(0f, 0.1f);
			}
			if (currentOptionIndex != 0)
			{
				currentOption.SetState(_highlighted: false, currentOption.selected);
			}
		}
		if (left)
		{
			if (show && showingRightPanel)
			{
				TogglePanel(left: false, show: false);
			}
			showingLeftPanel = show;
		}
		else
		{
			if (show && showingLeftPanel)
			{
				TogglePanel(left: true, show: false);
			}
			showingRightPanel = show;
		}
		printe($"showing panels left {showingLeftPanel} right {showingRightPanel}");
		target.DOKill();
		Ease ease = show ? Ease.OutQuad : Ease.InQuad;
		target.DOAnchorPosY(endValue, 0.1f).SetEase(ease);
		if (show)
		{
			currentOptionIndex = 0;
			currentOption.SetState(_highlighted: true, currentOption.selected);
		}
	}

	private IEnumerator EnableInputCo()
	{
		yield return new WaitForEndOfFrame();
		if (!showingLeftPanel && !showingRightPanel)
		{
			ADOBase.controller.responsive = true;
		}
	}

	private void ChangeOption(int direction)
	{
		Option[] array = showingLeftPanel ? leftPanelOptions : rightPanelOptions;
		if ((currentOptionIndex != array.Length - 1 || direction <= 0) && (currentOptionIndex != 0 || direction >= 0))
		{
			currentOption.SetState(_highlighted: false, currentOption.selected);
			currentOptionIndex += direction;
			currentOption.SetState(_highlighted: true, currentOption.selected);
		}
	}

	private void SelectOption(OptionName name, bool leftOptions)
	{
		bool num = leftOptions ? showingLeftPanel : showingRightPanel;
		Option[] array = leftOptions ? leftPanelOptions : rightPanelOptions;
		if (num)
		{
			currentOption.SetState(_highlighted: false, currentOption.selected);
			currentOptionIndex = Array.FindIndex(array, (Option option) => option.name == name);
			bool selected = !currentOption.canBeUnselected || !currentOption.selected;
			currentOption.SetState(_highlighted: true, selected);
			return;
		}
		Option[] array2 = array;
		foreach (Option option2 in array2)
		{
			if (option2.name == name)
			{
				bool selected2 = !option2.canBeUnselected || !option2.selected;
				option2.SetState(_highlighted: true, selected2);
			}
		}
	}

	private void UpdateSorting()
	{
		ADOBase.cls.sortedLevelKeys = SortedLevelKeys();
		ADOBase.cls.SearchLevels(ADOBase.cls.searchParameter);
		UpdateOrderText();
		Persistence.SetCLSSortingParameter(sortingMethod);
	}

	public List<string> SortedLevelKeys()
	{
		List<string> list = new List<string>();
		Dictionary<string, GenericDataCLS> loadedLevels = ADOBase.cls.loadedLevels;
		IOrderedEnumerable<KeyValuePair<string, GenericDataCLS>> orderedEnumerable = null;
		switch (sortingMethod)
		{
		case OptionName.Difficulty:
			orderedEnumerable = loadedLevels.OrderByDescending(delegate(KeyValuePair<string, GenericDataCLS> pair)
			{
				KeyValuePair<string, GenericDataCLS> keyValuePair16 = pair;
				return keyValuePair16.Value.difficulty;
			}).ThenBy(delegate(KeyValuePair<string, GenericDataCLS> pair)
			{
				KeyValuePair<string, GenericDataCLS> keyValuePair15 = pair;
				return keyValuePair15.Value.artist.RemoveRichTags();
			}).ThenBy(delegate(KeyValuePair<string, GenericDataCLS> pair)
			{
				KeyValuePair<string, GenericDataCLS> keyValuePair14 = pair;
				return keyValuePair14.Value.title.RemoveRichTags();
			});
			break;
		case OptionName.LastPlayed:
			orderedEnumerable = ((ADOBase.cls.newlyInstalledLevelKeys.Count == 0) ? loadedLevels.OrderByDescending(delegate(KeyValuePair<string, GenericDataCLS> pair)
			{
				KeyValuePair<string, GenericDataCLS> keyValuePair10 = pair;
				return Persistence.GetCustomWorldPlayIndex(keyValuePair10.Value.Hash);
			}).ThenBy(delegate(KeyValuePair<string, GenericDataCLS> pair)
			{
				KeyValuePair<string, GenericDataCLS> keyValuePair9 = pair;
				return keyValuePair9.Value.artist;
			}).ThenBy(delegate(KeyValuePair<string, GenericDataCLS> pair)
			{
				KeyValuePair<string, GenericDataCLS> keyValuePair8 = pair;
				return keyValuePair8.Value.title;
			}) : loadedLevels.OrderBy(delegate(KeyValuePair<string, GenericDataCLS> pair)
			{
				List<string> newlyInstalledLevelKeys = ADOBase.cls.newlyInstalledLevelKeys;
				KeyValuePair<string, GenericDataCLS> keyValuePair7 = pair;
				return newlyInstalledLevelKeys.Contains(keyValuePair7.Key);
			}).ThenByDescending(delegate(KeyValuePair<string, GenericDataCLS> pair)
			{
				KeyValuePair<string, GenericDataCLS> keyValuePair6 = pair;
				return Persistence.GetCustomWorldPlayIndex(keyValuePair6.Value.Hash);
			}).ThenBy(delegate(KeyValuePair<string, GenericDataCLS> pair)
			{
				KeyValuePair<string, GenericDataCLS> keyValuePair5 = pair;
				return keyValuePair5.Value.artist;
			})
				.ThenBy(delegate(KeyValuePair<string, GenericDataCLS> pair)
				{
					KeyValuePair<string, GenericDataCLS> keyValuePair4 = pair;
					return keyValuePair4.Value.title;
				}));
			break;
		case OptionName.Song:
			orderedEnumerable = loadedLevels.OrderBy(delegate(KeyValuePair<string, GenericDataCLS> pair)
			{
				KeyValuePair<string, GenericDataCLS> keyValuePair13 = pair;
				return keyValuePair13.Value.title.RemoveRichTags();
			}).ThenBy(delegate(KeyValuePair<string, GenericDataCLS> pair)
			{
				KeyValuePair<string, GenericDataCLS> keyValuePair12 = pair;
				return keyValuePair12.Value.artist.RemoveRichTags();
			}).ThenByDescending(delegate(KeyValuePair<string, GenericDataCLS> pair)
			{
				KeyValuePair<string, GenericDataCLS> keyValuePair11 = pair;
				return keyValuePair11.Value.difficulty;
			});
			break;
		case OptionName.Artist:
			orderedEnumerable = loadedLevels.OrderBy(delegate(KeyValuePair<string, GenericDataCLS> pair)
			{
				KeyValuePair<string, GenericDataCLS> keyValuePair3 = pair;
				return keyValuePair3.Value.artist.RemoveRichTags();
			}).ThenBy(delegate(KeyValuePair<string, GenericDataCLS> pair)
			{
				KeyValuePair<string, GenericDataCLS> keyValuePair2 = pair;
				return keyValuePair2.Value.title.RemoveRichTags();
			}).ThenByDescending(delegate(KeyValuePair<string, GenericDataCLS> pair)
			{
				KeyValuePair<string, GenericDataCLS> keyValuePair = pair;
				return keyValuePair.Value.difficulty;
			});
			break;
		case OptionName.Author:
			orderedEnumerable = loadedLevels.OrderBy(delegate(KeyValuePair<string, GenericDataCLS> pair)
			{
				KeyValuePair<string, GenericDataCLS> keyValuePair19 = pair;
				return keyValuePair19.Value.author.RemoveRichTags();
			}).ThenBy(delegate(KeyValuePair<string, GenericDataCLS> pair)
			{
				KeyValuePair<string, GenericDataCLS> keyValuePair18 = pair;
				return keyValuePair18.Value.title.RemoveRichTags();
			}).ThenBy(delegate(KeyValuePair<string, GenericDataCLS> pair)
			{
				KeyValuePair<string, GenericDataCLS> keyValuePair17 = pair;
				return keyValuePair17.Value.artist.RemoveRichTags();
			});
			break;
		}
		foreach (KeyValuePair<string, GenericDataCLS> item in orderedEnumerable)
		{
			list.Add(item.Key);
		}
		return list;
	}

	public void UpdateOrderText()
	{
		string str = RDString.Get("cls.orderBy." + sortingMethod.ToString());
		string text = RDString.Get("cls.shortcut.order") + " <color=lightblue><i>[" + str + "]</i></color>";
		currentOrderText.text = text;
	}

	private void ToggleNoFail()
	{
		GCS.useNoFail = !GCS.useNoFail;
		scrFlash.Flash(GCS.useNoFail ? Color.green.WithAlpha(0.3f) : Color.gray.WithAlpha(0.7f), 0.5f);
		scrSfx.instance.PlaySfx(GCS.useNoFail ? SfxSound.ModifierActivate : SfxSound.ModifierDeactivate);
	}

	private void ToggleSpeedTrial()
	{
		speedTrial = !speedTrial;
		scrFlash.Flash(Color.white, 0.5f);
		bgSprite.color = (speedTrial ? bgColorSpeedTrial : bgColor);
	}

	private void LateUpdate()
	{
		fadeBackground.raycastTarget = showingAnyPanel;
	}

	public IEnumerator ToggleSearchMode(bool search)
	{
		searchMode = search;
		if (search && RDC.runningOnSteamDeck)
		{
			while (!SteamWorkshop.ShowTextInput())
			{
				yield return null;
			}
		}
		if (searchingUsingShortcut)
		{
			TogglePanel(left: true, search);
		}
		if (search)
		{
			searchInputField.ActivateInputField();
			yield break;
		}
		if (!eventSystem.alreadySelecting)
		{
			eventSystem.SetSelectedGameObject(null);
		}
		searchingUsingShortcut = false;
		toggleSearchModeCoroutine = null;
	}
}
