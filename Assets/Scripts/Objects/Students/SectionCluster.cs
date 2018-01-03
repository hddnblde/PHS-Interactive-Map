using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Faculty;
using Map;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Students
{
	[CreateAssetMenu(menuName = "Students/Section Cluster", order = 1, fileName = "Section Cluster")]
	public class SectionCluster : ScriptableObject
	{
		[Header("Description")]
		[SerializeField]
		private Section m_section = null;

		[SerializeField]
		private int m_rank = 1;

		[Header("Advisory")]
		[SerializeField]
		private Teacher m_adviser = null;

		[SerializeField]
		private Room m_room = null;

		public Section section
		{
			get { return m_section; }
		}

		public int rank
		{
			get { return m_rank; }
		}
	
		public Teacher adviser
		{
			get { return m_adviser; }
		}
	
		public Room advisoryRoom
		{
			get { return m_room; }
		}
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(SectionCluster))]
	public class SectionClusterEditor : Editor
	{
		private SerializedProperty
		sectionProperty = null,
		rankProperty = null,
		adviserProperty = null,
		roomProperty = null;

		private void OnEnable()
		{
			Initialize();
		}

		public override void OnInspectorGUI()
		{
			DrawCustomInspector();
		}

		private void Initialize()
		{
			sectionProperty = serializedObject.FindProperty("m_section");
			rankProperty = serializedObject.FindProperty("m_rank");
			adviserProperty = serializedObject.FindProperty("m_adviser");
			roomProperty = serializedObject.FindProperty("m_room");
		}

		private void DrawCustomInspector()
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(sectionProperty);

			int totalRanks = 1;

			if(sectionProperty.objectReferenceValue != null)
			{
				SerializedObject countObject = new SerializedObject(sectionProperty.objectReferenceValue);
				SerializedProperty countProperty = countObject.FindProperty("m_count");
				if(countProperty != null)
					totalRanks = countProperty.intValue;

				rankProperty.intValue = Mathf.Clamp(EditorGUILayout.IntField(rankProperty.displayName, rankProperty.intValue), 1, totalRanks);
			}
			
			EditorGUILayout.PropertyField(adviserProperty);
			EditorGUILayout.PropertyField(roomProperty);

			if(EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();
		}
	}
	#endif
}
