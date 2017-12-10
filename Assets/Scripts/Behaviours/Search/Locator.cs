using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Map
{
	public class Locator : MonoBehaviour
	{
		#region Static Implementation
		public delegate void FinishedSearch(int matches);
		public static event FinishedSearch OnFinishedSearch;
		private static List<Location> locationMatches = new List<Location>();
		public static Location GetLocationFromSearchResult(int index)
		{
			if(locationMatches == null || locationMatches.Count == 0 || index < 0 || index >= locationMatches.Count)
				return null;
			else
				return locationMatches[index];
		}
		#endregion


		#region Fields
		private List<Location> locationTable = new List<Location>();
		private Coroutine searchRoutine = null;

		private const string TablePath = "Map/PHS Location Table";
		#endregion


		#region MonoBehaviour Implementation
		private void Awake()
		{
			LoadLocationTable();
		}
		#endregion


		#region Public Methods
		public void Search(string location, bool includeTags = true)
		{
			if(locationTable == null || locationTable.Count == 0)
				return;

			BeginSearch(location, includeTags);
		}

		public void ClearSearch()
		{
			locationMatches.Clear();
			locationMatches.Capacity = 0;
		}
		#endregion


		#region Private Methods
		private void LoadLocationTable()
		{
			LocationTable table = Resources.Load<LocationTable>(TablePath);
			locationTable = table.GetTable();
		}

		private void BeginSearch(string keyword, bool includeTags)
		{
			if(searchRoutine != null)
				StopCoroutine(searchRoutine);

			searchRoutine = StartCoroutine(SearchRoutine(keyword, includeTags));
		}

		private IEnumerator SearchRoutine(string keyword, bool includeTags)
		{
			char[] separator = {' '};
			string[] keywords = keyword.Split(separator);

			locationMatches.Clear();

			foreach(Location item in locationTable)
			{
				if(item == null)
					continue;

				bool containsName = false;
				bool containsTag = false;

				foreach(string key in keywords)
				{
					if(!containsName)
						containsName = item.name.ToLower().Contains(key.ToLower());

					if(!includeTags)
						continue;

					if(!containsTag)
						containsTag = item.tags.ToLower().Contains(key.ToLower() + ";");
				}

				if(containsName || containsTag)
					locationMatches.Add(item);

//				yield return null;
			}

			if(OnFinishedSearch != null)
				OnFinishedSearch(locationMatches.Count);

			searchRoutine = null;
			yield return null;
		}
		#endregion
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(Locator))]
	public class SearchEngineEditor : Editor
	{
		private static string searchKeyword = "";
		private Locator searchEngine = null;

		private void OnEnable()
		{
			searchEngine = target as Locator;
		}

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			searchKeyword = EditorGUILayout.TextField(searchKeyword);

			bool canSearch = Application.isPlaying && (searchKeyword.Length > 0);
			GUIStyle style = new GUIStyle(GUI.skin.button);

			if(!canSearch)
				style.normal = style.active;
			
			if(GUILayout.Button("Search", style) && canSearch)
				searchEngine.Search(searchKeyword);
		}
	}
	#endif
}