using System.Collections.Generic;

public abstract class RDInputType
{
	public class MainStateCount
	{
		public int lastFrameUpdated;

		public List<object> keys;

		public MainStateCount()
		{
			lastFrameUpdated = -1;
			keys = new List<object>();
		}
	}

	protected int schemeIndex;

	public bool isActive = true;

	public MainStateCount pressCount = new MainStateCount();

	public MainStateCount heldCount = new MainStateCount();

	public MainStateCount releaseCount = new MainStateCount();

	public MainStateCount isReleaseCount = new MainStateCount();

	public MainStateCount dummyCount = new MainStateCount();

	protected scrController controller => scrController.instance;

	protected bool isPlaying
	{
		get
		{
			if (controller != null)
			{
				return !controller.paused;
			}
			return false;
		}
	}

	public abstract int Main(ButtonState state);

	public abstract bool Restart();

	public abstract bool Cancel();

	public abstract bool Quit();

	public abstract bool Left(ButtonState state);

	public abstract bool Right(ButtonState state);

	public abstract bool Up(ButtonState state);

	public abstract bool Down(ButtonState state);

	public abstract bool LeftAlt(ButtonState state);

	public abstract bool RightAlt(ButtonState state);

	public abstract bool UpAlt(ButtonState state);

	public abstract bool DownAlt(ButtonState state);

	public virtual void Update()
	{
	}

	protected MainStateCount GetStateCount(ButtonState state)
	{
		if (!isActive)
		{
			return dummyCount;
		}
		switch (state)
		{
		case ButtonState.WentDown:
			return pressCount;
		case ButtonState.IsDown:
			return heldCount;
		case ButtonState.WentUp:
			return releaseCount;
		default:
			return isReleaseCount;
		}
	}

	public bool Get(InputAction action, ButtonState state = ButtonState.WentDown)
	{
		if (!isActive)
		{
			return false;
		}
		switch (action)
		{
		case InputAction.Cancel:
			return Cancel();
		case InputAction.Quit:
			return Quit();
		case InputAction.Left:
			return Left(state);
		case InputAction.Right:
			return Right(state);
		case InputAction.Up:
			return Up(state);
		case InputAction.Down:
			return Down(state);
		case InputAction.LeftAlt:
			return LeftAlt(state);
		case InputAction.RightAlt:
			return RightAlt(state);
		case InputAction.UpAlt:
			return UpAlt(state);
		case InputAction.DownAlt:
			return DownAlt(state);
		default:
			return false;
		}
	}
}
