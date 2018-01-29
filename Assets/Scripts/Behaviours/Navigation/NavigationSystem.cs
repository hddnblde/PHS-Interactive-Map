using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Navigation
{
	/// <summary>
	/// The NavigationSystem handles pathfinding and drawing of lines for map navigation.
	/// </summary>
	[RequireComponent(typeof(LineRenderer))]
	public class NavigationSystem : MonoBehaviour
	{
		#region Serialized Fields
		[SerializeField]
		private Transform originMarker = null;

		[SerializeField]
		private Transform destinationMarker = null;
		#endregion


		#region Hidden Fields
		private delegate void NavigateAction(Vector3 origin, Vector3 destination);
		private delegate void ClearAction();

		private static event NavigateAction OnNavigate;
		private static event ClearAction OnClear;

		private LineRenderer lineRenderer = null;
		private const float LineWidthLowerLimit = 3f;
		private const float LineWidthUpperLimit = 7f;
		private const float NearestPointThreshold = 300f;
		#endregion


		#region MonoBehaviour Implementation
		private void Awake()
		{
			Initialize();
		}

		private void OnEnable()
		{
			RegisterInternalEvents();
		}

		private void OnDisable()
		{
			DeregisterInternalEvents();
		}
		#endregion


		#region Initializers
		private void Initialize()
		{
			lineRenderer = GetComponent<LineRenderer>();
		}

		private void RegisterInternalEvents()
		{
			NavigationCamera.OnViewAdjust += OnViewAdjust;
			OnNavigate += Internal_Navigate;
			OnClear += Internal_Clear;
		}

		private void DeregisterInternalEvents()
		{
			NavigationCamera.OnViewAdjust -= OnViewAdjust;
			OnNavigate -= Internal_Navigate;
			OnClear -= Internal_Clear;
		}
		#endregion


		#region Events
		private void OnViewAdjust(float view)
		{
			if(lineRenderer == null)
				return;
			lineRenderer.widthMultiplier = Mathf.Lerp(LineWidthUpperLimit, LineWidthLowerLimit, view);
		}
		#endregion


		#region Actions
		/// <summary>
		/// Navigates the map and draw a line from the origin to destination.
		/// </summary>
		/// <param name="origin">The start position.</param>
		/// <param name="destination">The goal position.</param>
		public static void Navigate(Vector3 origin, Vector3 destination)
		{
			if(OnNavigate != null)
				OnNavigate(origin, destination);
		}

		/// <summary>
		/// Clears the map from drawn lines.
		/// </summary>
		public static void Clear()
		{
			if(OnClear != null)
				OnClear();
		}		

		private void Internal_Navigate(Vector3 origin, Vector3 destination)
		{
			Vector3[] path = FindPath(origin, destination);
			DrawNavigationLine(path);
			FocusCameraToPath(path);
			DrawMarker(originMarker, origin);
			DrawMarker(destinationMarker, destination);
		}

		private void Internal_Clear()
		{
			if(lineRenderer == null)
				return;
			
			lineRenderer.positionCount = 0;
			ClearMarker(originMarker);
			ClearMarker(destinationMarker);
		}
		#endregion

		
		#region Helpers
		private void DrawMarker(Transform marker, Vector3 position)
		{
			if(marker == null)
				return;

			marker.gameObject.SetActive(true);
			marker.position = new Vector3(position.x, NavigationCamera.CameraHeight - 1f, position.z);
			marker.SetAsLastSibling();
		}

		private void ClearMarker(Transform marker)
		{
			if(marker != null)
				marker.gameObject.SetActive(false);
		}

		private void DrawNavigationLine(Vector3[] path)
		{
			if(lineRenderer == null)
				return;

			bool hasPath = (path != null) && (path.Length > 0);
			lineRenderer.positionCount = (hasPath ? path.Length : 0);

			if(hasPath)
				lineRenderer.SetPositions(path);
		}

		private void FocusCameraToPath(Vector3[] path)
		{
			if(path.Length == 0)
				return;

			Bounds frame = new Bounds();

			foreach(Vector3 corner in path)
				frame.Encapsulate(corner);

			NavigationCamera.FrameTo(frame);
		}

		private Vector3[] FindPath(Vector3 origin, Vector3 destination)
		{
			NavMeshPath navMeshPath = new NavMeshPath();

			GetNearestPointInNavMesh(ref origin);
			GetNearestPointInNavMesh(ref destination);

			if(NavMesh.CalculatePath(origin, destination, NavMesh.AllAreas, navMeshPath))
					return navMeshPath.corners;
				else
					return null;
		}
		private void GetNearestPointInNavMesh(ref Vector3 point)
		{
			NavMeshHit hit;
			if(NavMesh.SamplePosition(point, out hit, NearestPointThreshold, NavMesh.AllAreas))
				point = hit.position;
		}
		#endregion
	}
}