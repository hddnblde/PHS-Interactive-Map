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
		private Vector3 m_position;

		[SerializeField, Multiline]
		private string m_tags;

		#region Properties
		public virtual string displayedName
		{
			get { return m_displayedName; }
		}

		public Vector3 position
		{
			get { return new Vector3(m_position.x, 0f, m_position.y); }
		}

		public string tags
		{
			get { return m_tags; }
		}
		#endregion
	}
}