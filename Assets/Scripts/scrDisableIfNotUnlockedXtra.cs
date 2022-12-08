using UnityEngine;

public class scrDisableIfNotUnlockedXtra : MonoBehaviour
{
	private void Start()
	{
		if (!Persistence.GetUnlockedXF())
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
