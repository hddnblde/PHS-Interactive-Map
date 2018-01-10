using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Menus
{
	public class NavigationButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
	{
		public delegate void ContextSelect(NavigationMenu.Context context);
		public event ContextSelect OnContextSelect;

		[Header("Navigation")]
		[SerializeField]
		private NavigationMenu.Context context = NavigationMenu.Context.Map;

		[SerializeField]
		private Graphic background = null;

		[SerializeField]
		private Image thumbnail = null;

		[SerializeField]
		private Text text = null;

		private bool selected = false;
		private AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
		private Color pressed = Color.white;
		private Color normal = Color.clear;
		private Color highlight = Color.clear;

		private Coroutine transitionBackgroundRoutine = null;
		private Coroutine transitionIconRoutine = null;

		public void Initialize(Color normal, Color highlight, Color pressed, AnimationCurve curve)
		{
			this.normal = normal;
			this.highlight = highlight;
			this.pressed = pressed;
			this.curve = curve;

			Select(false);
		}

		public void Select(bool selected)
		{
			this.selected = selected;
			Color color = (selected ? highlight : normal);
			SetColors(color);
			TransitionIcon(selected);
		}

		private void SetColors(Color color)
		{
			if(thumbnail != null)
				thumbnail.color = color;

			if(text != null)
				text.color = new Color(color.r, color.g, color.b, text.color.a);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if(selected)
				return;
			
			TransitionBackground(true);
			SetColors(pressed);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if(selected)
				return;
			
			TransitionBackground(false);
			Color color = (selected ? highlight : normal);
			SetColors(color);
		}

		public void OnPointerClick (PointerEventData eventData)
		{
			if(selected)
				return;
			
			if(OnContextSelect != null)
				OnContextSelect(context);

			TransitionBackground(false);
		}

		private void TransitionBackground(bool highlighted)
		{
			if(transitionBackgroundRoutine != null)
				StopCoroutine(transitionBackgroundRoutine);

			transitionBackgroundRoutine = StartCoroutine(TransitionBackgroundRoutine(highlighted));
		}

		private void TransitionIcon(bool highlighted)
		{
			if(transitionIconRoutine != null)
				StopCoroutine(transitionIconRoutine);

			transitionIconRoutine = StartCoroutine(TransitionIconRoutine(highlighted));
		}

		private IEnumerator TransitionBackgroundRoutine(bool highlighted)
		{
			const float Duration = 0.35f;
			Color clearColor = new Color(highlight.r, highlight.g, highlight.b, 0f);
			Color a = (highlighted ? clearColor : highlight);
			Color b = (highlighted ? highlight : clearColor);

			for(float current = 0f; current < Duration; current += Time.deltaTime)
			{
				float t = Mathf.InverseLerp(0f, Duration, current);

				if(background != null)
					background.color = Color.Lerp(a, b, t);
				yield return null;
			}

			if(background != null)
			background.color = b;
		}

		private IEnumerator TransitionIconRoutine(bool highlighted)
		{
			if(text != null && thumbnail != null)
			{
				const float Duration = 0.3f;
				const float IconLowerSize = 0.8f;
				const float IconUpperSize = 1f;

				float textAlphaA = text.color.a;
				float textAlphaB = (highlighted ? 1f : 0f);
				float thumbScaleA = thumbnail.rectTransform.localScale.x;
				float thumbScaleB = (highlighted ? IconUpperSize : IconLowerSize);

				for(float current = 0f; current < Duration; current += Time.deltaTime)
				{
					float t = Mathf.InverseLerp(0f, Duration, current);
					text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Lerp(textAlphaA, textAlphaB, t));
					thumbnail.rectTransform.localScale = Vector3.one * Mathf.Max(Mathf.LerpUnclamped(thumbScaleA, thumbScaleB, curve.Evaluate(t)), IconLowerSize);
					yield return null;
				}

				text.color = new Color(text.color.r, text.color.g, text.color.b, textAlphaB);
				thumbnail.rectTransform.localScale = Vector3.one * thumbScaleB;
			}
		}
	}
}