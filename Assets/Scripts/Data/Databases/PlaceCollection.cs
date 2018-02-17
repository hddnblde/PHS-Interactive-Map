using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
	[System.Serializable]
	public class PlaceCollection
	{
		public PlaceCollection(Place place, List<Room> rooms)
		{
			this.place = place;
			this.rooms = rooms;
		}

		#region Fields
		[SerializeField]
		private Place place = null;

		[SerializeField]
		private List<Room> rooms = new List<Room>();

		public int count
		{
			get
			{
				int placeCount = (place != null ? 1 : 0);
				if(hasRooms)
					return rooms.Count + placeCount;
				else
					return placeCount;
			}
		}

		public int roomCount
		{
			get { return (rooms != null ? rooms.Count : 0); }
		}

		public bool hasRooms
		{
			get { return roomCount > 0; }
		}
		#endregion


		#region Functions
		public Location GetPlaceLocation()
		{
			return place as Location;
		}

		public Place GetPlace()
		{
			return place;
		}

		public Room GetRoom(int index)
		{
			if(index < 0 || !hasRooms || index >= rooms.Count)
				return null;
			else
				return rooms[index];
		}

		public Location GetRoomLocation(int index)
		{
			Room room = GetRoom(index);
			if(room == null)
				return null;
			else
				return room as Location;
		}

		public Location GetLocation(int index)
		{
			if(index < 0)
				return null;
			else if(!hasRooms)
				return place as Location;
			else
			{
				if(index == 0)
					return place as Location;
				else
					return rooms[index - 1] as Location;
			}
		}

		public bool HasRoom(Room room)
		{
			if(room == null)
				return false;
			else
				return rooms.Contains(room);
		}
		#endregion
	}
}