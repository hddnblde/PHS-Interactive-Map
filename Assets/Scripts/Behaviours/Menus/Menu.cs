using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Menus
{
	public class Menu : MonoBehaviour
	{
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

		[Header("References")]
		[SerializeField]
		private Graphic background = null;

		[SerializeField]
		private NavigationButton infoButton = null;

		[SerializeField]
		private NavigationButton mapButton = null;

		[SerializeField]
		private NavigationButton menuButton = null;

		private const Context DefaultContext = Context.Map;

		private void Awake()
		{
			Initialize();
			SelectContext(DefaultContext);
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

			if(background != null)
				background.color = backgroundColor;
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
			HighlightButton(infoButton, context == Context.Info);
			HighlightButton(mapButton, context == Context.Map);
			HighlightButton(menuButton, context == Context.Menu);
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

		private void HighlightButton(NavigationButton button, bool highlighted)
		{
			if(button != null)
				button.Select(highlighted);
		}
		#endregion
	}
}