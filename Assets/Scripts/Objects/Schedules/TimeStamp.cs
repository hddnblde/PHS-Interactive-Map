using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Schedules
{
	[CreateAssetMenu(menuName = "Schedules/Time Stamp", order = 0, fileName = "Time Stamp")]
	public class TimeStamp : ScriptableObject
	{
		public enum Meridiem
		{
			AM,
			PM
		}

		[SerializeField]
		private int m_hour = 12;

		[SerializeField]
		private int m_minute = 0;

		[SerializeField]
		private Meridiem m_meridiem = Meridiem.AM;

		public int hour
		{
			get { return m_hour % 12; }
		}

		public int minute
		{
			get { return Mathf.Clamp(m_minute, 0, 59); }
		}

		public Meridiem meridiem
		{
			get { return m_meridiem; }
		}
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(TimeStamp))]
	public class TimeFormatEditor : Editor
	{
		private SerializedProperty
		hourProperty = null,
		minuteProperty = null,
		meridiemProperty = null;

		private void OnEnable()
		{
			Initialize();
		}

		public override void OnInspectorGUI()
		{
			DrawCustomInspector();
		}

		private void Initialize()
		{
			hourProperty = serializedObject.FindProperty("m_hour");
			minuteProperty = serializedObject.FindProperty("m_minute");
			meridiemProperty = serializedObject.FindProperty("m_meridiem");
		}

		private void DrawCustomInspector()
		{
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.PropertyField(hourProperty);
			EditorGUILayout.PropertyField(minuteProperty);
			EditorGUILayout.PropertyField(meridiemProperty);

			Validate();

			if(EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();
		}

		private void Validate()
		{
			bool revalidate = false;

			if(hourProperty.intValue > 12 || hourProperty.intValue < 1)
			{
				hourProperty.intValue = hourProperty.intValue % 12;
				FlipMeridiem();
				revalidate = true;
			}

			if(minuteProperty.intValue > 59 || minuteProperty.intValue < 1)
			{
				int direction = (minuteProperty.intValue > 59 ? 1 : -1);
				minuteProperty.intValue = minuteProperty.intValue % 59;
				hourProperty.intValue += direction;
				revalidate = true;
			}

			if(revalidate)
				Validate();
		}

		private void FlipMeridiem()
		{
			meridiemProperty.enumValueIndex = (meridiemProperty.enumValueIndex == 0 ? 1 : 0);
		}

		private string GetTimeStamp(int hour, int minute)
		{
			string timeStamp = "@hr:@min @m";
			string meridiem = (hour > 12 ? "PM" : "AM");
//			string hour 
			return timeStamp;

		}

		private int Repeat(int value, int length)
		{
			return Mathf.FloorToInt(Mathf.Repeat(value, length));
		}
	}
	#endif
}