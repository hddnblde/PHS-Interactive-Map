using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ModestUI.Panels;
using Databases;
using Map;

namespace Menus.PHS
{
	public class SearchLocationPanel : SimplePanel
	{
		#region Serialized Fields
		[Header("References")]
		[SerializeField]
		private Text displayedContext = null;

		[SerializeField]
		private InputField textField = null;

		[SerializeField]
		private RectTransform contentContainer = null;

		[SerializeField]
		private Button clearButton = null;

		[SerializeField]
		private Graphic searchIcon = null;

		[Header("Contents")]
		[SerializeField]
		private MenuContentLayout contentPrefab = null;

		[SerializeField]
		private int poolCount = 70;		
		#endregion


		#region Unserialized Fields
		private const string DefaultContext = "Seach";
		public delegate void SelectItem(int index);
		private List<MenuContentLayout> contentLayoutList = new List<MenuContentLayout>();
		private event SelectItem OnSelectItem;
		#endregion


		#region MonoBehaviour Implementation
		protected override void Awake()
		{
			base.Awake();
			Initialize();
			PoolContents();
		}

		private void OnEnable()
		{
			RegisterEvent();
		}

		private void OnDisable()
		{
			DeregisterEvent();
		}
		#endregion


		#region Initializers
		private void Initialize()
		{
			if(textField != null)
				textField.onValueChanged.AddListener(OnTextEdit);

			if(clearButton != null)
				clearButton.onClick.AddListener(OnClearButtonClicked);
		}

		private void RegisterEvent()
		{
			LocationDatabase.OnResult += OnResult;
		}

		private void DeregisterEvent()
		{
			LocationDatabase.OnResult -= OnResult;
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
		#endregion


		#region Events
		private void OnResult(int count)
		{
			MenuContent[] contents = null;
			
			if(count > 0)
			{
				contents = new MenuContent[count];
				for(int i = 0; i < count; i++)
				{
					Location location = LocationDatabase.GetLocationFromSearch(i);
					Sprite thumbnail = null;

					if(location is Place)
					{
						Place building = location as Place;
						thumbnail = building.thumbnail;
					}

					contents[i] = new MenuContent(thumbnail, location.displayedName);
				}
			}

			SetContents(contents);
		}

		private void OnClearButtonClicked()
		{
			if(textField != null)
				textField.text = null;
		}

		private void OnTextEdit(string text)
		{
			LocationDatabase.Search(text);

			bool hasText = !string.IsNullOrEmpty(text);

			if(clearButton != null)
				clearButton.gameObject.SetActive(hasText);

			if(searchIcon != null)
				searchIcon.gameObject.SetActive(!hasText);
		}

		private void OnContentClicked(int index)
		{
			if(!visible)
				return;
			
			if(OnSelectItem != null)
				OnSelectItem(index);

			OnSelectItem = null;
			Close();
		}
		#endregion


		#region Actions
		public override bool Open()
		{
			return Open(DefaultContext, null);
		}

		public bool Open(string context, SelectItem selectItemCallback)
		{
			if(!base.Open())
				return false;

			SetContext(context);
			OnSelectItem = selectItemCallback;
			return true;
		}
		#endregion


		#region Helpers
		private void SetContents(MenuContent[] contents)
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

		private void SetContext(string context)
		{
			if(textField != null)
				textField.text = "";

			if(displayedContext != null)
				displayedContext.text = context;
		}
		#endregion
	}
}