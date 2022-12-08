using DG.Tweening;
using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PracticeTimeline : ADOBase
{
	[Header("References")]
	public RectTransform timeline;

	public RectTransform leftArea;

	public CanvasGroup leftBar;

	public Image leftArrow;

	public RectTransform rightArea;

	public CanvasGroup rightBar;

	public Image rightArrow;

	public RectTransform spoilerArea;

	public RectTransform playBar;

	public RectTransform border;

	public InputField startInput;

	public InputField endInput;

	public Button btnSpeedLeft;

	public Button btnSpeedRight;

	public RectTransform arrowSpeedLeft;

	public RectTransform arrowSpeedRight;

	public Text speedText;

	public AudioSource audioSource;

	public RectTransform timelineContainer;

	public GameObject segmentPrefab;

	[NonSerialized]
	public int practiceStart;

	[NonSerialized]
	public int practiceEnd;

	[Header("Private")]
	private RectTransform rt;

	private int difficultySegments = 100;

	private int levelLength;

	private int spoilerStart;

	private int speedPercent;

	private float[] floorTimings;

	private float levelDur;

	private bool dragging;

	private bool dragEndpoint;

	private bool initialized;

	private float defaultSongPitch = 1f;

	private Image timelineBorderImage;

	private Image leftBarImage;

	private Image rightBarImage;

	private Color barDefaultColor;

	private RectTransform speed;

	private float timelineInitialY;

	private float speedInitialY;

	private const int minimumLength = 5;

	private void Awake()
	{
		rt = GetComponent<RectTransform>();
		leftBarImage = leftBar.GetComponent<Image>();
		rightBarImage = rightBar.GetComponent<Image>();
		barDefaultColor = leftBarImage.color;
		timelineBorderImage = border.GetComponent<Image>();
		timelineInitialY = timeline.anchoredPosition.y;
		speed = speedText.GetComponent<RectTransform>();
		speedInitialY = speed.anchoredPosition.y;
		startInput.onEndEdit.AddListener(delegate(string s)
		{
			if (int.TryParse(s, out int result2))
			{
				practiceStart = result2;
				UpdatePositions();
			}
		});
		endInput.onEndEdit.AddListener(delegate(string s)
		{
			if (int.TryParse(s, out int result))
			{
				practiceEnd = result;
				UpdatePositions(changedEnd: true);
			}
		});
		btnSpeedLeft.onClick.AddListener(delegate
		{
			ChangeSpeed(increase: false);
		});
		btnSpeedRight.onClick.AddListener(delegate
		{
			ChangeSpeed(increase: true);
		});
		Init();
	}

	private void Update()
	{
		rt.sizeDelta = rt.sizeDelta.WithX(rt.rect.width);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, UnityEngine.Input.mousePosition, null, out Vector2 localPoint);
		float num = Mathf.InverseLerp(0f - rt.rect.size.x, rt.rect.size.x, localPoint.x * 2f);
		int num2 = TimeToFloor(num * levelDur);
		if (Input.GetMouseButtonDown(0))
		{
			Rect rect = timelineContainer.rect;
			Vector2 vector = border.rect.size - rt.rect.size;
			rect.size += vector;
			rect.position -= vector / 2f;
			dragging = rect.Contains(localPoint);
			int num3 = (practiceStart + practiceEnd) / 2;
			dragEndpoint = (num2 >= num3);
			ADOBase.controller.pauseMenu.SelectVerticalFixed((!dragEndpoint) ? 1 : 2, dragEndpoint ? 1 : (-1));
		}
		if (dragging)
		{
			if (!dragEndpoint)
			{
				practiceStart = num2;
			}
			else
			{
				practiceEnd = num2;
			}
			UpdatePositions(dragEndpoint);
			SongPlayback(play: false);
		}
		if (Input.GetMouseButtonUp(0))
		{
			if (dragging)
			{
				SongPlayback(play: true);
			}
			dragging = false;
		}
		SongUpdate();
	}

	private void SongPlayback(bool play)
	{
		AudioSource audioSource = this.audioSource;
		if (play)
		{
			audioSource.clip = ADOBase.conductor.song.clip;
			if (!(audioSource.clip == null))
			{
				audioSource.volume = ADOBase.conductor.song.volume;
				audioSource.ignoreListenerPause = true;
				audioSource.Play();
				audioSource.time = (float)ADOBase.lm.listFloors[practiceStart].entryTime;
				float num = (float)speedPercent / 100f;
				audioSource.pitch = num * (GCS.standaloneLevelMode ? ((float)ADOBase.customLevel.levelData.pitch / 100f) : defaultSongPitch);
			}
		}
		else
		{
			audioSource.Pause();
		}
	}

	private void SongUpdate()
	{
		AudioSource audioSource = this.audioSource;
		if (practiceEnd < ADOBase.lm.listFloors.Count)
		{
			float num = (float)ADOBase.lm.listFloors[practiceEnd].entryTime;
			float num2 = (float)ADOBase.lm.listFloors.Last().entryTime;
			if (audioSource.time >= num)
			{
				SongPlayback(play: false);
			}
			playBar.gameObject.SetActive(audioSource.isPlaying);
			playBar.AnchorPosX(timelineContainer.rect.size.x * (audioSource.time / num2));
		}
	}

	public void SetPositions()
	{
		if (GCS.practiceMode)
		{
			float num = (float)speedPercent / 100f;
			while ((ADOBase.lm.listFloors[practiceStart].midSpin || ADOBase.lm.listFloors[practiceStart].freeroam) && practiceStart > 0)
			{
				practiceStart--;
			}
			while ((ADOBase.lm.listFloors[practiceEnd].midSpin || ADOBase.lm.listFloors[practiceEnd].freeroam) && practiceEnd < ADOBase.lm.listFloors.Count - 1)
			{
				practiceEnd++;
			}
			int num2 = practiceEnd - practiceStart;
			if (GCS.checkpointNum != practiceStart || GCS.practiceLength != num2 || GCS.currentSpeedTrial != num)
			{
				ADOBase.controller.pauseMenu.requireRestart = true;
				GCS.checkpointNum = practiceStart;
				GCS.practiceLength = num2;
				GCS.nextSpeedRun = num;
				GCS.currentSpeedTrial = num;
			}
		}
	}

	public void ChangeSpeed(bool increase)
	{
		int num = (UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift)) ? 1 : 5;
		int num2 = increase ? 1 : (-1);
		speedPercent += num * num2;
		RectTransform target = increase ? arrowSpeedRight : arrowSpeedLeft;
		target.DOComplete(withCallbacks: true);
		target.DOPunchAnchorPos(Vector2.zero.WithX(1f * (float)num2), 0.2f, 0).SetUpdate(isIndependentUpdate: true);
		scrSfx.instance.PlaySfx(SfxSound.MenuSquelch, 1.5f);
		UpdateSpeed();
		SongPlayback(play: true);
	}

	private void UpdateSpeed()
	{
		speedPercent = Mathf.Clamp(speedPercent, 20, 1000);
		speedText.text = speedPercent.ToString() + "%";
	}

	private int TimeToFloor(float pos)
	{
		return floorTimings.TakeWhile((float t) => t < pos).Count();
	}

	private float FloorToTime(int floor)
	{
		return floorTimings[Mathf.Clamp(floor, 0, levelLength - 1)];
	}

	public void UpdatePositions(bool changedEnd = false)
	{
		if (!changedEnd)
		{
			practiceStart = Mathf.Clamp(practiceStart, 0, levelLength - 1 - 5);
			practiceStart = Mathf.Min(practiceStart, spoilerStart - 5);
			practiceEnd = Mathf.Max(practiceEnd, practiceStart + 5);
		}
		else
		{
			practiceEnd = Mathf.Clamp(practiceEnd, 5, levelLength - 1);
			practiceEnd = Mathf.Min(practiceEnd, spoilerStart);
			practiceStart = Mathf.Min(practiceStart, practiceEnd - 5);
		}
		leftArea.SizeDeltaX(FloorToTime(practiceStart) / levelDur * rt.rect.size.x);
		rightArea.SizeDeltaX((1f - FloorToTime(practiceEnd) / levelDur) * rt.rect.size.x);
		startInput.text = practiceStart.ToString();
		endInput.text = practiceEnd.ToString();
		StartCoroutine(_003CUpdatePositions_003Eg__FocusHack_007C50_0());
	}

	public void Init()
	{
		if ((!ADOBase.controller.gameworld && (ADOBase.controller.gameworld || (!ADOBase.controller.currFloor.freeroam && !ADOBase.controller.currFloor.freeroamGenerated))) || !GCS.practiceMode)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		base.gameObject.SetActive(value: true);
		playBar.gameObject.SetActive(value: false);
		floorTimings = (from f in ADOBase.lm.listFloors
			select (float)f.entryTime).ToArray();
		levelLength = floorTimings.Count();
		levelDur = floorTimings.Last();
		practiceStart = GCS.checkpointNum;
		practiceEnd = practiceStart + GCS.practiceLength;
		speedPercent = Mathf.RoundToInt(GCS.currentSpeedTrial * 100f);
		if (ADOBase.controller.isbosslevel && !GCS.standaloneLevelMode && !Persistence.IsWorldComplete(scrController.currentWorld))
		{
			float percentCompletion = Persistence.GetPercentCompletion(scrController.currentWorld);
			spoilerStart = Mathf.Max(20, Mathf.FloorToInt(percentCompletion * (float)levelLength) + 10);
		}
		else
		{
			spoilerStart = levelLength;
		}
		UpdatePositions();
		UpdateSpeed();
		if (initialized)
		{
			return;
		}
		initialized = true;
		float[] array = new float[difficultySegments];
		float a = 999f;
		float num = 0f;
		float num2 = levelDur / (float)difficultySegments;
		int i = 0;
		float num3 = 0f;
		int num4 = 0;
		for (; i < levelLength - 1; i++)
		{
			float num5 = floorTimings[i];
			float num6 = (float)(num4 + 1) * num2;
			if (num5 > num6)
			{
				a = Mathf.Min(a, num3);
				num = Mathf.Max(num, num3);
				array[num4] = num3;
				num4++;
				num3 = 0f;
			}
			else if (num5 >= (float)num4 * num2)
			{
				num3 += 1f;
			}
		}
		float num7 = timelineContainer.rect.size.x / (float)difficultySegments;
		for (int j = 0; j < difficultySegments; j++)
		{
			float value = array[j];
			float t = Mathf.InverseLerp(a, num, value);
			float y = Mathf.Lerp(2f, timelineContainer.rect.height - 2f, t);
			RectTransform component = UnityEngine.Object.Instantiate(segmentPrefab, timelineContainer).GetComponent<RectTransform>();
			component.anchoredPosition = Vector3.zero.WithX((float)j * num7);
			component.sizeDelta = new Vector2(num7, y);
			if ((float)j * num2 >= FloorToTime(spoilerStart))
			{
				component.GetComponent<Image>().color = Color.white.WithAlpha(0.25f);
			}
		}
	}

	public void Select(int index, int direction)
	{
		timelineBorderImage.color = new Color(1f, 1f, 1f, 0.5f);
		leftBarImage.DOKill();
		leftArrow.DOKill();
		leftBarImage.color = barDefaultColor;
		leftBar.alpha = 0.6f;
		leftArrow.color.WithAlpha(0.6f);
		leftArrow.gameObject.SetActive(value: false);
		rightBarImage.DOKill();
		rightArrow.DOKill();
		rightBarImage.color = barDefaultColor;
		rightBar.alpha = 0.6f;
		rightArrow.color.WithAlpha(0.6f);
		rightArrow.gameObject.SetActive(value: false);
		speedText.color = new Color(1f, 1f, 1f, 0.6f);
		bool flag = index == 1;
		bool flag2 = index == 2;
		if (flag | flag2)
		{
			if ((flag && direction == 1) || (flag2 && direction == -1))
			{
				Jump(timeline, timelineInitialY);
			}
			timelineBorderImage.color = Color.white;
			CanvasGroup canvasGroup = flag ? leftBar : rightBar;
			Image target = flag ? leftBarImage : rightBarImage;
			Image image = flag ? leftArrow : rightArrow;
			canvasGroup.alpha = 1f;
			target.DOColor(Color.white, 0.5f).SetEase(Ease.InQuint).SetLoops(-1, LoopType.Yoyo)
				.SetUpdate(isIndependentUpdate: true);
			image.DOColor(Color.white, 0.5f).SetEase(Ease.InQuint).SetLoops(-1, LoopType.Yoyo)
				.SetUpdate(isIndependentUpdate: true);
			image.gameObject.SetActive(value: true);
		}
		else if (index == 3)
		{
			Jump(speed, speedInitialY);
			speedText.color = Color.white;
		}
	}

	private void Jump(RectTransform rt, float initialY)
	{
		rt.DOKill();
		rt.anchoredPosition = rt.anchoredPosition.WithY(initialY);
		rt.DOAnchorPosY(initialY + 4f, 0.1f).SetEase(Ease.OutQuart).SetUpdate(isIndependentUpdate: true)
			.SetLoops(2, LoopType.Yoyo);
	}

	[CompilerGenerated]
	private IEnumerator _003CUpdatePositions_003Eg__FocusHack_007C50_0()
	{
		yield return new WaitForEndOfFrame();
		startInput.interactable = false;
		startInput.interactable = true;
		endInput.interactable = false;
		endInput.interactable = true;
	}
}
