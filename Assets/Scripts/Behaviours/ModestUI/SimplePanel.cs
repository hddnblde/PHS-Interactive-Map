using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModestUI.Behaviour;

namespace ModestUI.Panels
{
	public class SimplePanel : PanelBehaviour
	{
		public void Open()
		{
			if(visible)
			{
				Debug.Log("Panel is already open.");
				return;
			}

			visible = true;
		}

		public void Close()
		{
			if(!visible)
			{
				Debug.Log("Panel is already closed.");
				return;
			}

			visible = false;
		}
	}
}