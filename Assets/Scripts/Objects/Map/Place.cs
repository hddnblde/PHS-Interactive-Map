using System.Collections;
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

		[SerializeField, Multiline]
		private string m_description;

		public Sprite thumbnail
		{
			get { return m_thumbnail; }
		}

		public string description
		{
			get { return m_description; }
		}
	}


	#if UNITY_EDITOR
	[CustomEditor(typeof(Place))]
	public class LocationEditor : Editor
	{
		private static bool useTools = true;

		#region Fields
		private Place location = null;
		private SerializedProperty
		displayedNameProperty = null,
		positionProperty = null,
		tagsProperty = null,
		thumbnailProperty = null,
		descriptionProperty = null;
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
			positionProperty.vector3Value = Handles.PositionHandle(positionProperty.vector3Value, Quaternion.identity);
		}
		#endregion


		#region Methods
		private void Initialize()
		{
			location = target as Place;
			displayedNameProperty = serializedObject.FindProperty("m_displayedName");
			positionProperty = serializedObject.FindProperty("m_position");
			tagsProperty = serializedObject.FindProperty("m_tags");
			thumbnailProperty = serializedObject.FindProperty("m_thumbnail");
			descriptionProperty = serializedObject.FindProperty("m_description");
		}

		private void DrawCustomInspector()
		{
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.LabelField("Location", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(displayedNameProperty);
			EditorGUILayout.PropertyField(positionProperty);

			EditorGUILayout.LabelField("Place", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(thumbnailProperty);
			EditorGUILayout.PropertyField(descriptionProperty, true);

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(tagsProperty);

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