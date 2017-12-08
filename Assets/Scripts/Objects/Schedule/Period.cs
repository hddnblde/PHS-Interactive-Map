using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Schedule
{
	public class Period : ScriptableObject
	{
		[SerializeField]
		private TimeFormat m_timeIn = null;

		[SerializeField]
		private TimeFormat m_timeOut = null;
	}
}