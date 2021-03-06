﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace PampangaHighSchool.Students
{
	public enum Grade
	{
		Grade7 = 7,
		Grade8 = 8,
		Grade9 = 9,
		Grade10 = 10,
		Grade11 = 11,
		Grade12 = 12
	}

	[CreateAssetMenu(menuName = "Students/Section", order = 0, fileName = "Section")]
	public class Section : ScriptableObject
	{
		[Header("Description")]
		[SerializeField]
		private Grade m_grade = Grade.Grade7;

		[SerializeField]
		private int m_order = 1;

		[SerializeField]
		private int m_count = 1;

		public Grade grade
		{
			get { return m_grade; }
		}

		public int order
		{
			get { return m_order; }
		}

		public int count
		{
			get { return m_count; }
		}
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(Section))]
	public class SectionEditor : Editor
	{
		private bool hasSpaceBeforeNumber = true;
		private bool alwaysIncludeNumber = false;

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			DrawTool();
		}

		private void OnEnable()
		{
			LoadPrefs();
		}

		private void OnDisable()
		{
			SavePrefs();
		}

		private void LoadPrefs()
		{
			hasSpaceBeforeNumber = EditorPrefs.GetBool("Section_HasSpaceBeforeNumber", true);
			alwaysIncludeNumber = EditorPrefs.GetBool("Section_AlwaysIncludeNumber", false);
		}

		private void SavePrefs()
		{
			EditorPrefs.SetBool("Section_HasSpaceBeforeNumber", hasSpaceBeforeNumber);
			EditorPrefs.SetBool("Section_AlwaysIncludeNumber", alwaysIncludeNumber);
		}

		private void DrawTool()
		{
			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Naming Options", EditorStyles.boldLabel);
			hasSpaceBeforeNumber = EditorGUILayout.Toggle("Has Space Before Number", hasSpaceBeforeNumber);
			alwaysIncludeNumber = EditorGUILayout.Toggle("Always Include Number", alwaysIncludeNumber);
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			if(GUILayout.Button("Generate Classes"))
			{
				SerializedProperty countProperty = serializedObject.FindProperty("m_count");

				if(countProperty == null)
					return;

				bool isMany = countProperty.intValue > 1 || alwaysIncludeNumber;

				for(int i = 0; i < countProperty.intValue; i++)
				{
					int rank = i + 1;
					string path = AssetDatabase.GetAssetPath(target).Replace(".asset", "/");
					string name = target.name + (hasSpaceBeforeNumber ? " " : "") + (isMany ? rank.ToString() : "");

					StudentClass studentClass = ScriptableObject.CreateInstance<StudentClass>();
					SetUpClass(new SerializedObject(studentClass), rank);
					
					if(!AssetDatabase.IsValidFolder(path))
						Directory.CreateDirectory(path);
						
					AssetDatabase.CreateAsset(studentClass, path + name + ".asset");
				}

				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
		}

		private void SetUpClass(SerializedObject studentClass, int rank)
		{
			SerializedProperty sectionProperty = studentClass.FindProperty("m_section");
			SerializedProperty rankProperty = studentClass.FindProperty("m_rank");

			sectionProperty.objectReferenceValue = target;
			rankProperty.intValue = rank;

			studentClass.ApplyModifiedPropertiesWithoutUndo();
		}
	}
	#endif
}