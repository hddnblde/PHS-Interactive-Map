using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;

namespace Menus
{
	public class MapMarkerSystem : MonoBehaviour
	{
		[SerializeField]
		private Transform markerContainer = null;

		[SerializeField]
		private LocationDatabase locationDatabase = null;

		[SerializeField]
		private GameObject mapMarkerPrefab = null;

		private void Awake()
		{
			CreateMarkers();
		}

		private void CreateMarkers()
		{
			if(locationDatabase == null)
				return;

			Location[] locationTable = locationDatabase.GetAllLocations();

			if(locationTable == null || locationTable.Length == 0)
				return;

			foreach(Location location in locationTable)
			{
				if(location == null)
					continue;

				bool isRoom = (location as Room) != null;

				if(isRoom)
					continue;

				CreateMarker(location);
			}
		}

		private void CreateMarker(Location location)
		{
			if(location == null || mapMarkerPrefab == null)
				return;

			GameObject markerObject = Instantiate(mapMarkerPrefab, markerContainer) as GameObject;
			MapMarker marker = markerObject.GetComponent<MapMarker>();

			if(marker != null)
				marker.Set(GetThumbnailFromLocation(location), location.displayedName, location.position);
		}

		private Sprite GetThumbnailFromLocation(Location location)
		{
			if(location == null)
				return null;

			Sprite thumbnail = null;

			Place place = location as Place;

			if(place != null)
				thumbnail = place.thumbnail;

			return thumbnail;
		}
	}
}