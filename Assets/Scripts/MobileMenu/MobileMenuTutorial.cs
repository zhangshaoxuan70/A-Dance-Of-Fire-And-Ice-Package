using DG.Tweening;
using System;
using UnityEngine;

namespace MobileMenu
{
	public class MobileMenuTutorial : ADOBase
	{
		public MobileMenuController menuController;

		public SpriteRenderer backgroundFader;

		public GameObject instructions;

		public scrFloor[] floors;

		public GameObject planets;

		[NonSerialized]
		public bool doingTutorial;

		public bool TryRunTutorial()
		{
			doingTutorial = (!Persistence.GetPassedMobileTutorial() && Persistence.GetOverallProgressStage() <= 0);
			base.gameObject.SetActive(doingTutorial);
			backgroundFader.gameObject.SetActive(doingTutorial);
			instructions.SetActive(doingTutorial);
			ADOBase.controller.responsive = doingTutorial;
			planets.transform.position = (doingTutorial ? planets.transform.position : ((Vector3)(Vector2.one * 10000f)));
			if (doingTutorial)
			{
				ToggleTutorial(enabled: true);
				return true;
			}
			return false;
		}

		private void ToggleTutorial(bool enabled)
		{
			base.gameObject.SetActive(enabled);
			backgroundFader.gameObject.SetActive(enabled);
			instructions.SetActive(enabled);
			doingTutorial = enabled;
			menuController.Enable(!enabled);
			HideAllButTitleScreen(enabled);
			ADOBase.controller.responsive = enabled;
			if (!enabled)
			{
				GCS.worldEntrance = null;
				menuController.JumpToMenuEntrance();
			}
		}

		private void DoCompleteTutorial()
		{
			Persistence.SetPassedMobileTutorial(passed: true);
			ADOBase.controller.responsive = false;
			Sequence s = DOTween.Sequence();
			float num = 1.5f;
			for (int i = 10; i < floors.Length; i++)
			{
				Transform transform = floors[i].transform;
				s.Insert(num / (float)floors.Length * (float)i, transform.DOScale(0f, 0.25f).SetEase(Ease.OutSine));
			}
			scrPlanet planet = ADOBase.controller.chosenplanet;
			planet.transform.parent.SetParent(planet.currfloor.transform);
			DOTween.To(() => planet.cosmeticRadius, delegate(float x)
			{
				planet.cosmeticRadius = x;
			}, 0f, 0.5f);
			Timer.Add(delegate
			{
				ToggleTutorial(enabled: false);
			}, num);
		}

		private void HideAllButTitleScreen(bool hide)
		{
			MobileMenuGroup rootGroup = menuController.map.rootGroup;
			foreach (MobileMenuGroup value in menuController.map.groupLUT.Values)
			{
				if (value.visibleScreens != null)
				{
					if (value == rootGroup)
					{
						scrLogoText[] componentsInChildren = (value[0] as MobileMenuTitle).transform.GetComponentsInChildren<scrLogoText>();
						for (int i = 0; i < componentsInChildren.Length; i++)
						{
							componentsInChildren[i].transform.DOLocalMoveY(hide ? 100f : 0f, 0.5f).SetEase(Ease.OutSine);
						}
					}
					else
					{
						foreach (MobileMenuScreen item in value)
						{
							item.transform?.gameObject.SetActive(!hide);
						}
					}
				}
			}
		}

		private void Update()
		{
			if (doingTutorial)
			{
				scrFloor scrFloor = floors[0];
				scrFloor[] array = floors;
				scrFloor y = array[array.Length - 1];
				scrCamera camy = ADOBase.controller.camy;
				float x = camy.transform.position.x;
				float x2 = ADOBase.controller.chosenplanet.transform.position.x;
				float num = Mathf.Lerp(x, x2, 4f * Time.deltaTime);
				camy.transform.position = camy.transform.position.WithX(num);
				float num2 = Mathf.InverseLerp(scrFloor.transform.position.x, scrFloor.transform.position.y, num);
				backgroundFader.color = backgroundFader.color.WithAlpha(1f - num2);
				ADOBase.conductor.song2.volume = num2;
				if (ADOBase.controller.chosenplanet.currfloor == y)
				{
					DoCompleteTutorial();
				}
			}
		}
	}
}
