using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ModestUI.Behaviour;

public class UltimateApplicationKiller : MonoBehaviour
{
	[SerializeField]
	private Button exitButton = null;

	private void Awake()
	{
		if(exitButton != null)
			exitButton.onClick.AddListener(QuitApplication);

		MobileInput.OnBackButtonPressed += OnBackButtonPressed;
	}

	private void OnBackButtonPressed()
	{
		int visiblePanels = PanelBehaviour.VisiblePanels;
	
		if(PanelBehaviour.VisiblePanels <= 0)
			QuitApplication();
	}

	private void QuitApplication()
	{
		Application.Quit();
	}
}