using DG.Tweening;
using RDTools;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI
{
	public class InspectorPanel : ADOBase
	{
		public const float padding_y = 8f;

		public const float height = 68f;

		private const float artistPrefabHeight = 35f;

		private const float artistPopupPaddingHeight = 12f;

		private const string addArtistKey = "editor.addArtist";

		[Header("UI")]
		public Text title;

		public RectTransform panels;

		public RectTransform tabs;

		public Button deleteEventButton;

		public Button disableEventButton;

		public Image disableEventButtonImage;

		public GameObject titleCanvas;

		public GameObject messageCanvas;

		public Text messageText;

		public bool showInspector;

		public bool permaHideInspector;

		public RectTransform artistPopup;

		public RectTransform artistContainer;

		public GameObject artistPrefab;

		public GameObject addArtist;

		public Text addArtistText;

		public ArtistUIDisclaimer artistUIDisclaimer;

		public float artistRectMaxHeight;

		public GameObject approvalLevelPrefab;

		[HideInInspector]
		public ApprovalLevelBadge approvalLevelBadge;

		public Sprite visibleIcon;

		public Sprite hiddenIcon;

		public Sprite powerIcon;

		[NonSerialized]
		[Header("Runtime")]
		public List<PropertiesPanel> panelsList;

		[NonSerialized]
		public LevelEventType selectedEventType;

		[NonSerialized]
		public LevelEvent selectedEvent;

		private Tween inspectorTween;

		public bool floorPanel;

		private ArtistData currentArtist;

		private string lastLevelPath;

		private bool showingPanel;

		[HideInInspector]
		public Action currentArtistDisclaimerAction;

		public EditorWebServices editorWebServices => ADOBase.editor.webServices;

		public void Init(Dictionary<string, LevelEventInfo> levelEventsInfo, bool floorPanel)
		{
			this.floorPanel = floorPanel;
			panelsList = new List<PropertiesPanel>();
			int num = 0;
			foreach (string key in levelEventsInfo.Keys)
			{
				LevelEventType levelEventType = RDUtils.ParseEnum(key, LevelEventType.None);
				GameObject gameObject = UnityEngine.Object.Instantiate(ADOBase.gc.prefab_propertiesPanel);
				gameObject.transform.SetParent(panels, worldPositionStays: false);
				gameObject.name = key;
				PropertiesPanel component = gameObject.GetComponent<PropertiesPanel>();
				component.levelEventType = levelEventType;
				if (levelEventType == LevelEventType.EditorComment)
				{
					panelsList.Insert(0, component);
				}
				else
				{
					panelsList.Add(component);
				}
				component.gameObject.SetActive(value: false);
				GameObject gameObject2 = UnityEngine.Object.Instantiate(ADOBase.gc.prefab_tab);
				gameObject2.transform.SetParent(tabs, worldPositionStays: false);
				if (levelEventType == LevelEventType.EditorComment)
				{
					gameObject2.transform.SetAsFirstSibling();
				}
				if (levelEventType == LevelEventType.Bookmark)
				{
					gameObject2.transform.SetAsLastSibling();
				}
				InspectorTab component2 = gameObject2.GetComponent<InspectorTab>();
				component2.Init(levelEventType, this);
				component2.GetComponent<RectTransform>().AnchorPosY(8f - 68f * (float)num);
				component2.SetSelected(selected: false);
				component.Init(this, levelEventsInfo[key]);
				num++;
			}
			deleteEventButton?.onClick.AddListener(delegate
			{
				ADOBase.editor.RemoveEventAtSelected(selectedEventType);
			});
			disableEventButton?.onClick.AddListener(delegate
			{
				if (selectedEvent != null)
				{
					if (selectedEventType == LevelEventType.AddDecoration || selectedEventType == LevelEventType.AddText)
					{
						ADOBase.editor.ShowEvent(selectedEvent, !selectedEvent.visible);
					}
					else
					{
						ADOBase.editor.EnableEvent(selectedEvent, !selectedEvent.active);
					}
				}
			});
		}

		public void Update()
		{
			Sprite sprite = (selectedEventType != LevelEventType.AddDecoration && selectedEventType != LevelEventType.AddText) ? powerIcon : (selectedEvent.visible ? visibleIcon : hiddenIcon);
			if (disableEventButtonImage != null)
			{
				disableEventButtonImage.sprite = sprite;
			}
		}

		public void ShowPanel(LevelEventType eventType, int eventIndex = 0)
		{
			showingPanel = true;
			using (new SaveStateScope(ADOBase.editor))
			{
				PropertiesPanel propertiesPanel = null;
				foreach (PropertiesPanel panels2 in panelsList)
				{
					if (panels2.levelEventType == eventType)
					{
						panels2.gameObject.SetActive(value: true);
						titleCanvas.SetActive(value: true);
						propertiesPanel = panels2;
					}
					else
					{
						panels2.gameObject.SetActive(value: false);
					}
				}
				if (eventType != 0)
				{
					title.text = RDString.Get("editor." + eventType.ToString());
					LevelEvent levelEvent = null;
					int num = 1;
					switch (eventType)
					{
					case LevelEventType.SongSettings:
						levelEvent = ADOBase.editor.levelData.songSettings;
						break;
					case LevelEventType.LevelSettings:
						levelEvent = ADOBase.editor.levelData.levelSettings;
						break;
					case LevelEventType.TrackSettings:
						levelEvent = ADOBase.editor.levelData.trackSettings;
						break;
					case LevelEventType.BackgroundSettings:
						levelEvent = ADOBase.editor.levelData.backgroundSettings;
						break;
					case LevelEventType.CameraSettings:
						levelEvent = ADOBase.editor.levelData.cameraSettings;
						break;
					case LevelEventType.MiscSettings:
						levelEvent = ADOBase.editor.levelData.miscSettings;
						break;
					case LevelEventType.DecorationSettings:
						levelEvent = ADOBase.editor.levelData.decorationSettings;
						break;
					case LevelEventType.AddDecoration:
					case LevelEventType.AddText:
						if (ADOBase.editor.selectedDecorations.Count > 1)
						{
							eventType = LevelEventType.None;
							levelEvent = null;
							ModifyMessageText(RDString.Get("editor.dialog.multipleDecorationSelected"), 0f, enable: true);
							titleCanvas.SetActive(value: false);
							ShowPanel(LevelEventType.None);
						}
						else if (ADOBase.editor.selectedDecorations.Count == 1)
						{
							levelEvent = ADOBase.editor.selectedDecorations[0];
							ModifyMessageText("", enable: false);
						}
						break;
					default:
					{
						ModifyMessageText("", enable: false);
						List<LevelEvent> selectedFloorEvents = ADOBase.editor.GetSelectedFloorEvents(eventType);
						num = selectedFloorEvents.Count;
						if (eventIndex > selectedFloorEvents.Count - 1)
						{
							RDBaseDll.printesw("undo is trying to break down, fix!! or dont");
						}
						else
						{
							levelEvent = selectedFloorEvents[eventIndex];
						}
						break;
					}
					}
					if (propertiesPanel == null)
					{
						RDBaseDll.printesw("selectedPanel should not be null!! >:(");
					}
					else if (levelEvent != null)
					{
						selectedEvent = levelEvent;
						selectedEventType = levelEvent.eventType;
						if (selectedEventType == LevelEventType.KillPlayer)
						{
							ModifyMessageText(RDString.Get("editor.dialog.usingKillPlayer"), -35f, enable: true);
						}
						propertiesPanel.SetProperties(levelEvent);
						foreach (RectTransform tab in tabs)
						{
							InspectorTab component = tab.gameObject.GetComponent<InspectorTab>();
							if (!(component == null))
							{
								if (eventType == component.levelEventType)
								{
									component.SetSelected(selected: true);
									component.eventIndex = eventIndex;
									if (component.cycleButtons != null)
									{
										component.cycleButtons.text.text = $"{eventIndex + 1}/{num}";
									}
								}
								else
								{
									component.SetSelected(selected: false);
								}
							}
						}
					}
				}
				else
				{
					selectedEventType = LevelEventType.None;
				}
				showingPanel = false;
			}
		}

		public void ShowPanelOfEvent(LevelEvent evnt)
		{
			int seqID = ADOBase.editor.selectedFloors[0].seqID;
			int num = 0;
			foreach (LevelEvent @event in ADOBase.editor.events)
			{
				if (seqID == @event.floor && @event.eventType == evnt.eventType)
				{
					if (@event == evnt)
					{
						break;
					}
					num++;
				}
			}
			ShowPanel(evnt.eventType, num);
		}

		public InspectorTab GetTabForEventType(LevelEventType eventType)
		{
			foreach (Transform tab in tabs)
			{
				InspectorTab component = tab.gameObject.GetComponent<InspectorTab>();
				if (component.levelEventType == eventType)
				{
					return component;
				}
			}
			return null;
		}

		public int EventNumOfTab(LevelEventType eventType)
		{
			InspectorTab tabForEventType = GetTabForEventType(eventType);
			if (!(tabForEventType == null))
			{
				return tabForEventType.eventIndex;
			}
			return 0;
		}

		public InspectorTab GetSelectedEventTab()
		{
			if (selectedEventType != 0)
			{
				return GetTabForEventType(selectedEventType);
			}
			return null;
		}

		public void CycleSelectedEventTab(bool next)
		{
			InspectorTab selectedEventTab = GetSelectedEventTab();
			if (selectedEventTab != null && selectedEventTab.cycleButtons != null)
			{
				selectedEventTab.cycleButtons.CycleEvent(next);
			}
		}

		public void CycleTabs(bool next)
		{
			if (selectedEventType == LevelEventType.None)
			{
				return;
			}
			InspectorTab[] componentsInChildren = tabs.GetComponentsInChildren<InspectorTab>(includeInactive: false);
			int num = 0;
			while (true)
			{
				if (num < componentsInChildren.Length)
				{
					if (componentsInChildren[num].levelEventType == selectedEventType)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			if (next)
			{
				if (num == componentsInChildren.Length - 1)
				{
					ShowPanel(componentsInChildren[0].levelEventType);
				}
				else
				{
					ShowPanel(componentsInChildren[num + 1].levelEventType);
				}
			}
			else if (num == 0)
			{
				ShowPanel(componentsInChildren[componentsInChildren.Length - 1].levelEventType);
			}
			else
			{
				ShowPanel(componentsInChildren[num - 1].levelEventType);
			}
		}

		public void HideAllInspectorTabs()
		{
			foreach (Transform tab in tabs)
			{
				tab.gameObject.SetActive(value: false);
			}
			titleCanvas.SetActive(value: false);
			if (ADOBase.editor.selectedFloors.Count > 0)
			{
				ModifyMessageText(RDString.Get("editor.dialog.noEventsOnTile"), enable: true);
			}
			else
			{
				ModifyMessageText(RDString.Get("editor.dialog.noSelectedDecoration"), enable: true);
			}
			ShowPanel(LevelEventType.None);
		}

		public void ShowTabsForFloor(int floorID)
		{
			List<LevelEventType> list = new List<LevelEventType>();
			foreach (LevelEvent @event in scnEditor.instance.events)
			{
				if (@event.floor == floorID)
				{
					list.Add(@event.eventType);
				}
			}
			titleCanvas.SetActive(list.Count > 0);
			ModifyMessageText("", enable: false);
			if (list.Count == 0)
			{
				ShowPanel(LevelEventType.None);
				ModifyMessageText(RDString.Get("editor.dialog.noEventsOnTile"), 0f, enable: true);
				ADOBase.editor.DeselectAllDecorations();
			}
			else
			{
				LevelEventType levelEventType = LevelEventType.None;
				foreach (LevelEventType value in Enum.GetValues(typeof(LevelEventType)))
				{
					if (value != 0)
					{
						foreach (LevelEventType item2 in list)
						{
							if (item2 == value)
							{
								levelEventType = item2;
								break;
							}
						}
						if (levelEventType != 0)
						{
							break;
						}
					}
				}
				selectedEventType = LevelEventType.None;
				if (levelEventType != LevelEventType.AddDecoration && levelEventType != LevelEventType.AddText)
				{
					ADOBase.editor.DeselectAllDecorations();
				}
				ShowPanel(levelEventType);
				ShowInspector(show: true);
			}
			List<string> list2 = new List<string>();
			foreach (LevelEventType item3 in list)
			{
				string item = item3.ToString();
				if (!list2.Contains(item))
				{
					list2.Add(item);
				}
			}
			int num = -1;
			float num2 = 68f;
			if (list2.Count > 7)
			{
				num2 = 476f / (float)list2.Count;
			}
			foreach (Transform tab in tabs)
			{
				bool flag = list2.Contains(tab.name);
				tab.gameObject.SetActive(flag);
				if (flag)
				{
					num++;
					list2.Remove(tab.name);
				}
				float y = 8f - num2 * (float)num;
				tab.GetComponent<RectTransform>().SetAnchorPosY(y);
			}
		}

		public void ShowInspector(bool show, bool forceAction = false)
		{
			if (!permaHideInspector || forceAction)
			{
				if (forceAction)
				{
					permaHideInspector = !show;
				}
				showInspector = show;
				RectTransform component = GetComponent<RectTransform>();
				float num = show ? 0f : component.sizeDelta.x;
				if (!floorPanel)
				{
					num *= -1f;
				}
				if (inspectorTween != null && inspectorTween.active)
				{
					inspectorTween.Kill();
				}
				inspectorTween = component.DOAnchorPosX(num, ADOBase.editor.UIPanelEaseDur).SetUpdate(isIndependentUpdate: true).SetEase(ADOBase.editor.UIPanelEaseMode);
			}
		}

		public void ToggleArtistPopup(string search, float yPos, PropertyControl_Text artistPropertyControl)
		{
			if (EditorWebServices.artists == null)
			{
				return;
			}
			bool flag = lastLevelPath != ADOBase.levelPath;
			lastLevelPath = ADOBase.levelPath;
			if (approvalLevelBadge == null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(approvalLevelPrefab, artistPropertyControl.transform);
				approvalLevelBadge = gameObject.GetComponent<ApprovalLevelBadge>();
				approvalLevelBadge.UpdateUI(ApprovalLevel.Pending, onlyColor: true);
			}
			if (!showingPanel)
			{
				ADOBase.editor.levelData.artist = search;
			}
			string text = search.Trim();
			if (text == "")
			{
				artistPopup.gameObject.SetActive(value: false);
				approvalLevelBadge.UpdateUI(ApprovalLevel.Pending, onlyColor: true);
				return;
			}
			search = text.ToLower();
			List<ArtistData> list = (from item in EditorWebServices.artists.AsParallel()
				where item.approvalLevel != 0 && item.name.ToLower().Contains(search)
				select item).ToList();
			foreach (Transform item in artistContainer)
			{
				if (item.gameObject != addArtist)
				{
					UnityEngine.Object.Destroy(item.gameObject);
				}
			}
			int num = 0;
			currentArtist = null;
			addArtist.SetActive(value: false);
			currentArtistDisclaimerAction = null;
			if (list != null && list.Count > 0)
			{
				bool flag2 = false;
				foreach (ArtistData artist in list)
				{
					if (!flag2 && artist.name.ToLower() == search)
					{
						approvalLevelBadge.UpdateUI(artist.approvalLevel, onlyColor: true);
						currentArtist = artist;
						flag2 = true;
						currentArtistDisclaimerAction = delegate
						{
							artistUIDisclaimer.SetData(artist, artistPropertyControl, onlyCheckingMode: true);
						};
					}
					if (!flag)
					{
						ArtistUISettings component = UnityEngine.Object.Instantiate(artistPrefab, artistContainer).GetComponent<ArtistUISettings>();
						component.SetData(artist.name, artist.approvalLevel);
						component.button.onClick.AddListener(delegate
						{
							HideArtistDropdown();
							artistUIDisclaimer.SetData(artist, artistPropertyControl, onlyCheckingMode: false);
						});
					}
				}
				num = list.Count;
				if (!flag2)
				{
					num++;
					approvalLevelBadge.UpdateUI(ApprovalLevel.Pending, onlyColor: true);
				}
				if (!flag)
				{
					addArtist.SetActive(!flag2);
				}
			}
			else if (!flag)
			{
				addArtist.SetActive(value: true);
				num++;
			}
			if (!flag)
			{
				if (addArtist.activeSelf)
				{
					addArtistText.text = RDString.Get("editor.addArtist", new Dictionary<string, object>
					{
						{
							"ArtistName",
							text
						}
					});
					addArtist.transform.SetAsLastSibling();
				}
				float num2 = (float)num * 35f + 12f;
				artistPopup.position = artistPopup.position.WithY(yPos);
				artistPopup.anchoredPosition = artistPopup.anchoredPosition.WithY(artistPopup.anchoredPosition.y - 44f);
				artistContainer.sizeDelta = new Vector2(artistContainer.sizeDelta.x, num2);
				float y = Mathf.Min(num2, artistRectMaxHeight);
				artistPopup.sizeDelta = new Vector2(artistPopup.sizeDelta.x, y);
				artistPopup.gameObject.SetActive(value: true);
				ADOBase.editor.ShowPopupBlocker(delegate
				{
					if (!artistUIDisclaimer.gameObject.activeSelf)
					{
						if (currentArtist != null)
						{
							artistUIDisclaimer.SetData(currentArtist, artistPropertyControl, onlyCheckingMode: false);
						}
						else
						{
							artistUIDisclaimer.gameObject.SetActive(value: false);
							HideArtistDropdown();
						}
					}
				});
			}
		}

		public void HideArtistDropdown()
		{
			artistPopup.gameObject.SetActive(value: false);
		}

		public void NewArtist()
		{
			HideArtistDropdown();
		}

		private void LateUpdate()
		{
			if (disableEventButton != null && selectedEvent != null)
			{
				Color color = new Color(214f / 255f, 0.2352941f, 0.2039216f, 1f);
				Color color2 = new Color(0.8f, 0.8f, 0.8f, 1f);
				ColorBlock colors = disableEventButton.colors;
				colors.normalColor = (selectedEvent.active ? color2 : color);
				colors.highlightedColor = colors.normalColor * 1.1f;
				colors.pressedColor = colors.normalColor;
				disableEventButton.colors = colors;
			}
		}

		private void ModifyMessageText(string text, bool enable)
		{
			if (!(messageCanvas == null))
			{
				messageCanvas.SetActive(enable);
				messageText.text = text;
			}
		}

		private void ModifyMessageText(string text, float yPos, bool enable)
		{
			if (!(messageCanvas == null))
			{
				messageCanvas.SetActive(enable);
				messageText.text = text;
				messageText.rectTransform.anchoredPosition = messageText.rectTransform.anchoredPosition.WithY(yPos);
			}
		}
	}
}
