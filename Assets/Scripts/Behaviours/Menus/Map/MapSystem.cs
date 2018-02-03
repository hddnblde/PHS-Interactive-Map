using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Menus;
using Databases;

namespace Map
{
	public class MapSystem : MonoBehaviour
	{
		public delegate void SelectLayerAction(int index);
		public event SelectLayerAction OnSelectLayer;

		[System.Serializable]
		private class ViewingBounds
		{
			public static ViewingBounds Default
			{
				get { return new ViewingBounds(0.35f, 1f); }
			}

			public ViewingBounds(){}

			public ViewingBounds(float lowerLimit, float upperLimit)
			{
				m_lowerLimit = lowerLimit;
				m_upperLimit = upperLimit;
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
		private GameObject mapMarkerPrefab = null;

		[Header("Viewing Bounds")]
		[SerializeField]
		private ViewingBounds placeViewingBounds = new ViewingBounds();

		[SerializeField]
		private ViewingBounds roomViewingBounds = new ViewingBounds();

		private void Start()
		{
			CreateMarkers();
		}

		public void SelectLayer(int index)
		{
			if(OnSelectLayer != null)
				OnSelectLayer(index);
		}

		private void CreateMarkers()
		{
			for(int i = 0; i < LocationDatabase.pointsOfInterestCount; i++)
			{
				PointOfInterestGroup pointOfInterestGroup = LocationDatabase.GetPointOfInterestGroup(i);
				
				for(int j = 0; j < pointOfInterestGroup.placeCollectionCount; j++)
				{
					PlaceCollection placeCollection = pointOfInterestGroup.GetPlaceCollection(j);
					if(placeCollection == null)
						continue;

					bool hasRooms = placeCollection.hasRooms;

					Place place = placeCollection.GetPlace();
					CreateMarker(place.thumbnail, place.displayedName, place.mapName, place.displayPosition, (hasRooms ? placeViewingBounds : ViewingBounds.Default), 0);

					if(!hasRooms)
						continue;

					for(int k = 0; k < placeCollection.roomCount; k++)
					{
						Room room = placeCollection.GetRoom(k);
						CreateMarker(null, room.displayedName, room.mapName, room.displayPosition, roomViewingBounds, room.floor);
					}					
				}
			}
		}

		private void CreateMarker(Sprite thumbnail, string displayedName, string mapName, Vector3 position, ViewingBounds viewingBounds, int floor)
		{
			if(mapMarkerPrefab == null)
				return;

			GameObject markerObject = Instantiate(mapMarkerPrefab, markerContainer) as GameObject;
			markerObject.name = displayedName + " Marker";
			MapMarker marker = markerObject.GetComponent<MapMarker>();

			if(marker != null)
				marker.Set(thumbnail, mapName, position, viewingBounds.lowerLimit, viewingBounds.upperLimit, floor);
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