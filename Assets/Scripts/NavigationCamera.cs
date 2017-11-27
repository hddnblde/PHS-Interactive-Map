using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationCamera : MonoBehaviour
{
	#region Serialized Fields
	[Header("Navigation")]
	[SerializeField, Range(0f, 1f)]
	private float view = 0.5f;

	[Header("Settings")]
	[SerializeField, Range(1f, 3f)]
	private float rotationSpeed = 1.25f;

	[SerializeField, Range(0.25f, 2f)]
	private float zoomSpeed = 1f;

	[SerializeField]
	private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	[SerializeField]
	private Vector2 boundary = new Vector2(100f, 170f);

	[Header("Input")]
	[SerializeField]
	private LayerMask groundLayer;
	#endregion


	#region Hidden Fields
	public delegate void ViewAdjust(float view);
	public static event ViewAdjust OnViewAdjust;
	private Camera m_camera = null;

	private const float RaycastDistance = 100f;
	private const float CameraHeight = 30f;
	private const float GroundHeight = 0f;
	private const float MovementSpeedLowerLimit = 1.5f;
	private const float MovementSpeedUpperLimit = 15f;
	private const float ViewLowerLimit = 10f;
	private const float ViewUpperLimit = 100f;
	private const float TransitionDuration = 0.25f;
	private const float ZoomDefault = 0.5f;
	private const float ZoomDampTime = 0.15f;

	private Coroutine transitionRoutine = null;
	private float zoomVelocity = 0f;
	#endregion


	#region MonoBehaviour Implementation
	private void Awake()
	{
		m_camera = GetComponent<Camera>();
	}

	private void OnValidate()
	{
		Zoom(0f);
	}

	private void Update()
	{
		ZoomUpdate();
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


	#region Gestures Implementation
	private void RegisterEvents()
	{
		TouchGesture.OnDoubleTap += OnDoubleTap;
		TouchGesture.OnDrag += OnDrag;
		TouchGesture.OnRotate += OnRotate;
		TouchGesture.OnPinch += OnPinch;
		TouchGesture.OnPress += OnPress;
	}

	private void DeregisterEvents()
	{
		TouchGesture.OnDoubleTap -= OnDoubleTap;
		TouchGesture.OnDrag -= OnDrag;
		TouchGesture.OnRotate -= OnRotate;
		TouchGesture.OnPinch -= OnPinch;
		TouchGesture.OnPress -= OnPress;
	}

	private void OnDoubleTap(Vector2 screenPoint)
	{
		ResetView();
	}

	private void OnDrag(Vector2 deltaPosition)
	{
		Pan(deltaPosition);
	}

	private void OnRotate(float delta)
	{
		Rotate(delta);
	}

	private void OnPinch(float delta)
	{
		Zoom(delta);
	}

	private void OnPress(Vector2 screenPoint)
	{
		NavigationSystem.Navigate(Vector3.zero, GetPosition(screenPoint));
	}
	#endregion


	#region Methods
	private void Pan(Vector2 delta)
	{
		delta *= -Mathf.Lerp(MovementSpeedUpperLimit, MovementSpeedLowerLimit, view);
		transform.Translate(delta.x, delta.y, 0f);

		Vector3 clampedPosition =
			new Vector3(Mathf.Clamp(transform.position.x, -boundary.x, boundary.x),
			CameraHeight,
			Mathf.Clamp(transform.position.z, -boundary.y, boundary.y));
		
		transform.position = clampedPosition;
	}

	private void Rotate(float delta)
	{
		StopTransition();
		float yaw = transform.eulerAngles.y - (delta * rotationSpeed);
		transform.eulerAngles = (Vector3.up * yaw) + (Vector3.right * 90f);
	}

	private void Zoom(float delta)
	{
		StopTransition();
		view = Mathf.Clamp01(view + (delta * zoomSpeed));

		if(OnViewAdjust != null)
			OnViewAdjust(view);
	}

	private void ResetView()
	{
		StopTransition();
		transitionRoutine = StartCoroutine(ResetViewRoutine());
	}

	public void FocusFrame(Vector3 frame)
	{
		StopTransition();
		transitionRoutine = StartCoroutine(FocusFrameRoutine(frame));
	}

	private void StopTransition()
	{
		if(transitionRoutine != null)
			StopCoroutine(transitionRoutine);
	}

	private void ZoomUpdate()
	{
		if(m_camera == null)
			return;

		float targetView = Mathf.Lerp(ViewUpperLimit, ViewLowerLimit, view);

		m_camera.orthographicSize = Mathf.SmoothDamp(m_camera.orthographicSize, targetView, ref zoomVelocity, ZoomDampTime);
	}
	#endregion


	#region Coroutines
	private IEnumerator ResetViewRoutine()
	{
		float currentZoom = view;
		for(float current = TransitionDuration; current > 0f; current -= Time.deltaTime)
		{
			float t = Mathf.InverseLerp(TransitionDuration, 0f, current);
			view = Mathf.LerpUnclamped(currentZoom, ZoomDefault, transitionCurve.Evaluate(t));
			yield return null;
		}
	}

	private IEnumerator FocusFrameRoutine(Vector3 frame, bool resetView = true)
	{
		Vector3 currentPosition = transform.position;
		float currentZoom = view;
		for(float current = TransitionDuration; current > 0f; current -= Time.deltaTime)
		{
			float t = Mathf.InverseLerp(TransitionDuration, 0f, current);
			transform.position = Vector3.LerpUnclamped(currentPosition, frame, t);

			if(resetView)
				view = Mathf.LerpUnclamped(currentZoom, ZoomDefault, transitionCurve.Evaluate(t));
			yield return null;
		}
	}
	#endregion


	#region Function
	public Vector3 GetPosition(Vector2 screenPoint)
	{
		if(m_camera == null)
			return Vector3.zero;
		else
		{
			RaycastHit hit;
			Ray ray = m_camera.ScreenPointToRay(screenPoint);
			Vector3 position = ray.GetPoint(RaycastDistance);

			if(Physics.Raycast(ray, out hit, RaycastDistance, groundLayer))
				position = hit.point;

			return new Vector3(position.x, GroundHeight, position.z);
		}
	}
	#endregion
}
