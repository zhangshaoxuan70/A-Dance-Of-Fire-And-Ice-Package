using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class ffxDamageRat : ffxBase
{
	public Transform target;

	private static SpriteRenderer ratObj;

	public int floorCount = 3;

	public float durationBeats = 1f;

	private List<scrFloor> targetFloors = new List<scrFloor>();

	private void Start()
	{
		if (ratObj == null)
		{
			GameObject gameObject = GameObject.Find("ratking1");
			if (gameObject != null)
			{
				ratObj = gameObject.GetComponent<SpriteRenderer>();
			}
		}
	}

	public override void doEffect()
	{
		if (!(ratObj == null))
		{
			if (cond == null)
			{
				printe("cond: null");
			}
			int num = floor.seqID - 1;
			float num2 = durationBeats * (float)cond.crotchetAtStart / (floor.speed * cond.song.pitch);
			for (int num3 = num; num3 >= num - floorCount; num3--)
			{
				scrFloor scrFloor = ADOBase.lm.listFloors[num3];
				targetFloors.Add(scrFloor);
				scrFloor.transform.SetParent(target, worldPositionStays: true);
				scrFloor.transform.DOLocalMove(Vector3.zero, num2).SetEase(Ease.InCubic);
				scrFloor.transform.DORotate(new Vector3(0f, 0f, Random.Range(-180, 180)), num2).SetEase(Ease.InCubic);
				scrFloor.MoveToBack();
			}
			Sequence s = DOTween.Sequence();
			s.AppendInterval(num2);
			s.AppendCallback(delegate
			{
				foreach (scrFloor targetFloor in targetFloors)
				{
					targetFloor.transform.localScale = Vector3.zero;
				}
				float duration = 0.25f * (float)cond.crotchet;
				ratObj.material.SetFloat("_Flash", 1f);
				ratObj.material.DOFloat(0f, "_Flash", duration).SetUpdate(isIndependentUpdate: true);
				DOTween.Shake(() => target.localPosition, delegate(Vector3 x)
				{
					target.localPosition = x;
				}, duration, 1f, 100, 90f, ignoreZAxis: false);
			});
		}
	}
}
