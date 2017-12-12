using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
	[CreateAssetMenu(menuName = "Map/Landmark", order = 0, fileName = "Landmark")]
	public class Landmark : ScriptableObject
	{
		[SerializeField]
		private Sprite m_icon = null;

		[SerializeField, Multiline]
		private string m_tags = "";

		public Sprite icon
		{
			get { return m_icon; }
		}

		public string tags
		{
			get { return m_tags; }
		}
	}
}