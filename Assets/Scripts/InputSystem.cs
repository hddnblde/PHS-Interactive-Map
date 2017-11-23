using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : MonoBehaviour
{
	#region Navigation
	[Header("Navigation")]
	[SerializeField]
	private NavigationSystem navigationSystem = null;

	[SerializeField]
	private LayerMask groundLayer;

	[SerializeField]
	private float groundLevel = 0f;

	private const float RaycastDistance = 100f;
	#endregion


	#region Camera
	[Header("Camera")]
	[SerializeField]
	private Camera mainCamera = null;

	private Vector3 origin = Vector3.zero;
	private Vector3 destination = Vector3.forward;
	#endregion


	private Vector3 GetPosition(Vector3 screenPoint)
	{
		if(mainCamera == null)
			return Vector3.zero;
		else
		{
			RaycastHit hit;
			Ray ray = mainCamera.ScreenPointToRay(screenPoint);
			Vector3 position = ray.GetPoint(RaycastDistance);

			if(Physics.Raycast(ray, out hit, RaycastDistance, groundLayer))
				position = hit.point;

			return new Vector3(position.x, groundLevel, position.z);
		}
	}

	private void SetDestination()
	{
		if(navigationSystem == null)
			return;

		navigationSystem.Navigate(origin, destination);
	}
}