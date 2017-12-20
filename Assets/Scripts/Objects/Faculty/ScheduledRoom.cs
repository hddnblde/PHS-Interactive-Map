using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;
using Students;

namespace Faculty.Schedules
{
	[CreateAssetMenu(menuName = "Faculty/Schedules/Scheduled Room", order = 1, fileName = "Scheduled Room")]
	public class ScheduledRoom : ScriptableObject
	{
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