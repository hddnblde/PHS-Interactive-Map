using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Databases;
using PampangaHighSchool.Students;

namespace Menus
{
	public class ClassScheduleMenu : MonoBehaviour
	{
		#region Serialized Fields
		[SerializeField]
		private GameObject itemPrefab = null;

		[SerializeField]
		private RectTransform itemContainer = null;

		[SerializeField]
		private Button backButton = null;
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
		#endregion


		#region MonoBehaviour Implementation
		private void Awake()
		{
			Initialize();
			CreateItemPool();
		}

		private void Start()
		{
			ShowItems();
		}
		#endregion


		#region Initializers
		private void Initialize()
		{
			if(backButton != null)
				backButton.onClick.AddListener(MoveOut);
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
		private void ClearAll()
		{
			foreach(Transform item in itemContainer)
				item.gameObject.SetActive(false);
		}

		private void ShowItems()
		{
			string[] items = GetItems();

			if(items == null || items.Length == 0)
			{
				if(selectedSectionIndex != -1)
					ScheduleMenu.Open(ClassScheduleDatabase.GetSchedule(selectedGrade, selectedSection, selectedSectionIndex));
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
		}

		private void MoveSelection(int direction)
		{
			int currentDepth = (int)selectionDepth;
			currentDepth = Mathf.Clamp(currentDepth + direction, 0, 3);
			selectionDepth = (SelectionDepth)currentDepth;
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