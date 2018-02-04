using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Menus.MapElement;

namespace Menus.New
{
	public class MapMenuNew : MonoBehaviour
	{
		#region Serialized Fields
		[Header("Generic Menus")]
		[SerializeField]
		private GenericMenu locationMarkers = null;

		[SerializeField]
		private GenericMenu locationSearch = null;

		[SerializeField]
		private GenericMenu setMarkerOnMapMenu = null;

		[SerializeField]
		private GenericMenu floorSelectMenu = null;

		[Header("Buttons")]
		[SerializeField]
		private Button floorSelectButton = null;

		[SerializeField]
		private Button directionsButton = null;

		[Header("Others")]
		[SerializeField]
		private MarkContextMenu markContextMenu = null;

		[SerializeField]
		private SearchMenuNew searchMenu = null;

		[Header("Markers")]
		[SerializeField]
		private MapButton originMarkerButton = null;

		[SerializeField]
		private MapButton destinationMarkerButton = null;
		#endregion


		#region Hidden Fields
		private enum Context
		{
			Browse,
			GetDirections
		}
		#endregion


		#region MonoBehaviour Implementation
		private void Awake()
		{
			Initialize();
		}
		#endregion


		#region Initializers
		private void Initialize()
		{
			RegisterButtonAction(directionsButton, () => SetContext(Context.GetDirections));
			RegisterButtonAction(floorSelectButton, () => ShowFloorSelect(true));
		}
		#endregion


		#region Helpers
		private void RegisterButtonAction(Button button, System.Action action)
		{
			if(button != null)
				button.onClick.AddListener(() => action());
		}

		private void SetContext(Context context)
		{
			if(context == Context.Browse)
			{
				locationSearch.Open();
				locationMarkers.Close();
			}
			else if(context == Context.GetDirections)
			{
				locationMarkers.Open(() => SetContext(Context.Browse));
				locationSearch.Close();
			}
		}

		private void ShowFloorSelect(bool shown)
		{
			if(shown)
				floorSelectMenu.Open();
			else
				floorSelectMenu.Close();
		}
		#endregion
	}
}