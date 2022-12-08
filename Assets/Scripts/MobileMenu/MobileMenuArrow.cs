using UnityEngine;
using UnityEngine.UI;

namespace MobileMenu
{
	public class MobileMenuArrow : MonoBehaviour
	{
		public Button button;

		public Image glow;

		public Text caption;

		public Image museDashIcon;

		public RectTransform rt => base.transform as RectTransform;

		public void Show(bool showButton, string captionKey)
		{
			base.gameObject.SetActive(showButton);
			if (!showButton)
			{
				return;
			}
			bool flag = captionKey == "museDash";
			if (museDashIcon != null)
			{
				museDashIcon.gameObject.SetActive(flag);
				if (flag)
				{
					captionKey = null;
				}
			}
			caption.text = (captionKey.IsNullOrEmpty() ? string.Empty : RDString.Get("levelSelect." + captionKey));
		}
	}
}
