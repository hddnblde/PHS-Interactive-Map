using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menus.Configurations
{
	/// <summary>
	/// The color theme containing color sets that are used within a menu.
	/// </summary>
	[CreateAssetMenu(menuName = "Menu Configuration/Colors", order = 0, fileName = "Menu Colors")]
	public class MenuColors : ScriptableObject
	{
		#region Data Structure
		/// <summary>
		/// The color set commonly used for menus.
		/// </summary>
		[System.Serializable]
		public class Set
		{
			public static Set DefaultNormalSet
			{
				get
				{
					Color foreground = new Color32(117, 117, 117, 255);
					Color background = Color.white;
					return new Set(foreground, background);
				}
			}

			public static Set DefaultHighlightSet
			{
				get
				{
					Color foreground = Color.white;
					Color background = new Color32(66, 133, 244, 255);
					return new Set(foreground, background);
				}
			}

			public Set(Color foreground, Color background)
			{
				m_foreground = foreground;
				m_background = background;
			}

			[SerializeField, Tooltip("The color used for text and icons.")]
			private Color m_foreground = Color.gray;

			[SerializeField, Tooltip("The color used for buttons and panels.")]
			private Color m_background = Color.white;

			/// <summary>
			/// The color used for text and icons.
			/// </summary>
			public Color foreground
			{
				get { return m_foreground; }
			}

			/// <summary>
			/// The color used for buttons and panels.
			/// </summary>
			public Color background
			{
				get { return m_background; }
			}
		}
		#endregion

		#region Fields and Properties
		[SerializeField, Tooltip("The color set when a ui element is idle.")]
		private Set m_normalSet = Set.DefaultNormalSet;

		[SerializeField, Tooltip("The color set when a ui element is highlighted")]
		private Set m_highlightSet = Set.DefaultHighlightSet;

		/// <summary>
		/// The color set when a ui element is idle.
		/// </summary>
		public Set normalSet
		{
			get { return m_normalSet; }
		}

		/// <summary>
		/// The color set when a ui element is highlighted.
		/// </summary>
		public Set highlightSet
		{
			get { return m_highlightSet; }
		}
		#endregion
	}
}