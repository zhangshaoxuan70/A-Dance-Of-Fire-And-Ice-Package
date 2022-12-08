using UnityEngine;
using UnityEngine.UI;

public class scrShortcutText : MonoBehaviour
{
	public bool isHeader;

	public KeyCode keyCode;

	public KeyCode otherKeyCode;

	public bool usingShift;

	public bool usingCtrl;

	public bool usingAlt;

	public string key;

	public bool otherKeyDoesntUseModifierKeys;

	private Text text;

	private void Awake()
	{
		text = GetComponent<Text>();
	}

	public void SetText()
	{
		if (isHeader)
		{
			text.text = RDString.Get("editor.shortcuts." + key);
			return;
		}
		string str = RDEditorUtils.KeyComboToString(usingCtrl, usingShift, usingAlt, keyCode);
		string str2 = string.Empty;
		if (otherKeyCode != 0)
		{
			if (otherKeyDoesntUseModifierKeys)
			{
				usingCtrl = (usingAlt = (usingShift = false));
			}
			str2 = " " + RDString.Get("editor.shortcuts.or") + " " + RDEditorUtils.KeyComboToString(usingCtrl, usingShift, usingAlt, otherKeyCode);
		}
		text.text = str + str2 + ": " + RDString.Get("editor.shortcuts." + key);
	}
}
