using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigator : MonoBehaviour
{
	#region Static Implementation
	public static event ModeChange OnModeChange;
	private static event Navigate OnNavigate;

	private static Marker marker = Marker.Origin;
	private static Mode mode = Mode.Idle;

	public static void ChangeMode(Mode mode)
	{
		Navigator.mode = mode;

		if(OnModeChange != null)
			OnModeChange(Navigator.mode);
	}

	public static void ChangeMarker(Marker marker)
	{
		Navigator.marker = marker;
	}

	public static void StartNavigation()
	{
		if(OnNavigate != null)
			OnNavigate();
	}
	#endregion


	#region Serialized Fields
	[Header("Markers")]
	[SerializeField]
	private Vector3 origin = Vector3.right * 7f;

	[SerializeField]
	private Vector3 destination = Vector3.left * 7f;
	#endregion


	#region Hidden Fields
	private NavigationSystem navigationSystem = null;

	public delegate void ModeChange(Mode mode);
	private delegate void Navigate();

	public enum Marker
	{
		Origin,
		Destination
	}

	public enum Mode
	{
		Idle,
		SetMarker
	}
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


	#region Methods
	private void Initialize()
	{
		navigationSystem = GetComponent<NavigationSystem>();
	}

	private void RegisterEvents()
	{
		OnNavigate += OnNavigateEvent;
		TouchGesture.OnPress += OnPress;
	}

	private void DeregisterEvents()
	{
		OnNavigate -= OnNavigateEvent;
		TouchGesture.OnPress -= OnPress;
	}

	private void OnPress(Vector2 point)
	{
		if(mode == Mode.Idle)
			return;
		
		Vector3 currentPoint = NavigationCamera.GetPosition(point);

		if(marker == Marker.Origin)
			origin = currentPoint;
		else if(marker == Marker.Destination)
			destination = currentPoint;
	}

	private void OnNavigateEvent()
	{
		if(navigationSystem == null)
			return;

		navigationSystem.Navigate(origin, destination);
	}
	#endregion
}