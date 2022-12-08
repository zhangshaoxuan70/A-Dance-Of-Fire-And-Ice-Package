using UnityEngine;

public class ToggleVisible : MonoBehaviour
{
	public MonoBehaviour scriptToToggle;

	private void OnBecameVisible()
	{
		scriptToToggle.enabled = true;
	}

	private void OnBecameInvisible()
	{
		scriptToToggle.enabled = false;
	}
}
