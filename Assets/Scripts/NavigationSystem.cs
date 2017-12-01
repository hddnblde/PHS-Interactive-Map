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
		#region Hidden Fields
		private LineRenderer lineRenderer = null;
		private const float LineWidthLowerLimit = 3f;
		private const float LineWidthUpperLimit = 7f;
		#endregion


		#region MonoBehaviour Implementation
		private void Awake()
		{
			Initialize();
		}

		private void OnEnable()
		{
			NavigationCamera.OnViewAdjust += OnViewAdjust;
		}

		private void OnDisable()
		{
			NavigationCamera.OnViewAdjust -= OnViewAdjust;
		}
		#endregion


		#region Methods
		/// <summary>
		/// Navigates the map and draw a line from the origin to destination.
		/// </summary>
		/// <param name="origin">The start position.</param>
		/// <param name="destination">The goal position.</param>
		public void Navigate(Vector3 origin, Vector3 destination)
		{
			DrawNavigationLine(FindPath(origin, destination));
		}

		private void Initialize()
		{
			lineRenderer = GetComponent<LineRenderer>();
		}

		private void OnViewAdjust(float view)
		{
			if(lineRenderer == null)
				return;

			lineRenderer.widthMultiplier  = Mathf.Lerp(LineWidthUpperLimit, LineWidthLowerLimit, view);
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

		private Vector3[] FindPath(Vector3 origin, Vector3 destination)
		{
			NavMeshPath navMeshPath = new NavMeshPath();
			NavMeshHit hit;

			GetNearestPointInNavMesh(ref origin);
			GetNearestPointInNavMesh(ref destination);

			if(NavMesh.Raycast(origin, destination, out hit, NavMesh.AllAreas))
			{
				if(NavMesh.CalculatePath(origin, destination, NavMesh.AllAreas, navMeshPath))
					return navMeshPath.corners;
				else
					return null;
			}
			else
				return new Vector3[] {origin, destination};
		}

		private void GetNearestPointInNavMesh(ref Vector3 point)
		{
			NavMeshHit hit;
			if(NavMesh.SamplePosition(point, out hit, 3f, NavMesh.AllAreas))
				point = hit.position;
		}
		#endregion
	}
}