using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Schedules;
using PampangaHighSchool.Students;

#if UNITY_EDITOR
using UnityEditor;
using System.Text;
using System.IO;
#endif

namespace Databases
{
	[DisallowMultipleComponent]
	public class ClassScheduleDatabase : MonoBehaviour
	{
		#region Data Structure
		[System.Serializable]
		private class GradeGroup
		{
			public GradeGroup(Grade grade)
			{
				m_grade = grade;
				m_sections = new List<SectionGroup>();
			}

			[SerializeField]
			private Grade m_grade = Grade.Grade7;
			
			[SerializeField]
			private List<SectionGroup> m_sections = new List<SectionGroup>();

			public Grade grade
			{
				get { return m_grade; }
			}

			public int sectionCount
			{
				get
				{
					if(m_sections == null)
						return 0;
					else
						return m_sections.Count;
				}
			}

			public SectionGroup GetSectionGroup(int index)
			{
				if(index < 0 || m_sections == null || index >= m_sections.Count)
					return null;
				else
					return m_sections[index];
			}

			public SectionGroup GetSectionGroup(Section section)
			{
				if(m_sections == null || m_sections.Count == 0)
					return null;
					
				SectionGroup sectionGroup = null;

				foreach(SectionGroup s in m_sections)
				{
					if(s.section == section)
					{
						sectionGroup = s;
						break;
					}
				}

				return sectionGroup;
			}

			public void AddSectionGroup(SectionGroup sectionGroup)
			{
				if(!m_sections.Contains(sectionGroup))
					m_sections.Add(sectionGroup);
			}
		}

		[System.Serializable]
		private class SectionGroup
		{
			public SectionGroup(Section section, List<Schedule> schedules)
			{
				m_section = section;
				m_schedules = schedules;
			}

			[SerializeField]
			private Section m_section = null;

			[SerializeField]
			private List<Schedule> m_schedules = new List<Schedule>();

			public int scheduleCount
			{
				get
				{
					if(m_schedules == null)
						return 0;
					else
						return m_schedules.Count;
				}
			}

			public Section section
			{
				get { return m_section; }
			}

			public Schedule GetSchedule(int index)
			{
				if(index < 0 || m_schedules == null || index >= m_schedules.Count)
					return null;
				else
					return m_schedules[index];
			}
		}
		#endregion


		#region Serialized Field
		[SerializeField]
		private List<GradeGroup> gradesGroup = new List<GradeGroup>();
		#endregion


		#region Hidden Fields
		private static ClassScheduleDatabase instance = null;
		#endregion


		#region MonoBehaviour Implementation
		private void OnEnable()
		{
			InitializeSingleton();
		}

		private void OnDisable()
		{
			UninitializeSingleton();
		}
		#endregion


		#region Initializers
		private void InitializeSingleton()
		{
			if(instance == null)
				instance = this;
			else
				Destroy(gameObject);
		}

		private void UninitializeSingleton()
		{
			if(instance == this)
				instance = null;
		}
		#endregion		


		#region Functions
		public static string[] GetGradeItems()
		{
			if(instance == null)
				return null;
			else
				return instance.Internal_GetGradeItems();
		}

		public static string[] GetSectionItems(Grade grade)
		{
			if(instance == null)
				return null;
			else
				return instance.Internal_GetSectionItems(grade);
		}

		public static string[] GetScheduleItems(Grade grade, int section)
		{
			if(instance == null)
				return null;
			else
				return instance.Internal_GetScheduleItems(grade, section);
		}

		public static Schedule GetSchedule(Grade grade, int section, int index)
		{
			if(instance == null)
				return null;
			else
				return instance.Internal_GetSchedule(grade, section, index);
		}
		
		private string[] Internal_GetGradeItems()
		{
			if(gradesGroup == null || gradesGroup.Count == 0)
					return null;

				List<string> grades = new List<string>();

				foreach(GradeGroup grade in gradesGroup)
					grades.Add("Grade " + grade.grade);

				return grades.ToArray();
		}

		private string[] Internal_GetSectionItems(Grade grade)
		{
			GradeGroup gradeGroup = GetGradeGroup(grade);

			if(gradeGroup == null)
				return null;

			List<string> sections = new List<string>();

			for(int i = 0; i < gradeGroup.sectionCount; i++)
				sections.Add(gradeGroup.GetSectionGroup(i).section.name);

			return sections.ToArray();
		}

		private string[] Internal_GetScheduleItems(Grade grade, int section)
		{
			SectionGroup sectionGroup = GetSectionGroup(grade, section);

			if(sectionGroup == null)
				return null;

			List<string> schedules = new List<string>();

			for(int i = 0; i < sectionGroup.scheduleCount; i++)
				schedules.Add(sectionGroup.GetSchedule(i).name.Replace(" Schedule", ""));
			
			return schedules.ToArray();
		}

		private Schedule Internal_GetSchedule(Grade grade, int section, int index)
		{
			SectionGroup sectionGroup = GetSectionGroup(grade, section);

			if(sectionGroup == null)
				return null;
			else
				return sectionGroup.GetSchedule(index);
		}
		#endregion


		#region Helpers
		private GradeGroup GetGradeGroup(Grade grade)
		{
			if(gradesGroup == null || gradesGroup.Count == 0)
				return null;

			GradeGroup gradeGroup = null;

			foreach(GradeGroup g in gradesGroup)
			{
				if(g.grade == grade)
				{
					gradeGroup = g;
					break;
				}
			}

			return gradeGroup;
		}

		private SectionGroup GetSectionGroup(Grade grade, int section)
		{
			GradeGroup gradeGroup = GetGradeGroup(grade);

			if(gradeGroup == null)
				return null;

			SectionGroup sectionGroup = gradeGroup.GetSectionGroup(section);
			return sectionGroup;
		}

		private SectionGroup GetSectionGroup(Grade grade, Section section)
		{
			GradeGroup gradeGroup = GetGradeGroup(grade);

			if(gradeGroup == null)
				return null;

			SectionGroup sectionGroup = gradeGroup.GetSectionGroup(section);
			return sectionGroup;
		}

		public void ResetSchedule()
		{
			gradesGroup.Clear();
			List<Grade> grades = new List<Grade> {Grade.Grade7, Grade.Grade8, Grade.Grade9, Grade.Grade10, Grade.Grade11, Grade.Grade12};
			
			foreach(Grade grade in grades)
			{
				GradeGroup gradeGroup = new GradeGroup(grade);
				gradesGroup.Add(gradeGroup);
			}
		}

		public void AddSchedule(Grade grade, Section section, List<Schedule> schedules)
		{
			GradeGroup gradeGroup = GetGradeGroup(grade);			
			SectionGroup sectionGroup = new SectionGroup(section, schedules);

			if(gradeGroup != null)
				gradeGroup.AddSectionGroup(sectionGroup);
			else
				Debug.Log("Failed to add schedule.");
		}
		#endregion
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(ClassScheduleDatabase))]
	public class ClassScheduleDatabaseEditor : Editor
	{
		private ClassScheduleDatabase classScheduleDatabase = null;
		private SerializedProperty gradesListProperty = null;
		private string sectionsPath;
		private bool foldout = false;

		private void OnEnable()
		{
			Initialize();
			LoadPrefs();
		}

		public override void OnInspectorGUI()
		{
			DrawCustomGUI();
		}

		private void LoadPrefs()
		{
			sectionsPath = EditorPrefs.GetString("ClassSchedule_SectionsPath", "Assets/Scriptable Objects/Sections");
		}

		private void SavePrefs()
		{
			EditorPrefs.SetString("ClassSchedule_SectionsPath", sectionsPath);
		}

		private void Initialize()
		{
			classScheduleDatabase = target as ClassScheduleDatabase;
			gradesListProperty = serializedObject.FindProperty("gradesGroup");
		}

		private void DrawCustomGUI()
		{
			EditorGUI.BeginChangeCheck();

			if(gradesListProperty.arraySize == 0)
			{
				EditorGUILayout.HelpBox("Table is empty. Please load data from assets. Make sure that all paths below are correct.", MessageType.Warning);
				return;
			}

			foldout = EditorGUILayout.Foldout(foldout, "Grades");

			if(foldout)
			{
				EditorGUI.indentLevel++;

				for(int i = 0; i < gradesListProperty.arraySize; i++)
				{
					SerializedProperty gradeGroupProperty = gradesListProperty.GetArrayElementAtIndex(i);
					SerializedProperty gradeProperty = gradeGroupProperty.FindPropertyRelative("m_grade");
					string displayedName = gradeProperty.enumDisplayNames[gradeProperty.enumValueIndex];
					EditorGUILayout.PropertyField(gradeGroupProperty, new GUIContent(displayedName));
					
					if(!gradeGroupProperty.isExpanded)
						continue;

					SerializedProperty sectionsProperty = gradeGroupProperty.FindPropertyRelative("m_sections");
					if(sectionsProperty.arraySize == 0)
						continue;
					
					EditorGUI.indentLevel++;
					for(int j = 0; j < sectionsProperty.arraySize; j++)
					{
						SerializedProperty sectionGroupProperty = sectionsProperty.GetArrayElementAtIndex(j);
						SerializedProperty sectionProperty = sectionGroupProperty.FindPropertyRelative("m_section");
						int sectionCount = sectionGroupProperty.FindPropertyRelative("m_schedules").arraySize;

						if(sectionProperty.objectReferenceValue == null)
							continue;

						string sectionName = sectionProperty.objectReferenceValue.name;

						if(sectionCount > 1)
							sectionName += " [@count]".Replace("@count", sectionCount.ToString());

						EditorGUILayout.LabelField(new GUIContent(sectionName));
					}
					EditorGUI.indentLevel--;
				}
				EditorGUI.indentLevel--;
			}

			sectionsPath = EditorGUILayout.TextField(new GUIContent("Sections"), sectionsPath);

			if(GUILayout.Button("Load Schedules"))
			{
				List<Grade> grades = new List<Grade> {Grade.Grade7, Grade.Grade8, Grade.Grade9, Grade.Grade10, Grade.Grade11, Grade.Grade12};
				CreateSchedule(grades);
			}

			if(EditorGUI.EndChangeCheck())
				SavePrefs();
		}

		private void CreateSchedule(List<Grade> grades)
		{
			classScheduleDatabase.ResetSchedule();
			List<string> sections = null;

			foreach(Grade g in grades)
			{
				GetSections(g, out sections);
				AddSchedule(g, sections);
			}
		}

		private void GetSections(Grade grade, out List<string> sections)
		{
			sections = new List<string>();
			string path = sectionsPath + '/' + "Grade " + (int)grade;

			if(!Directory.Exists(path))
				return;
			
			string[] sectionPaths = Directory.GetFiles(path, "*.asset");

			foreach(string sectionPath in sectionPaths)
				sections.Add(sectionPath.Replace(path, "").Replace(".asset", ""));
		}

		private void AddSchedule(Grade grade, List<string> sections)
		{
			if(sections == null || sections.Count == 0)
				return;

			foreach(string sectionName in sections)
				AddSchedule(grade, sectionName);
		}

		private void AddSchedule(Grade grade, string sectionName)
		{
			sectionName = sectionName.Replace("\\", "");
			Section section;
			List<Schedule> schedules;
			Debug.Log(sectionName);
			GetSchedules(sectionName, out section, out schedules);
			classScheduleDatabase.AddSchedule(grade, section, schedules);
			serializedObject.ApplyModifiedProperties();
			serializedObject.Update();
		}

		private void GetSchedules(string name, out Section section, out List<Schedule> schedules)
		{
			section = null;
			schedules = new List<Schedule>();
			string[] resultGUID = null;

			// Find Section
			resultGUID = AssetDatabase.FindAssets(name + " t:Section");
			string sectionPath = (resultGUID.Length > 0 ? AssetDatabase.GUIDToAssetPath(resultGUID[0]) : "");
			section = AssetDatabase.LoadAssetAtPath(sectionPath, typeof(Section)) as Section;

			// Find Schedules
			resultGUID = AssetDatabase.FindAssets(name + " t:Schedule");
			foreach(string scheduleGUID in resultGUID)
			{
				string path = AssetDatabase.GUIDToAssetPath(scheduleGUID);
				Schedule schedule = AssetDatabase.LoadAssetAtPath(path, typeof(Schedule)) as Schedule;
				schedules.Add(schedule);
			}
		}
	}
	#endif
}