using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.New
{
	public class SearchMenuNew : MenuBehaviour
	{
		#region Serialized Fields
		[Header("Contents")]
		[SerializeField]
		private MenuContentLayout contentPrefab = null;

		[SerializeField]
		private RectTransform container = null;

		[Header("Text")]
		[SerializeField]
		private InputField textField = null;

		[SerializeField]
		private Text placeholderText = null;

		[Header("Buttons")]
		[SerializeField]
		private Button cancelButton = null;

		[SerializeField]
		private Button clearButton = null;

		[Space()]
		[SerializeField]
		private int poolCount = 70;
		#endregion


		#region Hidden Fields
		public delegate void SearchAction(string text);
		private event SearchAction OnSearch;

		public delegate void SelectAction(int index);
		private event SelectAction OnSelect;

		private event Action OnCancel;
		private List<MenuContentLayout> contentLayoutList = new List<MenuContentLayout>();
		#endregion


		#region MonoBehaviour Implementation
		private void Awake()
		{
			Initialize();
			PoolContents();
		}
		#endregion


		#region Initializers
		private void Initialize()
		{
			RegisterTextFieldAction(OnTextEdit);
			RegisterButtonAction(clearButton, OnClearButtonClicked);
			RegisterButtonAction(cancelButton, OnCancelButtonClicked);
			RegisterButtonAction(cancelButton, base.Hide);
		}

		private void PoolContents()
		{
			if(contentPrefab == null || container == null)
				return;

			for(int i = 0; i < poolCount; i++)
			{
				MenuContentLayout contentLayout = Instantiate(contentPrefab, container);
				contentLayout.OnSelect += OnContentClicked;
				contentLayoutList.Add(contentLayout);
				contentLayout.gameObject.SetActive(false);
			}
		}
		#endregion


		#region Methods
		public void Open(string context, SearchAction searchAction, Action cancelAction, SelectAction selectAction, bool focused)
		{
			if(shown)
				return;
			
			OnSearch = searchAction;
			OnCancel = cancelAction;
			OnSelect = selectAction;

			InitializeTextField(context);
			ClearContentLayout();
			base.Show();

			if(focused)
				FocusOnText();
		}

		public void SetContents(MenuContent[] contents)
		{
			if(!shown)
				return;
			
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

		private void Close()
		{
			if(!shown)
				return;
			
			OnSearch = null;
			OnSelect = null;
			OnCancel = null;

			ClearContentLayout();
			base.Hide();
		}

		private void ValidateClearButton(string text)
		{
			if(clearButton != null)
				clearButton.gameObject.SetActive(!string.IsNullOrEmpty(text));
		}
		#endregion


		#region Events
		private void OnTextEdit(string text)
		{
			if(OnSearch != null)
				OnSearch(text);

			ValidateClearButton(text);
		}

		private void OnClearButtonClicked()
		{
			if(textField != null)
				textField.text = null;
		}

		private void OnCancelButtonClicked()
		{
			if(OnCancel != null)
				OnCancel();

			Close();
		}

		private void OnContentClicked(int index)
		{
			if(OnSelect != null)
				OnSelect(index);

			Close();
		}
		#endregion


		#region Helpers
		private void RegisterTextFieldAction(SearchAction action)
		{
			if(textField != null)
				textField.onValueChanged.AddListener((t) => { action(t); });
		}

		private void RegisterButtonAction(Button button, Action action)
		{
			if(button != null)
				button.onClick.AddListener(() => { action(); });
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

		private void ClearContentLayout()
		{
			foreach(MenuContentLayout contentLayout in contentLayoutList)
				contentLayout.gameObject.SetActive(false);
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
		#endregion
	}
}