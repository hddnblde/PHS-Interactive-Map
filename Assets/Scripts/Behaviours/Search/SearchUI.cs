using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Map;

public class SearchUI : MonoBehaviour
{
	[SerializeField]
	private GameObject searchItemPrefab = null;

	[SerializeField]
	private Transform searchContent = null;

	[SerializeField]
	private ScrollRect searchView = null;

	private void OnEnable()
	{
		RegisterEvents();
	}

	private void OnDisable()
	{
		DeregisterEvents();
	}

	private void RegisterEvents()
	{
		Locator.OnFinishedSearch += OnSearch;
	}

	private void DeregisterEvents()
	{
		Locator.OnFinishedSearch -= OnSearch;
	}

	private void OnSearch(int matches)
	{
		Clear();
		if(matches == 0)
			return;

		List<string> matchList = new List<string>();

		for(int i = 0; i < matches; i++)
		{
			Location location = Locator.GetLocationFromSearchResult(i);
			matchList.Add(location.displayedName);
		}

		SetItems(matchList.ToArray());
	}

	public void Clear()
	{
		if(searchContent != null)
		{
			foreach(Transform item in searchContent)
				Destroy(item.gameObject);
		}

		ToggleView(false);
	}

	public void SetItems(string[] items)
	{
		Clear();

		if(items == null || items.Length == 0 || searchItemPrefab == null || searchContent == null)
			return;

		for(int i = 0; i < items.Length; i++)
		{
			GameObject item = Instantiate(searchItemPrefab, searchContent);
			SetItem(item, items[i]);
		}

		ToggleView();
	}

	private void SetItem(GameObject item, string name)
	{
		if(item == null)
			return;

		Text nameText = item.GetComponentInChildren<Text>();
		if(nameText != null)
			nameText.text = name;
	}

	private void ToggleView(bool visible)
	{
		if(searchView == null)
			return;
		searchView.gameObject.SetActive(visible);
	}

	private void ToggleView()
	{
		if(searchContent == null)
			return;
		ToggleView(searchContent.childCount > 0);
	}
}
