#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PampangaHighSchool.Students;
using System.IO;

public class ClassGenerator : EditorWindow
{
	[SerializeField]
	private List<Section> sections = new List<Section>();

	[MenuItem("Tools/Class Generator")]
	private static void Initialize()
	{
		ClassGenerator classGenerator = (ClassGenerator)EditorWindow.GetWindow(typeof(ClassGenerator));
		classGenerator.Show();
	}

	private void OnGUI()
	{
		SerializedObject target = new SerializedObject(this);
		SerializedProperty sections = target.FindProperty("sections");

		EditorGUI.BeginChangeCheck();

		EditorGUILayout.PropertyField(sections, true);
		
		if(EditorGUI.EndChangeCheck())
		{
			target.ApplyModifiedProperties();
			target.Update();
			return;
		}

		DrawGenerateButton();
	}

	private void DrawGenerateButton()
	{
		if(sections == null || sections.Count == 0)
			return;

		if(!GUILayout.Button("Generate Classes"))
			return;

		foreach(Section section in sections)
			GenerateClass(section);
	}

	private void GenerateClass(Section section)
	{
		if(section == null)
			return;
		
		bool isMany = section.count > 1;

		for(int i = 0; i < section.count; i++)
		{
			int rank = i + 1;
			string path = AssetDatabase.GetAssetPath(section).Replace(".asset", "/");
			string name = section.name + (isMany ? ' ' + rank.ToString() : "");

			StudentClass studentClass = ScriptableObject.CreateInstance<StudentClass>();
			SetUpClass(section, new SerializedObject(studentClass), rank);
			
			if(!AssetDatabase.IsValidFolder(path))
				Directory.CreateDirectory(path);
				
			AssetDatabase.CreateAsset(studentClass, path + name + ".asset");
		}

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	private void SetUpClass(Section section, SerializedObject studentClass, int rank)
	{
		SerializedProperty sectionProperty = studentClass.FindProperty("m_section");
		SerializedProperty rankProperty = studentClass.FindProperty("m_rank");

		sectionProperty.objectReferenceValue = section;
		rankProperty.intValue = rank;

		studentClass.ApplyModifiedPropertiesWithoutUndo();
	}
}
#endif