using System;
using System.Collections.Generic;
using UnityEngine;

public static class AsyncInputUtils
{
	public static double GetSongPosition(scrConductor conductor, long nowTick)
	{
		if (!GCS.d_oldConductor && !GCS.d_webglConductor)
		{
			return ((double)nowTick / 10000000.0 - AsyncInputManager.dspTimeSong - (double)scrConductor.calibration_i) * (double)conductor.song.pitch - conductor.addoffset;
		}
		return (double)(conductor.song.time - scrConductor.calibration_i) - conductor.addoffset / (double)conductor.song.pitch;
	}

	public static double GetAngle(scrPlanet planet, double snappedLastAngle, long nowTick)
	{
		return snappedLastAngle + (GetSongPosition(planet.conductor, nowTick) - planet.conductor.lastHit) / planet.conductor.crotchet * 3.141592653598793 * planet.controller.speed * (double)(planet.controller.isCW ? 1 : (-1));
	}

	public static void AdjustAngle(scrController controller, long targetTick)
	{
		if (AsyncInputManager.isActive && AsyncInputManager.offsetTickUpdated)
		{
			AsyncInputManager.targetSongTick = targetTick - AsyncInputManager.offsetTick;
			controller.chosenplanet.AsyncRefreshAngles();
		}
	}

	public static void WhileFloorNotChangeOrOnceIfAsyncInputDisabled(scrController controller, Action action)
	{
		if (!AsyncInputManager.isActive)
		{
			action();
			return;
		}
		int num = -1;
		while (num != controller.currFloor.seqID)
		{
			num = controller.currFloor.seqID;
			action();
		}
	}

	public static int GetValidKeyCount(IReadOnlyList<KeyCode> keyCodes)
	{
		int num = 0;
		for (int i = 0; i < keyCodes.Count; i++)
		{
			if (++num > 4)
			{
				break;
			}
		}
		return num;
	}
}
