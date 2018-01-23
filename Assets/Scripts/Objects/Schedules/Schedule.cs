using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;
using PampangaHighSchool.Students;

namespace Schedules
{
	[CreateAssetMenu(fileName = "Schedule", menuName = "Schedules/Schedule", order = 0)]
	public class Schedule : ScriptableObject
	{
		[SerializeField]
		private ScheduleObject m_object = null;

		[SerializeField]
		private List<PeriodGroup> m_periods = PeriodGroup.DefaultList;

		public PeriodGroup[] periods
		{
			get { return m_periods.ToArray(); }
		}
	}
}