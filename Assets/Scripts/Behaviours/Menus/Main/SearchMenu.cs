﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Menus
{
	public class SearchMenu : MonoBehaviour
	{
		#region Serialized Fields
		[SerializeField]
		private InputField textField = null;

		[SerializeField]
		private Text placeholderText = null;

		[SerializeField]
		private MenuContentLayout contentPrefab = null;

		[SerializeField]
		private RectTransform contentContainer = null;

		[SerializeField]
		private Button cancelButton = null;

		[SerializeField]
		private Button clearButton = null;

		[SerializeField]
		private int poolCount = 70;
		#endregion


		#region Hidden Fields
		private delegate void OpenAction(Search searchAction, Select selectAction, Quit quitAction, string placeholder, bool focused);
		private delegate void CloseAction();
		private delegate void SetContentAction(MenuContent[] contents);

		public delegate void Search(string text);
		public delegate void Select(int index);
		public delegate void Quit();

		private static event OpenAction OnOpenMenu;
		private static event CloseAction OnCloseMenu;
		private static event SetContentAction OnSetContent;

		private event Search OnSearch;
		private event Select OnSelect;
		private event Quit OnQuit;

		private CanvasGroup canvasGroup = null;
		private List<MenuContentLayout> contentLayoutList = new List<MenuContentLayout>();
		private static bool m_isOpen = false;
		#endregion


		#region Property
		public static bool isOpen
		{
			get { return m_isOpen; }
		}
		#endregion


		#region MonoBehaviour Implementation
		private void Awake()
		{
			Initialize();
			PoolContents();
		}

		private void OnEnable()
		{
			RegisterInternalEvents();
		}

		private void OnDisable()
		{
			DeregisterInternalEvents();
		}
		#endregion


		#region Initializers
		private void Initialize()
		{
			if(textField != null)
				textField.onValueChanged.AddListener(OnTextEdit);

			if(cancelButton != null)
				cancelButton.onClick.AddListener(OnQuitButtonClicked);

			if(clearButton != null)
				clearButton.onClick.AddListener(OnClearButtonClicked);

			canvasGroup = GetComponent<CanvasGroup>();
		}

		private void PoolContents()
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

		private void RegisterInternalEvents()
		{
			OnOpenMenu += Internal_Open;
			OnCloseMenu += Internal_Close;
			OnSetContent += Internal_SetContents;
		}

		private void DeregisterInternalEvents()
		{
			OnOpenMenu -= Internal_Open;
			OnCloseMenu -= Internal_Close;
			OnSetContent -= Internal_SetContents;
		}
		#endregion


		#region Events
		private void OnTextEdit(string text)
		{
			if(OnSearch != null)
				OnSearch(text);

			if(clearButton != null)
				clearButton.gameObject.SetActive(!string.IsNullOrEmpty(text));
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
			
			Close();
		}

		private void OnClearButtonClicked()
		{
			if(textField != null)
				textField.text = null;
		}
		#endregion
		

		#region Actions
		public static void Open(Search searchAction, Select selectAction, Quit quitAction, string placeholder = "Search", bool focused = false)
		{
			if(m_isOpen)
			{
				Debug.Log("Search menu is already opened.");
				return;
			}

			if(OnOpenMenu != null)
				OnOpenMenu(searchAction, selectAction, quitAction, placeholder, focused);
		
			m_isOpen = true;
		}

		public static void Close()
		{
			if(!m_isOpen)
			{
				Debug.Log("Search menu is already closed.");
				return;
			}

			if(OnCloseMenu != null)
				OnCloseMenu();

			m_isOpen = false;
		}

		public static void SetContents(MenuContent[] contents)
		{
			if(!isOpen)
			{
				Debug.Log("Cannot set contents. Search menu isn't open.");
				return;
			}

			if(OnSetContent != null)
				OnSetContent(contents);
		}

		private void Internal_Open(Search searchAction, Select selectAction, Quit quitAction, string placeholder, bool focused)
		{
			if(m_isOpen)
			{
				Debug.Log("Search menu is already opened by another activity! Make sure to close it before opening on this activity.");
				return;
			}

			InitializeTextField(placeholder);
			ClearContentLayout();
			OnSearch = searchAction;
			OnSelect = selectAction;
			OnQuit = quitAction;

			m_isOpen = true;
			Show(true);

			if(focused)
				FocusOnText();
		}

		private void Internal_Close()
		{
			if(!m_isOpen)
			{
				Debug.Log("Search menu is already closed.");
				return;
			}

			OnSearch = null;
			OnSelect = null;
			OnQuit = null;
			ClearContentLayout();

			m_isOpen = false;
			Show(false);
		}		

		private void Internal_SetContents(MenuContent[] contents)
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
		#endregion


		#region Helpers
		private void ClearContentLayout()
		{
			foreach(MenuContentLayout contentLayout in contentLayoutList)
				contentLayout.gameObject.SetActive(false);
		}

		private void Show(bool shown)
		{
			if(canvasGroup == null)
				return;

			canvasGroup.alpha = (shown ? 1f : 0f);
			canvasGroup.blocksRaycasts = shown;
			canvasGroup.interactable = shown;
		}

		private void FocusOnText()
		{
			if(textField != null)
				textField.Select();
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

		private void InitializeTextField(string placeholder)
		{
			if(textField != null)
				textField.text = "";

			if(placeholderText != null)
				placeholderText.text = placeholder;

			if(clearButton != null)
				clearButton.gameObject.SetActive(false);
		}
		#endregion
	}
}