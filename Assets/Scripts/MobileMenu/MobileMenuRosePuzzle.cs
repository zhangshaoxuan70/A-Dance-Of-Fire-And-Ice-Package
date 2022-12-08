using DG.Tweening;
using System;
using UnityEngine;

namespace MobileMenu
{
	public class MobileMenuRosePuzzle : ADOBase
	{
		public MobileMenuController menuController;

		public Transform hareContainer;

		public MobileMenuHare[] hares;

		public MobileMenuGrabbableRose[] roses;

		public Transform[] roseContainers;

		private int roseIndex;

		private int stage;

		private void Start()
		{
			bool unlockedXR = Persistence.GetUnlockedXR();
			if (!Persistence.IsWorldComplete(ADOBase.worldData["XH"].index) || unlockedXR)
			{
				base.gameObject.SetActive(value: false);
				return;
			}
			MobileMenuGroup mobileMenuGroup = menuController.map.groupLUT["mainGroup"];
			Transform[] array = roseContainers;
			foreach (Transform obj in array)
			{
				MobileMenuScreen mobileMenuScreen = mobileMenuGroup[UnityEngine.Random.Range(1, mobileMenuGroup.visibleScreens.Count)];
				obj.SetParent(mobileMenuScreen.transform, worldPositionStays: false);
				obj.localScale = Vector2.one / mobileMenuScreen.transform.localScale;
			}
			hareContainer.SetParent(ADOBase.controller.camy.camobj.transform, worldPositionStays: false);
			MobileMenuGrabController grabController = menuController.grabController;
			grabController.onGrab = (Action<MobileMenuGrabbable>)Delegate.Combine(grabController.onGrab, new Action<MobileMenuGrabbable>(OnGrab));
			grabController.onUngrab = (Action<MobileMenuGrabbable>)Delegate.Combine(grabController.onUngrab, new Action<MobileMenuGrabbable>(OnUngrab));
		}

		private void OnGrab(MobileMenuGrabbable obj)
		{
			if (obj is MobileMenuGrabbableRose)
			{
				MobileMenuGrabbable[] array = roses;
				roseIndex = Array.IndexOf(array, obj);
				hares[roseIndex].Show(show: true);
			}
		}

		private void OnUngrab(MobileMenuGrabbable obj)
		{
			if (obj is MobileMenuGrabbableRose)
			{
				Collider2D component = obj.GetComponent<Collider2D>();
				Transform spriteTransform = hares[roseIndex].spriteTransform;
				Collider2D component2 = spriteTransform.GetComponent<Collider2D>();
				if (component.Distance(component2).isOverlapped)
				{
					obj.DOKill();
					obj.grabbable = false;
					obj.transform.SetParent(spriteTransform.transform, worldPositionStays: true);
					AdvanceStage();
				}
				hares[roseIndex].Show(show: false);
			}
		}

		private void UnlockXR()
		{
			scrFlash.Flash();
			Persistence.SetUnlockedXR(unlocked: true);
			menuController.map.portalLUT["XR"].visible = true;
			menuController.map.Build();
		}

		private void AdvanceStage()
		{
			MobileMenuController.PlaySfx((stage == 0) ? SfxSound.MobileMenuXR1 : ((stage == 1) ? SfxSound.MobileMenuXR2 : SfxSound.MobileMenuXR3));
			if (stage == roses.Length - 1)
			{
				DOVirtual.DelayedCall(2.6f, UnlockXR);
			}
			stage++;
		}
	}
}
