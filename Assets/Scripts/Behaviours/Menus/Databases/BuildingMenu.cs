using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Map;
using Databases;

namespace Menus
{
	[RequireComponent(typeof(CanvasGroup))]
	public class BuildingMenu : MonoBehaviour
	{
		private class BuildingTrivia
		{
			private string m_name = "";
			private PlaceTrivia m_trivia = null;

			public BuildingTrivia(string name, PlaceTrivia trivia)
			{
				m_name = name;
				m_trivia = trivia;
			}

			public string name
			{
				get { return m_name; }
			}

			public PlaceTrivia trivia
			{
				get { return m_trivia; }
			}
		}
		[SerializeField]
		private RectTransform listContainer = null;

		[SerializeField]
		private Image thumbnail = null;

		[SerializeField]
		private Text details = null;

		[SerializeField]
		private GameObject itemPrefab = null;

		[SerializeField]
		private Button backButton = null;

		private List<BuildingTrivia> trivias = new List<BuildingTrivia>();
		private CanvasGroup canvasGroup = null;

		private void Start()
		{
			Initialize();
			Close();
		}

		public void Open()
		{
			SelectTrivia(-1);
			Show(true);
		}

		public void Close()
		{
			Show(false);
		}

		public void Show(bool shown)
		{
			if(canvasGroup == null)
				return;

			canvasGroup.alpha = (shown ? 1f : 0f);
			canvasGroup.blocksRaycasts = shown;
		}

		private void ViewTrivia(BuildingTrivia buildingTrivia)
		{
			if(buildingTrivia == null)
			{
				SetThumbnail(null);
				SetDetails("<b>Select A Building</b>");
			}
			else
			{
				SetThumbnail(buildingTrivia.trivia.thumbnail);

				string detailPattern = "<b>@name</b>\n\n@details".Replace("@name", buildingTrivia.name).Replace("@details", buildingTrivia.trivia.details);
				SetDetails(detailPattern);
			}
		}

		private void SetThumbnail(Sprite sprite)
		{
			if(thumbnail != null)
				thumbnail.sprite = sprite;
		}

		private void SetDetails(string text)
		{
			if(details != null)
				details.text = text;
		}

		private void Initialize()
		{
			Place[] buildings = LocationDatabase.GetAllBuildings();
			CreateList(buildings);
			canvasGroup = GetComponent<CanvasGroup>();

			if(backButton != null)
				backButton.onClick.AddListener(Close);
		}

		private void CreateList(Place[] buildings)
		{
			if(buildings == null)
				return;
			
			for(int i = 0; i < buildings.Length; i++)
			{
				Place building = buildings[i];
				if(building.trivia == null)
					continue;

				CreateBuildingItem(building);
			}
		}

		private void CreateBuildingItem(Place building)
		{
			if(building == null || itemPrefab == null || listContainer == null)
				return;

			PlaceTrivia buildingTrivia = building.trivia;
			trivias.Add(new BuildingTrivia(building.displayedName, buildingTrivia));

			GameObject buildingItem = Instantiate(itemPrefab, listContainer) as GameObject;
			buildingItem.name = building.displayedName;

			Text nameText = buildingItem.GetComponent<Text>();

			if(nameText != null)
				nameText.text = building.displayedName;

			Button button = buildingItem.GetComponent<Button>();

			if(button != null)
				button.onClick.AddListener(() => SelectTrivia(buildingItem.transform.GetSiblingIndex()));
		}

		private void SelectTrivia(int index)
		{
			if(index < 0 || index >= trivias.Count)
				ViewTrivia(null);
			else
			{
				BuildingTrivia trivia = trivias[index];
				ViewTrivia(trivia);
			}
		}
	}
}