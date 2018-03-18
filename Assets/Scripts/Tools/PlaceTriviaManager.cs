#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Map;
using System.IO;

public class PlaceTriviaManager : EditorWindow
{
	[MenuItem("Tools/Trivia Manager")]
	private static void ShowWindow()
	{
		PlaceTriviaManager window = (PlaceTriviaManager)EditorWindow.GetWindow(typeof(PlaceTriviaManager));
		window.Show();
	}

	[SerializeField]
	private string triviasPath = "";
	private SerializedObject serializedObject = null;
	private SerializedProperty triviasPathProperty = null;	

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
		DrawGUI();
	}

	private void LoadPrefs()
	{
		triviasPath = EditorPrefs.GetString("PlaceTrivia_Path", "Assets\\Scriptable Objects\\Place Trivias");
	}

	private void SavePrefs()
	{
		EditorPrefs.SetString("PlaceTrivia_Path", triviasPath);
	}

	private void Initialize()
	{
		if(serializedObject == null)
		{
			serializedObject = new SerializedObject(this);
			triviasPathProperty = serializedObject.FindProperty("triviasPath");
		}
	}

	private void DrawGUI()
	{
		EditorGUI.BeginChangeCheck();

		EditorGUILayout.PropertyField(triviasPathProperty);
		
		if(EditorGUI.EndChangeCheck())
			serializedObject.ApplyModifiedProperties();

		if(GUILayout.Button("Locate Places"))
			LocatePlaces();
	}

	private void LocatePlaces()
	{
		if(!Directory.Exists(triviasPath))
		{
			Debug.Log("Cannot locate trivias.");
			return;
		}

		string[] triviaPaths = Directory.GetFiles(triviasPath, "*.asset");

		foreach(string triviaPath in triviaPaths)
		{
			PlaceTrivia trivia = AssetDatabase.LoadAssetAtPath<PlaceTrivia>(triviaPath);

			if(trivia == null)
			{
				Debug.Log("Failed to load " + triviaPath);
				continue;
			}

			if(!LocatePlace(trivia))
				Debug.Log("Failed to locate place for " + trivia.name);

		}
	}
	
	private bool LocatePlace(PlaceTrivia trivia)
	{
		if(trivia == null)
			return false;

		string filter = "\"" + trivia.name.Replace(" Trivia", "") + "\" t: Place";
		string[] GUIDResult = AssetDatabase.FindAssets(filter);

		if(GUIDResult == null || GUIDResult.Length == 0)
		{
			Debug.Log("Could not find " + filter);
			return false;
		}

		string path = AssetDatabase.GUIDToAssetPath(GUIDResult[0]);
		Place place = AssetDatabase.LoadAssetAtPath<Place>(path);

		if(place == null)
		{
			Debug.Log("Failed : " + path);
			return false;
		}

		return SetTrivia(place, trivia);
	}

	private bool SetTrivia(Place place, PlaceTrivia trivia)
	{
		if(place == null || trivia == null)
			return false;

		SerializedObject serializedObject = new SerializedObject(place);
		SerializedProperty triviaProperty = serializedObject.FindProperty("m_trivia");
		
		triviaProperty.objectReferenceValue = trivia as Object;
		serializedObject.ApplyModifiedPropertiesWithoutUndo();

		return true;
	}
}
#endif