using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gestures
{
	[DisallowMultipleComponent]
	public class TouchGestures : MonoBehaviour
	{
		#region Serialized Fields
		[Header("Single Touch Activities")]
		[SerializeField]
		private bool tap = true;
		[SerializeField]
		private bool press = true;
		[SerializeField]
		private bool drag = true;

		[Header("Double Touch Activities")]
		[SerializeField]
		private bool pinch = true;
		[SerializeField]
		private bool rotate = true;

		[Header("Constraints")]
		[SerializeField, Range(1f, 3f)]
		private float dragMultiplier = 1f;

		[SerializeField, Range(MinLongPressDuration, MaxLongPressDuration)]
		private float longPressDuration = DefaultLongPressDuration;
		#endregion


		#region Hidden Fields
		private const float DefaultLongPressDuration = 0.625f;
		public const float MinLongPressDuration = 0.25f;
		public const float MaxLongPressDuration = 1f;
		private const float FlickDetectionDuration = 0.35f;

		private bool canPress = true;
		private bool pressed = false;
		private float currentTouchTime = 0f;

		private Coroutine dragInertia = null;
		#endregion


		#region Delegates
		private delegate void Evaluate(int touchCount, float deltaTime);
		public delegate void Tap(Vector2 position);
		public delegate void Drag(Vector2 delta);
		public delegate void PinchRotate(float delta);
		#endregion


		#region Events
		private event Evaluate OnEvaluate;
		public static event Tap OnTap;
		public static event Tap OnPress;
		public static event Tap OnDoubleTap;
		public static event Drag OnDrag;
		public static event PinchRotate OnPinch;
		public static event PinchRotate OnRotate;
		#endregion


		#region MonoBehaviour Implementation
		private void OnEnable()
		{
			RegisterEvents();
		}

		private void OnDisable()
		{
			DeregisterEvents();
		}

		private void Update()
		{
			EvaluateActivity();
		}
		#endregion


		#region Methods
		private void RegisterEvents()
		{
			OnEvaluate += SingleTouchActivity;
			OnEvaluate += DoubleTouchActivity;
		}

		private void DeregisterEvents()
		{
			OnEvaluate -= SingleTouchActivity;
			OnEvaluate -= DoubleTouchActivity;
		}
		#endregion


		#region Main Activities
		private void EvaluateActivity()
		{			
			if(OnEvaluate != null)
				OnEvaluate(Input.touchCount, Time.deltaTime);
		}

		private void SingleTouchActivity(int touchCount, float deltaTime)
		{
			if(touchCount != 1)
			{
				ResetPress();
				ResetTouchTime();
				return;
			}

			Touch touch = Input.GetTouch(0);

			DetectTouchTime(deltaTime);

			if(tap && touch.tapCount > 0)
				DetectTaps(touch.tapCount, touch.position);

			if(press && canPress && !pressed)
				DetectPress(touch.phase, touch.position);
		
			if(drag)
				DetectDrag(touch.phase, touch.deltaPosition, deltaTime);
		}

		private void DoubleTouchActivity(int touchCount, float deltaTime)
		{
			if(touchCount != 2)
				return;

			Touch touch1 = Input.GetTouch(0);
			Touch touch2 = Input.GetTouch(1);

			Vector2 previousTouch1 = touch1.position - touch1.deltaPosition;
			Vector2 previousTouch2 = touch2.position - touch2.deltaPosition;

			if(pinch)
				DetectPinch(previousTouch1, previousTouch2, touch1.position, touch2.position, deltaTime);

			if(rotate)
				DetectRotation(previousTouch1, previousTouch2, touch1.position, touch2.position, deltaTime);
		}
		#endregion


		#region Sub Activities
		// Single touch Gestures
		private void DetectTaps(int tapCount, Vector2 position)
		{
			if(tapCount == 1 && OnTap != null)
				OnTap(position);

			if(tapCount == 2 && OnDoubleTap != null)
				OnDoubleTap(position);
		}

		private void DetectPress(TouchPhase phase, Vector2 position)
		{
			if(canPress)
				canPress = (phase != TouchPhase.Moved && phase != TouchPhase.Canceled);
			else
				return;

			bool pressDetected = (currentTouchTime >= longPressDuration);

			if(!pressed && pressDetected)
			{
				pressed = true;
				if(OnPress != null)
					OnPress(position);
			}
		}

		private void DetectDrag(TouchPhase phase, Vector2 deltaPosition, float deltaTime)
		{
			bool canFlick = currentTouchTime <= FlickDetectionDuration;
			StopDragInertia();

			if(phase == TouchPhase.Moved)
				ApplyDrag(deltaPosition, deltaTime);
			else if(phase == TouchPhase.Ended && canFlick && deltaPosition.sqrMagnitude > 0f)
				StartDragInertia(deltaPosition);
		}

		// Double touch Gestures
		private void DetectPinch(Vector2 previousTouch1, Vector2 previousTouch2, Vector2 currentTouch1, Vector2 currentTouch2, float deltaTime)
		{
			float previousDelta = (previousTouch1 - previousTouch2).magnitude;
			float currentDelta = (currentTouch1 - currentTouch2).magnitude;
			float pinchDelta = currentDelta - previousDelta;

			if(Mathf.Abs(pinchDelta) > 0f && OnPinch != null)
				OnPinch(pinchDelta * deltaTime);
		}

		private void DetectRotation(Vector2 previousTouch1, Vector2 previousTouch2, Vector2 currentTouch1, Vector2 currentTouch2, float deltaTime)
		{
			Vector2 previousDirection = previousTouch1 - previousTouch2;
			Vector2 currentDirection = currentTouch1 - currentTouch2;

			float rotationDelta = Vector2.SignedAngle(currentDirection, previousDirection);

			if(Mathf.Abs(rotationDelta) > 0f && OnRotate != null)
				OnRotate(rotationDelta);
		}

		private void ResetTouchTime()
		{
			currentTouchTime = 0f;
		}

		// Others
		private void DetectTouchTime(float deltaTime)
		{
			if(currentTouchTime < MaxLongPressDuration)
				currentTouchTime = Mathf.Min(currentTouchTime + deltaTime, MaxLongPressDuration);
		}

		private void ResetPress()
		{
			canPress = true;
			pressed = false;
		}

		private void StopDragInertia()
		{
			if(dragInertia != null)
			{
				StopCoroutine(dragInertia);
				dragInertia = null;
			}
		}

		private void StartDragInertia(Vector2 deltaPosition)
		{
			dragInertia = StartCoroutine(DragInertiaRoutine(deltaPosition));
		}

		private void ApplyDrag(Vector2 deltaPosition, float deltaTime)
		{
			if(OnDrag != null)
				OnDrag((deltaPosition * dragMultiplier) * deltaTime);
		}
		#endregion


		#region Coroutines
		private IEnumerator DragInertiaRoutine(Vector2 deltaPosition)
		{
			while(deltaPosition.sqrMagnitude > 0)
			{
				float deltaTime = Time.deltaTime;
				ApplyDrag(deltaPosition, deltaTime);

				deltaPosition = Vector2.MoveTowards(deltaPosition, Vector2.zero, 10f * deltaTime);
				yield return null;
			}
		}
		#endregion
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(TouchGestures))]
	public class TouchEditor : Editor
	{
		#region Serialized Properties
		private SerializedProperty
		// single touch activities
		tapProperty = null,
		pressProperty = null,
		dragProperty = null,

		// double touch activities
		pinchProperty = null,
		rotateProperty = null,

		// constraints
		dragMultiplierProperty = null,
		longPressDurationProperty = null;
		#endregion


		#region Editor Implementation
		private void OnEnable()
		{
			Initialize();
		}

		public override void OnInspectorGUI()
		{
			DrawCustomInspector();
		}
		#endregion


		#region Methods
		private void Initialize()
		{
			tapProperty = serializedObject.FindProperty("tap");
			pressProperty = serializedObject.FindProperty("press");
			dragProperty = serializedObject.FindProperty("drag");
			pinchProperty = serializedObject.FindProperty("pinch");
			rotateProperty = serializedObject.FindProperty("rotate");
			dragMultiplierProperty = serializedObject.FindProperty("dragMultiplier");
			longPressDurationProperty = serializedObject.FindProperty("longPressDuration");
		}

		private void DrawCustomInspector()
		{
			EditorGUI.BeginChangeCheck();

			// single touch activities
			DrawTapProperty();
			DrawPressProperty();
			DrawDragProperty();

			// double touch activities
			DrawPinchProperty();
			DrawRotateProperty();

			if(EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();
		}

		private void DrawTapProperty()
		{
			EditorGUILayout.PropertyField(tapProperty);
		}

		private void DrawBoolFloatProperty(SerializedProperty boolProperty, SerializedProperty floatProperty, string floatLabel, float min, float max)
		{
			EditorGUILayout.PropertyField(boolProperty);
			if(boolProperty.boolValue)
			{
				EditorGUI.indentLevel++;
				floatProperty.floatValue = EditorGUILayout.Slider(floatLabel, floatProperty.floatValue, min, max);
				EditorGUI.indentLevel--;
			}
		}

		private void DrawPressProperty()
		{
			DrawBoolFloatProperty(pressProperty, longPressDurationProperty, "Duration", TouchGestures.MinLongPressDuration, TouchGestures.MaxLongPressDuration);
		}

		private void DrawDragProperty()
		{
			DrawBoolFloatProperty(dragProperty, dragMultiplierProperty, "Multiplier", 1f, 3f);
		}

		private void DrawPinchProperty()
		{
			EditorGUILayout.PropertyField(pinchProperty);
		}

		private void DrawRotateProperty()
		{
			EditorGUILayout.PropertyField(rotateProperty);
		}
		#endregion
	}
	#endif
}