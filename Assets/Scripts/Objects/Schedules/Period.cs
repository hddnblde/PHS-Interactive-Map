using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Schedules
{
	[CreateAssetMenu(menuName = "Schedules/Period", order = 1, fileName = "Period")]
	public class Period : ScriptableObject
	{
		[SerializeField]
		private TimeStamp m_timeIn = null;

		[SerializeField]
		private TimeStamp m_timeOut = null;

		public TimeStamp timeIn
		{
			get { return m_timeIn; }
		}

		public TimeStamp timeOut
		{
			get { return m_timeOut; }
		}
	}
}