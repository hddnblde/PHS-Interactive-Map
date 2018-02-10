using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ModestUI.Panels;

namespace Menus.PHS
{
	public class FloorPanel : SlidingPanel
	{
		#region Data Structure
		[System.Serializable]
		private class FloorSelectionButton
		{
			[SerializeField]
			private int m_floor = 1;

			[SerializeField]
			private Button button = null;			

			[SerializeField]
			private Graphic checkIcon = null;

			public void AddListener(FloorSelect floorSelect)
			{
				if(button == null)
					return;

				button.onClick.AddListener(() => floorSelect(m_floor));
			}

			public void SetToggle(bool value)
			{
				if(checkIcon != null)
					checkIcon.enabled = value;
			}

			public int floor
			{
				get { return m_floor; }
			}
		}
		#endregion


		#region Serialized Field
		[SerializeField]
		private List<FloorSelectionButton> buttons = new List<FloorSelectionButton>();
		#endregion


		#region Unserialized Fields
		public delegate void FloorSelect(int floor);
		public event FloorSelect OnFloorSelect;
		#endregion


		#region MonoBehaviour Implementation
		protected override void Awake()
		{
			base.Awake();
			Initialize();
		}
		#endregion


		#region Floor Selection Implementation
		private void Initialize()
		{
			foreach(FloorSelectionButton button in buttons)
				button.AddListener(SelectFloor);
		}

		private void SelectFloor(int floor)
		{
			if(OnFloorSelect != null)
				OnFloorSelect(floor);

			SetToggle(floor);
		}

		private void SetToggle(int floor)
		{
			foreach(FloorSelectionButton button in buttons)
				button.SetToggle(button.floor == floor);
		}
		#endregion
	}
}