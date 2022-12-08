using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectCutsceneController : ADOBase
{
	public RectTransform letterboxTop;

	public RectTransform letterboxBottom;

	public Image fade;

	public Transform[] bridgeGems;

	public scrPortal[] bottomPortals;

	public Sequence CheckForCutscene()
	{
		bool flag = false;
		for (int i = 6; i < 11; i++)
		{
			if (Persistence.GetWorldAttempts(i) > 0)
			{
				flag = true;
				break;
			}
		}
		int overallProgressStage = Persistence.GetOverallProgressStage();
		int result;
		bool flag2 = int.TryParse(GCS.worldEntrance, out result);
		if (flag2 && result >= 1 && result <= 5 && overallProgressStage == 3 && !Persistence.GetPlayedFirs5WorldsCutscene())
		{
			Persistence.SetPlayedFirst5WorldsCutscene(played: true);
			Persistence.Save();
			return PassedFirst5Cutscene();
		}
		if (((flag2 && result == 6 && overallProgressStage == 5) || (!flag && overallProgressStage >= 5)) && !Persistence.GetPlayedWorld6Cutscene())
		{
			Persistence.SetPlayedWorld6Cutscene(played: true);
			Persistence.Save();
			return PassedLevel6Cutscene();
		}
		return null;
	}

	public Sequence ShowLetterbox(bool show = true)
	{
		int num = show ? 1 : 0;
		if (show)
		{
			letterboxTop.gameObject.SetActive(value: true);
			letterboxBottom.gameObject.SetActive(value: true);
		}
		return DOTween.Sequence().SetEase(Ease.OutExpo).Join(letterboxTop.DOPivotY(num, 1f))
			.Join(letterboxBottom.DOPivotY(num, 1f));
	}

	public Tween FadeOut(bool fadeOut = true)
	{
		if (fadeOut)
		{
			fade.gameObject.SetActive(value: true);
		}
		return fade.DOFade(fadeOut ? 1 : 0, 0.75f).SetEase(Ease.Linear);
	}

	public Sequence PassedFirst5Cutscene()
	{
		Sequence sequence = DOTween.Sequence();
		Transform transform = ADOBase.controller.camy.transform;
		ADOBase.controller.camy.isMoveTweening = false;
		ADOBase.controller.camy.followMode = false;
		ADOBase.controller.responsive = false;
		Vector3 position = transform.position;
		Sequence sequence2 = DOTween.Sequence();
		Sequence sequence3 = DOTween.Sequence();
		for (int i = 0; i < bridgeGems.Length; i++)
		{
			Transform transform2 = bridgeGems[i];
			Material material = transform2.GetComponent<SpriteRenderer>().material;
			Sequence t = DOTween.Sequence().Append(transform2.DOScale(transform2.localScale * 1.5f, 0.25f).SetEase(Ease.OutSine)).Append(transform2.DOScale(transform2.localScale, 0.25f).SetEase(Ease.InOutSine))
				.AppendInterval(0.3f)
				.SetLoops(3);
			sequence2.Insert((float)i * 0.06f, t);
			sequence3.Join(material.DOFloat(1f, "_Flash", 0f)).Join(material.DOFloat(0f, "_Flash", 1f));
		}
		sequence.AppendInterval(0.25f).Append(ShowLetterbox()).Append(transform.DOMoveX(bridgeGems[2].transform.position.x, 1f).SetEase(Ease.InOutSine))
			.Append(sequence2)
			.Append(sequence3)
			.Append(transform.DOMoveX(ADOBase.controller.chosenplanet.transform.position.x, 1f).SetEase(Ease.InOutSine))
			.AppendCallback(delegate
			{
				ShowLetterbox(show: false);
				ADOBase.controller.camy.isMoveTweening = true;
				ADOBase.controller.camy.followMode = true;
				ADOBase.controller.responsive = true;
				ADOBase.levelSelect.playingCutscene = false;
			});
		return sequence;
	}

	public Sequence PassedLevel6Cutscene()
	{
		Sequence sequence = DOTween.Sequence();
		Transform cam = ADOBase.controller.camy.transform;
		ADOBase.controller.camy.isMoveTweening = false;
		ADOBase.controller.camy.followMode = false;
		ADOBase.controller.responsive = false;
		Vector3 position = cam.position;
		float y = cam.position.y;
		Sequence portalsAppear = DOTween.Sequence().Pause();
		for (int i = 0; i < bottomPortals.Length; i++)
		{
			scrPortal portal = bottomPortals[i];
			Transform parent = portal.sign.transform.parent;
			float y2 = parent.localPosition.y;
			parent.localPosition = parent.localPosition.WithY(-15f);
			DOVirtual.DelayedCall(0f, delegate
			{
				portal.sprPortal.transform.localScale = Vector3.zero;
			});
			portalsAppear.Insert((float)i * 0.85f - 0f, portal.sprPortal.transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack)).Join(parent.DOLocalMoveY(y2, 1.25f).SetEase(Ease.OutBack));
		}
		Sequence s = sequence.AppendInterval(0.25f).Append(ShowLetterbox()).Append(FadeOut());
		Transform target = cam;
		GCNS.WorldData worldData = GCNS.worldData["7"];
		s.Append(target.DOMoveX((float)worldData.jumpPortalPosition.x - 0.5f, 0f)).Join(cam.DOMoveY(-7f, 0f)).AppendCallback(delegate
		{
			Transform target2 = cam;
			GCNS.WorldData worldData2 = GCNS.worldData["12"];
			target2.DOMoveX(worldData2.jumpPortalPosition.x + 4, 6f).SetEase(Ease.Linear);
			cam.DOMoveY(-10f, 0.75f).SetEase(Ease.OutSine);
			portalsAppear.Play();
		})
			.Join(FadeOut(fadeOut: false))
			.AppendInterval(3.5f)
			.Append(FadeOut())
			.AppendCallback(delegate
			{
				cam.DOKill();
				cam.MoveXY(ADOBase.controller.chosenplanet.transform.position.x, -2f);
			})
			.Append(FadeOut(fadeOut: false))
			.Join(cam.DOMoveY(y, 1f).SetEase(Ease.OutSine))
			.AppendCallback(delegate
			{
				ShowLetterbox(show: false);
				ADOBase.controller.camy.isMoveTweening = true;
				ADOBase.controller.camy.followMode = true;
				ADOBase.controller.responsive = true;
				ADOBase.levelSelect.playingCutscene = false;
			});
		return sequence;
	}
}
