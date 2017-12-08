using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LocationTable : MonoBehaviour
{
	#region Fields
	public delegate void FinishedSearch(int matches);
	public event FinishedSearch OnFinishedSearch;

	private List<Location> database = new List<Location>();
	private List<Location> matches = new List<Location>();
	private Coroutine searchRoutine = null;
	#endregion


	#region MonoBehaviour Implementation
	private void Awake()
	{
		LoadAllLocations();
	}
	#endregion


	#region Public Methods
	public void Search(string keyword, bool includeTags = true)
	{
		if(database == null || database.Count == 0)
			return;

		BeginSearch(keyword, includeTags);
	}

	public void ClearSearch()
	{
		matches.Clear();
		matches.Capacity = 0;
	}

	public Location GetLocationFromSearchResult(int index)
	{
		if(matches == null || matches.Count == 0 || index < 0 || index >= matches.Count)
			return null;
		else
			return matches[index];
	}
	#endregion


	#region Private Methods
	private void LoadAllLocations()
	{
		
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

		foreach(Location item in database)
		{
			if(item == null)
				continue;

			bool containsName = false;
			bool containsTag = false;

			foreach(string key in keywords)
			{
				if(!containsName)
					containsName = item.name.Contains(key);

				if(!includeTags)
					continue;

				if(!containsTag)
					containsTag = item.tags.Contains(key + ";");
			}

			if(containsName || containsTag)
				matches.Add(item);

			yield return null;
		}

		if(OnFinishedSearch != null)
			OnFinishedSearch(matches.Count);

		searchRoutine = null;
	}
	#endregion
}

#if UNITY_EDITOR
[CustomEditor(typeof(LocationTable))]
public class SearchEngineEditor : Editor
{
	private static string searchKeyword = "";
	private LocationTable searchEngine = null;

	private void OnEnable()
	{
		searchEngine = target as LocationTable;
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