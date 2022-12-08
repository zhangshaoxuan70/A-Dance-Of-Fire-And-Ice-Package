public class scrMenuContinueDisable : ADOBase
{
	private void Awake()
	{
		string text = Persistence.GetSavedCurrentLevel();
		if (text.IsTaro() && !ADOBase.ownsTaroDLC)
		{
			text = "0-0";
		}
		if (text == "0-0")
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
