using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LaserSpawner : MonoBehaviour
{
	public List<GameObject> planets;

	public GameObject beamTop;

	public Mawaru_Sprite beamPrepare;

	public List<GameObject> beamParts;

	public List<Vector3> beamPartScale;

	private float beamWidth;

	private float planetScale;

	public bool active;

	public bool killsFloors;

	public bool killsPlanets;

	private Color whiteClear = new Color(1f, 1f, 1f, 0f);

	private ContactFilter2D cfPlanets;

	private ContactFilter2D cfFloors;

	private List<Collider2D> planetColls = new List<Collider2D>();

	private List<Collider2D> floorColls = new List<Collider2D>();

	private Collider2D coll;

	private scrConductor conductor;

	private scrController controller;

	public Dictionary<scrFloor, Mawaru_Sprite> floorWarns;

	private float ang;

	public bool killedPlanet;

	public Sequence moveAnim;

	private float pulse;

	private Vector3 scatter = Vector3.zero;

	private void Awake()
	{
		ang = MathF.PI / 180f * base.transform.eulerAngles.z;
		killedPlanet = false;
		controller = scrController.instance;
		conductor = scrConductor.instance;
		coll = GetComponent<BoxCollider2D>();
		for (int i = 0; i < 3; i++)
		{
			planets[i].SetActive(value: false);
		}
		beamPrepare.render.DOColor(whiteClear, 0f);
		for (int j = 0; j < beamParts.Count; j++)
		{
			beamPartScale.Add(beamParts[j].transform.localScale);
		}
		cfPlanets = default(ContactFilter2D);
		cfPlanets.SetLayerMask(LayerMask.GetMask("Planet"));
		cfPlanets.useLayerMask = true;
		cfPlanets.useTriggers = true;
		cfFloors = default(ContactFilter2D);
		cfFloors.SetLayerMask(LayerMask.GetMask("Floor"));
		cfFloors.useLayerMask = true;
		cfFloors.useTriggers = true;
	}

	public void Prepare(float startup)
	{
		killedPlanet = false;
		for (int i = 0; i < 3; i++)
		{
			planets[i].SetActive(value: true);
		}
		beamPrepare.render.DOColor(Color.white, startup);
		DOTween.To(() => planetScale, delegate(float x)
		{
			planetScale = x;
		}, 1f, startup * 0.5f).SetEase(Ease.OutSine);
	}

	public void Fire()
	{
		active = true;
		DOTween.Sequence().Append(DOTween.To(() => planetScale, delegate(float x)
		{
			planetScale = x;
		}, 1.6f, 0f)).Append(DOTween.To(() => planetScale, delegate(float x)
		{
			planetScale = x;
		}, 1f, 0.4f).SetEase(Ease.OutSine));
		DOTween.Sequence().Append(DOTween.To(() => beamWidth, delegate(float x)
		{
			beamWidth = x;
		}, 1.6f, 0f)).Append(DOTween.To(() => beamWidth, delegate(float x)
		{
			beamWidth = x;
		}, 1f, 0.4f).SetEase(Ease.OutSine));
	}

	public void End(float endtime)
	{
		active = false;
		beamPrepare.render.DOColor(whiteClear, endtime);
		DOTween.To(() => planetScale, delegate(float x)
		{
			planetScale = x;
		}, 0f, endtime * 0.5f).SetEase(Ease.OutSine).OnComplete(delegate
		{
			for (int i = 0; i < 3; i++)
			{
				planets[i].SetActive(value: false);
			}
		});
		DOTween.To(() => beamWidth, delegate(float x)
		{
			beamWidth = x;
		}, 0f, endtime * 0.25f).SetEase(Ease.OutExpo);
	}

	private void Update()
	{
		if (active && killsPlanets)
		{
			coll.OverlapCollider(cfPlanets, planetColls);
			foreach (Collider2D planetColl in planetColls)
			{
				if (planetColl != null)
				{
					scrPlanet component = planetColl.gameObject.GetComponent<scrPlanet>();
					if (component != null && !component.dead)
					{
						if (!controller.freeroamInvulnerability)
						{
							component.Die();
							if (!killedPlanet)
							{
								controller.FailByHitbox();
								killedPlanet = true;
							}
						}
						else
						{
							if (!killedPlanet)
							{
								killedPlanet = true;
							}
							scrSfx.instance.PlaySfx(SfxSound.ModifierActivate, 0.8f);
							End(8f * (float)conductor.crotchet / conductor.song.pitch);
							controller.mistakesManager.AddHit(HitMargin.FailMiss);
							controller.chosenplanet.MarkFail()?.BlinkForSeconds(3f);
							Vector3 position = controller.chosenplanet.transform.position;
							position.y += 1f;
							controller.ShowHitText(HitMargin.FailMiss, position, 0f);
							if (moveAnim != null)
							{
								moveAnim.Kill();
							}
						}
					}
					scrSpike component2 = planetColl.gameObject.GetComponent<scrSpike>();
					if (component2 != null && !component2.destroyed)
					{
						component2.Die();
						scatter = Vector3.right * 2f * Mathf.Sin(ang) + Vector3.down * 2f * Mathf.Cos(ang) + Vector3.right * UnityEngine.Random.Range(-0.5f, 0.5f) + Vector3.up * UnityEngine.Random.Range(-0.5f, 0.5f);
						component2.ballSprite.render.DOColor(whiteClear, (float)conductor.crotchet / conductor.song.pitch).SetEase(Ease.Linear);
						component2.transform.DOLocalMove(scatter, (float)conductor.crotchet / conductor.song.pitch).SetEase(Ease.OutCubic).SetRelative(isRelative: true);
						component2.transform.DORotate(Vector3.forward * UnityEngine.Random.Range(-90f, 90f), (float)conductor.crotchet / conductor.song.pitch, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetRelative(isRelative: true);
					}
				}
			}
		}
		if (active && killsFloors)
		{
			coll.OverlapCollider(cfFloors, floorColls);
			foreach (Collider2D floorColl in floorColls)
			{
				if (floorColl != null)
				{
					scrFloor component3 = floorColl.transform.parent.gameObject.GetComponent<scrFloor>();
					if (component3 != null && component3.isLandable && (!component3.freeroamGenerated || (component3.freeroamGenerated && !controller.freeroamInvulnerability)))
					{
						if (floorWarns.ContainsKey(component3))
						{
							DOTween.Kill(floorWarns[component3]);
							floorWarns[component3].transform.DOScale(Vector3.zero, 0f);
							floorWarns[component3].transform.DOLocalMoveY(-9999f, 0f);
						}
						DOTween.Kill(component3);
						foreach (scrPlanet dummyPlanet in component3.dummyPlanets)
						{
							if (dummyPlanet.onlyRing)
							{
								dummyPlanet.Destroy();
							}
							else
							{
								dummyPlanet.Die();
							}
						}
						component3.isLandable = false;
						component3.TweenOpacity(0f, (float)conductor.crotchet / conductor.song.pitch);
						scatter = Vector3.right * 2f * Mathf.Sin(ang) + Vector3.down * 2f * Mathf.Cos(ang) + Vector3.right * UnityEngine.Random.Range(-0.5f, 0.5f) + Vector3.up * UnityEngine.Random.Range(-0.5f, 0.5f);
						component3.transform.DOLocalMove(scatter, (float)conductor.crotchet / conductor.song.pitch).SetEase(Ease.OutCubic).SetRelative(isRelative: true);
						component3.transform.DORotate(Vector3.forward * UnityEngine.Random.Range(-90f, 90f), (float)conductor.crotchet / conductor.song.pitch, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetRelative(isRelative: true);
					}
				}
			}
		}
		pulse = 0.06f * Mathf.Sin(Time.time * 8f * MathF.PI * 2f);
		for (int i = 0; i < beamParts.Count; i++)
		{
			beamParts[i].transform.localScale = Vector3.right * beamWidth * beamPartScale[i].x + Vector3.right * pulse * beamWidth + Vector3.up * beamPartScale[i].y + Vector3.forward * beamPartScale[i].z;
		}
		beamTop.transform.localScale = Vector3.right * beamWidth * 1.2f + Vector3.up * (0.5f + beamWidth * 0.5f) * 1.2f + Vector3.forward * 1.2f;
		for (int j = 0; j < 3; j++)
		{
			planets[j].transform.localScale = Vector3.one * planetScale * 0.8f + Vector3.one * planetScale * 0.2f * Mathf.Cos((Time.time + (float)j / 3f) * 2f * MathF.PI);
			planets[j].transform.localPosition = Vector3.right * 0.9f * Mathf.Sin((Time.time + (float)j / 3f) * 2f * MathF.PI) + Vector3.up * 0.3f * Mathf.Cos((Time.time + (float)j / 3f) * 2f * MathF.PI) + Vector3.up * 0.8f;
		}
	}
}
