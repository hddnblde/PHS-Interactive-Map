﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Menu
{
	public class MenuLayout : MonoBehaviour
	{
		#region Data Structures
		private enum ButtonType
		{
			Primary,
			Secondary
		}

		public delegate void ButtonClick();
		public delegate void TextInput(string text);

		public static event ButtonClick OnPrimaryClick;
		public static event ButtonClick OnSecondaryClick;
		public static event TextInput OnTextInput;
		public static event TextInput OnTextEndInput;
		#endregion


		#region Serialized Fields
		[Header("Colors")]
		[SerializeField]
		private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		[SerializeField, ColorUsage(false)]
		private Color foreground1 = Color.black;

		[SerializeField, ColorUsage(false)]
		private Color foreground2 = Color.gray;

		[SerializeField]
		private Color backgroundColor = Color.white;

		[Header("Graphics")]
		[SerializeField]
		private Button primaryButton = null;

		[SerializeField]
		private Button secondaryButton = null;

		[SerializeField]
		private InputField textField = null;

		[SerializeField]
		private Image background = null;

		[SerializeField]
		private Image textUnderline = null;

		[SerializeField]
		private Image headerUnderline = null;

		[Header("Sprites")]
		[SerializeField]
		private Sprite searchSprite = null;

		[SerializeField]
		private Sprite clearSprite = null;

		[SerializeField]
		private List<Sprite> menuSpriteSheet = null;
		#endregion


		#region Hidden Fields
		private const float TransitionTime = 0.29f;
		private bool m_showBackground = false;
		private bool m_catchTextInput = true;
		private Coroutine backgroundTransitionRoutine = null;
		#endregion


		#region Properties
		public bool backgroundShown
		{
			get { return m_showBackground; }
		}
		#endregion

		#region MonoBehaviour Implementation
		private void Awake()
		{
			Initialize();
			ShowBackground(m_showBackground);
		}
		#endregion


		#region Methods
		private void Initialize()
		{
			RegisterInputEvents();
		}
		#endregion


		#region Input Implementation
		private void RegisterInputEvents()
		{
			RegisterInput(primaryButton, ButtonType.Primary);
			RegisterInput(secondaryButton, ButtonType.Secondary);
			RegisterInput(textField);
		}

		private void RegisterInput(Button button, ButtonType type)
		{
			if(button != null)
				button.onClick.AddListener(() => ButtonClickEvent(type));
		}

		private void RegisterInput(InputField textField)
		{
			if(textField != null)
			{
				textField.onValueChanged.AddListener(TextInputEvent);
				textField.onEndEdit.AddListener(TextInputEndEvent);
			}
		}

		private void ButtonClickEvent(ButtonType type)
		{
			if(type == ButtonType.Primary)
			{
				if(OnPrimaryClick != null)
					OnPrimaryClick();
			}
			else
			{
				if(OnSecondaryClick != null)
					OnSecondaryClick();
			}
		}

		private void TextInputEvent(string text)
		{
			if(m_catchTextInput && OnTextInput != null)
				OnTextInput(text);

			m_catchTextInput = true;
		}

		private void TextInputEndEvent(string text)
		{
			if(OnTextEndInput != null)
				OnTextEndInput(text);
		}
		#endregion


		#region Actions
		public void ClearText()
		{
			EnterText("", false);
		}

		public void EnterText(string text)
		{
			EnterText(text, true);
		}

		public void EnterText(string text, bool catchTextInput)
		{
			if(textField == null)
				return;

			m_catchTextInput = catchTextInput;
			textField.text = text;
		}

		public void ShowBackground(bool show)
		{
			m_showBackground = show;

			if(backgroundTransitionRoutine != null)
				StopCoroutine(backgroundTransitionRoutine);

			backgroundTransitionRoutine = StartCoroutine(TransitionBackgroundRoutine());
		}

		private void TransitionBackground(float t)
		{
			Color backgroundColor = Color.Lerp(Color.clear, this.backgroundColor, t);
			Color underlineColor = Color.Lerp(Color.clear, foreground2, t);

			SetColor(background, backgroundColor, false);
			SetColor(headerUnderline, underlineColor, false);
		}

		private void TransitionPrimaryButton(float t)
		{
			if(primaryButton == null || menuSpriteSheet == null || menuSpriteSheet.Count == 0)
				return;

			int index = Mathf.RoundToInt(Mathf.Lerp(0, menuSpriteSheet.Count - 1, t));
			primaryButton.image.sprite = menuSpriteSheet[index];
		}

		private void TransitionSecondaryButton(float t)
		{
			TransitionSecondaryButton(t, searchSprite, null);
		}

		private void TransitionSecondaryButton(float t, Sprite from, Sprite to)
		{
			if(secondaryButton == null)
				return;

			Image image = secondaryButton.image;

			bool firstHalf = t < 0.5f;
			float a = (firstHalf ? 1f : 0f);
			float b = (firstHalf ? 0f : 1f);
			Sprite sprite = (firstHalf ? from : to);

			if(sprite == null)
			{
				image.enabled = false;
				return;
			}

			image.enabled = true;
			Color color = image.color;
			color = new Color(color.r, color.g, color.b, Mathf.Lerp(a, b, t));

			image.color = color;
			image.sprite = sprite;
		}

		private void TransitionGraphicsToAlternateColor(float t)
		{
			Color targetColor = Color.Lerp(foreground1, foreground2, t);

			SetColor(primaryButton.image, targetColor);
			SetColor(secondaryButton.image, targetColor);
			SetColor(textField.textComponent, targetColor);
			SetColor(textField.placeholder, targetColor);
			SetColor(textUnderline, targetColor);
		}

		private void SetColor(Graphic graphic, Color color)
		{
			SetColor(graphic, color, true);
		}

		private void SetColor(Graphic graphic, Color color, bool useAlpha)
		{
			if(graphic != null)
				graphic.color = new Color(color.r, color.g, color.b, (useAlpha ? graphic.color.a : color.a));
		}
		#endregion


		#region Coroutines
		private IEnumerator TransitionBackgroundRoutine()
		{
			float a = (m_showBackground ? 0f : 1f);
			float b = (m_showBackground ? 1f : 0f);
			float c = Mathf.InverseLerp(a, b, background.color.a);

			for(float current = Mathf.Lerp(0f, TransitionTime, c); current < TransitionTime; current += Time.deltaTime)
			{
				float t = Mathf.Lerp(a, b, Mathf.InverseLerp(0f, TransitionTime, current));
				float curvedT = transitionCurve.Evaluate(t);

				TransitionGraphicsToAlternateColor(curvedT);
				TransitionPrimaryButton(t);
				TransitionSecondaryButton(t);
				TransitionBackground(curvedT);
				yield return null;
			}

			TransitionPrimaryButton(b);
			TransitionGraphicsToAlternateColor(b);
			TransitionBackground(b);
		}
		#endregion
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(MenuLayout))]
	public class MenuLayoutEditor : Editor
	{
		private MenuLayout menuLayout = null;

		private void OnEnable()
		{
			menuLayout = target as MenuLayout;
		}

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			if(!Application.isPlaying)
				return;
			
			EditorGUILayout.Space();
			DrawLayoutCommands();
		}

		private void DrawLayoutCommands()
		{
			EditorGUILayout.LabelField("Commands", EditorStyles.boldLabel);
			if(GUILayout.Button("Show Background"))
				menuLayout.ShowBackground(!menuLayout.backgroundShown);
		}
	}
	#endif
}