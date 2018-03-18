using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Schedules
{
	public abstract class ScheduleObject : ScriptableObject
	{
		public abstract string title { get; }
		public abstract string subtitle { get; }
	}
}