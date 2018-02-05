using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Databases;
using PampangaHighSchool.Students;

namespace Menus
{
	[RequireComponent(typeof(CanvasGroup))]
	public class ClassScheduleMenu : MonoBehaviour
	{
		#region Serialized Fields
		[SerializeField]
		private GameObject itemPrefab = null;

		[SerializeField]
		private RectTransform itemContainer = null;

		[SerializeField]
		private Button backButton = null;

		[SerializeField]
		private Text context = null;
		#endregion


		#region Hidden Fields
		private enum SelectionDepth
		{
			Grades = 0,
			Sections = 1,
			Schedules = 2,
			Schedule = 3
		}

		private SelectionDepth selectionDepth = SelectionDepth.Grades;
		private Grade selectedGrade = Grade.Grade7;
		private int selectedSection = -1;
		private int selectedSectionIndex = -1;
		private const int ItemPoolCount = 20;
		private CanvasGroup canvasGroup = null;
		#endregion


		#region MonoBehaviour Implementation
		private void Awake()
		{
			Initialize();
			CreateItemPool();
		}

		private void Start()
		{
			Show(false);
		}
		#endregion


		#region Initializers
		private void Initialize()
		{
			if(backButton != null)
				backButton.onClick.AddListener(MoveOut);

			canvasGroup = GetComponent<CanvasGroup>();
		}

		private void CreateItemPool()
		{
			if(itemPrefab == null)
				return;
			
			for(int i = 0; i < ItemPoolCount; i++)
			{
				GameObject item = Instantiate(itemPrefab, itemContainer) as GameObject;
				Button itemButton = item.GetComponent<Button>();

				if(itemButton != null)
					itemButton.onClick.AddListener(() => MoveIn(item.transform.GetSiblingIndex()));

				item.SetActive(false);
			}
		}
		#endregion


		#region Actions
		public void Open()
		{
			ShowItems();
			Show(true);
		}

		public void Close()
		{
			Show(false);
		}

		private void ClearAll()
		{
			foreach(Transform item in itemContainer)
				item.gameObject.SetActive(false);
		}

		private void ShowItems()
		{
			string[] items = GetItems();

			bool lastItem = (items != null && items.Length == 1) && (selectionDepth == SelectionDepth.Schedules);

			if(lastItem)
			{
				Debug.Log("Skipping...");
				selectedSectionIndex = 0;
				MoveSelection(1);
				items = GetItems();
			}

			if(items == null || items.Length == 0 || selectionDepth == SelectionDepth.Schedule)
			{
				if(selectedSectionIndex != -1)
				{
					ScheduleMenu.Open(ClassScheduleDatabase.GetSchedule(selectedGrade, selectedSection, selectedSectionIndex));
					MoveSelection((lastItem ? -2 : -1));
				}
				return;
			}

			ClearAll();
			for(int i = 0; i < items.Length; i++)
				ShowItem(i, items[i]);
		}

		private void ShowItem(int index, string label)
		{
			if(itemContainer == null)
				return;

			if(index < 0 || index >= itemContainer.childCount)
				return;

			Transform item = itemContainer.GetChild(index);
			Text text = item.GetComponent<Text>();

			if(text != null)
				text.text = label;

			item.gameObject.SetActive(true);
		}

		private void Show(bool shown)
		{
			if(canvasGroup == null)
				return;

			canvasGroup.alpha = (shown ? 1f : 0f);
			canvasGroup.blocksRaycasts = shown;
		}
		#endregion


		#region Methods
		private void MoveIn(int selectedIndex)
		{
			switch(selectionDepth)
			{
				case SelectionDepth.Grades:
				selectedGrade = GetGradeFromIndex(selectedIndex);
				break;

				case SelectionDepth.Sections:
				selectedSection = selectedIndex;
				break;

				case SelectionDepth.Schedules:
				selectedSectionIndex = selectedIndex;
				break;
			}

			MoveSelection(1);
			ShowItems();
			SetContext();
		}

		private void MoveOut()
		{
			switch(selectionDepth)
			{
				case SelectionDepth.Grades:
				selectedGrade = Grade.Grade7;
				break;

				case SelectionDepth.Sections:
				selectedSection = -1;
				break;

				case SelectionDepth.Schedules:
				selectedSectionIndex = -1;
				break;
			}
			
			MoveSelection(-1);
			ShowItems();
			SetContext();
		}

		private void MoveSelection(int direction)
		{
			int currentDepth = (int)selectionDepth;
			bool close = (currentDepth + direction) < 0;
			currentDepth = Mathf.Clamp(currentDepth + direction, 0, 3);
			selectionDepth = (SelectionDepth)currentDepth;

			if(close)
				Close();
		}

		private void SetContext()
		{
			if(context == null)
				return;

			string contextMessage = "";
			switch(selectionDepth)
			{
				case SelectionDepth.Grades:
				contextMessage = "Classes";
				break;

				case SelectionDepth.Sections:
				contextMessage = "Grade " + (int)selectedGrade;
				break;

				case SelectionDepth.Schedules:
				contextMessage = "Choose Section";
				break;
			}

			context.text = contextMessage;
		}
		#endregion


		#region Helpers
		private string[] GetItems()
		{
			switch(selectionDepth)
			{
				case SelectionDepth.Grades:
				return ClassScheduleDatabase.GetGradeItems();

				case SelectionDepth.Sections:
				return ClassScheduleDatabase.GetSectionItems(selectedGrade);

				case SelectionDepth.Schedules:
				return ClassScheduleDatabase.GetScheduleItems(selectedGrade, selectedSection);

				default:
				return null;
			}
		}

		private Grade GetGradeFromIndex(int index)
		{
			index = Mathf.Clamp(index, 0, 5);
			
			if(index == 0)
				return Grade.Grade7;
			else if(index == 1)
				return Grade.Grade8;
			else if(index == 2)
				return Grade.Grade9;
			else if(index == 3)
				return Grade.Grade10;
			else if(index == 4)
				return Grade.Grade11;
			else
				return Grade.Grade12;
		}
		#endregion
	}
}