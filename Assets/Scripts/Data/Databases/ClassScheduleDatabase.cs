using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Schedules;
using PampangaHighSchool.Students;

namespace Databases
{
	

	[DisallowMultipleComponent]
	public class ClassScheduleDatabase : MonoBehaviour
	{
		#region Data Structure
		private class GradeGroup
		{
			[SerializeField]
			private Grade m_grade = Grade.Grade7;
			
			[SerializeField]
			private List<SectionGroup> m_sections = new List<SectionGroup>();

			public Grade grade
			{
				get { return m_grade; }
			}

			public int sectionCount
			{
				get
				{
					if(m_sections == null)
						return 0;
					else
						return m_sections.Count;
				}
			}

			public SectionGroup GetSectionGroup(int index)
			{
				if(index < 0 || m_sections == null || index >= m_sections.Count)
					return null;
				else
					return m_sections[index];
			}
		}

		private class SectionGroup
		{
			[SerializeField]
			private Section m_section = null;

			[SerializeField]
			private List<Schedule> m_schedules = new List<Schedule>();

			public int scheduleCount
			{
				get
				{
					if(m_schedules == null)
						return 0;
					else
						return m_schedules.Count;
				}
			}

			public Section section
			{
				get { return m_section; }
			}

			public Schedule GetSchedule(int index)
			{
				if(index < 0 || m_schedules == null || index >= m_schedules.Count)
					return null;
				else
					return m_schedules[index];
			}
		}
		#endregion


		#region Serialized Field
		[SerializeField]
		private List<GradeGroup> gradesGroup = new List<GradeGroup>();
		#endregion


		#region Hidden Fields
		public enum SelectionDepth
		{
			Grades = 0,
			Sections = 1,
			Schedules = 2,
			Schedule = 3
		}

		private SelectionDepth selectionDepth = SelectionDepth.Grades;
		private Grade currentGrade = Grade.Grade7;
		private int sectionIndex = -1;
		private int scheduleIndex = -1;
		#endregion


		#region Property
		public SelectionDepth currentSelectionDepth
		{
			get { return selectionDepth; }	
		}
		#endregion

		#region Actions
		public string[] GetItems()
		{
			switch(selectionDepth)
			{
				case SelectionDepth.Grades:
				return GetGradeItems();

				case SelectionDepth.Sections:
				return GetSectionItems();

				case SelectionDepth.Schedules:
				return GetScheduleItems();

				default:
				return null;
			}
		}

		public void ResetSelection()
		{
			selectionDepth = SelectionDepth.Grades;
		}

		public void MoveIn(int selectedIndex)
		{
			if(selectionDepth == SelectionDepth.Schedule)
			{
				Debug.Log("Cannot move in any further, selection depth is already deep.");
				return;
			}

			switch(selectionDepth)
			{
				case SelectionDepth.Grades:
				currentGrade = GetGradeFromIndex(selectedIndex);
				break;

				case SelectionDepth.Sections:
				sectionIndex = selectedIndex;
				break;

				case SelectionDepth.Schedules:
				scheduleIndex = selectedIndex;
				break;
			}
		}		
		#endregion


		#region Helpers
		private Grade GetGradeFromIndex(int index)
		{
			index = Mathf.Clamp(index, 0, 5);
			
			if(index == 0)
				return Grade.Grade7;
			else if(index == 1)
				return Grade.Grade8;
			else if(index == 2)
				return Grade.Grade9;
			else if(index == 3)
				return Grade.Grade10;
			else if(index == 4)
				return Grade.Grade11;
			else
				return Grade.Grade12;
		}

		private string[] GetGradeItems()
		{
			if(gradesGroup == null || gradesGroup.Count == 0)
					return null;

				List<string> grades = new List<string>();

				foreach(GradeGroup grade in gradesGroup)
					grades.Add("Grade " + grade.grade);

				return grades.ToArray();
		}

		private string[] GetSectionItems()
		{
			GradeGroup grade = GetGradeGroup(currentGrade);

			if(grade == null)
				return null;

			List<string> sections = new List<string>();

			for(int i = 0; i < grade.sectionCount; i++)
				sections.Add(grade.GetSectionGroup(i).section.name);

			return sections.ToArray();
		}

		private string[] GetScheduleItems()
		{
			SectionGroup section = GetSectionGroup(sectionIndex);

			if(section == null)
				return null;

			List<string> schedules = new List<string>();

			for(int i = 0; i < section.scheduleCount; i++)
				schedules.Add(section.GetSchedule(i).name.Replace(" Schedule", ""));
			
			return schedules.ToArray();
		}

		private GradeGroup GetGradeGroup(Grade grade)
		{
			if(gradesGroup == null || gradesGroup.Count == 0)
				return null;

			GradeGroup gradeGroup = null;

			foreach(GradeGroup g in gradesGroup)
			{
				if(g.grade == grade)
				{
					gradeGroup = g;
					break;
				}
			}

			return gradeGroup;
		}

		private SectionGroup GetSectionGroup(int sectionIndex)
		{
			GradeGroup grade = GetGradeGroup(currentGrade);

			if(grade == null)
				return null;

			SectionGroup section = grade.GetSectionGroup(sectionIndex);
			return section;
		}
		#endregion
	}
}