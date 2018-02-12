using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ModestUI.Panels;
using Map;
using Navigation;

namespace Menus.PHS
{
	public class LocationDetailPanel : SimplePanel
	{
		#region Serialized Fields
		[Header("References")]
		[SerializeField]
		private DirectionsPanel directionsPanel = null;

		// [SerializeField]
		// private BuildingInformationPanel buildingInformationPanel = null;

		[Header("UI Elements")]
		[SerializeField]
		private Text displayedText = null;

		[SerializeField]
		private Button buildingInformationButton = null;

		[SerializeField]
		private Button focusButton = null;

		[SerializeField]
		private Button startLocationButton = null;

		[SerializeField]
		private Button destinationButton = null;
		#endregion


		#region Unserialized Fields
		private Location selectedLocation = null;
		private bool selectedLocationIsBuilding = false;
		#endregion

		public override bool Open()
		{
			return Open(null);
		}

		public bool Open(Location location)
		{
			selectedLocation = location;

			if(location == null)
				return false;
			
			FocusToSelectedLocation();
			SetDisplayedTextWithSelectedLocation();

			if(!visible)
				base.Open();
			
			return true;
		}

		protected override void Awake()
		{
			base.Awake();
			Initialize();
		}

		private void Initialize()
		{
			if(startLocationButton != null)
				startLocationButton.onClick.AddListener(SetSelectedLocationAsStart);

			if(destinationButton != null)
				destinationButton.onClick.AddListener(SetSelectedLocationAsDestination);

			if(focusButton != null)
				focusButton.onClick.AddListener(FocusToSelectedLocation);
		}

		private void SetDetails(Location location)
		{
			selectedLocationIsBuilding = location is Place;
			buildingInformationButton.gameObject.SetActive(selectedLocationIsBuilding);
		}

		private void SetSelectedLocationAsStart()
		{
			if(selectedLocation == null || directionsPanel == null)
				return;

			directionsPanel.SetStartLocation(selectedLocation);
			Close();
		}

		private void SetSelectedLocationAsDestination()
		{
			if(selectedLocation == null || directionsPanel == null)
				return;

			directionsPanel.SetDestination(selectedLocation);
			Close();
		}

		private void FocusToSelectedLocation()
		{
			if(selectedLocation != null)
			{
				float zoom = (selectedLocationIsBuilding ? 0.5f : 0.75f);
				NavigationCamera.FocusTo(selectedLocation.position, zoom);
				// NavigationCamera.zoom
			}
		}

		private void ShowSelectedLocationInfo()
		{
			if(!selectedLocationIsBuilding)
				return;

			// buildingInformationPanel.Open(selectedLocation as Place);
		}

		private void SetDisplayedTextWithSelectedLocation()
		{
			if(displayedText == null || selectedLocation == null)
				return;

			displayedText.text = selectedLocation.displayedName;
		}
	}
}