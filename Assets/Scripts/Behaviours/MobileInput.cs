using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInput : MonoBehaviour
{
	public delegate void Action();
	public static event Action OnBackButtonPressed;

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			if(OnBackButtonPressed != null)
				OnBackButtonPressed();
		}
	}
}
