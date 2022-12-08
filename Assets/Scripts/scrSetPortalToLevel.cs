using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class scrSetPortalToLevel : ffxBase
{
	public enum WorldClearType
	{
		NormalClear,
		SpeedTrialClear
	}

	public Sprite levelIcon;

	public scrFloor portalFloor;

	[FormerlySerializedAs("worldIndex")]
	public string world;

	public bool speedTrial;

	public string worldToRequireClearing;

	public WorldClearType worldClearType;

	public float speedTrialCompletionRequirement;

	public scrBestMultiplierText bestText;

	public override void Awake()
	{
		base.Awake();
		if (!portalFloor.dontChangeMySprite)
		{
			portalFloor.dontChangeMySprite = true;
			portalFloor.topGlow.color = Color.clear;
			if ((bool)portalFloor.bottomGlow)
			{
				portalFloor.bottomGlow.color = Color.clear;
			}
		}
	}

	private void Start()
	{
		ADOBase.levelSelect.portalToLevelComponents.Add(this);
		if (speedTrial && !Persistence.IsWorldComplete(ADOBase.worldData[world].index))
		{
			base.enabled = false;
			return;
		}
		if (worldToRequireClearing != "")
		{
			if (worldClearType == WorldClearType.NormalClear)
			{
				if (!Persistence.IsWorldComplete(ADOBase.worldData[worldToRequireClearing].index))
				{
					base.enabled = false;
					return;
				}
			}
			else if (Persistence.GetBestSpeedMultiplier(ADOBase.worldData[worldToRequireClearing].index) < speedTrialCompletionRequirement)
			{
				base.enabled = false;
				return;
			}
		}
		StartCoroutine(SetIcon());
	}

	public override void doEffect()
	{
		List<scrSetPortalToLevel> portalToLevelComponents = ADOBase.levelSelect.portalToLevelComponents;
		foreach (scrSetPortalToLevel item in portalToLevelComponents)
		{
			scrFloor scrFloor = item.portalFloor;
			if (item.portalFloor.isportal)
			{
				scrFloor.isportal = false;
				scrFloor.CheckPortalSprite();
				scrFloor.arguments = "";
			}
		}
		scrFlash.Flash(Color.white.WithAlpha(0.4f));
		portalFloor.isportal = true;
		portalFloor.CheckPortalSprite();
		portalFloor.levelnumber = (speedTrial ? (-15) : (-16));
		portalFloor.arguments = (speedTrial ? (world + "-X") : world);
		scrPortalParticles componentInChildren = portalFloor.GetComponentInChildren<scrPortalParticles>();
		componentInChildren.speedTrial = speedTrial;
		componentInChildren.disabled = false;
		foreach (scrPortal portal in scrPortal.portals)
		{
			if (portal.world.IsXtra() || portal.world.IsMuseDashWorld())
			{
				if (portal.world == world)
				{
					portal.ShowStats(show: true, instant: true);
					portal.ExpandPortal(expand: true, instant: true);
					if (portal.xtraDecoration != null)
					{
						SpriteRenderer xtraDecoration = portal.xtraDecoration;
						if (xtraDecoration != null && !base.gameObject.name.Contains("DONT"))
						{
							xtraDecoration.color = (speedTrial ? new Color(1f, 0.7f, 0.7f) : Color.white);
						}
						SpriteRenderer[] componentsInChildren = portal.xtraDecoration.GetComponentsInChildren<SpriteRenderer>();
						foreach (SpriteRenderer spriteRenderer in componentsInChildren)
						{
							if (!spriteRenderer.gameObject.name.Contains("DONT"))
							{
								spriteRenderer.color = (speedTrial ? new Color(1f, 0.7f, 0.7f) : Color.white);
							}
						}
					}
				}
				else
				{
					portal.ShowStats(show: false, instant: true);
					portal.ExpandPortal(expand: false, instant: true);
				}
			}
		}
		foreach (scrSetPortalToLevel item2 in portalToLevelComponents)
		{
			scrBestMultiplierText bestText2 = item2.bestText;
			if (item2.bestText != null)
			{
				bestText.gameObject.SetActive(speedTrial);
				bestText.UpdateText(showLongVersion: true);
			}
		}
	}

	private IEnumerator SetIcon()
	{
		yield return null;
		floor.SetIconSprite(levelIcon);
		if (speedTrial)
		{
			floor.SetIconColor(Color.red);
		}
	}
}
