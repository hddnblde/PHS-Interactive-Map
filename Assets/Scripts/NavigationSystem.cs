using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]
public class NavigationSystem : MonoBehaviour
{
	#region Hidden Fields
	private static LineRenderer m_lineRenderer = null;
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
	private void Initialize()
	{
		m_lineRenderer = GetComponent<LineRenderer>();
	}

	private void OnViewAdjust(float view)
	{
		if(m_lineRenderer == null)
			return;

		m_lineRenderer.widthMultiplier  = Mathf.Lerp(LineWidthUpperLimit, LineWidthLowerLimit, view);
	}
	#endregion


	#region Static Implementation
	public static void Navigate(Vector3 origin, Vector3 destination)
	{
		DrawNavigationPath(FindPath(origin, destination));
	}

	private static void DrawNavigationPath(Vector3[] path)
	{
		if(m_lineRenderer == null)
			return;

		bool hasPath = (path != null) && (path.Length > 0);
		m_lineRenderer.positionCount = (hasPath ? path.Length : 0);

		if(hasPath)
			m_lineRenderer.SetPositions(path);
	}

	private static Vector3[] FindPath(Vector3 origin, Vector3 destination)
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

	private static void GetNearestPointInNavMesh(ref Vector3 point)
	{
		NavMeshHit hit;
		if(NavMesh.SamplePosition(point, out hit, 3f, NavMesh.AllAreas))
			point = hit.position;
	}
	#endregion
}
