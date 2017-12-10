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
		private Landmark m_landmark = null;

		[SerializeField]
		private List<Room> m_rooms = new List<Room>();

		public Landmark landmark
		{
			get { return m_landmark; }
		}

		public Location place
		{
			get { return this; }
		}

		public Room[] rooms
		{
			get
			{
				if(!hasRooms)
					return null;
				else
				{
					if(m_rooms == null || m_rooms.Count == 0)
						return null;
					else
						return m_rooms.ToArray();
				}
			}
		}

		public bool hasRooms
		{
			get
			{
				if(m_landmark == null)
					return false;
				else
					return m_landmark.hasRooms;// && m_rooms != null && m_rooms.Count > 0;
			}
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
		landmarkProperty = null,
		roomsProperty = null;
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
			landmarkProperty = serializedObject.FindProperty("m_landmark");
			roomsProperty = serializedObject.FindProperty("m_rooms");
		}

		private void DrawCustomInspector()
		{
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.LabelField("Location", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(displayedNameProperty);

			if(!location.hasRooms)
				EditorGUILayout.PropertyField(positionProperty);
			else
			{
				GUIContent content = new GUIContent("General Position");
				EditorGUILayout.PropertyField(positionProperty, content);
			}

			EditorGUILayout.PropertyField(landmarkProperty);

			if(location.hasRooms)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(roomsProperty, true);
				EditorGUI.indentLevel--;
			}

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

			DrawTransformCopyTool();
			DrawNameCopyTool();
			SetBuildingNameToRooms();
		}

		private void DrawTransformCopyTool()
		{

		}

		private void DrawNameCopyTool()
		{
			if(GUILayout.Button("Copy name from this asset"))
			{
				displayedNameProperty.stringValue = target.name;
				serializedObject.ApplyModifiedProperties();
			}
		}

		private void SetBuildingNameToRooms()
		{
			if(!location.hasRooms || roomsProperty.arraySize == 0)
				return;

			if(GUILayout.Button("Set Building Name To Rooms"))
			{
				
			}
		}
		#endregion
	}
	#endif
}