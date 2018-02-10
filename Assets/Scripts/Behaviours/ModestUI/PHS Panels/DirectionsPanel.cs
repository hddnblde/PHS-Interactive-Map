using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ModestUI.Panels;

namespace Menus.PHS
{
	public class DirectionsPanel : SimplePanel
	{
		[SerializeField]
		private Button startLocationButton = null;

		[SerializeField]
		private Button destinationButton = null;

		[SerializeField]
		private SearchLocationPanel searchLocationPanel = null;

		[SerializeField]
		private Button mapMarkerButton = null;

		[SerializeField]
		private MapMarkerPanel mapMarkerPanel = null;

		private Context context = Context.SearchStartLocation;

		private enum Context
		{
			SearchStartLocation,
			SearchDestination
		}

		protected override void Awake()
		{
			base.Awake();
			Initialize();
		}
		
		private void OnEnable()
		{
			RegisterEvents();
		}

		private void OnDisable()
		{
			DeregisterEvents();
		}

		private void Initialize()
		{
			if(startLocationButton != null)
				startLocationButton.onClick.AddListener(() => OpenSearch(Context.SearchStartLocation));

			if(destinationButton != null)
				destinationButton.onClick.AddListener(() => OpenSearch(Context.SearchDestination));
		}

		private void RegisterEvents()
		{
			if(searchLocationPanel != null)
			{
				searchLocationPanel.OnOpen += OnSearchOpen;
				searchLocationPanel.OnClose += OnSearchClose;
			}
		}

		private void DeregisterEvents()
		{
			if(searchLocationPanel != null)
			{
				searchLocationPanel.OnOpen -= OnSearchOpen;
				searchLocationPanel.OnClose -= OnSearchClose;
			}

			if(mapMarkerPanel != null)
			{
				// mapMarkerPanel.OnOpen -= ;
			}
		}

		private void OpenSearch(Context context)
		{
			this.context = context;
			string displayedContext = "Search " + (context == Context.SearchStartLocation ? "Start Location" : "Destination");

			if(searchLocationPanel != null)
				searchLocationPanel.Open(displayedContext, SelectLocation);
		}

		private void SelectLocation(int index)
		{
			if(context == Context.SearchStartLocation)
			{

			}
			else
			{

			}
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

		private void OnMapMarkerMark(LocationMarker marker)
		{
			Open();
			//do stuff
		}

		private void OnMapMarkerCloseCancel()
		{
			OpenSearch(context);
		}

		private void OnSearchClose()
		{
			Open();
			ShowMapMarkerButton(false);
		}

		private void ShowMapMarkerButton(bool shown)
		{
			if(mapMarkerButton != null)
				mapMarkerButton.gameObject.SetActive(shown);
		}
	}
}