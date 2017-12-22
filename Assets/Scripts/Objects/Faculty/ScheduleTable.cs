using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;

namespace Faculty.Schedules
{
	[System.Serializable]
	public class ScheduleItem
	{
		public ScheduleItem(Period period)
		{
			m_period = period;
		}

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