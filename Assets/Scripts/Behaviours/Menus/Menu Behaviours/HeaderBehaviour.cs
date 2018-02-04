using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.New
{
	public abstract class HeaderBehaviour : MenuBehaviour
	{
		#region Serialized Field
		[Header("Header References")]
		[SerializeField]
		protected Button confirmButton = null;

		[SerializeField]
		protected Button cancelButton = null;
		#endregion


		#region Method
		protected void RegisterActions(Action confirmAction, Action cancelAction)
		{
			RegisterActions(confirmAction, cancelAction, null);
		}

		protected void RegisterActions(Action confirmAction, Action cancelAction, Action closeAction)
		{
			RegisterButtonAction(confirmButton, confirmAction);
			RegisterButtonAction(cancelButton, cancelAction);
			RegisterButtonAction(cancelButton, closeAction);
		}

		protected void DeregisterActions()
		{
			DeregisterButtonAction(confirmButton);
			DeregisterButtonAction(cancelButton);
		}

		private void RegisterButtonAction(Button button, Action action)
		{
			if(button != null)
				button.onClick.AddListener(() => action());
		}

		private void DeregisterButtonAction(Button button)
		{
			if(button != null)
				button.onClick.RemoveAllListeners();
		}
		#endregion
	}
}