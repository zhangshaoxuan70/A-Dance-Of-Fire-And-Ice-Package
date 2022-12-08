public struct KeyEvent
{
	public long tick;

	public ushort keyCode;

	public bool press;

	public KeyEvent(long tick, ushort keyCode, bool press)
	{
		this.tick = tick;
		this.keyCode = keyCode;
		this.press = press;
	}
}
