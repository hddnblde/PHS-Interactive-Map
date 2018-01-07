using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Faculty;
using Faculty.Schedules;
using Students;

public class ScheduleMenu : MonoBehaviour
{
	public enum Context
	{
		ByFaculty,
		ByStudent
	}

	private Context currentContext = Context.ByFaculty;

	private Teacher teacher = null;

	public void ViewTeacher(Teacher teacher)
	{
//		teacher.GetScheduleItem
	}

	public void ViewStudent(StudentClass studentClass)
	{
		
	}
}