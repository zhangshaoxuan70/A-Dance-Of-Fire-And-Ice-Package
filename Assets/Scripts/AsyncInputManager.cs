using RDTools;
using SharpHook;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public static class AsyncInputManager
{
	public static ConcurrentQueue<KeyEvent> keyQueue = new ConcurrentQueue<KeyEvent>();

	public static long currFrameTick;

	public static long targetSongTick;

	public static long prevFrameTick;

	public static long offsetTick;

	public static bool offsetTickUpdated = false;

	public static double dspTime;

	public static double dspTimeSong;

	public static double lastReportedDspTime;

	public static double previousFrameTime;

	public static Thread hookThread;

	private static IGlobalHook hook;

	public static readonly HashSet<ushort> keyMask = new HashSet<ushort>();

	public static readonly HashSet<ushort> keyDownMask = new HashSet<ushort>();

	public static readonly HashSet<ushort> keyUpMask = new HashSet<ushort>();

	public static readonly HashSet<ushort> frameDependentKeyMask = new HashSet<ushort>();

	public static readonly HashSet<ushort> frameDependentKeyDownMask = new HashSet<ushort>();

	public static readonly HashSet<ushort> frameDependentKeyUpMask = new HashSet<ushort>();

	public static bool isActive
	{
		get
		{
			if (hookThread != null)
			{
				IGlobalHook globalHook = hook;
				if (globalHook == null)
				{
					return true;
				}
				return !globalHook.IsDisposed;
			}
			return false;
		}
	}

	public static void ToggleHook(bool active)
	{
		if (isActive == active)
		{
			RDBaseDll.printem($"Hook is already in desired state: {active}");
			return;
		}
		RDBaseDll.printem((active ? "Start" : "Stopp") + "ing hook..");
		if (active)
		{
			StartHook();
		}
		else
		{
			StopHook();
		}
	}

	public static void StartHook()
	{
		if (isActive)
		{
			RDBaseDll.printesw("Tried to enable hook when the hook's thread was already initialized!");
			return;
		}
		ResetHookMetadata();
		hookThread = new Thread((ThreadStart)delegate
		{
			bool flag = false;
			try
			{
				hook = new SimpleGlobalHook();
				hook.KeyPressed += OnKeyPressed;
				hook.KeyReleased += OnKeyReleased;
				hook.MousePressed += OnMousePressed;
				hook.MouseReleased += OnMouseReleased;
				hook.Run();
			}
			catch (HookException ex)
			{
				if (ex.GetBaseException() is ThreadAbortException)
				{
					flag = true;
					RDInput.OnAsyncInputToggle(asyncInputEnabled: false);
				}
				else
				{
					UnityEngine.Debug.LogError($"An exception occurred while running hook: {ex}");
				}
			}
			catch (Exception arg)
			{
				UnityEngine.Debug.LogError($"An exception occurred while running hook: {arg}");
			}
			finally
			{
				if (!flag)
				{
					StartHook();
				}
			}
		});
		RDInput.OnAsyncInputToggle(asyncInputEnabled: true);
		hookThread.Start();
	}

	public static void StopHook()
	{
		try
		{
			hook.Dispose();
			if (hookThread?.IsAlive ?? false)
			{
				hookThread.Abort();
			}
			RDInput.OnAsyncInputToggle(asyncInputEnabled: false);
		}
		catch (Exception arg)
		{
			UnityEngine.Debug.LogError($"An exception occurred while disposing hook: {arg}");
		}
	}

	private static void OnKeyPressed(object sender, KeyboardHookEventArgs e)
	{
		ushort keyCode = (ushort)e.Data.KeyCode;
		keyQueue.Enqueue(new KeyEvent(DateTime.Now.Ticks, keyCode, press: true));
	}

	private static void OnKeyReleased(object sender, KeyboardHookEventArgs e)
	{
		ushort keyCode = (ushort)e.Data.KeyCode;
		keyQueue.Enqueue(new KeyEvent(DateTime.Now.Ticks, keyCode, press: false));
	}

	private static void OnMousePressed(object sender, MouseHookEventArgs e)
	{
		ushort keyCode = (ushort)(e.Data.Button + AsyncInput.MouseButtonCodeOffset);
		keyQueue.Enqueue(new KeyEvent(DateTime.Now.Ticks, keyCode, press: true));
	}

	private static void OnMouseReleased(object sender, MouseHookEventArgs e)
	{
		ushort keyCode = (ushort)(e.Data.Button + AsyncInput.MouseButtonCodeOffset);
		keyQueue.Enqueue(new KeyEvent(DateTime.Now.Ticks, keyCode, press: false));
	}

	private static void ResetHookMetadata()
	{
		keyMask.Clear();
		keyQueue.Clear();
		keyDownMask.Clear();
		keyUpMask.Clear();
		currFrameTick = (targetSongTick = (prevFrameTick = (offsetTick = 0L)));
		offsetTickUpdated = false;
		dspTime = (dspTimeSong = 0.0);
		lastReportedDspTime = (previousFrameTime = 0.0);
	}
}
