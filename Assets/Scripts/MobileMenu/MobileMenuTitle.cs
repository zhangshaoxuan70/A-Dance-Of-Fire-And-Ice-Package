using DG.Tweening;
using UnityEngine;

namespace MobileMenu
{
	public class MobileMenuTitle : MobileMenuScreen
	{
		public CanvasGroup canvasGroup;

		public override void Instantiate()
		{
			transform = Object.Instantiate(RDConstants.data.prefab_titleScreen).transform;
			canvasGroup = transform.GetComponentInChildren<CanvasGroup>();
		}

		public override void Select(bool select = true, bool instant = false)
		{
			float duration = instant ? 0f : 0.25f;
			canvasGroup.DOFade(select ? 1f : 0f, duration);
		}
	}
}
