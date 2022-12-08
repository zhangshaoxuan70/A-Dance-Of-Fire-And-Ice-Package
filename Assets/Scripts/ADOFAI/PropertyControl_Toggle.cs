using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI
{
	public class PropertyControl_Toggle : PropertyControl
	{
		public GameObject enumButtonPrefab;

		public GameObject dropdownPrefab;

		public TweakableDropdown dropdown;

		public Image dropdownArrow;

		public Sprite selectedToggleBackground;

		public Sprite unselectedToggleBackground;

		public List<string> enumValList = new List<string>();

		public Dictionary<string, Button> buttons;

		public string selected;

		public bool settingText;

		private bool useButtons
		{
			get
			{
				if (enumValList.Count >= 3)
				{
					return propertyInfo.type == PropertyType.Tile;
				}
				return true;
			}
		}

		public override string text
		{
			get
			{
				return selected;
			}
			set
			{
				if (useButtons)
				{
					foreach (string key in buttons.Keys)
					{
						SetButtonEnabled(buttons[key], key == value);
					}
					return;
				}
				settingText = true;
				List<TweakableDropdownItem> items = dropdown.items;
				int index = enumValList.IndexOf(value);
				TweakableDropdownItem targetItem = items[index];
				dropdown.SelectItem(targetItem);
				settingText = false;
			}
		}

		public override void EnumSetup(string enumTypeString, List<string> enumVals, bool localize = true)
		{
			enumValList = enumVals;
			dropdown = UnityEngine.Object.Instantiate(dropdownPrefab).GetComponent<TweakableDropdown>();
			((RectTransform)dropdown.transform).SetParent(base.transform, worldPositionStays: false);
			dropdown.gameObject.SetActive(value: true);
			dropdownPrefab.SetActive(value: false);
			dropdownArrow = dropdown.dropdownButton.image;
			dropdown.enumTypeString = enumTypeString;
			dropdown.localizeEnumStrings = localize;
			if (useButtons)
			{
				base.gameObject.AddComponent<HorizontalLayoutGroup>();
				dropdown.gameObject.SetActive(value: false);
				buttons = new Dictionary<string, Button>();
				float num = 0f;
				int num2 = 250 / enumValList.Count;
				foreach (string enumVar in enumValList)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(enumButtonPrefab, base.transform);
					gameObject.GetComponentInChildren<Text>().text = (localize ? RDString.GetEnumValue(enumTypeString, enumVar) : enumVar);
					RectTransform component = gameObject.GetComponent<RectTransform>();
					component.AnchorPosX(num);
					component.sizeDelta = new Vector2(num2, component.sizeDelta.y);
					num += (float)num2;
					Button component2 = gameObject.GetComponent<Button>();
					buttons.Add(enumVar, component2);
					component2.onClick.AddListener(delegate
					{
						SelectVar(enumVar);
					});
				}
				return;
			}
			dropdown.itemValues.Clear();
			dropdown.itemValues.AddRange(enumValList);
			dropdown.ReloadList();
			for (int i = 0; i < dropdown.itemValues.Count; i++)
			{
				string value = enumVals[i];
				dropdown.items[i].value = value;
			}
			dropdown.onValueChanged = delegate(TweakableDropdownItem selectedItem)
			{
				SelectVar(enumVals[selectedItem.index]);
			};
		}

		public override void Setup(bool addListener)
		{
		}

		private void Update()
		{
			if (dropdownArrow != null)
			{
				dropdownArrow.color = Color.white.WithAlpha(dropdown.interactable ? 1f : 0.3f);
			}
		}

		public void SelectVar(string var)
		{
			if (!settingText)
			{
				using (new SaveStateScope(ADOBase.editor))
				{
					if (buttons != null && buttons.Count > 0)
					{
						foreach (string key in buttons.Keys)
						{
							SetButtonEnabled(buttons[key], key == var);
						}
					}
					LevelEvent selectedEvent = propertiesPanel.inspectorPanel.selectedEvent;
					selected = var;
					Type enumType = propertyInfo.enumType;
					if (propertyInfo.type == PropertyType.Tile)
					{
						Tuple<int, TileRelativeTo> tuple = selectedEvent[propertyInfo.name] as Tuple<int, TileRelativeTo>;
						selectedEvent[propertyInfo.name] = new Tuple<int, TileRelativeTo>(tuple.Item1, (TileRelativeTo)Enum.Parse(enumType, var));
					}
					else if (propertyInfo.name == "component")
					{
						selectedEvent[propertyInfo.name] = var;
					}
					else
					{
						selectedEvent[propertyInfo.name] = Enum.Parse(enumType, var);
					}
					ToggleOthersEnabled();
					if (selectedEvent.eventType == LevelEventType.BackgroundSettings)
					{
						ADOBase.customLevel.SetBackground();
					}
					else if (selectedEvent.eventType == LevelEventType.AddDecoration || selectedEvent.eventType == LevelEventType.AddText)
					{
						ADOBase.editor.UpdateDecorationObject(selectedEvent);
					}
					if (propertyInfo.affectsFloors)
					{
						ADOBase.editor.ApplyEventsToFloors();
					}
				}
			}
		}

		public void SetButtonEnabled(Button button, bool enabled)
		{
			button.image.sprite = (enabled ? selectedToggleBackground : unselectedToggleBackground);
			button.GetComponentInChildren<Text>().color = (enabled ? Color.black : Color.white);
		}

		public override void OnSelectedEventChanged()
		{
			if (useButtons || dropdown.arrowSelectedDropdownItems.Count == 0)
			{
				return;
			}
			for (int i = 0; i < dropdown.arrowSelectedDropdownItems.Count; i++)
			{
				TweakableDropdownItem tweakableDropdownItem = dropdown.arrowSelectedDropdownItems[i];
				if (!(tweakableDropdownItem == dropdown.selectedItem))
				{
					tweakableDropdownItem.OnArrowSelect(selected: false);
				}
			}
		}
	}
}
