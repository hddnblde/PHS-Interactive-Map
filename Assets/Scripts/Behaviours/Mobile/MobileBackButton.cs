using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModestUI.Panels;

public class MobileBackButton : MonoBehaviour
{
	public delegate void Action();
	private static List<Action> backEventQueue = new List<Action>();

	public static void AddListenerToStack(Action backButtonCallback)
	{
		backEventQueue.Add(backButtonCallback);
	}

	public static void RemoveListenerFromStack(Action backButtonCallback)
	{
		if(backEventQueue.Contains(backButtonCallback))
			backEventQueue.Remove(backButtonCallback);
	}
	

	public static event Action OnBackButtonPressed;

	private void Update()
	{
		ListenToBackButton();
	}

	private void ListenToBackButton()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
			InvokeBackButtonEvent();
	}

	private void InvokeBackButtonEvent()
	{
		if(OnBackButtonPressed != null)
			OnBackButtonPressed();
			
		if(backEventQueue == null || backEventQueue.Count == 0)
			return;

		Action backButtonEvent = Dequeue();

		if(backButtonEvent != null)
			backButtonEvent();
	}

	private Action Dequeue()
	{
		if(backEventQueue == null || backEventQueue.Count == 0)
			return null;
		
		int lastIndex = backEventQueue.Count - 1;
		Action backButtonEvent = backEventQueue[lastIndex];
		backEventQueue.RemoveAt(lastIndex);

		return backButtonEvent;
	}
}