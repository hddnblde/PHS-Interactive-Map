#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Schedules;
using PampangaHighSchool.Students;
using PampangaHighSchool.Faculty;
using System.IO;
using Map;

public class ScheduleGenerator : EditorWindow
{
	[SerializeField]
	private TextAsset textAsset = null;

	[SerializeField]
	private Schedule scheduleTemplate = null;

	[MenuItem("Tools/Schedule Generator")]
	private static void Initialize()
	{
		ScheduleGenerator scheduleGenerator = (ScheduleGenerator)EditorWindow.GetWindow(typeof(ScheduleGenerator));
		scheduleGenerator.Show();
	}

	private void OnGUI()
	{
		SerializedObject target = new SerializedObject(this);
		SerializedProperty textAssetProperty = target.FindProperty("textAsset");
		SerializedProperty scheduleTemplateProperty = target.FindProperty("scheduleTemplate");

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(textAssetProperty);
		EditorGUILayout.PropertyField(scheduleTemplateProperty);

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
		if(textAsset == null || string.IsNullOrEmpty(textAsset.text))
			return;

		if(GUILayout.Button("Generate Schedules"))
			GenerateScheduleFromTextAsset();
	}

	private void GenerateScheduleFromTextAsset()
	{
		if(textAsset == null)
			return;

		// const int sampledIndex = 7;

		string[] lines = textAsset.text.Split("\n".ToCharArray());
		for(int i = 0; i < lines.Length; i++)
		{
			// if(i != sampledIndex)
			// 	continue;
			
			string scheduleText = AssembleScheduleText(lines[i]);
			GenerateSchedule(scheduleText);
		}
	}

	private string AssembleScheduleText(string line)
	{
		string[] entries = line.Split("\t".ToCharArray());

		if(entries.Length < 5)
			return "";
		
		string period = entries[0];
		string timeIn = entries[1];
		string timeOut = entries[2];
		string grade = entries[3];
		string section = entries[4];

		string monday = AssembleSchedule(entries[5], entries[6], entries[7]);
		string tuesday = AssembleSchedule(entries[8], entries[9], entries[10]);
		string wednesday = AssembleSchedule(entries[11], entries[12], entries[13]);
		string thursday = AssembleSchedule(entries[14], entries[15], entries[16]);
		string friday = AssembleSchedule(entries[17], entries[18], entries[19]);

		string scheduleText = "@period (@timeIn - @timeOut)~Grade @grade~@section~@mon|@tue|@wed|@thu|@fri"
			.Replace("@period", period).Replace("@timeIn", timeIn).Replace("@timeOut", timeOut)
			.Replace("@grade", grade).Replace("@section", section).Replace("@mon", monday)
			.Replace("@tue", tuesday).Replace("@wed", wednesday).Replace("@thu", thursday)
			.Replace("@fri", friday);

		return scheduleText;
	}

	private string AssembleSchedule(string subject, string room, string teacher)
	{
		string message = "@subject/@room/@teacher"
			.Replace("@subject", subject)
			.Replace("@room", room).Replace("@teacher", teacher);
		
		return message;
	}

	private void GenerateSchedule(string scheduleText)
	{
		string[] items = scheduleText.Split("~".ToCharArray());

		if(items == null || items.Length < 4)
		{
			Debug.Log("Failed to generate schedule.");
			return;
		}

		int periodIndex = GetPeriod(items[0]);
		string grade = items[1];
		string section = items[2];
		string entry = items[3];
		
		Schedule schedule = GetSchedule(section, grade);
		ScheduleObject target = GetTarget(section, typeof(StudentClass).ToString());
		
		if(schedule == null)
		{
			Debug.Log("Failed to create schedule.");
			return;
		}
		SetSchedule(new SerializedObject(schedule), target, periodIndex, entry);
	}

	private int GetPeriod(string periodAndTimeText)
	{
		string[] items = periodAndTimeText.Split(" ".ToCharArray());
		int period = -1;

		if(items != null && items.Length > 0)
			int.TryParse(items[0], out period);

		return period - 1;
	}

	private void SetSchedule(SerializedObject schedule, ScheduleObject target, int index, string entryText)
	{
		SerializedProperty targetProperty = schedule.FindProperty("target");
		SerializedProperty periodsProperty = schedule.FindProperty("m_periods");

		targetProperty.objectReferenceValue = target;
		schedule.ApplyModifiedPropertiesWithoutUndo();

		if(index < 0 || index >= periodsProperty.arraySize)
			return;

		SerializedProperty prop = periodsProperty.GetArrayElementAtIndex(index);
		SerializedProperty entriesProperty = prop.FindPropertyRelative("m_entries");

		if(entriesProperty == null)
			return;

		string[] entryPerDay = entryText.Split("|".ToCharArray());

		for(int i = 0; i < entriesProperty.arraySize; i++)
		{
			SerializedProperty entry = entriesProperty.GetArrayElementAtIndex(i);
			Room room;
			ScheduleObject title;
			string subtitle;

			GetEntries(entryPerDay[i], out room, out title, out subtitle);			
			SetEntry(entry, room, title, subtitle);
		}
	}

	private void SetEntry(SerializedProperty entryProperty, Room room, ScheduleObject title, string subtitle)
	{
		SerializedProperty roomProperty = entryProperty.FindPropertyRelative("m_room");
		SerializedProperty titleProperty = entryProperty.FindPropertyRelative("m_title");
		SerializedProperty subtitleProperty = entryProperty.FindPropertyRelative("m_subtitle");
		
		roomProperty.objectReferenceValue = room as Object;
		titleProperty.objectReferenceValue = title as Object;
		subtitleProperty.stringValue = subtitle;
		entryProperty.serializedObject.ApplyModifiedPropertiesWithoutUndo();
		entryProperty.serializedObject.Update();
	}

	private void GetEntries(string entryText, out Room room, out ScheduleObject title, out string subtitle)
	{
		string[] items = entryText.Split("/".ToCharArray());
		subtitle = items[0].Replace("@empty", "");
		room = GetRoom(items[1]);
		title = GetTarget(FilterOutMiddleInitial(items[2]), typeof(Teacher).ToString());
	}

	private string FilterOutMiddleInitial(string name)
	{
		if(name.Contains("."))
			return name.TrimEnd(System.Environment.NewLine.ToCharArray()).TrimEnd('\t').Trim().TrimEnd().Substring(0, name.Length - 3);
		else
			return name;
	}

	private string CatchFormat(string name)
	{
		bool isPseudoTeacher = name.Contains("Teacher A") || name.Contains("Teacher B") || name.Contains("Teacher C") || name.Contains("Teacher D");
		if(!isPseudoTeacher)
			return name;
		else
			return name.TrimEnd('\n', '\r') + ",\"";
	}

	private Room GetRoom(string roomName)
	{
		if(string.IsNullOrEmpty(roomName))
			return null;
		
		string[] result = AssetDatabase.FindAssets(roomName + " t:" + typeof(Room).ToString());

		if(result == null || result.Length == 0)
			result = AssetDatabase.FindAssets("\"" + roomName + "\"" + " t:" + typeof(Room).ToString());

		if(result == null || result.Length == 0)
			return null;

		string assetPath = AssetDatabase.GUIDToAssetPath(result[0]);
		Room room = AssetDatabase.LoadAssetAtPath<Room>(assetPath);
		return room;
	}
	
	private Schedule GetSchedule(string section, string grade)
	{
		string[] result = AssetDatabase.FindAssets("\"" + section + " \"" + " t:" + typeof(Schedule).ToString());

		if(result == null || result.Length == 0)
		{
			ScheduleObject studentClass = GetTarget(section, typeof(StudentClass).ToString());

			if(studentClass == null)
			{
				Debug.Log("Failed to get " + section);
				return null;
			}

			return CreateNewSchedule(studentClass, grade);
		}

		string assetPath = AssetDatabase.GUIDToAssetPath(result[0]);
		Schedule schedule = AssetDatabase.LoadAssetAtPath<Schedule>(assetPath);
		return schedule;
	}

	private Schedule CreateNewSchedule(ScheduleObject target, string grade)
	{
		if(target == null)
		{
			Debug.Log("Failed to create new schedule because target is empty.");
			return null;
		}
		
		string path = "Assets/Scriptable Objects/Schedules/" + grade + '/';
		string name = target.name + " Schedule";

		Schedule schedule = (scheduleTemplate != null ? ScriptableObject.Instantiate<Schedule>(scheduleTemplate) : ScriptableObject.CreateInstance<Schedule>());
		
		if(!AssetDatabase.IsValidFolder(path))
			Directory.CreateDirectory(path);
				
		AssetDatabase.CreateAsset(schedule, path + name + ".asset");
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		return schedule;
	}

	private ScheduleObject GetTarget(string target, string type)
	{
		// target = target.TrimEnd().TrimStart();

		if(string.IsNullOrEmpty(target.TrimEnd().TrimStart()))
			return null;

		string[] result = FindAssets(target, type);

		if(result == null || result.Length == 0)
		{
			target = target.TrimEnd().TrimStart();
			result = FindAssets(target, type);
			Debug.Log("Attempting to search again.");
		}

		if(result == null || result.Length == 0)
		{
			Debug.Log("Cannot get schedule object for : " + target + " end of line.");
			return null;
		}

		string assetPath = AssetDatabase.GUIDToAssetPath(result[0]);
		ScheduleObject scheduleObject = AssetDatabase.LoadAssetAtPath<ScheduleObject>(assetPath);

		if(scheduleObject == null)
		{
			// Debug.Log("Failed to get " + target + " with result of " + result.Length);
			Debug.Log("asset path ? " + assetPath);
			// scheduleObject = AssetDatabase.LoadAssetAtPath<ScheduleObject>(result[0]);

			// if(scheduleObject == null)
			// 	Debug.Log("Really failed hard.");
		}
		return scheduleObject;
	}

	private string[] FindAssets(string target, string type)
	{
		string[] result = AssetDatabase.FindAssets("\"" + target + "\"" + " t:" + type);
		return result;
	}

	private Day GetDay(string index)
	{
		int i = 1;
		if(int.TryParse(index, out i))
			return (Day)i;
		else
		{
			Debug.Log("Failed to parse day.");
			return Day.Friday;
		}
	}
}

#endif