using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
	[System.Serializable]
	public class PlaceCluster
	{
		#region Fields
		public PlaceCluster(Place place, List<Room> rooms)
		{
			this.place = place;
			this.rooms = rooms;
		}

		[SerializeField]
		private Place place = null;

		[SerializeField]
		private List<Room> rooms = new List<Room>();

		public int count
		{
			get
			{
				int placeCount = (place != null ? 1 : 0);
				if(HasRooms())
					return rooms.Count + placeCount;
				else
					return placeCount;
			}
		}
		#endregion


		#region Functions
		private bool HasRooms()
		{
			return rooms != null && rooms.Count > 0;
		}

		public Location GetLocation(int index)
		{
			if(index < 0)
				return null;
			else if(!HasRooms())
				return place as Location;
			else
			{
				if((index - 1) >= rooms.Count)
					return null;
				else
					return rooms[index - 1] as Location;
			}
		}
		#endregion
	}
}