using UnityEngine;
using UnityEngine.UI;

public class scrOffsetText : MonoBehaviour
{
	private void Start()
	{
		GetComponent<Text>().color = Color.clear;
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.LeftBracket) || UnityEngine.Input.GetKeyDown(KeyCode.RightBracket))
		{
			GetComponent<Text>().color = Color.blue;
		}
	}
}
