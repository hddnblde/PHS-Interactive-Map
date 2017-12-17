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
		private List<LandmarkCluster> landmarkClusters = new List<LandmarkCluster>();

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

			if(landmarkClusters == null || landmarkClusters.Count == 0 || searchItem.primaryIndex >= landmarkClusters.Count || searchItem.primaryIndex < 0)
				return null;

			LandmarkCluster landmarkCluster = landmarkClusters[searchItem.primaryIndex];
			landmark = landmarkCluster.landmark;
			return landmarkCluster.GetLocation(searchItem.secondaryIndex, searchItem.tertiaryIndex);
		}

		public void Search(string keyword)
		{
			searchKeys.Clear();
			keyword = RemoveMultipleWhiteSpaces(keyword).ToLower();

			if(keyword.Length == 0 || landmarkClusters == null || landmarkClusters.Count == 0)
				return;

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

			searchKeys = searchKeys.OrderByDescending(s => s.strength).OrderBy(s => s.nearestPoint).OrderBy(s => s.primaryIndex).ToList();

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
			for(int i = 0; i < landmarkClusters.Count; i++)
			{
				LandmarkCluster landmarkCluster = landmarkClusters[i];
				landmarkCluster.Search(keyword, i, searchKeys, category, deepSearch);
			}
		}

		private void PrintOutAllResults()
		{
			for(int i = 0; i < searchResultCount; i++)
				Debug.Log(GetLocationFromSearch(i).displayedName + ((" strength : @s  nearest : @n").Replace("@s", searchKeys[i].strength.ToString()).Replace("@n", searchKeys[i].nearestPoint.ToString())));
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
		public void SetLandmarkClusters(List<LandmarkCluster> landmarkClusters)
		{
			this.landmarkClusters = landmarkClusters;
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
			landmarkClustersProperty = serializedObject.FindProperty("landmarkClusters");
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
					locationTable.SetLandmarkClusters(GetPlaces());
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