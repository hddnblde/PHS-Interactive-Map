using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Map
{
	[DisallowMultipleComponent]
	public class LocationFinder : MonoBehaviour
	{
		#region Fields
		[SerializeField]
		private GameObject resultPrefab = null;

		[SerializeField]
		private Transform resultContainer = null;

		public delegate void LocationFound(Location location);
		public static event LocationFound OnLocationFound;

		private LocationDatabase database = null;
		private Coroutine resultRoutine = null;
		private const int ResultPoolLimit = 70;
		#endregion


		#region MonoBehaviour Implementation
		private void Awake()
		{
			Initialize();
			GenerateResultPool();
		}

		private void OnEnable()
		{
			RegisterEvent();
		}

		private void OnDisable()
		{
			DeregisterEvent();
		}
		#endregion


		#region Methods
		private void Initialize()
		{
			database = GetComponent<LocationDatabase>();
		}

		private void GenerateResultPool()
		{
			if(resultPrefab == null || resultContainer == null)
				return;

			for(int count = 0; count < ResultPoolLimit; count++)
			{
				GameObject resultItem = Instantiate<GameObject>(resultPrefab, resultContainer);
				resultItem.name = resultItem.name.Replace("(Clone)", "");

				Button resultButton = resultItem.GetComponent<Button>();
				if(resultButton != null)
					resultButton.onClick.AddListener(() => OnSelectResult(resultItem.transform.GetSiblingIndex()));
			}

			HideAllResult();
		}

		private void RegisterEvent()
		{
			if(database != null)
				database.OnResult += OnResult;
		}

		private void DeregisterEvent()
		{
			if(database != null)
				database.OnResult -= OnResult;
		}

		private void OnResult(int count)
		{
			HideAllResult();

			if(resultRoutine != null)
				StopCoroutine(resultRoutine);
			
			resultRoutine = StartCoroutine(GenerateResult(count));
		}

		private CanvasGroup AddResult(int index, Landmark landmark, Location location)
		{
			if(index == -1 || resultContainer == null || index >= resultContainer.childCount)
				return null;

			Place place = location as Place;
			Transform result = resultContainer.GetChild(index);
			Image thumbnail = result.GetChild(0).GetComponent<Image>();
			Text text = result.GetChild(1).GetComponent<Text>();

			if(thumbnail != null)
			{
				thumbnail.sprite = landmark.icon;
				thumbnail.enabled = (landmark.icon != null);
			}

			if(text != null)
			{
				string textPattern = "<color=#000000>@name</color>\n<color=#7F7F7F>@info</color>";
				text.text = textPattern.Replace("@name", place.displayedName).Replace("@info", place.description);
			}

			result.gameObject.SetActive(true);
			CanvasGroup canvasGroup = result.GetComponent<CanvasGroup>();
			canvasGroup.alpha = 0f;

			return canvasGroup;
		}

		private void HideAllResult()
		{
			for(int i = 0; i < resultContainer.childCount; i++)
				resultContainer.GetChild(i).gameObject.SetActive(false);
		}

		private void OnSelectResult(int selected)
		{
			Location location = database.GetLocationFromSearch(selected);

			if(OnLocationFound != null)
				OnLocationFound(location);
		}
		#endregion


		#region Coroutine
		private IEnumerator GenerateResult(int count)
		{
			for(int i = 0; i < count; i++)
			{
				Landmark landmark;
				Location location = database.GetLocationFromSearch(i, out landmark);
				CanvasGroup result = AddResult(i, landmark, location);
				yield return StartCoroutine(FadeInResult(result));
			}
		}

		private IEnumerator FadeInResult(CanvasGroup result)
		{
			if(result != null)
			{
				result.blocksRaycasts = false;
				const float duration = 0.15f;

				for(float current = 0f; current < duration; current += Time.deltaTime)
				{
					float t = Mathf.InverseLerp(0f, duration, current);

					result.alpha = t;
					yield return null;
				}

				result.blocksRaycasts = true;
			}
		}
		#endregion
	}
}