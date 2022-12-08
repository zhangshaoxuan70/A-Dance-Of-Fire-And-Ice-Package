using UnityEngine;

public struct MutualKeyCode
{
	public const ushort AsyncNullKeyCode = 0;

	public const KeyCode SyncNullKeyCode = (KeyCode)(-2147483648);

	public const KeyCode SyncButton = (KeyCode)(-2147483647);

	public bool isTouch;

	public bool isAsync;

	public ushort asyncKeyCode;

	public KeyCode syncKeyCode;

	public string syncButton;

	public object keyCodeObject
	{
		get
		{
			if (syncKeyCode != (KeyCode)(-2147483647))
			{
				return syncKeyCode;
			}
			return syncButton;
		}
	}

	public MutualKeyCode(bool isAsync, object keyCode)
	{
		syncButton = null;
		isTouch = false;
		if (this.isAsync = isAsync)
		{
			if (keyCode is ushort)
			{
				ushort num = (ushort)keyCode;
				asyncKeyCode = num;
			}
			else
			{
				asyncKeyCode = 0;
			}
			syncKeyCode = (KeyCode)(-2147483648);
		}
		else
		{
			if (keyCode is KeyCode)
			{
				KeyCode keyCode2 = syncKeyCode = (KeyCode)keyCode;
			}
			else
			{
				syncKeyCode = (KeyCode)(-2147483647);
				syncButton = (string)keyCode;
			}
			asyncKeyCode = 0;
		}
	}

	public MutualKeyCode(object _)
	{
		isTouch = true;
		isAsync = false;
		asyncKeyCode = 0;
		syncKeyCode = (KeyCode)(-2147483648);
		syncButton = null;
	}
}
