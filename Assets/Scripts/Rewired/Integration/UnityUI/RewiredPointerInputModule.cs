using Rewired.UI;
using Rewired.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Rewired.Integration.UnityUI
{
	public abstract class RewiredPointerInputModule : BaseInputModule
	{
		protected class MouseState
		{
			private List<ButtonState> m_TrackedButtons = new List<ButtonState>();

			public bool AnyPressesThisFrame()
			{
				for (int i = 0; i < m_TrackedButtons.Count; i++)
				{
					if (m_TrackedButtons[i].eventData.PressedThisFrame())
					{
						return true;
					}
				}
				return false;
			}

			public bool AnyReleasesThisFrame()
			{
				for (int i = 0; i < m_TrackedButtons.Count; i++)
				{
					if (m_TrackedButtons[i].eventData.ReleasedThisFrame())
					{
						return true;
					}
				}
				return false;
			}

			public ButtonState GetButtonState(int button)
			{
				ButtonState buttonState = null;
				for (int i = 0; i < m_TrackedButtons.Count; i++)
				{
					if (m_TrackedButtons[i].button == button)
					{
						buttonState = m_TrackedButtons[i];
						break;
					}
				}
				if (buttonState == null)
				{
					buttonState = new ButtonState
					{
						button = button,
						eventData = new MouseButtonEventData()
					};
					m_TrackedButtons.Add(buttonState);
				}
				return buttonState;
			}

			public void SetButtonState(int button, PointerEventData.FramePressState stateForMouseButton, PlayerPointerEventData data)
			{
				ButtonState buttonState = GetButtonState(button);
				buttonState.eventData.buttonState = stateForMouseButton;
				buttonState.eventData.buttonData = data;
			}
		}

		public class MouseButtonEventData
		{
			public PointerEventData.FramePressState buttonState;

			public PlayerPointerEventData buttonData;

			public bool PressedThisFrame()
			{
				if (buttonState != 0)
				{
					return buttonState == PointerEventData.FramePressState.PressedAndReleased;
				}
				return true;
			}

			public bool ReleasedThisFrame()
			{
				if (buttonState != PointerEventData.FramePressState.Released)
				{
					return buttonState == PointerEventData.FramePressState.PressedAndReleased;
				}
				return true;
			}
		}

		protected class ButtonState
		{
			private int m_Button;

			private MouseButtonEventData m_EventData;

			public MouseButtonEventData eventData
			{
				get
				{
					return m_EventData;
				}
				set
				{
					m_EventData = value;
				}
			}

			public int button
			{
				get
				{
					return m_Button;
				}
				set
				{
					m_Button = value;
				}
			}
		}

		private sealed class UnityInputSource : IMouseInputSource, ITouchInputSource
		{
			private Vector2 m_MousePosition;

			private Vector2 m_MousePositionPrev;

			private int m_LastUpdatedFrame = -1;

			int IMouseInputSource.playerId
			{
				get
				{
					TryUpdate();
					return 0;
				}
			}

			int ITouchInputSource.playerId
			{
				get
				{
					TryUpdate();
					return 0;
				}
			}

			bool IMouseInputSource.enabled
			{
				get
				{
					TryUpdate();
					return true;
				}
			}

			bool IMouseInputSource.locked
			{
				get
				{
					TryUpdate();
					return Cursor.lockState == CursorLockMode.Locked;
				}
			}

			int IMouseInputSource.buttonCount
			{
				get
				{
					TryUpdate();
					return 3;
				}
			}

			Vector2 IMouseInputSource.screenPosition
			{
				get
				{
					TryUpdate();
					return UnityEngine.Input.mousePosition;
				}
			}

			Vector2 IMouseInputSource.screenPositionDelta
			{
				get
				{
					TryUpdate();
					return m_MousePosition - m_MousePositionPrev;
				}
			}

			Vector2 IMouseInputSource.wheelDelta
			{
				get
				{
					TryUpdate();
					return Input.mouseScrollDelta;
				}
			}

			bool ITouchInputSource.touchSupported
			{
				get
				{
					TryUpdate();
					return Input.touchSupported;
				}
			}

			int ITouchInputSource.touchCount
			{
				get
				{
					TryUpdate();
					return UnityEngine.Input.touchCount;
				}
			}

			bool IMouseInputSource.GetButtonDown(int button)
			{
				TryUpdate();
				return Input.GetMouseButtonDown(button);
			}

			bool IMouseInputSource.GetButtonUp(int button)
			{
				TryUpdate();
				return Input.GetMouseButtonUp(button);
			}

			bool IMouseInputSource.GetButton(int button)
			{
				TryUpdate();
				return Input.GetMouseButton(button);
			}

			Touch ITouchInputSource.GetTouch(int index)
			{
				TryUpdate();
				return UnityEngine.Input.GetTouch(index);
			}

			private void TryUpdate()
			{
				if (Time.frameCount != m_LastUpdatedFrame)
				{
					m_LastUpdatedFrame = Time.frameCount;
					m_MousePositionPrev = m_MousePosition;
					m_MousePosition = UnityEngine.Input.mousePosition;
				}
			}
		}

		public const int kMouseLeftId = -1;

		public const int kMouseRightId = -2;

		public const int kMouseMiddleId = -3;

		public const int kFakeTouchesId = -4;

		private const int customButtonsStartingId = -2147483520;

		private const int customButtonsMaxCount = 128;

		private const int customButtonsLastId = -2147483392;

		private readonly List<IMouseInputSource> m_MouseInputSourcesList = new List<IMouseInputSource>();

		private Dictionary<int, Dictionary<int, PlayerPointerEventData>[]> m_PlayerPointerData = new Dictionary<int, Dictionary<int, PlayerPointerEventData>[]>();

		private ITouchInputSource m_UserDefaultTouchInputSource;

		private UnityInputSource __m_DefaultInputSource;

		private readonly MouseState m_MouseState = new MouseState();

		private UnityInputSource defaultInputSource
		{
			get
			{
				if (__m_DefaultInputSource == null)
				{
					return __m_DefaultInputSource = new UnityInputSource();
				}
				return __m_DefaultInputSource;
			}
		}

		private IMouseInputSource defaultMouseInputSource => defaultInputSource;

		protected ITouchInputSource defaultTouchInputSource => defaultInputSource;

		protected virtual bool isMouseSupported
		{
			get
			{
				int count = m_MouseInputSourcesList.Count;
				if (count == 0)
				{
					return defaultMouseInputSource.enabled;
				}
				for (int i = 0; i < count; i++)
				{
					if (m_MouseInputSourcesList[i].enabled)
					{
						return true;
					}
				}
				return false;
			}
		}

		protected bool IsDefaultMouse(IMouseInputSource mouse)
		{
			return defaultMouseInputSource == mouse;
		}

		public IMouseInputSource GetMouseInputSource(int playerId, int mouseIndex)
		{
			if (mouseIndex < 0)
			{
				throw new ArgumentOutOfRangeException("mouseIndex");
			}
			if (m_MouseInputSourcesList.Count == 0 && IsDefaultPlayer(playerId))
			{
				return defaultMouseInputSource;
			}
			int count = m_MouseInputSourcesList.Count;
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				IMouseInputSource mouseInputSource = m_MouseInputSourcesList[i];
				if (!UnityTools.IsNullOrDestroyed(mouseInputSource) && mouseInputSource.playerId == playerId)
				{
					if (mouseIndex == num)
					{
						return mouseInputSource;
					}
					num++;
				}
			}
			return null;
		}

		public void RemoveMouseInputSource(IMouseInputSource source)
		{
			if (source == null)
			{
				throw new System.ArgumentNullException("source");
			}
			m_MouseInputSourcesList.Remove(source);
		}

		public void AddMouseInputSource(IMouseInputSource source)
		{
			if (UnityTools.IsNullOrDestroyed(source))
			{
				throw new System.ArgumentNullException("source");
			}
			m_MouseInputSourcesList.Add(source);
		}

		public int GetMouseInputSourceCount(int playerId)
		{
			if (m_MouseInputSourcesList.Count == 0 && IsDefaultPlayer(playerId))
			{
				return 1;
			}
			int count = m_MouseInputSourcesList.Count;
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				IMouseInputSource mouseInputSource = m_MouseInputSourcesList[i];
				if (!UnityTools.IsNullOrDestroyed(mouseInputSource) && mouseInputSource.playerId == playerId)
				{
					num++;
				}
			}
			return num;
		}

		public ITouchInputSource GetTouchInputSource(int playerId, int sourceIndex)
		{
			if (!UnityTools.IsNullOrDestroyed(m_UserDefaultTouchInputSource))
			{
				return m_UserDefaultTouchInputSource;
			}
			return defaultTouchInputSource;
		}

		public void RemoveTouchInputSource(ITouchInputSource source)
		{
			if (source == null)
			{
				throw new System.ArgumentNullException("source");
			}
			if (m_UserDefaultTouchInputSource == source)
			{
				m_UserDefaultTouchInputSource = null;
			}
		}

		public void AddTouchInputSource(ITouchInputSource source)
		{
			if (UnityTools.IsNullOrDestroyed(source))
			{
				throw new System.ArgumentNullException("source");
			}
			m_UserDefaultTouchInputSource = source;
		}

		public int GetTouchInputSourceCount(int playerId)
		{
			if (!IsDefaultPlayer(playerId))
			{
				return 0;
			}
			return 1;
		}

		protected void ClearMouseInputSources()
		{
			m_MouseInputSourcesList.Clear();
		}

		protected abstract bool IsDefaultPlayer(int playerId);

		protected bool GetPointerData(int playerId, int pointerIndex, int pointerTypeId, out PlayerPointerEventData data, bool create, PointerEventType pointerEventType)
		{
			if (!m_PlayerPointerData.TryGetValue(playerId, out Dictionary<int, PlayerPointerEventData>[] value))
			{
				value = new Dictionary<int, PlayerPointerEventData>[pointerIndex + 1];
				for (int i = 0; i < value.Length; i++)
				{
					value[i] = new Dictionary<int, PlayerPointerEventData>();
				}
				m_PlayerPointerData.Add(playerId, value);
			}
			if (pointerIndex >= value.Length)
			{
				Dictionary<int, PlayerPointerEventData>[] array = new Dictionary<int, PlayerPointerEventData>[pointerIndex + 1];
				for (int j = 0; j < value.Length; j++)
				{
					array[j] = value[j];
				}
				array[pointerIndex] = new Dictionary<int, PlayerPointerEventData>();
				value = array;
				m_PlayerPointerData[playerId] = value;
			}
			Dictionary<int, PlayerPointerEventData> dictionary = value[pointerIndex];
			if (!dictionary.TryGetValue(pointerTypeId, out data))
			{
				if (!create)
				{
					return false;
				}
				data = CreatePointerEventData(playerId, pointerIndex, pointerTypeId, pointerEventType);
				dictionary.Add(pointerTypeId, data);
				return true;
			}
			data.mouseSource = ((pointerEventType == PointerEventType.Mouse) ? GetMouseInputSource(playerId, pointerIndex) : null);
			data.touchSource = ((pointerEventType == PointerEventType.Touch) ? GetTouchInputSource(playerId, pointerIndex) : null);
			return false;
		}

		private PlayerPointerEventData CreatePointerEventData(int playerId, int pointerIndex, int pointerTypeId, PointerEventType pointerEventType)
		{
			PlayerPointerEventData playerPointerEventData = new PlayerPointerEventData(base.eventSystem)
			{
				playerId = playerId,
				inputSourceIndex = pointerIndex,
				pointerId = pointerTypeId,
				sourceType = pointerEventType
			};
			switch (pointerEventType)
			{
			case PointerEventType.Mouse:
				playerPointerEventData.mouseSource = GetMouseInputSource(playerId, pointerIndex);
				break;
			case PointerEventType.Touch:
				playerPointerEventData.touchSource = GetTouchInputSource(playerId, pointerIndex);
				break;
			}
			if (pointerTypeId == -1)
			{
				playerPointerEventData.buttonIndex = 0;
			}
			else if (pointerTypeId == -2)
			{
				playerPointerEventData.buttonIndex = 1;
			}
			else if (pointerTypeId == -3)
			{
				playerPointerEventData.buttonIndex = 2;
			}
			else if (pointerTypeId >= -2147483520 && pointerTypeId <= -2147483392)
			{
				playerPointerEventData.buttonIndex = pointerTypeId - -2147483520;
			}
			return playerPointerEventData;
		}

		protected void RemovePointerData(PlayerPointerEventData data)
		{
			if (m_PlayerPointerData.TryGetValue(data.playerId, out Dictionary<int, PlayerPointerEventData>[] value) && (uint)data.inputSourceIndex < (uint)value.Length)
			{
				value[data.inputSourceIndex].Remove(data.pointerId);
			}
		}

		protected PlayerPointerEventData GetTouchPointerEventData(int playerId, int touchDeviceIndex, Touch input, out bool pressed, out bool released)
		{
			PlayerPointerEventData data;
			bool pointerData = GetPointerData(playerId, touchDeviceIndex, input.fingerId, out data, create: true, PointerEventType.Touch);
			data.Reset();
			pressed = (pointerData || input.phase == TouchPhase.Began);
			released = (input.phase == TouchPhase.Canceled || input.phase == TouchPhase.Ended);
			if (pointerData)
			{
				data.position = input.position;
			}
			if (pressed)
			{
				data.delta = Vector2.zero;
			}
			else
			{
				data.delta = input.position - data.position;
			}
			data.position = input.position;
			data.button = PointerEventData.InputButton.Left;
			base.eventSystem.RaycastAll(data, m_RaycastResultCache);
			RaycastResult raycastResult2 = data.pointerCurrentRaycast = BaseInputModule.FindFirstRaycast(m_RaycastResultCache);
			m_RaycastResultCache.Clear();
			return data;
		}

		protected virtual MouseState GetMousePointerEventData(int playerId, int mouseIndex)
		{
			IMouseInputSource mouseInputSource = GetMouseInputSource(playerId, mouseIndex);
			if (mouseInputSource == null)
			{
				return null;
			}
			PlayerPointerEventData data;
			bool pointerData = GetPointerData(playerId, mouseIndex, -1, out data, create: true, PointerEventType.Mouse);
			data.Reset();
			if (pointerData)
			{
				data.position = mouseInputSource.screenPosition;
			}
			Vector2 screenPosition = mouseInputSource.screenPosition;
			if (mouseInputSource.locked || !mouseInputSource.enabled)
			{
				data.position = new Vector2(-1f, -1f);
				data.delta = Vector2.zero;
			}
			else
			{
				data.delta = screenPosition - data.position;
				data.position = screenPosition;
			}
			data.scrollDelta = mouseInputSource.wheelDelta;
			data.button = PointerEventData.InputButton.Left;
			base.eventSystem.RaycastAll(data, m_RaycastResultCache);
			RaycastResult raycastResult2 = data.pointerCurrentRaycast = BaseInputModule.FindFirstRaycast(m_RaycastResultCache);
			m_RaycastResultCache.Clear();
			GetPointerData(playerId, mouseIndex, -2, out PlayerPointerEventData data2, create: true, PointerEventType.Mouse);
			CopyFromTo(data, data2);
			data2.button = PointerEventData.InputButton.Right;
			GetPointerData(playerId, mouseIndex, -3, out PlayerPointerEventData data3, create: true, PointerEventType.Mouse);
			CopyFromTo(data, data3);
			data3.button = PointerEventData.InputButton.Middle;
			for (int i = 3; i < mouseInputSource.buttonCount; i++)
			{
				GetPointerData(playerId, mouseIndex, -2147483520 + i, out PlayerPointerEventData data4, create: true, PointerEventType.Mouse);
				CopyFromTo(data, data4);
				data4.button = (PointerEventData.InputButton)(-1);
			}
			m_MouseState.SetButtonState(0, StateForMouseButton(playerId, mouseIndex, 0), data);
			m_MouseState.SetButtonState(1, StateForMouseButton(playerId, mouseIndex, 1), data2);
			m_MouseState.SetButtonState(2, StateForMouseButton(playerId, mouseIndex, 2), data3);
			for (int j = 3; j < mouseInputSource.buttonCount; j++)
			{
				GetPointerData(playerId, mouseIndex, -2147483520 + j, out PlayerPointerEventData data5, create: false, PointerEventType.Mouse);
				m_MouseState.SetButtonState(j, StateForMouseButton(playerId, mouseIndex, j), data5);
			}
			return m_MouseState;
		}

		protected PlayerPointerEventData GetLastPointerEventData(int playerId, int pointerIndex, int pointerTypeId, bool ignorePointerTypeId, PointerEventType pointerEventType)
		{
			if (!ignorePointerTypeId)
			{
				GetPointerData(playerId, pointerIndex, pointerTypeId, out PlayerPointerEventData data, create: false, pointerEventType);
				return data;
			}
			if (!m_PlayerPointerData.TryGetValue(playerId, out Dictionary<int, PlayerPointerEventData>[] value))
			{
				return null;
			}
			if ((uint)pointerIndex >= (uint)value.Length)
			{
				return null;
			}
			using (Dictionary<int, PlayerPointerEventData>.Enumerator enumerator = value[pointerIndex].GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current.Value;
				}
			}
			return null;
		}

		private static bool ShouldStartDrag(Vector2 pressPos, Vector2 currentPos, float threshold, bool useDragThreshold)
		{
			if (!useDragThreshold)
			{
				return true;
			}
			return (pressPos - currentPos).sqrMagnitude >= threshold * threshold;
		}

		protected virtual void ProcessMove(PlayerPointerEventData pointerEvent)
		{
			GameObject newEnterTarget;
			if (pointerEvent.sourceType == PointerEventType.Mouse)
			{
				IMouseInputSource mouseInputSource = GetMouseInputSource(pointerEvent.playerId, pointerEvent.inputSourceIndex);
				newEnterTarget = ((mouseInputSource == null) ? null : ((!mouseInputSource.enabled || mouseInputSource.locked) ? null : pointerEvent.pointerCurrentRaycast.gameObject));
			}
			else
			{
				if (pointerEvent.sourceType != PointerEventType.Touch)
				{
					throw new NotImplementedException();
				}
				newEnterTarget = pointerEvent.pointerCurrentRaycast.gameObject;
			}
			HandlePointerExitAndEnter(pointerEvent, newEnterTarget);
		}

		protected virtual void ProcessDrag(PlayerPointerEventData pointerEvent)
		{
			if (!pointerEvent.IsPointerMoving() || pointerEvent.pointerDrag == null)
			{
				return;
			}
			if (pointerEvent.sourceType == PointerEventType.Mouse)
			{
				IMouseInputSource mouseInputSource = GetMouseInputSource(pointerEvent.playerId, pointerEvent.inputSourceIndex);
				if (mouseInputSource == null || mouseInputSource.locked || !mouseInputSource.enabled)
				{
					return;
				}
			}
			if (!pointerEvent.dragging && ShouldStartDrag(pointerEvent.pressPosition, pointerEvent.position, base.eventSystem.pixelDragThreshold, pointerEvent.useDragThreshold))
			{
				ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.beginDragHandler);
				pointerEvent.dragging = true;
			}
			if (pointerEvent.dragging)
			{
				if (pointerEvent.pointerPress != pointerEvent.pointerDrag)
				{
					ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);
					pointerEvent.eligibleForClick = false;
					pointerEvent.pointerPress = null;
					pointerEvent.rawPointerPress = null;
				}
				ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.dragHandler);
			}
		}

		public override bool IsPointerOverGameObject(int pointerTypeId)
		{
			foreach (KeyValuePair<int, Dictionary<int, PlayerPointerEventData>[]> playerPointerDatum in m_PlayerPointerData)
			{
				Dictionary<int, PlayerPointerEventData>[] value = playerPointerDatum.Value;
				for (int i = 0; i < value.Length; i++)
				{
					if (value[i].TryGetValue(pointerTypeId, out PlayerPointerEventData value2) && value2.pointerEnter != null)
					{
						return true;
					}
				}
			}
			return false;
		}

		protected void ClearSelection()
		{
			BaseEventData baseEventData = GetBaseEventData();
			foreach (KeyValuePair<int, Dictionary<int, PlayerPointerEventData>[]> playerPointerDatum in m_PlayerPointerData)
			{
				Dictionary<int, PlayerPointerEventData>[] value = playerPointerDatum.Value;
				for (int i = 0; i < value.Length; i++)
				{
					foreach (KeyValuePair<int, PlayerPointerEventData> item in value[i])
					{
						HandlePointerExitAndEnter(item.Value, null);
					}
					value[i].Clear();
				}
			}
			base.eventSystem.SetSelectedGameObject(null, baseEventData);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("<b>Pointer Input Module of type: </b>" + GetType()?.ToString());
			stringBuilder.AppendLine();
			foreach (KeyValuePair<int, Dictionary<int, PlayerPointerEventData>[]> playerPointerDatum in m_PlayerPointerData)
			{
				stringBuilder.AppendLine("<B>Player Id:</b> " + playerPointerDatum.Key.ToString());
				Dictionary<int, PlayerPointerEventData>[] value = playerPointerDatum.Value;
				for (int i = 0; i < value.Length; i++)
				{
					stringBuilder.AppendLine("<B>Pointer Index:</b> " + i.ToString());
					foreach (KeyValuePair<int, PlayerPointerEventData> item in value[i])
					{
						stringBuilder.AppendLine("<B>Button Id:</b> " + item.Key.ToString());
						stringBuilder.AppendLine(item.Value.ToString());
					}
				}
			}
			return stringBuilder.ToString();
		}

		protected void DeselectIfSelectionChanged(GameObject currentOverGo, BaseEventData pointerEvent)
		{
			if (ExecuteEvents.GetEventHandler<ISelectHandler>(currentOverGo) != base.eventSystem.currentSelectedGameObject)
			{
				base.eventSystem.SetSelectedGameObject(null, pointerEvent);
			}
		}

		protected void CopyFromTo(PointerEventData from, PointerEventData to)
		{
			to.position = from.position;
			to.delta = from.delta;
			to.scrollDelta = from.scrollDelta;
			to.pointerCurrentRaycast = from.pointerCurrentRaycast;
			to.pointerEnter = from.pointerEnter;
		}

		protected PointerEventData.FramePressState StateForMouseButton(int playerId, int mouseIndex, int buttonId)
		{
			IMouseInputSource mouseInputSource = GetMouseInputSource(playerId, mouseIndex);
			if (mouseInputSource == null)
			{
				return PointerEventData.FramePressState.NotChanged;
			}
			bool buttonDown = mouseInputSource.GetButtonDown(buttonId);
			bool buttonUp = mouseInputSource.GetButtonUp(buttonId);
			if (buttonDown && buttonUp)
			{
				return PointerEventData.FramePressState.PressedAndReleased;
			}
			if (buttonDown)
			{
				return PointerEventData.FramePressState.Pressed;
			}
			if (buttonUp)
			{
				return PointerEventData.FramePressState.Released;
			}
			return PointerEventData.FramePressState.NotChanged;
		}
	}
}
