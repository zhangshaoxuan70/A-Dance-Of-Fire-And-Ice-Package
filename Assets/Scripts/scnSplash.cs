using DG.Tweening;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class scnSplash : ADOBase
{
	private static readonly List<string> UNSTABLE_BRANCHES = new List<string>
	{
		"alpha",
		"stardust",
		"frontline"
	};

	public Image fade;

	public float fadeDuration = 0.3f;

	public Ease fadeEase = Ease.Linear;

	public Text healthGameAdvice;

	public float healthGameAdviceDuration;

	public Text alphaWarning;

	public Text translationWarning;

	public float alphaWarningDuration;

	public VideoPlayer player;

	public GameObject tapjoy;

	private void Awake()
	{
		ADOBase.Startup();
	}

	private void Start()
	{
		StartCoroutine(ShowAlphaWarningCoroutine());
	}

	private void Update()
	{
		bool isMobile = ADOBase.isMobile;
	}

	private IEnumerator ShowAlphaWarningCoroutine()
	{
		yield return null;
		yield return null;
		string language = Persistence.GetLanguage();
		bool isRomanian = language == SystemLanguage.Romanian.ToString();
		if (SteamIntegration.Instance.initialized && SteamApps.GetCurrentBetaName(out string pchName, 20) && UNSTABLE_BRANCHES.Contains(pchName) && !isRomanian)
		{
			alphaWarning.enabled = true;
			alphaWarning.text = alphaWarning.text.Replace("[branch]", pchName);
			alphaWarning.color = Color.clear;
			alphaWarning.DOColor(Color.white, 0.5f);
			yield return new WaitForSeconds(0.5f);
			float startTime2 = Time.unscaledTime;
			while (Time.unscaledTime < startTime2 + alphaWarningDuration && !Input.anyKeyDown)
			{
				yield return null;
			}
			alphaWarning.DOColor(Color.clear, 0.5f);
			yield return new WaitForSeconds(0.5f);
		}
		if (isRomanian)
		{
			translationWarning.enabled = true;
			translationWarning.color = Color.clear;
			translationWarning.DOColor(Color.white, 0.5f);
			yield return new WaitForSeconds(0.5f);
			float startTime2 = Time.unscaledTime;
			while (Time.unscaledTime < startTime2 + alphaWarningDuration && !Input.anyKeyDown)
			{
				yield return null;
			}
			translationWarning.DOColor(Color.clear, 0.5f);
			yield return new WaitForSeconds(0.5f);
		}
		GoToMenu();
	}

	private void ShowHealthGameAdvice()
	{
		healthGameAdvice.enabled = true;
		healthGameAdvice.color = Color.white.WithAlpha(0f);
		DOTween.Sequence().AppendInterval(1f).Append(healthGameAdvice.DOColor(Color.white, 0.5f))
			.SetUpdate(isIndependentUpdate: true)
			.AppendInterval(healthGameAdviceDuration)
			.Append(healthGameAdvice.DOColor(Color.white.WithAlpha(0f), 0.5f))
			.AppendCallback(delegate
			{
				GoToMenu();
			});
		base.enabled = false;
	}

	private void GoToMenu()
	{
		fade.DOFade(1f, fadeDuration).SetUpdate(isIndependentUpdate: true).SetEase(fadeEase)
			.OnComplete(delegate
			{
				ADOBase.GoToLevelSelect();
			});
	}
}
