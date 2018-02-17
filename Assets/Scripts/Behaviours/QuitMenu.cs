using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ModestUI.Behaviour;
using ModestUI.Panels;

public class QuitMenu : MonoBehaviour
{
	[Header("Quit Transition")]
	[SerializeField]
	private Image blocker = null;

	[SerializeField]
	private float blockTransitionDuration = 0.75f;

	[Header("References")]
	[SerializeField]
	private CanvasGroup menuPanel = null;

	[SerializeField]
	private Button exitButton = null;

	[SerializeField]
	private ContextPanel quitPanel = null;

	private Coroutine transitionMenuPanelRoutine = null;
	private Coroutine quitRoutine = null;

	private void Awake()
	{
		if(exitButton != null)
			exitButton.onClick.AddListener(QuitApplication);

		if(quitPanel != null)
		{
			quitPanel.OnConfirm += BeginQuit;
			quitPanel.OnShow += () => TransitionMenuPanel(false);
			quitPanel.OnHide += () => TransitionMenuPanel(true);
		}

		MobileBackButton.OnBackButtonPressed += OnBackButtonPressed;
	}

	private void Start()
	{
		if(blocker != null)
			blocker.CrossFadeAlpha(0f, 1.5f, true);
	}

	private void OnBackButtonPressed()
	{
		int visiblePanels = PanelBehaviour.VisiblePanels;
	
		if(PanelBehaviour.VisiblePanels <= 0)
			QuitApplication();
	}

	private void QuitApplication()
	{
		if(quitPanel != null)
			quitPanel.Open("Quit application?");
	}

	private void BeginQuit()
	{
		if(quitRoutine != null)
			StopCoroutine(quitRoutine);

		quitRoutine = StartCoroutine(QuitRoutine());
	}

	private void OnQuitConfirm()
	{
		Application.Quit();
	}

	private void TransitionMenuPanel(bool shown)
	{
		if(transitionMenuPanelRoutine != null)
			StopCoroutine(transitionMenuPanelRoutine);

		transitionMenuPanelRoutine = StartCoroutine(TransitionMenuPanelRoutine(shown));
	}

	private IEnumerator TransitionMenuPanelRoutine(bool shown)
	{
		if(menuPanel == null)
			yield break;
		
		float a = (shown ? 0f : 1f);
		float b = (shown ? 1f : 0f);

		menuPanel.blocksRaycasts = false;

		for(float current = quitPanel.transitionDuration; current > 0f; current -= Time.deltaTime)
		{
			float t = Mathf.InverseLerp(quitPanel.transitionDuration, 0f, current);
			float alpha = Mathf.Lerp(a, b, t);
			menuPanel.alpha = alpha;
			yield return null;
		}

		menuPanel.alpha = b;
		menuPanel.blocksRaycasts = shown;
	}

	private IEnumerator QuitRoutine()
	{
		if(blocker == null)
			yield break;
		
		blocker.raycastTarget = true;
		blocker.CrossFadeAlpha(1f, blockTransitionDuration, true);
		yield return new WaitForSeconds(blockTransitionDuration);
		OnQuitConfirm();
	}
}