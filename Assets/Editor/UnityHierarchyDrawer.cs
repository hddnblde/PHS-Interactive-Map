using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class UnityHierarchyDrawer
{
	private const int IconSoftLimit = 7;
	private enum InspectMode
	{
		Components,
		Layers
	}

	private static InspectMode inspectMode = InspectMode.Components;

	static UnityHierarchyDrawer()
	{
		EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowItemUpdate;
		EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemUpdate;

		Undo.undoRedoPerformed -= PerformRedraw;
		Undo.undoRedoPerformed += PerformRedraw;
	}

	[MenuItem("Hierarchy/Inspect Components")]
	private static void InspectComponents()
	{
		inspectMode = InspectMode.Components;
		EditorApplication.DirtyHierarchyWindowSorting();
	}

	[MenuItem("Hierarchy/Inspect Layers")]
	private static void InspectLayers()
	{
		inspectMode = InspectMode.Layers;
		EditorApplication.DirtyHierarchyWindowSorting();
	}

	private static void DrawToggle(Rect rect, GameObject gameObject)
	{
		Transform parent = gameObject.transform.parent;

		// check if there is a parent, if there is and the parent is inactive
		// then skip drawing
		if(parent != null && !parent.gameObject.activeInHierarchy)
			return;

		// create a new rect and offset it to left
		Rect r = new Rect(rect);
		r.x -= 27;
		r.size = Vector2.one * 17f;

		// use the toggle's style for gui button
		GUIStyle style = new GUIStyle(EditorStyles.toggle);
		style.normal = (gameObject.activeInHierarchy ? style.onNormal : style.normal);

		// draw the button, toggle the gameObject's active
		if(GUI.Button(r, "", style))
		{
			Undo.RecordObject(gameObject, "toggled gameObject");
			gameObject.SetActive(!gameObject.activeInHierarchy);
		}

		r.x -= r.width;
	}

	private static void DrawComponents(Rect rect, GameObject gameObject)
	{
		List<Component> components = new List<Component>(gameObject.GetComponents<Component>());

		if(components.Count > 0)
		{
			// check if the number of components exceed the soft limit
			// trim down the excess and only show them in label for now
			int excess = components.Count - IconSoftLimit - 1;
			if(IconSoftLimit > 0 && components.Count > IconSoftLimit)
				components.RemoveRange(IconSoftLimit + 1, excess);

			// reverse the list, because drawing will begin from the right, going to left
			components.Reverse();

			// create a new rect instance, and reposition it starting from the right
			Rect r = new Rect(rect);
			r.x = r.width + r.x - 20f;
			r.width = 17f;

			// if there are excess, draw a label to indicate the number of exceeding icons
			if(excess > 0)
			{
				string excessLabel = "(+val)".Replace("val", excess.ToString());
				r.size = GUI.skin.label.CalcSize(new GUIContent(excessLabel));
				r.position -= Vector2.right * (r.width - 15f);
				GUI.Label(r, excessLabel);
				r.position -= Vector2.right * 15f;
			}

			foreach(Component component in components)
			{
				if(component == null)
					continue;

				r.height = 17f;
				r.width = 17f;

				// if the component is either Transform, RectTransform, or CanvasRenderer just skip drawing
				System.Type componentType = component.GetType();
				if(componentType == typeof(Transform) || componentType == typeof(CanvasRenderer) || componentType == typeof(RectTransform))
					continue;

				// get the icon of the component
				Texture2D icon = AssetPreview.GetMiniThumbnail(component);

				// get the instance as behaviour to enable/disable
				Behaviour mono = component as Behaviour;

				if(mono == null)
					continue;

				// store the previous GUI color before tweaking
				Color previousGUIColor = GUI.color;

				// if component is disabled, assign a gray color
				if(!mono.enabled)
				{
					Color disabledColor = Color.Lerp(GUI.color, Color.gray, 0.65f);
					disabledColor = new Color(disabledColor.r, disabledColor.g, disabledColor.b, 0.75f);
					GUI.color = disabledColor;
				}

				// draw the button with the component's icon
				GUI.Label(r, icon);
				r.width = 15f;
				r.height = 15f;

				if(GUI.Button(r, "", new GUIStyle()))
				{
					Undo.RecordObject(component, "component edited");
					if(Event.current.button == 1)
					{
						if(EditorUtility.DisplayDialog("Removing Component", "Remove @component?".Replace("@component", GetBehaviourName(mono)), "Yes Please!", "No"))
							GameObject.DestroyImmediate(mono);
					}
					else
						mono.enabled = !mono.enabled;
				}

				// reassign the previous color
				GUI.color = previousGUIColor;

				// move the rect to the left
				r.position -= Vector2.right * 17f;
			}
		}
	}

	private static void DrawLayerPopup(Rect rect, GameObject gameObject)
	{
		Rect r = new Rect(rect);
		r.x = r.width + r.x - 105f;
		r.width = 100f;

		GUIStyle style = new GUIStyle(EditorStyles.miniLabel);
		int selected = EditorGUI.LayerField(r, gameObject.layer, style);

		if(selected != gameObject.layer)
		{
			Undo.RecordObject(gameObject, "layer changed");
			gameObject.layer = selected;
		}
	}

	private static void OnHierarchyWindowItemUpdate(int instanceID, Rect rect)
	{
		// get the instance using the instanceID
		Object instance = EditorUtility.InstanceIDToObject(instanceID);

		// do nothing if instance is null
		if(instance == null)
			return;

		// get all the list of components from instance
		GameObject o = instance as GameObject;
		DrawToggle(rect, o);
		switch(inspectMode)
		{
		case InspectMode.Components:
			DrawComponents(rect, o);
			break;
		case InspectMode.Layers:
			DrawLayerPopup(rect, o);
			break;
		}
	}

	private static string GetBehaviourName(Behaviour behaviour)
	{
		string fullName = behaviour.ToString();
		string[] cuts = fullName.Split('.');

		if(cuts != null && cuts.Length > 0)
		{
			string name = cuts[cuts.Length - 1];
			name = name.Replace("(", "");
			name = name.Replace(")", "");
			return name;
		}
		else
			return "";
	}

	private static void PerformRedraw()
	{
		EditorApplication.RepaintHierarchyWindow();
	}

	private static void MoveComponentUp(Component component)
	{
		UnityEditorInternal.ComponentUtility.MoveComponentUp(component);
	}

	private static void MoveComponentDown(Component component)
	{
		UnityEditorInternal.ComponentUtility.MoveComponentDown(component);
	}
}