using DG.Tweening;

public static class Timer
{
	public static void Add(TweenCallback action, float delay, bool ignoreTimeScale = true)
	{
		DOVirtual.DelayedCall(delay, action, ignoreTimeScale);
	}
}
