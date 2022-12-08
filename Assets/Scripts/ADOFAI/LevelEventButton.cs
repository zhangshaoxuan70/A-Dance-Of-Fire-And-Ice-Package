using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ADOFAI
{
	public class LevelEventButton : ADOBase, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
	{
		public Button button;

		public Image icon;

		public int page;

		public int keyCode;

		public LevelEventType type;

		private bool showingAsFiltered;

		private Color baseColor = Color.white;

		public void Init(LevelEventType type, int pageNum = 0, int keyCode = 0)
		{
			page = pageNum;
			this.type = type;
			this.keyCode = keyCode;
			icon.sprite = GCS.levelEventIcons[type];
			button.onClick.AddListener(OnClicky);
			ShowAsSelected(selected: false);
		}

		private void OnClicky()
		{
			if (UnityEngine.Input.GetKey(KeyCode.LeftControl) || UnityEngine.Input.GetKey(KeyCode.RightControl) || UnityEngine.Input.GetKey(KeyCode.LeftMeta) || UnityEngine.Input.GetKey(KeyCode.RightMeta))
			{
				ADOBase.editor.FilterEventType(type);
			}
			else
			{
				ADOBase.editor.AddEventAtSelected(type);
			}
			ShowAsFiltered(ADOBase.editor.filteredEventType == type);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			ShowAsSelected(selected: true);
			ADOBase.editor.eventPickerText.text = RDString.Get($"editor.{type}");
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			ShowAsSelected(selected: false);
			ADOBase.editor.eventPickerText.text = "";
		}

		private void ShowAsSelected(bool selected)
		{
			icon.color = (selected ? baseColor : baseColor.WithAlpha(0.6f));
		}

		public void ShowAsFiltered(bool filtered)
		{
			icon.color = (baseColor = (filtered ? Color.cyan : Color.white));
			showingAsFiltered = filtered;
			if (!filtered)
			{
				ShowAsSelected(selected: false);
			}
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Right)
			{
				if (ADOBase.editor.currentCategory == LevelEventCategory.Favorites)
				{
					ADOBase.editor.RemoveFavoriteEvent(type);
					ADOBase.editor.eventPickerText.text = "";
				}
				else
				{
					ADOBase.editor.AddFavoriteEvent(type);
				}
			}
		}
	}
}
