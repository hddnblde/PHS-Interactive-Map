using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Faculty;
using Map;

namespace Students
{
	[CreateAssetMenu(menuName = "Students/Section Cluster", order = 1, fileName = "Section Cluster")]
	public class SectionCluster : ScriptableObject
	{
		[Header("Description")]
		[SerializeField]
		private Section m_section = null;

		[SerializeField]
		private int m_rank = 1;

		[Header("Advisory")]
		[SerializeField]
		private Teacher m_adviser = null;

		[SerializeField]
		private Room m_room = null;

		public Section section
		{
			get { return m_section; }
		}

		public int rank
		{
			get { return m_rank; }
		}
	
		public Teacher adviser
		{
			get { return m_adviser; }
		}
	
		public Room advisoryRoom
		{
			get { return m_room; }
		}
	}
}
