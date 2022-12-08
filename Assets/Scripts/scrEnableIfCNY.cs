public class scrEnableIfCNY : ADOBase
{
	private void OnEnable()
	{
		if (!ADOBase.IsCNY())
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
