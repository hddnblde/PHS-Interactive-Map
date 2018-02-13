using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ModestUI.Panels;
using Map;
using Databases;

namespace Menus.PHS
{
	public class BuildingInformationPanel : SimplePanel
	{
		#region Data Structure
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
		#endregion


		#region Serialized Fields
		[SerializeField]
		private RectTransform listContainer = null;

		[SerializeField]
		private Image thumbnail = null;

		[SerializeField]
		private Text details = null;

		[SerializeField]
		private GameObject itemPrefab = null;
		#endregion


		#region Unserialized Field
		private List<BuildingTrivia> trivias = new List<BuildingTrivia>();
		#endregion


		#region MonoBehaviour Implementation
		private void Start()
		{
			Initialize();
		}
		#endregion


		#region Initializers
		private void Initialize()
		{
			Place[] buildings = LocationDatabase.GetAllBuildings();
			CreateList(buildings);
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
		#endregion


		#region Actions
		public override bool Open()
		{
			return Open(null);
		}

		public bool Open(Place building)
		{
			if(!base.Open())
				return false;

			int index = FindTriviaIndex(building);
			SelectTrivia(index);
			return true;
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
		#endregion


		#region Helpers
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

		private int FindTriviaIndex(Place building)
		{
			if(building == null || trivias == null || trivias.Count == 0)
				return -1;

			int index = -1;

			for(int i = 0; i < trivias.Count; i++)
			{
				BuildingTrivia trivia = trivias[i];

				if(trivia.name == building.displayedName)
				{
					index = i;
					break;
				}
			}
			
			return index;
		}
		#endregion
	}
}