using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
	public class FloorMenu : MonoBehaviour
	{
		#region Serialized Field
		[SerializeField]
		private List<Button> buttons = new List<Button>();
		#endregion


		#region Hidden Fields
		public delegate void FloorSelect(int floor);
		public static event FloorSelect OnFloorSelect;

		private delegate void MenuAction();
		private static event MenuAction OnOpen;
		private static event MenuAction OnClose;

		private CanvasGroup canvasGroup = null;
		private static int m_currentFloor = 1;
		private static bool m_isOpen = false;
		#endregion


		#region Properties
		public static bool isOpen
		{
			get { return m_isOpen; }
		}

		public static int currentFloor
		{
			get { return m_currentFloor; }
		}
		#endregion


		#region MonoBehaviour Implementation
		private void Awake()
		{
			Initialize();
			RegisterButtonEvents();
		}

		private void OnEnable()
		{
			RegisterInternalEvents();
		}

		private void OnDisable()
		{
			DeregisterInternalEvents();
		}
		#endregion


		#region Initializers
		private void Initialize()
		{
			canvasGroup = GetComponent<CanvasGroup>();
		}

		private void RegisterButtonEvents()
		{
			for(int i = 0; i < buttons.Count; i++)
			{
				Button button = buttons[i];
				RegisterButtonEvent(button, i + 1);
			}
		}

		private void RegisterInternalEvents()
		{
			OnOpen += Internal_Open;
			OnClose += Internal_Close;
		}

		private void DeregisterInternalEvents()
		{
			OnOpen -= Internal_Open;
			OnClose -= Internal_Close;
		}
		#endregion


		#region Actions
		public static void Open()
		{
			if(m_isOpen)
			{
				Debug.Log("Floor menu is already open.");
				return;
			}

			if(OnOpen != null)
				OnOpen();
		}

		public static void Close()
		{
			if(!m_isOpen)
			{
				Debug.Log("Floor menu is already closed.");
				return;
			}

			if(OnClose != null)
				OnClose();
		}

		private void Internal_Open()
		{

		}

		private void Internal_Close()
		{

		}
		#endregion


		#region Helpers
		private void RegisterButtonEvent(Button button, int floorLevel)
		{
			if(button != null)
				button.onClick.AddListener(() => SelectFloor(floorLevel));
		}

		private void Show(bool shown)
		{
			if(canvasGroup == null)
				return;
			
			canvasGroup.alpha = (shown ? 1f : 0f);
			canvasGroup.blocksRaycasts = shown;
		}

		private void SelectFloor(int floor)
		{
			if(OnFloorSelect != null)
				OnFloorSelect(floor);

			m_currentFloor = floor;
		}
		#endregion
	}
}