using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SearchEngine : MonoBehaviour
{
	[SerializeField]
	private string keyword = "";

	private List<SearchItem> table = SearchItem.SampleTable();

	public void Search()
	{
		Search(keyword);
	}

	public void Search(string keyword, bool includeTags = true)
	{
		if(table == null || table.Count == 0)
			return;

		char[] separator = {' '};
		string[] keywords = keyword.Split(separator);

		List<SearchItem> matches = new List<SearchItem>();

		foreach(SearchItem item in table)
		{
			if(item == null)
				continue;

			string itemKey = (item.name + "; " + item.tags).ToLower();
			
			bool matched = true;

			foreach(string key in keywords)
				matched &= itemKey.Contains(key);

			if(matched)
				matches.Add(item);
		}

		if(matches.Count > 0)
		{
			foreach(SearchItem item in matches)
				Debug.Log(item.name);
		}
		else
			Debug.Log("No match found.");

	}
}

[System.Serializable]
public class SearchItem
{
	public static List<SearchItem> SampleTable()
	{
		List<SearchItem> items = new List<SearchItem>();

		items.Add(new SearchItem("Apollo", "boat; ship; sailing; famous;"));
		items.Add(new SearchItem("Rufus", "crack; pirate; piracy; installer;"));
		items.Add(new SearchItem("Ex Machina", "plot; armor; unkillable;"));
		items.Add(new SearchItem("Dimitry", "russian; stereotype; hitman;"));
		items.Add(new SearchItem("Venom Snake", "big; boss; plot; twist;"));
		items.Add(new SearchItem("Phalinger Rex", "random; stuff; dinosaur;"));
		items.Add(new SearchItem("Hidden Blade", "software; engineer; engineering; game development;"));

		return items;
	}

	public SearchItem(string name, string tags)
	{
		m_name = name;
		m_tags = tags;
	}

	private string m_name;
	private string m_tags;

	public string name
	{
		get { return m_name; }
	}

	public string tags
	{
		get { return m_tags; }
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(SearchEngine))]
public class SearchEngineEditor : Editor
{
	private SearchEngine searchEngine = null;

	private void OnEnable()
	{
		searchEngine = target as SearchEngine;
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		if(GUILayout.Button("Search"))
		{
			searchEngine.Search();
		}
	}
}
#endif