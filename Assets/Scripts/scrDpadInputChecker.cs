using UnityEngine;

public class scrDpadInputChecker : MonoBehaviour
{
	public bool up;

	public bool down;

	public bool left;

	public bool right;

	public bool anyDir;

	private bool lastHeldDown;

	private const float t = 0.95f;

	private bool lastHeldX;

	private bool lastHeldY;

	public bool pressedX;

	public bool pressedY;

	public bool holdingX;

	public bool holdingY;

	public bool anyDirDown
	{
		get
		{
			if (!pressedX)
			{
				return pressedY;
			}
			return true;
		}
	}

	private void Update()
	{
		float axis = UnityEngine.Input.GetAxis("DPadX");
		float axis2 = UnityEngine.Input.GetAxis("DPadY");
		if (Mathf.Abs(axis) > 0.95f)
		{
			holdingX = true;
		}
		if (Mathf.Abs(axis2) > 0.95f)
		{
			holdingY = true;
		}
		pressedX = (!lastHeldX && holdingX);
		pressedY = (!lastHeldY && holdingY);
		lastHeldX = holdingX;
		lastHeldY = holdingY;
	}

	public int CountPressed()
	{
		int num = 0;
		if (pressedX)
		{
			num++;
		}
		if (pressedY)
		{
			num++;
		}
		return num;
	}
}
