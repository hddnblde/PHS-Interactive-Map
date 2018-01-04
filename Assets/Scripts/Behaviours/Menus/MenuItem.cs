using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menus.DataStructure
{
	[System.Serializable]
	public class MenuItem
	{
		[SerializeField]
		private string m_title = "Title";

		[SerializeField]
		private int m_depth = 0;

		[SerializeField]
		private bool m_toggleable = false;

		public string title
		{
			get { return m_title; }
		}

		public int depth
		{
			get { return m_depth; }
		}

		public bool toggleable
		{
			get { return m_toggleable; }
		}
	}
}