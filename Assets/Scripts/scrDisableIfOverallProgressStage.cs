using UnityEngine;

public class scrDisableIfOverallProgressStage : MonoBehaviour
{
	public int requiredStage;

	public bool inverted;

	private void Start()
	{
		bool flag = Persistence.GetOverallProgressStage() < requiredStage;
		if (inverted)
		{
			flag = !flag;
		}
		if (flag)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
