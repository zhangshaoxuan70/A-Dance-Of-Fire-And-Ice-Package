using Rewired;
using UnityEngine;

public class RewiredControllerTest : MonoBehaviour
{
	private void Start()
	{
	}

	private void LateUpdate()
	{
		int playerId = 0;
		if (ReInput.players.GetPlayer(playerId).GetButtonDown("MainLevelSelect7"))
		{
			MonoBehaviour.print("hey yoo");
		}
	}
}
