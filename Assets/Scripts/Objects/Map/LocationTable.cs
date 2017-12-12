using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

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

		#region Fields
		[SerializeField]
		private List<LandmarkCluster> landmarkClusters = new List<LandmarkCluster>();

		private List<SearchItem> searchResults = new List<SearchItem>();

		public int searchResultCount
		{
			get
			{
				if(searchResults == null)
					return 0;
				else
					return searchResults.Count;
			}
		}

		public int locationCount
		{
			get
			{
				int count = 0;
				foreach(LandmarkCluster cluster in landmarkClusters)
					count += cluster.count;
				
				return count;
			}
		}
		#endregion


		#region Functions
		public void SetLandmarkClusters(List<LandmarkCluster> landmarkClusters)
		{
			this.landmarkClusters = landmarkClusters;
		}

		public void Search(string keyword)
		{
			searchResults.Clear();

			if(landmarkClusters == null || landmarkClusters.Count == 0)
				return;

			string[] keywords = keyword.Replace(' ', ',').Split(new char[] {','});

			foreach(string word in keywords)
			{
				
			}

			foreach(LandmarkCluster landmarkCluster in landmarkClusters)
			{
//				landmarkCluster.
			}
		}

		public Location GetLocationFromSearch(int index)
		{
			if(index < 0 || searchResults == null || searchResults.Count == 0 || index >= searchResults.Count)
				return null;

			SearchItem searchItem = searchResults[index];

			if(landmarkClusters == null || landmarkClusters.Count == 0 || searchItem.mainIndex >= landmarkClusters.Count || searchItem.mainIndex < 0)
				return null;

			LandmarkCluster landmark = landmarkClusters[searchItem.mainIndex];
			return landmark.GetLocation(searchItem.subIndex);
		}

		private void SearchInNames(string keyword)
		{
			
		}

		private void SearchInTags(string keyword)
		{
			
		}
		#endregion
	}

	[System.Serializable]
	public class LandmarkCluster
	{
		#region Fields
		public LandmarkCluster(Landmark landmark, List<PlaceCluster> places)
		{
			m_landmark = landmark;
			this.places = places;
		}

		[SerializeField, HideInInspector]
		private Landmark m_landmark = null;

		[SerializeField, Multiline]
		private string cachedPlaces;

		[SerializeField, HideInInspector]
		private int cacheCount = 0;

		[SerializeField, HideInInspector]
		private List<PlaceCluster> places = new List<PlaceCluster>();

		public Landmark landmark
		{
			get { return m_landmark; }
		}

		public int count
		{
			get { return cacheCount; } 
		}
		#endregion


		#region Functions
		public Location GetLocation(int index)
		{
			if(index < 0 || places == null || places.Count == 0 || index >= places.Count)
				return null;

			return places[index].GetLocation(index);
		}

		public void CachePlaces()
		{
			StringBuilder stringBuilder = new StringBuilder();
			cacheCount = 0;

			foreach(PlaceCluster place in places)
			{
				for(int i = 0; i < place.count; i++)
				{
					string name = place.GetLocation(i).displayedName;
					stringBuilder.AppendLine(name);
					cacheCount++;
				}
			}

			cachedPlaces = stringBuilder.ToString().TrimEnd('\n');
		}
		#endregion
	}

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

	#if UNITY_EDITOR
	[CustomEditor(typeof(LocationTable))]
	public class LocationTableEditor : Editor
	{
		private SerializedProperty landmarkClustersProperty = null;

		private string
		landmarkPath,
		placesPath,
		roomsPath;

		private LocationTable locationTable = null;

		private bool foldout = false;

		private void OnEnable()
		{
			locationTable = target as LocationTable;
			landmarkClustersProperty = serializedObject.FindProperty("landmarkClusters");
			LoadPrefs();
		}

		public override void OnInspectorGUI()
		{
			DrawLandmarks();
			DrawTools();
		}

		private void DrawLandmarks()
		{
			EditorGUI.BeginChangeCheck();

			if(landmarkClustersProperty.arraySize == 0)
			{
				EditorGUILayout.HelpBox("Table is empty. Please load data from assets. Make sure that all paths below are correct.", MessageType.Warning);
				return;
			}

			foldout = EditorGUILayout.Foldout(foldout, "Landmarks");

			if(foldout)
			{
				EditorGUI.indentLevel++;
				for(int i = 0; i < landmarkClustersProperty.arraySize; i++)
				{
					SerializedProperty landmarkCluster = landmarkClustersProperty.GetArrayElementAtIndex(i);
					SerializedProperty landmark = landmarkCluster.FindPropertyRelative("m_landmark");
					string displayedName = landmark.objectReferenceValue.name;
					EditorGUILayout.PropertyField(landmarkCluster, new GUIContent(displayedName), true);
				}
				EditorGUI.indentLevel--;
			}

			if(EditorGUI.EndChangeCheck())
				SavePrefs();
			
			EditorGUILayout.HelpBox("Location Count : " + locationTable.locationCount, MessageType.Info);
		}

		private void DrawTools()
		{
			EditorGUILayout.Space();
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.LabelField("Tools", EditorStyles.boldLabel);
			EditorGUILayout.LabelField("Paths");

			EditorGUI.indentLevel++;
			landmarkPath = EditorGUILayout.TextField("Landmarks", landmarkPath);
			placesPath = EditorGUILayout.TextField("Places", placesPath);
			roomsPath = EditorGUILayout.TextField("Rooms", roomsPath);
			EditorGUI.indentLevel--;

			if(EditorGUI.EndChangeCheck())
				SavePrefs();

			EditorGUILayout.Space();

			if(GUILayout.Button("Load Places"))
			{
				locationTable.SetLandmarkClusters(GetPlaces());
				serializedObject.ApplyModifiedProperties();
				serializedObject.Update();
			}
		}

		private void LoadPrefs()
		{
			landmarkPath = EditorPrefs.GetString("Location_LandmarkPath", "Assets/Scriptable Objects/Landmarks");
			placesPath = EditorPrefs.GetString("Location_PlacesPath", "Assets/Scriptable Objects/Places");
			roomsPath = EditorPrefs.GetString("Location_RoomsPath", "Assets/Scriptable Objects/Rooms");
			foldout = EditorPrefs.GetBool("Location_Folout", false);
		}

		private void SavePrefs()
		{
			EditorPrefs.SetString("Location_LandmarkPath", landmarkPath);
			EditorPrefs.SetString("Location_PlacesPath", placesPath);
			EditorPrefs.SetString("Location_RoomsPath", roomsPath);
			EditorPrefs.SetBool("Location_Folout", foldout);
		}

		private List<LandmarkCluster> GetPlaces()
		{
			List<Landmark> landmarks = GetLandmarks();

			if(landmarks == null || landmarks.Count == 0)
				return null;

			List<LandmarkCluster> landmarkClusters = new List<LandmarkCluster>();

			foreach(Landmark landmark in landmarks)
			{
				List<PlaceCluster> places = GetPlaceCluster(landmark);

				if(places != null)
				{
					LandmarkCluster landmarkCluster = new LandmarkCluster(landmark, places);
					landmarkCluster.CachePlaces();
					landmarkClusters.Add(landmarkCluster);
				}
			}
	
			return landmarkClusters;
		}

		private List<Landmark> GetLandmarks()
		{
			if(!Directory.Exists(landmarkPath))
				return null;

			string[] directories = Directory.GetDirectories(landmarkPath);
			List<Landmark> landmarks = new List<Landmark>();

			foreach(string directory in directories)
			{
				string[] files = Directory.GetFiles(directory, "*.asset");

				foreach(string file in files)
				{
					Landmark landmark = AssetDatabase.LoadAssetAtPath<Landmark>(file);

					if(landmark != null)
						landmarks.Add(landmark);
				}
			}

			return landmarks;
		}
	
		private List<PlaceCluster> GetPlaceCluster(Landmark landmark)
		{
			string currentPath = placesPath + '\\' + landmark.name;

			if(!Directory.Exists(currentPath))
				return null;

			string[] files = Directory.GetFiles(currentPath, "*.asset");
			List<PlaceCluster> placeCluster = new List<PlaceCluster>();

			foreach(string file in files)
			{
				Place place = AssetDatabase.LoadAssetAtPath<Place>(file);

				if(place != null)
					placeCluster.Add(new PlaceCluster(place, null));
			}

			return placeCluster;
		}
	}
	#endif
}