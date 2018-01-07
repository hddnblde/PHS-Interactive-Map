#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Faculty.Schedules;
using Students;
using Map;

public class FacultyScheduler : EditorWindow
{
	[MenuItem("Tools/Faculty Scheduler")]
	private static void OpenWindow()
	{
		FacultyScheduler scheduler = EditorWindow.CreateInstance<FacultyScheduler>();
		scheduler.maxSize = new Vector2(570f, 630f);
		scheduler.minSize = new Vector2(440, 480f);
		scheduler.Show();
	}

	#region Data Structure
	private struct SelectedItem
	{
		public int column;
		public int row;

		public bool NoneSelected()
		{
			return column == -1 && row == -1;
		}
	}

	private enum SelectionDepth
	{
		SelectRoom,
		SelectSectionCluster
	}
	#endregion
	private SelectedItem currentSelectedItem;
	private SelectionDepth selectionDepth = SelectionDepth.SelectRoom;
	private bool objectPickerWindowOpen = false;

	private void OnGUI()
	{
		DrawTable(position.width, position.height, 3);
		HandleScheduling();
	}

	private void DrawTable(float width, float height, int period)
	{
		float headerHeight = 75f;
		float sidebarWidth = 100f;
		Rect header = new Rect(Vector2.zero + (Vector2.right * sidebarWidth), new Vector2(width - sidebarWidth, headerHeight));
		Rect sidebar = new Rect(Vector2.zero + (Vector2.up * header.height), new Vector2(sidebarWidth, height - headerHeight));
		Rect table = new Rect(Vector2.zero + (Vector2.up * header.height) + (Vector2.right * sidebar.width), new Vector2(width - sidebarWidth, height - headerHeight));
	
//		GUI.Box(header, "");
//		GUI.Box(sidebar, "");
		DrawBoxes(table, 5, 14);
	}

	private void DrawBoxes(Rect rect, int columns, int rows)
	{
		DrawBoxes(rect, columns, rows, 0f);
	}

	private void DrawBoxes(Rect rect, int columns, int rows, float spacing)
	{
		if(columns == 0 || rows == 0)
			return;

		float width = (rect.width / columns) - (spacing * 2);
		float height = (rect.height / rows) - (spacing * 2);
		Rect boxRect = new Rect(rect.position, new Vector2(width, height));

		Color defaultGUIColor = GUI.color;

		for(int column = 0; column < columns; column++)
		{
			boxRect.x = rect.position.x + ((spacing * 1.5f) * (column + 1)) + (width * column);
			for(int row = 0; row < rows; row++)
			{
				boxRect.y = rect.position.y + ((spacing * 1.5f) * (row + 1)) + (height * row);

//				Room selectedRoom = null;

				GUI.color = Color.clear;
				if(!currentSelectedItem.NoneSelected() && GUI.Button(boxRect, ""))
				{
					currentSelectedItem.column = column;
					currentSelectedItem.row = row;
					objectPickerWindowOpen = true;
				}
				GUI.color = defaultGUIColor;

				GUI.Box(boxRect, "");
			}
		}
	}

	private void HandleScheduling()
	{
		if(currentSelectedItem.NoneSelected())
			return;

		Room selectedRoom = null;
		StudentClass selectedStudentClass = null;

		if(!objectPickerWindowOpen)
		{
			int controlID = EditorGUIUtility.GetControlID(FocusType.Passive);

			if(selectionDepth == SelectionDepth.SelectRoom)
				EditorGUIUtility.ShowObjectPicker<Room>(selectedRoom, false, "", controlID);
			else
				EditorGUIUtility.ShowObjectPicker<StudentClass>(selectedStudentClass, false, "", controlID);

			objectPickerWindowOpen = true;
		}

		string commandName = Event.current.commandName;

		if(commandName == "ObjectSelectorClosed")
		{
			if(selectionDepth == SelectionDepth.SelectRoom)
				selectedRoom = (Room)EditorGUIUtility.GetObjectPickerObject();
			else
			{
				selectedStudentClass = (StudentClass)EditorGUIUtility.GetObjectPickerObject();
				currentSelectedItem.column = -1;
				currentSelectedItem.row = -1;

				Debug.Log("SELECTED ROOM : " + selectedRoom.name);
				Debug.Log("SELECTED SECTION : " + selectedStudentClass.name);
			}

			objectPickerWindowOpen = false;
		}
	}
}
#endif