using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menus.Configurations
{
	[CreateAssetMenu(menuName = "Menu Configuration/Transition", order = 1, fileName = "Menu Transition")]
	public class MenuTransition : ScriptableObject
	{
		[SerializeField]
		private AnimationCurve m_curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		[SerializeField]
		private float m_duration = 0.33f;

		public AnimationCurve curve
		{
			get { return m_curve; }
		}

		public float duration
		{
			get { return m_duration; }
		}
	}
}