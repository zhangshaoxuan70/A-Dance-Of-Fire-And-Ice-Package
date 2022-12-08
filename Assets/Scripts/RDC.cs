public class RDC
{
	private const bool restrictDebugToDiscordUsernames = true;

	private static bool _practice;

	public static RDConstants data => RDConstants.data;

	public static bool debug
	{
		get
		{
			return data.debug;
		}
		set
		{
			if (GCS.IsDev())
			{
				data.debug = value;
			}
		}
	}

	public static bool auto
	{
		get
		{
			return data.auto;
		}
		set
		{
			data.auto = value;
		}
	}

	public static bool practice
	{
		get
		{
			return GCS.practiceMode;
		}
		set
		{
			if (value != _practice)
			{
				_practice = value;
				GCS.practiceMode = value;
			}
		}
	}

	public static bool noHud
	{
		get
		{
			return data.noHud;
		}
		set
		{
			data.noHud = value;
		}
	}

	public static bool forceMobile
	{
		get
		{
			return data.forceMobile;
		}
		set
		{
			data.forceMobile = value;
		}
	}

	public static bool useOldAuto
	{
		get
		{
			return data.useOldAuto;
		}
		set
		{
			data.useOldAuto = value;
		}
	}

	public static bool skipCutscenes
	{
		get
		{
			return data.skipCutscenes;
		}
		set
		{
			data.skipCutscenes = value;
		}
	}

	public static bool customCheckpoint
	{
		get
		{
			return data.customCheckpoint;
		}
		set
		{
			data.customCheckpoint = value;
		}
	}

	public static int customCheckpointPos
	{
		get
		{
			return data.customCheckpointPos;
		}
		set
		{
			data.customCheckpointPos = value;
		}
	}

	public static bool forceNoSteamworks
	{
		get
		{
			return data.forceNoSteamworks;
		}
		set
		{
			data.forceNoSteamworks = value;
		}
	}

	public static bool hideTaroGimmicks
	{
		get
		{
			return data.hideTaroGimmicks;
		}
		set
		{
			data.hideTaroGimmicks = value;
		}
	}

	public static bool deleteSavedProgress
	{
		get
		{
			return data.deleteSavedProgress;
		}
		set
		{
			data.deleteSavedProgress = value;
		}
	}

	public static bool runningOnSteamDeck
	{
		get
		{
			return data.runningOnSteamDeck;
		}
		set
		{
			data.runningOnSteamDeck = value;
		}
	}

	public static bool useAsyncInput
	{
		get
		{
			return AsyncInputManager.isActive;
		}
		set
		{
			AsyncInputManager.ToggleHook(value);
		}
	}
}
