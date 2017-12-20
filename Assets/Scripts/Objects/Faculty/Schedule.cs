using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;

namespace Faculty.Schedules
{
	[CreateAssetMenu(menuName = "Faculty/Schedules/Schedule", order = 2, fileName = "Schedule")]
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
		private ScheduledRoom m_monday = new ScheduledRoom();

		[SerializeField]
		private ScheduledRoom m_tuesday = new ScheduledRoom();

		[SerializeField]
		private ScheduledRoom m_wednesday = new ScheduledRoom();

		[SerializeField]
		private ScheduledRoom m_thursday = new ScheduledRoom();

		[SerializeField]
		private ScheduledRoom m_friday = new ScheduledRoom();
		#endregion


		#region Properties
		public Period period
		{
			get { return m_period; }
		}

		public ScheduledRoom monday
		{
			get { return m_monday; }
		}

		public ScheduledRoom tuesday
		{
			get { return m_tuesday; }
		}

		public ScheduledRoom wednesday
		{
			get { return m_wednesday; }
		}

		public ScheduledRoom thursday
		{
			get { return m_thursday; }
		}

		public ScheduledRoom friday
		{
			get { return m_friday; }
		}
		#endregion
	}
}