using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;
using Students;

namespace Faculty.Schedules
{
	[System.Serializable]
	public class ScheduledRoom
	{
		public ScheduledRoom()
		{}

		public ScheduledRoom(Room room, StudentClass studentClass)
		{
			m_room = room;
			m_studentClass = studentClass;
		}

		[SerializeField]
		private Day m_day = Day.Monday;

		[SerializeField]
		private Room m_room = null;

		[SerializeField]
		private StudentClass m_studentClass = null;

		public Day day
		{
			get { return m_day; }
		}

		public Room room
		{
			get { return m_room; }
		}

		public StudentClass studentClass
		{
			get { return m_studentClass; }
		}
	}
}