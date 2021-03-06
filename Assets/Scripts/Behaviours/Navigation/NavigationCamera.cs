﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gestures;
using Menus;

namespace Navigation
{
	/// <summary>
	/// The NavigationCamera handles input to navigate through a map.
	/// </summary>
	public class NavigationCamera : MonoBehaviour
	{
		#region Static Implementation
		private static Camera navigationCamera = null;
		private static LayerMask groundLayer;
		private delegate void FocusAction(Vector3 position, float zoom);
		private delegate void FrameAction(Bounds frame);
		private static event FocusAction OnFocus;
		private static event FrameAction OnFrame;

		/// <summary>
		/// Focuses the navigation camera to a specified position on the map.
		/// </summary>
		public static void FocusTo(Vector3 position, float zoom = ZoomDefault)
		{
			if(OnFocus != null)
				OnFocus(position, zoom);
		}

		public static void FrameTo(Bounds frame)
		{
			if(OnFrame != null)
				OnFrame(frame);
		}

		/// <summary>
		/// Gets the world position from the camera's screen point.
		/// </summary>
		public static Vector3 GetPosition(Vector2 screenPoint)
		{
			if(navigationCamera == null)
				return Vector3.zero;
			else
			{
				RaycastHit hit;
				Ray ray = navigationCamera.ScreenPointToRay(screenPoint);
				Vector3 position = ray.GetPoint(RaycastDistance);

				if(Physics.Raycast(ray, out hit, RaycastDistance, groundLayer))
					position = hit.point;

				return new Vector3(position.x, GroundHeight, position.z);
			}
		}
		#endregion


		#region Serialized Fields
		[Header("Navigation")]
		[SerializeField, Range(0f, 1f)]
		private float m_view = 0.5f;

		[Header("Settings")]
		[SerializeField]
		private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		[SerializeField]
		private Vector2 boundary = new Vector2(100f, 170f);

		[Header("Input")]
		[SerializeField]
		private LayerMask m_groundLayer;
		#endregion


		#region Hidden Fields
		public delegate void ViewAdjust(float view);
		public static event ViewAdjust OnViewAdjust;

		private const float RaycastDistance = 100f;
		public const float CameraHeight = 30f;
		private const float GroundHeight = 0f;
		private const float ViewLowerLimit = 10f;
		private const float ViewUpperLimit = 300f;
		private const float TransitionDuration = 0.85f;
		private const float TranslateSpeed = 7f;
		private const float ZoomDefault = 0.5f;
		private const float ZoomDampTime = 0.15f;

		private Coroutine transitionRoutine = null;
		private float zoomVelocity = 0f;
		private float currentView = 0f;
		#endregion


		#region Property
		private float view
		{
			get { return m_view; }
			set
			{
				m_view = value;
				// ViewAdjustEvent();
			}
		}
		#endregion


		#region MonoBehaviour Implementation
		private void Awake()
		{
			Initialize();
		}

		private void Start()
		{
			ViewAdjustEvent();
		}

		private void OnValidate()
		{
			ViewAdjustEvent();
		}

		private void Update()
		{
			ZoomUpdate();
		}

		private void OnEnable()
		{
			RegisterGestureEvents();
			RegisterMenuContextEvent();
		}

		private void OnDisable()
		{
			DeregisterGestureEvents();
			DeregisterMenuContextEvent();
		}
		#endregion


		#region Gestures Implementation
		private void RegisterGestureEvents()
		{
			TouchGestures.OnDoubleTap += OnDoubleTap;
			TouchGestures.OnDrag += OnDrag;
			TouchGestures.OnRotate += OnRotate;
			TouchGestures.OnPinch += OnPinch;

			OnFocus += FocusFrame;
			OnFrame += FrameByBounds;
		}

		private void DeregisterGestureEvents()
		{
			TouchGestures.OnDoubleTap -= OnDoubleTap;
			TouchGestures.OnDrag -= OnDrag;
			TouchGestures.OnRotate -= OnRotate;
			TouchGestures.OnPinch -= OnPinch;

			OnFocus -= FocusFrame;
			OnFrame -= FrameByBounds;
		}

		private void OnDoubleTap(Vector2 screenPoint)
		{
			if(!listenToGestures)
				return;

			ResetView();
		}

		private void OnDrag(Vector2 deltaPosition)
		{
			if(!listenToGestures)
				return;

			Pan(deltaPosition);
		}

		private void OnRotate(float delta)
		{
			if(!listenToGestures)
				return;

			Rotate(delta);
		}

		private void OnPinch(float delta)
		{
			if(!listenToGestures)
				return;

			Zoom(delta);
		}
		#endregion


		#region Methods
		private void Initialize()
		{
			navigationCamera = GetComponent<Camera>();
			groundLayer = m_groundLayer;
		}

		private void Pan(Vector2 delta)
		{
			float speedRatio = (navigationCamera.orthographicSize / ViewLowerLimit) * Time.deltaTime;
			Vector3 translation = new Vector3(delta.x, delta.y, 0f) * -(speedRatio * TranslateSpeed);

			transform.Translate(translation);

			Vector3 clampedPosition =
				new Vector3(Mathf.Clamp(transform.position.x, -boundary.x, boundary.x),
				CameraHeight,
				Mathf.Clamp(transform.position.z, -boundary.y, boundary.y));
			
			transform.position = clampedPosition;
		}

		private void Rotate(float delta)
		{
			StopTransition();
			float yaw = transform.eulerAngles.y - delta;
			transform.eulerAngles = (Vector3.up * yaw) + (Vector3.right * 90f);
		}

		private void Zoom(float delta)
		{
			StopTransition();
			view = Mathf.Clamp01(view + delta);
		}

		private void ResetView()
		{
			StopTransition();
			transitionRoutine = StartCoroutine(ResetViewRoutine());
		}

		private void FocusFrame(Vector3 frame)
		{
			FocusFrame(frame, ZoomDefault);
		}
		
		private void FocusFrame(Vector3 frame, float targetZoom)
		{
			frame = new Vector3(frame.x, CameraHeight, frame.z);
			StopTransition();
			transitionRoutine = StartCoroutine(FocusFrameRoutine(frame, targetZoom));
		}

		private void FrameByBounds(Bounds frame)
		{
			float targetView = Mathf.Lerp(1f, 0f, frame.size.magnitude / ViewUpperLimit) - 0.25f;
			FocusFrame(frame.center, targetView);
		}

		private void StopTransition()
		{
			if(transitionRoutine != null)
				StopCoroutine(transitionRoutine);
		}

		private void ZoomUpdate()
		{
			if(navigationCamera == null)
				return;

			if(!Mathf.Approximately(currentView, m_view))
				ViewAdjustEvent();

			currentView = Mathf.SmoothDamp(currentView, m_view, ref zoomVelocity, ZoomDampTime);
			float targetView = Mathf.Lerp(ViewUpperLimit, ViewLowerLimit, currentView);

			navigationCamera.orthographicSize = targetView;// Mathf.SmoothDamp(navigationCamera.orthographicSize, targetView, ref zoomVelocity, ZoomDampTime);
		}

		private void ViewAdjustEvent()
		{
			if(OnViewAdjust != null)
				OnViewAdjust(currentView); //OnViewAdjust(m_view);
		}
		#endregion


		#region Coroutines
		private IEnumerator ResetViewRoutine()
		{
			float currentZoom = m_view;
			for(float current = TransitionDuration; current > 0f; current -= Time.deltaTime)
			{
				float t = Mathf.InverseLerp(TransitionDuration, 0f, current);
				view = Mathf.LerpUnclamped(currentZoom, ZoomDefault, transitionCurve.Evaluate(t));
				yield return null;
			}
		}

		private IEnumerator FocusFrameRoutine(Vector3 frame, float targetZoom)
		{
			TouchGestures.StopDrag();

			Vector3 currentPosition = transform.position;
			float currentZoom = view;
			for(float current = TransitionDuration; current > 0f; current -= Time.deltaTime)
			{
				float t = Mathf.InverseLerp(TransitionDuration, 0f, current);
				float curvedT = transitionCurve.Evaluate(t);
				transform.position = Vector3.LerpUnclamped(currentPosition, frame, curvedT);

				view = Mathf.LerpUnclamped(currentZoom, targetZoom, curvedT);
				yield return null;
			}

			transform.position = Vector3.LerpUnclamped(currentPosition, frame, 1f);
			view = Mathf.LerpUnclamped(currentZoom, targetZoom, transitionCurve.Evaluate(1f));
		}
		#endregion
	

		#region Menu Context Implementation
		private bool listenToGestures = true;

		private void RegisterMenuContextEvent()
		{
			NavigationMenu.OnContextSelect += OnContextSelect;
		}

		private void DeregisterMenuContextEvent()
		{
			NavigationMenu.OnContextSelect -= OnContextSelect;
		}

		private void OnContextSelect(NavigationMenu.Context context)
		{
			listenToGestures = context == NavigationMenu.Context.Map;
		}
		#endregion
	}
}