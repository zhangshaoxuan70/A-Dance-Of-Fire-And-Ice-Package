using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI
{
	public class PropertyControl_Color : PropertyControl
	{
		public InputField inputField;

		public Image sample;

		public override string text
		{
			get
			{
				return Validate();
			}
			set
			{
				inputField.text = value;
				sample.color = value.HexToColor();
			}
		}

		public string Validate()
		{
			string text = inputField.text;
			if (propertyInfo == null)
			{
				return text;
			}
			if (propertyInfo.type == PropertyType.Color)
			{
				if (!RDUtils.IsHex(text) || text.Length < 6)
				{
					return (string)propertyInfo.value_default;
				}
				int length = 6;
				if (text.Length >= 8)
				{
					length = 8;
				}
				return text.Substring(0, length);
			}
			UnityEngine.Debug.LogError("propertyInfo.type is not PropertyType.Color");
			return "";
		}

		public override void ValidateInput()
		{
			inputField.text = Validate();
		}

		public override void Setup(bool addListener)
		{
			if (addListener)
			{
				inputField.onEndEdit.AddListener(delegate(string s)
				{
					OnEndEdit(s);
				});
			}
		}

		public void ShowColorPicker()
		{
			ADOBase.editor.colorPickerPopup.Show(this);
		}

		public void OnEndEdit(string s)
		{
			using (new SaveStateScope(ADOBase.editor))
			{
				ValidateInput();
				LevelEvent selectedEvent = propertiesPanel.inspectorPanel.selectedEvent;
				PropertyType type = propertyInfo.type;
				string text = inputField.text;
				selectedEvent[propertyInfo.name] = text;
				sample.color = text.HexToColor();
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
}
