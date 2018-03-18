using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ModestUI.Behaviour;

namespace ModestUI.Panels
{
	public class SimplePanel : PanelBehaviour
	{
		#region Serialized Field
		[SerializeField]
		private bool subscribeToBackButton = false;

		[SerializeField]
		private Button openButton = null;

		[SerializeField]
		private Button closeButton = null;
		#endregion


		#region Unserialized Field
		public event Action OnOpen;
		public event Action OnClose;
		#endregion

		
		#region MonoBehaviour Implementation
		protected override void Awake()
		{
			base.Awake();
			Initialize();
		}
		#endregion


		#region Button Implementation
		private void Initialize()
		{
			if(openButton != null)
				openButton.onClick.AddListener(OpenButtonClicked);
				
			if(closeButton != null)
				closeButton.onClick.AddListener(CloseButtonClicked);
		}
		#endregion


		#region Actions
		protected virtual void OpenButtonClicked()
		{
			Open();
		}

		protected virtual void CloseButtonClicked()
		{
			Close();
		}

		public virtual bool Open()
		{
			if(visible)
				return false;

			SetVisible(true);

			if(OnOpen != null)
				OnOpen();

			if(subscribeToBackButton)
				MobileBackButton.AddListenerToStack(CloseButtonClicked);
			return true;
		}

		public virtual bool Close()
		{
			if(!visible)
				return false;

			SetVisible(false);

			if(OnClose != null)
				OnClose();

			if(subscribeToBackButton)
				MobileBackButton.RemoveListenerFromStack(CloseButtonClicked);
			return true;
		}
		#endregion
	}
}