using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gestures;

namespace Navigation
{
	/// <summary>
	/// The Navigator handles input to interface with the NavigationSystem.
	/// </summary>
	[RequireComponent(typeof(NavigationSystem))]
	public class Navigator : MonoBehaviour
	{
		#region Serialized Fields
		[Header("Markers")]
		[SerializeField]
		private Vector3 origin = Vector3.right * 7f;

		[SerializeField]
		private Vector3 destination = Vector3.left * 7f;
		#endregion


		#region Hidden Fields
		public delegate void ActivityChange(Activity activity);
		public delegate void MarkerChange(Marker marker);
		public delegate void MarkerAssignment();

		/// <summary>
		/// The context of which position to set in the navigator.
		/// </summary>
		public enum Marker
		{
			/// <summary>
			/// Sets the origin position of the navigator.
			/// </summary>
			Origin,
			/// <summary>
			/// Sets the destination position of the navigator.
			/// </summary>
			Destination
		}

		/// <summary>
		/// The activity of the navigator.
		/// </summary>
		public enum Activity
		{
			/// <summary>
			/// THe navigator will do nothing.
			/// </summary>
			Idle,
			/// <summary>
			/// The navigator will set the marker position after receiving a press event from touch gesture.
			/// </summary>
			SetMarker
		}

		/// <summary>
		/// Occurs when the navigator's activity change.
		/// </summary>
		public static event ActivityChange OnActivityChange;

		/// <summary>
		/// Occurs when the navigator's marker change.
		/// </summary>
		public static event MarkerChange OnMarkerChange;

		/// <summary>
		/// Occurs when the navigator's marker positions is assigned.
		/// </summary>
		public static event MarkerAssignment OnMarkerAssignment;

		private NavigationSystem navigationSystem = null;
		private Marker marker = Marker.Origin;
		private Activity activity = Activity.Idle;
		#endregion


		#region MonoBehaviour Implmentation
		private void Awake()
		{
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
		#endregion


		#region Public Methods
		/// <summary>
		/// Starts navigation with the given marker positions.
		/// </summary>
		public void Navigate()
		{
			if(navigationSystem == null)
				return;

			navigationSystem.Navigate(origin, destination);
		}

		/// <summary>
		/// Changes the activity of the navigator.
		/// </summary>
		public void ChangeActivity(Activity activity)
		{
			this.activity = activity;

			if(OnActivityChange != null)
				OnActivityChange(this.activity);
		}

		/// <summary>
		/// Changes the marker to set position.
		/// </summary>
		public void ChangeMarker(Marker marker)
		{
			this.marker = marker;

			if(OnMarkerChange != null)
				OnMarkerChange(this.marker);
		}
		#endregion


		#region Private Methods
		private void Initialize()
		{
			navigationSystem = GetComponent<NavigationSystem>();
		}

		private void RegisterEvents()
		{
			TouchGestures.OnPress += OnPress;
		}

		private void DeregisterEvents()
		{
			TouchGestures.OnPress -= OnPress;
		}

		private void OnPress(Vector2 point)
		{
			if(activity == Activity.Idle)
				return;
			
			Vector3 markerPosition = NavigationCamera.GetPosition(point);
			AssignMarkerPosition(markerPosition);
		}

		private void AssignMarkerPosition(Vector3 position)
		{
			if(marker == Marker.Origin)
				origin = position;
			else if(marker == Marker.Destination)
				destination = position;

			if(OnMarkerAssignment != null)
				OnMarkerAssignment();
		}
		#endregion
	}
}