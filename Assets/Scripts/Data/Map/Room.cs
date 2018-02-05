using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Map
{
	[CreateAssetMenu(menuName = "Map/Room", order = 2, fileName = "Room")]
	public class Room : Location
	{
		[SerializeField]
		private bool m_standaloneName = false;

		[SerializeField]
		private bool m_standaloneNumber = false;

		[SerializeField]
		private int m_floor = 1;

		[SerializeField]
		private int m_room = 1;

		[SerializeField, Multiline]
		private string m_mapName;

		public int floor
		{
			get { return ((m_standaloneNumber || m_standaloneName) ? 0 : m_floor); }
		}

		public override string displayedName
		{
			get { return FloorNamingConvention(); }
		}

		public string mapName
		{
			get { return m_mapName; }
		}

		private string FloorNamingConvention()
		{
			if(m_standaloneName)
				return base.displayedName;
			
			string baseName = base.displayedName + ' ';

			if(m_standaloneNumber)
				return baseName + m_room.ToString();
			else
				return baseName + m_floor.ToString() + m_room.ToString("00");
		}
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(Room))]
	public class RoomEditor : Editor
	{
		private Room room = null;
		#region Fields
		private SerializedProperty
		displayedNameProperty = null,
		mapNameProperty = null,
		displayPositionProperty = null,
		positionProperty = null,
		useDisplayPositionProperty = null,
		tagsProperty = null,
		standaloneNameProperty = null,
		standaloneNumberProperty = null,
		floorProperty = null,
		roomProperty = null;
		#endregion


		#region Editor Implementation
		private void OnEnable()
		{
			Initialize();
			SceneView.onSceneGUIDelegate += OnSceneGUI;
		}

		private void OnDisable()
		{
			SceneView.onSceneGUIDelegate -= OnSceneGUI;
		}

		public override void OnInspectorGUI()
		{
			DrawCustomInspector();
		}
		#endregion


		#region Methods
		private void Initialize()
		{
			room = target as Room;
			displayedNameProperty = serializedObject.FindProperty("m_displayedName");
			mapNameProperty = serializedObject.FindProperty("m_mapName");
			displayPositionProperty = serializedObject.FindProperty("m_displayPosition");
			positionProperty = serializedObject.FindProperty("m_position");
			useDisplayPositionProperty = serializedObject.FindProperty("m_useDisplayPosition");
			tagsProperty = serializedObject.FindProperty("m_tags");
			standaloneNumberProperty = serializedObject.FindProperty("m_standaloneNumber");
			standaloneNameProperty = serializedObject.FindProperty("m_standaloneName");
			floorProperty = serializedObject.FindProperty("m_floor");
			roomProperty = serializedObject.FindProperty("m_room");
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

		private void DrawHandle()
		{
			Color color = GUI.color;

			GUI.color = Color.red;
			positionProperty.vector3Value = Handles.PositionHandle(positionProperty.vector3Value, Quaternion.identity);

			string positionLabel = (!useDisplayPositionProperty.boolValue ? "[" + room.displayedName + "]" : "[Position]");
			Handles.Label(positionProperty.vector3Value, positionLabel);
			
			if(useDisplayPositionProperty.boolValue)
			{
				GUI.color = Color.magenta;
				displayPositionProperty.vector3Value = Handles.PositionHandle(displayPositionProperty.vector3Value, Quaternion.identity);
				Handles.Label(displayPositionProperty.vector3Value, "[" + room.displayedName + "]");
			}
			else
				displayPositionProperty.vector3Value = positionProperty.vector3Value;

			GUI.color = color;	
		}

		private void DrawCustomInspector()
		{
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.LabelField("Location", EditorStyles.boldLabel);

			string labelString = (standaloneNameProperty.boolValue ? "Room Name" : "Building Name");
			GUIContent label = new GUIContent(labelString);
			EditorGUILayout.PropertyField(displayedNameProperty, label);

			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(standaloneNameProperty);
			EditorGUILayout.PropertyField(mapNameProperty);
				
			EditorGUI.indentLevel--;

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

			if(!standaloneNameProperty.boolValue)
			{
				EditorGUILayout.Space();
				EditorGUILayout.LabelField("Room", EditorStyles.boldLabel);

				EditorGUILayout.PropertyField(standaloneNumberProperty);
				floorProperty.intValue = Mathf.Max(EditorGUILayout.IntField("Floor", floorProperty.intValue), 0);
				roomProperty.intValue = Mathf.Clamp(EditorGUILayout.IntField("Room", roomProperty.intValue), 1, 99);
			}

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(tagsProperty);

			EditorGUILayout.HelpBox(room.displayedName, MessageType.Info);

			if(EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();
		}
		#endregion
	}
	#endif
}