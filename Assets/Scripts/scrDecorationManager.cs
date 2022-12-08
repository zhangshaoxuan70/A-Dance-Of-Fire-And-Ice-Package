using ADOFAI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class scrDecorationManager : ADOBase
{
	public Dictionary<string, List<scrDecoration>> taggedDecorations;

	public List<scrDecoration> allDecorations;

	[NonSerialized]
	public scrExtImgHolder imageHolder;

	[Header("References")]
	public Sprite defaultSprite;

	public Sprite notFoundSprite;

	[Header("Prefabs")]
	public GameObject prefab_visualDecoration;

	public GameObject prefab_textDecoration;

	public GameObject prefab_prefabDecoration;

	private static scrDecorationManager _instance;

	public static scrDecorationManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType<scrDecorationManager>();
			}
			return _instance;
		}
	}

	public static scrDecoration GetDecoration(LevelEvent source)
	{
		return instance.allDecorations.Find((scrDecoration d) => d.sourceLevelEvent == source);
	}

	public static scrDecoration GetDecoration(int index)
	{
		List<scrDecoration> list = instance.allDecorations;
		if (index < 0 || index >= list.Count)
		{
			return null;
		}
		return list[index];
	}

	public static int GetDecorationIndex(LevelEvent dec)
	{
		int num = 0;
		foreach (scrDecoration allDecoration in instance.allDecorations)
		{
			if (dec == allDecoration.sourceLevelEvent)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	private void Awake()
	{
		taggedDecorations = new Dictionary<string, List<scrDecoration>>();
		if (ADOBase.isLevelEditor)
		{
			imageHolder = ADOBase.customLevel.imgHolder;
		}
		else
		{
			foreach (Transform item in base.transform)
			{
				string name = item.name;
				if (!(name == "NO TAG"))
				{
					string[] array = name.Split(',');
					foreach (string key in array)
					{
						if (!taggedDecorations.ContainsKey(key))
						{
							taggedDecorations[key] = new List<scrDecoration>();
						}
						taggedDecorations[key].Add(item.GetComponentInChildren<scrDecoration>());
					}
				}
			}
		}
	}

	public void ScaleSprite(SpriteRenderer target, float x, float y)
	{
		if (!(target.sprite == null))
		{
			target.transform.localScale = new Vector2(x, y);
		}
	}

	public void ShowEmptyDecorations(bool show)
	{
		foreach (scrDecoration allDecoration in instance.allDecorations)
		{
			if (allDecoration.sourceLevelEvent.eventType == LevelEventType.AddDecoration && allDecoration.decType == DecorationType.Image && ((scrVisualDecoration)allDecoration).spriteRenderer.sprite == defaultSprite)
			{
				((scrVisualDecoration)allDecoration).spriteRenderer.sprite = (show ? defaultSprite : null);
			}
		}
	}

	public void CreateDecoration(LevelEvent dec, out bool spritesLoaded)
	{
		spritesLoaded = false;
		scrDecoration scrDecoration = null;
		GameObject gameObject = null;
		string text = null;
		DecorationType decorationType = DecorationType.Image;
		if (dec.eventType == LevelEventType.AddDecoration)
		{
			text = (string)dec["decorationImage"];
			if (text != null && text.StartsWith("prefab:", StringComparison.CurrentCultureIgnoreCase))
			{
				string text2 = text.Substring(7);
				decorationType = DecorationType.Prefab;
				text = text2;
				gameObject = prefab_prefabDecoration;
			}
			else
			{
				decorationType = DecorationType.Image;
				gameObject = prefab_visualDecoration;
			}
		}
		else if (dec.eventType == LevelEventType.AddText)
		{
			decorationType = DecorationType.Text;
			gameObject = prefab_textDecoration;
			text = (string)dec["decText"];
		}
		if (!(gameObject == null))
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, base.transform);
			gameObject2.name = text;
			switch (decorationType)
			{
			case DecorationType.Text:
				scrDecoration = gameObject2.GetComponent<scrTextDecoration>();
				break;
			case DecorationType.Image:
				scrDecoration = gameObject2.GetComponent<scrVisualDecoration>();
				break;
			case DecorationType.Prefab:
			{
				string text3 = text;
				GameObject gameObject3 = Resources.Load<GameObject>("PublicPrefabs/" + text3);
				GameObject gameObject4 = (gameObject3 != null) ? UnityEngine.Object.Instantiate(gameObject3, gameObject2.transform.position, Quaternion.identity, gameObject2.transform.transform) : new GameObject(text3);
				scrDecoration = gameObject4.GetOrAddComponent<scrPrefabDecoration>();
				(scrDecoration as scrPrefabDecoration).prefabType = text3.ToEnum(PublicPrefabType.None, showWarning: false);
				scrParallax orAddComponent = gameObject4.GetOrAddComponent<scrParallax>();
				scrDecoration.pivotTrans = gameObject2.transform;
				scrDecoration.decTrans = gameObject4.transform;
				scrDecoration.parallax = orAddComponent;
				break;
			}
			}
			if (scrDecoration != null)
			{
				scrDecoration.manager = this;
				scrDecoration.decType = decorationType;
				scrDecoration.Setup(dec, out spritesLoaded);
				scrDecoration.UpdateHitbox();
				allDecorations.Add(scrDecoration);
			}
		}
	}

	public void ClearDecorations()
	{
		taggedDecorations.Clear();
		allDecorations.Clear();
		Transform[] componentsInChildren = base.gameObject.GetComponentsInChildren<Transform>();
		foreach (Transform transform in componentsInChildren)
		{
			if (transform != null && transform.gameObject != base.gameObject)
			{
				UnityEngine.Object.DestroyImmediate(transform.gameObject);
			}
		}
	}

	public void ShowSelectionBorders(LevelEvent source, bool show = true)
	{
		scrDecoration decoration = GetDecoration(source);
		decoration.ShowSelectionBorders(show);
		decoration.ShowHitboxBorders(show);
	}

	public void ClearDecorationBorders()
	{
		foreach (scrDecoration allDecoration in instance.allDecorations)
		{
			allDecoration.ShowSelectionBorders(show: false);
			allDecoration.ShowHitboxBorders(show: false);
		}
	}
}
