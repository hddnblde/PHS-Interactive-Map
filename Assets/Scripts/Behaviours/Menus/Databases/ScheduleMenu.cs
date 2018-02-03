using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PampangaHighSchool;
using Schedules;
using PampangaHighSchool.Faculty;
using PampangaHighSchool.Students;

namespace Menus
{
	public class ScheduleMenu : MonoBehaviour
	{
		#region Serialized Fields
		[Header("References")]
		[SerializeField]
		private Text titleText = null;

		[SerializeField]
		private Schedule schedule = null;

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

		[Header("Text Colors")]
		[SerializeField, ColorUsage(false)]
		private Color normalColor = Color.black;

		[SerializeField, ColorUsage(false)]
		private Color highlightedColor = Color.white;
		#endregion


		#region Hidden Fields
		private delegate void OpenAction(Schedule schedule);
		private delegate void CloseAction();

		private static event OpenAction OnOpenMenu;
		private static event CloseAction OnCloseMenu;

		private List<ScheduleItem> items = new List<ScheduleItem>();
		private Graphic mondayText = null;
		private Graphic tuesdayText = null;
		private Graphic wednesdayText = null;
		private Graphic thursdayText = null;
		private Graphic fridayText = null;

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
			GenerateEntries();
			SetupButtons();
		}

		private void Start()
		{
			Open(schedule);
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

		private void RegisterInternalEvents()
		{
			OnOpenMenu += Internal_Open;
		}

		private void DeregisterInternalEvents()
		{
			OnOpenMenu -= Internal_Open;
		}
		#endregion


		#region Actions
		public static void Open(Schedule schedule)
		{
			if(m_isOpen)
			{
				Debug.Log("Schedule is already opened.");
				return;
			}

			if(OnOpenMenu != null)
				OnOpenMenu(schedule);

			m_isOpen = true;
		}

		public static void Close()
		{
			if(!m_isOpen)
			{
				Debug.Log("Schedule is already closed.");
				return;
			}

			if(OnCloseMenu != null)
				OnCloseMenu();

			m_isOpen = false;
		}

		private void Internal_Open(Schedule schedule)
		{
			this.schedule = schedule;
			SetTitle();
			SetPeriods();
			SelectDay(currentDay);
		}

		private void Internal_Close()
		{
			
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
			if(items == null || items.Count == 0)
				return;
			
			for(int i = 0; i < items.Count; i++)
			{
				ScheduleItem item = items[i];
				PeriodGroup period = schedule.periods[i];
				item.Set(period);
			}
		}

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

		private void ViewEntries()
		{
			if(items == null || items.Count == 0)
				return;
			
			foreach(ScheduleItem item in items)
				item.ViewEntry(currentDay);
		}

		private void SelectButton(Button button, Graphic text, bool selected)
		{
			if(button == null || text == null)
				return;

			button.interactable = !selected;
			Color textColor = (selected ? highlightedColor : normalColor);
			text.color = textColor;
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
	}
}