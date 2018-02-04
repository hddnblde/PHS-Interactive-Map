using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Menus.Configurations;

namespace Menus.Buttons
{
	public class MenuButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		#region Serialized Fields
		[SerializeField]
		private MenuTransition m_transition = null;

		[SerializeField]
		private MenuColors m_colors = null;

		[Header("Graphics")]
		[SerializeField]
		private List<UnityEngine.UI.Graphic> m_foreground = new List<UnityEngine.UI.Graphic>();

		[SerializeField]
		private List<UnityEngine.UI.Graphic> m_background = new List<UnityEngine.UI.Graphic>();
		#endregion


		#region Hidden Fields
		private bool m_highlighted = false;
		private bool m_interactable = true;
		private float pressRatio = 0f;

		private delegate void PressEvent(float ratio);
		private event PressEvent OnPress;

		public delegate void ClickEvent();
		public event ClickEvent OnClick;

		private RectTransform rectTransform = null;
		private Coroutine transitionRoutine = null;
		#endregion


		#region Properties
		public bool highlighted
		{
			get { return m_highlighted; }
			set
			{
				m_highlighted = value;
				StartTransition(m_highlighted);
			}
		}

		public bool interactable
		{
			get { return m_interactable; }
			set { m_interactable = value; }
		}
		#endregion


		#region MonoBehaviour Implementation
		private void Awake()
		{
			rectTransform = transform as RectTransform;
			highlighted = true;
		}

		private void OnEnable()
		{
			OnPress += Transition;
		}

		private void OnDisable()
		{
			OnPress -= Transition;
		}
		#endregion


		#region PointerEvent Implementation
		public void OnPointerDown(PointerEventData eventData)
		{
			if(!m_interactable)
				return;

			if(!m_highlighted)
				StartTransition(true);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if(!m_interactable)
				return;
			
			if(!m_highlighted)
				StartTransition(false);

			bool clicked = RectTransformUtility.RectangleContainsScreenPoint(rectTransform, eventData.position);
			if(clicked && OnClick != null)
				OnClick();
		}
		#endregion


		#region Helpers
		private void SetPressRatio(float ratio)
		{
			this.pressRatio = ratio;

			if(OnPress != null)
				OnPress(ratio);
		}

		protected void StartTransition(bool pressed)
		{
			if(transitionRoutine != null)
				StopCoroutine(transitionRoutine);

			transitionRoutine = StartCoroutine(TransitionRoutine(pressed));
		}
		#endregion


		#region Coroutines
		private IEnumerator TransitionRoutine(bool pressed)
		{
			if(m_transition == null)
				yield break;
			
			float a = (pressed ? 0f : 1f);
			float b = (pressed ? 1f : 0f);
			float duration = m_transition.duration;
			AnimationCurve curve = m_transition.curve;

			for(float currentDuration = 0; currentDuration < duration; currentDuration += Time.deltaTime)
			{
				float c = curve.Evaluate(Mathf.InverseLerp(0f, duration, currentDuration));
				float transitionTime = Mathf.Lerp(a, b, c);
				SetPressRatio(transitionTime);				
				yield return null;
			}

			SetPressRatio(b);
		}
		#endregion


		#region Button Methods
		private void Transition(float ratio)
		{
			TransitionForeground(ratio);
			TransitionBackground(ratio);
		}

		private void TransitionForeground(float pressRatio)
		{
			if(m_colors == null)
				return;

			Color color = Color.Lerp(m_colors.normalSet.foreground, m_colors.highlightSet.foreground, pressRatio);
			SetForegroundColor(color);
		}

		protected void TransitionBackground(float pressRatio)
		{
			if(m_colors == null)
				return;
			
			Color color = Color.Lerp(m_colors.normalSet.background, m_colors.highlightSet.background, pressRatio);
			SetBackgroundColor(color);
		}

		private void SetForegroundColor(Color color)
		{
			SetGraphicsColor(m_foreground, color);
		}

		private void SetBackgroundColor(Color color)
		{
			SetGraphicsColor(m_background, color);
		}

		private void SetGraphicsColor(List<UnityEngine.UI.Graphic> graphics, Color color)
		{
			foreach(UnityEngine.UI.Graphic graphic in graphics)
				graphic.color = color;
		}
		#endregion
	}
}