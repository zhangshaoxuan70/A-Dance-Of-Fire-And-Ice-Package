using Rewired.Integration.UnityUI;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rewired.Demos
{
	[AddComponentMenu("")]
	public sealed class PlayerPointerEventHandlerExample : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler, IScrollHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		public Text text;

		private const int logLength = 10;

		private List<string> log = new List<string>();

		private void Log(string o)
		{
			log.Add(o);
			if (log.Count > 10)
			{
				log.RemoveAt(0);
			}
		}

		private void Update()
		{
			if (text != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string item in log)
				{
					stringBuilder.AppendLine(item);
				}
				text.text = stringBuilder.ToString();
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (eventData is PlayerPointerEventData)
			{
				PlayerPointerEventData playerPointerEventData = (PlayerPointerEventData)eventData;
				Log("OnPointerEnter:  Player = " + playerPointerEventData.playerId.ToString() + ", Pointer Index = " + playerPointerEventData.inputSourceIndex.ToString() + ", Source = " + GetSourceName(playerPointerEventData));
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (eventData is PlayerPointerEventData)
			{
				PlayerPointerEventData playerPointerEventData = (PlayerPointerEventData)eventData;
				Log("OnPointerExit:  Player = " + playerPointerEventData.playerId.ToString() + ", Pointer Index = " + playerPointerEventData.inputSourceIndex.ToString() + ", Source = " + GetSourceName(playerPointerEventData));
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (eventData is PlayerPointerEventData)
			{
				PlayerPointerEventData playerPointerEventData = (PlayerPointerEventData)eventData;
				Log("OnPointerUp:  Player = " + playerPointerEventData.playerId.ToString() + ", Pointer Index = " + playerPointerEventData.inputSourceIndex.ToString() + ", Source = " + GetSourceName(playerPointerEventData) + ", Button Index = " + playerPointerEventData.buttonIndex.ToString());
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (eventData is PlayerPointerEventData)
			{
				PlayerPointerEventData playerPointerEventData = (PlayerPointerEventData)eventData;
				Log("OnPointerDown:  Player = " + playerPointerEventData.playerId.ToString() + ", Pointer Index = " + playerPointerEventData.inputSourceIndex.ToString() + ", Source = " + GetSourceName(playerPointerEventData) + ", Button Index = " + playerPointerEventData.buttonIndex.ToString());
			}
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData is PlayerPointerEventData)
			{
				PlayerPointerEventData playerPointerEventData = (PlayerPointerEventData)eventData;
				Log("OnPointerClick:  Player = " + playerPointerEventData.playerId.ToString() + ", Pointer Index = " + playerPointerEventData.inputSourceIndex.ToString() + ", Source = " + GetSourceName(playerPointerEventData) + ", Button Index = " + playerPointerEventData.buttonIndex.ToString());
			}
		}

		public void OnScroll(PointerEventData eventData)
		{
			if (eventData is PlayerPointerEventData)
			{
				PlayerPointerEventData playerPointerEventData = (PlayerPointerEventData)eventData;
				Log("OnScroll:  Player = " + playerPointerEventData.playerId.ToString() + ", Pointer Index = " + playerPointerEventData.inputSourceIndex.ToString() + ", Source = " + GetSourceName(playerPointerEventData));
			}
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (eventData is PlayerPointerEventData)
			{
				PlayerPointerEventData playerPointerEventData = (PlayerPointerEventData)eventData;
				Log("OnBeginDrag:  Player = " + playerPointerEventData.playerId.ToString() + ", Pointer Index = " + playerPointerEventData.inputSourceIndex.ToString() + ", Source = " + GetSourceName(playerPointerEventData) + ", Button Index = " + playerPointerEventData.buttonIndex.ToString());
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (eventData is PlayerPointerEventData)
			{
				PlayerPointerEventData playerPointerEventData = (PlayerPointerEventData)eventData;
				Log("OnDrag:  Player = " + playerPointerEventData.playerId.ToString() + ", Pointer Index = " + playerPointerEventData.inputSourceIndex.ToString() + ", Source = " + GetSourceName(playerPointerEventData) + ", Button Index = " + playerPointerEventData.buttonIndex.ToString());
			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if (eventData is PlayerPointerEventData)
			{
				PlayerPointerEventData playerPointerEventData = (PlayerPointerEventData)eventData;
				Log("OnEndDrag:  Player = " + playerPointerEventData.playerId.ToString() + ", Pointer Index = " + playerPointerEventData.inputSourceIndex.ToString() + ", Source = " + GetSourceName(playerPointerEventData) + ", Button Index = " + playerPointerEventData.buttonIndex.ToString());
			}
		}

		private static string GetSourceName(PlayerPointerEventData playerEventData)
		{
			if (playerEventData.sourceType == PointerEventType.Mouse)
			{
				if (playerEventData.mouseSource is Behaviour)
				{
					return (playerEventData.mouseSource as Behaviour).name;
				}
			}
			else if (playerEventData.sourceType == PointerEventType.Touch && playerEventData.touchSource is Behaviour)
			{
				return (playerEventData.touchSource as Behaviour).name;
			}
			return null;
		}
	}
}
