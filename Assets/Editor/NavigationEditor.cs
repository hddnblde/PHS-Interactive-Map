using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NavigationSystem))]
public class NavigationEditor : Editor
{
	private class MicroTransform
	{
		private Vector3 m_position = Vector3.zero;
		private Vector3 m_eulerAngles = Vector3.zero;

		public Vector3 position
		{
			get { return m_position; }
			set { m_position = value; }
		}

		public Quaternion rotation
		{
			get { return Quaternion.Euler(m_eulerAngles); }
			set { m_eulerAngles = value.eulerAngles; }
		}
	
		public MicroTransform(Vector3 position, Vector3 eulerAngles)
		{
			m_position = position;
			m_eulerAngles = eulerAngles;
		}

		public MicroTransform(){}
	}

	private static MicroTransform origin = new MicroTransform();
	private static MicroTransform destination = new MicroTransform();
	private NavigationSystem navigationSystem = null;

	private void OnEnable()
	{
		navigationSystem = target as NavigationSystem;
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
		DrawMicroTransformGizmo(origin, Color.yellow);
		DrawMicroTransformGizmo(destination, Color.blue);
		Repaint();
	}

	private void LoadPointData()
	{
		
	}

	private void SavePointData()
	{
		
	}

	private void ClearPointData()
	{
		
	}

//	private MicroTransform Deserialize(string pattern)
//	{
//		char[] pairSeparator = {':'};
//		string[] transformPair = pattern.Split(pairSeparator);
//
//		if(transformPair == null || transformPair.Length != 2)
//			return null;
//
//		char[] valuesSeparator = {';'};
//		string[] positions = transformPair[0].Split(valuesSeparator);
//		string[] rotations = transformPair[1].Split(valuesSeparator);
//
//		MicroTransform transform = new MicroTransform();
////		transform.position = new Vector3
//	}

	private void DrawCustomInspector()
	{
		EditorGUILayout.LabelField("Points", EditorStyles.boldLabel);
		origin.position = EditorGUILayout.Vector3Field("Origin", origin.position);
		destination.position = EditorGUILayout.Vector3Field("Destination", destination.position);

		EditorGUILayout.Space();

		bool isPlaying = Application.isPlaying;
		GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);

		if(!isPlaying)
			buttonStyle.normal = buttonStyle.active;

		if(GUILayout.Button("Navigate", buttonStyle) && isPlaying)
			navigationSystem.Navigate(origin.position, destination.position);
	}

	private void DrawMicroTransformGizmo(MicroTransform microTransform, Color color)
	{
		Color previousColor = Handles.color;
		Handles.color = color;

		switch(Tools.current)
		{
		case Tool.Move:
			microTransform.position = Handles.PositionHandle(microTransform.position, microTransform.rotation);
			break;

		case Tool.Rotate:
			microTransform.rotation = Handles.RotationHandle(microTransform.rotation, microTransform.position);
			break;
		}
		Handles.color = previousColor;
	}
}
