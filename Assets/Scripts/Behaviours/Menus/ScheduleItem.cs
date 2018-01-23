using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Schedules;

public class ScheduleItem : MonoBehaviour
{
	[SerializeField]
	private Text timeText = null;

	[SerializeField]
	private Text entryText = null;

	private Schedule m_schedule = null;

	public void Set()
	{
		// m_schedule.periods[0].rooms[0].entity.subtitle;
	}


	private void SetTime(string time)
	{
		if(timeText == null)
			return;
	}

	private void SetEntry(string entry)
	{
		if(entryText == null)
			return;

		entryText.text = entry;
	}
}
