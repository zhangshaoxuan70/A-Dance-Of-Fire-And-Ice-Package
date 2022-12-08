using UnityEngine;

public struct PolygonInfo
{
	public RangeInt ccwCurve;

	public RangeInt cwCurve;

	public PolygonInfo(RangeInt ccwCurve, RangeInt cwCurve)
	{
		this.ccwCurve = ccwCurve;
		this.cwCurve = cwCurve;
	}
}
