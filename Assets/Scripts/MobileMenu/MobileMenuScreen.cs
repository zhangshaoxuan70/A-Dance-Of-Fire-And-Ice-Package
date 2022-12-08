using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MobileMenu
{
	public abstract class MobileMenuScreen
	{
		public Transform transform;

		public bool visible = true;

		public string[] visibilityConditions;

		public MobileMenuGroup parentGroup;

		public static MobileMenuScreen New(string screenType)
		{
			if (!(screenType == "portal"))
			{
				if (!(screenType == "title"))
				{
					if (!(screenType == "credits"))
					{
						if (screenType == "colors")
						{
							return new MobileMenuColorScreen();
						}
						return null;
					}
					return new MobileMenuCredits();
				}
				return new MobileMenuTitle();
			}
			return new MobileMenuPortal();
		}

		public virtual float GetWidth()
		{
			return GetBaseWidth();
		}

		public static float GetBaseWidth()
		{
			Camera camobj = scrController.instance.camy.camobj;
			return 2f * camobj.orthographicSize * camobj.aspect;
		}

		public static float GetAspect()
		{
			return scrController.instance.camy.camobj.aspect;
		}

		public virtual void Select(bool select = true, bool instant = false)
		{
		}

		public virtual void Instantiate()
		{
			Select(select: false);
		}

		public virtual void Decode(Dictionary<string, object> dict)
		{
			if (dict.TryGetValueAs("visibleIf", out List<object> valueAs))
			{
				visibilityConditions = valueAs.OfType<string>().ToArray();
			}
		}
	}
}
