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
		private RectTransform markerPanel = null;

		[SerializeField]
		private MapMenuMarkerButton originMarkerButton = null;

		[SerializeField]
		private MapMenuMarkerButton destinationMarkerButton = null;

		[SerializeField]
		private Button chooseOnMapButton = null;

		[SerializeField]
		private SearchMenu searchMenu = null;

		[SerializeField]
		private LocationDatabase locationDatabase = null;

		[SerializeField]
		private NavigationSystem navigationSystem = null;

		[SerializeField]
		private NavigationMenu navigationMenu = null;

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
			RegisterButton(originMarkerButton, "Choose starting point", Context.SetOrigin);
			RegisterButton(destinationMarkerButton, "Choose destination", Context.SetDestination);

			if(chooseOnMapButton != null)
				chooseOnMapButton.onClick.AddListener(MarkLocation);

			if(locationDatabase != null)
				locationDatabase.OnResult += OnResult;
			
		}

		private void OnSearch(string text)
		{
			if(locationDatabase == null)
				return;

			locationDatabase.Search(text);
		}

		private void OnSelect(int index)
		{
			if(locationDatabase == null)
				return;
			
			Location location = locationDatabase.GetLocationFromSearch(index);
			LocationMarker marker = new LocationMarker(location);

			if(marker == null)
				return;

			SetMarker(marker, currentContext);
			CloseContext();
		}

		private void OnResult(int count)
		{
			if(locationDatabase == null || searchMenu == null)
				return;

			if(count == 0)
			{
				searchMenu.SetContent(null);
				return;
			}

			MenuContent[] contents = new MenuContent[count];

			for(int i = 0; i < count; i++)
			{
				Location location = locationDatabase.GetLocationFromSearch(i);
				contents[i] = new MenuContent(null, location.displayedName);
			}

			searchMenu.SetContent(contents);
		}

		private void RegisterButton(MapMenuMarkerButton button, string placeholder, Context context)
		{
			if(button == null)
				return;

			UnityEngine.Events.UnityAction selectAction = () => SearchLocation(context);
			UnityEngine.Events.UnityAction clearAction = () => SetMarker(null, context);

			button.AddListener(selectAction, clearAction, placeholder);
		}

		private void SearchLocation(Context context)
		{
			currentContext = context;
			ToggleSearch(true);
		}

		private void MarkLocation()
		{
			
		}

		private void SetMarker(LocationMarker marker, Context context)
		{
			string displayedName = (marker != null ? marker.displayedName : "");

			if(context == Context.SetDestination)
			{
				destinationMarker = marker;
				destinationMarkerButton.SetDisplayedText(displayedName);
			}
			else
			{
				originMarker = marker;
				originMarkerButton.SetDisplayedText(displayedName);
			}

			Navigate();
		}

		private void CloseContext()
		{
			ToggleSearch(false);
		}

		private void ToggleSearch(bool show)
		{
			if(searchMenu == null || navigationMenu == null || markerPanel == null || chooseOnMapButton != null)
				return;

			if(show)
			{
				string placeholder = "Choose " + (currentContext == Context.SetDestination ? "destination" : "starting point");
				searchMenu.Open(OnSearch, OnSelect, CloseContext, placeholder);
			}
			else
				searchMenu.Close();

			navigationMenu.ShowBackground(show);
			chooseOnMapButton.gameObject.SetActive(show);
			markerPanel.gameObject.SetActive(!show);
		}

		private void Navigate()
		{
			if(navigationSystem == null)
				return;

			if(originMarker == null || destinationMarker == null)
				navigationSystem.Clear();
			else
				navigationSystem.Navigate(originMarker.position, destinationMarker.position);
		}
	}
}