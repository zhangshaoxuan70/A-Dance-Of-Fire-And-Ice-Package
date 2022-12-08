using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class scrUIController : ADOBase
{
	private const float TransitionDuration = 0.3f;

	public static int deathCounterToFixDotweenBug = 0;

	public static int deathCeiling = 50;

	public static float speedUpAtDeathMax = 8f;

	public static WipeDirection wipeDirection;

	private static scrUIController _instance;

	public Canvas canvas;

	public Text txtLevelName;

	public Text txtOffset;

	public Text txtDebug;

	public Text txtCountdown;

	public Text txtCongrats;

	public Text txtAllStrictClear;

	public Text txtResults;

	public Text txtPercent;

	public Text txtPressToStart;

	public Text txtTryCalibrating;

	public Image transitionPanel;

	public Button autoplayButton;

	public Image mutedImage;

	public Button pauseButton;

	public Sprite[] autoSprites;

	[Header("Difficulty")]
	public RectTransform difficultyContainer;

	public CanvasGroup difficultyFadeContainer;

	public float difficultyAnimDuration;

	public RectTransform leftArrow;

	public RectTransform rightArrow;

	public Button difficultyButtonLeft;

	public Button difficultyButtonRight;

	public Image difficultyImage;

	public Text difficultyText;

	[NonSerialized]
	public DifficultyUIMode difficultyUIMode;

	[Header("Modifiers")]
	public RectTransform modifiersContainer;

	public Image noFailImage;

	private Vector2 leftArrowDefaultPos;

	private Vector2 rightArrowDefaultPos;

	private Tweener wipeToBlack;

	public static scrUIController instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType<scrUIController>();
			}
			return _instance;
		}
	}

	public void Awake()
	{
		leftArrowDefaultPos = leftArrow.anchoredPosition;
		rightArrowDefaultPos = rightArrow.anchoredPosition;
		GCS.difficulty = ((!ADOBase.isEditingLevel) ? Difficulty.Normal : Difficulty.Strict);
		autoplayButton.gameObject.SetActive(value: false);
		difficultyButtonLeft.onClick.AddListener(delegate
		{
			DifficultyArrowPressed(rightPressed: false);
		});
		difficultyButtonRight.onClick.AddListener(delegate
		{
			DifficultyArrowPressed(rightPressed: true);
		});
	}

	public void Start()
	{
		modifiersContainer.gameObject.SetActive(GCS.standaloneLevelMode || ADOBase.isCLS);
	}

	public void PrepareWipeFromBlack()
	{
		transitionPanel.gameObject.SetActive(value: true);
		transitionPanel.color = Color.black;
		RectTransform rectTransform = transitionPanel.rectTransform;
		rectTransform.pivot = new Vector2(0f, 0.5f);
		rectTransform.localScale = Vector3.one;
	}

	public void WipeFromBlack()
	{
		scrSfx.instance.PlaySfx(SfxSound.ScreenWipeIn, 0.5f);
		transitionPanel.gameObject.SetActive(value: true);
		transitionPanel.color = Color.black;
		RectTransform rectTransform = transitionPanel.rectTransform;
		float x = (wipeDirection == WipeDirection.StartsFromLeft) ? 1f : 0f;
		rectTransform.pivot = new Vector2(x, 0.5f);
		rectTransform.localScale = Vector3.one;
		rectTransform.DOKill();
		float duration = GCS.speedTrialMode ? (0.3f / GCS.currentSpeedTrial) : 0.3f;
		rectTransform.DOScaleX(0f, duration).SetEase(Ease.InOutQuint).SetUpdate(isIndependentUpdate: true)
			.OnComplete(delegate
			{
				transitionPanel.gameObject.SetActive(value: false);
			});
	}

	public void WipeToBlack(WipeDirection direction, Action onComplete = null)
	{
		RDUtils.SetGarbageCollectionEnabled(enabled: true);
		scrSfx.instance.PlaySfx(SfxSound.ScreenWipeOut, 0.5f);
		if (wipeToBlack != null && wipeToBlack.active && wipeToBlack.IsPlaying())
		{
			printe("wipe2black canceled");
			return;
		}
		wipeDirection = direction;
		transitionPanel.gameObject.SetActive(value: true);
		transitionPanel.color = Color.black;
		RectTransform rectTransform = transitionPanel.rectTransform;
		float x = (direction == WipeDirection.StartsFromLeft) ? 0f : 1f;
		rectTransform.pivot = new Vector2(x, 0.5f);
		rectTransform.localScale = new Vector3(0f, 1f, 1f);
		rectTransform.DOKill();
		float duration = GCS.speedTrialMode ? (0.3f / GCS.currentSpeedTrial) : 0.3f;
		wipeToBlack = rectTransform.DOScaleX(1f, duration).SetUpdate(isIndependentUpdate: true).SetEase(Ease.InOutQuint)
			.OnComplete(delegate
			{
				if (onComplete != null)
				{
					onComplete();
				}
				else
				{
					DOTween.KillAll();
					ADOBase.LoadScene("scnLoading");
				}
			});
	}

	public void FadeFromBlack(float duration = 1f)
	{
		ResetScale();
		transitionPanel.gameObject.SetActive(value: true);
		transitionPanel.DOKill();
		transitionPanel.DOFade(0f, duration).SetUpdate(isIndependentUpdate: true).OnComplete(delegate
		{
			transitionPanel.gameObject.SetActive(value: false);
		});
	}

	public void FadeToBlack(float duration = 1f)
	{
		ResetScale();
		transitionPanel.gameObject.SetActive(value: true);
		transitionPanel.DOKill();
		transitionPanel.DOFade(1f, duration).SetUpdate(isIndependentUpdate: true).OnComplete(delegate
		{
			transitionPanel.gameObject.SetActive(value: false);
		});
	}

	private void ResetScale()
	{
		transitionPanel.rectTransform.localScale = Vector3.one;
	}

	public void SetToBlack()
	{
		ResetScale();
		transitionPanel.gameObject.SetActive(value: true);
		transitionPanel.DOKill();
		transitionPanel.color = Color.black;
	}

	public void SetToTransparent()
	{
		ResetScale();
		transitionPanel.gameObject.SetActive(value: false);
		transitionPanel.DOKill();
		transitionPanel.color = Color.black;
	}

	private void Update()
	{
		if (GCS.useNoFail != noFailImage.gameObject.activeSelf)
		{
			noFailImage.gameObject.SetActive(GCS.useNoFail);
		}
	}

	public void ToggleAutoplay()
	{
		RDC.auto = !RDC.auto;
	}

	public void ShowDifficultyContainer(DifficultyUIMode mode)
	{
		difficultyUIMode = mode;
		UpdateDifficultyUI(GCS.difficulty);
		if (mode == DifficultyUIMode.DontShow)
		{
			difficultyContainer.gameObject.SetActive(value: false);
			return;
		}
		difficultyContainer.gameObject.SetActive(value: true);
		difficultyContainer.DOKill();
		difficultyContainer.DOAnchorPosX(-20f, difficultyAnimDuration).SetEase(Ease.OutBack);
		difficultyFadeContainer.DOKill();
		difficultyFadeContainer.alpha = 1f;
		difficultyFadeContainer.blocksRaycasts = true;
		difficultyButtonLeft.enabled = true;
		difficultyButtonRight.enabled = true;
		HorizontalLayoutGroup layout = modifiersContainer.GetComponent<HorizontalLayoutGroup>();
		layout.DOKill();
		DOTween.To(() => layout.spacing, delegate(float s)
		{
			layout.spacing = s;
		}, 0f, 0.5f).SetEase(Ease.OutQuad).SetId(layout);
	}

	public void MinimizeDifficultyContainer()
	{
		if (difficultyUIMode != 0)
		{
			difficultyContainer.DOKill();
			difficultyContainer.DOAnchorPosX(335f, difficultyAnimDuration).SetEase(Ease.OutExpo);
			difficultyFadeContainer.DOKill();
			difficultyFadeContainer.DOFade(0f, difficultyAnimDuration).SetEase(Ease.OutExpo);
			difficultyFadeContainer.blocksRaycasts = false;
			difficultyButtonLeft.enabled = false;
			difficultyButtonRight.enabled = false;
			HorizontalLayoutGroup layout = modifiersContainer.GetComponent<HorizontalLayoutGroup>();
			layout.DOKill();
			DOTween.To(() => layout.spacing, delegate(float s)
			{
				layout.spacing = s;
			}, -40f, 0.5f).SetEase(Ease.OutQuad).SetId(layout);
		}
	}

	public void DifficultyArrowPressed(bool rightPressed)
	{
		printe("difficultyArrowPressed: " + rightPressed.ToString());
		bool flag = difficultyUIMode == DifficultyUIMode.ShowNormalAndStrict;
		int num = (difficultyUIMode == DifficultyUIMode.ShowLenientAndNormal) ? 2 : 3;
		RectTransform obj = rightPressed ? rightArrow : leftArrow;
		Vector2 anchoredPosition = rightPressed ? rightArrowDefaultPos : leftArrowDefaultPos;
		Vector2 punch = rightPressed ? new Vector2(5f, 0f) : new Vector2(-5f, 0f);
		obj.DOKill();
		obj.anchoredPosition = anchoredPosition;
		obj.DOPunchAnchorPos(punch, 0.2f, 0);
		int difficulty = (int)GCS.difficulty;
		difficulty += (rightPressed ? 1 : (-1));
		if (difficulty >= num)
		{
			difficulty = (flag ? 1 : 0);
		}
		else if (difficulty < 0)
		{
			difficulty = num - 1;
		}
		if (flag && difficulty == 0)
		{
			difficulty = 2;
		}
		scrSfx.instance.PlaySfx(SfxSound.MenuSquelch, 1.5f);
		UpdateDifficultyUI((Difficulty)difficulty);
	}

	private void UpdateDifficultyUI(Difficulty difficulty)
	{
		if (difficulty == Difficulty.Strict && difficultyUIMode == DifficultyUIMode.ShowLenientAndNormal)
		{
			difficulty = Difficulty.Normal;
		}
		else if (difficulty == Difficulty.Lenient && difficultyUIMode == DifficultyUIMode.ShowNormalAndStrict)
		{
			difficulty = Difficulty.Strict;
		}
		else if (difficultyUIMode == DifficultyUIMode.DontShow)
		{
			difficulty = Difficulty.Normal;
		}
		GCS.difficulty = difficulty;
		difficultyText.text = RDString.Get("enum.Difficulty." + difficulty.ToString());
		difficultyImage.sprite = RDConstants.data.bullseyeSprites[(int)difficulty];
	}
}
