using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Students
{
	public enum Grade
	{
		Grade7 = 7,
		Grade8 = 8,
		Grade9 = 9,
		Grade10 = 10,
		Grade11 = 11,
		Grade12 = 12
	}

	[CreateAssetMenu(menuName = "Students/Section", order = 0, fileName = "Section")]
	public class Section : ScriptableObject
	{
		[Header("Description")]
		[SerializeField]
		private Grade m_grade = Grade.Grade7;

		[SerializeField]
		private int m_order = 1;

		[SerializeField]
		private int m_count = 1;

		public Grade grade
		{
			get { return m_grade; }
		}

		public int order
		{
			get { return m_order; }
		}

		public int count
		{
			get { return m_count; }
		}
	}
}