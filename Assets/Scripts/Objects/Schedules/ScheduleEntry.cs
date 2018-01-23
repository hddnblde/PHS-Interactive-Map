using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;
using PampangaHighSchool.Students;

namespace Schedules
{
	[System.Serializable]
	public class ScheduleEntry
	{
		public static List<ScheduleEntry> DefaultList
		{
			get
			{
				List<ScheduleEntry> entryList = new List<ScheduleEntry>();
				const int days = 5;
				
				for(int dayIndex = 0; dayIndex < days; dayIndex++)
				{
					Day day = (Day)(dayIndex + 1);
					ScheduleEntry room = new ScheduleEntry(day);
					entryList.Add(room);
				}

				return entryList;
			}
		}

		[SerializeField]
		private Day m_day = Day.Monday;

		[SerializeField]
		private Room m_room = null;

		[Header("Entry")]
		[SerializeField]
		private ScheduleObject m_title = null;

		[SerializeField]
		private string m_subtitle = "";

		public ScheduleEntry(Day day)
		{
			Constructor(day, null, null, "");
		}

		public ScheduleEntry(Day day, Room room)
		{
			Constructor(day, room, null, "");
		}

		public ScheduleEntry(Day day, Room room, ScheduleObject title, string subtitle)
		{
			Constructor(day, room, title, subtitle);
		}

		private void Constructor(Day day, Room room, ScheduleObject title, string subtitle)
		{
			m_day = day;
			m_room = room;
			m_title = title;
		}

		public Day day
		{
			get { return m_day; }
		}

		public Room room
		{
			get { return m_room; }
		}

		public string title
		{
			get
			{
				if(m_title == null)
					return "";
				else
					return m_title.subtitle;
			}
		}

		public string subtitle
		{
			get { return m_subtitle; }
		}
	}
}