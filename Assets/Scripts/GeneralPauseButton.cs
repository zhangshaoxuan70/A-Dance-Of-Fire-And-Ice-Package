using System;
using UnityEngine;

public abstract class GeneralPauseButton : MonoBehaviour
{
	[NonSerialized]
	public int index;

	[NonSerialized]
	public RectTransform rectangleRT;

	[NonSerialized]
	public RectTransform rectTransform;

	public abstract void SetFocus(bool value);

	public abstract void Select();
}
