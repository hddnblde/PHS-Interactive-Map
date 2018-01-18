using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;

namespace Menus
{
	public class MapMarkerSystem : MonoBehaviour
	{
		[System.Serializable]
		private class ViewingBounds
		{
			[SerializeField]
			private float m_lowerLimit = 0f;

			[SerializeField]
			private float m_upperLimit = 1f;

			public float lowerLimit
			{
				get { return m_lowerLimit; }
			}

			public float upperLimit
			{
				get { return m_upperLimit; }
			}
		}

		[SerializeField]
		private Transform markerContainer = null;

		[SerializeField]
		private LocationDatabase locationDatabase = null;

		[SerializeField]
		private GameObject mapMarkerPrefab = null;

		[Header("Viewing Bounds")]
		[SerializeField]
		private ViewingBounds placeViewingBounds = new ViewingBounds();

		[SerializeField]
		private ViewingBounds roomViewingBounds = new ViewingBounds();

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
				ViewingBounds viewingBounds = (isRoom ? roomViewingBounds : placeViewingBounds);

				CreateMarker(location, viewingBounds);
			}
		}

		private void CreateMarker(Location location, ViewingBounds viewingBounds)
		{
			if(location == null || mapMarkerPrefab == null)
				return;

			GameObject markerObject = Instantiate(mapMarkerPrefab, markerContainer) as GameObject;
			MapMarker marker = markerObject.GetComponent<MapMarker>();

			if(marker != null)
				marker.Set(GetThumbnailFromLocation(location), location.displayedName, location.position, viewingBounds.lowerLimit, viewingBounds.upperLimit);
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