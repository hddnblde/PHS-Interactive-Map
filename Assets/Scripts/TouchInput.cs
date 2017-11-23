using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInput : MonoBehaviour
{
	#region Serialized Fields
	[Header("Swipe")]
	[SerializeField, Range(0.1f, 3f)]
	private float swipeMultiplier = 1f;
	[SerializeField]
	private float swipeFriction = 3f;
	#endregion


	#region Hidden Fields
	private Vector2 dragDirection = Vector2.zero;
	private float currentPinchDistance = 0f;
	private const float MaxDeltaSpeed = 15f;

	public delegate void Pinch(float amount);
	public static event Pinch OnPinch;

	public delegate void Drag(Vector2 direction);
	public static event Drag OnDrag;
	#endregion


	#region MonoBehaviour Implementation
	private void OnValidate()
	{
		swipeFriction = Mathf.Max(0f, swipeFriction);
	}

	private void Update()
	{
		DetectDrag();
		DecaySwipe();
		DetectSqueeze();
	}
	#endregion


	#region Functions
	private void DetectDrag()
	{
		if(Input.touchCount != 1)
			return;
		
		Touch currentTouch = Input.GetTouch(0);
		TouchPhase phase = currentTouch.phase;

		switch(phase)
		{
		case TouchPhase.Moved:
		case TouchPhase.Stationary:
			dragDirection = currentTouch.deltaPosition;

			if(dragDirection.magnitude > MaxDeltaSpeed)
				dragDirection = Vector2.ClampMagnitude(dragDirection, MaxDeltaSpeed);

			dragDirection *= Time.deltaTime;
			break;
		}
	}

	private void DetectPinch()
	{
		if(Input.touchCount != 2)
			return;
	}

	private void DetectSqueeze()
	{
		if(Input.touchCount < 2)
			return;

		List<Vector2> touchPoints = new List<Vector2>();

		for(int i = 0; i < Input.touchCount; i++)
		{
			Touch currentTouch = Input.GetTouch(i);
			if(currentTouch.phase == TouchPhase.Moved)
				touchPoints.Add(currentTouch.position);
		}

		float averageDistance = GetAverageDistance(touchPoints.ToArray());
		float distance = averageDistance - currentPinchDistance;

		if(Mathf.Abs(distance) > 0)
		{
			if(OnPinch != null)
				OnPinch(distance * Time.deltaTime);

			currentPinchDistance = averageDistance;;
		}
	}

	private void DecaySwipe()
	{
		if(Input.touchCount != 1)
			dragDirection = Vector2.zero;
		
		if(dragDirection.sqrMagnitude == 0)
			return;

		if(float.IsNaN(dragDirection.x) || float.IsNaN(dragDirection.y))
			dragDirection = Vector2.zero;

		if(OnDrag != null)
			OnDrag(dragDirection);

		float x = dragDirection.x;
		float y = dragDirection.y;
		float deltaTime = Time.deltaTime;

		x = Mathf.Max(0f, (Mathf.Abs(x) - (swipeFriction * deltaTime))) * Mathf.Sign(x);
		y = Mathf.Max(0f, (Mathf.Abs(y) - (swipeFriction * deltaTime))) * Mathf.Sign(y);

		dragDirection = new Vector2(x, y);
	}
	#endregion


	#region Helpers
	private Vector2 GetCenterPoint(Vector2[] points)
	{
		if(points == null || points.Length == 0)
			return Vector2.zero;

		Vector2 result = Vector2.zero;
		foreach(Vector2 point in points)
			result += point;

		result /= points.Length;
		return result;
	}

	private float GetAverageDistance(Vector2[] polygonalPoints)
	{
		if(polygonalPoints == null || polygonalPoints.Length < 3)
			return 0f;

		Vector2 centerPoint = GetCenterPoint(polygonalPoints);
		float averageDistance = 0f;

		foreach(Vector2 point in polygonalPoints)
			averageDistance += (point - centerPoint).sqrMagnitude;

		averageDistance /= polygonalPoints.Length;
		return averageDistance;
	}
	#endregion
}