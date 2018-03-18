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

		[SerializeField]
		private float m_transitionDuration = 0.15f;

		private CanvasGroup m_canvasGroup = null;
		public delegate void Action();
		public event Action OnShow;
		public event Action OnHide;

		private Coroutine transitionRoutine = null;

		private static int visiblePanels = 0;
		public static int VisiblePanels
		{
			get { return visiblePanels; }
		}
		#endregion


		#region Property
		public bool visible
		{
			get { return m_visible; }
		}

		public float transitionDuration
		{
			get { return m_transitionDuration; }
		}
		#endregion


		#region MonoBehaviour Implementation
		protected virtual void Awake()
		{
			Initialize();
			visiblePanels = 0;
		}

		protected virtual void OnValidate()
		{
			SetVisible(m_visible, true);
		}
		#endregion

		
		#region Methods
		private void Initialize()
		{
			InitializeCanvasGroup();
			SetVisible(m_visible, true);
		}

		private void InitializeCanvasGroup()
		{
			m_canvasGroup = GetComponent<CanvasGroup>();
			
			if(m_canvasGroup == null)
				m_canvasGroup = gameObject.AddComponent<CanvasGroup>();
		}

		protected void SetVisible(bool value)
		{
			SetVisible(value, false);
		}

		protected void SetVisible(bool value, bool immediately)
		{
			m_visible = value;
			visiblePanels += (value ? 1 : -1);
			
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

			BeginTransition(value, immediately);
		}

		private void BeginTransition(bool shown, bool immediately)
		{
			if(transitionRoutine != null)
				StopCoroutine(transitionRoutine);

			transitionRoutine = StartCoroutine(TransitionRoutine(shown, immediately));
		}

		private IEnumerator TransitionRoutine(bool shown, bool immediately)
		{
			if(m_canvasGroup == null)
				yield break;

			float a = (shown ? 0f : 1f);
			float b = (shown ? 1f : 0f);

			if(immediately)
			{
				m_canvasGroup.blocksRaycasts = shown;
				m_canvasGroup.alpha = b;
				yield break;
			}

			m_canvasGroup.blocksRaycasts = shown;

			for(float current = m_transitionDuration; current > 0f; current -= Time.deltaTime)
			{
				float t = Mathf.InverseLerp(m_transitionDuration, 0f, current);
				float alpha = Mathf.Lerp(a, b, t);
				m_canvasGroup.alpha = alpha;
				yield return null;
			}

			m_canvasGroup.alpha = b;
		}
		#endregion
	}
}