using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Menus
{
	public class NavigationButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
	{
		public delegate void ContextSelect(Menu.Context context);
		public event ContextSelect OnContextSelect;

		[Header("Navigation")]
		[SerializeField]
		private Menu.Context context = Menu.Context.Map;

		[SerializeField]
		private Graphic background = null;

		[SerializeField]
		private Color pressedColor = Color.white;

		[SerializeField]
		private Graphic[] graphics = null;

		private Color normal = Color.clear;
		private Color highlight = Color.clear;

		public void InitializeColors(Color normal, Color highlight)
		{
			this.normal = normal;
			this.highlight = highlight;

			SetActive(false);
		}

		public void SetActive(bool active)
		{
			Color color = (active ? highlight : normal);
			SetColors(color);
		}

		private void SetColors(Color color)
		{
			foreach(Graphic graphic in graphics)
				graphic.color = color;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			TransitionBackground(true);
			SetColors(pressedColor);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			TransitionBackground(false);
		}

		public void OnPointerClick (PointerEventData eventData)
		{
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

		private Coroutine transitionBackgroundRoutine = null;
		private IEnumerator TransitionBackgroundRoutine(bool highlighted)
		{
			const float Duration = 0.35f;
			Color clearColor = new Color(highlight.r, highlight.g, highlight.b, 0f);
			Color a = (highlighted ? clearColor : highlight);
			Color b = (highlighted ? highlight : clearColor);

			for(float current = 0f; current < Duration; current += Time.deltaTime)
			{
				float t = Mathf.InverseLerp(0f, Duration, current);
				background.color = Color.Lerp(a, b, t);
				yield return null;
			}

			background.color = b;
		}
	}
}