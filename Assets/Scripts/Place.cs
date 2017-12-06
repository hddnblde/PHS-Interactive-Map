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
	
		public bool FindPlaces(string name, out Location[] places)
		{
			if(hasRooms)
				places = m_rooms.FindAll(l => l.displayedName.Contains(name)).ToArray();
			else
			{
				if(place.displayedName.Contains(name))
				{
					places = new Location[1];
					places[0] = place;
				}
				else
					places = null;
			}
			return places != null && places.Length > 0;
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
				GUIContent content = new GUIContent("Main Position");
				EditorGUILayout.PropertyField(positionProperty, content);
			}

			EditorGUILayout.PropertyField(landmarkProperty);

			if(location.hasRooms)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(roomsProperty, true);
				EditorGUI.indentLevel--;
			}
			
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
		}

		private void DrawTransformCopyTool()
		{
			
		}

		private void DrawNameCopyTool()
		{
			if(GUILayout.Button("Copy name from this asset."))
			{
				displayedNameProperty.stringValue = target.name;
				serializedObject.ApplyModifiedProperties();
			}
		}
		#endregion
	}
	#endif
}