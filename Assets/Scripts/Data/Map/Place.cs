﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Map
{
	[CreateAssetMenu(menuName = "Map/Place", order = 1, fileName = "Place")]
	public class Place : Location
	{
		[SerializeField]
		private Sprite m_thumbnail = null;

		[SerializeField]
		private PlaceTrivia m_trivia = null;

		[SerializeField, Multiline]
		private string m_mapName;

		public Sprite thumbnail
		{
			get { return m_thumbnail; }
		}

		public PlaceTrivia trivia
		{
			get { return m_trivia; }
		}

		public string mapName
		{
			get { return m_mapName; }
		}
	}


	#if UNITY_EDITOR
	[CustomEditor(typeof(Place))]
	public class LocationEditor : Editor
	{
		private static bool useTools = true;

		#region Fields
		private SerializedProperty
		displayedNameProperty = null,
		mapNameProperty = null,
		displayPositionProperty = null,
		positionProperty = null,
		useDisplayPositionProperty = null,
		tagsProperty = null,
		thumbnailProperty = null,
		triviaProperty = null,
		isHiddenProperty = null;
		#endregion


		#region Editor Implementation
		private void OnEnable()
		{
			Initialize();
			SceneView.onSceneGUIDelegate += OnSceneGUI;
			Undo.undoRedoPerformed += Redo;
		}

		private void OnDisable()
		{
			SceneView.onSceneGUIDelegate -= OnSceneGUI;
			Undo.undoRedoPerformed -= Redo;
		}

		public override void OnInspectorGUI()
		{
			DrawCustomInspector();
			DrawTools();
		}

		private void OnSceneGUI(SceneView sceneView)
		{
			EditorGUI.BeginChangeCheck();

			DrawHandle();

			if(EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
				Repaint();
			}
		}

		private void Redo()
		{
			DrawHandle();
			Repaint();
		}

		private void DrawHandle()
		{
			Color color = GUI.color;

			GUI.color = Color.red;
			positionProperty.vector3Value = Handles.PositionHandle(positionProperty.vector3Value, Quaternion.identity);
			
			string positionLabel = (!useDisplayPositionProperty.boolValue ? "[" + displayedNameProperty.stringValue + "]" : "[Position]");
			Handles.Label(positionProperty.vector3Value, positionLabel);

			if(useDisplayPositionProperty.boolValue)
			{
				GUI.color = Color.magenta;
				displayPositionProperty.vector3Value = Handles.PositionHandle(displayPositionProperty.vector3Value, Quaternion.identity);
				Handles.Label(displayPositionProperty.vector3Value, "[" + displayedNameProperty.stringValue + "]");
			}
			else
				displayPositionProperty.vector3Value = positionProperty.vector3Value;

			GUI.color = color;
		}
		#endregion


		#region Methods
		private void Initialize()
		{
			displayedNameProperty = serializedObject.FindProperty("m_displayedName");
			displayPositionProperty = serializedObject.FindProperty("m_displayPosition");
			mapNameProperty = serializedObject.FindProperty("m_mapName");
			useDisplayPositionProperty = serializedObject.FindProperty("m_useDisplayPosition");
			positionProperty = serializedObject.FindProperty("m_position");
			tagsProperty = serializedObject.FindProperty("m_tags");
			thumbnailProperty = serializedObject.FindProperty("m_thumbnail");
			triviaProperty = serializedObject.FindProperty("m_trivia");
			isHiddenProperty = serializedObject.FindProperty("m_isHidden");
		}

		private void DrawCustomInspector()
		{
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.LabelField("Location", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(displayedNameProperty);
			EditorGUILayout.PropertyField(mapNameProperty);
			EditorGUILayout.PropertyField(positionProperty);

			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(useDisplayPositionProperty);

			if(useDisplayPositionProperty.boolValue)
			{
				EditorGUILayout.PropertyField(displayPositionProperty);

				if(GUILayout.Button("Copy from position"))
					displayPositionProperty.vector3Value = positionProperty.vector3Value;
			}

			EditorGUI.indentLevel--;

			EditorGUILayout.LabelField("Place", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(thumbnailProperty);
			EditorGUILayout.PropertyField(triviaProperty, true);

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(tagsProperty);

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(isHiddenProperty);

			if(EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();
		}

		private void DrawTools()
		{
			if(!useTools)
				return;
			
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Tools", EditorStyles.boldLabel);

			DrawNameCopyTool();
		}

		private void DrawNameCopyTool()
		{
			if(GUILayout.Button("Copy name from this asset"))
			{
				displayedNameProperty.stringValue = target.name;
				serializedObject.ApplyModifiedProperties();
			}
		}
		#endregion
	}
	#endif
}