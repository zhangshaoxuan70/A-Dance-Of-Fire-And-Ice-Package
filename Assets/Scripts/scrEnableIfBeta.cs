using System.Linq;
using UnityEngine.UI;

public class scrEnableIfBeta : ADOBase
{
	public bool setBuildText;

	private void Awake()
	{
		base.gameObject.SetActive(value: false);
		if (!RDC.debug && SteamIntegration.Instance.initialized && GCNS.betaBranches.Contains(GCS.steamBranchName))
		{
			base.gameObject.SetActive(value: true);
			if (setBuildText)
			{
				GetComponent<Text>().text = char.ToUpper(GCS.steamBranchName[0]).ToString() + GCS.steamBranchName.Substring(1) + " Build";
			}
		}
	}
}
