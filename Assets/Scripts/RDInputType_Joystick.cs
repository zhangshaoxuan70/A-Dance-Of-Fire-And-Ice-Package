using Rewired;
using Rewired.Data;
using System.Collections.Generic;
using UnityEngine;

public class RDInputType_Joystick : RDInputType
{
	private Player player;

	private Dictionary<string, string[]> mapCategoryMains;

	private string currentMapCategoryName;

	private string restartActionName;

	private string cancelActionName;

	private string leftActionName;

	private string rightActionName;

	private string upActionName;

	private string downActionName;

	private string leftAltActionName;

	private string rightAltActionName;

	private string upAltActionName;

	private string downAltActionName;

	public RDInputType_Joystick(int schemeIndex)
	{
		base.schemeIndex = schemeIndex;
		player = ReInput.players.GetPlayer(schemeIndex);
		mapCategoryMains = new Dictionary<string, string[]>();
		UserData userData = RDC.data.prefab_rewiredManager.userData;
		foreach (Player_Editor.Mapping defaultJoystickMap in userData.GetPlayer(schemeIndex + 1).defaultJoystickMaps)
		{
			string[] sortedActionNamesInCategory = userData.GetSortedActionNamesInCategory(defaultJoystickMap.categoryId);
			List<string> list = new List<string>();
			string[] array = sortedActionNamesInCategory;
			foreach (string text in array)
			{
				if (text.StartsWith("Main"))
				{
					list.Add(text);
				}
			}
			string mapCategoryNameById = userData.GetMapCategoryNameById(defaultJoystickMap.categoryId);
			mapCategoryMains.Add(mapCategoryNameById, list.ToArray());
			if (defaultJoystickMap.enabled)
			{
				SetMapCategoryName(userData.GetMapCategoryNameById(defaultJoystickMap.categoryId));
			}
		}
	}

	public void SetMapping(string newMapName)
	{
		player.controllers.maps.SetAllMapsEnabled(state: false);
		player.controllers.maps.SetMapsEnabled(state: true, newMapName);
		SetMapCategoryName(newMapName);
	}

	private void SetMapCategoryName(string newMapName)
	{
		currentMapCategoryName = newMapName;
		restartActionName = "Restart" + currentMapCategoryName;
		cancelActionName = "Cancel" + currentMapCategoryName;
		leftActionName = "Left" + currentMapCategoryName;
		rightActionName = "Right" + currentMapCategoryName;
		upActionName = "Up" + currentMapCategoryName;
		downActionName = "Down" + currentMapCategoryName;
		leftAltActionName = "LeftAlt" + currentMapCategoryName;
		rightAltActionName = "RightAlt" + currentMapCategoryName;
		upAltActionName = "UpAlt" + currentMapCategoryName;
		downAltActionName = "DownAlt" + currentMapCategoryName;
	}

	private bool GetActionState(string actionName, ButtonState state = ButtonState.WentDown)
	{
		bool flag = false;
		switch (state)
		{
		case ButtonState.WentDown:
			return player.GetButtonDown(actionName);
		case ButtonState.WentUp:
			return player.GetButtonUp(actionName);
		case ButtonState.IsUp:
			return !player.GetButton(actionName);
		case ButtonState.IsDown:
			return player.GetButton(actionName);
		default:
			return false;
		}
	}

	public override int Main(ButtonState state)
	{
		MainStateCount stateCount = GetStateCount(state);
		if (player.controllers.Joysticks.Count == 0)
		{
			return 0;
		}
		if (stateCount.lastFrameUpdated == Time.frameCount)
		{
			return stateCount.keys.Count;
		}
		stateCount.lastFrameUpdated = Time.frameCount;
		stateCount.keys = new List<object>();
		string[] array = mapCategoryMains[currentMapCategoryName];
		foreach (string text in array)
		{
			if (GetActionState(text, state))
			{
				stateCount.keys.Add(text);
			}
		}
		return stateCount.keys.Count;
	}

	public override bool Restart()
	{
		return GetActionState(restartActionName);
	}

	public override bool Cancel()
	{
		return GetActionState(cancelActionName);
	}

	public override bool Quit()
	{
		return false;
	}

	public override bool Left(ButtonState state)
	{
		return GetActionState(leftActionName, state);
	}

	public override bool Right(ButtonState state)
	{
		return GetActionState(rightActionName, state);
	}

	public override bool Up(ButtonState state)
	{
		return GetActionState(upActionName, state);
	}

	public override bool Down(ButtonState state)
	{
		return GetActionState(downActionName, state);
	}

	public override bool LeftAlt(ButtonState state)
	{
		return GetActionState(leftAltActionName, state);
	}

	public override bool RightAlt(ButtonState state)
	{
		return GetActionState(rightAltActionName, state);
	}

	public override bool UpAlt(ButtonState state)
	{
		return GetActionState(upAltActionName, state);
	}

	public override bool DownAlt(ButtonState state)
	{
		return GetActionState(downAltActionName, state);
	}
}
