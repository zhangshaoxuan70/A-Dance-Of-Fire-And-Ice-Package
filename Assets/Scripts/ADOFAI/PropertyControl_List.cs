using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ADOFAI
{
	public class PropertyControl_List : PropertyControl
	{
		public static string stringNoItems;

		public static string stringNoText;

		public static string stringNoImage;

		public static string stringNoTag;

		public static string stringSearch;

		public static string stringNoTextFormatted;

		public static string stringNoImageFormatted;

		public static string stringNoTagFormatted;

		public static bool isDraggingItems;

		public static bool isDraggingOverItemMiddle;

		[NonSerialized]
		[Header("References")]
		public RectTransform parentReferenceRT;

		public RectTransform contentRT;

		public Button addDecorationButton;

		public Button addTextButton;

		public Button removeButton;

		public Button showTagsToggleButton;

		public Button toggleAllButton;

		public InputField searchField;

		public Text searchFieldPlaceholder;

		public Text noItemsText;

		public Button clearSearchFieldButton;

		public RectTransform viewportRect;

		public RectTransform draggingIndicatorBar;

		public RectTransform multiSelectIcon;

		public Image toggleAllIndicator;

		public Action<LevelEvent> OnItemSelected;

		public Action OnAllItemsDeselected;

		public Sprite eyeOpenSprite;

		public Sprite eyeClosedSprite;

		[NonSerialized]
		public int lastSelectedIndex;

		private List<ListItem> shownItems = new List<ListItem>();

		private List<LevelEvent> filteredDecorations = new List<LevelEvent>();

		private static float itemHeight;

		private const float buttonSectionHeight = 10f;

		private Vector2 cachedMousePos;

		private RectTransform cachedDraggedItemTransform;

		private List<scrDecoration> cachedDecorations = new List<scrDecoration>();

		private LevelEvent cachedDraggedDecoration;

		private ListItem cachedHighlightedItem;

		private float itemSegmentSize;

		private int distanceUnits;

		private int itemDistanceUnits;

		private bool mouseIsInsideContent;

		private float cacheContentPosY;

		private float cacheViewportSizeY;

		private List<ListItem> toRemove = new List<ListItem>();

		private bool showTags;

		private bool shownTagsToggle;

		private bool forceHideAll;

		private bool shouldUpdate;

		private bool shouldUpdateForce;

		public ListItemPool listItemPool => ListItemPool.instance;

		[HideInInspector]
		public bool ShowingTagsOnItems
		{
			get;
			private set;
		}

		private List<LevelEvent> selectedDecorations => ADOBase.editor.selectedDecorations;

		private List<scrDecoration> allDecorations => scrDecorationManager.instance.allDecorations;

		private bool shiftHolding => RDInput.holdingShift;

		private bool ctrlHolding => RDInput.holdingControl;

		private bool altHolding => RDInput.holdingAlt;

		private void Awake()
		{
			itemHeight = ADOBase.gc.prefab_controlListItem.GetComponent<RectTransform>().rect.height;
			stringNoItems = RDString.Get("editor.noItems");
			stringNoText = RDString.Get("editor.noText");
			stringNoImage = RDString.Get("editor.noImage");
			stringNoTag = RDString.Get("editor.noTag");
			stringSearch = RDString.Get("editor.search");
			stringNoTextFormatted = $"<i>({stringNoText.ToLower()})</i>";
			stringNoImageFormatted = $"<i>({stringNoImage.ToLower()})</i>";
			stringNoTagFormatted = $"<i>({stringNoTag.ToLower()})</i>";
		}

		private void Start()
		{
			addDecorationButton.onClick.AddListener(delegate
			{
				ADOBase.editor.DeselectAllDecorations();
			});
			addDecorationButton.onClick.AddListener(delegate
			{
				ADOBase.editor.AddDecoration(LevelEventType.AddDecoration);
			});
			addTextButton.onClick.AddListener(delegate
			{
				ADOBase.editor.DeselectAllDecorations();
			});
			addTextButton.onClick.AddListener(delegate
			{
				ADOBase.editor.AddDecoration(LevelEventType.AddText);
			});
			removeButton.onClick.AddListener(delegate
			{
				ADOBase.editor.DeleteMultiSelectionDecorations();
			});
			clearSearchFieldButton.onClick.AddListener(delegate
			{
				searchField.text = "";
			});
			showTagsToggleButton.onClick.AddListener(delegate
			{
				shownTagsToggle = !shownTagsToggle;
			});
			toggleAllButton.onClick.AddListener(delegate
			{
				ToggleHideAll();
			});
			searchField.onValueChanged.AddListener(delegate(string s)
			{
				FilterSearchResults(s);
			});
			searchFieldPlaceholder.text = stringSearch;
			RectTransform obj = rectTransform.parent as RectTransform;
			obj.offsetMax = obj.offsetMax.WithY(0f);
			multiSelectIcon.transform.SetParent(parentReferenceRT);
			noItemsText.text = stringNoItems;
		}

		private void Update()
		{
			showTags = (altHolding || shownTagsToggle);
			ShowTagsOnItems(showTags);
			bool flag = !ADOBase.editor.SelectionDecorationIsEmpty();
			if (flag)
			{
				if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow))
				{
					SelectPreviousItem();
				}
				else if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow))
				{
					SelectNextItem();
				}
				if (isDraggingItems)
				{
					if (Input.GetMouseButton(0))
					{
						SetDraggedPosition(UnityEngine.Input.mousePosition);
					}
					if (Input.GetMouseButtonUp(0))
					{
						EndDrag(UnityEngine.Input.mousePosition);
					}
				}
			}
			removeButton.interactable = flag;
			noItemsText.gameObject.SetActive(allDecorations.Count < 1);
		}

		private void LateUpdate()
		{
			rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, parentReferenceRT.rect.size.y - 10f);
			toggleAllIndicator.sprite = (forceHideAll ? eyeClosedSprite : eyeOpenSprite);
			if (contentRT.anchoredPosition.y != cacheContentPosY || viewportRect.rect.size.y != cacheViewportSizeY)
			{
				UpdateItemList();
				cacheContentPosY = contentRT.anchoredPosition.y;
				cacheViewportSizeY = viewportRect.rect.size.y;
			}
			if (shouldUpdate)
			{
				if (shouldUpdateForce)
				{
					ApplyUpdateList(forceRefreshAll: true);
				}
				else
				{
					ApplyUpdateList();
				}
			}
		}

		public void PointerDown(PointerEventData eventData, ListItem itemRef)
		{
			if (!(itemRef == null))
			{
				cachedDraggedDecoration = itemRef.sourceLevelEvent;
				cachedDraggedItemTransform = itemRef.rt;
				cachedMousePos = eventData.position;
				multiSelectIcon.position = cachedMousePos;
			}
		}

		public void BeginDrag(PointerEventData eventData, ListItem itemRef)
		{
			if (selectedDecorations.Count > 0 && selectedDecorations.Contains(itemRef.sourceLevelEvent))
			{
				foreach (LevelEvent selectedDecoration in selectedDecorations)
				{
					cachedDecorations.Add(scrDecorationManager.GetDecoration(selectedDecoration));
				}
				itemSegmentSize = itemHeight / 4f * ADOBase.editor.levelEditorCanvas.scaleFactor;
				isDraggingItems = true;
			}
		}

		public void SetDraggedPosition(Vector3 eventData)
		{
			float x = viewportRect.transform.position.x;
			float num = (viewportRect.rect.size.x + x) * ADOBase.editor.levelEditorCanvas.scaleFactor;
			float y = viewportRect.transform.position.y;
			float num2 = y - viewportRect.rect.size.y * ADOBase.editor.levelEditorCanvas.scaleFactor;
			mouseIsInsideContent = (eventData.x >= x && eventData.x <= num && eventData.y >= num2 && eventData.y <= y);
			bool active = mouseIsInsideContent && isDraggingItems;
			multiSelectIcon.gameObject.SetActive(active);
			multiSelectIcon.position = eventData;
			Vector3 position = cachedDraggedItemTransform.position;
			float num3 = eventData.y - position.y;
			if (Mathf.Abs(num3) > 1000000f)
			{
				num3 = 0f;
				printe($"endDistance: {num3}, itemHeight: {itemHeight}, segmentSize: {itemSegmentSize}, cachedItemStartPos: {position}");
			}
			distanceUnits = Mathf.RoundToInt(num3 / itemSegmentSize);
			if (Math.Abs(distanceUnits) % 4 == 0)
			{
				isDraggingOverItemMiddle = false;
				if (cachedHighlightedItem != null)
				{
					cachedHighlightedItem.ShowHighlight(show: false);
				}
				draggingIndicatorBar.gameObject.SetActive(active);
				draggingIndicatorBar.position = new Vector2(draggingIndicatorBar.position.x, position.y + itemSegmentSize * (float)distanceUnits);
			}
			else
			{
				isDraggingOverItemMiddle = true;
				draggingIndicatorBar.gameObject.SetActive(value: false);
			}
		}

		public void EndDrag(Vector3 eventData)
		{
			multiSelectIcon.gameObject.SetActive(value: false);
			draggingIndicatorBar.gameObject.SetActive(value: false);
			using (new SaveStateScope(ADOBase.editor))
			{
				if (mouseIsInsideContent && !isDraggingOverItemMiddle && cachedHighlightedItem != null)
				{
					int num = 0;
					int value = 0;
					scrDecoration decoration = scrDecorationManager.GetDecoration(cachedHighlightedItem.sourceLevelEvent);
					bool flag = cachedDecorations.Contains(decoration);
					if (flag)
					{
						value = scrDecorationManager.GetDecorationIndex(cachedHighlightedItem.sourceLevelEvent);
					}
					else
					{
						Vector3 position = cachedHighlightedItem.rt.position;
						num = ((!Mathf.Approximately(draggingIndicatorBar.position.y, position.y)) ? 1 : 0);
					}
					for (int i = 0; i <= cachedDecorations.Count - 2; i++)
					{
						for (int j = 0; j <= cachedDecorations.Count - 2; j++)
						{
							if (scrDecorationManager.GetDecorationIndex(cachedDecorations[j].sourceLevelEvent) > scrDecorationManager.GetDecorationIndex(cachedDecorations[j + 1].sourceLevelEvent))
							{
								scrDecoration value2 = cachedDecorations[j + 1];
								cachedDecorations[j + 1] = cachedDecorations[j];
								cachedDecorations[j] = value2;
							}
						}
					}
					foreach (scrDecoration cachedDecoration in cachedDecorations)
					{
						cachedDecoration.transform.SetParent(null);
						scrDecorationManager.instance.allDecorations.Remove(cachedDecoration);
						ADOBase.editor.levelData.decorations.Remove(cachedDecoration.sourceLevelEvent);
					}
					if (!flag)
					{
						value = scrDecorationManager.GetDecorationIndex(cachedHighlightedItem.sourceLevelEvent) + num;
					}
					value = Math.Clamp(value, 0, allDecorations.Count);
					int count = cachedDecorations.Count;
					for (int k = 0; k < count; k++)
					{
						cachedDecorations[k].transform.SetParent(scrDecorationManager.instance.transform);
						cachedDecorations[k].transform.SetSiblingIndex(value);
						scrDecorationManager.instance.allDecorations.Insert(value, cachedDecorations[k]);
						ADOBase.editor.levelData.decorations.Insert(value, cachedDecorations[k].sourceLevelEvent);
						lastSelectedIndex = value;
						value++;
					}
				}
				ClearDragCache();
				OnDecorationUpdate();
			}
		}

		public void PointerEnter(PointerEventData eventData, ListItem item)
		{
			if (cachedHighlightedItem != null)
			{
				cachedHighlightedItem.ShowHighlight(show: false);
			}
			if (isDraggingItems && !selectedDecorations.Contains(item.sourceLevelEvent))
			{
				cachedHighlightedItem = item;
			}
		}

		public void PointerExit(PointerEventData eventData, ListItem item)
		{
			item.ShowHighlight(show: false);
		}

		public void ClearDragCache()
		{
			cachedDecorations.Clear();
			cachedHighlightedItem = null;
			isDraggingItems = false;
		}

		public ListItem SearchForVisibleItem(LevelEvent levelEvent)
		{
			foreach (ListItem shownItem in shownItems)
			{
				if (shownItem.sourceLevelEvent == levelEvent)
				{
					return shownItem;
				}
			}
			return null;
		}

		public void SelectItemsInRange(LevelEvent endRangeItem)
		{
			using (new SaveStateScope(ADOBase.editor))
			{
				int decorationIndex = scrDecorationManager.GetDecorationIndex(endRangeItem);
				if (decorationIndex == lastSelectedIndex && selectedDecorations.Count == 1)
				{
					printe("Selection is not multiple");
					ADOBase.editor.SelectDecoration(endRangeItem, jumpToDecoration: false, showPanel: false);
				}
				else
				{
					int num = decorationIndex;
					bool num2 = num >= lastSelectedIndex;
					int num3 = num2 ? lastSelectedIndex : num;
					int num4 = num2 ? num : lastSelectedIndex;
					for (int i = num3; i <= num4; i++)
					{
						if (filteredDecorations.Contains(scrDecorationManager.GetDecoration(i).sourceLevelEvent))
						{
							ADOBase.editor.SelectDecoration(i, jumpToDecoration: false, showPanel: true, ignoreDeselection: true, ignoreAdjustRect: true);
						}
					}
				}
			}
		}

		public void OnDecorationUpdate()
		{
			if (contentRT == null)
			{
				contentRT = ADOBase.editor.decorationsListContent;
			}
			ApplyHideAll();
			if (searchField != null)
			{
				FilterSearchResults(searchField.text, adjustRect: false);
			}
			SelectAllNewlyAddedDecorations();
			UpdateItemList(forceRefreshAll: true);
		}

		public void UpdateItemList(bool forceRefreshAll = false)
		{
			shouldUpdate = true;
			shouldUpdateForce = forceRefreshAll;
		}

		public void AdjustItemListScrollRect(LevelEvent levelEvent)
		{
			int num = 0;
			using (List<LevelEvent>.Enumerator enumerator = filteredDecorations.GetEnumerator())
			{
				while (enumerator.MoveNext() && enumerator.Current != levelEvent)
				{
					num++;
				}
			}
			AdjustItemListScrollRect(num);
		}

		public void AdjustItemListScrollRect(int decorationIndex)
		{
			contentRT.SizeDeltaY(itemHeight * (float)filteredDecorations.Count);
			if (!IsItemFullyVisible(decorationIndex))
			{
				float y = contentRT.anchoredPosition.y;
				float num = (float)decorationIndex * itemHeight * -1f + y;
				float height = viewportRect.rect.height;
				float num2 = (num > 0f) ? num : (num - itemHeight + height);
				float y2 = contentRT.anchoredPosition.y - num2;
				contentRT.SetAnchorPosY(y2);
			}
			UpdateItemList();
		}

		public void ClearCache()
		{
			ClearDragCache();
			filteredDecorations.Clear();
			shownItems.Clear();
		}

		private void ApplyUpdateList(bool forceRefreshAll = false)
		{
			if (filteredDecorations.Count <= 0)
			{
				return;
			}
			if (forceRefreshAll)
			{
				ClearShownItems();
			}
			else
			{
				toRemove.Clear();
				foreach (ListItem shownItem in shownItems)
				{
					if (!IsItemVisible(shownItem.rt))
					{
						toRemove.Add(shownItem);
						listItemPool.SendItemBackToPool(shownItem.gameObject);
					}
				}
				for (int i = 0; i < toRemove.Count; i++)
				{
					shownItems.Remove(toRemove[i]);
				}
			}
			contentRT.SizeDeltaY(itemHeight * (float)filteredDecorations.Count);
			int num = Mathf.RoundToInt(contentRT.anchoredPosition.y / itemHeight) - 1;
			int num2 = Mathf.RoundToInt(viewportRect.rect.height / itemHeight) + 2;
			for (int j = num; j < num + num2; j++)
			{
				if (j < 0)
				{
					continue;
				}
				if (j >= filteredDecorations.Count)
				{
					break;
				}
				scrDecoration decoration = scrDecorationManager.GetDecoration(filteredDecorations[j]);
				if (IsItemVisible(j) && decoration != null)
				{
					ListItem listItem = SearchForVisibleItem(decoration.sourceLevelEvent);
					Vector3 vector = new Vector3(0f, (float)j * itemHeight * -1f, 0f);
					if (listItem == null)
					{
						ListItem component = listItemPool.GetPooledItem(contentRT, vector).GetComponent<ListItem>();
						component.SetDecoration(decoration);
						shownItems.Add(component);
					}
					else
					{
						bool selectedState = ADOBase.editor.selectedDecorations.Contains(decoration.sourceLevelEvent);
						listItem.SetSelectedState(selectedState);
						listItem.rt.anchoredPosition = vector;
					}
				}
			}
			shouldUpdate = false;
			shouldUpdateForce = false;
		}

		private void SelectAllNewlyAddedDecorations()
		{
			if (!ADOBase.editor.SelectionDecorationIsEmpty())
			{
				foreach (LevelEvent selectedDecoration in this.selectedDecorations)
				{
					ADOBase.editor.SelectDecoration(selectedDecoration, jumpToDecoration: false, showPanel: true, ignoreDeselection: true);
				}
				if (OnItemSelected != null)
				{
					Action<LevelEvent> onItemSelected = OnItemSelected;
					List<LevelEvent> selectedDecorations = this.selectedDecorations;
					int index = selectedDecorations.Count - 1;
					onItemSelected(selectedDecorations[index]);
				}
			}
		}

		private bool IsItemVisible(int itemIndex)
		{
			float y = contentRT.anchoredPosition.y;
			float num = (float)itemIndex * itemHeight * -1f + y;
			float height = viewportRect.rect.height;
			if (num < itemHeight)
			{
				return num > 0f - height;
			}
			return false;
		}

		private bool IsItemVisible(RectTransform itemRT)
		{
			float y = contentRT.anchoredPosition.y;
			float num = itemRT.anchoredPosition.y + y;
			float height = viewportRect.rect.height;
			if (num < itemHeight)
			{
				return num > 0f - height;
			}
			return false;
		}

		private bool IsItemFullyVisible(int itemIndex)
		{
			float y = contentRT.anchoredPosition.y;
			float num = (float)itemIndex * itemHeight * -1f + y;
			float height = viewportRect.rect.height;
			if (num <= 0f)
			{
				return num - itemHeight > 0f - height;
			}
			return false;
		}

		private void ToggleHideAll()
		{
			forceHideAll = !forceHideAll;
			ApplyHideAll();
		}

		private void ApplyHideAll()
		{
			foreach (scrDecoration allDecoration in allDecorations)
			{
				ADOBase.editor.ForceHideEvent(allDecoration.sourceLevelEvent, forceHideAll);
			}
		}

		private void ShowTagsOnItems(bool show)
		{
			if (showTags != ShowingTagsOnItems)
			{
				foreach (ListItem shownItem in shownItems)
				{
					shownItem.ShowItemTag(show);
				}
				ShowingTagsOnItems = show;
				showTagsToggleButton.image.color = (show ? Color.white : Color.grey);
			}
		}

		private void SelectPreviousItem()
		{
			if (filteredDecorations.Count >= 1)
			{
				lastSelectedIndex = (lastSelectedIndex + filteredDecorations.Count - 1) % filteredDecorations.Count;
				ADOBase.editor.SelectDecoration(lastSelectedIndex, jumpToDecoration: true, showPanel: true, ctrlHolding);
			}
		}

		private void SelectNextItem()
		{
			if (filteredDecorations.Count >= 1)
			{
				lastSelectedIndex = (lastSelectedIndex + 1) % filteredDecorations.Count;
				ADOBase.editor.SelectDecoration(lastSelectedIndex, jumpToDecoration: true, showPanel: true, ctrlHolding);
			}
		}

		private void FilterSearchResults(string search, bool adjustRect = true)
		{
			ClearShownItems();
			filteredDecorations.Clear();
			if (string.IsNullOrEmpty(search))
			{
				clearSearchFieldButton.gameObject.SetActive(value: false);
				foreach (LevelEvent decoration2 in ADOBase.editor.decorations)
				{
					filteredDecorations.Add(decoration2);
				}
			}
			else
			{
				clearSearchFieldButton.gameObject.SetActive(value: true);
				string value = search.ToLower();
				foreach (LevelEvent decoration3 in ADOBase.editor.decorations)
				{
					scrDecoration decoration = scrDecorationManager.GetDecoration(decoration3);
					if (decoration != null)
					{
						string text = decoration.decorationName.ToLower();
						string text2 = decoration.decorationTag.ToLower();
						bool num = decoration.transform.name == "";
						bool flag = text2 == stringNoTagFormatted;
						if ((!num && text.Contains(value)) || (!flag && text2.Contains(value)))
						{
							filteredDecorations.Add(decoration3);
						}
						else
						{
							filteredDecorations.Remove(decoration3);
						}
					}
				}
				printe("Filtered: " + filteredDecorations.Count.ToString());
			}
			if (adjustRect)
			{
				AdjustItemListScrollRect(0);
			}
		}

		private void ClearShownItems()
		{
			while (contentRT.childCount > 0)
			{
				listItemPool.SendItemBackToPool(contentRT.GetChild(0).gameObject);
			}
			shownItems.Clear();
		}
	}
}
