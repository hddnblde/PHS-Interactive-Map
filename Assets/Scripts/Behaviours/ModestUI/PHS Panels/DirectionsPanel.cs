using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ModestUI.Panels;
using Map;

namespace Menus.PHS
{
	public class DirectionsPanel : SimplePanel
	{
		#region Data Structure
		[System.Serializable]
		private class MarkerButton
		{
			[SerializeField]
			private Button mainButton = null;

			[SerializeField]
			private Button clearButton = null;

			[SerializeField]
			private Text displayedText = null;

			[SerializeField]
			private string label;

			public void AddListener(Action onClick, Action onClear)
			{
				if(mainButton != null)
					mainButton.onClick.AddListener(() => onClick());

				if(clearButton != null)
					clearButton.onClick.AddListener(() => onClear());
			}

			public void Set(LocationMarker marker)
			{
				string displayedText = (marker == null ? "" : marker.displayedName);
				SetDisplayedText(displayedText);
			}

			private void SetDisplayedText(string text)
			{
				if(displayedText == null)
					return;

				bool isEmpty = string.IsNullOrEmpty(text);

				if(isEmpty)
					text = "<i>@label</i>".Replace("@label", label);
				
				ShowClearButton(!isEmpty);
				displayedText.text = text;
			}

			private void ShowClearButton(bool shown)
			{
				if(clearButton == null)
					return;

				clearButton.gameObject.SetActive(shown);
				clearButton.enabled = shown;
			}
		}

		private enum Context
		{
			StartLocation,
			Destination
		}
		#endregion


		#region Serialized Fields
		[SerializeField]
		private MarkerButton startLocationButton = null;

		[SerializeField]
		private MarkerButton destinationButton = null;

		[SerializeField]
		private SearchLocationPanel searchLocationPanel = null;

		[SerializeField]
		private Button mapMarkerButton = null;

		[SerializeField]
		private MapMarkerPanel mapMarkerPanel = null;
		#endregion


		#region Unserialized Fields
		private Context context = Context.StartLocation;
		private LocationMarker startLocationMarker = null;
		private LocationMarker destinationMarker = null;
		#endregion


		#region MonoBehaviour Implementation
		protected override void Awake()
		{
			base.Awake();
			Initialize();
		}
		#endregion


		#region Initializers
		private void Initialize()
		{
			SetupMarkerButton(startLocationButton, Context.StartLocation);
			SetupMarkerButton(destinationButton, Context.Destination);

			if(mapMarkerPanel != null)
			{
				mapMarkerPanel.OnMark += SetMarkerByContext;
				mapMarkerPanel.OnCancel += OnMapMarkerCancel;
				mapMarkerPanel.OnOpen += OnMapMarkerOpen;
			}
		}

		private void SetupMarkerButton(MarkerButton markerButton, Context context)
		{
			if(markerButton == null)
				return;

			Action onClick = () => OpenSearch(context);
			Action onClear = () => ClearMarker(context);
			markerButton.AddListener(onClick, onClear);
			markerButton.Set(null);
		}
		#endregion


		#region Actions
		public void SetStartLocation(Location location)
		{
			if(destinationMarker != null && destinationMarker.displayedName == location.displayedName)
				ClearMarker(Context.Destination);

			SetMarker(Context.StartLocation, new LocationMarker(location));
			Open();
		}

		public void SetDestination(Location location)
		{
			if(startLocationMarker != null && startLocationMarker.displayedName == location.displayedName)
				ClearMarker(Context.StartLocation);
			
			SetMarker(Context.Destination, new LocationMarker(location));
			Open();
		}

		private void OpenSearch(Context context)
		{
			if(searchLocationPanel == null)
				return;
			
			this.context = context;
			string displayedContext = "Where " + (context == Context.StartLocation ? "from" : "to") + '?';
			
			searchLocationPanel.Open(displayedContext, OnSelectLocation, OnSearchClose, false, true);
			OnSearchOpen();
		}

		private void ClearMarker(Context context)
		{
			SetMarker(context, null);
		}
		#endregion


		#region Events
		private void OnSelectLocation(Location location)
		{
			SetMarkerByContext(location);
		}

		private void OnMapMarkerOpen()
		{
			if(searchLocationPanel != null)
				searchLocationPanel.Close();

			Close();
		}

		private void OnSearchOpen()
		{
			Close();
			ShowMapMarkerButton(true);
		}

		private void OnMapMarkerCancel()
		{
			OpenSearch(context);
		}

		private void OnSearchClose()
		{
			Open();
			ShowMapMarkerButton(false);
		}
		#endregion


		#region Helpers
		private void ShowMapMarkerButton(bool shown)
		{
			if(mapMarkerButton != null)
				mapMarkerButton.gameObject.SetActive(shown);
		}

		private void SetMarkerByContext(Location location)
		{
			SetMarker(context, new LocationMarker(location));
		}

		private void SetMarkerByContext(LocationMarker locationMarker)
		{
			SetMarker(context, locationMarker);
			Open();
		}

		private void SetMarker(Context context, LocationMarker marker)
		{
			if(context == Context.StartLocation)
			{
				startLocationButton.Set(marker);
				startLocationMarker = marker;
			}
			else
			{
				destinationButton.Set(marker);
				destinationMarker = marker;
			}

			Navigate();
		}

		private void Navigate()
		{
			Navigation.NavigationSystem.Clear();

			if(startLocationMarker != null && destinationMarker != null)
				Navigation.NavigationSystem.Navigate(startLocationMarker.position, destinationMarker.position);
		}
		#endregion		
	}
}