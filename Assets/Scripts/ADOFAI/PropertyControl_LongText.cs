using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI
{
	public class PropertyControl_LongText : PropertyControl
	{
		public InputField inputField;

		public InputField.EndEditEvent onEndEdit => inputField.onEndEdit;

		public override string text
		{
			get
			{
				return inputField.text;
			}
			set
			{
				inputField.text = value;
			}
		}

		public override void Setup(bool addListener)
		{
			inputField.textComponent.transform.position += new Vector3(0f, -7f, 0f);
			if (addListener)
			{
				inputField.onEndEdit.AddListener(delegate
				{
					using (new SaveStateScope(ADOBase.editor))
					{
						propertiesPanel.inspectorPanel.selectedEvent[propertyInfo.name] = inputField.text;
						if (propertyInfo.affectsFloors)
						{
							ADOBase.editor.ApplyEventsToFloors();
						}
						if (ADOBase.editor.SelectionIsSingle())
						{
							ADOBase.editor.ShowEventIndicators(ADOBase.editor.selectedFloors[0]);
						}
					}
				});
			}
		}

		public override void ValidateInput()
		{
		}
	}
}
