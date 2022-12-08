using DG.Tweening;
using UnityEngine;

public class ffxButterflyCircle : ffxBase
{
	private GameObject butterflyPrefab;

	public int butterflyCount = 12;

	public Color butterflyColor = Color.white;

	public bool randomColor;

	public bool planetColors;

	public override void Awake()
	{
		base.Awake();
		butterflyPrefab = ADOBase.gc.prefab_butterfly;
	}

	public override void doEffect()
	{
		Color playerColor = Persistence.GetPlayerColor(red: true);
		Color playerColor2 = Persistence.GetPlayerColor(red: false);
		float num = 1f / scrConductor.instance.song.pitch;
		for (int i = 0; i < butterflyCount; i++)
		{
			Color color = (!randomColor) ? ((!planetColors) ? butterflyColor : ((i % 2 == 0) ? playerColor : playerColor2)) : Color.HSVToRGB(Random.Range(0f, 1f), 1f, 1f);
			Quaternion rotation = Quaternion.Euler(0f, 0f, 360 / butterflyCount * i);
			GameObject newButterfly = Object.Instantiate(butterflyPrefab, base.transform.position, Quaternion.identity);
			newButterfly.GetComponent<SpriteRenderer>().color = color;
			Vector3 vector = newButterfly.transform.position + rotation * new Vector3(3f, 0f, 0f);
			DOTween.Sequence().Append(newButterfly.transform.DOMove(vector, 0.75f * num).SetEase(Ease.OutExpo)).Append(newButterfly.transform.DOMoveY((newButterfly.transform.position + vector).y + 50f, 3f * num).SetEase(Ease.InSine))
				.AppendCallback(delegate
				{
					UnityEngine.Object.Destroy(newButterfly);
				});
		}
	}
}
