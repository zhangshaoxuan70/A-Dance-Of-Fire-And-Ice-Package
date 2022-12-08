using RDTools;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI
{
	public class PropertyControl_Vector2 : PropertyControl
	{
		public InputField inputX;

		public InputField inputY;

		public Text unitX;

		public Text unitY;

		private Rect startRect;

		private Vector2 lastValue = Vector2.zero;

		public override string text
		{
			get
			{
				return Validate(inputX, inputY).ToString();
			}
			set
			{
				Vector2 vector = lastValue = RDUtils.StringToVector2(value);
				inputX.text = vector.x.ToString("0.######");
				inputY.text = vector.y.ToString("0.######");
			}
		}

		public void Awake()
		{
			if (rectTransform != null)
			{
				startRect = rectTransform.rect;
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
					rectTransform.anchorMin = rectTransform.anchorMin.WithY(0.4f);
					rectTransform.rect.Set(startRect.x, startRect.y + 2.4f, startRect.width, startRect.height);
				}
				else
				{
					randomControl.gameObject.SetActive(value: false);
					rectTransform.anchorMin = rectTransform.anchorMin.WithY(0f);
					rectTransform.rect.Set(startRect.x, startRect.y, startRect.width, startRect.height);
				}
			}
		}

		public (string, string) Validate(InputField x, InputField y)
		{
			Vector2 vector = new Vector2(lastValue.x, lastValue.y);
			if (float.TryParse(x.text, out float result) && float.TryParse(y.text, out float result2))
			{
				vector = new Vector2(result, result2);
				vector = propertyInfo.Validate(vector);
			}
			else
			{
				DataTable dataTable = new DataTable();
				try
				{
					object dictValue = dataTable.Compute(x.text, "");
					vector.x = RDEditorUtils.DecodeFloat(dictValue);
				}
				catch
				{
					RDBaseDll.printesw("Invalid coordinates (x): " + x.text);
				}
				try
				{
					object dictValue2 = dataTable.Compute(y.text, "");
					vector.y = RDEditorUtils.DecodeFloat(dictValue2);
				}
				catch
				{
					RDBaseDll.printesw("Invalid coordinates (y): " + y.text);
				}
			}
			if (propertiesPanel.inspectorPanel.selectedEvent.eventType == LevelEventType.AddDecoration && propertyInfo.name == "tile")
			{
				vector.x = Mathf.RoundToInt(vector.x);
				vector.y = Mathf.RoundToInt(vector.y);
			}
			return (vector.x.ToString("0.######"), vector.y.ToString("0.######"));
		}

		public override void ValidateInput()
		{
			InputField inputField = inputX;
			InputField inputField2 = inputY;
			(string, string) valueTuple = Validate(inputX, inputY);
			string text = inputField.text = valueTuple.Item1;
			text = (inputField2.text = valueTuple.Item2);
		}

		public override void Setup(bool addListener)
		{
			if (addListener)
			{
				inputX.onEndEdit.AddListener(delegate
				{
					SetVectorVals(inputX.text, inputY.text);
				});
				inputY.onEndEdit.AddListener(delegate
				{
					SetVectorVals(inputX.text, inputY.text);
				});
			}
			if (!string.IsNullOrEmpty(propertyInfo.unit))
			{
				unitX.gameObject.SetActive(value: true);
				unitX.text = RDString.Get("editor.unit." + propertyInfo.unit);
				unitY.gameObject.SetActive(value: true);
				unitY.text = RDString.Get("editor.unit." + propertyInfo.unit);
			}
		}

		private void SetVectorVals(string sX, string sY)
		{
			using (new SaveStateScope(ADOBase.editor))
			{
				ValidateInput();
				LevelEvent selectedEvent = propertiesPanel.inspectorPanel.selectedEvent;
				string text = inputX.text;
				string text2 = inputY.text;
				float x = float.Parse(text);
				float y = float.Parse(text2);
				Vector2 vector = new Vector2(x, y);
				selectedEvent[propertyInfo.name] = vector;
				ToggleOthersEnabled();
				if (selectedEvent.eventType == LevelEventType.BackgroundSettings)
				{
					ADOBase.customLevel.SetBackground();
				}
				else if (selectedEvent.eventType == LevelEventType.AddDecoration || selectedEvent.eventType == LevelEventType.AddText)
				{
					printe($"update decoration {selectedEvent}");
					ADOBase.editor.UpdateDecorationObject(selectedEvent);
				}
				if (selectedEvent.eventType == LevelEventType.PositionTrack || selectedEvent.eventType == LevelEventType.FreeRoam || selectedEvent.eventType == LevelEventType.FreeRoamTwirl || selectedEvent.eventType == LevelEventType.FreeRoamRemove || selectedEvent.eventType == LevelEventType.FreeRoamWarning)
				{
					ADOBase.editor.ApplyEventsToFloors();
					if (ADOBase.editor.SelectionIsSingle())
					{
						ADOBase.editor.floorButtonCanvas.transform.position = ADOBase.editor.selectedFloors[0].transform.position;
					}
				}
			}
		}
	}
}
