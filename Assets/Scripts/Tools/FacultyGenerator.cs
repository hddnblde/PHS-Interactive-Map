#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using PampangaHighSchool.Faculty;

public class FacultyGenerator : EditorWindow
{
	private static string listPath;
	private static string scriptableObjectsPath;
	private static Department department = Department.Aralin;

	[MenuItem("Tools/Faculty Generator")]
	private static void Initialize()
	{
		FacultyGenerator facultyLoader = (FacultyGenerator)EditorWindow.GetWindow(typeof(FacultyGenerator));
		facultyLoader.Show();
	}

	private void Awake()
	{
		LoadPrefs();
	}

	private void OnDestroy()
	{
		SavePrefs();
	}

	private void OnGUI()
	{
		DrawPath();
		DrawButton();
	}

	private void LoadPrefs()
	{
		listPath = EditorPrefs.GetString("FacultyLoader_ListPath", "");
		scriptableObjectsPath = EditorPrefs.GetString("FacultyLoader_ScriptableObjectsPath", "Scriptable Objects\\Faculty\\");
		department = (Department)EditorPrefs.GetInt("FacultyLoader_Department", 0);
	}

	private void SavePrefs()
	{
		EditorPrefs.SetString("FacultyLoader_ListPath", listPath);
		EditorPrefs.SetString("FacultyLoader_ScriptableObjectsPath", scriptableObjectsPath);
		EditorPrefs.SetInt("FacultyLoader_Department", (int)department);
	}

	private void DrawPath()
	{
		department = (Department)EditorGUILayout.EnumPopup((System.Enum)department);
		EditorGUILayout.LabelField("Paths", EditorStyles.boldLabel);
		listPath = EditorGUILayout.TextField("Root List Path", listPath);
		scriptableObjectsPath = EditorGUILayout.TextField("Scriptable Objects Path", scriptableObjectsPath);
	}

	private void DrawButton()
	{
		if(GUILayout.Button("Load"))
		{
			string targetRootListPath = listPath + '\\' + department.ToString();
			string targetScriptableObjectsPath = scriptableObjectsPath + '\\' + department.ToString();
			if(!Directory.Exists(targetRootListPath) || !Directory.Exists(targetScriptableObjectsPath))
			{
				Debug.Log("Failed to load faculty list!");
				return;
			}

			string[] files = Directory.GetFiles(targetRootListPath, "*.pdf");

			if(files == null || files.Length == 0)
			{
				Debug.Log("Failed to load faculty list!");
				return;
			}

			foreach(string file in files)
			{
				string name = file.Replace(".pdf", "").Replace(department.ToString() + " - ", "").Replace(targetRootListPath + "\\", "");
				string[] names = name.Replace(", ", "*").Split("*".ToCharArray());

				if(names == null || names.Length != 2)
				{
					Debug.Log("Failed to create teacher.");
					continue;
				}

				Teacher teacher = ScriptableObject.CreateInstance<Teacher>();
				SetTeacherName(new SerializedObject(teacher), names[1], names[0]);
				string assetPath = Application.dataPath.Replace("/", "\\");
				assetPath = targetScriptableObjectsPath.Replace(assetPath + '\\', "");
				AssetDatabase.CreateAsset(teacher, "Assets/" + assetPath.Replace('\\', '/') + '/' + name + ".asset");
			}
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
	}

	private void SetTeacherName(SerializedObject serializedObject, string firstName, string lastName)
	{
		SerializedProperty firstNameProperty = serializedObject.FindProperty("m_firstName");
		SerializedProperty lastNameProperty = serializedObject.FindProperty("m_lastName");
		SerializedProperty middleNameProperty = serializedObject.FindProperty("m_middleName");
		SerializedProperty departmentProperty = serializedObject.FindProperty("m_department");

		firstNameProperty.stringValue = firstName;
		lastNameProperty.stringValue = lastName;
		middleNameProperty.stringValue = "";
		departmentProperty.enumValueIndex = (int)department;

		serializedObject.ApplyModifiedPropertiesWithoutUndo();
	}
}
#endif