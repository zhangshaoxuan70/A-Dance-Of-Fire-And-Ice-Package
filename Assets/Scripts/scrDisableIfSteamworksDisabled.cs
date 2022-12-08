public class scrDisableIfSteamworksDisabled : ADOBase
{
	private void Start()
	{
		if (!SteamIntegration.Instance.initialized)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
