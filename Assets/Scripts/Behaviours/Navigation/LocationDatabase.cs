using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using Search;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace Map
{
	[DisallowMultipleComponent]
	public class LocationDatabase : MonoBehaviour
	{
		#region Fields
		public delegate void ResultEvent(int count);
		public event ResultEvent OnResult;

		[SerializeField]
		private List<LandmarkCollection> landmarkCollectionList = new List<LandmarkCollection>();

		private List<SearchKey> searchKeys = new List<SearchKey>();

		public int searchResultCount
		{
			get
			{
				if(searchKeys == null)
					return 0;
				else
					return searchKeys.Count;
			}
		}

		public int landmarkCollectionCount
		{
			get
			{
				if(landmarkCollectionList == null)
					return 0;
				else
					return landmarkCollectionList.Count;
			}
		}

		public int locationCount
		{
			get
			{
				int count = 0;
				foreach(LandmarkCollection cluster in landmarkCollectionList)
					count += cluster.placeCollectionCount;
				
				return count;
			}
		}
		#endregion


		#region Functions
		public Location GetLocationFromSearch(int index)
		{
			Landmark landmark;
			return GetLocationFromSearch(index, out landmark);
		}

		public Location GetLocationFromSearch(int index, out Landmark landmark)
		{
			landmark = null;

			if(index < 0 || searchKeys == null || searchKeys.Count == 0 || index >= searchKeys.Count)
				return null;

			SearchKey searchItem = searchKeys[index];

			if(landmarkCollectionList == null || landmarkCollectionList.Count == 0 || searchItem.landmarkIndex >= landmarkCollectionList.Count || searchItem.landmarkIndex < 0)
				return null;

			LandmarkCollection landmarkCollection = landmarkCollectionList[searchItem.landmarkIndex];

			if(landmarkCollection == null)
				return null;

			landmark = landmarkCollection.landmark;

			PlaceCollection placeCollection = landmarkCollection.GetPlaceCollection(searchItem.placeIndex);
			Location location = null;
			
			if(placeCollection == null)
				return null;

			int locationIndex = searchItem.locationIndex - 1;

			if(locationIndex == -1)
				location = placeCollection.GetPlaceLocation();
			else
				location = placeCollection.GetRoomLocation(locationIndex);

			return location;
		}

		public LandmarkCollection GetLandmarkCollection(int index)
		{
			if(index < 0 || landmarkCollectionList.Count == 0 || landmarkCollectionList == null || index >= landmarkCollectionList.Count)
				return null;
			else
				return landmarkCollectionList[index];
		}

		public void Search(string keyword)
		{
			searchKeys.Clear();
			keyword = RemoveMultipleWhiteSpaces(keyword).ToLower();

			if(string.IsNullOrEmpty(keyword) || landmarkCollectionList == null || landmarkCollectionList.Count == 0)
				goto result;

			SearchByCategory(keyword, SearchCategory.Name, false);

			if(searchResultCount == 0 || (SimilarKeysFound() && searchResultCount > 1))
				SearchByCategory(keyword, SearchCategory.SubTag, false);

			if(searchResultCount == 0)
				SearchByCategory(keyword, SearchCategory.MainTag, false);

			if(searchResultCount == 0)
				SearchByCategory(keyword, SearchCategory.Name, true);

			if(searchResultCount == 0 || (SimilarKeysFound() && searchResultCount > 1))
				SearchByCategory(keyword, SearchCategory.SubTag, true);

			if(searchResultCount == 0)
				SearchByCategory(keyword, SearchCategory.MainTag, true);

			searchKeys = searchKeys.OrderByDescending(s => s.strength).OrderBy(s => s.nearestPoint).OrderBy(s => s.landmarkIndex).ToList();

			result:
			if(OnResult != null)
				OnResult(searchResultCount);
		}
		#endregion


		#region Helpers
		private string RemoveMultipleWhiteSpaces(string s)
		{
			string[] words = s.Split(" ".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
			StringBuilder stringBuilder = new StringBuilder();

			foreach(string word in words)
				stringBuilder.Append(word + ' ');

			return stringBuilder.ToString().TrimEnd(' ');
		}

		private void SearchByCategory(string keyword, SearchCategory category, bool deepSearch)
		{
			for(int i = 0; i < landmarkCollectionList.Count; i++)
			{
				LandmarkCollection landmarkCluster = landmarkCollectionList[i];
				landmarkCluster.Search(keyword, i, searchKeys, category, deepSearch);
			}
		}

		private bool SimilarKeysFound()
		{
			bool similarKeyFound = false;
			char[] splitter = " ".ToCharArray();
			for(int i = 0; i < searchResultCount; i++)
			{
				string firstString = GetLocationFromSearch(i).displayedName.ToLower();
				for(int j = 0; j < searchResultCount; i++)
				{
					if(j == i)
						continue;

					string secondString = GetLocationFromSearch(j).displayedName.ToLower();

					string[] split1 = firstString.Split(splitter);
					string[] split2 = secondString.Split(splitter);

					List<string> common = split1.Intersect(split2).ToList();

					similarKeyFound = common.Count > 0;
					if(similarKeyFound)
						break;
				}

				if(similarKeyFound)
					break;
			}

			return similarKeyFound;
		}
		#endregion


		#if UNITY_EDITOR
		public void SetLandmarkCollectionList(List<LandmarkCollection> landmarkCollectionList)
		{
			this.landmarkCollectionList = landmarkCollectionList;
		}
		#endif
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(LocationDatabase))]
	public class LocationTableEditor : Editor
	{
		private SerializedProperty landmarkClustersProperty = null;

		private string
		landmarkPath,
		placesPath,
		roomsPath,
		searchKeyword;

		private LocationDatabase locationTable = null;

		private bool foldout = false;

		private void OnEnable()
		{
			locationTable = target as LocationDatabase;
			landmarkClustersProperty = serializedObject.FindProperty("landmarkCollectionList");
			LoadPrefs();
		}

		public override void OnInspectorGUI()
		{
			DrawInspector();
			DrawTools();
		}

		private void DrawInspector()
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

			if(Application.isPlaying)
			{
				EditorGUILayout.BeginHorizontal();

				if(GUILayout.Button("Search"))
					locationTable.Search(searchKeyword);
				
				searchKeyword = EditorGUILayout.TextField(searchKeyword);

				if(GUILayout.Button("Clear"))
				{
					searchKeyword = "";
					EditorGUILayout.TextField(searchKeyword);
				}

				EditorGUILayout.EndHorizontal();
			}
			else
			{
				EditorGUILayout.LabelField("Paths");

				EditorGUI.indentLevel++;
				landmarkPath = EditorGUILayout.TextField("Landmarks", landmarkPath);
				placesPath = EditorGUILayout.TextField("Places", placesPath);
				roomsPath = EditorGUILayout.TextField("Rooms", roomsPath);
				EditorGUI.indentLevel--;

				if(GUILayout.Button("Load Places"))
				{
					locationTable.SetLandmarkCollectionList(GetPlaces());
					serializedObject.ApplyModifiedProperties();
					serializedObject.Update();
				}
			}

			if(EditorGUI.EndChangeCheck())
				SavePrefs();

			EditorGUILayout.Space();
		}

		private void LoadPrefs()
		{
			landmarkPath = EditorPrefs.GetString("Location_LandmarkPath", "Assets/Scriptable Objects/Landmarks");
			placesPath = EditorPrefs.GetString("Location_PlacesPath", "Assets/Scriptable Objects/Places");
			roomsPath = EditorPrefs.GetString("Location_RoomsPath", "Assets/Scriptable Objects/Rooms");
			foldout = EditorPrefs.GetBool("Location_Folout", false);
			searchKeyword = EditorPrefs.GetString("Location_SearchKeyword", "");
		}

		private void SavePrefs()
		{
			EditorPrefs.SetString("Location_LandmarkPath", landmarkPath);
			EditorPrefs.SetString("Location_PlacesPath", placesPath);
			EditorPrefs.SetString("Location_RoomsPath", roomsPath);
			EditorPrefs.SetBool("Location_Folout", foldout);
			EditorPrefs.SetString("Location_SearchKeyword", searchKeyword);
		}

		private List<LandmarkCollection> GetPlaces()
		{
			List<Landmark> landmarks = GetLandmarks();

			if(landmarks == null || landmarks.Count == 0)
				return null;

			List<LandmarkCollection> landmarkClusters = new List<LandmarkCollection>();

			foreach(Landmark landmark in landmarks)
			{
				List<PlaceCollection> places = GetPlaceCluster(landmark);

				if(places != null)
				{
					LandmarkCollection landmarkCluster = new LandmarkCollection(landmark, places);
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
	
		private List<PlaceCollection> GetPlaceCluster(Landmark landmark)
		{
			string currentPath = placesPath + '\\' + landmark.name;

			if(!Directory.Exists(currentPath))
				return null;

			string[] files = Directory.GetFiles(currentPath, "*.asset");
			List<PlaceCollection> placeCluster = new List<PlaceCollection>();

			foreach(string file in files)
			{
				Place place = AssetDatabase.LoadAssetAtPath<Place>(file);
				List<Room> rooms = GetRooms(place, landmark);

				if(place != null)
					placeCluster.Add(new PlaceCollection(place, rooms));
			}

			return placeCluster;
		}

		private List<Room> GetRooms(Place place, Landmark landmark)
		{
			string currentPath = roomsPath + '\\' + landmark.name + '\\' + place.name;

			if(!Directory.Exists(currentPath))
				return null;

			string[] files = Directory.GetFiles(currentPath, "*.asset");
			List<Room> rooms = new List<Room>();

			foreach(string file in files)
			{
				Room room = AssetDatabase.LoadAssetAtPath<Room>(file);

				if(room != null)
					rooms.Add(room);
			}

			return rooms;
		}
	}
	#endif
}