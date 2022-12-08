using UnityEngine;

public class scrDisableIfUnlockedXtra : MonoBehaviour
{
	private void Start()
	{
		if (Persistence.GetUnlockedXF())
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
