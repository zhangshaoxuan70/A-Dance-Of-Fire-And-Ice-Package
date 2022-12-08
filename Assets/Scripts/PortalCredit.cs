using UnityEngine;
using UnityEngine.UI;

public class PortalCredit : MonoBehaviour
{
	public Text titleText;

	public Text peopleText;

	public scrButtonURL soundcloudButton;

	public scrButtonURL youtubeButton;

	public scrButtonURL twitterButton;

	public Color soundcloudColor;

	public Color youtubeColor;

	public Color twitterColor;

	public void Load(PortalCreditData data)
	{
		if (data != null)
		{
			base.gameObject.SetActive(value: true);
			titleText.text = data.credit;
			peopleText.text = data.people;
			scrButtonURL scrButtonURL;
			switch (data.linkType)
			{
			case PortalCreditData.LinkType.Soundcloud:
				scrButtonURL = soundcloudButton;
				peopleText.color = soundcloudColor;
				break;
			case PortalCreditData.LinkType.Youtube:
				scrButtonURL = youtubeButton;
				peopleText.color = youtubeColor;
				break;
			case PortalCreditData.LinkType.Twitter:
				scrButtonURL = twitterButton;
				peopleText.color = twitterColor;
				break;
			default:
				scrButtonURL = null;
				break;
			}
			GetComponent<VerticalLayoutGroup>().enabled = true;
			if (scrButtonURL != null)
			{
				scrButtonURL.gameObject.SetActive(value: true);
				scrButtonURL.link = data.link;
			}
		}
	}
}
