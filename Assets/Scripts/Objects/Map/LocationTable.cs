using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using System.Linq;
#endif

namespace Map
{
	[CreateAssetMenu(menuName = "Map/Location Table", order = 3, fileName = "Location Table")]
	public class LocationTable : ScriptableObject
	{
		#region Sub classes
		[System.Serializable]
		private class LandmarkCluster
		{
			public LandmarkCluster(Landmark landmark, List<PlaceCluster> places)
			{
				m_landmark = landmark;
				m_places = places;
			}

			[SerializeField]
			private Landmark m_landmark = null;

			[SerializeField]
			private List<PlaceCluster> m_places = new List<PlaceCluster>();

			private bool m_cachedTags = false;

			public Landmark landmark
			{
				get { return m_landmark; }
			}

			public bool cachedTags
			{
				get { return m_cachedTags; }
			}

			private string tags;

			public void CacheTags()
			{
				m_cachedTags = false;
				tags = m_landmark.tags.Replace(" ", "");

				if(m_places == null || m_places.Count == 0)
					return;

				foreach(PlaceCluster place in m_places)
				{
					foreach(string tag in place.GetTags())
					{
						if(!tags.Contains(tag))
							tags += tag + ";";
					}
				}

				m_cachedTags = true;
			}

			public Location GetLocation(int index)
			{
				if(index < 0 || m_places == null || m_places.Count == 0 || index >= m_places.Count)
					return null;

				return m_places[index].GetLocation(index);
			}

			public bool HasTag(string keyword)
			{
				if(!cachedTags)
					return false;
				else
					return tags.Contains(keyword);
			}
		
			public bool Contains(string keyword)
			{
				
				return false;
			}
		}

		[System.Serializable]
		private class PlaceCluster
		{
			public PlaceCluster(Place place, List<Room> rooms)
			{
				m_place = place;
				m_rooms = rooms;
			}

			[SerializeField]
			private Place m_place = null;

			[SerializeField]
			private List<Room> m_rooms = new List<Room>();

			private bool HasRooms()
			{
				return m_rooms != null && m_rooms.Count > 0;
			}

			public Location GetLocation(int index)
			{
				if(index < 0)
					return null;
				else if(!HasRooms())
				{
					if(index != null)
						return null;
					else
						return m_place as Location;
				}
				else
				{
					if((index - 1) >= m_rooms.Count)
						return null;
					else
						return m_rooms[index - 1] as Location;
				}
			}
		
			public string[] GetTags()
			{
				List<string> tags = new List<string>();

				char[] separator = {';'};
				foreach(string tag in m_place.tags.Replace(" ", "").Split(separator))
				{
					if(!tags.Contains(tag))
						tags.Add(tag);
				}

				if(m_rooms == null || m_rooms.Count == 0)
					return tags.ToArray();

				foreach(Room room in m_rooms)
				{
					foreach(string tag in room.tags.Replace(" ", "").Split(separator))
					{
						if(!tags.Contains(tag))
							tags.Add(tag);
					}
				}

				return tags.ToArray();
			}
		}

		private class SearchItem
		{
			public SearchItem()
			{
				Constructor(0, 0, 0);
			}

			public SearchItem(int mainIndex, int subIndex)
			{
				Constructor(mainIndex, subIndex, 0);
			}

			public SearchItem(int mainIndex, int subIndex, int strength)
			{
				Constructor(mainIndex, subIndex, strength);
			}

			private void Constructor(int mainIndex, int subIndex, int strength)
			{
				m_mainIndex = mainIndex;
				m_subIndex = subIndex;
				m_strength = strength;
			}

			private int m_mainIndex = -1;
			private int m_subIndex = -1;
			private int m_strength = 0;

			public int mainIndex
			{
				get { return m_mainIndex; }
			}

			public int subIndex
			{
				get { return m_subIndex; }
			}

			public int strength
			{
				get { return m_strength; }
			}
		}
		#endregion
	
		[SerializeField]
		private List<LandmarkCluster> m_landmarkClusters = new List<LandmarkCluster>();

		private List<SearchItem> m_searchResults = new List<SearchItem>();

		public int searchResultCount
		{
			get
			{
				if(m_searchResults == null)
					return 0;
				else
					return m_searchResults.Count;
			}
		}

		public void Search(string keyword)
		{
			m_searchResults.Clear();

			if(m_landmarkClusters == null || m_landmarkClusters.Count == 0)
				return;

			foreach(LandmarkCluster landmarkCluster in m_landmarkClusters)
			{
//				landmarkCluster.
			}
		}

		public Location GetLocationFromSearch(int index)
		{
			if(index < 0 || m_searchResults == null || m_searchResults.Count == 0 || index >= m_searchResults.Count)
				return null;

			SearchItem searchItem = m_searchResults[index];

			if(m_landmarkClusters == null || m_landmarkClusters.Count == 0 || searchItem.mainIndex >= m_landmarkClusters.Count || searchItem.mainIndex < 0)
				return null;

			LandmarkCluster landmark = m_landmarkClusters[searchItem.mainIndex];
			return landmark.GetLocation(searchItem.subIndex);
		}
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(LocationTable))]
	public class LocationTableEditor : Editor
	{
//		private SerializedProperty landmarkClusterProperty = null;
//
//		private void OnEnable()
//		{
//			landmarkClusterProperty = serializedObject.FindProperty("m_landmarkCluster");
//		}
//
//		public override void OnInspectorGUI()
//		{
//			DrawLandmarkCluster(landmarkClusterProperty);
//		}
//
//		private void DrawLandmarkCluster(SerializedProperty property, bool show)
//		{
//			EditorGUILayout.LabelField(property.displayName);
//		}
	}
	#endif
}