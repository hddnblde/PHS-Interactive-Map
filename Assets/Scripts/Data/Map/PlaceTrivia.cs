using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
	[CreateAssetMenu(menuName = "Map/Place Trivia", order = 2, fileName = "Place Trivia")]
	public class PlaceTrivia : ScriptableObject
	{
		[SerializeField]
		private Sprite m_thumbnail = null;

		[SerializeField, Multiline]
		private string m_details;

		public Sprite thumbnail
		{
			get { return m_thumbnail; }
		}

		public string details
		{
			get { return m_details; }
		}
	}
}