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
		[Header("References")]
		[SerializeField]
		private LocationDatabase database = null;

		[SerializeField]
		private InputField searchInput = null;

		[SerializeField]
		private CanvasGroup contentGroup = null;

		[SerializeField]
		private GameObject resultPrefab = null;

		[SerializeField]
		private Transform resultContainer = null;

		public delegate void LocationFound(Location location);
		public static event LocationFound OnLocationFound;

		private Coroutine resultRoutine = null;
		private const int ResultPoolLimit = 70;
		private string previousKeyword = "";
		#endregion


		#region MonoBehaviour Implementation
		private void Awake()
		{
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
		public void Search(string keyword)
		{
			if(contentGroup != null)
			{
				contentGroup.alpha = 0f;
				contentGroup.blocksRaycasts = false;				
			}

			if(database != null && previousKeyword != keyword)
				database.Search(keyword);

			previousKeyword = keyword;
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
			if(searchInput != null)
				searchInput.onValueChanged.AddListener(Search);
			
			if(database != null)
				database.OnResult += OnResult;
		}

		private void DeregisterEvent()
		{
			if(searchInput != null)
				searchInput.onValueChanged.RemoveListener(Search);
			
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

		private void AddResult(int index, Landmark landmark, Location location)
		{
			if(index == -1 || resultContainer == null || index >= resultContainer.childCount)
				return;

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
		}

		private void HideAllResult()
		{
			for(int i = 0; i < resultContainer.childCount; i++)
				resultContainer.GetChild(i).gameObject.SetActive(false);
		}

		private void OnSelectResult(int selected)
		{
			if(database == null)
				return;
			
			Location location = database.GetLocationFromSearch(selected);

			if(OnLocationFound != null)
				OnLocationFound(location);
		}
		#endregion


		#region Coroutine
		private IEnumerator GenerateResult(int count)
		{
			if(contentGroup != null)
			{
				contentGroup.alpha = 0f;
				contentGroup.blocksRaycasts = false;				
			}

			for(int i = 0; i < count; i++)
			{
				Landmark landmark;
				Location location = database.GetLocationFromSearch(i, out landmark);
				AddResult(i, landmark, location);
			}

			yield return StartCoroutine(FadeInResult());
		}

		private IEnumerator FadeInResult()
		{
			if(contentGroup != null)
			{
				contentGroup.blocksRaycasts = false;
				const float duration = 0.75f;

				for(float current = 0f; current < duration; current += Time.deltaTime)
				{
					float t = Mathf.InverseLerp(0f, duration, current);

					contentGroup.alpha = t;
					yield return null;
				}

				contentGroup.blocksRaycasts = true;
			}
		}
		#endregion
	}
}