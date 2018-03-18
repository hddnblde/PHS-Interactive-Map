using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace ModestUI.Behaviour
{
	public abstract class ButtonBehaviour : Button
	{
		#region Serialized Fields
		[SerializeField]
		private List<Graphic> targetGraphics = new List<Graphic>();

		[SerializeField]
		private Graphic pressHighlight = null;


		[Header("Transition Settings")]
		[SerializeField]
		private float transitionDuration = 0.1f;

		[SerializeField]
		private float pressHighlightDuration = 0.5f;

		[SerializeField]
		private AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		[SerializeField]
		private Color normalColor = Color.black;

		[SerializeField]
		private Color pressedColor = Color.white;
		#endregion


		#region Unserialized Fields
		private Coroutine transitionRoutine = null;
		private Coroutine highlightPressRoutine = null;
		private bool pressed = false;
		#endregion


		#region MonoBehaviour Implementation
		protected override void OnEnable()
		{
			base.OnEnable();
			TransitionGraphics(0f);
			TransitionPressHighlight(1f);
		}

		#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();
			TransitionGraphics(0f);
		}
		#endif
		#endregion


		#region Pointer Implementation
		public override void OnPointerDown(PointerEventData eventData)
		{
			pressed = true;			
			base.OnPointerDown(eventData);			
			BeginTransitionGraphicsRoutine(true);
		}

		public override void OnPointerUp(PointerEventData eventData)
   		{
			pressed = false;
			base.OnPointerUp(eventData);

			if(!PointerIsWithinRect(eventData.position))
				return;
			
			BeginTransitionGraphicsRoutine(false);
		}

		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);

			if(pressed)
				TransitionGraphics(1f);
			else
				TransitionGraphics(0f);
		}

		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);			
			TransitionGraphics(0f);
		}

		public override void OnPointerClick(PointerEventData eventData)
		{
			base.OnPointerClick(eventData);
			BeginHighlightPressRoutine();
		}
		#endregion	


		#region Methods
		private void BeginTransitionGraphicsRoutine(bool pressed)
		{
			if(transitionRoutine != null)
				StopCoroutine(transitionRoutine);

			if(!gameObject.activeInHierarchy)
				return;

			transitionRoutine = StartCoroutine(TransitionGraphicsRoutine(pressed));
		}

		private void BeginHighlightPressRoutine()
		{
			if(highlightPressRoutine != null)
				StopCoroutine(highlightPressRoutine);

			if(!gameObject.activeInHierarchy)
				return;

			highlightPressRoutine = StartCoroutine(HighlightPressRoutine());
		}
		#endregion


		#region Coroutine
		private IEnumerator TransitionGraphicsRoutine(bool pressed)
		{
			float a = (pressed ? 0f : 1f);
			float b = (pressed ? 1f : 0f);

			for(float current = transitionDuration; current > 0f; current -= Time.deltaTime)
			{
				float t = Mathf.InverseLerp(transitionDuration, 0f, current);
				float c = Mathf.Lerp(a, b, t);
				TransitionGraphics(c);
				yield return null;
			}

			TransitionGraphics(b);
		}

		private IEnumerator HighlightPressRoutine()
		{
			for(float current = pressHighlightDuration; current > 0f; current -= Time.deltaTime)
			{
				float t = Mathf.InverseLerp(pressHighlightDuration, 0f, current);
				TransitionPressHighlight(t);
				yield return null;
			}

			TransitionPressHighlight(1f);
		}
		#endregion


		#region Helpers
		private bool PointerIsWithinRect(Vector2 position)
		{
			return RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, position);
		}

		private void TransitionGraphics(float t)
		{
			t = curve.Evaluate(t);
			Color targetColor = Color.Lerp(normalColor, pressedColor, t);

			foreach(Graphic targetGraphic in targetGraphics)
				SetGraphicColor(targetGraphic, targetColor);
		}

		private void SetGraphicColor(Graphic graphic, Color color)
		{
			if(graphic != null)
				graphic.color = color;
		}

		private void TransitionPressHighlight(float t)
		{
			if(pressHighlight == null)
				return;

			const float alphaLowerBound = 0.35f;
			const float alphaUpperBound = 0f;
			float targetAlpha = Mathf.Lerp(alphaLowerBound, alphaUpperBound, t);

			const float scaleLowerBound = 0.95f;
			const float scaleUpperBound = 1.35f;
			float targetScale = Mathf.Lerp(scaleLowerBound, scaleUpperBound, t);
			
			pressHighlight.color = new Color(pressHighlight.color.r, pressHighlight.color.g, pressHighlight.color.b, targetAlpha);
			pressHighlight.transform.localScale = Vector3.one * targetScale;
		}
		#endregion
	}

	
}