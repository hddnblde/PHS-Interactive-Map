using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Map;
using Navigation;

namespace Menus
{
	public class MapMenu : MonoBehaviour
	{
		private class LocationMarker
		{
			public LocationMarker(Location location)
			{
				this.location = location;
			}

			private Location location = null;

			public Vector3 position
			{
				get
				{
					if(location == null)
						return Vector3.zero;
					else
						return location.position;
				}
			}

			public string displayedName
			{
				get
				{
					if(location == null)
						return "";
					else
						return location.displayedName;
				}
			}
		}

		[SerializeField]
		private Button setOriginButton = null;

		[SerializeField]
		private Button setDestinationButton = null;

		[SerializeField]
		private SearchMenu searchMenu = null;

		[SerializeField]
		private LocationDatabase locationDatabase = null;

		[SerializeField]
		private NavigationSystem navigationSystem = null;

		private LocationMarker originMarker = null;
		private LocationMarker destinationMarker = null;
		private Context currentContext = Context.SetOrigin;

		private enum Context
		{
			SetOrigin,
			SetDestination
		}

		private void Awake()
		{
			Initialize();
		}

		private void Initialize()
		{
			RegisterButton(setOriginButton, Context.SetOrigin);
			RegisterButton(setDestinationButton, Context.SetDestination);

			if(locationDatabase != null)
				locationDatabase.OnResult += OnResult;
		}

		private void OnSearch(string text)
		{
			locationDatabase.Search(text);

		}

		private void OnSelect(int index)
		{
			if(locationDatabase == null)
				return;
			
			Location location = locationDatabase.GetLocationFromSearch(index);
			LocationMarker marker = new LocationMarker(location);

			if(currentContext == Context.SetDestination)
				destinationMarker = marker;
			else
				originMarker = marker;
		}

		private void OnResult(int count)
		{
			if(count <= 0 || searchMenu == null)
				return;

			MenuContent[] contents = new MenuContent[count];

			for(int i = 0; i < count; i++)
			{
				Location location = locationDatabase.GetLocationFromSearch(i);
				contents[i] = new MenuContent(null, location.displayedName);
			}

			searchMenu.SetContent(contents);
		}

		private void RegisterButton(Button button, Context context)
		{
			if(button == null)
				return;

			button.onClick.AddListener(() => SetContext(context));
		}

		private void SetContext(Context context)
		{
			currentContext = context;
		}

		private void OpenSearch()
		{
			if(searchMenu != null)
				searchMenu.Open(OnSearch, OnSelect);
		}

		private void CloseSearch()
		{
			if(searchMenu != null)
				searchMenu.Close();
		}

		private void Navigate()
		{
			if(navigationSystem == null || originMarker == null || destinationMarker == null)
				return;

			navigationSystem.Navigate(originMarker.position, destinationMarker.position);
		}
	}
}