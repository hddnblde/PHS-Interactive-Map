#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using PampangaHighSchool.Faculty;

public class FacultyTool : EditorWindow
{
	[SerializeField]
	private string assetPath = "Assets\\Scriptable Objects\\Faculty";

	[SerializeField]
	private TextAsset teacherList = null;

	[SerializeField]
	private Department department = Department.Aralin;

	private SerializedObject serializedObject = null;
	private SerializedProperty assetPathProperty = null;
	private SerializedProperty teacherListProperty = null;
	private SerializedProperty departmentProperty = null;


	[MenuItem("Tools/Faculty Generator")]
	private static void OpenWindow()
	{
		FacultyTool facultyLoader = (FacultyTool)EditorWindow.GetWindow(typeof(FacultyTool));
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
		Initialize();
		DrawInspector();
	}

	private void Initialize()
	{
		if(serializedObject != null)
			return;
		
		serializedObject = new SerializedObject(this);
		assetPathProperty = serializedObject.FindProperty("assetPath");
		teacherListProperty = serializedObject.FindProperty("teacherList");
		departmentProperty = serializedObject.FindProperty("department");
	}

	private void LoadPrefs()
	{
		assetPath = EditorPrefs.GetString("FacultyGenerator_AssetPath", "Assets\\Scriptable Objects\\Faculty");
		department = (Department)EditorPrefs.GetInt("FacultyGenerator_Department", 0);
	}

	private void SavePrefs()
	{
		EditorPrefs.SetString("FacultyGenerator_AssetPath", assetPath);
		EditorPrefs.SetInt("FacultyGenerator_Department", (int)department);
	}

	private void DrawInspector()
	{
		EditorGUILayout.LabelField("References", EditorStyles.boldLabel);

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(assetPathProperty);
		EditorGUILayout.PropertyField(teacherListProperty);
		EditorGUILayout.PropertyField(departmentProperty);

		if(EditorGUI.EndChangeCheck())
			serializedObject.ApplyModifiedProperties();

		EditorGUILayout.Space();
		
		if(GUILayout.Button("Generate"))
			GenerateTeacherFromList();
	}

	private void GenerateTeacherFromList()
	{
		if(!Directory.Exists(assetPath) || teacherList == null)
		{
			Debug.Log("Failed to generate teacher list.");
			return;
		}

		string[] teachers = teacherList.text.Split("\n".ToCharArray());

		foreach(string teacher in teachers)
		{
			string[] names = teacher.TrimEnd(System.Environment.NewLine.ToCharArray()).TrimEnd('\t').Replace(", ", "*").Split("*".ToCharArray());
			
			if(names == null || names.Length != 2)
			{
				Debug.Log("Failed to create teacher.");
				continue;
			}

			CreateTeacher(names[1], names[0], department, assetPath);
		}
	}

	private void CreateTeacher(string firstName, string lastName, Department department, string path)
	{
		if(!Directory.Exists(path))
			// AssetDatabase.CreateFolder(path, departmentProperty.displayName);
			return;
		
		Teacher teacher = ScriptableObject.CreateInstance<Teacher>();
		SerializedObject teacherObject = new SerializedObject(teacher);
		SerializeTeacher(teacherObject, firstName, lastName, department);

		string assetName = "@last, @first.asset".Replace("@last", lastName).Replace("@first", firstName);
		string assetPath = path + '\\' + assetName;

		AssetDatabase.CreateAsset(teacher, assetPath);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	private void SerializeTeacher(SerializedObject serializedObject, string firstName, string lastName, Department department)
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