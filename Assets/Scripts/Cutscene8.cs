using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Cutscene8 : TaroCutsceneScript
{
	private List<Transform> charAdd = new List<Transform>();

	private List<Vector3> charStartPos = new List<Vector3>();

	public TextMeshPro exitText;

	public Crack crack;

	private bool shouldDisplay;

	public scrFloor triggerFloor;

	private bool triggered;

	public GameObject scene8Marker;

	public RectTransform creditsContent;

	public RectTransform creditsContentCopy;

	private bool animating;

	private int exWorldsComplete;

	private float floating = 0.08f;

	private new void Awake()
	{
		base.Awake();
		if (RDC.skipCutscenes)
		{
			shouldDisplay = false;
		}
		characters[0].SetState(0);
		characters[0].render.enabled = false;
		characters[0].render.material.DOColor(whiteClear, 0f).SetEase(Ease.Linear);
		runnables["ShowCrack"] = ShowCrack;
		runnables["BackFromCrack"] = BackFromCrack;
		runnables["CrackState0"] = CrackState0;
		runnables["CrackState1"] = CrackState1;
		runnables["CrackState2"] = CrackState2;
		runnables["CrackState3"] = CrackState3;
		runnables["EndScene"] = FinishCrackScene;
		bool shouldDisplay2 = shouldDisplay;
		charAdd.Add(new GameObject().transform);
		charAdd[0].position = Vector3.down;
		charStartPos.Add(characters[0].transform.localPosition);
	}

	private void PlayCutscene()
	{
		ADOBase.controller.isCutscene = true;
		scene_ended = false;
		ADOBase.controller.MoveCameraToObject(scene8Marker, 1f, Ease.Linear);
		exitText.transform.DOScale(0f, 0f);
		runnables["Scene2"] = Scene2;
		runnables["OnComplete"] = FinishCutscene;
		dialog.Add(RDString.Get("neoCosmosStory.4.1"));
		dialog.Add(RDString.Get("neoCosmosStory.4.2"));
		dialog.Add(RDString.Get("neoCosmosStory.4.3"));
		dialog.Add(RDString.Get("neoCosmosStory.4.4"));
		dialog.Add(RDString.Get("neoCosmosStory.4.5"));
		dialog.Add(RDString.Get("neoCosmosStory.4.6"));
		dialog.Add(RDString.Get("neoCosmosStory.4.7"));
		dialog.Add(RDString.Get("neoCosmosStory.4.8"));
		dialog.Add(RDString.Get("neoCosmosStory.4.9"));
		dialog.Add(RDString.Get("neoCosmosStory.4.10"));
		dialog.Add(RDString.Get("neoCosmosStory.4.11"));
		dialog.Add(RDString.Get("neoCosmosStory.4.12"));
		dialog.Add(RDString.Get("neoCosmosStory.4.13"));
		dialog.Add(RDString.Get("neoCosmosStory.4.14"));
		dialog.Add(RDString.Get("neoCosmosStory.4.15"));
		StartScene();
	}

	private void Scene2()
	{
		scrUIController.instance.txtPressToStart.transform.DOLocalMoveY(9999f, 0f).SetRelative(isRelative: true);
		Persistence.SetTaroStoryProgress(7);
		Persistence.Save();
		CharFadeIn(characters[0], 0.5f);
		charAdd[0].DOMoveY(0f, 0.5f).SetEase(Ease.OutCubic);
		creditsContent.transform.DOLocalMoveX(1200f, 1f).SetEase(Ease.Linear);
		creditsContentCopy.transform.DOLocalMoveX(1200f, 1f).SetEase(Ease.Linear);
	}

	private void FinishCutscene()
	{
		ADOBase.controller.MoveCameraToPlayer(1f, Ease.Linear);
		scrUIController.instance.txtPressToStart.transform.DOLocalMoveY(-9999f, 0f).SetRelative(isRelative: true);
		CharFadeOut(characters[0], 0.5f);
		charAdd[0].DOMoveY(-1f, 0.5f).SetEase(Ease.InCubic);
		exitText.transform.DOScale(1f, 0f);
		creditsContent.transform.DOLocalMoveX(0f, 1f).SetEase(Ease.Linear);
		creditsContentCopy.transform.DOLocalMoveX(0f, 1f).SetEase(Ease.Linear);
	}

	private void nop()
	{
	}

	private void FinishCrackScene()
	{
		AdvanceText();
		crack.Localize();
	}

	private void ShowCrack()
	{
		ADOBase.controller.MoveCameraToObject(crack.gameObject, 1f, Ease.InOutCubic);
	}

	private void CrackState0()
	{
		crack.gameObject.SetActive(value: true);
		scrSfx.instance.PlaySfxPitch(SfxSound.MenuCrack, 1f, 1.44f);
		crack.SetState(0);
		ADOBase.controller.ScreenShake(0.2f, 0.5f);
	}

	private void CrackState1()
	{
		crack.gameObject.SetActive(value: true);
		scrSfx.instance.PlaySfxPitch(SfxSound.MenuCrack, 1f, 1.2f);
		crack.SetState(1);
		ADOBase.controller.ScreenShake(0.2f, 0.5f);
	}

	private void CrackState2()
	{
		crack.gameObject.SetActive(value: true);
		scrSfx.instance.PlaySfxPitch(SfxSound.MenuCrack);
		crack.SetState(2);
		ADOBase.controller.ScreenShake(0.2f, 0.5f);
	}

	private void CrackState3()
	{
		crack.gameObject.SetActive(value: true);
		scrSfx.instance.PlaySfx(SfxSound.MenuCrackFinal);
		crack.SetState(3);
		ADOBase.controller.ScreenShake(0.2f, 0.5f);
		if (RDString.language == SystemLanguage.English)
		{
			crack.TextSequence();
		}
	}

	private void BackFromCrack()
	{
		ADOBase.controller.MoveCameraToPlayer(1f, Ease.InOutCubic);
	}

	private void CutsceneKillPlayer()
	{
		printe("(kills u cutely)");
		ADOBase.controller.planetList[0].Die();
		ADOBase.controller.FailAction();
	}

	private void Start()
	{
		exWorldsComplete = 0;
		exWorldsComplete += (Persistence.IsWorldComplete("T1EX") ? 1 : 0);
		exWorldsComplete += (Persistence.IsWorldComplete("T2EX") ? 1 : 0);
		exWorldsComplete += (Persistence.IsWorldComplete("T3EX") ? 1 : 0);
		exWorldsComplete += (Persistence.IsWorldComplete("T4EX") ? 1 : 0);
		printe(string.Format("story prog: {0} ex prog: {1} world EX1 cleared: {2}", Persistence.GetTaroStoryProgress(), Persistence.GetTaroEXProgress(), Persistence.IsWorldComplete("T1EX")));
		if (Persistence.GetTaroEXProgress() == 0)
		{
			crack.gameObject.SetActive(value: false);
		}
		if (Persistence.GetTaroEXProgress() > 0)
		{
			crack.SetState(Persistence.GetTaroEXProgress() - 1);
		}
		if (Persistence.GetTaroEXProgress() == 4)
		{
			crack.FadeText(0f);
		}
		if (exWorldsComplete > Persistence.GetTaroEXProgress())
		{
			CrackScene(exWorldsComplete - 1);
		}
		Persistence.SetTaroEXProgress(exWorldsComplete);
		Persistence.Save();
	}

	private void CrackScene(int val)
	{
		runnables["OnComplete"] = nop;
		ADOBase.controller.isCutscene = true;
		displayBox = false;
		canSkip = false;
		canAdvance = false;
		bool flag = RDString.language == SystemLanguage.English;
		float num = (val == 3 && flag) ? 3f : 1f;
		dialog.Add($"..`p,.5;..`f,ShowCrack;..`p,2;..`f,CrackState{val};..`p,{num};..`f,BackFromCrack;..`p,0.1;..`f,EndScene;");
		crack.tbcText.text = "";
		StartScene();
	}

	private new void Update()
	{
		if (!triggered && Persistence.GetTaroStoryProgress() == 6 && ADOBase.controller.chosenplanet.currfloor == triggerFloor)
		{
			triggered = true;
			PlayCutscene();
		}
		if (animating)
		{
			characters[0].transform.localPosition = charStartPos[0] + floating * Vector3.up * Mathf.Sin(Time.time * 0.5f * MathF.PI) + charAdd[0].position;
		}
		base.Update();
	}
}
