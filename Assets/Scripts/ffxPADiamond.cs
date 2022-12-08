using DG.Tweening;
using UnityEngine;

public class ffxPADiamond : ffxBase
{
	public bool hideDiamond;

	public bool doubleDiamond;

	public bool circle;

	public bool heart;

	private GameObject diamondPrefab;

	private GameObject circlePrefab;

	private GameObject heartPrefab;

	public override void Awake()
	{
		base.Awake();
		circlePrefab = ADOBase.gc.prefab_PA_circle;
		diamondPrefab = ADOBase.gc.prefab_PA_diamond;
		heartPrefab = ADOBase.gc.prefab_PA_heart;
	}

	public override void doEffect()
	{
		float num = 1f / scrConductor.instance.song.pitch;
		if (!hideDiamond)
		{
			Transform transform = Object.Instantiate(diamondPrefab, base.transform.position, base.transform.rotation).transform;
			transform.ScaleXY(0f, 0f);
			transform.DOScale(new Vector2(1f, 1f), 3f * num).SetEase(Ease.OutExpo);
			transform.GetComponent<SpriteRenderer>().DOFade(0f, num).SetDelay(num);
		}
		if (doubleDiamond)
		{
			Transform transform2 = Object.Instantiate(diamondPrefab, base.transform.position, base.transform.rotation).transform;
			transform2.ScaleXY(0f, 0f);
			transform2.DOScale(new Vector2(1.5f, 1.5f), 2.5f * num).SetEase(Ease.OutExpo);
			transform2.GetComponent<SpriteRenderer>().DOFade(0f, num).SetDelay(0.75f * num);
		}
		if (circle)
		{
			Transform transform3 = Object.Instantiate(circlePrefab, base.transform.position, Quaternion.identity).transform;
			transform3.ScaleXY(0f, 0f);
			transform3.DOScale(new Vector2(1f, 1f), 10f * num).SetEase(Ease.OutExpo);
			transform3.GetComponent<SpriteRenderer>().DOFade(0f, 2f * num).SetDelay(num);
		}
		if (heart)
		{
			Transform transform4 = Object.Instantiate(heartPrefab, base.transform.position.WithY(base.transform.position.y + 0.5f), Quaternion.identity).transform;
		}
	}
}
