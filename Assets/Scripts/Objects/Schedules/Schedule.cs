using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;

namespace Schedules
{
	[CreateAssetMenu(menuName = "Schedules/Schedule", order = 2, fileName = "Schedule")]
	public class Schedule : ScriptableObject
	{
		[SerializeField]
		private List<ScheduleItem> table = new List<ScheduleItem>();

		public int periodCount
		{
			get
			{
				if(table == null)
					return 0;
				else
					return table.Count;
			}
		}

		public ScheduleItem GetItem(int period)
		{
			if(period == -1 || periodCount == 0 || period >= periodCount)
				return null;
			else
				return table[period];
		}
	}

	[System.Serializable]
	public class ScheduleItem
	{
		#region Serialized Fields
		[SerializeField]
		private Period m_period = null;

		[Header("Daily Room Assignment")]
		[SerializeField]
		private Room m_monday = null;

		[SerializeField]
		private Room m_tuesday = null;

		[SerializeField]
		private Room m_wednesday = null;

		[SerializeField]
		private Room m_thursday = null;

		[SerializeField]
		private Room m_friday = null;
		#endregion


		#region Properties
		public Period period
		{
			get { return m_period; }
		}

		public Room monday
		{
			get { return m_monday; }
		}

		public Room tuesday
		{
			get { return m_tuesday; }
		}

		public Room wednesday
		{
			get { return m_wednesday; }
		}

		public Room thursday
		{
			get { return m_thursday; }
		}

		public Room friday
		{
			get { return m_friday; }
		}
		#endregion
	}
}