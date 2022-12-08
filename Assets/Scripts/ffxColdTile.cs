using DG.Tweening;
using UnityEngine;

public class ffxColdTile : ffxBase
{
	public int starCount = 6;

	public int puffCount = 6;

	private Color whiteClear = new Color(1f, 1f, 1f, 0f);

	private Color fadedAlp = new Color(1f, 1f, 1f, 0.7f);

	public override void Awake()
	{
		base.Awake();
	}

	private float RandF(float mi = 0f, float ma = 1f)
	{
		return Random.Range(mi, ma);
	}

	public override void doEffect()
	{
		float num = 1f / scrConductor.instance.song.pitch;
		float num2 = RandF() * 90f;
		for (int i = 0; i < starCount; i++)
		{
			float num3 = RandF(0.5f, 0.9f);
			Quaternion rotation = Quaternion.Euler(0f, 0f, num2 + (float)(360 / starCount * i));
			Mawaru_Sprite newStar = TaroBGScript.instance.GetTileStar();
			newStar.transform.position = base.transform.position;
			newStar.transform.localScale = Vector3.one * RandF(0.2f, 0.4f);
			newStar.transform.parent.gameObject.transform.position = Vector3.zero;
			newStar.render.enabled = true;
			Vector3 endValue = rotation * new Vector3(RandF(1f, 2f), 0f, 0f);
			newStar.transform.DOMove(endValue, num3 * num).SetEase(Ease.OutExpo).SetRelative(isRelative: true);
			DOTween.Sequence().Append(newStar.render.DOColor(fadedAlp, 0f).SetEase(Ease.Linear)).Append(newStar.render.DOColor(whiteClear, num3 * num).SetEase(Ease.Linear))
				.AppendCallback(delegate
				{
					newStar.render.enabled = false;
				});
		}
		for (int j = 0; j < puffCount; j++)
		{
			float num4 = RandF(0.9f, 1.5f);
			Quaternion rotation2 = Quaternion.Euler(0f, 0f, num2 + (float)(360 / puffCount * j));
			Mawaru_Sprite newPuff = TaroBGScript.instance.GetTileGlow();
			newPuff.transform.position = base.transform.position;
			newPuff.transform.localScale = Vector3.one * RandF(1.4f, 2f);
			newPuff.transform.parent.gameObject.transform.position = Vector3.zero;
			newPuff.render.enabled = true;
			Vector3 endValue2 = rotation2 * new Vector3(RandF(0.4f, 1.2f), 0f, 0f);
			newPuff.transform.DOMove(endValue2, num4 * num).SetEase(Ease.OutExpo).SetRelative(isRelative: true);
			DOTween.Sequence().Append(newPuff.render.DOColor(Color.white, 0f).SetEase(Ease.Linear)).Append(newPuff.render.DOColor(whiteClear, num4 * num).SetEase(Ease.Linear))
				.AppendCallback(delegate
				{
					newPuff.render.enabled = false;
				});
		}
	}
}
