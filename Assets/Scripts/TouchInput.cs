using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInput : MonoBehaviour
{
	#region MonoBehaviour Implementation
	private void Update()
	{
		PinchActivity();
		DragActivity();
	}
	#endregion


	#region Drag Implementation
	public delegate void Drag(Vector2 delta);
	public static event Drag OnDrag;
	private Vector2 dragDelta = Vector2.zero;
	private const float DragFriction = 15f;

	public void DragActivity()
	{
		if(Input.touchCount != 1)
			return;

		Touch currentTouch = Input.GetTouch(0);
		TouchPhase phase = currentTouch.phase;

		switch(phase)
		{
		case TouchPhase.Moved:
		case TouchPhase.Stationary:
			dragDelta = currentTouch.deltaPosition * Time.deltaTime;
			break;
		}

		if(dragDelta.sqrMagnitude == 0)
			return;

		if(OnDrag != null)
			OnDrag(dragDelta);

		dragDelta = Vector2.MoveTowards(dragDelta, Vector2.zero, DragFriction * Time.deltaTime);
	}
	#endregion


	#region Pinch Implementation
	public delegate void Pinch(float delta);
	public static event Pinch OnPinch;
	private float pinchDistance = 0f;

	public void PinchActivity()
	{
		if(Input.touchCount != 2)
			return;

		Touch touch1 = Input.GetTouch(0);
		Touch touch2 = Input.GetTouch(1);

		bool pinching = touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved;
		float distance = Vector2.Distance(touch1.position, touch2.position);

		if(pinching)
		{
			if(OnPinch != null)
				OnPinch((distance - pinchDistance) * Time.deltaTime);
		}

		pinchDistance = distance;
	}
	#endregion
}