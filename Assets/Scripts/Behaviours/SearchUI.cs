using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchUI : MonoBehaviour
{
	[SerializeField]
	private GameObject searchItemPrefab = null;

	[SerializeField]
	private Transform searchContent = null;

	[SerializeField]
	private ScrollRect searchView = null;

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
