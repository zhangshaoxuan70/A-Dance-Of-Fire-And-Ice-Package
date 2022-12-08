public class ffxSpeed : ffxBase
{
	public float speed = 1f;

	public override void doEffect()
	{
		ADOBase.d_speed = speed;
	}
}
