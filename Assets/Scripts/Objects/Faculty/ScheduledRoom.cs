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
		{
			
		}

		public ScheduledRoom(Room room, SectionCluster sectionCluster)
		{
			m_room = room;
			m_sectionCluster = sectionCluster;
		}

		[SerializeField]
		private Room m_room = null;

		[SerializeField]
		private SectionCluster m_sectionCluster = null;

		public Room room
		{
			get { return m_room; }
		}

		public SectionCluster sectionCluster
		{
			get { return m_sectionCluster; }
		}
	}
}