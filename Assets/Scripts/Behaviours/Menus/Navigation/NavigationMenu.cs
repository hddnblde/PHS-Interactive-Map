using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Menus
{
	public class NavigationMenu : MonoBehaviour
	{
		public delegate void ContextSelect(Context context);
		public static event ContextSelect OnContextSelect;

		public enum Context
		{
			Info,
			Map,
			Menu
		}

		[Header("Animation")]
		[SerializeField, ColorUsage(false)]
		private Color normalColor =  new Color(0.458f, 0.458f, 0.458f);

		[SerializeField, ColorUsage(false)]
		private Color highlightColor = new Color(0.26f, 0.52f, 0.956f);

		[SerializeField]
		private Color pressedColor = Color.white;

		[SerializeField, ColorUsage(false)]
		private Color backgroundColor = Color.white;

		[SerializeField]
		private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		[Header("GUI References")]
		[SerializeField]
		private Graphic background = null;

		[SerializeField]
		private Image backgroundOverlay = null;

		[SerializeField]
		private NavigationButton infoButton = null;

		[SerializeField]
		private NavigationButton mapButton = null;

		[SerializeField]
		private NavigationButton menuButton = null;

		[Header("Menu References")]
		[SerializeField]
		private CanvasGroup infoCanvasGroup = null;
		
		[SerializeField]
		private CanvasGroup mapCanvasGroup = null;
		
		[SerializeField]
		private CanvasGroup menuCanvasGroup = null;

		private Coroutine backgroundOverlayTransitionRoutine = null;
		private const Context DefaultContext = Context.Map;
		private Context currentContext = Context.Map;

		private void Awake()
		{
			Initialize();
			SelectContext(DefaultContext);
		}

		private void Start()
		{
			SelectContext(currentContext);
		}

		private void OnValidate()
		{
			Initialize();
		}

		private void OnEnable()
		{
			RegisterEvents();
		}

		private void OnDisable()
		{
			DeregisterEvents();
		}

		#region Methods
		private void Initialize()
		{
			DisableImmersiveModeForAndroid();

			SetButtonColor(infoButton);
			SetButtonColor(mapButton);
			SetButtonColor(menuButton);
			SetBackgroundColor();
		}

		private void RegisterEvents()
		{
			RegisterButtonEvent(infoButton);
			RegisterButtonEvent(mapButton);
			RegisterButtonEvent(menuButton);
		}

		private void DeregisterEvents()
		{
			DeregisterButtonEvent(infoButton);
			DeregisterButtonEvent(mapButton);
			DeregisterButtonEvent(menuButton);
		}

		private void SelectContext(Context context)
		{
			currentContext = context;

			HighlightButton(infoButton, context == Context.Info);
			HighlightButton(mapButton, context == Context.Map);
			HighlightButton(menuButton, context == Context.Menu);

			bool showBackground = context != Context.Map;
			ShowBackground(showBackground);

			if(OnContextSelect != null)
				OnContextSelect(context);

			ShowMenu(infoCanvasGroup, context == Context.Info);
			ShowMenu(mapCanvasGroup, context == Context.Map);
			ShowMenu(menuCanvasGroup, context == Context.Menu);
		}

		private void ShowMenu(CanvasGroup menu, bool shown)
		{
			if(menu == null)
				return;

			menu.alpha = (shown ? 1f : 0f);
			menu.blocksRaycasts = shown;
			// menu.interactable = shown;
		}

		private void DisableImmersiveModeForAndroid()
		{
			// removes Immersive Mode for Android
			// this is to mitigate the problem with
			// custom bottom ui
			Screen.fullScreen = false;
		}
		#endregion


		#region Button Helpers
		private void RegisterButtonEvent(NavigationButton button)
		{
			if(button != null)
				button.OnContextSelect += SelectContext;
		}

		private void DeregisterButtonEvent(NavigationButton button)
		{
			if(button != null)
				button.OnContextSelect -= SelectContext;
		}

		private void SetButtonColor(NavigationButton button)
		{
			if(button != null)
				button.Initialize(normalColor, highlightColor, pressedColor, transitionCurve);
		}

		private void SetBackgroundColor()
		{
			if(background != null)
				background.color = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, background.color.a);

			if(backgroundOverlay != null)
				backgroundOverlay.color = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, backgroundOverlay.color.a);
		}

		private void HighlightButton(NavigationButton button, bool highlighted)
		{
			if(button != null)
				button.Select(highlighted);
		}
		#endregion


		#region Background Helpers
		public void ShowBackground(bool show)
		{
			if(backgroundOverlayTransitionRoutine != null)
				StopCoroutine(backgroundOverlayTransitionRoutine);

			if(backgroundOverlay == null)
				return;
			
			backgroundOverlay.raycastTarget = show;
			backgroundOverlayTransitionRoutine = StartCoroutine(BackgroundOverlayTransition(show));
		}

		private IEnumerator BackgroundOverlayTransition(bool show)
		{
			const float duration = 0.15f;
			float a = backgroundOverlay.color.a;
			float b =(show ? 1f : 0f);

			for(float current = 0f; current < duration; current += Time.deltaTime)
			{
				float t = Mathf.InverseLerp(0f, duration, current);
				float alpha = Mathf.Lerp(a, b, transitionCurve.Evaluate(t));
				backgroundOverlay.color = new Color(backgroundOverlay.color.r, backgroundOverlay.color.g, backgroundOverlay.color.b, alpha);
				yield return null;
			}

			backgroundOverlay.color = new Color(backgroundOverlay.color.r, backgroundOverlay.color.g, backgroundOverlay.color.b, b);
		}
		#endregion
	}
}