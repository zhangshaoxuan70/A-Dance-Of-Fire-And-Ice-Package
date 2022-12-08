using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI
{
	public class KeyIndicator : ADOBase
	{
		public KeyCode keyCodeA;

		public KeyCode keyCodeB;

		public Graphic[] graphics;

		public Color idleColor;

		public Color activeColor;

		public Text text;

		private void LateUpdate()
		{
			KeyCode[] obj = new KeyCode[2]
			{
				keyCodeA,
				keyCodeB
			};
			bool flag = false;
			KeyCode[] array = obj;
			for (int i = 0; i < array.Length; i++)
			{
				if (UnityEngine.Input.GetKey(array[i]))
				{
					flag = true;
				}
			}
			Graphic[] array2 = graphics;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].color = (flag ? activeColor : idleColor);
			}
		}

		public void SetKeyCode(KeyCode newKeyA, KeyCode newKeyB = KeyCode.None)
		{
			keyCodeA = newKeyA;
			keyCodeB = newKeyB;
			if (keyCodeA == KeyCode.Space)
			{
				text.text = "Space ↺";
			}
			else if (keyCodeA == KeyCode.Tab)
			{
				text.text = "Tab ↔";
			}
			else if (keyCodeA == KeyCode.LeftShift || keyCodeA == KeyCode.RightShift)
			{
				text.text = "Shift";
			}
			else
			{
				text.text = keyCodeA.ToString();
			}
		}
	}
}
