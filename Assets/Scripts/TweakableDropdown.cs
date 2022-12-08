using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TweakableDropdown : Selectable
{
	[Header("Prefabs")]
	public GameObject dropdownItemPrefab;

	[Header("UI")]
	public InputField inputField;

	public Button dropdownButton;

	public ScrollRect dropdownScroll;

	public TweakableDropdownScrollDragHandler dropdownScrollDragHandler;

	[Header("UI Options")]
	public bool rememberLastScrollPosition;

	public bool rememberLastSearchText;

	public bool useStrictSearch;

	public bool suggestiveList;

	public int visibleItemsCount;

	public float itemWidth;

	public float itemHeight;

	public float colorAnimDuration;

	public Color normalItemBGColor;

	public Color hoveredItemBGColor;

	public Color selectedItemBGColor;

	public Color normalItemTextColor;

	public Color searchedItemTextColor;

	[Header("Scroll Options")]
	public float secondsForHold;

	public float autoScrollInterval;

	[Header("Localization Keys")]
	public string keyNoneSelected;

	public string keyNoItemInList;

	public string keyNoItemFound;

	[Header("Values")]
	public List<TweakableDropdownItem> items;

	public List<string> itemValues = new List<string>();

	[Header("Runtime")]
	public TweakableDropdownItem selectedItem;

	public TweakableDropdownItem noItem;

	public TweakableDropdownItem searchNotFoundItem;

	public List<TweakableDropdownItem> arrowSelectedDropdownItems = new List<TweakableDropdownItem>();

	public List<string> customItemValues = new List<string>();

	public int arrowSelectIndex;

	public bool localizeEnumStrings;

	public string enumTypeString;

	public Action<TweakableDropdownItem> onValueChanged;

	public Predicate<string> suggestiveListItemAppendCriteria;

	private string lastSearchText;

	private bool resetSearchText = true;

	private bool lastInputFieldFocused;

	private TweakableDropdownItem lastArrowSelectedItem;

	private bool skipArrowSelectionScrolling;

	private bool skippedArrowSelectionColoring;

	private int lastCaretPosition;

	private bool isSearching;

	private bool hasPressedEscape;

	private float holdTimer;

	private float autoscrollTimer;

	private static GameObject lastSelectedGO;

	private static TweakableDropdown lastSelectedDropdown;

	public List<TweakableDropdownItem> enabledItems
	{
		get
		{
			if (!isSearching)
			{
				return items;
			}
			return (from i in items
				where i.isVisible
				select i).ToList();
		}
	}

	public bool isShowingList
	{
		get;
		private set;
	}

	public bool releasedDragging
	{
		get;
		set;
	} = true;


	public static TweakableDropdown currentSelectedDropdown
	{
		get
		{
			GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
			if ((bool)currentSelectedGameObject)
			{
				if (lastSelectedGO == currentSelectedGameObject && (bool)lastSelectedDropdown)
				{
					return lastSelectedDropdown;
				}
				lastSelectedGO = currentSelectedGameObject;
				lastSelectedDropdown = RDEditorUtils.GetComponentInAllParents<TweakableDropdown>(currentSelectedGameObject);
				return lastSelectedDropdown;
			}
			return null;
		}
	}

	public void Setup()
	{
		dropdownScroll.gameObject.SetActive(value: false);
		RectTransform obj = (RectTransform)dropdownScroll.transform;
		if (!rememberLastScrollPosition)
		{
			dropdownScroll.verticalNormalizedPosition = 0f;
		}
		UpdateInputFieldText();
		visibleItemsCount = Math.Max(1, visibleItemsCount);
		obj.sizeDelta = new Vector2(obj.sizeDelta.x, itemHeight * (float)visibleItemsCount);
		int count = enabledItems.Count;
		count = ((count == visibleItemsCount) ? (count - 1) : count);
		dropdownScroll.content.sizeDelta = new Vector2(0f, itemHeight * (float)count);
		UpdateCheckmark();
	}

	public void ShowList()
	{
		if (isShowingList)
		{
			return;
		}
		if (!inputField.isFocused)
		{
			inputField.ActivateInputField();
		}
		inputField.text = (rememberLastSearchText ? lastSearchText : "");
		inputField.textComponent.alignment = TextAnchor.MiddleLeft;
		if (isSearching && lastSearchText.IsNullOrEmpty())
		{
			SearchItem("");
		}
		dropdownScroll.gameObject.SetActive(value: true);
		if (rememberLastScrollPosition)
		{
			TweakableDropdownItem tweakableDropdownItem = lastArrowSelectedItem;
			if ((object)tweakableDropdownItem == null || !tweakableDropdownItem.isVisible)
			{
				arrowSelectIndex = (selectedItem?.index ?? 0);
			}
		}
		UpdateArrowSelection();
		isShowingList = true;
	}

	public void HideList()
	{
		if (isShowingList)
		{
			if (inputField.isFocused)
			{
				inputField.DeactivateInputField();
			}
			if (!hasPressedEscape)
			{
				lastSearchText = ((rememberLastSearchText && !resetSearchText) ? inputField.text : "");
			}
			else
			{
				hasPressedEscape = false;
			}
			UpdateInputFieldText();
			dropdownScroll.gameObject.SetActive(value: false);
			resetSearchText = false;
			isShowingList = false;
		}
	}

	public void ToggleList(bool customFactor)
	{
		if (customFactor)
		{
			ShowList();
		}
		else
		{
			HideList();
		}
	}

	public void OnDropdownButtonClick()
	{
		ToggleList(!isShowingList);
	}

	public void ReloadList()
	{
		RectTransform content = dropdownScroll.content;
		noItem = (searchNotFoundItem = null);
		foreach (Transform item in content)
		{
			if (!(item.name == "TweakableDropdownItem"))
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
		}
		items.Clear();
		List<string> list = new List<string>(itemValues)
		{
			RDString.Get(keyNoItemInList),
			RDString.Get(keyNoItemFound)
		};
		for (int i = 0; i < list.Count; i++)
		{
			bool flag = i >= list.Count - 2;
			GameObject gameObject = UnityEngine.Object.Instantiate(dropdownItemPrefab, content, worldPositionStays: false);
			TweakableDropdownItem component = gameObject.GetComponent<TweakableDropdownItem>();
			component.value = list[i];
			component.isReadonly = flag;
			if (!flag)
			{
				component.index = i;
			}
			if (flag)
			{
				if (noItem == null)
				{
					noItem = component;
				}
				else if (searchNotFoundItem == null)
				{
					searchNotFoundItem = component;
				}
			}
			else
			{
				if (localizeEnumStrings && !customItemValues.Contains(list[i]))
				{
					component.localizeValue = true;
				}
				items.Add(component);
			}
			component.ResetVisuals();
			component.SetChecked(check: false);
			RectTransform rectTransform = (RectTransform)gameObject.transform;
			rectTransform.sizeDelta = new Vector2(0f, itemHeight);
			rectTransform.localPosition = new Vector3(0f, GetItemVerticalPos(component.index), rectTransform.localPosition.z);
			gameObject.SetActive(!flag || list.Count == 1);
		}
	}

	private void UpdateInputFieldText()
	{
		if ((bool)selectedItem)
		{
			inputField.text = selectedItem.localizedValue;
			inputField.textComponent.alignment = TextAnchor.MiddleLeft;
		}
		else
		{
			inputField.text = RDString.Get(keyNoneSelected);
			inputField.textComponent.alignment = TextAnchor.MiddleCenter;
		}
	}

	public void ResetItemPosition()
	{
		for (int i = 0; i < items.Count; i++)
		{
			RectTransform rectTransform = (RectTransform)items[i].transform;
			rectTransform.localPosition = new Vector3(0f, GetItemVerticalPos(i), rectTransform.localPosition.z);
			items[i].text.text = items[i].localizedValue;
			items[i].SetVisible(visible: true);
		}
		ResetArrowSelection();
		noItem.SetVisible(items.Count == 0);
		dropdownScroll.content.sizeDelta = new Vector2(0f, itemHeight * (float)items.Count);
	}

	public void UpdateItemPosition()
	{
		List<TweakableDropdownItem> enabledItems = this.enabledItems;
		for (int i = 0; i < enabledItems.Count; i++)
		{
			RectTransform rectTransform = (RectTransform)enabledItems[i].transform;
			rectTransform.localPosition = new Vector3(0f, GetItemVerticalPos(i), rectTransform.localPosition.z);
		}
		ResetArrowSelection();
		((items.Count == 0) ? noItem : searchNotFoundItem).SetVisible(enabledItems.Count == 0);
		dropdownScroll.content.sizeDelta = new Vector2(0f, itemHeight * (float)enabledItems.Count);
	}

	public void ResetArrowSelection()
	{
		arrowSelectIndex = Math.Max(0, (rememberLastScrollPosition && (bool)lastArrowSelectedItem) ? enabledItems.IndexOf(lastArrowSelectedItem) : 0);
		UpdateArrowSelection();
	}

	public void UpdateArrowSelection()
	{
		List<TweakableDropdownItem> enabledItems = this.enabledItems;
		skippedArrowSelectionColoring = false;
		if ((bool)lastArrowSelectedItem && !enabledItems.Contains(lastArrowSelectedItem))
		{
			arrowSelectIndex = -1;
			lastArrowSelectedItem = null;
		}
		if (arrowSelectIndex >= 0 && enabledItems.Count != 0)
		{
			if ((bool)lastArrowSelectedItem && lastArrowSelectedItem != enabledItems[arrowSelectIndex])
			{
				lastArrowSelectedItem.OnArrowSelect(selected: false);
			}
			lastArrowSelectedItem = enabledItems[arrowSelectIndex];
			lastArrowSelectedItem.OnArrowSelect(selected: true);
			if (!skipArrowSelectionScrolling)
			{
				if (enabledItems.Count == visibleItemsCount)
				{
					dropdownScroll.verticalNormalizedPosition = 1f;
					return;
				}
				int num = enabledItems.Count - visibleItemsCount;
				float num2 = 0.5f / (float)num;
				float num3 = (float)(enabledItems.Count - arrowSelectIndex - visibleItemsCount) / (float)num;
				if (!(dropdownScroll.verticalNormalizedPosition > num3) || !(dropdownScroll.verticalNormalizedPosition - num3 + num2 < (float)visibleItemsCount / (float)num))
				{
					if (dropdownScroll.verticalNormalizedPosition < num3 + num2)
					{
						dropdownScroll.verticalNormalizedPosition = num3;
						return;
					}
					num3 = (float)Math.Max(0, enabledItems.Count - arrowSelectIndex - 1) / (float)num;
					dropdownScroll.verticalNormalizedPosition = num3;
				}
			}
			else
			{
				skipArrowSelectionScrolling = false;
			}
		}
		else if ((bool)lastArrowSelectedItem)
		{
			lastArrowSelectedItem.OnArrowSelect(selected: false);
		}
	}

	public void UpdateCheckmark()
	{
		int num = (selectedItem != null) ? selectedItem.index : (-1);
		for (int i = 0; i < items.Count; i++)
		{
			items[i].SetChecked(i == num);
		}
	}

	public bool AddItem(string value, bool tryRevertSelection = true)
	{
		if (itemValues.Contains(value))
		{
			return false;
		}
		HideList();
		if (!customItemValues.Contains(value))
		{
			customItemValues.Add(value);
		}
		itemValues.Add(value);
		bool flag = false;
		int index = 0;
		string selectedItemValue = null;
		if (tryRevertSelection && (bool)selectedItem)
		{
			flag = true;
			index = selectedItem.index;
			selectedItemValue = selectedItem.value;
		}
		ReloadList();
		Setup();
		if (tryRevertSelection && flag)
		{
			TweakableDropdownItem tweakableDropdownItem = items[index];
			if (tweakableDropdownItem.value != selectedItemValue)
			{
				tweakableDropdownItem = (items.Find((TweakableDropdownItem e) => e.value == selectedItemValue) ?? tweakableDropdownItem);
			}
			SelectItem(tweakableDropdownItem);
		}
		else if (!tryRevertSelection)
		{
			List<TweakableDropdownItem> list = items;
			int index2 = list.Count - 1;
			SelectItem(list[index2]);
		}
		return true;
	}

	public bool AddItems(IEnumerable<string> values, bool tryRevertSelection = true)
	{
		List<string> list = values.Distinct().Except(itemValues).ToList();
		list.Union(itemValues.Except(list));
		if (list.Count == 0)
		{
			return false;
		}
		string value = list.Pop();
		for (int i = 0; i < list.Count; i++)
		{
			string item = list[i];
			if (!customItemValues.Contains(item))
			{
				customItemValues.Add(item);
			}
			itemValues.Add(item);
		}
		AddItem(value, tryRevertSelection);
		return true;
	}

	public bool RemoveItem(string value)
	{
		if (!itemValues.Contains(value))
		{
			return false;
		}
		HideList();
		if (!customItemValues.Contains(value))
		{
			customItemValues.Add(value);
		}
		itemValues.Remove(value);
		bool flag = false;
		int num = 0;
		string selectedItemValue = null;
		if ((bool)selectedItem)
		{
			flag = true;
			num = selectedItem.index;
			selectedItemValue = selectedItem.value;
		}
		ReloadList();
		Setup();
		if (flag)
		{
			if (num >= items.Count)
			{
				num = items.Count - 1;
			}
			TweakableDropdownItem tweakableDropdownItem = items[num];
			if (tweakableDropdownItem.value != selectedItemValue)
			{
				tweakableDropdownItem = items.Find((TweakableDropdownItem e) => e.value == selectedItemValue);
			}
			SelectItem(tweakableDropdownItem);
		}
		return true;
	}

	public bool RemoveItems(IEnumerable<string> values)
	{
		List<string> list = values.Distinct().Intersect(itemValues).ToList();
		if (list.Count == 0)
		{
			return false;
		}
		HideList();
		string value = list.Pop();
		for (int i = 0; i < list.Count; i++)
		{
			string item = list[i];
			if (!customItemValues.Contains(item))
			{
				customItemValues.Add(item);
			}
			itemValues.Remove(item);
		}
		RemoveItem(value);
		return true;
	}

	public void SearchItem(string searchText)
	{
		if ((bool)lastArrowSelectedItem)
		{
			lastArrowSelectedItem.OnArrowSelect(selected: false);
		}
		arrowSelectIndex = 0;
		lastArrowSelectedItem = null;
		if (searchText.Length == 0)
		{
			isSearching = false;
			ResetItemPosition();
		}
		else
		{
			isSearching = true;
			foreach (TweakableDropdownItem item in items)
			{
				item.OnSearch(searchText);
			}
			UpdateItemPosition();
		}
	}

	public void SelectItem(TweakableDropdownItem targetItem)
	{
		if ((bool)lastArrowSelectedItem)
		{
			lastArrowSelectedItem.OnArrowSelect(selected: false);
		}
		bool num = selectedItem != targetItem;
		lastArrowSelectedItem = (selectedItem = targetItem);
		resetSearchText = true;
		skipArrowSelectionScrolling = true;
		SearchItem(lastSearchText = "");
		targetItem?.OnArrowSelect(selected: true);
		arrowSelectIndex = (targetItem?.index ?? 0);
		UpdateCheckmark();
		HideList();
		if (num && onValueChanged != null)
		{
			onValueChanged(targetItem);
		}
		UpdateInputFieldText();
	}

	protected override void Awake()
	{
		if (Application.isPlaying)
		{
			base.Awake();
			Setup();
			ReloadList();
		}
	}

	private void Update()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (isShowingList)
		{
			if (inputField.isFocused)
			{
				if (UnityEngine.Input.GetKey(KeyCode.UpArrow) || UnityEngine.Input.GetKey(KeyCode.DownArrow))
				{
					inputField.caretPosition = lastCaretPosition;
				}
				if (UnityEngine.Input.GetKeyDown(KeyCode.Tab) && !skippedArrowSelectionColoring && arrowSelectIndex != -1)
				{
					inputField.text = this.enabledItems[arrowSelectIndex].localizedValue;
					inputField.caretPosition = inputField.text.Length;
				}
				if (lastSearchText != inputField.text)
				{
					lastSearchText = inputField.text;
					SearchItem(lastSearchText);
				}
				lastCaretPosition = inputField.caretPosition;
			}
			List<TweakableDropdownItem> enabledItems = this.enabledItems;
			int num = 0;
			bool flag = false;
			bool flag2 = RDEditorUtils.ControlIsPressed();
			bool keyDown = UnityEngine.Input.GetKeyDown(KeyCode.UpArrow);
			bool keyDown2 = UnityEngine.Input.GetKeyDown(KeyCode.DownArrow);
			bool keyDown3 = UnityEngine.Input.GetKeyDown(KeyCode.PageUp);
			bool keyDown4 = UnityEngine.Input.GetKeyDown(KeyCode.PageDown);
			bool num2 = keyDown | keyDown3;
			bool flag3 = keyDown2 | keyDown4;
			if (num2 | flag3)
			{
				if (keyDown)
				{
					if (flag2)
					{
						arrowSelectIndex = 0;
						flag = true;
					}
					else
					{
						num--;
					}
				}
				if (keyDown2)
				{
					if (flag2)
					{
						arrowSelectIndex = enabledItems.Count - 1;
						flag = true;
					}
					else
					{
						num++;
					}
				}
				if (keyDown3)
				{
					num -= visibleItemsCount;
				}
				if (keyDown4)
				{
					num += visibleItemsCount;
				}
			}
			else
			{
				bool key = UnityEngine.Input.GetKey(KeyCode.UpArrow);
				bool key2 = UnityEngine.Input.GetKey(KeyCode.DownArrow);
				bool key3 = UnityEngine.Input.GetKey(KeyCode.PageUp);
				bool key4 = UnityEngine.Input.GetKey(KeyCode.PageDown);
				bool num3 = key | key3;
				bool flag4 = key2 | key4;
				if (num3 | flag4)
				{
					holdTimer += Time.unscaledDeltaTime;
				}
				else
				{
					holdTimer = 0f;
					autoscrollTimer = 0f;
				}
				if (holdTimer > secondsForHold)
				{
					autoscrollTimer += Time.unscaledDeltaTime;
					if (autoscrollTimer > autoScrollInterval)
					{
						if (key)
						{
							num--;
						}
						if (key2)
						{
							num++;
						}
						if (key3)
						{
							num -= visibleItemsCount;
						}
						if (key4)
						{
							num += visibleItemsCount;
						}
						autoscrollTimer = 0f;
					}
				}
			}
			if ((num != 0) | flag)
			{
				arrowSelectIndex = Math.Min(enabledItems.Count - 1, Math.Max(0, arrowSelectIndex + num));
				UpdateArrowSelection();
			}
			if (currentSelectedDropdown != this || !EventSystem.current.currentSelectedGameObject)
			{
				GameObject[] array = RDEditorUtils.ObjectsAtPointer();
				if ((array.Length == 0 || !array.Any((GameObject o) => RDEditorUtils.GetComponentInAllParents<TweakableDropdown>(o) == this)) && !dropdownScrollDragHandler.isDragging)
				{
					HideList();
				}
				else if (releasedDragging)
				{
					releasedDragging = false;
					inputField.ActivateInputField();
				}
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Return))
			{
				if (enabledItems.Count > arrowSelectIndex && arrowSelectIndex != -1)
				{
					SelectItem(enabledItems[arrowSelectIndex]);
					return;
				}
				if (enabledItems.Count == 0 && suggestiveList && (suggestiveListItemAppendCriteria?.Invoke(lastSearchText) ?? true))
				{
					AddItem(lastSearchText, tryRevertSelection: false);
					return;
				}
				HideList();
			}
			if (hasPressedEscape = UnityEngine.Input.GetKeyDown(KeyCode.Escape))
			{
				HideList();
			}
		}
		else if (lastInputFieldFocused != inputField.isFocused)
		{
			lastInputFieldFocused = inputField.isFocused;
			ToggleList(lastInputFieldFocused && currentSelectedDropdown == this);
		}
	}

	private float GetItemVerticalPos(int index)
	{
		return (0f - itemHeight) * (float)index;
	}
}
