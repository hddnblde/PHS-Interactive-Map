using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Schedule
{
	[CreateAssetMenu(menuName = "Schedule/Time Format", order = 0, fileName = "Time Format")]
	public class TimeFormat : ScriptableObject
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
			get { return (m_hour > 12 ? Meridiem.PM : Meridiem.AM);}
		}
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(TimeFormat))]
	public class TimeFormatEditor : Editor
	{
		private SerializedProperty
		hourProperty = null,
		minuteProperty = null;

		private void OnEnable()
		{
			
		}

		public override void OnInspectorGUI()
		{
			
		}

		private void Initialize()
		{
			hourProperty = serializedObject.FindProperty("m_hour");
			minuteProperty = serializedObject.FindProperty("m_minute");
		}

		private void DrawCustomInspector()
		{
			EditorGUI.BeginChangeCheck();

			hourProperty.intValue = Repeat(hourProperty.intValue, 24);
			minuteProperty.intValue = Repeat(minuteProperty.intValue, 60);

			if(EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();
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