using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using Search;
using Map;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace Databases
{
	[DisallowMultipleComponent]
	public class LocationDatabase : MonoBehaviour
	{
		#region Serialized Fields
		[SerializeField]
		private List<PointOfInterestGroup> pointsOfInterestGroup = new List<PointOfInterestGroup>();
		#endregion


		#region Hidden Fields
		private static LocationDatabase instance = null;

		public delegate void ResultEvent(int count);
		public static event ResultEvent OnResult;
		private List<SearchKey> searchKeys = new List<SearchKey>();
		#endregion


		#region Properties
		public static int searchResultCount
		{
			get
			{
				if(instance == null)
					return 0;
				else
					return instance.Internal_searchResultCount;
			}
		}

		public static int pointsOfInterestCount
		{
			get
			{
				if(instance == null)
					return 0;
				else
					return instance.Internal_pointsOfInterestCount;
			}
		}

		private int Internal_searchResultCount
		{
			get
			{
				if(searchKeys == null)
					return 0;
				else
					return searchKeys.Count;
			}
		}

		private int Internal_pointsOfInterestCount
		{
			get
			{
				if(pointsOfInterestGroup == null)
					return 0;
				else
					return pointsOfInterestGroup.Count;
			}
		}
		#endregion


		#region MonoBehaviour Implementation
		private void Awake()
		{
			InitializeSingleton();
		}

		private void OnDisable()
		{
			UninitializeSingleton();
		}
		#endregion


		#region Initializers
		private void InitializeSingleton()
		{
			if(instance == null)
				instance = this;
			else
			{
				Destroy(gameObject);
				return;
			}
		}

		private void UninitializeSingleton()
		{
			if(instance == this)
				instance = null;
		}
		#endregion


		#region Actions
		public static Location GetLocationFromSearch(int index)
		{
			if(instance == null)
				return null;
			else
				return instance.Internal_GetLocationFromSearch(index);
		}

		public static Location GetLocationFromSearch(int index, out PointOfInterest pointOfInterest)
		{
			pointOfInterest = null;
			if(instance == null)
				return null;
			else
				return instance.Internal_GetLocationFromSearch(index, out pointOfInterest);
		}

		public static PointOfInterestGroup GetPointOfInterestGroup(int index)
		{
			if(instance == null)
				return null;
			else
				return instance.Internal_GetPointOfInterestGroup(index);
		}

		public static void Search(string keyword)
		{
			if(instance != null)
				instance.Internal_Search(keyword);
		}

		public static Place[] GetAllBuildings()
		{
			if(instance == null)
				return null;
			else
				return instance.Internal_GetAllBuildings();
		}

		public static Place GetBuildingFromRoom(Room room)
		{
			if(instance == null)
				return null;
			else
				return instance.Internal_GetBuildingFromRoom(room);
		}

		private Place Internal_GetBuildingFromRoom(Room room)
		{
			Place building = null;

			for(int i = 0; i < pointsOfInterestGroup.Count; i++)
			{
				PointOfInterestGroup pointOfInterest = pointsOfInterestGroup[i];

				for(int j = 0; j < pointOfInterest.placeCollectionCount; j++)
				{
					PlaceCollection placeCollection = pointOfInterest.GetPlaceCollection(j);
					
					if(placeCollection == null)
						continue;

					if(placeCollection.HasRoom(room))
					{
						building = placeCollection.GetPlace();
						break;
					}
				}
			}

			return building;
		}

		private Location Internal_GetLocationFromSearch(int index)
		{
			PointOfInterest pointOfInterest;
			return GetLocationFromSearch(index, out pointOfInterest);
		}

		private Location Internal_GetLocationFromSearch(int index, out PointOfInterest pointOfInterest)
		{
			pointOfInterest = null;

			if(index < 0 || searchKeys == null || searchKeys.Count == 0 || index >= searchKeys.Count)
				return null;

			SearchKey searchItem = searchKeys[index];

			if(pointsOfInterestGroup == null || pointsOfInterestGroup.Count == 0 || searchItem.poiIndex >= pointsOfInterestGroup.Count || searchItem.poiIndex < 0)
				return null;

			PointOfInterestGroup pointOfInterestGroup = pointsOfInterestGroup[searchItem.poiIndex];

			if(pointOfInterestGroup == null)
				return null;

			pointOfInterest = pointOfInterestGroup.pointOfInterest;

			PlaceCollection placeCollection = pointOfInterestGroup.GetPlaceCollection(searchItem.placeIndex);
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

		private PointOfInterestGroup Internal_GetPointOfInterestGroup(int index)
		{
			if(index < 0 || pointsOfInterestGroup.Count == 0 || pointsOfInterestGroup == null || index >= pointsOfInterestGroup.Count)
				return null;
			else
				return pointsOfInterestGroup[index];
		}

		private void Internal_Search(string keyword)
		{
			searchKeys.Clear();
			keyword = RemoveMultipleWhiteSpaces(keyword).ToLower();

			if(string.IsNullOrEmpty(keyword) || pointsOfInterestGroup == null || pointsOfInterestGroup.Count == 0)
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

			searchKeys = searchKeys.OrderByDescending(s => s.strength).OrderBy(s => s.nearestPoint).OrderBy(s => s.poiIndex).ToList();

			result:
			if(OnResult != null)
				OnResult(searchResultCount);
		}

		private Place[] Internal_GetAllBuildings()
		{
			List<Place> buildings = new List<Place>();

			for(int i = 0; i < pointsOfInterestGroup.Count; i++)
			{
				PointOfInterestGroup pointOfInterest = pointsOfInterestGroup[i];

				for(int j = 0; j < pointOfInterest.placeCollectionCount; j++)
				{
					PlaceCollection placeCollection = pointOfInterest.GetPlaceCollection(j);
					
					if(placeCollection == null)
						continue;

					Place place = placeCollection.GetPlace();

					if(place != null)
						buildings.Add(place);
				}
			}

			return buildings.ToArray();
		}
		#endregion


		#region Helpers
		public int GetLocationCount()
		{
			int count = 0;
			foreach(PointOfInterestGroup cluster in pointsOfInterestGroup)
				count += cluster.placeCollectionCount;
				
			return count;
		}
		
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
			for(int i = 0; i < pointsOfInterestGroup.Count; i++)
			{
				PointOfInterestGroup pointOfInterestGroup = pointsOfInterestGroup[i];
				pointOfInterestGroup.Search(keyword, i, searchKeys, category, deepSearch);
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
		public void SetPointsOfInterestGroup(List<PointOfInterestGroup> pointsOfInterestGroup)
		{
			this.pointsOfInterestGroup = pointsOfInterestGroup;
		}
		#endif
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(LocationDatabase))]
	public class LocationTableEditor : Editor
	{
		private SerializedProperty pointsOfInterestProperty = null;

		private string
		pointsOfInterestPath,
		placesPath,
		roomsPath,
		searchKeyword;

		private LocationDatabase locationDatabase = null;

		private bool foldout = false;

		private void OnEnable()
		{
			locationDatabase = target as LocationDatabase;
			pointsOfInterestProperty = serializedObject.FindProperty("pointsOfInterestGroup");
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

			if(pointsOfInterestProperty.arraySize == 0)
			{
				EditorGUILayout.HelpBox("Table is empty. Please load data from assets. Make sure that all paths below are correct.", MessageType.Warning);
				return;
			}

			foldout = EditorGUILayout.Foldout(foldout, "Points of Interest");

			if(foldout)
			{
				EditorGUI.indentLevel++;
				for(int i = 0; i < pointsOfInterestProperty.arraySize; i++)
				{
					SerializedProperty pointsOfInterestGroup = pointsOfInterestProperty.GetArrayElementAtIndex(i);
					SerializedProperty pointOfInterest = pointsOfInterestGroup.FindPropertyRelative("m_pointOfInterest");
					string displayedName = pointOfInterest.objectReferenceValue.name;
					EditorGUILayout.PropertyField(pointsOfInterestGroup, new GUIContent(displayedName), true);
				}
				EditorGUI.indentLevel--;
			}

			if(EditorGUI.EndChangeCheck())
				SavePrefs();

			EditorGUILayout.HelpBox("Location Count : " + locationDatabase.GetLocationCount(), MessageType.Info);
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
					LocationDatabase.Search(searchKeyword);
				
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
				pointsOfInterestPath = EditorGUILayout.TextField("Points of Interest", pointsOfInterestPath);
				placesPath = EditorGUILayout.TextField("Places", placesPath);
				roomsPath = EditorGUILayout.TextField("Rooms", roomsPath);
				EditorGUI.indentLevel--;

				if(GUILayout.Button("Load Places"))
				{
					locationDatabase.SetPointsOfInterestGroup(GetPlaces());
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
			pointsOfInterestPath = EditorPrefs.GetString("Location_LandmarkPath", "Assets/Scriptable Objects/Points of Interest");
			placesPath = EditorPrefs.GetString("Location_PlacesPath", "Assets/Scriptable Objects/Places");
			roomsPath = EditorPrefs.GetString("Location_RoomsPath", "Assets/Scriptable Objects/Rooms");
			foldout = EditorPrefs.GetBool("Location_Folout", false);
			searchKeyword = EditorPrefs.GetString("Location_SearchKeyword", "");
		}

		private void SavePrefs()
		{
			EditorPrefs.SetString("Location_LandmarkPath", pointsOfInterestPath);
			EditorPrefs.SetString("Location_PlacesPath", placesPath);
			EditorPrefs.SetString("Location_RoomsPath", roomsPath);
			EditorPrefs.SetBool("Location_Folout", foldout);
			EditorPrefs.SetString("Location_SearchKeyword", searchKeyword);
		}

		private List<PointOfInterestGroup> GetPlaces()
		{
			List<PointOfInterest> pointsOfInterest = GetPointsOfInterest();

			if(pointsOfInterest == null || pointsOfInterest.Count == 0)
				return null;

			List<PointOfInterestGroup> pointsOfInterestGroup = new List<PointOfInterestGroup>();

			foreach(PointOfInterest pointOfInterest in pointsOfInterest)
			{
				List<PlaceCollection> places = GetPlaceCluster(pointOfInterest);

				if(places != null)
				{
					PointOfInterestGroup pointOfInterestGroup = new PointOfInterestGroup(pointOfInterest, places);
					pointOfInterestGroup.CachePlaces();
					pointsOfInterestGroup.Add(pointOfInterestGroup);
				}
			}
	
			return pointsOfInterestGroup;
		}

		private List<PointOfInterest> GetPointsOfInterest()
		{
			if(!Directory.Exists(pointsOfInterestPath))
				return null;

			string[] directories = Directory.GetDirectories(pointsOfInterestPath);
			List<PointOfInterest> pointsOfInterest = new List<PointOfInterest>();

			foreach(string directory in directories)
			{
				string[] files = Directory.GetFiles(directory, "*.asset");

				foreach(string file in files)
				{
					PointOfInterest pointOfInterest = AssetDatabase.LoadAssetAtPath<PointOfInterest>(file);

					if(pointOfInterest != null)
						pointsOfInterest.Add(pointOfInterest);
				}
			}

			return pointsOfInterest;
		}
	
		private List<PlaceCollection> GetPlaceCluster(PointOfInterest pointOfInterest)
		{
			string currentPath = placesPath + '\\' + pointOfInterest.name;

			if(!Directory.Exists(currentPath))
				return null;

			string[] files = Directory.GetFiles(currentPath, "*.asset");
			List<PlaceCollection> placeCluster = new List<PlaceCollection>();

			foreach(string file in files)
			{
				Place place = AssetDatabase.LoadAssetAtPath<Place>(file);
				List<Room> rooms = GetRooms(place, pointOfInterest);

				if(place != null)
					placeCluster.Add(new PlaceCollection(place, rooms));
			}

			return placeCluster;
		}

		private List<Room> GetRooms(Place place, PointOfInterest pointOfInterest)
		{
			string currentPath = roomsPath + '\\' + pointOfInterest.name + '\\' + place.name;

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