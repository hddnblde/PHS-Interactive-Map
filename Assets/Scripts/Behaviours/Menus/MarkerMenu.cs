using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
	public class MarkerMenu : MonoBehaviour
	{
		[SerializeField]
		private Text displayedText = null;

		[SerializeField]
		private Button quitButton = null;

		[SerializeField]
		private Button checkButton = null;

		private void Awake()
		{
			Initialize();
		}

		private void Initialize()
		{
			
		}
	}
}