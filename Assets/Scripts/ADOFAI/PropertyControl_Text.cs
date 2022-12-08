using RDTools;
using System;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI
{
	public class PropertyControl_Text : PropertyControl
	{
		public InputField inputField;

		public Text unit;

		public InputField.EndEditEvent onEndEdit => inputField.onEndEdit;

		public override string text
		{
			get
			{
				return Validate();
			}
			set
			{
				inputField.text = value;
			}
		}

		public override void SetRandomLayout()
		{
			RandomMode randomMode = (RandomMode)propertiesPanel.inspectorPanel.selectedEvent[propertyInfo.randModeKey];
			if (randomControl != null && rectTransform != null)
			{
				if (randomMode != 0)
				{
					randomControl.gameObject.SetActive(value: true);
					rectTransform.anchorMax = rectTransform.anchorMax.WithX(0.48f);
				}
				else
				{
					randomControl.gameObject.SetActive(value: false);
					rectTransform.anchorMax = rectTransform.anchorMax.WithX(1f);
				}
			}
		}

		public string Validate()
		{
			if (propertyInfo == null)
			{
				return inputField.text;
			}
			if (propertyInfo.type == PropertyType.Float)
			{
				float result = 1f;
				if (float.TryParse(inputField.text, out result))
				{
					result = propertyInfo.Validate(result);
				}
				else
				{
					DataTable dataTable = new DataTable();
					try
					{
						result = RDEditorUtils.DecodeFloat(dataTable.Compute(inputField.text, ""));
					}
					catch
					{
						RDBaseDll.printesw("[" + propertyInfo.name + "] Invalid float value: " + inputField.text);
						result = (float)propertyInfo.value_default;
					}
				}
				return result.ToString();
			}
			if (propertyInfo.type == PropertyType.Int || propertyInfo.type == PropertyType.Tile)
			{
				int num = 1;
				if (float.TryParse(inputField.text, out float result2))
				{
					num = Mathf.RoundToInt(result2);
					num = propertyInfo.Validate(num);
				}
				else
				{
					DataTable dataTable2 = new DataTable();
					try
					{
						num = RDEditorUtils.DecodeInt(dataTable2.Compute(inputField.text, ""));
					}
					catch
					{
						RDBaseDll.printesw("[" + propertyInfo.name + "] Invalid int value: " + inputField.text);
						num = (int)propertyInfo.value_default;
					}
				}
				return num.ToString();
			}
			return inputField.text;
		}

		public override void ValidateInput()
		{
			inputField.text = Validate();
		}

		public override void Setup(bool addListener)
		{
			if (addListener)
			{
				if (propertyInfo.name == "artist")
				{
					inputField.onValueChanged.AddListener(delegate(string s)
					{
						ADOBase.editor.settingsPanel.ToggleArtistPopup(s, rectTransform.position.y, this);
						ToggleOthersEnabled();
					});
				}
				inputField.onEndEdit.AddListener(delegate
				{
					if (!(propertyInfo.name == "artist"))
					{
						using (new SaveStateScope(ADOBase.editor))
						{
							ValidateInput();
							LevelEvent selectedEvent = propertiesPanel.inspectorPanel.selectedEvent;
							PropertyType type = propertyInfo.type;
							string text = inputField.text;
							object obj = null;
							switch (type)
							{
							case PropertyType.Int:
								obj = int.Parse(text);
								break;
							case PropertyType.Float:
								obj = float.Parse(text);
								break;
							case PropertyType.String:
								obj = text;
								break;
							case PropertyType.Tile:
							{
								Tuple<int, TileRelativeTo> tuple = selectedEvent[propertyInfo.name] as Tuple<int, TileRelativeTo>;
								obj = new Tuple<int, TileRelativeTo>(int.Parse(text), tuple.Item2);
								break;
							}
							}
							if (propertyInfo.name == "floor")
							{
								selectedEvent.floor = (int)obj;
							}
							else
							{
								selectedEvent[propertyInfo.name] = obj;
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
							if (ADOBase.editor.SelectionIsSingle())
							{
								ADOBase.editor.ShowEventIndicators(ADOBase.editor.selectedFloors[0]);
							}
						}
					}
				});
			}
			if (!string.IsNullOrEmpty(propertyInfo.unit))
			{
				unit.gameObject.SetActive(value: true);
				unit.text = RDString.Get("editor.unit." + propertyInfo.unit);
			}
		}
	}
}
