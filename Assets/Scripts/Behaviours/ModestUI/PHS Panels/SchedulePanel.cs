using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ModestUI.Panels;
using Schedules;
using PampangaHighSchool.Faculty;
using PampangaHighSchool.Students;

namespace Menus.PHS
{
	public class SchedulePanel : SimplePanel
	{
		#region Serialized Fields
		[Header("References")]
		[SerializeField]
		private Text titleText = null;

		[SerializeField]
		private GameObject scheduleItemPrefab = null;

		[SerializeField]
		private Transform entryContainer = null;

		[SerializeField]
		private Day currentDay = Day.Monday;

		[Header("Buttons")]
		[SerializeField]
		private Button mondayButton = null;

		[SerializeField]
		private Button tuesdayButton = null;

		[SerializeField]
		private Button wednesdayButton = null;

		[SerializeField]
		private Button thursdayButton = null;

		[SerializeField]
		private Button fridayButton = null;

		[Header("Button Colors")]
		[SerializeField, ColorUsage(false)]
		private Color normalColor = Color.black;

		[SerializeField, ColorUsage(false)]
		private Color highlightedColor = Color.white;
		#endregion


		#region Unserialized Fields
		private Schedule schedule = null;
		private List<ScheduleItem> items = new List<ScheduleItem>();
		private Graphic mondayText = null;
		private Graphic tuesdayText = null;
		private Graphic wednesdayText = null;
		private Graphic thursdayText = null;
		private Graphic fridayText = null;
		#endregion


		#region MonoBehaviour Implementation
		protected override void Awake()
		{
			base.Awake();
			GenerateEntries();
			SetupButtons();
		}
		#endregion


		#region Initializers
		private void GenerateEntries()
		{
			if(scheduleItemPrefab == null || entryContainer == null)
				return;

			const int EntryCount = 14;

			for(int i = 0; i < EntryCount; i++)
			{
				GameObject entry = Instantiate(scheduleItemPrefab, entryContainer) as GameObject;
				ScheduleItem item = entry.GetComponent<ScheduleItem>();
				items.Add(item);
			}
		}

		private void SetupButtons()
		{
			SetupButton(mondayButton, ref mondayText, Day.Monday);
			SetupButton(tuesdayButton, ref tuesdayText, Day.Tuesday);
			SetupButton(wednesdayButton, ref wednesdayText, Day.Wednesday);
			SetupButton(thursdayButton, ref thursdayText, Day.Thursday);
			SetupButton(fridayButton, ref fridayText, Day.Friday);
		}

		private void SetupButton(Button button, ref Graphic textGraphic, Day day)
		{
			if(button == null)
				return;

			button.onClick.AddListener(() => SelectDay(day));
			Transform textTransform = button.transform.GetChild(0);

			if(textTransform == null)
				return;

			textGraphic = textTransform.GetComponent<Graphic>();
		}
		#endregion


		#region Actions
		public override bool Open()
		{
			return false;
		}

		public bool Open(Schedule schedule)
		{
			if(!base.Open())
				return false;

			this.schedule = schedule;
			SetTitle();
			SetPeriods();
			ValidateDayButtons();
			SelectFirstActive();
			return true;
		}
		#endregion


		#region Methods
		private void SelectDay(Day day)
		{
			currentDay = day;
			SelectButton(mondayButton, mondayText, day == Day.Monday);
			SelectButton(tuesdayButton, tuesdayText, day == Day.Tuesday);
			SelectButton(wednesdayButton, wednesdayText, day == Day.Wednesday);
			SelectButton(thursdayButton, thursdayText, day == Day.Thursday);
			SelectButton(fridayButton, fridayText, day == Day.Friday);
			ViewEntries();
		}

		private void SelectButton(Button button, Graphic text, bool selected)
		{
			if(button == null || text == null)
				return;

			button.interactable = !selected;
			Color textColor = (selected ? highlightedColor : normalColor);
			text.color = textColor;
		}

		private void ViewEntries()
		{
			if(items == null || items.Count == 0)
				return;

			foreach(ScheduleItem item in items)
				item.ViewEntry(currentDay);
		}
		#endregion


		#region Helpers
		private void SetTitle()
		{
			if(schedule == null || titleText == null)
				return;

			string pattern = "<b>@title</b>\n@subtitle".Replace("@title", schedule.title).Replace("@subtitle", schedule.subtitle);
			titleText.text = pattern;
		}

		private void SetPeriods()
		{
			if(items == null || items.Count == 0 || schedule == null)
				return;
			
			for(int i = 0; i < items.Count; i++)
			{
				ScheduleItem item = items[i];
				PeriodGroup period = schedule.periods[i];
				item.Set(period);
			}
		}

		private void ValidateDayButtons()
		{
			ValidateDayButton(mondayButton, Day.Monday);
			ValidateDayButton(tuesdayButton, Day.Tuesday);
			ValidateDayButton(wednesdayButton, Day.Wednesday);
			ValidateDayButton(thursdayButton, Day.Thursday);
			ValidateDayButton(fridayButton, Day.Friday);
		}

		private void ValidateDayButton(Button button, Day day)
		{
			if(button != null)
				button.gameObject.SetActive(!DayIsEmpty(day));
		}

		private bool DayIsEmpty(Day day)
		{
			if(schedule == null || schedule.periods == null || schedule.periods.Length == 0)
				return true;
			
			int emptyPeriodCount = 0;
			int periodCount = schedule.periods.Length;

			for(int i = 0; i < periodCount; i++)
			{
				PeriodGroup period = schedule.periods[i];
				ScheduleEntry entry = period.GetEntry(day);

				if(entry == null)
					continue;

				if(entry.isEmpty)
					emptyPeriodCount++;
			}

			return emptyPeriodCount >= periodCount;
		}

		private void SelectFirstActive()
		{
			if(mondayButton.gameObject.activeInHierarchy)
			{
				SelectDay(Day.Monday);
				return;
			}
			else if(tuesdayButton.gameObject.activeInHierarchy)
			{
				SelectDay(Day.Tuesday);
				return;
			}
			else if(wednesdayButton.gameObject.activeInHierarchy)
			{
				SelectDay(Day.Wednesday);
				return;
			}
			else if(thursdayButton.gameObject.activeInHierarchy)
			{
				SelectDay(Day.Thursday);
				return;
			}
			else if(fridayButton.gameObject.activeInHierarchy)
			{
				SelectDay(Day.Friday);
				return;
			}
		}
		#endregion
	}
}