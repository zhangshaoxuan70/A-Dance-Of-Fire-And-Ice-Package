using UnityEngine;

namespace MobileMenu
{
	public class MobileMenuCredits : MobileMenuScreen
	{
		public scrCreditsText credits;

		public override void Instantiate()
		{
			transform = Object.Instantiate(RDConstants.data.prefab_creditsScreen).transform;
			credits = transform.GetComponentInChildren<scrCreditsText>();
			credits.Reset(instant: true);
			credits.transform.LocalMoveY(4.5f);
			transform.Find("pigStatue").GetComponent<scrGfxFloat>().transform.TranslateY(4.5f);
		}

		public override void Select(bool select = true, bool instant = false)
		{
			credits.SetScroll(select);
		}
	}
}
