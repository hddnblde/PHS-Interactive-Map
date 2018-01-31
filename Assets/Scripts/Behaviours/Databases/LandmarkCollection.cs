using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Search;

namespace Map
{
	[System.Serializable]
	public class LandmarkCollection
	{
		public LandmarkCollection(Landmark landmark, List<PlaceCollection> places)
		{
			m_landmark = landmark;
			this.places = places;
		}

		#region Fields
		[SerializeField, HideInInspector]
		private Landmark m_landmark = null;

		[Header("Cached Words")]
		[SerializeField, TextArea(1, 4)]
		private string m_names;

		[SerializeField]
		private string m_tags;

		[SerializeField, HideInInspector]
		private int cacheCount = 0;

		[SerializeField, HideInInspector]
		private List<PlaceCollection> places = new List<PlaceCollection>();

		public Landmark landmark
		{
			get { return m_landmark; }
		}

		public int placeCollectionCount
		{
			get { return cacheCount; } 
		}
		#endregion


		#region Functions
		public PlaceCollection GetPlaceCollection(int index)
		{
			if(index < 0 || places == null || places.Count == 0 || index >= places.Count)
				return null;
			else
				return places[index];
		}

		public void Search(string keyword, int primaryIndex, List<SearchKey> keys, SearchCategory category, bool deepSearch)
		{
			if(!deepSearch)
				ShallowSearch(keyword, category, primaryIndex, keys);
			else
				DeepSearch(keyword, category, primaryIndex, keys);
		}

		private void ShallowSearch(string keyword, SearchCategory category, int primaryIndex, List<SearchKey> searchKeys)
		{
			if(category == SearchCategory.MainTag)
			{
				if((' ' + m_landmark.tags.ToLower()).Contains(' ' + keyword + ';'))
					AddAllKeys(primaryIndex, searchKeys);
			}
			else
			{
				if(category == SearchCategory.Name && !m_names.Contains(keyword))
					return;
				else if(category == SearchCategory.SubTag && !m_tags.Contains(' ' + keyword + ';'))
					return;
				
				string target = "";
				string key = "";

				for(int secondaryIndex = 0; secondaryIndex < places.Count; secondaryIndex++)
				{
					PlaceCollection place = places[secondaryIndex];
					for(int tertiaryIndex = 0; tertiaryIndex < place.count; tertiaryIndex++)
					{
						GetTargetAndKeyFromLocationByCategory(place.GetLocation(tertiaryIndex), category, keyword, ref target, ref key);;

						if(target.Contains(key))
						{
							SearchKey item = StrengthenSearchKey(primaryIndex, secondaryIndex, tertiaryIndex, searchKeys);
							if(category == SearchCategory.Name)
							{
								char firstLetter = keyword.ToCharArray()[0];
								List<char> characters = new List<char>(target.ToCharArray());
								int index = characters.IndexOf(firstLetter) - 1;

								if(index > item.nearestPoint)
									item.nearestPoint = index;
							}
						}
					}
				}

			}
		}

		private void DeepSearch(string keyword, SearchCategory category, int primaryIndex, List<SearchKey> searchKeys)
		{
			foreach(string word in keyword.Split(new char[] {' '}))
			{
				int numericValue;
				if(int.TryParse(word, out numericValue))
					continue;
				
				ShallowSearch(word, category, primaryIndex, searchKeys);
			}
		}
		#endregion


		#region Helpers
		private void GetTargetAndKeyFromLocationByCategory(Location location, SearchCategory category, string keyword, ref string target, ref string key)
		{
			if(category == SearchCategory.Name)
			{
				target = location.displayedName.ToLower();
				key = keyword;
			}
			else if(category == SearchCategory.SubTag)
			{
				target = ' ' + location.tags.ToLower();
				key = ' ' + keyword + ';';
			}
		}

		private void AddAllKeys(int primaryIndex, List<SearchKey> searchKeys)
		{
			for(int secondaryIndex = 0; secondaryIndex < places.Count; secondaryIndex++)
			{
				PlaceCollection place = places[secondaryIndex];

				for(int tertiaryIndex = 0; tertiaryIndex < place.count; tertiaryIndex++)
					StrengthenSearchKey(primaryIndex, secondaryIndex, tertiaryIndex, searchKeys);
			}
		}

		private SearchKey StrengthenSearchKey(int primaryIndex, int secondaryIndex, int tertiaryIndex, List<SearchKey> searchKeys, int increment = 1)
		{
			SearchKey item = FindKey(primaryIndex, secondaryIndex, tertiaryIndex, searchKeys);

			if(item == null)
			{
				item = new SearchKey(primaryIndex, secondaryIndex, tertiaryIndex);
				searchKeys.Add(item);
			}

			item.strength += increment;
			return item;
		}

		private SearchKey FindKey(int primaryIndex, int secondaryIndex, int tertiaryIndex, List<SearchKey> searchKeys)
		{
			SearchKey key = searchKeys.Find(r => r.landmarkIndex == primaryIndex && r.placeIndex == secondaryIndex && r.locationIndex == tertiaryIndex);
			return key;
		}

		private bool KeyExists(int primaryIndex, int secondaryIndex, int tertiaryIndex, List<SearchKey> searchKeys)
		{
			return FindKey(primaryIndex, secondaryIndex, tertiaryIndex, searchKeys) != null;
		}
		#endregion

		#if UNITY_EDITOR
		public void CachePlaces()
		{
			StringBuilder nameBuilder = new StringBuilder();
			List<string> tagsList = new List<string>();
			cacheCount = 0;

			foreach(PlaceCollection place in places)
			{
				for(int i = 0; i < place.count; i++)
				{
					Location location = place.GetLocation(i);
					string name = location.displayedName.ToLower();
					nameBuilder.AppendLine(name);

					if(location.tags != null)
						AppendString(tagsList, location.tags);

					cacheCount++;
				}
			}

			m_names = nameBuilder.ToString().TrimEnd('\n');
			m_tags = BuildString(tagsList);
		}

		private void AppendString(List<string> stringList, string s)
		{
			string[] keywords = s.Split(" ".ToCharArray());
			foreach(string item in keywords)
			{
				if(!stringList.Contains(item))
					stringList.Add(item);
			}
		}

		private string BuildString(List<string> stringList)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach(string item in stringList)
				stringBuilder.Append(item + ' ');

			return stringBuilder.ToString().TrimEnd(' ').TrimStart(' ');
		}
		#endif
	}
}