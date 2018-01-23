using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;
using PampangaHighSchool.Students;

namespace Schedules
{
	[System.Serializable]
	public class RoomGroup
	{
		public static List<RoomGroup> DefaultList
		{
			get
			{
				List<RoomGroup> roomList = new List<RoomGroup>();
				const int days = 5;
				
				for(int dayIndex = 0; dayIndex < days; dayIndex++)
				{
					Day day = (Day)(dayIndex + 1);
					RoomGroup room = new RoomGroup(day);
					roomList.Add(room);
				}

				return roomList;
			}
		}
		[SerializeField]
		private Day m_day = Day.Monday;

		[SerializeField]
		private Room m_room = null;

		[SerializeField]
		private ScheduleObject m_entity = null;

		public RoomGroup(Day day)
		{
			Constructor(day, null, null);
		}

		public RoomGroup(Day day, Room room)
		{
			Constructor(day, room, null);
		}

		public RoomGroup(Day day, Room room, ScheduleObject entity)
		{
			Constructor(day, room, entity);
		}

		private void Constructor(Day day, Room room, ScheduleObject entity)
		{
			m_day = day;
			m_room = room;
			m_entity = entity;
		}

		public Day day
		{
			get { return m_day; }
		}

		public Room room
		{
			get { return m_room; }
		}

		public ScheduleObject entity
		{
			get { return m_entity; }
		}
	}
}