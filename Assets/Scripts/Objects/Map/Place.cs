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
					return m_landmark.hasRooms;
			}
		}

		public void Search(string keyword)
		{
			if(!Application.isPlaying)
				return;

			if(displayedName.Contains(keyword) || HasTag(keyword))
				MapTable.AddLocation(this as Location);
			
			if(hasRooms)
			{
				foreach(Room room in rooms)
				{
					if(room == null)
						continue;
					
					if(room.displayedName.Contains(keyword) || room.HasTag(keyword))
						MapTable.AddLocation(room as Location);
				}
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
		}

		public override void OnInspectorGUI()
		{
			DrawCustomInspector();
			DrawTools();
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
				Debug.Log(serializedObject.FindProperty("m_rooms[0].m_floor").intValue);

//				for(int i = 0; i < roomsProperty.arraySize; i++)
//				{
//					SerializedProperty room = roomsProperty.GetArrayElementAtIndex(i);
//					Debug.Log(room.serializedObject.FindProperty(");
//
////					SerializedProperty buildingName = room.FindPropertyRelative("m_floor");
//////					Debug.Log(buildingName.serializedObject.targetObject.name);
////					if(buildingName != null)
////						buildingName.stringValue = target.name;
////					else
////						Debug.Log("Failed");
//				}
			}
		}
		#endregion
	}
	#endif
}