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
		private List<RoomGroup> m_rooms = RoomGroup.DefaultList;
		#endregion


		#region Functions
		private RoomGroup GetRoom(Day day)
		{
			RoomGroup room = null;

			foreach(RoomGroup scheduledRoom in m_rooms)
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


		#region Properties
		public RoomGroup[] rooms
		{
			get
			{
				List<RoomGroup> rooms = new List<RoomGroup>();
				
				const int days = 5;
				for(int dayIndex = 0; dayIndex < days; dayIndex++)
				{
					Day day = (Day)dayIndex;
					RoomGroup room = GetRoom(day);
					rooms.Add(room);
				}

				return rooms.ToArray();
			}
		}

		public Period period
		{
			get { return m_period; }
		}
		#endregion
	}
}