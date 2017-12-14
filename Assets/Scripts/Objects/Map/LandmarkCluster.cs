using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace Map
{
	[System.Serializable]
	public class LandmarkCluster
	{
		#region Fields
		public LandmarkCluster(Landmark landmark, List<PlaceCluster> places)
		{
			m_landmark = landmark;
			this.places = places;
		}

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
		private List<PlaceCluster> places = new List<PlaceCluster>();

		private enum Category
		{
			Name,
			MainTag,
			SubTag
		}

		public Landmark landmark
		{
			get { return m_landmark; }
		}

		public int count
		{
			get { return cacheCount; } 
		}
		#endregion


		#region Functions
		public Location GetLocation(int secondaryIndex, int tertiaryIndex)
		{
			if(secondaryIndex < 0 || places == null || places.Count == 0 || secondaryIndex >= places.Count)
				return null;

			return places[secondaryIndex].GetLocation(tertiaryIndex);
		}
			
		public void Search(string keyword, int primaryIndex, List<SearchKey> searchKeys, bool deepSearch)
		{
			if(!deepSearch)
			{
				ShallowSearch(keyword, Category.Name, primaryIndex, searchKeys);

				if(SimilarKeysFound(searchKeys))
					ShallowSearch(keyword, Category.SubTag, primaryIndex, searchKeys);

				if(searchKeys.Count > 0)
					return;

				ShallowSearch(keyword, Category.MainTag, primaryIndex, searchKeys);

				if(searchKeys.Count > 0)
					return;
			}
			else
			{
				DeepSearch(keyword, Category.Name, primaryIndex, searchKeys);

				if(SimilarKeysFound(searchKeys))
					DeepSearch(keyword, Category.SubTag, primaryIndex, searchKeys);

				if(searchKeys.Count > 0)
					return;

				DeepSearch(keyword, Category.MainTag, primaryIndex, searchKeys);

				if(searchKeys.Count > 0)
					return;
			}
		}

		private void ShallowSearch(string keyword, Category category, int primaryIndex, List<SearchKey> searchKeys)
		{
			if(category == Category.MainTag)
			{
				if(m_tags.ToLower().Contains(' ' + keyword +';'))
					AddAllKeys(primaryIndex, searchKeys);
			}
			else
			{
				string target = "";
				string key = "";

				for(int secondaryIndex = 0; secondaryIndex < places.Count; secondaryIndex++)
				{
					PlaceCluster place = places[secondaryIndex];
					for(int tertiaryIndex = 0; tertiaryIndex < place.count; tertiaryIndex++)
					{
						GetTargetAndKeyFromLocationByCategory(place.GetLocation(tertiaryIndex), category, keyword, ref target, ref key);

						if(target.Contains(key))
						{
							SearchKey item = StrengthenSearchKey(primaryIndex, secondaryIndex, tertiaryIndex, searchKeys);

							if(category == Category.Name)
							{
								char firstLetter = keyword.ToCharArray()[0];
								List<char> characters = new List<char>(target.ToCharArray());
								item.nearestPoint = characters.IndexOf(firstLetter);
							}
						}
					}
				}

			}
		}

		private void DeepSearch(string keyword, Category category, int primaryIndex, List<SearchKey> searchKeys)
		{
			foreach(string word in keyword.Split(new char[] {' '}))
				ShallowSearch(word, category, primaryIndex, searchKeys);
		}
		#endregion


		#region Helpers
		private void GetTargetAndKeyFromLocationByCategory(Location location, Category category, string keyword, ref string target, ref string key)
		{
			if(category == Category.Name)
			{
				target = location.displayedName.ToLower();
				key = keyword;
			}
			else if(category == Category.SubTag)
			{
				target = ' ' + location.tags.ToLower();
				key = ' ' + keyword + ';';
			}
		}

		private void AddAllKeys(int primaryIndex, List<SearchKey> searchKeys)
		{
			for(int secondaryIndex = 0; secondaryIndex < places.Count; secondaryIndex++)
			{
				PlaceCluster place = places[secondaryIndex];

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
			SearchKey key = searchKeys.Find(r => r.primaryIndex == primaryIndex && r.secondaryIndex == secondaryIndex && r.tertiaryIndex == tertiaryIndex);
			return key;
		}

		private bool KeyExists(int primaryIndex, int secondaryIndex, int tertiaryIndex, List<SearchKey> searchKeys)
		{
			return FindKey(primaryIndex, secondaryIndex, tertiaryIndex, searchKeys) != null;
		}

		private bool SimilarKeysFound(List<SearchKey> searchKeys)
		{
			return true;
		}
		#endregion

		#if UNITY_EDITOR
		public void CachePlaces()
		{
			StringBuilder nameBuilder = new StringBuilder();
			List<string> tagsList = new List<string>();
			cacheCount = 0;

			foreach(PlaceCluster place in places)
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