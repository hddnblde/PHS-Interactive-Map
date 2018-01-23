using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Schedules
{
	[CreateAssetMenu(menuName = "Faculty/Schedules/Period", order = 0, fileName = "Period")]
	public class Period : ScriptableObject
	{
		[SerializeField]
		private TimeStamp m_start = null;

		[SerializeField]
		private TimeStamp m_end = null;

		public TimeStamp start
		{
			get { return m_start; }
		}

		public TimeStamp end
		{
			get { return m_end; }
		}
	
		private float CalculateTotalHours()
		{
			float difference = Mathf.Abs(TimeStampToMinutes(m_end) - TimeStampToMinutes(m_start));
			return MinutesToHours(difference);
		}

		public void CalculateTimeSpan(out int hours, out int minutes)
		{
			float totalHours = CalculateTotalHours();
			minutes = Mathf.FloorToInt(Mathf.Repeat(totalHours, 1f) * 60f);
			hours = Mathf.FloorToInt(totalHours);
		}

		private float MinutesToHours(float minutes)
		{
			return minutes / 60f;
		}

		private float TimeStampToMinutes(TimeStamp timeStamp)
		{
			int totalHours = (timeStamp.meridiem == TimeStamp.Meridiem.PM ? timeStamp.hour + 12 : timeStamp.hour);
			return (totalHours * 60f) + timeStamp.minute;
		}
	}

	[System.Serializable]
	public class TimeStamp
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

		public override string ToString()
		{
			return m_hour.ToString("D2") + ':' + m_minute.ToString("D2") + ' ' + meridiem.ToString();
		}
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(Period))]
	public class PeriodEditor : Editor
	{
		private Period period = null;

		private SerializedProperty
		startProperty = null,
		endProperty = null;

		private void OnEnable()
		{
			Initialize();
		}

		public override void OnInspectorGUI()
		{
			DrawTimeStamp(startProperty);
			EditorGUILayout.Space();
			DrawTimeStamp(endProperty);
			EditorGUILayout.Space();
			DrawTimeSpan();
		}

		private void Initialize()
		{
			period = target as Period;
			startProperty = serializedObject.FindProperty("m_start");
			endProperty = serializedObject.FindProperty("m_end");
		}

		private void DrawTimeStamp(SerializedProperty timeStamp)
		{
			EditorGUILayout.LabelField(timeStamp.displayName, EditorStyles.boldLabel);
			EditorGUI.indentLevel++;

			SerializedProperty
			hourProperty = timeStamp.FindPropertyRelative("m_hour"),
			minuteProperty = timeStamp.FindPropertyRelative("m_minute"),
			meridiemProperty = timeStamp.FindPropertyRelative("m_meridiem");

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.PropertyField(hourProperty);
			EditorGUILayout.PropertyField(minuteProperty);
			EditorGUILayout.PropertyField(meridiemProperty);
			EditorGUI.indentLevel--;

			Validate(hourProperty, minuteProperty, meridiemProperty);

			if(EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();
		}

		private void DrawTimeSpan()
		{
			int hours;
			int minutes;
			period.CalculateTimeSpan(out hours, out minutes);
			string message = "@h @m";
			string hourMessage = (hours > 0 ? hours.ToString("##") + " hour" + (hours > 1 ? "s" : "") : "");
			string minuteMessage = (minutes > 0 ? minutes.ToString("##") + " minute" + (minutes > 1 ? "s" : "") : "");

			message = message.Replace("@h", hourMessage);
			message = message.Replace("@m", minuteMessage);

			EditorGUILayout.HelpBox(message, MessageType.Info);
		}

		private void Validate(SerializedProperty hourProperty, SerializedProperty minuteProperty, SerializedProperty meridiemProperty)
		{
			if(hourProperty.intValue > 12 || hourProperty.intValue < 1)
			{
				hourProperty.intValue = Mathf.Clamp(Mathf.FloorToInt(Mathf.Repeat(hourProperty.intValue, 13)), 1, 12);
				FlipMeridiem(meridiemProperty);
				Validate(hourProperty, minuteProperty, meridiemProperty);
			}

			if(minuteProperty.intValue > 59 || minuteProperty.intValue < 0)
			{
				int direction = (minuteProperty.intValue > 59 ? 1 : -1);
				minuteProperty.intValue = Mathf.FloorToInt(Mathf.Repeat(minuteProperty.intValue, 60));
				hourProperty.intValue += direction;
				Validate(hourProperty, minuteProperty, meridiemProperty);
			}
		}

		private void FlipMeridiem(SerializedProperty meridiemProperty)
		{
			meridiemProperty.enumValueIndex = (meridiemProperty.enumValueIndex == 0 ? 1 : 0);
		}
	}
	#endif
}