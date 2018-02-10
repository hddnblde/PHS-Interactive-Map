using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModestUI.Behaviour;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ModestUI.Buttons
{
	public class SimpleButton : ButtonBehaviour
	{}

	#if UNITY_EDITOR
	[CustomEditor(typeof(SimpleButton))]
	public class ButtonBehaviourEditor : Editor
	{
		private SerializedProperty
		targetGraphicsProperty = null,
		pressHighlightProperty = null,
		transitionDurationProperty = null,
		pressHighlightDurationProperty = null,
		curveProperty = null,
		normalColorProperty = null,
		pressedColorProperty = null;

		private void OnEnable()
		{
			targetGraphicsProperty = serializedObject.FindProperty("targetGraphics");
			pressHighlightProperty = serializedObject.FindProperty("pressHighlight");
			transitionDurationProperty = serializedObject.FindProperty("transitionDuration");
			pressHighlightDurationProperty = serializedObject.FindProperty("pressHighlightDuration");
			curveProperty = serializedObject.FindProperty("curve");
			normalColorProperty = serializedObject.FindProperty("normalColor");
			pressedColorProperty = serializedObject.FindProperty("pressedColor");
		}

		public override void OnInspectorGUI()
		{
			DrawCustomInpsector();
		}

		private void DrawCustomInpsector()
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(targetGraphicsProperty, true);
			EditorGUILayout.PropertyField(pressHighlightProperty);
			EditorGUILayout.PropertyField(transitionDurationProperty);
			EditorGUILayout.PropertyField(pressHighlightDurationProperty);
			EditorGUILayout.PropertyField(curveProperty);
			EditorGUILayout.PropertyField(normalColorProperty);
			EditorGUILayout.PropertyField(pressedColorProperty);

			if(EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();
		}
	}
	#endif
}