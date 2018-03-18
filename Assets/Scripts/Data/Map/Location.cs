using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
	public class Location : ScriptableObject
	{
		[SerializeField, Multiline]
		private string m_displayedName;

		[SerializeField]
		private Vector3 m_displayPosition = Vector3.zero;

		[SerializeField]
		private Vector3 m_position = Vector3.zero;

		[SerializeField]
		private bool m_useDisplayPosition = true;

		[SerializeField, Multiline]
		private string m_tags;

		[SerializeField]
		private bool m_isHidden = false;

		#region Properties
		public virtual string displayedName
		{
			get { return m_displayedName; }
		}

		public virtual Vector3 position
		{
			get { return m_position; }
		}

		public virtual Vector3 displayPosition
		{
			get { return (m_useDisplayPosition ? m_displayPosition : position); }
		}

		public string tags
		{
			get { return m_tags; }
		}

		public bool isHidden
		{
			get { return m_isHidden; }
		}
		#endregion
	}
}