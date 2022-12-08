using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MobileMenu
{
	public class MobileMenuSublevelBrowserGenerator : MonoBehaviour
	{
		public Sprite spriteSublevel;

		public Sprite spriteSublevelLocked;

		public Sprite spriteSublevelCrown;

		public Sprite spriteSublevelCrownLocked;

		public GameObject sublevelButtonPrefab;

		public GameObject submenuPrefab;

		public RectTransform container;

		public Dictionary<string, GameObject> submenu = new Dictionary<string, GameObject>();

		public void GenerateSubmenu(string world)
		{
			Dictionary<string, GCNS.WorldData> worldData = GCNS.worldData;
			int levelCount = worldData[world].levelCount;
			if (levelCount <= 1)
			{
				return;
			}
			int index = worldData[world].index;
			int levelTutorialProgress = Persistence.GetLevelTutorialProgress(index);
			GameObject gameObject = UnityEngine.Object.Instantiate(submenuPrefab, base.transform);
			submenu.Add(world, gameObject);
			float num = 135f * (float)(levelCount - 2) + 168.75f;
			for (int i = 0; i < levelCount; i++)
			{
				bool isBoss = i == levelCount - 1;
				bool num2 = Persistence.GetWorldAttempts(index) > 0;
				bool flag = world.IsCrownWorld();
				bool flag2 = true;
				GameObject gameObject2 = UnityEngine.Object.Instantiate(sublevelButtonPrefab, gameObject.transform);
				Image component = gameObject2.transform.GetChild(0).GetComponent<Image>();
				RectTransform t = gameObject2.transform as RectTransform;
				Transform transform = component.transform;
				float num3 = isBoss ? 16.875f : 0f;
				t.LocalMoveX(135f * (float)i - num / 2f + num3);
				if (isBoss)
				{
					t.ScaleXY(1.25f, 1.25f);
				}
				if (!num2 && !Persistence.IsWorldComplete(index) && i > 0 && i > levelTutorialProgress)
				{
					flag2 = false;
				}
				Sprite sprite2 = component.sprite = ((!isBoss) ? ((!flag) ? (flag2 ? spriteSublevel : spriteSublevelLocked) : (flag2 ? spriteSublevelCrown : spriteSublevelLocked)) : Resources.Load<Sprite>(flag2 ? ("boss" + world) : ("boss" + world + "_locked")));
				if (flag2)
				{
					int lvl = i;
					gameObject2.GetComponent<Button>().onClick.AddListener(delegate
					{
						string str = isBoss ? "X" : (lvl + 1).ToString();
						MobileMenuController.EnterLevel(world + "-" + str, speedTrial: false);
					});
				}
				gameObject2.gameObject.SetActive(value: true);
				if (!isBoss)
				{
					Text componentInChildren = gameObject2.transform.GetComponentInChildren<Text>(includeInactive: true);
					componentInChildren.gameObject.SetActive(value: true);
					componentInChildren.text = (i + 1).ToString();
					if (flag)
					{
						componentInChildren.color = "333333".HexToColor();
						componentInChildren.GetComponent<Shadow>().effectColor = Color.black.WithAlpha(0.35f);
					}
				}
			}
		}
	}
}
