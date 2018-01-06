using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gestures;

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
		private delegate void FocusAction(Vector3 position);
		private static event FocusAction OnFocus;

		/// <summary>
		/// Focuses the navigation camera to a specified position on the map.
		/// </summary>
		public static void FocusTo(Vector3 position)
		{
			if(OnFocus != null)
				OnFocus(position);
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
		private LayerMask m_groundLayer;
		#endregion


		#region Hidden Fields
		public delegate void ViewAdjust(float view);
		public static event ViewAdjust OnViewAdjust;

		private const float RaycastDistance = 100f;
		private const float CameraHeight = 30f;
		private const float GroundHeight = 0f;
		private const float ViewLowerLimit = 10f;
		private const float ViewUpperLimit = 170f;
		private const float TransitionDuration = 0.25f;
		private const float TranslateSpeed = 7f;
		private const float ZoomDefault = 0.5f;
		private const float ZoomDampTime = 0.15f;

		private Coroutine transitionRoutine = null;
		private float zoomVelocity = 0f;
		#endregion


		#region Property
		private float view
		{
			get { return m_view; }
			set
			{
				m_view = value;
				ViewAdjustEvent();
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
			TouchGestures.OnDoubleTap += OnDoubleTap;
			TouchGestures.OnDrag += OnDrag;
			TouchGestures.OnRotate += OnRotate;
			TouchGestures.OnPinch += OnPinch;

			OnFocus += FocusFrame;
		}

		private void DeregisterEvents()
		{
			TouchGestures.OnDoubleTap -= OnDoubleTap;
			TouchGestures.OnDrag -= OnDrag;
			TouchGestures.OnRotate -= OnRotate;
			TouchGestures.OnPinch -= OnPinch;

			OnFocus -= FocusFrame;
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
			float yaw = transform.eulerAngles.y - (delta * rotationSpeed);
			transform.eulerAngles = (Vector3.up * yaw) + (Vector3.right * 90f);
		}

		private void Zoom(float delta)
		{
			StopTransition();
			view = Mathf.Clamp01(view + (delta * zoomSpeed));
		}

		private void ResetView()
		{
			StopTransition();
			transitionRoutine = StartCoroutine(ResetViewRoutine());
		}

		private void FocusFrame(Vector3 frame)
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
			if(navigationCamera == null)
				return;

			float targetView = Mathf.Lerp(ViewUpperLimit, ViewLowerLimit, m_view);

			navigationCamera.orthographicSize = Mathf.SmoothDamp(navigationCamera.orthographicSize, targetView, ref zoomVelocity, ZoomDampTime);
		}

		private void ViewAdjustEvent()
		{
			if(OnViewAdjust != null)
				OnViewAdjust(m_view);
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
	}
}