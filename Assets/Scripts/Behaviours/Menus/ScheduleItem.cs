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

	private PeriodGroup period = null;

	public void Set(PeriodGroup period)
	{
		if(period == null)
			return;

		this.period = period;
		SetTime(period.period);
	}

	public void ViewEntry(Day day)
	{
		ScheduleEntry entry = period.GetEntry(day);
		SetEntry(entry);
	}

	private void SetTime(Period period)
	{
		if(timeText == null)
			return;

		if(period == null)
		{
			timeText.text = "";
			return;
		}

		string time = "@start - @end".Replace("@start", period.start.ToString()).Replace("@end", period.end.ToString());
		timeText.text = time;
	}

	private void SetEntry(ScheduleEntry entry)
	{
		if(entryText == null)
			return;

		if(entry == null)
		{
			entryText.text = "";
			return;
		}

		string entryPattern = "@title\n@subtitle".Replace("@title", entry.title).Replace("@subtitle", entry.subtitle);
		entryText.text = entryPattern;
	}
}
