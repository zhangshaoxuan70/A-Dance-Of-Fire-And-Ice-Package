using System.Collections.Generic;
using System.Linq;

public class PortalCreditData
{
	public enum LinkType
	{
		None,
		Soundcloud,
		Youtube,
		Twitter
	}

	public string credit;

	public string people;

	public string link;

	public LinkType linkType;

	private const string LocKeyPrefix = "levelSelect.";

	public PortalCreditData(Dictionary<string, object> dict)
	{
		Decode(dict);
	}

	public void Decode(Dictionary<string, object> dict)
	{
		string key = "levelSelect." + (string)dict["credit"];
		credit = RDString.Get(key);
		IEnumerable<string> values = from p in ((List<object>)dict["people"]).OfType<string>()
			select RDString.Get("levelSelect." + p);
		people = string.Join('\n', values);
		string valueAs = null;
		if (dict.TryGetValueAs("soundcloud", out valueAs))
		{
			linkType = LinkType.Soundcloud;
		}
		else if (dict.TryGetValueAs("youtube", out valueAs))
		{
			linkType = LinkType.Youtube;
		}
		else if (dict.TryGetValueAs("twitter", out valueAs))
		{
			linkType = LinkType.Twitter;
		}
		if (valueAs != null)
		{
			link = RDString.Get("levelSelect." + valueAs);
		}
	}
}
