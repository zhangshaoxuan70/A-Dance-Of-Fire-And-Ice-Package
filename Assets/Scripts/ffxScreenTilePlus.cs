public class ffxScreenTilePlus : ffxPlusBase
{
	public float tileX;

	public float tileY;

	private static ScreenTile screenTile;

	public override void Awake()
	{
		base.Awake();
		if (screenTile == null)
		{
			screenTile = cam.GetComponent<ScreenTile>();
		}
	}

	public override void StartEffect()
	{
		SetScreenTile(tileX, tileY);
	}

	private void SetScreenTile(float tileX, float tileY)
	{
		screenTile.enabled = (tileX != 1f || tileY != 1f);
		screenTile.tileX = tileX;
		screenTile.tileY = tileY;
	}
}
