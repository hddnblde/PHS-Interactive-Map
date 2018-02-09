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

		private CanvasGroup m_canvasGroup = null;
		public delegate void Action();
		public event Action OnShow;
		public event Action OnHide;
		#endregion


		#region Property
		public bool visible
		{
			get { return m_visible; }
		}
		#endregion


		#region MonoBehaviour Implementation
		protected virtual void Awake()
		{
			Initialize();
		}

		protected virtual void OnValidate()
		{
			SetVisible(m_visible);
		}
		#endregion

		
		#region Methods
		private void Initialize()
		{
			InitializeCanvasGroup();
			SetVisible(m_visible);
		}

		private void InitializeCanvasGroup()
		{
			CanvasGroup m_canvasGroup = GetComponent<CanvasGroup>();
			
			if(m_canvasGroup == null)
				m_canvasGroup = gameObject.AddComponent<CanvasGroup>();
		}

		protected void SetVisible(bool value)
		{
			if(value)
			{
				if(OnShow != null)
					OnShow();
			}
			else
			{
				if(OnHide != null)
					OnHide();
			}

			if(m_canvasGroup == null)
				return;

			m_canvasGroup.alpha = (value ? 1f : 0f);
			m_canvasGroup.blocksRaycasts = value;
		}
		#endregion
	}
}