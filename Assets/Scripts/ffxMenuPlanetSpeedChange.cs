using DG.Tweening;

public class ffxMenuPlanetSpeedChange : ffxBase
{
	public override void Awake()
	{
		base.Awake();
		floor.topGlow.enabled = false;
		floor.floorIcon = FloorIcon.Rabbit;
		floor.UpdateIconSprite();
	}

	public void Start()
	{
		floor.topGlow.gameObject.SetActive(value: false);
		floor.bottomGlow.gameObject.SetActive(value: false);
	}

	public override void doEffect()
	{
		if (ctrl.speed == 1.0)
		{
			ctrl.speed = 2.0;
			floor.floorIcon = FloorIcon.Snail;
			ADOBase.conductor.song2.DOFade(ADOBase.IsHalloweenWeek() ? 0.7f : 0.7f, 0.2f);
		}
		else
		{
			ctrl.speed = 1.0;
			floor.floorIcon = FloorIcon.Rabbit;
			ADOBase.conductor.song2.DOFade(0f, 0.2f);
		}
		floor.UpdateIconSprite();
	}
}
