using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]
public class NavigationSystem : MonoBehaviour
{
	#region Serialized Fields
	[SerializeField]
	private Vector3 origin = Vector3.zero;

	[SerializeField]
	private Vector3 destination = Vector3.forward;
	#endregion

	#region Hidden Fields
	private LineRenderer lineRenderer = null;
	#endregion


	#region MonoBehaviour Implementation
	private void Awake()
	{
		Initialize();
	}
	#endregion


	#region Methods
	private void Initialize()
	{
		lineRenderer = GetComponent<LineRenderer>();
	}

	public void Navigate(Vector3 origin, Vector3 destination)
	{
		this.origin = origin;
		this.destination = destination;
		Navigate();
	}

	public void Navigate()
	{
		DrawNavigationPath(FindPath());
	}

	private void DrawNavigationPath(Vector3[] path)
	{
		if(lineRenderer == null)
			return;

		bool hasPath = (path != null) && (path.Length > 0);
		lineRenderer.positionCount = (hasPath ? path.Length : 0);

		if(hasPath)
			lineRenderer.SetPositions(path);
	}

	private Vector3[] FindPath()
	{
		NavMeshPath navMeshPath = new NavMeshPath();
		NavMeshHit hit;

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
	#endregion
}
