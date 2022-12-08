using UnityEngine;

public struct DebugPoint
{
	public Vector2 position;

	public Color color;

	public float radius;

	public DebugPoint(Vector2 position, Color color, float radius = 0.05f)
	{
		this.position = position;
		this.color = color;
		this.radius = radius;
	}
}
