using SharpHook.Native;

public static class AsyncInput
{
	public static ushort MouseButtonCodeOffset = 1000;

	public static bool GetKey(MutualKeyCode keyCode, bool frameDependent = true)
	{
		return GetKey(keyCode.asyncKeyCode, frameDependent);
	}

	public static bool GetKey(KeyCode keyCode, bool frameDependent = true)
	{
		return GetKey((ushort)keyCode, frameDependent);
	}

	public static bool GetKey(MouseButton mouseButton, bool frameDependent = true)
	{
		return GetKey((ushort)(mouseButton + MouseButtonCodeOffset), frameDependent);
	}

	public static bool GetKey(ushort keyCode, bool frameDependent = true)
	{
		return (frameDependent ? AsyncInputManager.frameDependentKeyMask : AsyncInputManager.keyMask).Contains(keyCode);
	}

	public static bool GetKeyDown(MutualKeyCode keyCode, bool frameDependent = true)
	{
		return GetKeyDown(keyCode.asyncKeyCode, frameDependent);
	}

	public static bool GetKeyDown(KeyCode keyCode, bool frameDependent = true)
	{
		return GetKeyDown((ushort)keyCode, frameDependent);
	}

	public static bool GetKeyDown(MouseButton mouseButton, bool frameDependent = true)
	{
		return GetKeyDown((ushort)(mouseButton + MouseButtonCodeOffset), frameDependent);
	}

	public static bool GetKeyDown(ushort keyCode, bool frameDependent = true)
	{
		return (frameDependent ? AsyncInputManager.frameDependentKeyDownMask : AsyncInputManager.keyDownMask).Contains(keyCode);
	}

	public static bool GetKeyUp(MutualKeyCode keyCode, bool frameDependent = true)
	{
		return GetKeyUp(keyCode.asyncKeyCode, frameDependent);
	}

	public static bool GetKeyUp(KeyCode keyCode, bool frameDependent = true)
	{
		return GetKeyUp((ushort)keyCode, frameDependent);
	}

	public static bool GetKeyUp(MouseButton mouseButton, bool frameDependent = true)
	{
		return GetKeyUp((ushort)(mouseButton + MouseButtonCodeOffset), frameDependent);
	}

	public static bool GetKeyUp(ushort keyCode, bool frameDependent = true)
	{
		return (frameDependent ? AsyncInputManager.frameDependentKeyUpMask : AsyncInputManager.keyUpMask).Contains(keyCode);
	}
}
