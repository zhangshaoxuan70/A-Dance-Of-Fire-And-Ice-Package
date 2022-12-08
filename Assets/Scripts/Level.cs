using RDTools;
using System.Collections.Generic;
using UnityEngine;

public class Level : ADOClass
{
	public virtual void Init()
	{
	}

	public virtual void Hit(int floor)
	{
	}

	public static T FindDecorationComponent<T>(string decorationTag) where T : Component
	{
		scrDecorationManager instance = scrDecorationManager.instance;
		if (instance == null)
		{
			return null;
		}
		if (instance.taggedDecorations.TryGetValue(decorationTag, out List<scrDecoration> value))
		{
			foreach (scrDecoration item in value)
			{
				if (item != null)
				{
					T componentInChildren = item.transform.GetComponentInChildren<T>();
					if ((Object)componentInChildren != (Object)null)
					{
						return componentInChildren;
					}
					RDClassDll.printesw(typeof(T).ToString() + " component was not found");
				}
			}
		}
		RDClassDll.printem("returning null because didn't found tag: " + decorationTag);
		return null;
	}

	public void HideDecorations(string tag)
	{
		SetDecorationVisibility(tag, visible: false);
	}

	public void ShowDecorations(string tag)
	{
		SetDecorationVisibility(tag, visible: true);
	}

	private void SetDecorationVisibility(string tag, bool visible)
	{
		if (!(base.decorationManager == null) && base.decorationManager.taggedDecorations.TryGetValue(tag, out List<scrDecoration> value))
		{
			foreach (scrDecoration item in value)
			{
				item.gameObject.SetActive(visible);
			}
		}
	}
}
