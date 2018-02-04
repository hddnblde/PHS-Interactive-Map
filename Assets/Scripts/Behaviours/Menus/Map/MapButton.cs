using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Menus.MapElement
{
	public class MapButton : MonoBehaviour
	{
		[SerializeField]
		private Button clearButton = null;

		[SerializeField]
		private Text displayedText = null;

		private Button selectionButton = null;
		private string placeholderText = "Choose marker";

		private void Awake()
		{
			Initialize();
		}

		private void Initialize()
		{
			selectionButton = GetComponent<Button>();

			if(clearButton != null)
				clearButton.onClick.AddListener(() => SetDisplayedText(placeholderText, true));
		}

		public void AddListener(UnityAction selectAction, UnityAction clearAction, string placeholder)
		{
			placeholderText = placeholder;

			if(selectionButton != null)
				selectionButton.onClick.AddListener(() => { selectAction(); ResetSelectionButton(); });

			if(clearButton != null)
				clearButton.onClick.AddListener(() => { clearAction(); ResetSelectionButton(); } );
		}

		private void ResetSelectionButton()
		{
			selectionButton.enabled = false;
			selectionButton.enabled = true;
		}

		public void SetDisplayedText(string text, bool isEmpty)
		{
			isEmpty |= string.IsNullOrEmpty(text);

			if(isEmpty)
				text = placeholderText;

			if(displayedText != null)
			{
				displayedText.text = text;
				displayedText.CrossFadeAlpha((isEmpty ? 0.75f : 1f), 0.1f, true);
			}

			if(clearButton != null)
				clearButton.gameObject.SetActive(!isEmpty);
		}
	}
}