using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Map
{
	[CreateAssetMenu(menuName = "Map/Room", order = 2, fileName = "Room")]
	public class Room : Location
	{
		[SerializeField]
		private bool m_oneStoryOnly = false;

		[SerializeField]
		private int m_floor = 1;

		[SerializeField]
		private int m_room = 1;

		public override string displayedName
		{
			get { return FloorNamingConvention(); }
		}

		private string FloorNamingConvention()
		{
			string baseName = base.displayedName + ' ';

			if(m_oneStoryOnly)
				return baseName + m_room.ToString();
			else
				return baseName + m_floor.ToString() + m_room.ToString("00");
		}
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(Room))]
	public class RoomEditor : Editor
	{
		#region Fields
		Room room = null;
		private SerializedProperty
		displayedNameProperty = null,
		positionProperty = null,
		tagsProperty = null,
		oneStoryOnlyProperty = null,
		floorProperty = null,
		roomProperty = null;
		#endregion


		#region Editor Implementation
		private void OnEnable()
		{
			Initialize();
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
			positionProperty = serializedObject.FindProperty("m_position");
			tagsProperty = serializedObject.FindProperty("m_tags");
			oneStoryOnlyProperty = serializedObject.FindProperty("m_oneStoryOnly");
			floorProperty = serializedObject.FindProperty("m_floor");
			roomProperty = serializedObject.FindProperty("m_room");
		}

		private void DrawCustomInspector()
		{
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.LabelField("Location", EditorStyles.boldLabel);

			GUIContent label = new GUIContent("Building Name");
			EditorGUILayout.PropertyField(displayedNameProperty, label);
			EditorGUILayout.PropertyField(positionProperty);

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Room", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(oneStoryOnlyProperty);

			if(!oneStoryOnlyProperty.boolValue)
				floorProperty.intValue = Mathf.Max(EditorGUILayout.IntField("Floor", floorProperty.intValue), 1);
			
			roomProperty.intValue = Mathf.Clamp(EditorGUILayout.IntField("Room", roomProperty.intValue), 1, 99);

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(tagsProperty);

			if(EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();
		}
		#endregion
	}
	#endif
}