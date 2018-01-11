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
		public delegate void Quit();

		private event Search OnSearch;
		private event Select OnSelect;
		private event Quit OnQuit;

		[SerializeField]
		private InputField textField = null;

		[SerializeField]
		private Text placeholderText = null;

		[SerializeField]
		private MenuContentLayout contentPrefab = null;

		[SerializeField]
		private RectTransform contentContainer = null;

		[SerializeField]
		private Button quitButton = null;

		[SerializeField]
		private Button clearButton = null;

		[SerializeField]
		private int poolCount = 70;

		private List<MenuContentLayout> contentLayoutList = new List<MenuContentLayout>();
		private bool isOpen = false;

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

			if(quitButton != null)
				quitButton.onClick.AddListener(OnQuitButtonClicked);

			if(clearButton != null)
				clearButton.onClick.AddListener(OnClearButtonClicked);
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

		private void OnQuitButtonClicked()
		{
			if(OnQuit != null)
				OnQuit();
		}

		private void OnClearButtonClicked()
		{
			if(textField != null)
				textField.text = null;
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

		public void Open(Search searchAction, Select selectAction, Quit quitAction, string placeholder = "Search")
		{
			if(isOpen)
			{
				Debug.Log("Search menu is already opened by another activity! Make sure to close it before opening on this activity.");
				return;
			}

			InitializeTextField(placeholder);
			ClearContentLayout();
			OnSearch = searchAction;
			OnSelect = selectAction;
			OnQuit = quitAction;

			isOpen = true;
		}

		public void Close()
		{
			if(!isOpen)
			{
				Debug.Log("Search menu is already closed.");
				return;
			}

			OnSearch = null;
			OnSelect = null;
			OnQuit = null;
			ClearContentLayout();

			isOpen = false;
		}

		private void InitializeTextField(string placeholder)
		{
			if(textField != null)
				textField.text = "";

			if(placeholderText != null)
				placeholderText.text = placeholder;
		}

		public void SetContent(MenuContent[] contents)
		{
			ClearContentLayout();

			if(contents == null || contents.Length == 0)
				return;
			
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