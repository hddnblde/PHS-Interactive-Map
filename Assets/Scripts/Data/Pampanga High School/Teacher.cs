using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Schedules;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using System.Linq;
#endif

namespace PampangaHighSchool.Faculty
{
	public enum Department
	{
		Aralin,
		English,
		Filipino,
		MAPEH,
		Math,
		Science,
		Values,
		SeniorHigh
	}

	[CreateAssetMenu(menuName = "Faculty/Teacher", order = 0, fileName = "Teacher")]
	public class Teacher : ScheduleObject
	{
		#region Data Structures
		[System.Serializable]
		public class AppointmentDate
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
		private AppointmentDate m_appointmentDate = new AppointmentDate();

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

		public override string title
		{
			get { return department.ToString(); }
		}

		public override string subtitle
		{
			get { return GetFullName(); }
		}
		#endregion


		#region Functions
		public string GetFullName()
		{
			string firstName = (string.IsNullOrEmpty(m_firstName) ? "" : m_firstName);
			string middleName = (string.IsNullOrEmpty(m_middleName) ? "" : (m_middleName.Substring(0, 1) + "."));
			string lastName = (string.IsNullOrEmpty(m_lastName) ? "" : " " + m_lastName);
			
			return "@f@m@l"
			.Replace("@f", firstName)
			.Replace("@m", middleName)
			.Replace("@l", lastName);
		}

		public string GetAppointmentDate()
		{
			if(m_appointmentDate == null)
				return "";
			else
			{
				string pattern = "@m @d @y";
				return pattern
					.Replace("@m", m_appointmentDate.month.ToString())
					.Replace("@d", m_appointmentDate.day.ToString("##"))
					.Replace("@y", m_appointmentDate.year.ToString("####"));
			}
		}
		#endregion
	}
}