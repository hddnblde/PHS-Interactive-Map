using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;

public class LocationDatabaseTester : MonoBehaviour
{
	[SerializeField]
	string[] wordList;

	private LocationDatabase database = null;

	private void Awake()
	{
		database = GetComponent<LocationDatabase>();
	}

	private void Start()
	{
		GetAverageResult();
	}

	private void GetAverageResult()
	{
		if(wordList == null || wordList.Length == 0)
			return;

		float total = 0f;
		float lowest = 999f;
		float highest = 0f;

		foreach(string word in wordList)
		{
			database.Search(word);
			int count = database.searchResultCount;

			total += count;
			if(count > highest)
				highest = count;
			else if(count < lowest)
				lowest = count;
		}

		float average = (total / wordList.Length);
		Debug.Log("Avg = @avg Hi = @hi Lo = @lo".Replace("@avg", average.ToString("F2")).Replace("@hi", highest.ToString()).Replace("@lo", lowest.ToString()));
	}
}