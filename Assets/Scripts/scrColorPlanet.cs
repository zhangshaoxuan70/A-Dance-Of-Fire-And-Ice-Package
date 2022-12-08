using DG.Tweening;
using UnityEngine;

public class scrColorPlanet : ffxBase
{
	public PlanetColor planetColor;

	[Header("Obsolete")]
	public Color color = Color.white;

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

	private void Start()
	{
		if (planetColor == PlanetColor.Rainbow)
		{
			SpriteRenderer component = GetComponent<SpriteRenderer>();
			component.color = Color.red;
			Color.RGBToHSV(Color.red, out float _, out float S, out float V);
			Tween[] array = new Tween[10];
			Sequence sequence = DOTween.Sequence();
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = component.DOColor(Color.HSVToRGB(0.1f + 0.1f * (float)i, S, V), 0.5f);
				sequence.Append(array[i]);
			}
			sequence.SetLoops(-1, LoopType.Restart).SetUpdate(isIndependentUpdate: true);
		}
	}

	public override void doEffect()
	{
		scrController.instance.chosenplanet.other.SetColor(planetColor);
		if ((bool)scrLogoText.instance)
		{
			scrLogoText.instance.UpdateColors();
		}
	}
}
