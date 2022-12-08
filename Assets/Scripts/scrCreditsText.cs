using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class scrCreditsText : ADOBase
{
	public enum CreditsType
	{
		GameCredits,
		NeoCosmosCredits
	}

	private bool forceScrolling;

	private readonly string[][] gameCredits = new string[13][]
	{
		new string[2]
		{
			"leadGameDesign",
			"hafiz"
		},
		new string[2]
		{
			"artAndVisualDesign",
			"kyle"
		},
		new string[2]
		{
			"leadProgramming",
			"giacomo"
		},
		new string[10]
		{
			"programming",
			"hafiz",
			"morphious",
			"jenny",
			"taro",
			"pizza",
			"crackThrough",
			"jose",
			"sebas",
			"antonio"
		},
		new string[3]
		{
			"levelDesign",
			"hafiz",
			"jenny"
		},
		new string[3]
		{
			"music",
			"hafiz",
			"jade"
		},
		new string[2]
		{
			"soundDesign",
			"satellite"
		},
		new string[3]
		{
			"sysadminAndWeb",
			"sora",
			"diego"
		},
		new string[2]
		{
			"curation",
			"nichipe"
		},
		new string[2]
		{
			"fontDesign",
			"wopa"
		},
		new string[2]
		{
			"specialThanks",
			"luxus"
		},
		new string[25]
		{
			"localization",
			"credits.language.korean",
			"auth",
			"credits.language.chinese",
			"craft",
			"credits.language.portugueseBR",
			"pedro",
			"credits.language.spanish",
			"coffee",
			"credits.language.japanese",
			"gefrorenes",
			"credits.language.polish",
			"tomasz",
			"credits.language.russian",
			"calvares",
			"credits.language.romanian",
			"morphious",
			"credits.language.vietnamese",
			"kudo",
			"credits.language.french",
			"dps&carlcool",
			"credits.language.czech",
			"wolfplay",
			"credits.language.german",
			"mariokirby"
		},
		new string[1]
		{
			"levelSelect.subtitle"
		}
	};

	private readonly string[][] neoCosmosCredits = new string[10][]
	{
		new string[2]
		{
			"director",
			"taro"
		},
		new string[5]
		{
			"programming",
			"taro",
			"giacomo",
			"morphious",
			"jenny"
		},
		new string[3]
		{
			"additionalProgramming",
			"pizza",
			"crackThrough"
		},
		new string[5]
		{
			"music",
			"levelSelect.canblaster",
			"levelSelect.frums",
			"levelSelect.dmDokuro",
			"levelSelect.ashAstral"
		},
		new string[2]
		{
			"soundDesign",
			"satellite"
		},
		new string[4]
		{
			"characterDesign",
			"levelSelect.7computation",
			"taro",
			"levelSelect.teri"
		},
		new string[2]
		{
			"backgroundDesign",
			"kyle"
		},
		new string[10]
		{
			"playtester",
			"hafiz",
			"levelSelect.rikri",
			"sora",
			"rentaP",
			"deadlySprinklez",
			"sgnDavid",
			"octaHeart",
			"jeni",
			"firestix"
		},
		new string[25]
		{
			"localization",
			"credits.language.korean",
			"auth",
			"credits.language.chinese",
			"craft",
			"credits.language.portugueseBR",
			"pedro",
			"credits.language.spanish",
			"coffee",
			"credits.language.japanese",
			"gefrorenes",
			"credits.language.polish",
			"tomasz",
			"credits.language.russian",
			"calvares",
			"credits.language.romanian",
			"morphious",
			"credits.language.vietnamese",
			"kudo",
			"credits.language.french",
			"carlcool&sora",
			"credits.language.czech",
			"wolfplay",
			"credits.language.german",
			"mariokirby"
		},
		new string[1]
		{
			"levelSelect.subtitle"
		}
	};

	public const float translateSpeed = 75f;

	public const float creditsLoopSpacing = 450f;

	public CreditsType creditsType;

	public Transform scrollSection;

	public RectTransform content;

	public RectTransform title;

	public Text text;

	public Vector2Int planetsPosition;

	[NonSerialized]
	public RectTransform contentCopy;

	private Vector3 startPos;

	private Vector3 copyStartPos;

	private bool activateSwitch;

	public OverseerIdle os;

	private void Awake()
	{
		string str = "";
		int num = 0;
		string[][] obj = (creditsType == CreditsType.GameCredits) ? gameCredits : neoCosmosCredits;
		float num2 = 34f * RDString.fontData.fontScale;
		string[][] array = obj;
		foreach (string[] array2 in array)
		{
			int num3 = 0;
			string a = array2[0];
			string[] array3 = array2;
			foreach (string text in array3)
			{
				bool flag = num3 == 0 && array2.Length > 1;
				string text2 = text.Contains(".") ? RDString.Get(text) : ((text != "") ? RDString.Get("credits." + text) : "");
				if (a == "localization")
				{
					text2 = ((num3 != 0) ? ((!text.Contains("credits.language")) ? (text2 + "\n") : $"<color=#ffffffaa><size={num2}>{text2}{RDString.GetColon()}</size></color>") : $"<color=#ffffffaa><size={Mathf.RoundToInt(num2 * 1.25f)}>{text2}</size></color>\n");
				}
				else
				{
					if (flag)
					{
						text2 = $"<color=#ffffffaa><size={num2}>{text2}</size></color>";
					}
					text2 += "\n";
				}
				num3++;
				str += text2;
			}
			str += "\n";
			num++;
		}
		this.text.text = str;
		this.text.SetLocalizedFont();
		this.text.rectTransform.SizeDeltaY(this.text.preferredHeight);
		content.SizeDeltaY(this.text.preferredHeight + title.rect.height);
		contentCopy = UnityEngine.Object.Instantiate(content, scrollSection.transform);
		content.localPosition += Vector3.down * 500f;
		startPos = content.localPosition + Vector3.up * 500f;
		copyStartPos = startPos + Vector3.down * (content.rect.height + 450f);
		contentCopy.transform.localPosition = copyStartPos + Vector3.down * 500f;
	}

	private void Update()
	{
		Vector3 position = ADOBase.controller.chosenplanet.transform.position;
		int num = Mathf.RoundToInt(position.x);
		int num2 = Mathf.RoundToInt(position.y);
		bool flag = num == planetsPosition.x && num2 == planetsPosition.y;
		if (forceScrolling | flag)
		{
			if (!activateSwitch && os != null)
			{
				os.FadeIn(0.5f);
			}
			activateSwitch = true;
			Vector3 vector = Vector3.up * 75f * Time.deltaTime;
			if (content.localPosition.y <= content.rect.height + 450f)
			{
				content.localPosition += vector;
			}
			else
			{
				content.localPosition = copyStartPos;
			}
			if (contentCopy.localPosition.y <= content.rect.height + 450f)
			{
				contentCopy.localPosition += vector;
			}
			else
			{
				contentCopy.localPosition = copyStartPos;
			}
		}
		else if (activateSwitch)
		{
			Reset();
		}
	}

	public void Reset(bool instant = false)
	{
		if (os != null)
		{
			os.FadeOut(instant ? 0f : 0.5f);
		}
		float y = content.localPosition.y;
		float y2 = contentCopy.localPosition.y;
		bool flag = y > y2;
		content.transform.DOLocalMoveY(flag ? startPos.y : copyStartPos.y, instant ? 0f : 1.5f);
		contentCopy.transform.DOLocalMoveY(flag ? copyStartPos.y : startPos.y, instant ? 0f : 1.5f);
		activateSwitch = false;
	}

	public void SetScroll(bool scroll)
	{
		forceScrolling = scroll;
	}
}
