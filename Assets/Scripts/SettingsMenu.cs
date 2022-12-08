using DG.Tweening;
using GDMiniJSON;
using SA.GoogleDoc;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : ADOBase
{
	public enum Interaction
	{
		Refresh,
		Activate,
		ActivateInfo,
		Increment,
		Decrement
	}

	[Header("Components")]
	public GameObject buttonPrefab;

	public GameObject tabButtonPrefab;

	public Transform settingsContainer;

	public RectTransform settingsScrollRectContent;

	public ScrollRect scrollRect;

	public Transform tabButtonsContainer;

	private int confirmationProgress;

	private int selectedTab;

	private int selectedIndex;

	private List<List<PauseSettingButton>> settingsTabs;

	private List<SettingsTabButton> tabButtons;

	public int settingsThatRequireRestart;

	public bool isSelectingTab;

	private const int NotYetKnown = -100;

	private int resolutionIndex = -100;

	private PauseMenu pauseMenu => scrController.instance.pauseMenu;

	private List<PauseSettingButton> currentSettings => settingsTabs[selectedTab];

	private PauseSettingButton selectedSetting => currentSettings[selectedIndex];

	private void Awake()
	{
		GenerateSettings();
		tabButtonsContainer.gameObject.SetActive(value: false);
	}

	public void Show()
	{
		isSelectingTab = true;
		base.gameObject.SetActive(value: true);
		selectedIndex = -1;
		SelectTab(0, force: true);
	}

	public void GenerateSettings()
	{
		string text = Resources.Load<TextAsset>("PauseMenuSettings").text;
		settingsTabs = new List<List<PauseSettingButton>>();
		tabButtons = new List<SettingsTabButton>();
		foreach (KeyValuePair<string, object> item in Json.Deserialize(text) as Dictionary<string, object>)
		{
			List<object> obj = item.Value as List<object>;
			string key = item.Key;
			List<PauseSettingButton> list = new List<PauseSettingButton>();
			foreach (object item2 in obj)
			{
				Dictionary<string, object> dictionary = item2 as Dictionary<string, object>;
				string text2 = dictionary["name"] as string;
				if (!dictionary.ContainsKey("exclude"))
				{
					if (dictionary.ContainsKey("available"))
					{
						bool flag = false;
						foreach (object item3 in dictionary["available"] as List<object>)
						{
							bool flag2 = (ADOBase.platform == Platform.Mac || ADOBase.platform == Platform.Windows || ADOBase.platform == Platform.Linux) && !RDC.runningOnSteamDeck;
							string a = (string)item3;
							if ((a == "iOS" && ADOBase.platform == Platform.iOS) || (a == "android" && ADOBase.platform == Platform.Android) || (a == "mobile" && ADOBase.isMobile) || (a == "desktop" && flag2) || (a == "steamDeck" && RDC.runningOnSteamDeck))
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							continue;
						}
					}
					PauseSettingButton component = UnityEngine.Object.Instantiate(buttonPrefab, settingsContainer).GetComponent<PauseSettingButton>();
					component.name = text2;
					list.Add(component);
					component.type = (string)dictionary["type"];
					if (component.type == "Action")
					{
						component.leftArrow.transform.ScaleXY(0f, 0f);
						component.rightArrow.transform.ScaleXY(0f, 0f);
					}
					if (dictionary.ContainsKey("min"))
					{
						component.minInt = (int)dictionary["min"];
					}
					if (dictionary.ContainsKey("max"))
					{
						component.maxInt = (int)dictionary["max"];
					}
					if (dictionary.ContainsKey("unit"))
					{
						component.unit = (string)dictionary["unit"];
					}
					if (dictionary.ContainsKey("changeBy"))
					{
						component.changeBy = (int)dictionary["changeBy"];
					}
					dictionary.ContainsKey("flipDescription");
					if (dictionary.ContainsKey("changeBySmall"))
					{
						component.changeBySmall = (int)dictionary["changeBySmall"];
					}
					else
					{
						component.changeBySmall = component.changeBy;
					}
					if (dictionary.ContainsKey("restartOnChange"))
					{
						component.restartOnChange = (bool)dictionary["restartOnChange"];
					}
					bool exists = false;
					if (ADOBase.isMobile)
					{
						PauseSettingButton pauseSettingButton = component;
						pauseSettingButton.descriptionKey = pauseSettingButton.descriptionKey + "pauseMenu.settings.info." + text2 + ".mobile";
						RDString.GetWithCheck(component.descriptionKey, out exists);
					}
					if (!exists)
					{
						component.descriptionKey = "pauseMenu.settings.info." + text2;
						RDString.GetWithCheck(component.descriptionKey, out exists);
					}
					bool flag3 = text2 == "inputOffset" && ADOBase.isGamepad;
					component.hasDescription = (!flag3 && exists);
					UpdateSetting(component, Interaction.Refresh);
					component.SetFocus(focus: false);
					component.label.text = RDString.Get("pauseMenu.settings." + component.name);
					component.label.SetLocalizedFont();
					component.valueLabel.SetLocalizedFont();
				}
			}
			if (list.Count > 0)
			{
				SettingsTabButton component2 = UnityEngine.Object.Instantiate(tabButtonPrefab, tabButtonsContainer).GetComponent<SettingsTabButton>();
				tabButtons.Add(component2);
				component2.index = tabButtons.Count - 1;
				component2.SetFocus(focus: false);
				component2.name = key;
				component2.label.text = RDString.Get("pauseMenu.settings.category." + key);
				component2.label.SetLocalizedFont();
				component2.icon.sprite = Resources.Load<Sprite>("SettingsTabIcon/" + key);
				settingsTabs.Add(list);
			}
		}
	}

	public void UpdateSelectedSetting(Interaction action)
	{
		UpdateSetting(selectedSetting, action);
	}

	private void Update()
	{
		if (RDInput.cancelPress)
		{
			if (ADOBase.isEditingLevel)
			{
				pauseMenu.Hide();
			}
			else
			{
				pauseMenu.PlaySfx(SfxSound.MenuSquelch, 1.5f);
			}
			Persistence.Save();
		}
		if (isSelectingTab)
		{
			if (RDInput.leftPress)
			{
				SelectPreviousTab();
			}
			else if (RDInput.rightPress)
			{
				SelectNextTab();
			}
			else if (RDInput.mainPress || RDInput.downPress)
			{
				isSelectingTab = false;
				Select(0, force: true);
			}
		}
		else if (RDInput.downPress)
		{
			SelectNextOption();
		}
		else if (RDInput.upPress)
		{
			SelectPreviousOption();
		}
		else if (RDInput.leftPress)
		{
			UpdateSelectedSetting(Interaction.Decrement);
		}
		else if (RDInput.rightPress)
		{
			UpdateSelectedSetting(Interaction.Increment);
		}
		else if (RDInput.mainPress)
		{
			UpdateSelectedSetting(Interaction.Activate);
		}
		ArrangeTabButtons();
	}

	private void UpdateSetting(PauseSettingButton setting, Interaction action)
	{
		bool flag = action == Interaction.Increment;
		bool flag2 = action == Interaction.Decrement;
		bool refresh = action == Interaction.Refresh;
		bool flag3 = setting.type == "Action";
		bool holdingShift = RDInput.holdingShift;
		bool flag4 = false;
		object value = null;
		Action<object> action2 = null;
		Action action3 = null;
		Action action4 = null;
		switch (setting.name)
		{
		case "volume":
			value = scrController.volume;
			action2 = delegate(object newVolume)
			{
				scrController.volume = (int)newVolume;
			};
			break;
		case "fullscreen":
			value = Screen.fullScreen;
			action2 = delegate(object fullscreen)
			{
				Screen.fullScreenMode = (((bool)fullscreen) ? FullScreenMode.MaximizedWindow : FullScreenMode.Windowed);
			};
			break;
		case "resolution":
			value = ((resolutionIndex == -100) ? NearestResolution(Screen.currentResolution) : Screen.resolutions[resolutionIndex]);
			action2 = delegate(object resolution)
			{
				resolutionIndex = Array.IndexOf(Screen.resolutions, (Resolution)resolution);
			};
			action3 = delegate
			{
				Resolution resolution3 = Screen.resolutions[resolutionIndex];
				Screen.SetResolution(resolution3.width, resolution3.height, Screen.fullScreenMode, resolution3.refreshRate);
			};
			break;
		case "mobileResolution":
			value = GetCurrentResolutionPercent();
			action2 = delegate(object percent)
			{
				int width = Mathf.RoundToInt((float)(int)percent * 0.01f * (float)ADOBase.GetDisplayWidth());
				int height = Mathf.RoundToInt((float)(int)percent * 0.01f * (float)ADOBase.GetDisplayHeight());
				Screen.SetResolution(width, height, fullscreen: true);
			};
			break;
		case "targetFramerate":
			value = Math.Clamp(Application.targetFrameRate, setting.minInt, setting.maxInt);
			if (refresh)
			{
				flag4 = true;
			}
			action2 = delegate(object framerate)
			{
				bool flag8 = (int)framerate == setting.maxInt;
				if (!refresh)
				{
					Persistence.SetTargetFramerate(flag8 ? 10000 : ((int)framerate));
				}
				if (flag8)
				{
					setting.valueLabel.text = RDString.Get("pauseMenu.settings.noFramerateLimit");
				}
			};
			break;
		case "visualEffects":
			value = Persistence.GetVisualEffects(getReal: true);
			action2 = delegate(object effects)
			{
				Persistence.SetVisualEffects((VisualEffects)effects);
			};
			break;
		case "visualQuality":
			value = Persistence.GetVisualQuality();
			action2 = delegate(object quality)
			{
				Persistence.SetVisualQuality((VisualQuality)quality);
			};
			break;
		case "animateSpeedChanges":
			value = Persistence.GetAnimateSpeedChange();
			action2 = delegate(object animate)
			{
				Persistence.SetAnimateSpeedChanges((bool)animate);
			};
			break;
		case "antiAliasing":
			value = QualitySettings.antiAliasing;
			action2 = delegate(object samples)
			{
				QualitySettings.antiAliasing = (int)samples;
				Persistence.SetFXAA((int)samples);
			};
			break;
		case "vibration":
			value = Persistence.GetVibration();
			action2 = delegate(object vibration)
			{
				Persistence.SetVibration((bool)vibration);
			};
			break;
		case "inputOffset":
			value = Mathf.RoundToInt(scrConductor.calibration_i * 1000f);
			action2 = delegate(object offset)
			{
				scrConductor.currentPreset.inputOffset = (int)offset;
				scrConductor.SaveCurrentPreset();
			};
			if (ADOBase.isMobile)
			{
				action4 = delegate
				{
					Persistence.Save();
					ADOBase.GoToCalibration();
				};
			}
			break;
		case "language":
			value = RDUtils.ParseEnum(Persistence.GetLanguage(), SystemLanguage.English);
			action2 = delegate(object language)
			{
				Persistence.SetLanguage((SystemLanguage)language);
			};
			action3 = delegate
			{
				RDString.ChangeLanguage((SystemLanguage)value);
				Persistence.Save();
				ADOBase.RestartScene();
			};
			action4 = action3;
			break;
		case "vsync":
			value = (QualitySettings.vSyncCount > 0);
			action2 = delegate(object vsync)
			{
				Persistence.SetVSync(QualitySettings.vSyncCount = (((bool)vsync) ? 1 : 0));
			};
			break;
		case "perfectsOnlyMode":
			value = GCS.perfectOnlyMode;
			action2 = delegate(object perfectOnly)
			{
				GCS.perfectOnlyMode = (bool)perfectOnly;
				Persistence.SetPerfectsOnlyMode((bool)perfectOnly);
			};
			break;
		case "showXAccuracy":
			value = Persistence.GetShowXAccuracy();
			action2 = delegate(object showXAccuracy)
			{
				Persistence.SetShowXAccuracy((bool)showXAccuracy);
			};
			break;
		case "hitErrorMeterSize":
			value = Persistence.GetHitErrorMeterSize();
			action2 = delegate(object meterSizeObj)
			{
				ErrorMeterSize errorMeterSize = (ErrorMeterSize)meterSizeObj;
				ErrorMeterShape hitErrorMeterShape = Persistence.GetHitErrorMeterShape();
				if ((bool)ADOBase.controller.errorMeter)
				{
					ADOBase.controller.errorMeter.UpdateLayout(errorMeterSize, hitErrorMeterShape);
				}
				Persistence.SetHitErrorMeterSize(errorMeterSize);
			};
			break;
		case "hitErrorMeterShape":
			value = Persistence.GetHitErrorMeterShape();
			action2 = delegate(object meterShapeObj)
			{
				ErrorMeterSize hitErrorMeterSize = Persistence.GetHitErrorMeterSize();
				ErrorMeterShape errorMeterShape = (ErrorMeterShape)meterShapeObj;
				if ((bool)ADOBase.controller.errorMeter)
				{
					ADOBase.controller.errorMeter.UpdateLayout(hitErrorMeterSize, errorMeterShape);
				}
				Persistence.SetHitErrorMeterShape(errorMeterShape);
			};
			break;
		case "showDetailedResults":
			value = scrController.showDetailedResults;
			action2 = delegate(object detailedResults)
			{
				scrController.showDetailedResults = (bool)detailedResults;
				Persistence.SetShowDetailedResults((bool)detailedResults);
			};
			break;
		case "hideCursorWhilePlaying":
			value = Persistence.GetHideCursorWhilePlaying();
			action2 = delegate(object hideCursorWhilePlaying)
			{
				Persistence.SetHideCursorWhilePlaying((bool)hideCursorWhilePlaying);
			};
			break;
		case "skipIntroAfterFirstTry":
			value = Persistence.GetSkipIntroAfterFirstTry();
			action2 = delegate(object skipIntroAfterFirstTry)
			{
				Persistence.SetSkipIntroAfterFirstTry((bool)skipIntroAfterFirstTry);
			};
			break;
		case "multitapTileBehavior":
			value = Persistence.GetMultitapTileBehavior();
			action2 = delegate(object behavior)
			{
				Persistence.SetMultitapTileBehavior((MultitapTileBehavior)behavior);
			};
			break;
		case "holdBehavior":
			value = Persistence.GetHoldBehavior();
			action2 = delegate(object behavior)
			{
				scrController.instance.strictHoldsSaved = ((int)behavior == 0);
				scrController.instance.strictHolds = scrController.instance.strictHoldsSaved;
				scrController.instance.requireHolding = ((int)behavior < 2);
				Persistence.SetHoldBehavior((HoldBehavior)behavior);
			};
			break;
		case "freeroamInvuln":
			value = Persistence.GetFreeroamInvulnerability();
			action2 = delegate(object freeroamInvulnerability)
			{
				scrController.instance.freeroamInvulnerability = (bool)freeroamInvulnerability;
				Persistence.SetFreeroamInvulnerability((bool)freeroamInvulnerability);
			};
			break;
		case "markFloorWithComment":
			value = Persistence.GetMarkFloorWithComment();
			action2 = delegate(object markFloorWithComment)
			{
				Persistence.SetMarkFloorWithComment((bool)markFloorWithComment);
				ADOBase.editor.ApplyEventsToFloors();
			};
			break;
		case "disableRewindButton":
			value = Persistence.GetDisableRewindButton();
			action2 = delegate(object disableRewindButton)
			{
				Persistence.SetDisableRewindButton((bool)disableRewindButton);
			};
			break;
		case "shortcutPitchedPlaySpeed":
			value = Persistence.GetShortcutPlaySpeed();
			action2 = delegate(object shortcutPitchedPlaySpeed)
			{
				Persistence.SetShortcutPlaySpeed((int)shortcutPitchedPlaySpeed);
			};
			break;
		case "recoverDataSteam":
			action3 = delegate
			{
				Persistence.RecoverSaveDataFromSteamAchievements();
				ADOBase.GoToLevelSelect();
			};
			break;
		case "clearData":
			action3 = delegate
			{
				ClearDataButtonWasActivated();
			};
			break;
		case "clearDlc":
			action3 = delegate
			{
				ClearDlcButtonWasActivated();
			};
			break;
		case "useAsynchronousInput":
			value = Persistence.GetChosenAsynchronousInput();
			action2 = delegate(object enabled)
			{
				bool num7 = (bool)enabled;
				RDC.useAsyncInput = num7;
				Persistence.SetChosenAsynchronousInput(num7);
			};
			break;
		}
		bool flag5 = false;
		if (!flag3)
		{
			if (value == null)
			{
				UnityEngine.Debug.LogError("No getter for " + setting.name + "!");
				return;
			}
			if (action2 == null)
			{
				UnityEngine.Debug.LogError("No setter for " + setting.name + "!");
				return;
			}
			if (action != 0)
			{
				flag5 = setting.initialValue.Equals(value);
			}
		}
		bool flag6 = false;
		if (action != Interaction.Activate)
		{
			if (setting.type == "Int")
			{
				int num = (setting.changeBy <= 0) ? 1 : (holdingShift ? setting.changeBySmall : setting.changeBy);
				int num2 = (int)value;
				if (flag)
				{
					value = (int)value + num;
				}
				else if (flag2)
				{
					value = (int)value - num;
				}
				if (setting.hasRange)
				{
					value = Mathf.Clamp((int)value, setting.minInt, setting.maxInt);
				}
				if ((int)value != num2)
				{
					flag6 = true;
				}
				if (flag6 | refresh)
				{
					setting.valueLabel.text = (value?.ToString() ?? "");
					if (!setting.unit.IsNullOrEmpty())
					{
						bool exists = false;
						string withCheck = RDString.GetWithCheck("editor.unit." + setting.unit, out exists);
						setting.valueLabel.text += (exists ? withCheck : setting.unit);
					}
				}
			}
			else if (setting.type == "Bool")
			{
				if (!refresh)
				{
					flag6 = true;
					value = !(bool)value;
				}
				setting.valueLabel.text = RDString.Get(((bool)value) ? "pauseMenu.settings.on" : "pauseMenu.settings.off");
			}
			else if (setting.type == "Resolution")
			{
				Resolution value2 = (Resolution)value;
				int num3 = Array.IndexOf(Screen.resolutions, value2);
				if (flag2 && num3 > 0)
				{
					flag6 = true;
					num3--;
				}
				else if (flag && num3 < Screen.resolutions.Length - 1)
				{
					flag6 = true;
					num3++;
				}
				value = Screen.resolutions[num3];
				Resolution resolution2 = (Resolution)value;
				setting.valueLabel.text = $"{resolution2.width}x{resolution2.height} {resolution2.refreshRate}hz";
			}
			else if (setting.type == "Samples")
			{
				value = Math.Max(1, (int)value);
				int num4 = (int)value;
				if (flag)
				{
					value = (int)value * 2;
				}
				else if (flag2)
				{
					value = (int)value / 2;
				}
				value = Mathf.Clamp((int)value, 1, 8);
				if ((int)value != num4)
				{
					flag6 = true;
				}
				setting.valueLabel.text = (((int)value == 1) ? RDString.Get("pauseMenu.settings.off") : (value?.ToString() + "x"));
			}
			else if (setting.type.StartsWith("Enum:"))
			{
				string text = setting.type.Remove(0, 5);
				Type type;
				int[] array;
				if (text == "Language")
				{
					type = typeof(SystemLanguage);
					array = Array.ConvertAll(RDString.AvailableLanguages, (SystemLanguage v) => (int)v);
				}
				else
				{
					type = Type.GetType(text);
					array = (int[])Enum.GetValues(type);
				}
				int num5 = 0;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == (int)value)
					{
						num5 = i;
						break;
					}
				}
				if (flag)
				{
					flag6 = true;
					num5 = (num5 + 1) % array.Length;
				}
				else if (flag2)
				{
					flag6 = true;
					num5 = (num5 + array.Length - 1) % array.Length;
				}
				value = Enum.ToObject(type, array[num5]);
				if (text == "Language")
				{
					SystemLanguage language2 = (SystemLanguage)value;
					FontData fontDataForLanguage = RDString.GetFontDataForLanguage(language2);
					setting.valueLabel.text = GetNativeLanguageName(language2);
					setting.valueLabel.font = fontDataForLanguage.font;
					setting.valueLabel.fontSize = Mathf.RoundToInt(fontDataForLanguage.fontScale * 9f);
				}
				else
				{
					setting.valueLabel.text = RDString.Get("pauseMenu.settings." + type.ToString() + "." + type.GetEnumName(value));
				}
			}
		}
		if (action == Interaction.Refresh)
		{
			setting.initialValue = value;
		}
		if (flag6 | flag4)
		{
			action2(value);
			if (!flag3)
			{
				if (flag)
				{
					setting.PlayArrowAnimation(isRight: true);
					pauseMenu.PlaySfx(SfxSound.MenuSquelch, 1.5f);
				}
				else if (flag2)
				{
					setting.PlayArrowAnimation(isRight: false);
					pauseMenu.PlaySfx(SfxSound.MenuSquelch, 1.5f);
				}
			}
			if (setting.restartOnChange)
			{
				bool flag7 = setting.initialValue.Equals(value);
				if (flag5 != flag7)
				{
					if (flag7)
					{
						settingsThatRequireRestart--;
					}
					else
					{
						settingsThatRequireRestart++;
					}
				}
			}
		}
		if (action == Interaction.Activate && action3 != null)
		{
			confirmationProgress++;
			action3();
		}
		if (action == Interaction.ActivateInfo)
		{
			action4?.Invoke();
		}
	}

	public void Select(PauseSettingButton button)
	{
		Select(currentSettings.IndexOf(button));
	}

	private void Select(int index, bool force = false)
	{
		PauseSettingButton pauseSettingButton = currentSettings[index];
		if ((selectedIndex != index) | force)
		{
			if (selectedIndex >= 0)
			{
				currentSettings[selectedIndex].SetFocus(focus: false);
			}
			tabButtons[selectedTab].SetFocus(focus: false, dontChangeSize: true);
			pauseSettingButton.SetFocus(focus: true);
			pauseMenu.PlaySfx(SfxSound.MenuSquelch, 1.5f);
			selectedIndex = index;
			StartCoroutine(UpdateContentY(index, pauseSettingButton));
		}
		else if (ADOBase.isMobile)
		{
			UpdateSetting(pauseSettingButton, Interaction.Activate);
		}
	}

	private IEnumerator UpdateContentY(int _index, PauseSettingButton _newSetting)
	{
		yield return null;
		float a;
		if (_index == currentSettings.Count - 1)
		{
			a = settingsScrollRectContent.sizeDelta.y - settingsScrollRectContent.parent.GetComponent<RectTransform>().rect.height;
			a = Mathf.Max(a, 0f);
			bool hasDescription = currentSettings[_index].hasDescription;
		}
		else
		{
			RectTransform rectTransform = _newSetting.rectTransform;
			float num = rectTransform.anchoredPosition.y - rectTransform.sizeDelta.y / (_newSetting.hasDescription ? 1f : 2f);
			float height = settingsScrollRectContent.parent.GetComponent<RectTransform>().rect.height;
			a = 0f - (num + height / 2f);
			float y = settingsScrollRectContent.sizeDelta.y;
			a = Mathf.Clamp(a, 0f, Mathf.Max(0f, y - height));
		}
		settingsScrollRectContent.DOKill();
		scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
		scrollRect.inertia = false;
		settingsScrollRectContent.DOAnchorPosY(a, 0.15f).SetUpdate(isIndependentUpdate: true).OnComplete(delegate
		{
			scrollRect.inertia = true;
			scrollRect.movementType = ScrollRect.MovementType.Elastic;
		});
		confirmationProgress = 0;
	}

	private void SelectPreviousOption()
	{
		int index = (selectedIndex + currentSettings.Count - 1) % currentSettings.Count;
		Select(index);
	}

	private void SelectNextOption()
	{
		int index = (selectedIndex + 1) % currentSettings.Count;
		Select(index);
	}

	private void SelectPreviousTab()
	{
		int index = (selectedTab + tabButtons.Count - 1) % tabButtons.Count;
		SelectTab(index);
	}

	private void SelectNextTab()
	{
		int index = (selectedTab + 1) % tabButtons.Count;
		SelectTab(index);
	}

	private int GetCurrentResolutionPercent()
	{
		return Mathf.RoundToInt((float)Screen.width / (float)ADOBase.GetDisplayWidth() * 100f);
	}

	private string GetNativeLanguageName(SystemLanguage language)
	{
		LangCode lang = RDUtils.ParseEnum(language.ToString(), LangCode.English);
		return Localization.GetLocalizedString("pauseMenu.settings.myLanguage", Localization.DefaultSection, lang);
	}

	private Resolution NearestResolution(Resolution curResolution)
	{
		int num = int.MaxValue;
		int num2 = int.MaxValue;
		int num3 = 0;
		int num4 = 0;
		Resolution[] resolutions = Screen.resolutions;
		for (int i = 0; i < resolutions.Length; i++)
		{
			Resolution resolution = resolutions[i];
			int num5 = Mathf.Abs(curResolution.width - resolution.width) + Mathf.Abs(curResolution.height - resolution.height);
			int num6 = curResolution.refreshRate - resolution.refreshRate;
			if (num5 < num)
			{
				num = num5;
				num2 = int.MaxValue;
				num3 = num4;
			}
			else if (num5 == num && num6 < num2)
			{
				num2 = num6;
				num3 = num4;
			}
			if (curResolution.width == resolution.width && curResolution.height == resolution.height && curResolution.refreshRate == resolution.refreshRate)
			{
				num3 = num4;
				break;
			}
			num4++;
		}
		return Screen.resolutions[num3];
	}

	private void ClearDataButtonWasActivated()
	{
		Text info = selectedSetting.info;
		switch (confirmationProgress)
		{
		case 1:
			info.text = RDString.Get("pauseMenu.settings.clearData2Left");
			break;
		case 2:
			info.text = RDString.Get("pauseMenu.settings.clearData1Left");
			break;
		case 3:
			info.text = RDString.Get("pauseMenu.settings.dataErased");
			DOVirtual.DelayedCall(0.25f, delegate
			{
				Persistence.ClearData();
				ADOBase.GoToLevelSelect();
			});
			break;
		}
	}

	private void ClearDlcButtonWasActivated()
	{
		Text info = selectedSetting.info;
		switch (confirmationProgress)
		{
		case 1:
			info.text = RDString.Get("pauseMenu.settings.clearData2Left");
			break;
		case 2:
			info.text = RDString.Get("pauseMenu.settings.clearData1Left");
			break;
		case 3:
			info.text = RDString.Get("pauseMenu.settings.dataErased");
			DOVirtual.DelayedCall(0.25f, delegate
			{
				Persistence.ResetTaroStoryProgress();
				ADOBase.GoToLevelSelect();
			});
			break;
		}
	}

	public void StopBrowsingTab()
	{
		isSelectingTab = true;
		if (selectedIndex >= 0)
		{
			currentSettings[selectedIndex].SetFocus(focus: false);
		}
		tabButtons[selectedTab].SetFocus(focus: true, dontChangeSize: true);
		selectedIndex = -1;
	}

	public void SelectTab(SettingsTabButton button)
	{
		SelectTab(tabButtons.IndexOf(button));
	}

	public void SelectTab(int index, bool force = false)
	{
		if ((selectedTab != index) | force)
		{
			tabButtons[selectedTab].SetFocus(focus: false);
			tabButtons[index].SetFocus(focus: true);
			selectedTab = index;
			if (selectedIndex >= 0)
			{
				currentSettings[selectedIndex].SetFocus(focus: false);
			}
			settingsScrollRectContent.AnchorPosY(0f);
			if (!force)
			{
				pauseMenu.PlaySfx(SfxSound.MenuSquelch, 1.5f);
			}
			for (int i = 0; i < settingsTabs.Count; i++)
			{
				foreach (PauseSettingButton item in settingsTabs[i])
				{
					item.gameObject.SetActive(i == index);
				}
			}
		}
	}

	private void ArrangeTabButtons()
	{
		tabButtonsContainer.gameObject.SetActive(value: true);
		float x = tabButtonsContainer.GetComponent<RectTransform>().sizeDelta.x;
		float num = 0f;
		for (int i = 0; i < tabButtons.Count; i++)
		{
			SettingsTabButton settingsTabButton = tabButtons[i];
			settingsTabButton.rectTransform.LocalMoveX(num);
			num += settingsTabButton.rectTransform.sizeDelta.x + 4f;
		}
		float num2 = (x - (num - 4f)) / 2f;
		foreach (SettingsTabButton tabButton in tabButtons)
		{
			tabButton.rectTransform.LocalMoveX(tabButton.rectTransform.localPosition.x + num2);
		}
	}
}
