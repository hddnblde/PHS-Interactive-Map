using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.New
{
	public class ContextMenu : HeaderBehaviour
	{
		#region Serialized Fields
		[Header("Context Reference")]
		[SerializeField]
		private Text displayedText = null;
		#endregion


		#region Methods
		public virtual void Open(string context, Action confirmAction, Action cancelAction)
		{
			if(shown)
				return;
			
			base.RegisterActions(confirmAction, cancelAction, Close);

			if(displayedText != null)
				displayedText.text = context;
			
			base.Show();
		}

		private void Close()
		{
			if(!shown)
				return;
			
			base.DeregisterActions();
			base.Hide();
		}
		#endregion
	}
}