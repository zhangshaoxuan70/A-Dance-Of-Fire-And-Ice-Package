using SharpHook.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RDInputType_AsyncKeyboard : RDInputType
{
	private static readonly SharpHook.Native.KeyCode[] pausedKeys;

	private static readonly SharpHook.Native.KeyCode[] SpecialKeys;

	public static readonly SharpHook.Native.KeyCode[] LevelSelectKeys;

	public static readonly SharpHook.Native.KeyCode[] CLSKeys;

	public static readonly SharpHook.Native.KeyCode[] MouseKeys;

	public static readonly ushort[] AllAsyncKeys;

	public RDInputType_AsyncKeyboard(int schemeIndex)
	{
		base.schemeIndex = schemeIndex;
		isActive = false;
	}

	private static bool CheckKeyState(SharpHook.Native.KeyCode key, ButtonState state = ButtonState.WentDown)
	{
		switch (state)
		{
		case ButtonState.WentDown:
			return AsyncInput.GetKeyDown(key);
		case ButtonState.WentUp:
			return AsyncInput.GetKeyUp(key);
		case ButtonState.IsDown:
			return AsyncInput.GetKey(key);
		case ButtonState.IsUp:
			return !AsyncInput.GetKey(key);
		default:
			return false;
		}
	}

	public override int Main(ButtonState state)
	{
		if (!isActive || !Application.isFocused)
		{
			return 0;
		}
		MainStateCount stateCount = GetStateCount(state);
		if (stateCount.lastFrameUpdated == Time.frameCount)
		{
			return stateCount.keys.Count;
		}
		stateCount.lastFrameUpdated = Time.frameCount;
		stateCount.keys = new List<object>();
		HashSet<ushort> hashSet = new HashSet<ushort>();
		switch (state)
		{
		case ButtonState.IsDown:
			hashSet = AsyncInputManager.keyMask;
			break;
		case ButtonState.IsUp:
			hashSet = AllAsyncKeys.Except(AsyncInputManager.keyMask).ToHashSet();
			break;
		case ButtonState.WentDown:
			hashSet = AsyncInputManager.keyDownMask;
			break;
		case ButtonState.WentUp:
			hashSet = AsyncInputManager.keyUpMask;
			break;
		}
		if (state == ButtonState.WentDown)
		{
			foreach (SharpHook.Native.KeyCode item in CountSpecialInput())
			{
				hashSet.Remove((ushort)item);
			}
		}
		foreach (ushort item2 in hashSet)
		{
			stateCount.keys.Add(item2);
		}
		return stateCount.keys.Count;
	}

	public List<SharpHook.Native.KeyCode> CountSpecialInput()
	{
		List<SharpHook.Native.KeyCode> keys = new List<SharpHook.Native.KeyCode>();
		if (Cancel())
		{
			keys.Add(SharpHook.Native.KeyCode.VcEscape);
		}
		if (!base.isPlaying)
		{
			Array.ForEach(pausedKeys, delegate(SharpHook.Native.KeyCode key)
			{
				if (CheckKeyState(key))
				{
					keys.Add(key);
				}
			});
		}
		Array.ForEach(SpecialKeys, delegate(SharpHook.Native.KeyCode key)
		{
			if (CheckKeyState(key))
			{
				keys.Add(key);
			}
		});
		if (base.controller.currentState != States.PlayerControl && !ADOBase.isEditingLevel && ADOBase.uiController.difficultyUIMode != 0)
		{
			if (Left(ButtonState.WentDown))
			{
				keys.Add(SharpHook.Native.KeyCode.VcNumPadLeft);
			}
			else if (Right(ButtonState.WentDown))
			{
				keys.Add(SharpHook.Native.KeyCode.VcNumPadRight);
			}
		}
		bool flag = ADOBase.sceneName.StartsWith("scnTaroMenu");
		if ((bool)scnLevelSelect.instance | flag)
		{
			Array.ForEach(LevelSelectKeys, delegate(SharpHook.Native.KeyCode key)
			{
				if (CheckKeyState(key))
				{
					keys.Add(key);
				}
			});
		}
		if (ADOBase.isCLS)
		{
			scnCLS instance = scnCLS.instance;
			if (!instance.showingInitialMenu)
			{
				Array.ForEach(CLSKeys, delegate(SharpHook.Native.KeyCode key)
				{
					if (CheckKeyState(key))
					{
						keys.Add(key);
					}
				});
			}
			if (instance.optionsPanels.showingAnyPanel)
			{
				Array.ForEach(MouseKeys, delegate(SharpHook.Native.KeyCode key)
				{
					if (CheckKeyState(key))
					{
						keys.Add(key);
					}
				});
			}
		}
		return keys;
	}

	public override bool Restart()
	{
		return AsyncInput.GetKeyDown(SharpHook.Native.KeyCode.VcR);
	}

	public override bool Cancel()
	{
		return AsyncInput.GetKeyDown(SharpHook.Native.KeyCode.VcEscape);
	}

	public override bool Quit()
	{
		return AsyncInput.GetKeyDown(SharpHook.Native.KeyCode.VcQ);
	}

	public override bool Left(ButtonState state)
	{
		return CheckKeyState(SharpHook.Native.KeyCode.VcNumPadLeft, state);
	}

	public override bool Right(ButtonState state)
	{
		return CheckKeyState(SharpHook.Native.KeyCode.VcNumPadRight, state);
	}

	public override bool Up(ButtonState state)
	{
		return CheckKeyState(SharpHook.Native.KeyCode.VcNumPadUp, state);
	}

	public override bool Down(ButtonState state)
	{
		return CheckKeyState(SharpHook.Native.KeyCode.VcNumPadDown, state);
	}

	public override bool LeftAlt(ButtonState state)
	{
		return CheckKeyState(SharpHook.Native.KeyCode.VcLeftShift, state);
	}

	public override bool RightAlt(ButtonState state)
	{
		return CheckKeyState(SharpHook.Native.KeyCode.VcRightShift, state);
	}

	public override bool UpAlt(ButtonState state)
	{
		return false;
	}

	public override bool DownAlt(ButtonState state)
	{
		return false;
	}

	static RDInputType_AsyncKeyboard()
	{
		SharpHook.Native.KeyCode[] obj = new SharpHook.Native.KeyCode[10]
		{
			SharpHook.Native.KeyCode.VcQ,
			SharpHook.Native.KeyCode.VcNumPadLeft,
			SharpHook.Native.KeyCode.VcNumPadRight,
			SharpHook.Native.KeyCode.VcNumPadUp,
			SharpHook.Native.KeyCode.VcNumPadDown,
			SharpHook.Native.KeyCode.VcUndefined,
			SharpHook.Native.KeyCode.VcUndefined,
			SharpHook.Native.KeyCode.VcUndefined,
			SharpHook.Native.KeyCode.VcUndefined,
			SharpHook.Native.KeyCode.VcUndefined
		};
		obj[5] = (SharpHook.Native.KeyCode)(1 + AsyncInput.MouseButtonCodeOffset);
		obj[6] = (SharpHook.Native.KeyCode)(2 + AsyncInput.MouseButtonCodeOffset);
		obj[7] = (SharpHook.Native.KeyCode)(3 + AsyncInput.MouseButtonCodeOffset);
		obj[8] = (SharpHook.Native.KeyCode)(4 + AsyncInput.MouseButtonCodeOffset);
		obj[9] = (SharpHook.Native.KeyCode)(5 + AsyncInput.MouseButtonCodeOffset);
		pausedKeys = obj;
		SpecialKeys = new SharpHook.Native.KeyCode[18]
		{
			SharpHook.Native.KeyCode.VcPrintscreen,
			SharpHook.Native.KeyCode.VcLeftAlt,
			SharpHook.Native.KeyCode.VcRightAlt,
			SharpHook.Native.KeyCode.VcLeftMeta,
			SharpHook.Native.KeyCode.VcTab,
			SharpHook.Native.KeyCode.VcNumPadEnd,
			SharpHook.Native.KeyCode.VcF1,
			SharpHook.Native.KeyCode.VcF2,
			SharpHook.Native.KeyCode.VcF3,
			SharpHook.Native.KeyCode.VcF4,
			SharpHook.Native.KeyCode.VcF5,
			SharpHook.Native.KeyCode.VcF6,
			SharpHook.Native.KeyCode.VcF7,
			SharpHook.Native.KeyCode.VcF8,
			SharpHook.Native.KeyCode.VcF9,
			SharpHook.Native.KeyCode.VcF10,
			SharpHook.Native.KeyCode.VcF11,
			SharpHook.Native.KeyCode.VcF12
		};
		LevelSelectKeys = new SharpHook.Native.KeyCode[13]
		{
			SharpHook.Native.KeyCode.VcBackquote,
			SharpHook.Native.KeyCode.Vc0,
			SharpHook.Native.KeyCode.Vc1,
			SharpHook.Native.KeyCode.Vc2,
			SharpHook.Native.KeyCode.Vc3,
			SharpHook.Native.KeyCode.Vc4,
			SharpHook.Native.KeyCode.Vc5,
			SharpHook.Native.KeyCode.Vc6,
			SharpHook.Native.KeyCode.Vc7,
			SharpHook.Native.KeyCode.VcNumPadLeft,
			SharpHook.Native.KeyCode.VcNumPadRight,
			SharpHook.Native.KeyCode.VcNumPadUp,
			SharpHook.Native.KeyCode.VcNumPadDown
		};
		CLSKeys = new SharpHook.Native.KeyCode[11]
		{
			SharpHook.Native.KeyCode.VcNumPadLeft,
			SharpHook.Native.KeyCode.VcNumPadRight,
			SharpHook.Native.KeyCode.VcNumPadUp,
			SharpHook.Native.KeyCode.VcNumPadDown,
			SharpHook.Native.KeyCode.VcR,
			SharpHook.Native.KeyCode.VcS,
			SharpHook.Native.KeyCode.VcNumPadDelete,
			SharpHook.Native.KeyCode.VcI,
			SharpHook.Native.KeyCode.VcF,
			SharpHook.Native.KeyCode.VcO,
			SharpHook.Native.KeyCode.VcN
		};
		MouseKeys = new SharpHook.Native.KeyCode[5]
		{
			(SharpHook.Native.KeyCode)(1 + AsyncInput.MouseButtonCodeOffset),
			(SharpHook.Native.KeyCode)(2 + AsyncInput.MouseButtonCodeOffset),
			(SharpHook.Native.KeyCode)(3 + AsyncInput.MouseButtonCodeOffset),
			(SharpHook.Native.KeyCode)(4 + AsyncInput.MouseButtonCodeOffset),
			(SharpHook.Native.KeyCode)(5 + AsyncInput.MouseButtonCodeOffset)
		};
		AllAsyncKeys = (from SharpHook.Native.KeyCode e in Enum.GetValues(typeof(SharpHook.Native.KeyCode))
			select (ushort)e).Concat(from MouseButton e in Enum.GetValues(typeof(MouseButton))
			select (ushort)(e + AsyncInput.MouseButtonCodeOffset)).ToArray();
	}
}
