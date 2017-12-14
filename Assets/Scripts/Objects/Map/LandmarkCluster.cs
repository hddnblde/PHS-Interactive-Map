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

		public void Search(int primaryIndex, string keyword, List<SearchItem> results)
		{
			if(ContainsInMainTags(keyword))
			{
				for(int i = 0; i < places.Count; i++)
				{
					PlaceCluster place = places[i];

					for(int j = 0; j < place.count; j++)
					{
						SearchItem item = new SearchItem(primaryIndex, i, j, 1);
						results.Add(item);
					}
				}
			}

			FindTargetInKeyword(keyword, primaryIndex, results, true);
			FindTargetInKeyword(keyword, primaryIndex, results, false);
		}


		#region Helpers
		private void FindTargetInKeyword(string keyword, int primaryIndex, List<SearchItem> results, bool isTag)
		{
			bool contains = (isTag ? ContainsInSubTags(ref keyword) : ContainsInNames(ref keyword));

			if(contains)
			{
				foreach(string word in keyword.Split(" ".ToCharArray()))
				{
					for(int secondaryIndex = 0; secondaryIndex < places.Count; secondaryIndex++)
					{
						PlaceCluster place = places[secondaryIndex];

						for(int tertiaryIndex = 0; tertiaryIndex < place.count; tertiaryIndex++)
						{
							Location location = place.GetLocation(tertiaryIndex);

							string target = (isTag ? (' ' + location.tags) : location.displayedName).ToLower();
							string key = word;

							if(isTag)
								key = " " + key + ";";

							if(target.Contains(key))
							{
								SearchItem item = results.Find(r => r.primaryIndex == primaryIndex && r.secondaryIndex == secondaryIndex && r.tertiaryIndex == tertiaryIndex);

								if(item == null)
								{
									item = new SearchItem(primaryIndex, secondaryIndex, tertiaryIndex);
									results.Add(item);
								}

								item.strength++;

								if(!isTag)
								{
									char firstLetter = word.ToCharArray()[0];
									List<char> characters = new List<char>(target.ToCharArray());
									item.nearestPoint = characters.IndexOf(firstLetter);
								}
							}
						}
					}
				}
			}
		}

		private bool ContainsInNames(ref string keyword)
		{
			bool foundName = false;
			StringBuilder matchBuilder = new StringBuilder();

			foreach(string word in keyword.Split(new char[] {' '}))
			{
				foundName = m_names.Contains(word);

				if(foundName)
					matchBuilder.Append(word + ' ');
			}

			if(foundName)
				keyword = matchBuilder.ToString().TrimEnd(' ');

			return foundName;
		}

		private bool ContainsInMainTags(string keyword)
		{
			bool foundTag = false;
			foreach(string word in keyword.Split(new char[] {' '}))
			{
				string tags = " " + m_landmark.tags;
				foundTag = tags.Contains(word + ';');

				if(foundTag)
					break;
			}

			return foundTag;
		}

		private bool ContainsInSubTags(ref string keyword)
		{
			bool foundTag = false;
			StringBuilder matchBuilder = new StringBuilder();
			foreach(string word in keyword.Split(new char[] {' '}))
			{
				string tags = " " + m_tags;
				foundTag = tags.Contains(" " + word + ";");

				if(foundTag)
					matchBuilder.Append(word + ' ');
			}

			if(foundTag)
				keyword = matchBuilder.ToString().TrimEnd(' ');

			return foundTag;
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

			return stringBuilder.ToString().TrimEnd(' ');
		}
		#endif
		#endregion
	}
}