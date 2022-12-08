using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MobileMenu
{
	public class MobileMenuPortal : MobileMenuScreen
	{
		public scrPortal portal;

		public string world;

		public int? stageForUnlock;

		public string[] unlockConditions;

		public PortalCreditData levelCredits;

		public PortalCreditData songCredits;

		public PortalCreditData secondaryLevelCredits;

		public PortalCreditData secondarySongCredits;

		public PortalCreditData tertiaryLevelCredits;

		private bool crown;

		public override void Select(bool select, bool instant = false)
		{
			portal.FadePortal(select ? 1f : 0.2f, instant);
			portal.FadeCredits(select ? 1f : 0f, instant);
			portal.ShowStats(select, instant);
		}

		public override void Instantiate()
		{
			transform = Object.Instantiate(RDConstants.data.prefab_worldPortal).transform;
			portal = transform.GetComponent<scrPortal>();
			portal.world = world;
			portal.levelCredits.Load(levelCredits);
			portal.songCredits.Load(songCredits);
			portal.secondaryLevelCredits.Load(secondaryLevelCredits);
			portal.secondarySongCredits.Load(secondarySongCredits);
			portal.tertiaryLevelCredits.Load(tertiaryLevelCredits);
			portal.sprPortal.sprite = Resources.Load<Sprite>("PortalImages\\" + world);
			portal.usesCrownSign = crown;
			RectTransform statsCanvasMobilePlaceholder = portal.statsCanvasMobilePlaceholder;
			RectTransform obj = portal.stats.transform as RectTransform;
			obj.localPosition = statsCanvasMobilePlaceholder.localPosition;
			obj.sizeDelta = statsCanvasMobilePlaceholder.sizeDelta;
			portal.stats.gameObject.AddComponent<scrScaleByAspectRatio>().referenceAspectRatio = 1.77777779f;
			RectTransform obj2 = portal.statsText.transform as RectTransform;
			obj2.sizeDelta = statsCanvasMobilePlaceholder.sizeDelta;
			obj2.anchoredPosition = Vector2.zero;
			Select(select: false, instant: true);
		}

		public void CheckLocked(bool speedTrial)
		{
			bool flag = Persistence.IsWorldComplete(portal.world);
			bool locked = (speedTrial && !flag) || (!flag && !MobileMenuMap.EvaluateConditions(unlockConditions));
			portal.LockWorld(locked, speedTrial);
			portal.UpdateWorldName(speedTrial);
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			base.Decode(dict);
			world = (string)dict["world"];
			dict.TryGetValueAs("crown", out crown);
			if (dict.TryGetValueAs("unlockedIf", out List<object> valueAs))
			{
				unlockConditions = valueAs.OfType<string>().ToArray();
			}
			if (dict.TryGetValueAs("levelCredits", out Dictionary<string, object> valueAs2))
			{
				levelCredits = new PortalCreditData(valueAs2);
			}
			if (dict.TryGetValueAs("secondaryLevelCredits", out Dictionary<string, object> valueAs3))
			{
				secondaryLevelCredits = new PortalCreditData(valueAs3);
			}
			if (dict.TryGetValueAs("tertiaryLevelCredits", out Dictionary<string, object> valueAs4))
			{
				tertiaryLevelCredits = new PortalCreditData(valueAs4);
			}
			if (dict.TryGetValueAs("songCredits", out Dictionary<string, object> valueAs5))
			{
				songCredits = new PortalCreditData(valueAs5);
			}
			if (dict.TryGetValueAs("secondarySongCredits", out Dictionary<string, object> valueAs6))
			{
				secondarySongCredits = new PortalCreditData(valueAs6);
			}
		}

		public override float GetWidth()
		{
			return base.GetWidth() / 2f;
		}
	}
}
