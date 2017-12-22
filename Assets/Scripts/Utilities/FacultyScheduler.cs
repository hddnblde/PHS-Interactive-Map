#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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

	private void OnGUI()
	{
		DrawTable(position.width, position.height, 3);
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
		for(int column = 0; column < columns; column++)
		{
			boxRect.x = rect.position.x + ((spacing * 1.5f) * (column + 1)) + (width * column);
			for(int row = 0; row < rows; row++)
			{
				boxRect.y = rect.position.y + ((spacing * 1.5f) * (row + 1)) + (height * row);
				GUI.Box(boxRect, "");
			}
		}
	}
}
#endif