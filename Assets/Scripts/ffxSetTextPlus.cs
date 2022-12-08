using System;
using System.Collections.Generic;

public class ffxSetTextPlus : ffxPlusBase
{
	[NonSerialized]
	public scrDecorationManager decManager;

	public string targetString;

	public List<string> targetTags = new List<string>();

	public override void StartEffect()
	{
		AdjustDurationForHardbake();
		List<scrTextDecoration> list = new List<scrTextDecoration>();
		foreach (string targetTag in targetTags)
		{
			if (decManager.taggedDecorations.ContainsKey(targetTag))
			{
				foreach (scrDecoration item in decManager.taggedDecorations[targetTag])
				{
					if (item is scrTextDecoration)
					{
						scrTextDecoration scrTextDecoration = (scrTextDecoration)item;
						if (!list.Contains(scrTextDecoration))
						{
							scrTextDecoration.SetText(targetString);
							list.Add(scrTextDecoration);
						}
					}
				}
			}
		}
	}
}
