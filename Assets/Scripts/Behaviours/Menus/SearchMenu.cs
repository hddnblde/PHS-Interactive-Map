using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
	public class SearchMenu : MonoBehaviour
	{
		public delegate void Search(string text);
		public delegate void Select(int index);
		private event Search OnSearch;
		private event Select OnSelect;

		[SerializeField]
		private InputField textField = null;

		[SerializeField]
		private MenuContentLayout contentPrefab = null;

		[SerializeField]
		private RectTransform contentContainer = null;

		[SerializeField]
		private NavigationMenu navigationMenu = null;

		[SerializeField]
		private int poolCount = 70;

		private List<MenuContentLayout> contentLayoutList = new List<MenuContentLayout>();

		private void Awake()
		{
			Initialize();
			PoolContent();
		}

		private void PoolContent()
		{
			if(contentPrefab == null || contentContainer == null)
				return;

			for(int i = 0; i < poolCount; i++)
			{
				MenuContentLayout contentLayout = Instantiate(contentPrefab, contentContainer);
				contentLayout.OnSelect += OnContentClicked;
				contentLayoutList.Add(contentLayout);
				contentLayout.gameObject.SetActive(false);
			}
		}

		private void Initialize()
		{
			if(textField != null)
				textField.onValueChanged.AddListener(OnTextEdit);
		}

		private void OnTextEdit(string text)
		{
			if(OnSearch != null)
				OnSearch(text);
		}

		private void OnContentClicked(int index)
		{
			if(OnSelect != null)
				OnSelect(index);
		}

		private void ClearContentLayout()
		{
			foreach(MenuContentLayout contentLayout in contentLayoutList)
				contentLayout.gameObject.SetActive(false);
		}

		private MenuContentLayout GetContentLayout()
		{
			MenuContentLayout currentContentLayout = null;

			foreach(MenuContentLayout contentLayout in contentLayoutList)
			{
				if(contentLayout.gameObject.activeInHierarchy)
					continue;

				contentLayout.gameObject.SetActive(true);
				currentContentLayout = contentLayout;
				break;
			}

			return currentContentLayout;
		}

		private void ShowBackground(bool show)
		{
//			if(navigationMenu != null)
//				navigationMenu.
		}

		public void Open(Search searchAction, Select selectAction)
		{
			OnSearch = searchAction;
			OnSelect = selectAction;
			ClearContentLayout();
			ShowBackground(true);
		}

		public void Close()
		{
			OnSearch = null;
			OnSelect = null;
			ClearContentLayout();
			ShowBackground(false);
		}

		public void SetContent(MenuContent[] contents)
		{
			ClearContentLayout();

			foreach(MenuContent content in contents)
			{
				MenuContentLayout layout = GetContentLayout();
				if(layout == null)
					break;

				layout.Set(content.thumbnail, content.text);
			}
		}
	}
}