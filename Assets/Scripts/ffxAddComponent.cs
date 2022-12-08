using GDMiniJSON;
using System;
using System.Collections.Generic;
using System.Reflection;

public class ffxAddComponent : ffxPlusBase
{
	public string componentName;

	public string properties;

	private bool isPlus;

	private ADOBase component;

	private Type componentType;

	public void Setup()
	{
		componentType = Type.GetType(componentName);
		if (floor == null)
		{
			floor = GetComponent<scrFloor>();
		}
		component = (floor.gameObject.AddComponent(componentType) as ADOBase);
		isPlus = componentType.IsSubclassOf(typeof(ffxPlusBase));
		if (isPlus)
		{
			floor.plusEffects.Add((ffxPlusBase)component);
			((ffxPlusBase)component).duration = duration;
		}
		Dictionary<string, object> dictionary = Json.Deserialize("{" + properties + "}") as Dictionary<string, object>;
		if (dictionary != null)
		{
			foreach (KeyValuePair<string, object> item in dictionary)
			{
				FieldInfo field = componentType.GetField(item.Key);
				if (field != null)
				{
					field.SetValue(component, item.Value);
				}
			}
		}
	}

	public override void StartEffect()
	{
		AdjustDurationForHardbake();
	}

	public override void SetStartTime(float bpm, float degreeOffset = 0f)
	{
		base.SetStartTime(bpm, degreeOffset);
		if (component != null && isPlus)
		{
			(component as ffxPlusBase).SetStartTime(bpm, degreeOffset);
		}
	}
}
