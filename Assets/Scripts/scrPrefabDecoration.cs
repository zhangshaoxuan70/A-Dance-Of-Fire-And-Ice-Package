using UnityEngine;

public class scrPrefabDecoration : scrDecoration
{
	public PublicPrefabType prefabType;

	public override void HitFloor()
	{
	}

	public override void SetDepth(int depth)
	{
	}

	public override void ApplyColor()
	{
	}

	public override void SetVisible(bool visible)
	{
	}

	public void SetTile(Vector2 newTile)
	{
	}

	public override float GetAlpha()
	{
		return 1f;
	}

	public override bool GetVisible()
	{
		return true;
	}
}
