using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Schedules
{
	public enum Day
	{
		Monday = 1,
		Tuesday = 2,
		Wednesday = 3,
		Thursday = 4,
		Friday = 5
	}

	[System.Serializable]
	public class PeriodGroup
	{
		public static List<PeriodGroup> DefaultList
		{
			get
			{
				List<PeriodGroup> periodList = new List<PeriodGroup>();
				const int periods = 14;

				for(int periodIndex = 0; periodIndex < periods; periodIndex++)
				{
					PeriodGroup period = new PeriodGroup(null);
					periodList.Add(period);
				}

				return periodList;
			}
		}
		public PeriodGroup(Period period)
		{
			m_period = period;
		}

		#region Serialized Fields
		[SerializeField]
		private Period m_period = null;

		[SerializeField]
		private List<ScheduleEntry> m_entries = ScheduleEntry.DefaultList;
		#endregion


		#region Functions
		public ScheduleEntry GetEntry(Day day)
		{
			ScheduleEntry entry = null;

			foreach(ScheduleEntry e in m_entries)
			{
				if(e.day == day)
				{
					entry = e;
					break;
				}
			}

			return entry;
		}
		#endregion


		#region Properties
		public ScheduleEntry[] entries
		{
			get
			{
				List<ScheduleEntry> entries = new List<ScheduleEntry>();
				
				const int days = 5;
				for(int dayIndex = 0; dayIndex < days; dayIndex++)
				{
					Day day = (Day)dayIndex;
					ScheduleEntry entry = GetEntry(day);
					entries.Add(entry);
				}

				return entries.ToArray();
			}
		}

		public Period period
		{
			get { return m_period; }
		}
		#endregion
	}
}