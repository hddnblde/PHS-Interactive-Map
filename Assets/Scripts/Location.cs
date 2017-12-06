using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
	public class Location : ScriptableObject
	{
		[SerializeField]
		private string m_displayedName;

		[SerializeField]
		private Vector2 m_position;

		#region Properties
		public string displayedName
		{
			get { return m_displayedName; }
		}

		public Vector3 position
		{
			get { return new Vector3(m_position.x, 0f, m_position.y); }
		}
		#endregion
	}
}