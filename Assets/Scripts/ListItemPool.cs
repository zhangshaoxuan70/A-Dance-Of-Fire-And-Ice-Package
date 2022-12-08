using System.Collections.Generic;
using UnityEngine;

public class ListItemPool : ADOBase
{
	public static ListItemPool instance;

	[HideInInspector]
	public Transform itemHolder;

	private List<GameObject> pooledItems = new List<GameObject>();

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		GameObject gameObject = new GameObject();
		gameObject.transform.SetParent(base.transform);
		gameObject.name = "ListItemPool";
		itemHolder = gameObject.transform;
	}

	public GameObject GetPooledItem(RectTransform parent, Vector3 position)
	{
		GameObject obj = (itemHolder.childCount > 0) ? itemHolder.GetChild(0).gameObject : CreateItem();
		obj.transform.SetParent(parent);
		RectTransform component = obj.GetComponent<RectTransform>();
		component.anchoredPosition = component.anchoredPosition.WithY(position.y);
		component.offsetMin = component.offsetMin.WithX(0f);
		component.offsetMax = component.offsetMax.WithX(0f);
		obj.SetActive(value: true);
		return obj;
	}

	public void SendItemBackToPool(GameObject item)
	{
		item.transform.SetParent(itemHolder);
		item.SetActive(value: false);
	}

	private GameObject CreateItem()
	{
		GameObject gameObject = Object.Instantiate(ADOBase.gc.prefab_controlListItem, itemHolder);
		gameObject.SetActive(value: false);
		pooledItems.Add(gameObject);
		return gameObject;
	}
}
