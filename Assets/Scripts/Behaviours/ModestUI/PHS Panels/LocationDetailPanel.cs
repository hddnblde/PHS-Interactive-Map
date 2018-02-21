using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ModestUI.Panels;
using Map;
using Navigation;
using Databases;

namespace Menus.PHS
{
	public class LocationDetailPanel : SlidingPanel
	{
		#region Serialized Fields
		[Header("References")]
		[SerializeField]
		private DirectionsPanel directionsPanel = null;

		[SerializeField]
		private BuildingInformationPanel buildingInformationPanel = null;

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
			ValidateBuildingInfoButton();

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

			if(buildingInformationButton != null)
				buildingInformationButton.onClick.AddListener(ShowSelectedLocationInfo);
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
				float zoom = (selectedLocation is Place ? 0.5f : 0.75f);
				NavigationCamera.FocusTo(selectedLocation.position, zoom);
			}
		}

		private void ShowSelectedLocationInfo()
		{
			Place building = GetBuilding();

			if(building == null)
				return;

			buildingInformationPanel.Open(building);
		}

		private void SetDisplayedTextWithSelectedLocation()
		{
			if(displayedText == null || selectedLocation == null)
				return;

			displayedText.text = selectedLocation.displayedName;
		}

		private void ValidateBuildingInfoButton()
		{
			if(buildingInformationButton == null)
				return;
			
			bool hasBuildingInformation = SelectedBuildingHasTrivia();
			buildingInformationButton.gameObject.SetActive(hasBuildingInformation);
		}

		private bool SelectedBuildingHasTrivia()
		{
			return LocationDatabase.PlaceHasTrivia(GetBuilding());
		}

		private Place GetBuilding()
		{
			if(selectedLocation as Place)
				return selectedLocation as Place;
			else
				return LocationDatabase.GetBuildingFromRoom(selectedLocation as Room);
		}
	}
}