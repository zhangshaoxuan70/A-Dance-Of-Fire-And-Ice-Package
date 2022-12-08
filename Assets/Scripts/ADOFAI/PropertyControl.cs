using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI
{
	public class PropertyControl : ADOBase
	{
		public PropertyInfo propertyInfo;

		public PropertiesPanel propertiesPanel;

		public RectTransform rectTransform;

		public PropertyControl randomControl;

		public virtual string text
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public virtual void OnRightClick()
		{
		}

		public virtual void ValidateInput()
		{
		}

		public virtual void Setup(bool addListener)
		{
		}

		public virtual void EnumSetup(string enumTypeString, List<string> enumVals, bool localize = true)
		{
		}

		public void ToggleOthersEnabled()
		{
			if (propertiesPanel.name == "LevelSettings")
			{
				Property property = propertiesPanel.properties["specialArtistType"];
				ApprovalLevelBadge approvalLevelBadge = ADOBase.editor.settingsPanel.approvalLevelBadge;
				bool enabled = approvalLevelBadge == null || approvalLevelBadge.approvalLevel == ApprovalLevel.Pending;
				property.control.SetEnabled(enabled);
				Property property2 = propertiesPanel.properties["artistPermission"];
				bool enabled2 = false;
				if (approvalLevelBadge != null)
				{
					if (approvalLevelBadge.approvalLevel == ApprovalLevel.Pending)
					{
						enabled2 = ((SpecialArtistType)ADOBase.editor.settingsPanel.selectedEvent["specialArtistType"] == SpecialArtistType.None);
					}
				}
				else
				{
					enabled2 = true;
				}
				property2.control.SetEnabled(enabled2);
			}
			foreach (Property value in propertiesPanel.properties.Values)
			{
				if (!(value.info.name == "specialArtistType") && !(value.info.name == "artistPermission"))
				{
					value.control.UpdateEnabled();
				}
			}
		}

		public void UpdateEnabled()
		{
			SetEnabled(CheckIfEnabled());
		}

		public virtual void SetEnabled(bool enabled)
		{
			Color color = enabled ? Color.white : Color.gray;
			Text[] componentsInChildren = GetComponentsInChildren<Text>();
			foreach (Text text in componentsInChildren)
			{
				if (!(text.color == Color.black))
				{
					text.color = color;
				}
			}
			InputField[] componentsInChildren2 = GetComponentsInChildren<InputField>();
			for (int i = 0; i < componentsInChildren2.Length; i++)
			{
				componentsInChildren2[i].interactable = enabled;
			}
			Button[] componentsInChildren3 = GetComponentsInChildren<Button>();
			for (int i = 0; i < componentsInChildren3.Length; i++)
			{
				componentsInChildren3[i].interactable = enabled;
			}
			Dropdown[] componentsInChildren4 = GetComponentsInChildren<Dropdown>();
			for (int i = 0; i < componentsInChildren4.Length; i++)
			{
				componentsInChildren4[i].interactable = enabled;
			}
			propertyInfo.isEnabled = enabled;
		}

		public bool CheckIfEnabled()
		{
			bool flag = true;
			bool flag2 = true;
			if (propertyInfo.enableIfVals.Count != 0)
			{
				flag = ValueMatch(propertyInfo.enableIfVals);
			}
			if (propertyInfo.disableIfVals.Count != 0)
			{
				flag2 = !ValueMatch(propertyInfo.disableIfVals);
			}
			return flag && flag2;
		}

		private bool ValueMatch(List<Tuple<string, string>> keysAndVals)
		{
			foreach (Tuple<string, string> keysAndVal in keysAndVals)
			{
				LevelEvent selectedEvent = propertiesPanel.inspectorPanel.selectedEvent;
				if (selectedEvent == null)
				{
					return false;
				}
				PropertyInfo propertyInfo = this.propertyInfo.levelEventInfo.propertiesInfo[keysAndVal.Item1];
				string item = keysAndVal.Item2;
				object obj = selectedEvent[propertyInfo.name];
				switch (propertyInfo.type)
				{
				case PropertyType.Int:
				case PropertyType.Rating:
					if (Convert.ToInt32(item) == (int)obj)
					{
						return true;
					}
					break;
				case PropertyType.Float:
					if (Mathf.Approximately(Convert.ToSingle(item), (float)obj))
					{
						return true;
					}
					break;
				case PropertyType.String:
				case PropertyType.File:
				case PropertyType.Enum:
				case PropertyType.List:
					if (item == obj.ToString())
					{
						return true;
					}
					break;
				}
			}
			return false;
		}

		public virtual void SetRandomLayout()
		{
		}

		public virtual void OnSelectedEventChanged()
		{
		}
	}
}
