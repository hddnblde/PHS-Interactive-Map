using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Navigation;

namespace Menus
{
	public class MarkerMenu : MonoBehaviour
	{
		#region Serialized Fields
		[SerializeField]
		private Text displayedText = null;

		[SerializeField]
		private Button quitButton = null;

		[SerializeField]
		private Button checkButton = null;

		[SerializeField]
		private RectTransform cursor = null;
		#endregion


		#region Hidden Fields
		private delegate void OpenAction(string displayedText, Mark markAction, Quit quitAction);
		private delegate void CloseAction();

		public delegate void Mark(LocationMarker marker);
		public delegate void Quit();

		private static event OpenAction OnOpenMenu;
		private static event CloseAction OnCloseMenu;

		private event Mark OnMark;
		private event Quit OnQuit;

		private CanvasGroup canvasGroup = null;		
		#endregion


		#region MonoBehaviour Implementation
		private void Awake()
		{
			Initialize();
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
			if(checkButton != null)
				checkButton.onClick.AddListener(CheckButtonClicked);
			
			if(quitButton != null)
				quitButton.onClick.AddListener(QuitButtonClicked);

			canvasGroup = GetComponent<CanvasGroup>();
		}

		private void RegisterInternalEvents()
		{
			OnOpenMenu += Internal_Open;
			OnCloseMenu += Internal_Close;
		}

		private void DeregisterInternalEvents()
		{
			OnOpenMenu -= Internal_Open;
			OnCloseMenu -= Internal_Close;
		}
		#endregion

		
		#region Events
		private void QuitButtonClicked()
		{
			if(OnQuit != null)
				OnQuit();

			Close();
		}

		private void CheckButtonClicked()
		{
			MarkByCursor();
			Close();
		}
		#endregion


		#region Actions
		public static void Open(string displayedText, Mark markAction, Quit quitAction)
		{
			if(OnOpenMenu != null)
				OnOpenMenu(displayedText, markAction, quitAction);
		}

		public static void Close()
		{
			if(OnCloseMenu != null)
				OnCloseMenu();
		}

		private void Internal_Open(string displayedText, Mark markAction, Quit quitAction)
		{
			this.displayedText.text = displayedText;
			OnMark = markAction;
			OnQuit = quitAction;
			Show(true);
		}

		private void Internal_Close()
		{
			OnMark = null;
			OnQuit = null;
			Show(false);
		}
		#endregion


		#region Helpers
		private Rect RectTransformToScreenSpace(RectTransform rectTransform)
		{
			Vector2 position = new Vector2(rectTransform.position.x, Screen.height - rectTransform.position.y);
			Vector2 size = Vector2.Scale(rectTransform.rect.size, rectTransform.lossyScale);
			return new Rect(position, size);
		}

		private void MarkByCursor()
		{
			if(cursor == null)
				return;

			Rect cursorRect = RectTransformToScreenSpace(cursor);
			Vector3 position = NavigationCamera.GetPosition(cursorRect.position);
			LocationMarker marker = new LocationMarker("Custom Marker", position);

			if(OnMark != null)
				OnMark(marker);
		}

		private void Show(bool shown)
		{
			if(canvasGroup == null)
				return;

			canvasGroup.alpha = (shown ? 1f : 0f);
			canvasGroup.blocksRaycasts = shown;
			canvasGroup.interactable = shown;
		}
		#endregion
	}
}