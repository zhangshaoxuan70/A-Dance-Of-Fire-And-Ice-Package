using System;
using UnityEngine;

namespace MobileMenu
{
	public class MobileMenuGrabController
	{
		public MobileMenuGrabbable grabbedObject;

		public Action<MobileMenuGrabbable> onGrab;

		public Action<MobileMenuGrabbable> onUngrab;

		public bool TryGrabObjectAt(Vector2 pos)
		{
			RaycastHit2D raycastHit2D = Physics2D.Raycast(pos, Vector2.zero);
			if (raycastHit2D.collider != null && raycastHit2D.collider.TryGetComponent(out MobileMenuGrabbable component))
			{
				if (!component.grabbable)
				{
					return false;
				}
				grabbedObject = component;
				grabbedObject.Grab();
				if (onGrab != null)
				{
					onGrab(grabbedObject);
				}
				return true;
			}
			return false;
		}

		public void UpdateGrabbedObject(Vector2 pos)
		{
			if ((bool)grabbedObject)
			{
				grabbedObject.Move(pos);
			}
		}

		public void UngrabObject()
		{
			if ((bool)grabbedObject)
			{
				grabbedObject.Ungrab();
				if (onUngrab != null)
				{
					onUngrab(grabbedObject);
				}
				grabbedObject = null;
			}
		}
	}
}
