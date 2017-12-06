using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
	[CreateAssetMenu(menuName = "Map/Landmark", order = 0, fileName = "Landmark")]
	public class Landmark : ScriptableObject
	{
		[SerializeField]
		private bool m_hasRooms = false;

		[SerializeField]
		private Sprite m_icon = null;

		[SerializeField, Multiline]
		private string m_description = "";

		public bool hasRooms
		{
			get { return m_hasRooms; }
		}

		public Sprite icon
		{
			get { return m_icon; }
		}

		public string description
		{
			get { return m_description; }
		}
	}
}