using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI
{
	public class PropertiesPanel : ADOBase
	{
		[Header("UI")]
		public GridLayoutGroup layout;

		public ScrollRect scrollRect;

		public RectTransform content;

		[Header("Runtime")]
		public Dictionary<string, Property> properties = new Dictionary<string, Property>();

		[NonSerialized]
		public LevelEventType levelEventType;

		public InspectorPanel inspectorPanel;

		private static readonly List<string> dontRenderKeys = new List<string>
		{
			"selectTarget"
		};

		public void Init(InspectorPanel panel, LevelEventInfo levelEventInfo)
		{
			inspectorPanel = panel;
			Dictionary<string, PropertyInfo> propertiesInfo = levelEventInfo.propertiesInfo;
			Dictionary<string, PropertyInfo>.KeyCollection keys = propertiesInfo.Keys;
			int num = 0;
			foreach (string propertyKey in keys)
			{
				if (!dontRenderKeys.Contains(propertyKey))
				{
					PropertyInfo propertyInfo = propertiesInfo[propertyKey];
					if (propertyInfo.controlType != ControlType.Hidden && (!propertyInfo.pro || Application.isEditor))
					{
						GameObject original = null;
						List<string> list = new List<string>();
						switch (propertyInfo.type)
						{
						case PropertyType.File:
							original = ADOBase.gc.prefab_controlBrowse;
							break;
						case PropertyType.Int:
						case PropertyType.Float:
						case PropertyType.String:
							original = ADOBase.gc.prefab_controlText;
							if (propertyInfo.name == "component")
							{
								original = ADOBase.gc.prefab_controlToggle;
								Type parentType3 = typeof(ffxPlusBase);
								Type parentType2 = typeof(ffxBase);
								foreach (Type item in from t in Assembly.GetExecutingAssembly().GetTypes()
									where (!t.IsSubclassOf(parentType3)) ? t.IsSubclassOf(parentType2) : true
									select t)
								{
									list.Add(item.ToString());
								}
							}
							break;
						case PropertyType.LongString:
							original = ADOBase.gc.prefab_controlLongText;
							break;
						case PropertyType.Enum:
						{
							Type enumType = propertyInfo.enumType;
							original = ADOBase.gc.prefab_controlToggle;
							string[] names = Enum.GetNames(enumType);
							foreach (string text in names)
							{
								if ((enumType != typeof(Ease) || (text != "Unset" && text != "INTERNAL_Zero" && text != "INTERNAL_Custom")) && (levelEventInfo.type != LevelEventType.CameraSettings || !(text == "LastPosition")))
								{
									list.Add(text);
								}
							}
							break;
						}
						case PropertyType.Color:
							original = ADOBase.gc.prefab_controlColor;
							break;
						case PropertyType.Bool:
							original = ADOBase.gc.prefab_controlToggle;
							break;
						case PropertyType.Vector2:
							original = ADOBase.gc.prefab_controlVector2;
							break;
						case PropertyType.Tile:
							original = ADOBase.gc.prefab_controlTile;
							break;
						case PropertyType.Export:
							if (!SteamIntegration.Instance.initialized)
							{
								continue;
							}
							original = ADOBase.gc.prefab_controlExport;
							break;
						case PropertyType.Rating:
							original = ADOBase.gc.prefab_controlRating;
							break;
						case PropertyType.List:
							original = ADOBase.gc.prefab_controlList;
							break;
						}
						GameObject gameObject = UnityEngine.Object.Instantiate(ADOBase.gc.prefab_property);
						gameObject.transform.SetParent(content, worldPositionStays: false);
						Property property = gameObject.GetComponent<Property>();
						property.gameObject.name = propertyKey;
						property.key = propertyKey;
						property.info = propertyInfo;
						GameObject gameObject2 = UnityEngine.Object.Instantiate(original);
						gameObject2.GetComponent<RectTransform>().SetParent(property.controlContainer, worldPositionStays: false);
						property.control = gameObject2.GetComponent<PropertyControl>();
						property.control.propertyInfo = propertyInfo;
						property.control.propertiesPanel = this;
						if (propertyInfo.type == PropertyType.Enum || propertyInfo.name == "component")
						{
							((PropertyControl_Toggle)property.control).EnumSetup(propertyInfo.enumTypeString, list, propertyInfo.type == PropertyType.Enum);
						}
						else if (propertyInfo.type == PropertyType.List && propertyKey == "decorations")
						{
							ADOBase.editor.decorationsListContent = gameObject2.GetComponent<ScrollRect>().content;
							scrollRect.content = ADOBase.editor.decorationsListContent;
							scrollRect.vertical = false;
							scrollRect.viewport = gameObject2.GetComponent<ScrollRect>().viewport;
							RectTransform component = scrollRect.verticalScrollbar.transform.parent.GetComponent<RectTransform>();
							component.offsetMin = component.offsetMin.WithY(60f);
							component.offsetMax = component.offsetMax.WithY(-50f);
							ADOBase.editor.propertyControlList = (PropertyControl_List)property.control;
							((PropertyControl_List)property.control).parentReferenceRT = GetComponent<RectTransform>();
						}
						else if (propertyInfo.type == PropertyType.String && !string.IsNullOrEmpty(propertyInfo.placeholder))
						{
							((property.control as PropertyControl_Text).inputField.placeholder as Text).text = propertyInfo.placeholder;
						}
						string key = "editor." + property.key + ".help";
						bool exists;
						string helpString = RDString.GetWithCheck(key, out exists);
						if (exists)
						{
							Button helpButton = property.helpButton;
							helpButton.gameObject.SetActive(value: true);
							string buttonText = RDString.GetWithCheck("editor." + property.key + ".help.buttonText", out exists);
							string buttonURL = RDString.GetWithCheck("editor." + property.key + ".help.buttonURL", out exists);
							helpButton.onClick.AddListener(delegate
							{
								ADOBase.editor.ShowPropertyHelp(show: true, helpButton.transform, helpString, buttonText, buttonURL);
							});
						}
						property.control.Setup(addListener: true);
						if (property.info.hasRandomValue)
						{
							string randValueKey = property.info.randValueKey;
							property.control.randomControl.propertyInfo = levelEventInfo.propertiesInfo[randValueKey];
							property.control.randomControl.propertiesPanel = this;
							property.control.randomControl.Setup(addListener: true);
							Button randomButton = property.randomButton;
							randomButton.gameObject.SetActive(value: true);
							randomButton.onClick.AddListener(delegate
							{
								string randModeKey = property.info.randModeKey;
								int num2 = ((int)inspectorPanel.selectedEvent[randModeKey] + 1) % 3;
								inspectorPanel.selectedEvent[randModeKey] = (RandomMode)num2;
								property.control.SetRandomLayout();
							});
						}
						if (property.info.canBeDisabled)
						{
							property.enabledCheckmark.transform.parent.gameObject.SetActive(value: true);
							property.enabledButton.gameObject.SetActive(value: true);
							property.enabledButton.onClick.AddListener(delegate
							{
								using (new SaveStateScope(ADOBase.editor))
								{
									bool flag = !inspectorPanel.selectedEvent.disabled[propertyKey];
									inspectorPanel.selectedEvent.disabled[propertyKey] = flag;
									property.offText.SetActive(flag);
									property.enabledCheckmark.SetActive(!flag);
									property.control.gameObject.SetActive(!flag);
									property.enabledButton.GetComponent<RectTransform>().offsetMin = new Vector2(0f, flag ? 0f : property.controlContainer.rect.height);
								}
							});
						}
						num++;
						properties.Add(propertyInfo.name, property);
					}
				}
			}
			VerticalLayoutGroup component2 = content.GetComponent<VerticalLayoutGroup>();
			float y = (69f + component2.spacing) * (float)num;
			content.sizeDelta = content.sizeDelta.WithY(y);
		}

		public void SetProperties(LevelEvent levelEvent, bool checkIfEnabled = true)
		{
			foreach (string key in levelEvent.data.Keys)
			{
				if (properties.ContainsKey(key))
				{
					PropertyControl control = properties[key].control;
					if (!(control == null))
					{
						PropertyType type = control.propertyInfo.type;
						if (type != PropertyType.Export && type != PropertyType.List)
						{
							switch (type)
							{
							case PropertyType.Vector2:
								control.text = ((Vector2)levelEvent[key]).ToString("f6");
								if (control.propertyInfo.hasRandomValue)
								{
									control.randomControl.text = ((Vector2)levelEvent[control.propertyInfo.randValueKey]).ToString("f6");
									control.SetRandomLayout();
								}
								break;
							case PropertyType.Tile:
								(control as PropertyControl_Tile).tileValue = (levelEvent[key] as Tuple<int, TileRelativeTo>);
								break;
							default:
								control.text = levelEvent[key].ToString();
								if (control.propertyInfo.hasRandomValue)
								{
									control.randomControl.text = levelEvent[control.propertyInfo.randValueKey].ToString();
									control.SetRandomLayout();
								}
								break;
							}
							if (checkIfEnabled)
							{
								control.ToggleOthersEnabled();
							}
							if (control.propertyInfo.canBeDisabled)
							{
								bool flag = inspectorPanel.selectedEvent.disabled[key];
								properties[key].offText.SetActive(flag);
								properties[key].enabledCheckmark.SetActive(!flag);
								control.gameObject.SetActive(!flag);
								properties[key].enabledButton.gameObject.SetActive(value: true);
								properties[key].enabledButton.GetComponent<RectTransform>().offsetMin = new Vector2(0f, (!flag) ? 41 : 0);
							}
							properties[key].control?.OnSelectedEventChanged();
						}
					}
				}
			}
			if (levelEvent.info.propertiesInfo.ContainsKey("floor"))
			{
				int num = Mathf.Clamp(levelEvent.floor, 0, ADOBase.editor.floors.Count - 1);
				properties["floor"].control.text = num.ToString();
			}
		}
	}
}
