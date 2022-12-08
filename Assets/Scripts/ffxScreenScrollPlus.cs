using UnityEngine;

public class ffxScreenScrollPlus : ffxPlusBase
{
	public float scrollX;

	public float scrollY;

	private static ScreenScroll screenScroll;

	public override void Awake()
	{
		base.Awake();
		if (screenScroll == null)
		{
			screenScroll = cam.GetComponent<ScreenScroll>();
		}
	}

	public override void StartEffect()
	{
		SetScreenScroll(scrollX, scrollY);
	}

	private void SetScreenScroll(float scrollX, float scrollY)
	{
		screenScroll.enabled = (scrollX != 0f || scrollY != 0f);
		screenScroll.scrollSpeed = new Vector2(scrollX, scrollY) * cond.song.pitch;
	}
}
