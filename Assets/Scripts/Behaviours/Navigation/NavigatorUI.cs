using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Navigation
{
	public class NavigatorUI : MonoBehaviour
	{
		#region Serialized Fields
		[SerializeField]
		private bool isActive = false;

		[Header("References")]
		[SerializeField]
		private Navigator navigator = null;

		[SerializeField]
		private SearchUI searchUI;

		[SerializeField]
		private Transform buildingsContainer = null;

		[SerializeField]
		private CanvasGroup chooseMarkerPanel = null;

		[SerializeField]
		private CanvasGroup setMarkerPanel = null;

		[SerializeField]
		private Text markerLabel = null;

		[SerializeField]
		private InputField searchField = null;

		[Header("Animation")]
		[SerializeField]
		private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
		#endregion


		#region Hidden Fields
		private const float TransitionDuration = 0.25f;
		private Coroutine chooseMarkerPanelTransition = null;
		private Coroutine setMarkerPanelTransition = null;
		#endregion


		#region MonoBehaviour Implementation
		private void Awake()
		{
			NavigationUtility.CacheLocations(buildingsContainer);
		}

		private void Start()
		{
			ChooseMarkerActivity();
		}

		private void OnEnable()
		{
			RegisterEvents();
		}

		private void OnDisable()
		{
			DeregisterEvents();
		}
		#endregion


		#region Events
		private void RegisterEvents()
		{
			if(searchField != null)
				searchField.onValueChanged.AddListener(SearchForLocation);
			
			Navigator.OnActivityChange += OnActivityChange;
			Navigator.OnMarkerChange += OnMarkerChange;
			Navigator.OnMarkerAssignment += OnMarkerAssignment;
		}

		private void DeregisterEvents()
		{
			if(searchField != null)
				searchField.onValueChanged.RemoveListener(SearchForLocation);
			
			Navigator.OnActivityChange -= OnActivityChange;
			Navigator.OnMarkerChange -= OnMarkerChange;
			Navigator.OnMarkerAssignment -= OnMarkerAssignment;
		}

		private void OnMarkerChange(Navigator.Marker marker)
		{
			if(markerLabel == null)
				return;

			string label = "Press on screen to set @marker.";
			switch(marker)
			{
			case Navigator.Marker.Origin:
				label = label.Replace("@marker", "origin");
				break;

			case Navigator.Marker.Destination:
				label = label.Replace("@marker", "destination");
				break;
			}

			markerLabel.text = label;
		}

		private void OnActivityChange(Navigator.Activity activity)
		{
			switch(activity)
			{
			case Navigator.Activity.Idle:
				ChooseMarkerActivity();
				break;

			case Navigator.Activity.SetMarker:
				SetMarkerActivity();
				break;
			}
		}

		private void OnMarkerAssignment()
		{
			if(navigator == null)
				return;
			
			navigator.ChangeActivity(Navigator.Activity.Idle);
		}
		#endregion


		#region Methods
		public void Activate(bool isActive)
		{
			this.isActive = isActive;

			if(isActive)
				ChooseMarkerActivity();
			else
				IdleActivity();
		}

		public void Navigate()
		{
			if(navigator == null)
				return;
			
			navigator.Navigate();
			Activate(false);
		}

		public void SetOriginMarker()
		{
			if(navigator == null)
				return;

			navigator.ChangeMarker(Navigator.Marker.Origin);
			navigator.ChangeActivity(Navigator.Activity.SetMarker);
		}

		public void SetDestinationMarker()
		{
			if(navigator == null)
				return;

			navigator.ChangeMarker(Navigator.Marker.Destination);
			navigator.ChangeActivity(Navigator.Activity.SetMarker);
		}

		public void CancelMarker()
		{
			ChooseMarkerActivity();
		}

		private void IdleActivity()
		{
			TransitionPanel(chooseMarkerPanelTransition, chooseMarkerPanel, false);
			TransitionPanel(setMarkerPanelTransition, setMarkerPanel, false);
		}

		private void ChooseMarkerActivity()
		{
			TransitionPanel(chooseMarkerPanelTransition, chooseMarkerPanel, true);
			TransitionPanel(setMarkerPanelTransition, setMarkerPanel, false);
		}

		private void SetMarkerActivity()
		{
			TransitionPanel(chooseMarkerPanelTransition, chooseMarkerPanel, false);
			TransitionPanel(setMarkerPanelTransition, setMarkerPanel, true);
		}

		private void SearchForLocation(string location)
		{
			if(searchUI == null)
				return;

			if(location.Length == 0)
				searchUI.Clear();
			else
			{
				string[] foundLocations = NavigationUtility.FindLocation(location);
				searchUI.SetItems(foundLocations);
			}
		}
		#endregion


		#region Coroutines
		private IEnumerator TransitionPanelRoutine(CanvasGroup panel, bool shown)
		{
			if(panel == null || panel.interactable == shown)
				goto end;

			panel.gameObject.SetActive(true);
			
			float start = panel.alpha;
			float end = (shown ? 1f : 0f);

			float progress = Mathf.InverseLerp((shown ? 0f : 1f), end, start);
			float duration = Mathf.Lerp(TransitionDuration, 0f, progress);

			panel.blocksRaycasts = false;
			panel.interactable = false;

			for(float currentDuration = duration; currentDuration > 0f; currentDuration -= Time.deltaTime)
			{
				float current = Mathf.InverseLerp(TransitionDuration, 0f, currentDuration);
				current = transitionCurve.Evaluate(current);

				panel.alpha = Mathf.LerpUnclamped(start, end, current);
				yield return null;
			}

			panel.alpha = end;
			panel.blocksRaycasts = shown;
			panel.interactable = shown;

			if(panel.gameObject.activeInHierarchy != shown)
				panel.gameObject.SetActive(shown);

			end:
			yield return null;
		}
		#endregion


		#region Helper
		private void TransitionPanel(Coroutine coroutine, CanvasGroup panel, bool shown)
		{
			StopAndStartCoroutine(coroutine, TransitionPanelRoutine(panel, shown));
		}

		private void StopAndStartCoroutine(Coroutine reference, IEnumerator coroutine)
		{
			if(reference != null)
				StopCoroutine(reference);

			reference = StartCoroutine(coroutine);
		}
		#endregion
	}
}