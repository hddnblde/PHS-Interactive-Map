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
		private Vector3 m_position = Vector3.zero;

		[SerializeField, Multiline]
		private string m_tags;

		#region Properties
		public virtual string displayedName
		{
			get { return m_displayedName; }
		}

		public Vector3 position
		{
			get { return m_position; }
		}

		public string tags
		{
			get { return m_tags; }
		}
		#endregion
	}
}