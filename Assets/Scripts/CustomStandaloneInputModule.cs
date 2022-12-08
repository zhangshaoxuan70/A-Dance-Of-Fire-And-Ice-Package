using UnityEngine.EventSystems;

public class CustomStandaloneInputModule : StandaloneInputModule
{
	public PointerEventData GetPointerData()
	{
		if (!m_PointerData.TryGetValue(-1, out PointerEventData value))
		{
			return null;
		}
		return value;
	}
}
