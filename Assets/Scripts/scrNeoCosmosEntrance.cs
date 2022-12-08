using DG.Tweening;
using System;
using UnityEngine;

public class scrNeoCosmosEntrance : ADOBase
{
	public scrPortalParticles portalParticles;

	public SpriteRenderer logoEN;

	public SpriteRenderer logoCN;

	public GameObject disabledHolder;

	public float sfxVolume = 1f;

	private FloorRenderer floorRenderer;

	private bool entering;

	private float planetAngle;

	private float planetRadius;

	private float orbitSpeed;

	private Transform orbittingPlanet;

	private void Awake()
	{
		floorRenderer = GetComponent<FloorRenderer>();
		bool flag = Persistence.GetOverallProgressStage() > 0;
		SystemLanguage language = RDString.language;
		bool flag2 = language == SystemLanguage.Chinese || language == SystemLanguage.ChineseSimplified || language == SystemLanguage.ChineseTraditional;
		logoEN.gameObject.SetActive(flag && !flag2);
		logoCN.gameObject.SetActive(flag && flag2);
		disabledHolder.SetActive(flag && !ADOBase.hasTaroDLC);
		base.gameObject.SetActive(flag && ADOBase.hasTaroDLC);
	}

	private void Update()
	{
		float a = floorRenderer.color.a;
		portalParticles.glowSprite.color = portalParticles.glowSprite.color.WithAlpha(a);
		portalParticles.cap.color = portalParticles.cap.color.WithAlpha(a);
		portalParticles.icon.color = portalParticles.color.WithAlpha(a * 0.45f);
		portalParticles.ps.GetComponent<Renderer>().material.SetColor("_Color", Color.gray.WithAlpha(a));
		if (entering)
		{
			planetAngle += Time.deltaTime * orbitSpeed;
			Vector2 b = new Vector2(Mathf.Sin(planetAngle) * planetRadius, Mathf.Cos(planetAngle) * planetRadius);
			orbittingPlanet.position = (Vector2)base.transform.position + b;
		}
	}

	public void Enter()
	{
		entering = true;
		scrPlanet chosenplanet = ADOBase.controller.chosenplanet;
		scrPlanet other = chosenplanet.other;
		orbittingPlanet = chosenplanet.transform;
		planetRadius = other.cosmeticRadius;
		planetAngle = other.cosmeticAngle;
		orbitSpeed = MathF.PI / 2f / (float)ADOBase.conductor.crotchet;
		other.enabled = false;
		chosenplanet.enabled = false;
		ADOBase.controller.responsive = false;
		scrCamera cam = ADOBase.controller.camy;
		cam.followMode = false;
		scrSfx.instance.PlaySfx(SfxSound.EnterNeoCosmos, sfxVolume);
		float duration = 0.5f;
		Ease ease = Ease.OutSine;
		Ease ease2 = Ease.OutSine;
		cam.isMoveTweening = false;
		cam.transform.DOMoveX(base.transform.position.x, duration).SetEase(ease);
		cam.transform.DOMoveY(base.transform.position.y, duration).SetEase(ease);
		DOTween.To(() => cam.zoomSize, delegate(float x)
		{
			cam.zoomSize = x;
		}, 0.5f, duration).SetEase(ease2);
		other.ring.DOFade(0f, duration);
		logoEN.DOFade(0f, duration);
		logoCN.DOFade(0f, duration);
		float duration2 = 0.5f;
		other.transform.DOScale(0f, duration2).SetEase(Ease.InExpo);
		float num = 2f;
		DOTween.To(() => planetRadius, delegate(float x)
		{
			planetRadius = x;
		}, 0f, num).SetEase(Ease.InSine);
		chosenplanet.transform.DOScale(0f, num / 2f).SetDelay(num / 2f).SetEase(Ease.InSine);
		DOTween.To(() => orbitSpeed, delegate(float x)
		{
			orbitSpeed = x;
		}, orbitSpeed * 8f, num).SetEase(Ease.Linear);
		float num2 = 1f;
		float num3 = num * 2f / 4f;
		DOTween.To(() => cam.zoomSize, delegate(float x)
		{
			cam.zoomSize = x;
		}, 0.025f, num2).SetDelay(num3).SetEase(Ease.InBack);
		DOVirtual.DelayedCall(num3 + num2 - 0.66f, delegate
		{
			ADOBase.levelSelect.cutsceneController.FadeOut().OnComplete(delegate
			{
				DOVirtual.DelayedCall(0.15f, delegate
				{
					GCS.sceneToLoad = ADOBase.controller.GetTaroMenuToGoTo();
					ADOBase.controller.StartLoadingScene(WipeDirection.StartsFromRight);
				});
			});
		});
	}

	public void OpenDLCLink()
	{
		Application.OpenURL(ADOBase.appIsInSteamLibrary ? "https://store.steampowered.com/app/1977570/A_Dance_of_Fire_and_Ice__Neo_Cosmos/" : "https://fizzd.itch.io/neo-cosmos");
	}
}
