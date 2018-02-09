using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModestUI.Behaviour
{
	[RequireComponent(typeof(CanvasGroup))]
	public abstract class PanelBehaviour : MonoBehaviour
	{
		#region Fields
		[SerializeField]
		private bool m_visible = true;

		private CanvasGroup canvasGroup = null;
		public delegate void Action();
		public event Action OnShow;
		public event Action OnHide;
		#endregion


		#region Property
		protected bool visible
		{
			get { return m_visible; }
			set
			{
				m_visible = value;
				Show(m_visible);
			}
		}
		#endregion


		#region MonoBehaviour Implementation
		private void Start()
		{
			Initialize();
		}

		private void OnValidate()
		{
			visible = m_visible;
			Debug.Log("Validating");
		}
		#endregion

		
		#region Methods
		private void Initialize()
		{
			canvasGroup = GetComponent<CanvasGroup>();
			Show(m_visible);
			Debug.Log("Initializing Panel Behaviour");
		}

		private void Show(bool shown)
		{
			if(shown)
			{
				if(OnShow != null)
					OnShow();
			}
			else
			{
				if(OnHide != null)
					OnHide();
			}

			if(canvasGroup == null)
				return;

			canvasGroup.alpha = (shown ? 1f : 0f);
			canvasGroup.blocksRaycasts = shown;
		}
		#endregion
	}
}