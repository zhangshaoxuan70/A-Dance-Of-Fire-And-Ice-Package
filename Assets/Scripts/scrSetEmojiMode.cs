public class scrSetEmojiMode : ffxBase
{
	public bool emojiMode;

	public override void Awake()
	{
		base.Awake();
		floor.dontChangeMySprite = true;
		floor.topGlow.gameObject.SetActive(value: false);
		if ((bool)floor.bottomGlow)
		{
			floor.bottomGlow.gameObject.SetActive(value: false);
		}
	}

	public override void doEffect()
	{
		scrPlanet other = scrController.instance.chosenplanet.other;
		other.SetFaceMode(emojiMode, pulseOnEnable: true);
		Persistence.SetFaceMode(emojiMode, other.isRed);
	}
}
