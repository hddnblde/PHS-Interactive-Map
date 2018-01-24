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
		private ScheduleObject target = null;

		[SerializeField]
		private List<PeriodGroup> m_periods = PeriodGroup.DefaultList;

		public PeriodGroup[] periods
		{
			get { return m_periods.ToArray(); }
		}

		public string title
		{
			get
			{
				if(target == null)
					return "";
				else
					return target.title;
			}
		}

		public string subtitle
		{
			get
			{
				if(target == null)
					return "";
				else
					return target.subtitle;
			}
		}
	}
}