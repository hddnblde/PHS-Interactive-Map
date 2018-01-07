using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;

namespace Faculty.Schedules
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
	public class ScheduleItem
	{
		public ScheduleItem(Period period)
		{
			m_period = period;
		}

		#region Serialized Fields
		[SerializeField]
		private Period m_period = null;

		[SerializeField]
		private List<ScheduledRoom> scheduledRooms = new List<ScheduledRoom>();
		#endregion


		#region Function
		public ScheduledRoom GetRoom(Day day)
		{
			ScheduledRoom room = null;

			foreach(ScheduledRoom scheduledRoom in scheduledRooms)
			{
				if(scheduledRoom.day == day)
				{
					room = scheduledRoom;
					break;
				}
			}

			return room;
		}
		#endregion
	}
}