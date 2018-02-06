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

		[SerializeField]
		private Room m_inheritedPosition = null;

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

		public override Vector3 position
		{
			get
			{
				if(m_inheritedPosition == null)
					return base.position;
				else
					return m_inheritedPosition.position;
			}
		}

		public override Vector3 displayPosition
		{
			get
			{
				if(m_inheritedPosition == null)
					return base.displayPosition;
				else
					return m_inheritedPosition.displayPosition;
			}
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
		roomProperty = null,
		inheritedPositionProperty = null;
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
			inheritedPositionProperty = serializedObject.FindProperty("m_inheritedPosition");
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

			bool hasInheritedPosition = inheritedPositionProperty.objectReferenceValue != null;

			positionProperty.vector3Value = Handles.PositionHandle(GetInheritedPosition(), Quaternion.identity);

			string positionLabel = (!useDisplayPositionProperty.boolValue ? "[" + room.displayedName + "]" : "[Position]") + (HasInheritedPosition() ? "\ninherited" : "");
			Handles.Label(positionProperty.vector3Value, positionLabel);
			
			if(useDisplayPositionProperty.boolValue)
			{
				GUI.color = Color.magenta;

				displayPositionProperty.vector3Value = Handles.PositionHandle(GetInheritedDisplayPosition(), Quaternion.identity);

				Handles.Label(displayPositionProperty.vector3Value, "[" + room.displayedName + "]" + (HasInheritedPosition() ? "\ninherited" : ""));
			}
			else
				displayPositionProperty.vector3Value = positionProperty.vector3Value;

			GUI.color = color;
		}

		private bool HasInheritedPosition()
		{
			return inheritedPositionProperty.objectReferenceValue != null;
		}

		private Vector3 GetInheritedPosition()
		{
			if(!HasInheritedPosition())
				return positionProperty.vector3Value;

			SerializedObject inheritedRoom = new SerializedObject(inheritedPositionProperty.objectReferenceValue);
			SerializedProperty inheritedPosition = inheritedRoom.FindProperty("m_position");

			return inheritedPosition.vector3Value;
		}

		private Vector3 GetInheritedDisplayPosition()
		{
			if(inheritedPositionProperty.objectReferenceValue == null)
				return displayPositionProperty.vector3Value;

			SerializedObject inheritedRoom = new SerializedObject(inheritedPositionProperty.objectReferenceValue);
			SerializedProperty inheritedDisplayPosition = inheritedRoom.FindProperty("m_displayPosition");

			return inheritedDisplayPosition.vector3Value;
		}

		private void DrawCustomInspector()
		{
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.LabelField("Location", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(inheritedPositionProperty);

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