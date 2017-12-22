using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Faculty.Schedules;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using System.Linq;
#endif

namespace Faculty
{
	[CreateAssetMenu(menuName = "Faculty/Teacher", order = 0, fileName = "Teacher")]
	public class Teacher : ScriptableObject
	{
		#region Data Structures
		[System.Serializable]
		public class Date
		{
			private enum Month
			{
				January = 1,
				February = 2,
				March = 3,
				April = 4,
				May = 5,
				June = 6,
				July = 7,
				August = 8,
				September = 9,
				October = 10,
				November = 11,
				December = 12
			}

			[SerializeField]
			private Month m_month = Month.February;

			[SerializeField, Range(1, 31)]
			private int m_day = 1;

			[SerializeField]
			private int m_year = 1900;

			public string month
			{
				get { return m_month.ToString(); }
			}

			public int day
			{
				get { return m_day; }
			}

			public int year
			{
				get { return m_year; }
			}
		}

		[System.Serializable]
		public class Course
		{
			#region Serialized Fields
			[SerializeField]
			private string m_title;

			[SerializeField]
			private string m_majorSubject;

			[SerializeField]
			private string m_minorSubject;
			#endregion


			#region Properties
			private string title
			{
				get { return m_title; }
			}

			private string majorSubject
			{
				get { return m_majorSubject; }
			}

			private string minorSubject
			{
				get { return m_minorSubject; }
			}
			#endregion
		}

		public enum Department
		{
			Aralin,
			English,
			Filipino,
			MAPEH,
			Math,
			Science,
			Values
		}
		#endregion


		#region Serialized Fields
		[Header("Name")]
		[SerializeField]
		private string m_firstName = "First Name";

		[SerializeField]
		private string m_middleName = "Middle Name";

		[SerializeField]
		private string m_lastName = "Last Name";

		[Header("Details")]
		[SerializeField]
		private Department m_department = Department.Aralin;

		[SerializeField]
		private Course m_course = new Course();

		[SerializeField]
		private Date m_appointmentDate = new Date();

		[SerializeField]
		private List<ScheduleItem> m_scheduleTable = new List<ScheduleItem>();
		#endregion


		#region Properties
		public string firstName
		{
			get { return m_firstName; }
		}

		public string middleName
		{
			get { return m_middleName; }
		}

		public string lastName
		{
			get { return m_lastName; }
		}

		public Course course
		{
			get { return m_course; }
		}

		public Department department
		{
			get { return m_department; }
		}

		public ScheduleItem GetScheduleItem(int period)
		{
			if(period < 0 || period >= m_scheduleTable.Count)
				return null;
			else
				return m_scheduleTable[period];
		}
		#endregion


		#region Methods
		public void GenerateSchedule(string schedulePeriodPath)
		{
			#if UNITY_EDITOR
			if(!Directory.Exists(schedulePeriodPath))
				return;

			string endDirectory = schedulePeriodPath.Split("/\\".ToCharArray()).Last() + " Period ";

			string[] files = Directory.GetFiles(schedulePeriodPath, "*.asset");//.OrderBy(f => int.Parse(f.Replace(endDirectory, ""))).ToArray();

			foreach(string file in files)
			{
				string period = file.Replace(schedulePeriodPath + '\\', "").Replace(".asset", "");
//				Debug.Log(period.Replace(endDirectory, ""));
				Debug.Log(period);
			}
			#endif
		}

		public string GetFullName(bool useMiddleNameInitials = true)
		{
			string middle = (useMiddleNameInitials ? (m_middleName.Substring(0, 1) + ".") : m_middleName);
			return "@f @m @l".Replace("@f", m_firstName).Replace("@l", m_lastName).Replace("@m", middle);
		}

		public string GetAppointmentDate()
		{
			if(m_appointmentDate == null)
				return "";
			else
			{
				string pattern = "@m @d @y";
				return pattern.Replace("@m", m_appointmentDate.month.ToString()).Replace("@d", m_appointmentDate.day.ToString("##")).Replace("@y", m_appointmentDate.year.ToString("####"));
			}
		}
		#endregion
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(Teacher))]
	public class TeacherEditor : Editor
	{
		private Teacher teacher = null;
		private static string schedulePeriodPath = "";

		private void OnEnable()
		{
			teacher = target as Teacher;
			LoadPref();
		}

		private void OnDisable()
		{
			SavePref();
		}

		private void LoadPref()
		{
			schedulePeriodPath = EditorPrefs.GetString("Teacher_SchedulePeriodPath", "Assets/Scriptable Objects/Schedules/Periods");
		}

		private void SavePref()
		{
			if(schedulePeriodPath.Length > 0)
				EditorPrefs.SetString("Teacher_SchedulePeriodPath", schedulePeriodPath);
		}

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			EditorGUILayout.Space();
			schedulePeriodPath = EditorGUILayout.TextField(schedulePeriodPath);
			if(GUILayout.Button("Generate Schedule"))
				teacher.GenerateSchedule(schedulePeriodPath);
		}
	}
	#endif
}