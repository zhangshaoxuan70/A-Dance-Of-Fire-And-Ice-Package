using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class MonsterChase : ADOBase
{
	public List<SpriteRenderer> monsters;

	public Transform offsetTransform;

	public float appearInterval = 0.3f;

	public float appearDuration = 0.4f;

	public float initialScale = 0.8f;

	public Vector2 positionOffset;

	public float movementLerpT = 0.5f;

	private List<Vector3> initialMonsterScales;

	public void Awake()
	{
		initialMonsterScales = new List<Vector3>(monsters.Count);
		int num = 0;
		foreach (SpriteRenderer monster in monsters)
		{
			monster.color = ADOBase.ClearWhite;
			Vector3 localScale = monster.transform.localScale;
			initialMonsterScales.Add(new Vector3(localScale.x, localScale.y, 1f));
			monster.transform.localScale = new Vector3(localScale.x * initialScale, localScale.y * initialScale, 1f);
			scrGfxFloat scrGfxFloat = monster.gameObject.AddComponent<scrGfxFloat>();
			scrGfxFloat.amplitude = Random.value * 0.3f;
			scrGfxFloat.period = Random.Range(1f, 2f);
			scrGfxFloat.useLocalPos = true;
			num++;
		}
		base.enabled = false;
	}

	public void StartMob()
	{
		int num = 0;
		foreach (SpriteRenderer monster in monsters)
		{
			monster.DOColor(Color.white, appearDuration).SetDelay(appearInterval * (float)num);
			ShortcutExtensions.DOScale(endValue: initialMonsterScales[num], target: monster.transform, duration: appearDuration).SetDelay(appearInterval * (float)num).SetEase(Ease.OutElastic);
			num++;
		}
		base.enabled = true;
	}

	public void StopMob()
	{
		Vector3 vector = new Vector3(initialScale, initialScale, 1f);
		int num = 0;
		foreach (SpriteRenderer monster in monsters)
		{
			monster.DOColor(ADOBase.ClearWhite, appearDuration).SetDelay(appearInterval * (float)num);
			Vector3 vector2 = initialMonsterScales[num];
			ShortcutExtensions.DOScale(endValue: new Vector3(vector.x * vector2.x, vector.y * vector2.y, 1f), target: monster.transform, duration: appearDuration).SetDelay(appearInterval * (float)num).SetEase(Ease.InElastic);
			num++;
		}
		base.enabled = false;
	}

	public void Update()
	{
		offsetTransform.localPosition = new Vector3(positionOffset.x, positionOffset.y, 0f);
		Vector2 vector = Vector2.Lerp(base.transform.position, ADOBase.controller.chosenplanet.currfloor.transform.position, movementLerpT);
		base.transform.MoveXY(vector.x, vector.y);
	}
}
