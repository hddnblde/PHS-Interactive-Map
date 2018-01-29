using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Map;
using Navigation;

namespace Menus
{
	public class LocationMarker
	{
		public LocationMarker(string displayedName, Vector3 position)
		{
			m_position = position;
			m_displayedName = displayedName;
		}

		public LocationMarker(Location location)
		{
			if(location == null)
				return;
			
			m_position = location.position;
			m_displayedName = location.displayedName;
		}

		private Vector3 m_position = Vector3.zero;
		private string m_displayedName = "Marker";

		public Vector3 position
		{
			get { return m_position; }
		}

		public string displayedName
		{
			get { return m_displayedName; }
		}
	}

	public class MapMenu : MonoBehaviour
	{
		public delegate void FloorSelect(int index);
		public static event FloorSelect OnFloorSelect;

		[SerializeField]
		private CanvasGroup markerPanel = null;

		[SerializeField]
		private Button chooseOnMapButton = null;

		[SerializeField]
		private MapMenuMarkerButton originMarkerButton = null;

		[SerializeField]
		private MapMenuMarkerButton destinationMarkerButton = null;

		[SerializeField]
		private MarkerMenu markerMenu = null;

		[SerializeField]
		private LocationDatabase locationDatabase = null;

		[SerializeField]
		private NavigationSystem navigationSystem = null;

		[SerializeField]
		private NavigationMenu navigationMenu = null;

		[SerializeField]
		private int currentFloor = 1;

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
		}

		private void OnResult(int count)
		{
			if(locationDatabase == null)
				return;

			if(count == 0)
			{
				SearchMenu.SetContent(null);
				return;
			}

			MenuContent[] contents = new MenuContent[count];

			for(int i = 0; i < count; i++)
			{
				Location location = locationDatabase.GetLocationFromSearch(i);
				contents[i] = new MenuContent(null, location.displayedName);
			}

			SearchMenu.SetContent(contents);
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
			ShowMarkerPanel(false);
		}

		private void MarkLocation()
		{
			ToggleSearch(false);
			ShowMarkerPanel(false);
			string displayedName = "Mark " + (currentContext == Context.SetDestination ? "destination" : "starting point");
			markerMenu.Open(displayedName, SetMarker, () => ToggleSearch(true));
		}

		private void SetMarker(LocationMarker marker)
		{
			SetMarker(marker, currentContext);
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
			CloseContext();
		}

		private void CloseContext()
		{
			ToggleSearch(false);
			ShowMarkerPanel(true);
		}

		private void ToggleSearch(bool show)
		{
			if(navigationMenu == null)
				return;

			if(show)
			{
				string placeholder = "Choose " + (currentContext == Context.SetDestination ? "destination" : "starting point");
				SearchMenu.Open(this.OnSearch, this.OnSelect, this.CloseContext, placeholder);
			}
			else if(SearchMenu.isOpen)
				SearchMenu.Close();

			navigationMenu.ShowBackground(show);
			chooseOnMapButton.gameObject.SetActive(show);
		}

		private void ShowMarkerPanel(bool show)
		{
			if(markerPanel == null)
				return;
			
			markerPanel.alpha = (show ? 1f : 0f);
			markerPanel.interactable = show;
			markerPanel.blocksRaycasts = show;
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

		private void FloorSelectEvent()
		{
			if(OnFloorSelect != null)
				OnFloorSelect(currentFloor);
		}

		private void Update()
		{
			if(Input.GetKeyDown(KeyCode.Space))
				FloorSelectEvent();
		}
	}
}