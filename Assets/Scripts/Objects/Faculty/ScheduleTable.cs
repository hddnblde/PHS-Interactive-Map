using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;

namespace Faculty.Schedules
{
	[CreateAssetMenu(menuName = "Faculty/Schedules/Table", order = 0, fileName = "Schedule Table")]
	public class ScheduleTable : ScriptableObject
	{
		[SerializeField]
		private List<ScheduleItem> items = new List<ScheduleItem>();

		public int periodCount
		{
			get
			{
				if(items == null)
					return 0;
				else
					return items.Count;
			}
		}

		public ScheduleItem GetItem(int period)
		{
			if(period == -1 || periodCount == 0 || period >= periodCount)
				return null;
			else
				return items[period];
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
		private ScheduledRoom m_monday = null;

		[SerializeField]
		private ScheduledRoom m_tuesday = null;

		[SerializeField]
		private ScheduledRoom m_wednesday = null;

		[SerializeField]
		private ScheduledRoom m_thursday = null;

		[SerializeField]
		private ScheduledRoom m_friday = null;
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