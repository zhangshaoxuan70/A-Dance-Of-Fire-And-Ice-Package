using UnityEngine;

public class TransformGizmo : MonoBehaviour
{
	public enum GizmoPosition
	{
		TopLeft,
		Top,
		TopRight,
		Right,
		BottomRight,
		Bottom,
		BottomLeft,
		Left
	}

	public GizmoPosition gizmoPosition;
}
