using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ModestUI.Panels;

namespace Menus.PHS
{
	public class FloorPanel : SimplePanel
	{
		#region Data Structure
		[System.Serializable]
		private class FloorSelectionButton
		{
			[SerializeField]
			private Button button = null;

			[SerializeField]
			private int floor = 1;

			public void AddListener(FloorSelect floorSelect)
			{
				if(button == null)
					return;

				button.onClick.AddListener(() => floorSelect(floor));
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
		}
		#endregion
	}
}