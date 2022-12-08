using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MobileMenu
{
	public class MobileMenuTreePuzzle : ADOBase
	{
		public MobileMenuController menuController;

		public Transform XHContainer;

		public Button XHButton;

		public Transform XHTree;

		private void Start()
		{
			bool unlockedXH = Persistence.GetUnlockedXH();
			if (!Persistence.IsWorldComplete(ADOBase.worldData["XC"].index) || unlockedXH)
			{
				base.gameObject.SetActive(value: false);
				return;
			}
			Transform transform = ADOBase.controller.pauseMenu.pauseMenuContainer.transform;
			XHContainer.transform.SetParent(transform);
			XHButton.onClick.AddListener(DoUnlock);
		}

		private void DoUnlock()
		{
			XHButton.enabled = false;
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Insert(0f, XHButton.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack))
				.Insert(0f, XHButton.transform.DOLocalRotate(Vector3.forward * 360f * -1.5f, 0.5f, RotateMode.LocalAxisAdd).SetEase(Ease.InExpo))
				.Insert(0.2f, XHTree.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack))
				.Insert(0.2f, XHTree.transform.DOLocalRotate(Vector3.forward * 360f * 1.5f, 0.5f, RotateMode.LocalAxisAdd).SetEase(Ease.InExpo));
			MobileMenuController.PlaySfx(SfxSound.MobileMenuXH);
			UnlockXH();
		}

		private void UnlockXH()
		{
			scrFlash.Flash();
			Persistence.SetUnlockedXH(unlocked: true);
			menuController.map.portalLUT["XH"].visible = true;
			menuController.map.Build();
		}
	}
}
