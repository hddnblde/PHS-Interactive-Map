using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
	public class MapTable : MonoBehaviour
	{
		#region Fields
		[SerializeField]
		private string keywordSearch = "";

		[SerializeField]
		private List<Place> m_places = new List<Place>();

		private List<Location> m_searchedLocations = new List<Location>();

		private delegate void SearchAction(string keyword);
		private delegate void AddAction(Location location);

		private event SearchAction OnSearch;
		#endregion


		#region Static Implementation
		private static event AddAction OnAddLocation;
		public static void AddLocation(Location location)
		{
			if(OnAddLocation != null)
				OnAddLocation(location);
		}
		#endregion


		#region MonoBehaviour Implementation
		private void Awake()
		{
			CreateInstanceOfPlaces();
		}

		private void OnEnable()
		{
			RegisterEvent();
		}

		private void OnDisable()
		{
			DeregisterEvent();
		}
		#endregion


		#region Methods
		private void RegisterEvent()
		{
			OnAddLocation += Add;
		}

		private void DeregisterEvent()
		{
			OnAddLocation -= Add;
		}

		private void Add(Location location)
		{
			if(!m_searchedLocations.Contains(location))
				m_searchedLocations.Add(location);
		}

		private void CreateInstanceOfPlaces()
		{
			if(m_places == null || m_places.Count == 0)
				return;
			
			List<Place> places = new List<Place>();

			foreach(Place place in m_places)
			{
				Place instance = Instantiate<Place>(place);
				OnSearch += instance.Search;
				places.Add(instance);
			}

			m_places = places;
		}

		private void SearchForLocation(string keyword)
		{
			if(OnSearch != null)
				OnSearch(keyword);
		}
		#endregion
	}
}