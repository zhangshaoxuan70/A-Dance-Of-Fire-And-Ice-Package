using UnityEngine;
using UnityEngine.EventSystems;

namespace ADOFAI
{
	public class PropertyControlRightClickHandler : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		public PropertyControl propertyControl;

		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Right)
			{
				propertyControl.OnRightClick();
			}
		}
	}
}
