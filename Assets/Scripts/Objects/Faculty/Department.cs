using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Faculty
{
	[CreateAssetMenu(menuName = "Faculty/Department", order = 1, fileName = "Department")]
	public class Department : ScriptableObject
	{
		#region Serialized Fields
		[SerializeField]
		private string m_displayedName;

		[SerializeField]
		private string m_abbreviatedName;
		#endregion


		#region Properties
		public string displayedName
		{
			get { return m_displayedName; }
		}

		public string abbreviatedName
		{
			get { return m_abbreviatedName; }
		}
		#endregion
	}
}