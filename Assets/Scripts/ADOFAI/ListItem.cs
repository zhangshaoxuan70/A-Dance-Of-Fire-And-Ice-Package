using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ADOFAI
{
	public class ListItem : ADOBase
	{
		[Header("Components")]
		[SerializeField]
		private Text itemName;

		[SerializeField]
		private Text itemTagText;

		public RectTransform rt;

		public Transform selectionBackground;

		public Transform selectionHighlight;

		public Button eyeButton;

		[Header("Image Components")]
		public Image itemImageType;

		public Image itemDisplayedIndicator;

		public Button itemDisplayedButton;

		public Image itemTagImage;

		[Header("Sprites")]
		public Sprite decorationSprite;

		public Sprite textSprite;

		public Sprite eyeOpenSprite;

		public Sprite eyeClosedSprite;

		[Header("Floor")]
		public Image floorRect;

		public Button floorButton;

		public Text floorIDText;

		[HideInInspector]
		public LevelEvent sourceLevelEvent;

		public AdofaiEventTrigger eventTrigger;

		private PropertyControl_List propertyControlList;

		private float displayedButtonColorMultiplier;

		private int floorID;

		private void Awake()
		{
			floorButton.onClick.AddListener(delegate
			{
				SelectFloorButton();
			});
			eyeButton.onClick.AddListener(delegate
			{
				ButtonPower();
			});
			eventTrigger.onPointerClick = delegate
			{
				SelectItemButton();
			};
			eventTrigger.onPointerDown = delegate(PointerEventData e)
			{
				propertyControlList.PointerDown(e, this);
			};
			eventTrigger.onBeginDrag = delegate(PointerEventData e)
			{
				propertyControlList.BeginDrag(e, this);
			};
			eventTrigger.onPointerEnter = delegate(PointerEventData e)
			{
				propertyControlList.PointerEnter(e, this);
			};
			eventTrigger.onPointerExit = delegate(PointerEventData e)
			{
				propertyControlList.PointerExit(e, this);
			};
			displayedButtonColorMultiplier = itemDisplayedButton.colors.colorMultiplier;
		}

		private void LateUpdate()
		{
			itemDisplayedIndicator.sprite = (sourceLevelEvent.visible ? eyeOpenSprite : eyeClosedSprite);
			bool visible = sourceLevelEvent.visible;
			itemDisplayedIndicator.sprite = (visible ? eyeOpenSprite : eyeClosedSprite);
			ColorBlock colors = itemDisplayedButton.colors;
			colors.colorMultiplier = displayedButtonColorMultiplier * (sourceLevelEvent.forceHide ? 0.5f : 1f);
			itemDisplayedButton.colors = colors;
		}

		public ListItem SetDecoration(scrDecoration dec)
		{
			propertyControlList = ADOBase.editor.propertyControlList;
			LevelEvent levelEvent = sourceLevelEvent = dec.sourceLevelEvent;
			floorID = levelEvent.floor;
			ValidateFloorID();
			itemName.text = dec.decorationName;
			base.transform.name = ((dec.transform.name == "") ? "" : dec.decorationName);
			itemTagText.text = dec.decorationTag;
			ShowItemTag(propertyControlList.ShowingTagsOnItems);
			bool flag = (DecPlacementType)levelEvent["relativeTo"] == DecPlacementType.Tile;
			floorRect.gameObject.SetActive(flag);
			floorIDText.gameObject.SetActive(flag);
			if (flag)
			{
				floorIDText.text = floorID.ToString();
			}
			LevelEventType eventType = levelEvent.eventType;
			Image image = itemImageType;
			object sprite;
			switch (eventType)
			{
			default:
				sprite = null;
				break;
			case LevelEventType.AddText:
				sprite = textSprite;
				break;
			case LevelEventType.AddDecoration:
				sprite = decorationSprite;
				break;
			}
			image.sprite = (Sprite)sprite;
			bool selectedState = ADOBase.editor.selectedDecorations.Contains(dec.sourceLevelEvent);
			SetSelectedState(selectedState);
			bool visible = levelEvent.visible;
			itemDisplayedIndicator.sprite = (visible ? eyeOpenSprite : eyeClosedSprite);
			return this;
		}

		public void ShowHighlight(bool show)
		{
			selectionHighlight.gameObject.SetActive(show);
		}

		public void ShowItemTag(bool show)
		{
			itemName.enabled = !show;
			itemImageType.enabled = !show;
			itemTagText.enabled = show;
			itemTagImage.enabled = show;
		}

		public void SetSelectedState(bool selected)
		{
			ShowSelectionBackground(selected);
			if (selected)
			{
				foreach (RectTransform tab in ADOBase.editor.levelEventsPanel.tabs)
				{
					InspectorTab component = tab.gameObject.GetComponent<InspectorTab>();
					if (!(component == null))
					{
						if (sourceLevelEvent.eventType == component.levelEventType)
						{
							component.gameObject.SetActive(value: true);
							component.GetComponent<RectTransform>().SetAnchorPosY(8f);
						}
						else
						{
							component.gameObject.SetActive(value: false);
						}
					}
				}
			}
		}

		private void ShowSelectionBackground(bool selected)
		{
			selectionBackground.gameObject.SetActive(selected);
			itemName.color = (selected ? Color.black : Color.white);
			itemImageType.color = (selected ? Color.black : Color.white);
			itemTagImage.color = (selected ? Color.black : Color.white);
			itemTagText.color = (selected ? Color.black : Color.white);
			itemDisplayedIndicator.color = (selected ? Color.black : Color.white);
			floorIDText.color = (selected ? Color.white : Color.black);
			floorRect.color = (selected ? Color.black : Color.white);
		}

		private void ValidateFloorID()
		{
			floorID = Mathf.Clamp(floorID, 0, ADOBase.editor.floors.Count - 1);
		}

		private void SelectItemButton()
		{
			if (RDInput.holdingShift)
			{
				propertyControlList.SelectItemsInRange(sourceLevelEvent);
				return;
			}
			int decorationIndex = scrDecorationManager.GetDecorationIndex(sourceLevelEvent);
			ADOBase.editor.SelectDecoration(decorationIndex);
		}

		private void ButtonPower()
		{
			ADOBase.editor.ShowEvent(sourceLevelEvent, !sourceLevelEvent.visible);
		}

		private void SelectFloorButton()
		{
			ValidateFloorID();
			scrFloor floorToSelect = ADOBase.editor.floors[floorID];
			ADOBase.editor.SelectFloor(floorToSelect);
		}
	}
}
