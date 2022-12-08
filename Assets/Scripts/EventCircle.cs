using UnityEngine;
using UnityEngine.UI;

public class EventCircle : MonoBehaviour
{
	public Image image;

	public Sprite dottedCircle;

	public Sprite dottedCirclePrecise;

	private void Update()
	{
		bool flag = UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift);
		image.sprite = (flag ? dottedCirclePrecise : dottedCircle);
	}
}
