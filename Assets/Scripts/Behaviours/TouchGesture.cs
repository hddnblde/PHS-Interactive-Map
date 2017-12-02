using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TapGesture(Vector2 screenPoint);
public delegate void DragGesture(Vector2 delta);
public delegate void TwoFingerGesture(float delta);

public class TouchGesture : MonoBehaviour
{
	#region Serialized Fields
	[Header("Activities")]
	[SerializeField]
	private bool drag = true;
	[SerializeField]
	private bool pinch = true;
	[SerializeField]
	private bool rotate = true;
	[SerializeField]
	private bool press = true;

	[Header("Constraint")]
	[SerializeField]
	private bool multiTapBlocksOtherActivities = false;

	[SerializeField, Range(MinimumPressWaitTime, MaximumPressWaitTime)]
	private float pressWaitTime = 1.75f;
	#endregion


	#region Hidden Fields
	// Events
	public static event TapGesture OnSingleTap;
	public static event TapGesture OnDoubleTap;
	public static event TapGesture OnPress;

	public static event DragGesture OnDrag;
	public static event TwoFingerGesture OnPinch;
	public static event TwoFingerGesture OnRotate;


	// Drag Activity
	private const float DragFriction = 0.5f;
	private const float DragMagnitudeLimit = 50f;
	private Vector2 dragDelta = Vector2.zero;

	// Pinch Activity
	private const float PinchDistanceLimit = 1920f;
	private float pinchDistance = 0f;

	// Rotate Activity
	private const float DeltaRotationLimit = 45f;
	private Vector2 pinchDirection = Vector2.zero;

	// Press Activity
	private const float MinimumPressWaitTime = 0.5f;
	private const float MaximumPressWaitTime = 3f;
	private float currentPressTime = 0f;
	private bool pressed = false;
	private bool pressCancelled = false;
	#endregion


	#region MonoBehaviour Implementation
	private void Update()
	{
		GestureActivities();
	}
	#endregion


	#region Method
	private void GestureActivities()
	{
		if(TapActivity() && multiTapBlocksOtherActivities)
			return;
		
		PinchAndRotateActivity();
		DragActivity();
		PressActivity();
	}

	private bool TapActivity()
	{
		if(Input.touchCount != 1)
			return false;

		Touch currentTouch = Input.GetTouch(0);

		if(currentTouch.tapCount == 1 && OnSingleTap != null)
			OnSingleTap(currentTouch.position);
		else if(currentTouch.tapCount == 2 && OnDoubleTap != null)
			OnDoubleTap(currentTouch.position);

		return currentTouch.tapCount > 1;
	}

	private void PressActivity()
	{
		if(!press || Input.touchCount != 1)
		{
			currentPressTime = 0f;
			pressed = false;
			return;
		}

		if(pressed)
			return;
		
		Touch currentTouch = Input.GetTouch(0);

		if(currentTouch.phase == TouchPhase.Stationary && !pressCancelled)
			currentPressTime = Mathf.Min(currentPressTime + Time.deltaTime, pressWaitTime);
		else if(currentTouch.phase == TouchPhase.Moved)
		{
			pressCancelled = true;
			currentPressTime = 0f;
		}
		else if(currentTouch.phase == TouchPhase.Ended || currentTouch.phase == TouchPhase.Canceled || currentTouch.phase == TouchPhase.Began)
			pressCancelled = false;
		
		if(currentPressTime >= pressWaitTime)
		{
			pressed = true;
			if(OnPress != null)
				OnPress(currentTouch.position);
		}
	}

	private void PinchAndRotateActivity()
	{
		if(Input.touchCount == 2)
		{
			Touch touch1 = Input.GetTouch(0);
			Touch touch2 = Input.GetTouch(1);
			bool began = touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began;
			bool moving = touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved;

			float distanceDelta = 0f;
			float rotationDelta = 0f;
			if(pinch)
			{
				float currentDistance = Vector2.Distance(touch1.position, touch2.position);
				distanceDelta = (currentDistance - pinchDistance) * Time.deltaTime;
				pinchDistance = Mathf.Clamp(currentDistance, 0f, PinchDistanceLimit);
			}

			if(rotate)
			{
				Vector2 direction = (touch2.position - touch1.position).normalized;
				rotationDelta = Mathf.Clamp(Vector2.SignedAngle(direction, pinchDirection), -DeltaRotationLimit, DeltaRotationLimit);
				pinchDirection = direction;
			}

			if(!began && moving && Mathf.Abs(rotationDelta) > 0f && OnRotate != null)
				OnRotate(rotationDelta);

			if(!began && moving && Mathf.Abs(distanceDelta) > 0f &&  OnPinch != null)
				OnPinch(distanceDelta);
		}
	}

	private void DragActivity()
	{
		if(drag && Input.touchCount == 1)
		{
			Touch currentTouch = Input.GetTouch(0);
			TouchPhase phase = currentTouch.phase;
			dragDelta = currentTouch.deltaPosition;
		}
		else if(Input.touchCount > 1)
			dragDelta = Vector2.zero;


		float dragMagnitude = dragDelta.magnitude;

		if(dragMagnitude <= 0)
			return;
		else if(dragMagnitude > DragMagnitudeLimit)
			dragDelta = Vector2.ClampMagnitude(dragDelta, DragMagnitudeLimit);

		if(OnDrag != null)
			OnDrag(dragDelta * Time.deltaTime);
		
		dragDelta = Vector2.MoveTowards(dragDelta, Vector2.zero, DragFriction);
	}
	#endregion
}