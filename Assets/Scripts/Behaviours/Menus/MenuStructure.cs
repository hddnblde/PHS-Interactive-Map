using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Menus
{
	[CreateAssetMenu(menuName = "Menu/Structure", fileName = "Menu Structure", order = 0)]
	public class MenuStructure : ScriptableObject
	{
		[SerializeField]
		private List<Menus.DataStructure.MenuItem> items = new List<Menus.DataStructure.MenuItem>();

		public Menus.DataStructure.MenuItem[] GetContext(int[] contextTable)
		{
			return null;
		}

		public Menus.DataStructure.MenuItem[] GetItems()
		{
			return items.ToArray();
		}
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(MenuStructure))]
	public class MenuStructureEditor : Editor
	{
		private SerializedProperty itemsProperty = null;
		private const float IndentionWidth = 10f;
		private const float IndentionOffset = 45f;
		private const int depthLimit = 12;

		private void OnEnable()
		{
			itemsProperty = serializedObject.FindProperty("items");
		}

		public override void OnInspectorGUI()
		{

			serializedObject.Update();
			DrawItems();
		}

		private void DrawItems()
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(itemsProperty);

			if(!itemsProperty.isExpanded)
				return;

			SerializedProperty arraySizeCountProperty = itemsProperty.FindPropertyRelative("Array.size");
			EditorGUILayout.PropertyField(arraySizeCountProperty);

			EditorGUI.indentLevel++;

			int previousDepth = 0;
			for(int i = 0; i < itemsProperty.arraySize; i++)
			{
				DrawItem(i, ref previousDepth, itemsProperty);
			}

			EditorGUI.indentLevel--;

			if(EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();
		}

		private void DrawItem(int index, ref int previousDepth, SerializedProperty parent)
		{
			SerializedProperty item = itemsProperty.GetArrayElementAtIndex(index);
			SerializedProperty titleProperty = item.FindPropertyRelative("m_title");
			SerializedProperty depthProperty = item.FindPropertyRelative("m_depth");
			SerializedProperty toggleProperty = item.FindPropertyRelative("m_toggleable");

			int currentDepth = depthProperty.intValue;

			if(currentDepth < previousDepth)
				EditorGUILayout.Space();

			previousDepth = currentDepth;
			
			Rect rect = new Rect(EditorGUILayout.GetControlRect());

			float indention = (IndentionWidth * currentDepth) + IndentionOffset;
			float buttonOffset = 40f;

			rect.width -= indention;
			rect.x += indention;
			rect.width -= buttonOffset;

			EditorGUI.PropertyField(rect, titleProperty, new GUIContent(""));
			Rect buttons1Rect = new Rect(rect);
			buttons1Rect.width = 20f;
			buttons1Rect.x += rect.width + 1;

			if(GUI.Button(buttons1Rect, "+"))
			{
				parent.InsertArrayElementAtIndex(index);
				return;
			}

			buttons1Rect.x += buttons1Rect.width + 1;
			if(GUI.Button(buttons1Rect, "D"))
			{
				parent.DeleteArrayElementAtIndex(index);
				return;
			}

			Rect buttonsRect2 = new Rect(rect);
			buttonsRect2.width = 20f;
			buttonsRect2.x -= 7;

			GUIStyle depthButtonStyle1 = new GUIStyle(GUI.skin.button);
			if(currentDepth >= depthLimit)
				depthButtonStyle1.normal = depthButtonStyle1.active;
			
			if(GUI.Button(buttonsRect2, "►", depthButtonStyle1) && currentDepth < depthLimit)
				currentDepth++;
			
			buttonsRect2.x -= buttonsRect2.width + 1;

			GUIStyle depthButtonStyle2 = new GUIStyle(GUI.skin.button);
			if(currentDepth <= 0)
				
				depthButtonStyle2.normal = depthButtonStyle2.active;
			if(GUI.Button(buttonsRect2, "◄", depthButtonStyle2) && currentDepth > 0)
				currentDepth--;
			
			buttonsRect2.x -= buttonsRect2.width + 1;

			currentDepth = Mathf.Clamp(currentDepth, 0, depthLimit);
			depthProperty.intValue = currentDepth;

			GUIStyle toggleButtonStyle = new GUIStyle(GUI.skin.button);
			if(toggleProperty.boolValue)
				toggleButtonStyle.normal = toggleButtonStyle.active;
			
			if(GUI.Button(buttonsRect2, "≡", toggleButtonStyle))
				toggleProperty.boolValue = !toggleProperty.boolValue;
		}
	}
	#endif
}