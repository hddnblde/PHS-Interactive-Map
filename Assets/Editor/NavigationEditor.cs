using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NavigationSystem))]
public class NavigationEditor : Editor
{
	private static Vector3 origin = Vector3.right * 7f;
	private static Vector3 destination = Vector3.left * 7f;

	private void OnEnable()
	{
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
		DrawPositionGizmo(ref origin, Color.yellow);
		DrawPositionGizmo(ref destination, Color.blue);
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

		origin = EditorGUILayout.Vector3Field("Origin", origin);
		destination = EditorGUILayout.Vector3Field("Destination", destination);

		if(GUILayout.Button("Navigate", buttonStyle) && isPlaying)
			NavigationSystem.Navigate(origin, destination);
	}

	private void DrawPositionGizmo(ref Vector3 position, Color color)
	{
		Color previousColor = Handles.color;
		Handles.color = color;
		position = Handles.PositionHandle(position, Quaternion.identity);
		Handles.color = previousColor;
	}
}
