using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModestUI.Panels;

namespace Menus.PHS
{
	public class SlidingPanel : SimplePanel
	{
		private enum Axis
		{
			Horizontal,
			Vertical
		}

		[SerializeField]
		private Axis axis = Axis.Horizontal;

		[SerializeField]
		private RectTransform container = null;

		[SerializeField]
		private AnimationCurve slidingCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		private Coroutine slidingRoutine = null;

		public override bool Open()
		{
			if(!base.Open())
				return false;

			BeginSlideContainerRoutine(true);
			return true;
		}

		public override bool Close()
		{
			if(!base.Close())
				return false;
			
			BeginSlideContainerRoutine(false);
			return true;
		}

		private void BeginSlideContainerRoutine(bool shown)
		{
			if(slidingRoutine != null)
				StopCoroutine(slidingRoutine);

			slidingRoutine = StartCoroutine(SlideContainerRoutine(shown));
		}

		private IEnumerator SlideContainerRoutine(bool shown)
		{
			float a = (shown ? 0f : 1f);
			float b = (shown ? 1f : 0f);

			for(float current = transitionDuration; current > 0f; current -= Time.deltaTime)
			{
				float t = Mathf.InverseLerp(transitionDuration, 0f, current);
				float c = Mathf.Lerp(a, b, t);
				SlideContainer(c);
				yield return null;
			}

			SlideContainer(b);
		}

		private void SlideContainer(float t)
		{
			if(container == null)
				return;

			t = slidingCurve.Evaluate(t);
			container.anchoredPosition = CalculateAnchoredPosition(t);
		}

		private Vector2 CalculateAnchoredPosition(float t)
		{
			float target = (axis == Axis.Horizontal ? container.rect.width : container.rect.height);
			float value = Mathf.Lerp(-target, 0f, t);

			return (axis == Axis.Horizontal ? Vector2.right * value : Vector2.up * value);
		}
	}
}