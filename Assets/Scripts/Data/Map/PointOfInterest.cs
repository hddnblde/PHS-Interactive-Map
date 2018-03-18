using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
	[CreateAssetMenu(menuName = "Map/Point of Interest", order = 0, fileName = "Point Of Interest")]
	public class PointOfInterest : ScriptableObject
	{
		[SerializeField, Multiline]
		private string m_tags = "";

		public string tags
		{
			get { return m_tags; }
		}
	}
}