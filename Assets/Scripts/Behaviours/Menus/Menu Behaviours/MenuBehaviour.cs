using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.New
{
	[RequireComponent(typeof(CanvasGroup))]
	public abstract class MenuBehaviour : MonoBehaviour
	{
		public delegate void Action();
		private CanvasGroup canvasGroup = null;
		private bool m_shown = false;

		public bool shown
		{
			get { return m_shown; }
		}

		private void Awake()
		{
			canvasGroup = GetComponent<CanvasGroup>();
		}

		protected internal void Show()
		{
			if(m_shown)
			{
				Debug.Log("Already shown.");
				return;
			}

			SetCanvas(true);
		}

		protected internal void Hide()
		{
			if(!m_shown)
			{
				Debug.Log("Already hidden.");
				return;
			}

			SetCanvas(false);
		}

		private void SetCanvas(bool shown)
		{
			this.m_shown = shown;

			if(canvasGroup == null)
				return;

			canvasGroup.alpha = (shown ? 1f : 0f);
			canvasGroup.blocksRaycasts = shown;
		}
	}
}