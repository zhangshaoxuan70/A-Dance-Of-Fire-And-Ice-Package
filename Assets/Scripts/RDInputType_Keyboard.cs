using System;
using System.Collections.Generic;
using UnityEngine;

public class RDInputType_Keyboard : RDInputType
{
	private static readonly KeyCode[] pausedKeys = new KeyCode[12]
	{
		KeyCode.Q,
		KeyCode.LeftArrow,
		KeyCode.RightArrow,
		KeyCode.UpArrow,
		KeyCode.DownArrow,
		KeyCode.Mouse0,
		KeyCode.Mouse1,
		KeyCode.Mouse2,
		KeyCode.Mouse3,
		KeyCode.Mouse4,
		KeyCode.Mouse5,
		KeyCode.Mouse6
	};

	private static readonly KeyCode[] SpecialKeys = new KeyCode[22]
	{
		KeyCode.Print,
		KeyCode.SysReq,
		KeyCode.LeftAlt,
		KeyCode.RightAlt,
		KeyCode.LeftWindows,
		KeyCode.RightWindows,
		KeyCode.LeftMeta,
		KeyCode.RightMeta,
		KeyCode.Tab,
		KeyCode.End,
		KeyCode.F1,
		KeyCode.F2,
		KeyCode.F3,
		KeyCode.F4,
		KeyCode.F5,
		KeyCode.F6,
		KeyCode.F7,
		KeyCode.F8,
		KeyCode.F9,
		KeyCode.F10,
		KeyCode.F11,
		KeyCode.F12
	};

	public static readonly KeyCode[] LevelSelectKeys = new KeyCode[14]
	{
		KeyCode.BackQuote,
		KeyCode.Alpha0,
		KeyCode.Alpha1,
		KeyCode.Alpha2,
		KeyCode.Alpha3,
		KeyCode.Alpha4,
		KeyCode.Alpha5,
		KeyCode.Alpha6,
		KeyCode.Alpha7,
		KeyCode.AltGr,
		KeyCode.LeftArrow,
		KeyCode.RightArrow,
		KeyCode.UpArrow,
		KeyCode.DownArrow
	};

	public static readonly KeyCode[] CLSKeys = new KeyCode[11]
	{
		KeyCode.UpArrow,
		KeyCode.DownArrow,
		KeyCode.LeftArrow,
		KeyCode.RightArrow,
		KeyCode.R,
		KeyCode.S,
		KeyCode.Delete,
		KeyCode.I,
		KeyCode.F,
		KeyCode.O,
		KeyCode.N
	};

	public static readonly KeyCode[] MouseKeys = new KeyCode[7]
	{
		KeyCode.Mouse0,
		KeyCode.Mouse1,
		KeyCode.Mouse2,
		KeyCode.Mouse3,
		KeyCode.Mouse4,
		KeyCode.Mouse5,
		KeyCode.Mouse6
	};

	private static KeyCode[] mainKeys;

	public RDInputType_Keyboard(int schemeIndex)
	{
		base.schemeIndex = schemeIndex;
		List<KeyCode> list = new List<KeyCode>();
		for (int i = 0; i <= 329; i++)
		{
			if ((i <= 322 || !ADOBase.isMobile) && (i < 320 || i > 322))
			{
				list.Add((KeyCode)i);
			}
		}
		mainKeys = list.ToArray();
	}

	private static bool CheckKeyState(KeyCode key, ButtonState state = ButtonState.WentDown)
	{
		switch (state)
		{
		case ButtonState.WentDown:
			return UnityEngine.Input.GetKeyDown(key);
		case ButtonState.WentUp:
			return UnityEngine.Input.GetKeyUp(key);
		case ButtonState.IsDown:
			return UnityEngine.Input.GetKey(key);
		case ButtonState.IsUp:
			return !Input.GetKey(key);
		default:
			return false;
		}
	}

	public override int Main(ButtonState state)
	{
		if (!isActive)
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
		List<KeyCode> keys = new List<KeyCode>();
		Array.ForEach(mainKeys, delegate(KeyCode key)
		{
			if (CheckKeyState(key, state))
			{
				keys.Add(key);
			}
		});
		if (state == ButtonState.WentDown)
		{
			foreach (KeyCode item in CountSpecialInput())
			{
				keys.Remove(item);
			}
		}
		foreach (KeyCode item2 in keys)
		{
			stateCount.keys.Add(item2);
		}
		return stateCount.keys.Count;
	}

	public List<KeyCode> CountSpecialInput()
	{
		List<KeyCode> keys = new List<KeyCode>();
		if (!isActive)
		{
			return keys;
		}
		if (Cancel())
		{
			keys.Add(KeyCode.Escape);
		}
		if (!base.isPlaying)
		{
			Array.ForEach(pausedKeys, delegate(KeyCode key)
			{
				if (CheckKeyState(key))
				{
					keys.Add(key);
				}
			});
		}
		Array.ForEach(SpecialKeys, delegate(KeyCode key)
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
				keys.Add(KeyCode.LeftArrow);
			}
			else if (Right(ButtonState.WentDown))
			{
				keys.Add(KeyCode.RightArrow);
			}
		}
		bool flag = ADOBase.sceneName.StartsWith("scnTaroMenu");
		if ((bool)scnLevelSelect.instance | flag)
		{
			Array.ForEach(LevelSelectKeys, delegate(KeyCode key)
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
				Array.ForEach(CLSKeys, delegate(KeyCode key)
				{
					if (CheckKeyState(key))
					{
						keys.Add(key);
					}
				});
			}
			if (instance.optionsPanels.showingAnyPanel)
			{
				Array.ForEach(MouseKeys, delegate(KeyCode key)
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
		return UnityEngine.Input.GetKeyDown(KeyCode.R);
	}

	public override bool Cancel()
	{
		return UnityEngine.Input.GetKeyDown(KeyCode.Escape);
	}

	public override bool Quit()
	{
		return UnityEngine.Input.GetKeyDown(KeyCode.Q);
	}

	public override bool Left(ButtonState state)
	{
		return CheckKeyState(KeyCode.LeftArrow, state);
	}

	public override bool Right(ButtonState state)
	{
		return CheckKeyState(KeyCode.RightArrow, state);
	}

	public override bool Up(ButtonState state)
	{
		return CheckKeyState(KeyCode.UpArrow, state);
	}

	public override bool Down(ButtonState state)
	{
		return CheckKeyState(KeyCode.DownArrow, state);
	}

	public override bool LeftAlt(ButtonState state)
	{
		return CheckKeyState(KeyCode.LeftShift, state);
	}

	public override bool RightAlt(ButtonState state)
	{
		return CheckKeyState(KeyCode.RightShift, state);
	}

	public override bool UpAlt(ButtonState state)
	{
		return false;
	}

	public override bool DownAlt(ButtonState state)
	{
		return false;
	}
}
