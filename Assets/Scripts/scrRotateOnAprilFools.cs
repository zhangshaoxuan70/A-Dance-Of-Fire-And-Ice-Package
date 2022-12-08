public class scrRotateOnAprilFools : ADOBase
{
	private void Awake()
	{
		if (ADOBase.IsAprilFools())
		{
			base.transform.eulerAngles = base.transform.eulerAngles.WithZ(180f);
		}
	}
}
