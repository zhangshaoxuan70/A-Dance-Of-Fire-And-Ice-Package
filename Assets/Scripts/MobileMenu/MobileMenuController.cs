using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MobileMenu
{
	public class MobileMenuController : ADOBase
	{
		public MobileMenuMap map;

		public int selectedScreenIndex;

		public MobileMenuGroup currentGroup;

		[Header("References")]
		public MobileMenuSublevelBrowserGenerator sublevelBrowser;

		public MobileMenuGrabController grabController;

		public MobileMenuArrow buttonUp;

		public MobileMenuArrow buttonDown;

		public MobileMenuArrow buttonLeft;

		public MobileMenuArrow buttonRight;

		public RectTransform topButtonsContainer;

		public RectTransform bottomButtonsContainer;

		public CanvasGroup buttonsCanvasGroup;

		public Button buttonSpeedTrial;

		public Button buttonEX;

		public Text caption;

		[Header("Background Elements")]
		public SpriteRenderer[] backgroundClouds;

		public SpriteRenderer background;

		public SpriteRenderer backgroundGradient;

		private bool expandedLevel;

		private bool speedTrial;

		private bool movingThisFrame;

		private float lastAspectRatio;

		private Sequence screenTransition;

		public static MobileMenuController instance;

		private Vector2 touchStartPos;

		private Vector2 cameraOrigPos;

		private bool dragIsVertical;

		private bool dragging;

		private bool dragCanBegin;

		private const float dragMinDistance = 5f;

		public MobileMenuScreen currentScreen => currentGroup.visibleScreens[selectedScreenIndex];

		private void Awake()
		{
			instance = this;
			buttonUp.button.onClick.AddListener(delegate
			{
				MoveInDirection(MoveDirection.Up);
			});
			buttonDown.button.onClick.AddListener(delegate
			{
				MoveInDirection(MoveDirection.Down);
			});
			buttonLeft.button.onClick.AddListener(delegate
			{
				MoveInDirection(MoveDirection.Left);
			});
			buttonRight.button.onClick.AddListener(delegate
			{
				MoveInDirection(MoveDirection.Right);
			});
			bool active = Persistence.GetOverallProgressStage() >= 5;
			buttonSpeedTrial.onClick.AddListener(delegate
			{
				ToggleSpeedTrial();
			});
			buttonSpeedTrial.gameObject.SetActive(active);
			grabController = new MobileMenuGrabController();
		}

		public void LoadMap(string mapName)
		{
			map = new MobileMenuMap(mapName);
			foreach (string key in map.portalLUT.Keys)
			{
				sublevelBrowser.GenerateSubmenu(key);
			}
			SetSpeedTrial(GCS.speedTrialMode);
		}

		public void JumpToMenuEntrance()
		{
			string worldEntrance = GCS.worldEntrance;
			MobileMenuScreen screen = map.rootGroup.visibleScreens[0];
			if (worldEntrance != null && map.portalLUT.ContainsKey(worldEntrance))
			{
				screen = map.portalLUT[worldEntrance];
			}
			JumpToScreen(screen, instant: true);
		}

		public void JumpToScreen(MobileMenuScreen screen = null, bool instant = false)
		{
			if (screen == null)
			{
				if (currentGroup == null)
				{
					return;
				}
				screen = currentScreen;
			}
			screenTransition?.Kill();
			screenTransition = DOTween.Sequence();
			float duration = instant ? 0f : 0.25f;
			Vector3 endValue = screen.transform.position.WithZ(ADOBase.controller.camy.transform.position.z);
			screenTransition.Join(ADOBase.controller.camy.transform.DOMove(endValue, duration).SetEase(Ease.OutSine));
			MobileMenuGroup.BackgroundTheme theme = screen.parentGroup.theme.WithDefaults(map.rootGroup.theme);
			screenTransition.Join(DoTheme(theme, instant));
			screenTransition.Play();
			screenTransition.SetUpdate(UpdateType.Manual);
			string text = "";
			if (screen is MobileMenuPortal)
			{
				MobileMenuPortal mobileMenuPortal = screen as MobileMenuPortal;
				text = ((!mobileMenuPortal.portal.locked) ? RDString.Get("world" + mobileMenuPortal.world + ".description") : RDString.Get("levelSelect.locked"));
			}
			caption.text = text;
			caption.raycastTarget = text.Contains("<a href");
			if (currentGroup != null)
			{
				if (screen == currentScreen)
				{
					return;
				}
				currentScreen.Select(select: false, instant);
			}
			currentGroup = screen.parentGroup;
			selectedScreenIndex = currentGroup.visibleScreens.IndexOf(screen);
			screen.Select(select: true, instant);
			movingThisFrame = true;
			RefreshButtons();
		}

		public void Enable(bool enable)
		{
			base.enabled = enable;
			ShowButtons(enable);
		}

		private Tween DoTheme(MobileMenuGroup.BackgroundTheme theme, bool instant = false)
		{
			Sequence sequence = DOTween.Sequence();
			float duration = instant ? 0f : 0.25f;
			if (speedTrial)
			{
				MobileMenuGroup.BackgroundTheme backgroundTheme = default(MobileMenuGroup.BackgroundTheme);
				backgroundTheme.backgroundColor = "67000045".HexToColor();
				backgroundTheme.gradientColor = "822D5480".HexToColor();
				backgroundTheme.cloudColor = "3F283817".HexToColor();
				theme = backgroundTheme;
			}
			SpriteRenderer[] array = backgroundClouds;
			foreach (SpriteRenderer target in array)
			{
				sequence.Join(target.DOColor(theme.cloudColor.Value, duration).SetEase(Ease.OutSine));
			}
			sequence.Join(background.DOColor(theme.backgroundColor.Value, duration).SetEase(Ease.OutSine));
			sequence.Join(backgroundGradient.DOColor(theme.gradientColor.Value, duration).SetEase(Ease.OutSine));
			return sequence;
		}

		public void MoveInDirection(MoveDirection direction, bool playSound = true)
		{
			if (expandedLevel)
			{
				return;
			}
			Vector2Int vector = direction.GetVector();
			MobileMenuGroup mobileMenuGroup = currentGroup;
			int num = selectedScreenIndex;
			bool flag = false;
			if (vector.x != 0)
			{
				num += vector.x;
				if (num < 0 || num > currentGroup.visibleScreens.Count - 1)
				{
					if (mobileMenuGroup.linkedGroup.ContainsKey(direction))
					{
						flag = true;
						mobileMenuGroup = mobileMenuGroup.linkedGroup[direction];
						num = ((direction != MoveDirection.Right) ? (mobileMenuGroup.visibleScreens.Count - 1) : 0);
					}
				}
				else
				{
					flag = true;
				}
			}
			else if (selectedScreenIndex == 0 && currentGroup.linkedGroup.ContainsKey(direction))
			{
				mobileMenuGroup = mobileMenuGroup.linkedGroup[direction];
				flag = true;
			}
			if (flag)
			{
				JumpToScreen(mobileMenuGroup.visibleScreens[num]);
			}
			else
			{
				JumpToScreen();
			}
			if (playSound)
			{
				bool flag2 = direction == MoveDirection.Right || direction == MoveDirection.Up;
				scrSfx.instance.PlaySfx(flag2 ? SfxSound.MobileButtonRight : SfxSound.MobileButtonLeft);
			}
		}

		private void RefreshButtons()
		{
			if (currentGroup != null)
			{
				bool flag = selectedScreenIndex == 0;
				bool flag2 = currentGroup.linkedGroup.ContainsKey(MoveDirection.Up) && flag;
				buttonUp.Show(flag2, flag2 ? currentGroup.linkedGroup[MoveDirection.Up].captionKey : null);
				bool flag3 = currentGroup.linkedGroup.ContainsKey(MoveDirection.Down) && flag;
				buttonDown.Show(flag3, flag3 ? currentGroup.linkedGroup[MoveDirection.Down].captionKey : null);
				bool showButton = currentGroup.linkedGroup.ContainsKey(MoveDirection.Left) || selectedScreenIndex > 0;
				bool flag4 = currentGroup.linkedGroup.ContainsKey(MoveDirection.Left) && flag;
				buttonLeft.Show(showButton, flag4 ? currentGroup.linkedGroup[MoveDirection.Left].captionKey : null);
				bool showButton2 = currentGroup.linkedGroup.ContainsKey(MoveDirection.Right) || selectedScreenIndex < currentGroup.visibleScreens.Count - 1;
				bool flag5 = currentGroup.linkedGroup.ContainsKey(MoveDirection.Right) && flag;
				buttonRight.Show(showButton2, flag5 ? currentGroup.linkedGroup[MoveDirection.Right].captionKey : null);
				bool flag6 = currentScreen is MobileMenuPortal;
				RectTransform obj = buttonDown.transform as RectTransform;
				obj.anchoredPosition = (flag6 ? Vector2.zero.WithX(135f) : Vector2.zero);
				Vector2 vector = flag6 ? Vector2.zero : new Vector2(0.5f, 0f);
				Vector2 vector4 = obj.anchorMin = (obj.anchorMax = vector);
				buttonUp.glow.gameObject.SetActive(value: false);
				buttonDown.glow.gameObject.SetActive(value: false);
				buttonLeft.glow.gameObject.SetActive(value: false);
				buttonRight.glow.gameObject.SetActive(value: false);
			}
		}

		private void Update()
		{
			if (map == null || currentGroup == null)
			{
				return;
			}
			DOTween.ManualUpdate(Time.deltaTime, Time.unscaledDeltaTime);
			Vector2 vector = Vector2.zero;
			bool flag = false;
			TouchPhase touchPhase = TouchPhase.Stationary;
			if (UnityEngine.Input.touchCount == 1)
			{
				Touch touch = UnityEngine.Input.GetTouch(0);
				vector = touch.position;
				touchPhase = touch.phase;
			}
			else if (Input.mousePresent)
			{
				vector = UnityEngine.Input.mousePosition;
				touchPhase = (Input.GetMouseButtonUp(0) ? TouchPhase.Ended : ((!Input.GetMouseButtonDown(0)) ? (Input.GetMouseButton(0) ? TouchPhase.Moved : TouchPhase.Stationary) : TouchPhase.Began));
			}
			flag = (movingThisFrame || IsPointOverUIObject(vector));
			switch (touchPhase)
			{
			case TouchPhase.Began:
				OnTouchStart(vector, flag);
				break;
			case TouchPhase.Moved:
				OnTouchMove(vector);
				break;
			case TouchPhase.Ended:
				if (!flag && !grabController.grabbedObject)
				{
					OnTouchedScreen();
				}
				OnTouchEnd(vector);
				break;
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow))
			{
				MoveInDirection(MoveDirection.Left);
			}
			else if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow))
			{
				MoveInDirection(MoveDirection.Right);
			}
			else if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow))
			{
				MoveInDirection(MoveDirection.Up);
			}
			else if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow))
			{
				MoveInDirection(MoveDirection.Down);
			}
			else if (UnityEngine.Input.GetKeyDown(KeyCode.Return))
			{
				OnTouchedScreen();
			}
			float aspect = Camera.main.aspect;
			if (Mathf.Abs(aspect - lastAspectRatio) > 0.01f)
			{
				if (lastAspectRatio != 0f)
				{
					map.Build();
				}
				lastAspectRatio = aspect;
				JumpToScreen(null, instant: true);
			}
			movingThisFrame = false;
		}

		private void OnTouchedScreen()
		{
			if (!dragging && currentScreen is MobileMenuPortal)
			{
				MobileMenuPortal mobileMenuPortal = currentScreen as MobileMenuPortal;
				ExpandPortal(!expandedLevel, mobileMenuPortal.portal);
			}
		}

		private bool IsPointOverUIObject(Vector2 screenPosition)
		{
			PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
			pointerEventData.position = screenPosition;
			List<RaycastResult> list = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pointerEventData, list);
			return list.Count > 0;
		}

		private void ExpandPortal(bool expand, scrPortal portal)
		{
			if (portal.locked)
			{
				scrSfx.instance.PlaySfx(SfxSound.PortalLocked);
				portal.ShakePortal();
				return;
			}
			if (ADOBase.worldData[portal.world].levelCount == 1 || speedTrial)
			{
				EnterLevel(portal.world + "-X", speedTrial);
				return;
			}
			JumpToScreen();
			expandedLevel = expand;
			portal.ExpandPortalMobile(expand);
			foreach (MobileMenuPortal value in map.portalLUT.Values)
			{
				scrPortal portal2 = value.portal;
				if (portal2 != null && portal2 != portal)
				{
					if (expand)
					{
						portal2.FadePortal(0f);
					}
					else
					{
						value.Select(select: false);
					}
				}
			}
			if (expand)
			{
				foreach (KeyValuePair<string, GameObject> item in sublevelBrowser.submenu)
				{
					string key = item.Key;
					item.Value.SetActive(key == portal.world);
				}
			}
			sublevelBrowser.container.DOPivotY((!expand) ? 1 : 0, 0.25f).SetEase((!expand) ? Ease.Linear : Ease.OutBack);
			portal.statsText.text.rectTransform.DOPivotY(expand ? 1 : 0, 0.25f).SetEase(Ease.OutSine);
			ShowButtons(!expand);
			scrSfx.instance.PlaySfx(expand ? SfxSound.PortalSelect : SfxSound.PortalDeselect);
		}

		public void ShowButtons(bool show)
		{
			float duration = 0.25f;
			topButtonsContainer.DOPivotY(show ? 1 : 0, duration).SetEase(Ease.OutSine);
			bottomButtonsContainer.DOPivotY((!show) ? 1 : 0, duration).SetEase(Ease.OutSine);
			buttonsCanvasGroup.interactable = show;
			buttonDown.museDashIcon.DOFade(show ? 1 : 0, duration);
		}

		public static void EnterLevel(string worldAndLevel, bool speedTrial)
		{
			ADOBase.controller.portalArguments = worldAndLevel;
			ADOBase.controller.PortalTravelAction(speedTrial ? (-15) : (-14));
		}

		public static void PlaySfx(SfxSound sound)
		{
			ADOBase.conductor.DuckSongStart();
			DOVirtual.DelayedCall(scrSfx.instance.PlaySfx(sound).length + 0.05f, delegate
			{
				ADOBase.conductor.DuckSongStop();
			});
		}

		private void ToggleSpeedTrial()
		{
			bool flag = !speedTrial;
			scrSfx.instance.PlaySfx(flag ? SfxSound.SpeedTrialOn : SfxSound.SpeedTrialOff);
			scrFlash.Flash();
			SetSpeedTrial(flag);
		}

		private void SetSpeedTrial(bool on)
		{
			speedTrial = on;
			foreach (MobileMenuPortal value in map.portalLUT.Values)
			{
				if (value.visible)
				{
					value.CheckLocked(speedTrial);
				}
			}
			buttonSpeedTrial.image.sprite = (on ? RDC.data.sprSpeedTrialButtonOn : RDC.data.sprSpeedTrialButtonOff);
			JumpToScreen();
		}

		private void OnTouchStart(Vector2 pos, bool touchingUI)
		{
			Vector3 v = ADOBase.controller.camy.camobj.ScreenToWorldPoint(pos);
			dragCanBegin = (!expandedLevel && !touchingUI && !grabController.TryGrabObjectAt(v));
			if (dragCanBegin)
			{
				touchStartPos = pos;
				cameraOrigPos = ADOBase.controller.camy.transform.position;
				screenTransition.Kill();
			}
		}

		private void OnTouchMove(Vector2 pos)
		{
			if (expandedLevel)
			{
				return;
			}
			Vector2 vector = new Vector2(pos.x - touchStartPos.x, pos.y - touchStartPos.y);
			Vector2 vector2 = new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
			bool flag = selectedScreenIndex == 0 && (currentGroup.linkedGroup.ContainsKey(MoveDirection.Up) || currentGroup.linkedGroup.ContainsKey(MoveDirection.Down));
			if ((vector2.x > 5f || vector2.y > 5f) && !dragging && dragCanBegin)
			{
				dragging = true;
				dragIsVertical = (vector2.y > vector2.x);
				if (!flag)
				{
					dragIsVertical = false;
				}
			}
			if (dragging)
			{
				Vector2 b = new Vector2((!dragIsVertical) ? vector.x : 0f, dragIsVertical ? vector.y : 0f) / Screen.height * ADOBase.controller.camy.camobj.orthographicSize * 2f;
				ADOBase.controller.camy.transform.position = (cameraOrigPos - b).WithZ(ADOBase.controller.camy.transform.position.z);
			}
			else
			{
				Vector3 v = ADOBase.controller.camy.camobj.ScreenToWorldPoint(pos);
				grabController.UpdateGrabbedObject(v);
			}
		}

		private void OnTouchEnd(Vector2 pos)
		{
			Vector2 vector = new Vector2(pos.x - touchStartPos.x, pos.y - touchStartPos.y) / Screen.height * ADOBase.controller.camy.camobj.orthographicSize * 2f;
			Vector2 vector2 = new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
			if (dragging)
			{
				if (dragIsVertical)
				{
					float num = 0.5f;
					if (vector2.y > num)
					{
						MoveInDirection((vector.y > 0f) ? MoveDirection.Down : MoveDirection.Up, playSound: false);
					}
					else
					{
						JumpToScreen();
					}
				}
				else
				{
					float num2 = 0.5f;
					if (vector2.x > num2)
					{
						MoveInDirection((vector.x > 0f) ? MoveDirection.Left : MoveDirection.Right, playSound: false);
					}
					else
					{
						JumpToScreen();
					}
				}
			}
			else if (grabController.grabbedObject != null)
			{
				grabController.UngrabObject();
			}
			else
			{
				JumpToScreen();
			}
			dragging = false;
		}
	}
}
