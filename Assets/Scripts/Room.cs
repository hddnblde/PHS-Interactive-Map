using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
	[CreateAssetMenu(menuName = "Map/Room", order = 2, fileName = "Room")]
	public class Room : Location
	{
		[SerializeField]
		private int m_floor = 1;

		public int floor
		{
			get { return m_floor; }
		}
	}
}