using UnityEngine;
using UnityEngine.UI;

public class EditorSpeedIndicator : ADOBase
{
	public Text percent;

	public void LessSpeed()
	{
		ShiftSpeed(-1);
	}

	public void MoreSpeed()
	{
		ShiftSpeed(1);
	}

	private void UpdatePercentText(int speedPercent)
	{
		percent.text = speedPercent.ToString() + "%";
	}

	private void Awake()
	{
		UpdatePercentText(Persistence.GetShortcutPlaySpeed());
	}

	private void ShiftSpeed(int direction)
	{
		int shortcutPlaySpeed = Persistence.GetShortcutPlaySpeed();
		int num = (UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift)) ? 1 : 10;
		num *= direction;
		shortcutPlaySpeed = Mathf.Clamp(shortcutPlaySpeed + num, 1, 1000);
		shortcutPlaySpeed = Persistence.SetShortcutPlaySpeed(shortcutPlaySpeed);
		UpdatePercentText(shortcutPlaySpeed);
	}
}
