using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
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
	#endregion


	#region Serialized Fields
	[Header("Inputs")]
	[SerializeField]
	private Button primaryButton = null;

	[SerializeField]
	private Button secondaryButton = null;

	[SerializeField]
	private InputField textField = null;

	[Header("Images")]
	[SerializeField]
	private Image background = null;
	#endregion


	#region Hidden Fields
	private bool m_catchTextInput = true;
	#endregion


	#region MonoBehaviour Implementation
	private void Awake()
	{
		Initialize();
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
	#endregion
}
