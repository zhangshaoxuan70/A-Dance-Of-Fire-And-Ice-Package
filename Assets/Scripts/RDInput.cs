using System.Collections.Generic;
using UnityEngine;

public static class RDInput
{
	public static List<RDInputType> inputs;

	private static RDInputType p1;

	private static RDInputType p2;

	public static RDInputType_Keyboard keyboardMouseInput;

	public static RDInputType_AsyncKeyboard asyncKeyboardMouseInput;

	private static bool didSetup;

	public static bool holdingControl
	{
		get
		{
			if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl) && !Input.GetKey(KeyCode.LeftMeta))
			{
				return UnityEngine.Input.GetKey(KeyCode.RightMeta);
			}
			return true;
		}
	}

	public static bool holdingShift
	{
		get
		{
			if (!Input.GetKey(KeyCode.LeftShift))
			{
				return UnityEngine.Input.GetKey(KeyCode.RightShift);
			}
			return true;
		}
	}

	public static bool holdingAlt
	{
		get
		{
			if (!Input.GetKey(KeyCode.LeftAlt))
			{
				return UnityEngine.Input.GetKey(KeyCode.RightAlt);
			}
			return true;
		}
	}

	public static int mainPressCount => GetMain();

	public static bool mainPress => mainPressCount > 0;

	public static int mainHeldCount => GetMain(ButtonState.IsDown);

	public static bool mainHeld => mainHeldCount > 0;

	public static bool restartPress => GetState(InputAction.Restart);

	public static bool cancelPress => GetState(InputAction.Cancel);

	public static bool quitPress => GetState(InputAction.Quit);

	public static bool leftPress => GetState(InputAction.Left);

	public static bool leftIsPressed => GetState(InputAction.Left, ButtonState.IsDown);

	public static bool rightPress => GetState(InputAction.Right);

	public static bool rightIsPressed => GetState(InputAction.Right, ButtonState.IsDown);

	public static bool upPress => GetState(InputAction.Up);

	public static bool upIsPressed => GetState(InputAction.Up, ButtonState.IsDown);

	public static bool downPress => GetState(InputAction.Down);

	public static bool downIsPressed => GetState(InputAction.Down, ButtonState.IsDown);

	public static bool leftAltPress => GetState(InputAction.LeftAlt);

	public static bool leftAltIsPressed => GetState(InputAction.LeftAlt, ButtonState.IsDown);

	public static bool rightAltPress => GetState(InputAction.RightAlt);

	public static bool rightAltIsPressed => GetState(InputAction.RightAlt, ButtonState.IsDown);

	public static void Setup()
	{
		if (!didSetup)
		{
			didSetup = true;
			keyboardMouseInput = new RDInputType_Keyboard(0);
			asyncKeyboardMouseInput = new RDInputType_AsyncKeyboard(0);
			p1 = new RDInputType_Joystick(0);
			inputs = new List<RDInputType>
			{
				p1,
				keyboardMouseInput,
				asyncKeyboardMouseInput
			};
		}
	}

	public static void Reinitialize()
	{
		didSetup = false;
		Setup();
	}

	public static void SetMapping(string mapName)
	{
		((RDInputType_Joystick)p1).SetMapping(mapName);
	}

	public static bool GetState(InputAction inputAction, ButtonState state = ButtonState.WentDown)
	{
		foreach (RDInputType input in inputs)
		{
			if (input != null && input.isActive && input.Get(inputAction, state))
			{
				return true;
			}
		}
		return false;
	}

	public static int GetMain(ButtonState state = ButtonState.WentDown)
	{
		int num = 0;
		foreach (RDInputType input in inputs)
		{
			if (input.isActive)
			{
				int num2 = input.Main(state);
				num += num2;
			}
		}
		return num;
	}

	public static List<object> GetStateKeys(ButtonState state = ButtonState.WentDown)
	{
		GetMain(state);
		List<object> list = new List<object>();
		foreach (RDInputType input in inputs)
		{
			if (input.isActive)
			{
				switch (state)
				{
				case ButtonState.WentDown:
					list.AddRange(input.pressCount.keys);
					break;
				case ButtonState.IsDown:
					list.AddRange(input.heldCount.keys);
					break;
				case ButtonState.WentUp:
					list.AddRange(input.releaseCount.keys);
					break;
				case ButtonState.IsUp:
					list.AddRange(input.isReleaseCount.keys);
					break;
				}
			}
		}
		return list;
	}

	public static void OnAsyncInputToggle(bool asyncInputEnabled)
	{
		keyboardMouseInput.isActive = !asyncInputEnabled;
		asyncKeyboardMouseInput.isActive = asyncInputEnabled;
	}

	public static List<object> GetMainPressKeys()
	{
		return GetStateKeys();
	}

	public static List<object> GetMainHeldKeys()
	{
		return GetStateKeys(ButtonState.IsDown);
	}
}
