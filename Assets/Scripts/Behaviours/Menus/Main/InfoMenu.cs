using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
	public class InfoMenu : MonoBehaviour
	{
		[Header("References")]
		[SerializeField]
		private ClassScheduleMenu classMenu = null;

		[SerializeField]
		private BuildingMenu buildingMenu = null;

		[Header("Buttons")]
		[SerializeField]
		private Button classScheduleButton = null;

		[SerializeField]
		private Button buildingButton = null;

		private void Awake()
		{
			Initialize();
		}
		
		private void Initialize()
		{
			if(classMenu != null && classScheduleButton != null)
				classScheduleButton.onClick.AddListener(classMenu.Open);

			if(buildingMenu != null && buildingButton != null)
				buildingButton.onClick.AddListener(buildingMenu.Open);
		}
	}
}