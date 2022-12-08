using UnityEngine;

public class EntranceTile : ADOBase
{
	private void LateUpdate()
	{
		scnCLS instance = scnCLS.instance;
		float num = ADOBase.controller.camy.pos.y;
		if ((float)instance.levelCount >= instance.levelCountForLoop)
		{
			float num2 = (float)instance.levelCount / 2f;
			num = Mathf.Clamp(num, instance.gemBottomY + 1, instance.gemTopY - 1);
		}
		base.transform.MoveY(num);
	}
}
