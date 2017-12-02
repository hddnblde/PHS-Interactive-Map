using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using System.Text.RegularExpressions;

public static class NavigationUtility
{
	private static List<string> locationList = new List<string>();
//	static Dictionary<string, Vector3> locationDictionary = new Dictionary<string, Vector3>();

	public static void CacheLocations(Transform locationContainer)
	{
		if(locationContainer == null)
			return;

		foreach(Transform location in locationContainer)
			locationList.Add(location.name);
	}

	public static string[] FindLocation(string location)
	{
		if(locationList == null || locationList.Count == 0)
			return null;

		location = location.ToLower();
		return locationList.Where(l => l.ToLower().Contains(location)).OrderBy(x => x).ToArray();
	}
}
