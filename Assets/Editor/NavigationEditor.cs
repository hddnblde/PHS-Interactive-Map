using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NavigationSystem))]
public class NavigationEditor : Editor
{
	private NavigationSystem navigationSystem = null;
	private SerializedProperty originProperty = null;
	private SerializedProperty destinationProperty = null;

	private void OnEnable()
	{
		navigationSystem = target as NavigationSystem;
		originProperty = serializedObject.FindProperty("origin");
		destinationProperty = serializedObject.FindProperty("destination");
		Tools.hidden = true;
	}

	private void OnDisable()
	{
		Tools.hidden = false;
	}

	public override void OnInspectorGUI()
	{
		DrawCustomInspector();
	}

	private void OnSceneGUI()
	{
		DrawPositionGizmo(originProperty, Color.yellow);
		DrawPositionGizmo(destinationProperty, Color.blue);
		Repaint();
	}

	private void DrawCustomInspector()
	{
		DrawDefaultInspector();
		EditorGUILayout.Space();

		bool isPlaying = Application.isPlaying;
		GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);

		if(!isPlaying)
			buttonStyle.normal = buttonStyle.active;

		if(GUILayout.Button("Navigate", buttonStyle) && isPlaying)
			navigationSystem.Navigate();
	}

	private void DrawPositionGizmo(SerializedProperty position, Color color)
	{
		EditorGUI.BeginChangeCheck();
		Color previousColor = Handles.color;
		Handles.color = color;
		position.vector3Value = Handles.PositionHandle(position.vector3Value, Quaternion.identity);

		Handles.color = previousColor;

		if(EditorGUI.EndChangeCheck())
			serializedObject.ApplyModifiedProperties();
	}
}
