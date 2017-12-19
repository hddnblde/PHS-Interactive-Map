using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Faculty
{
	[CreateAssetMenu(menuName = "Faculty/Course", order = 2, fileName = "Course")]
	public class Course : ScriptableObject
	{
		#region Serialized Fields
		[SerializeField]
		private string m_displayedName;

		[SerializeField]
		private string m_majorSubject;

		[SerializeField]
		private string m_minorSubject;
		#endregion


		#region Properties
		private string displayedName
		{
			get { return m_displayedName; }
		}

		private string majorSubject
		{
			get { return m_majorSubject; }
		}

		private string minorSubject
		{
			get { return m_minorSubject; }
		}
		#endregion
	}
}