using System;
using System.Collections.Generic;
using UnityEngine;

public class KeyCodeConverter
{
	private static readonly IReadOnlyDictionary<ushort, ushort> NativeKeyCodeMapper;

	public static readonly IReadOnlyList<Tuple<KeyCode, ushort>> UnityNativeKeyCodeList;

	static KeyCodeConverter()
	{
		NativeKeyCodeMapper = InitNativeKeyCodeMapper();
		UnityNativeKeyCodeList = InitUnityNativeKeyCodeList();
	}

	private static List<Tuple<KeyCode, ushort>> InitUnityNativeKeyCodeList()
	{
		return new List<Tuple<KeyCode, ushort>>
		{
			new Tuple<KeyCode, ushort>(KeyCode.None, 0),
			new Tuple<KeyCode, ushort>(KeyCode.Escape, 1),
			new Tuple<KeyCode, ushort>(KeyCode.Alpha1, 2),
			new Tuple<KeyCode, ushort>(KeyCode.Alpha2, 3),
			new Tuple<KeyCode, ushort>(KeyCode.Alpha3, 4),
			new Tuple<KeyCode, ushort>(KeyCode.Alpha4, 5),
			new Tuple<KeyCode, ushort>(KeyCode.Alpha5, 6),
			new Tuple<KeyCode, ushort>(KeyCode.Alpha6, 7),
			new Tuple<KeyCode, ushort>(KeyCode.Alpha7, 8),
			new Tuple<KeyCode, ushort>(KeyCode.Alpha8, 9),
			new Tuple<KeyCode, ushort>(KeyCode.Alpha9, 10),
			new Tuple<KeyCode, ushort>(KeyCode.Alpha0, 11),
			new Tuple<KeyCode, ushort>(KeyCode.Minus, 12),
			new Tuple<KeyCode, ushort>(KeyCode.Equals, 13),
			new Tuple<KeyCode, ushort>(KeyCode.Backspace, 14),
			new Tuple<KeyCode, ushort>(KeyCode.Tab, 15),
			new Tuple<KeyCode, ushort>(KeyCode.Q, 16),
			new Tuple<KeyCode, ushort>(KeyCode.W, 17),
			new Tuple<KeyCode, ushort>(KeyCode.E, 18),
			new Tuple<KeyCode, ushort>(KeyCode.R, 19),
			new Tuple<KeyCode, ushort>(KeyCode.T, 20),
			new Tuple<KeyCode, ushort>(KeyCode.Y, 21),
			new Tuple<KeyCode, ushort>(KeyCode.U, 22),
			new Tuple<KeyCode, ushort>(KeyCode.I, 23),
			new Tuple<KeyCode, ushort>(KeyCode.O, 24),
			new Tuple<KeyCode, ushort>(KeyCode.P, 25),
			new Tuple<KeyCode, ushort>(KeyCode.LeftBracket, 26),
			new Tuple<KeyCode, ushort>(KeyCode.RightBracket, 27),
			new Tuple<KeyCode, ushort>(KeyCode.Return, 28),
			new Tuple<KeyCode, ushort>(KeyCode.LeftControl, 29),
			new Tuple<KeyCode, ushort>(KeyCode.A, 30),
			new Tuple<KeyCode, ushort>(KeyCode.S, 31),
			new Tuple<KeyCode, ushort>(KeyCode.D, 32),
			new Tuple<KeyCode, ushort>(KeyCode.F, 33),
			new Tuple<KeyCode, ushort>(KeyCode.G, 34),
			new Tuple<KeyCode, ushort>(KeyCode.H, 35),
			new Tuple<KeyCode, ushort>(KeyCode.J, 36),
			new Tuple<KeyCode, ushort>(KeyCode.K, 37),
			new Tuple<KeyCode, ushort>(KeyCode.L, 38),
			new Tuple<KeyCode, ushort>(KeyCode.Semicolon, 39),
			new Tuple<KeyCode, ushort>(KeyCode.Quote, 40),
			new Tuple<KeyCode, ushort>(KeyCode.BackQuote, 41),
			new Tuple<KeyCode, ushort>(KeyCode.LeftShift, 42),
			new Tuple<KeyCode, ushort>(KeyCode.Backslash, 43),
			new Tuple<KeyCode, ushort>(KeyCode.Z, 44),
			new Tuple<KeyCode, ushort>(KeyCode.X, 45),
			new Tuple<KeyCode, ushort>(KeyCode.C, 46),
			new Tuple<KeyCode, ushort>(KeyCode.V, 47),
			new Tuple<KeyCode, ushort>(KeyCode.B, 48),
			new Tuple<KeyCode, ushort>(KeyCode.N, 49),
			new Tuple<KeyCode, ushort>(KeyCode.M, 50),
			new Tuple<KeyCode, ushort>(KeyCode.Comma, 51),
			new Tuple<KeyCode, ushort>(KeyCode.Period, 52),
			new Tuple<KeyCode, ushort>(KeyCode.Slash, 53),
			new Tuple<KeyCode, ushort>(KeyCode.RightShift, 54),
			new Tuple<KeyCode, ushort>(KeyCode.KeypadMultiply, 55),
			new Tuple<KeyCode, ushort>(KeyCode.LeftAlt, 56),
			new Tuple<KeyCode, ushort>(KeyCode.Space, 57),
			new Tuple<KeyCode, ushort>(KeyCode.CapsLock, 58),
			new Tuple<KeyCode, ushort>(KeyCode.F1, 59),
			new Tuple<KeyCode, ushort>(KeyCode.F2, 60),
			new Tuple<KeyCode, ushort>(KeyCode.F3, 61),
			new Tuple<KeyCode, ushort>(KeyCode.F4, 62),
			new Tuple<KeyCode, ushort>(KeyCode.F5, 63),
			new Tuple<KeyCode, ushort>(KeyCode.F6, 64),
			new Tuple<KeyCode, ushort>(KeyCode.F7, 65),
			new Tuple<KeyCode, ushort>(KeyCode.F8, 66),
			new Tuple<KeyCode, ushort>(KeyCode.F9, 67),
			new Tuple<KeyCode, ushort>(KeyCode.F10, 68),
			new Tuple<KeyCode, ushort>(KeyCode.Numlock, 69),
			new Tuple<KeyCode, ushort>(KeyCode.ScrollLock, 70),
			new Tuple<KeyCode, ushort>(KeyCode.Keypad7, 71),
			new Tuple<KeyCode, ushort>(KeyCode.Keypad8, 72),
			new Tuple<KeyCode, ushort>(KeyCode.Keypad9, 73),
			new Tuple<KeyCode, ushort>(KeyCode.KeypadMinus, 74),
			new Tuple<KeyCode, ushort>(KeyCode.Keypad4, 75),
			new Tuple<KeyCode, ushort>(KeyCode.Keypad5, 76),
			new Tuple<KeyCode, ushort>(KeyCode.Keypad6, 77),
			new Tuple<KeyCode, ushort>(KeyCode.KeypadPlus, 78),
			new Tuple<KeyCode, ushort>(KeyCode.Keypad1, 79),
			new Tuple<KeyCode, ushort>(KeyCode.Keypad2, 80),
			new Tuple<KeyCode, ushort>(KeyCode.Keypad3, 81),
			new Tuple<KeyCode, ushort>(KeyCode.Keypad0, 82),
			new Tuple<KeyCode, ushort>(KeyCode.KeypadPeriod, 83),
			new Tuple<KeyCode, ushort>(KeyCode.F11, 87),
			new Tuple<KeyCode, ushort>(KeyCode.F12, 88),
			new Tuple<KeyCode, ushort>(KeyCode.F13, 91),
			new Tuple<KeyCode, ushort>(KeyCode.F14, 92),
			new Tuple<KeyCode, ushort>(KeyCode.F15, 93),
			new Tuple<KeyCode, ushort>(KeyCode.Underscore, 115),
			new Tuple<KeyCode, ushort>(KeyCode.KeypadEquals, 3597),
			new Tuple<KeyCode, ushort>(KeyCode.RightControl, 3613),
			new Tuple<KeyCode, ushort>(KeyCode.KeypadDivide, 3637),
			new Tuple<KeyCode, ushort>(KeyCode.Print, 3639),
			new Tuple<KeyCode, ushort>(KeyCode.RightAlt, 3640),
			new Tuple<KeyCode, ushort>(KeyCode.Pause, 3653),
			new Tuple<KeyCode, ushort>(KeyCode.LeftMeta, 3675),
			new Tuple<KeyCode, ushort>(KeyCode.RightMeta, 3676),
			new Tuple<KeyCode, ushort>(KeyCode.Menu, 3677),
			new Tuple<KeyCode, ushort>(KeyCode.Clear, 28),
			new Tuple<KeyCode, ushort>(KeyCode.KeypadEnter, 3612),
			new Tuple<KeyCode, ushort>(KeyCode.LeftMeta, 3675),
			new Tuple<KeyCode, ushort>(KeyCode.UpArrow, 61000),
			new Tuple<KeyCode, ushort>(KeyCode.DownArrow, 61008),
			new Tuple<KeyCode, ushort>(KeyCode.LeftArrow, 61003),
			new Tuple<KeyCode, ushort>(KeyCode.RightArrow, 61005),
			new Tuple<KeyCode, ushort>(KeyCode.Insert, 61010),
			new Tuple<KeyCode, ushort>(KeyCode.Home, 60999),
			new Tuple<KeyCode, ushort>(KeyCode.PageUp, 61001),
			new Tuple<KeyCode, ushort>(KeyCode.Delete, 61011),
			new Tuple<KeyCode, ushort>(KeyCode.End, 61007),
			new Tuple<KeyCode, ushort>(KeyCode.PageDown, 61009),
			new Tuple<KeyCode, ushort>(KeyCode.Mouse0, (ushort)(AsyncInput.MouseButtonCodeOffset + 1)),
			new Tuple<KeyCode, ushort>(KeyCode.Mouse1, (ushort)(AsyncInput.MouseButtonCodeOffset + 2)),
			new Tuple<KeyCode, ushort>(KeyCode.Mouse2, (ushort)(AsyncInput.MouseButtonCodeOffset + 3)),
			new Tuple<KeyCode, ushort>(KeyCode.Mouse3, (ushort)(AsyncInput.MouseButtonCodeOffset + 4)),
			new Tuple<KeyCode, ushort>(KeyCode.Mouse4, (ushort)(AsyncInput.MouseButtonCodeOffset + 5))
		};
	}

	private static Dictionary<ushort, ushort> InitNativeKeyCodeMapper()
	{
		return new Dictionary<ushort, ushort>
		{
			[3667] = 61011,
			[3666] = 61010,
			[3663] = 61007,
			[57424] = 61008,
			[3665] = 61009,
			[57419] = 61003,
			[57421] = 61005,
			[3655] = 60999,
			[57416] = 61000,
			[3657] = 61001
		};
	}

	public static ushort ConvertNativeKeyCode(ushort keyCode)
	{
		if (!NativeKeyCodeMapper.TryGetValue(keyCode, out ushort value))
		{
			return keyCode;
		}
		return value;
	}
}
