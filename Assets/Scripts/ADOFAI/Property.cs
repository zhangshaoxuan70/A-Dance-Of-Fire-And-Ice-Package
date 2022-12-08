using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI
{
	public class Property : MonoBehaviour
	{
		[Header("UI")]
		public Text label;

		public Button helpButton;

		public Button randomButton;

		public Button enabledButton;

		public GameObject enabledCheckmark;

		public RectTransform controlContainer;

		public GameObject offText;

		[Header("Runtime")]
		public string key;

		public PropertyControl control;

		private PropertyInfo _info;

		public PropertyInfo info
		{
			get
			{
				return _info;
			}
			set
			{
				_info = value;
				bool exists = false;
				string text = "";
				if (value.customLocalizationKey == null)
				{
					string str = "editor." + value.levelEventInfo.name + "." + value.name;
					text = RDString.GetWithCheck(str, out exists);
					if (!exists)
					{
						text = RDString.GetWithCheck("editor." + value.name, out exists);
						if (!exists)
						{
							UnityEngine.Debug.LogWarning("Didn't find localized key " + str + " or editor." + value.name);
						}
					}
				}
				else
				{
					text = ((value.customLocalizationKey == "") ? "" : RDString.Get(value.customLocalizationKey));
				}
				if (value.required)
				{
					text += " <color=red>*</color>";
				}
				label.text = text;
			}
		}
	}
}
