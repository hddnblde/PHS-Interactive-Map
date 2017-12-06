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
		private int m_floor = 1;

		public int floor
		{
			get { return m_floor; }
		}
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(Room))]
	public class RoomEditor : Editor
	{
		#region Fields
		private SerializedProperty
		displayedNameProperty = null,
		positionProperty = null,
		floorProperty = null;
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
			displayedNameProperty = serializedObject.FindProperty("m_displayedName");
			positionProperty = serializedObject.FindProperty("m_position");
			floorProperty = serializedObject.FindProperty("m_floor");
		}

		private void DrawCustomInspector()
		{
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.LabelField("Location", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(displayedNameProperty);
			EditorGUILayout.PropertyField(positionProperty);

			floorProperty.intValue = Mathf.Max(EditorGUILayout.IntField("Floor", floorProperty.intValue), 1);

			if(EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();
		}
		#endregion
	}
	#endif
}