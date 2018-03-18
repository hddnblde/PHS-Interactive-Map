using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;

namespace Menus
{
	public class LocationMarker
	{
		public LocationMarker(string displayedName, Vector3 position)
		{
			m_position = position;
			m_displayedName = displayedName;
		}

		public LocationMarker(Location location)
		{
			if(location == null)
				return;
			
			m_position = location.position;
			m_displayedName = location.displayedName;
		}

		private Vector3 m_position = Vector3.zero;
		private string m_displayedName = "Marker";

		public Vector3 position
		{
			get { return m_position; }
		}

		public string displayedName
		{
			get { return m_displayedName; }
		}
	}
}