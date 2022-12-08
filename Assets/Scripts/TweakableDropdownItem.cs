using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TweakableDropdownItem : Button, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	[Header("UI")]
	public Image background;

	public Image checkmark;

	public Text text;

	[Header("Runtime")]
	public TweakableDropdown dropdown;

	public bool isChecked;

	public bool isVisible;

	public bool isArrowSelected;

	public bool isReadonly;

	public string value;

	public int index;

	public bool localizeValue;

	public string localizedValue
	{
		get
		{
			if (!localizeValue)
			{
				return value;
			}
			return RDString.GetEnumValue(dropdown.enumTypeString, value);
		}
	}

	private Color contextualColor
	{
		get
		{
			Color color = isArrowSelected ? dropdown.selectedItemBGColor : dropdown.normalItemBGColor;
			if (IsHighlighted())
			{
				color += dropdown.hoveredItemBGColor - dropdown.normalItemBGColor;
			}
			return color;
		}
	}

	public void ResetVisuals()
	{
		isVisible = true;
		text.text = localizedValue;
		background.color = contextualColor;
	}

	public void SetChecked(bool check)
	{
		checkmark.gameObject.SetActive(check);
		isChecked = check;
	}

	public void SetVisible(bool visible)
	{
		base.gameObject.SetActive(visible);
		isVisible = visible;
	}

	public void OnArrowSelect(bool selected)
	{
		if (!isReadonly && (selected ^ isArrowSelected))
		{
			bool flag = dropdown.arrowSelectedDropdownItems.Contains(this);
			if (isArrowSelected = (selected && !flag))
			{
				dropdown.arrowSelectedDropdownItems.Add(this);
			}
			else if (flag)
			{
				dropdown.arrowSelectedDropdownItems.Remove(this);
			}
			background.color = contextualColor;
		}
	}

	public void OnSearch(string searchText)
	{
		string localizedValue = this.localizedValue;
		int num = SanitizeSearchString(localizedValue).IndexOf(SanitizeSearchString(searchText));
		if (num >= 0)
		{
			text.text = localizedValue.Insert(num + searchText.Length, "</color>").Insert(num, "<color=" + dropdown.searchedItemTextColor.ToHex() + ">");
			SetVisible(visible: true);
			return;
		}
		SetVisible(visible: false);
		text.text = localizedValue;
		isArrowSelected = false;
		background.color = contextualColor;
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		background.color = contextualColor;
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		background.color = contextualColor;
	}

	public void OnClick()
	{
		if (!isReadonly)
		{
			dropdown.SelectItem(this);
		}
	}

	private string SanitizeSearchString(string str)
	{
		if (!dropdown.useStrictSearch)
		{
			str = Regex.Replace(str, "-", " ");
		}
		return str.ToLower();
	}
}
