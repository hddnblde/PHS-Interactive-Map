using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Menus;

namespace Map
{
	public class MapSystem : MonoBehaviour
	{
		[System.Serializable]
		private class ViewingBounds
		{
			public static ViewingBounds Default
			{
				get { return new ViewingBounds(); }
			}

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

			for(int i = 0; i < locationDatabase.landmarkCollectionCount; i++)
			{
				LandmarkCollection landmarkCollection = locationDatabase.GetLandmarkCollection(i);
				
				for(int j = 0; j < landmarkCollection.placeCollectionCount; j++)
				{
					PlaceCollection placeCollection = landmarkCollection.GetPlaceCollection(j);
					if(placeCollection == null)
						continue;

					bool hasRooms = placeCollection.hasRooms;

					Location placeLocation = placeCollection.GetPlaceLocation();
					CreateMarker(placeLocation, (hasRooms ? placeViewingBounds : ViewingBounds.Default));

					if(!hasRooms)
						continue;

					for(int k = 0; k < placeCollection.roomCount; k++)
					{
						Location roomLocation = placeCollection.GetRoomLocation(k);
						CreateMarker(roomLocation, roomViewingBounds);
					}					
				}
			}
		}

		private void CreateMarker(Location location, ViewingBounds viewingBounds)
		{
			if(location == null || mapMarkerPrefab == null)
				return;

			GameObject markerObject = Instantiate(mapMarkerPrefab, markerContainer) as GameObject;
			markerObject.name = location.displayedName + " Marker";
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