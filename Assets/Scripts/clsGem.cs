using DG.Tweening;

public class clsGem : ffxBase
{
	public bool down;

	private bool moving;

	public override void doEffect()
	{
		int num = down ? scnCLS.instance.gemBottomY : scnCLS.instance.gemTopY;
		Move(move: true);
		base.transform.DOMoveY(num, 1f).SetEase(Ease.InOutSine).OnComplete(delegate
		{
			Move(move: false);
		});
	}

	private void Move(bool move)
	{
		GetComponent<scrMenuMovingFloor>().moving = move;
		moving = move;
		(down ? scnCLS.instance.gemBottom.gameObject : scnCLS.instance.gemTop.gameObject).SetActive(!move);
		if (!move)
		{
			base.transform.MoveY(down ? scnCLS.instance.gemTopY : scnCLS.instance.gemBottomY);
		}
		if (down)
		{
			scnCLS.instance.LoadTileIconsNearby(scnCLS.instance.levelCount - 1);
		}
		else
		{
			scnCLS.instance.LoadTileIconsNearby(0);
		}
	}

	private void Update()
	{
		if (moving)
		{
			cam.Refocus(base.transform);
		}
	}
}
