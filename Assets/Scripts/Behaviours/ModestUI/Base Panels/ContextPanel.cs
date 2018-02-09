using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ModestUI.Behaviour;

namespace ModestUI.Panels
{
	public class ContextPanel : PanelBehaviour
	{
		#region Serialized Fields
		[SerializeField]
		private Button openButton = null;

		[SerializeField]
		private Button cancelButton = null;

		[SerializeField]
		private Button confirmButton = null;

		[SerializeField]
		private Text displayedContext = null;
		#endregion


		#region Unserialized Fields
		public event Action OnOpen;
		public event Action OnConfirm;
		public event Action OnCancel;
		#endregion


		#region MonoBehaviour Implementation
		protected override void Awake()
		{
			base.Awake();
			Initialize();
		}
		#endregion


		#region Context Implementation
		private void Initialize()
		{
			if(openButton != null)
				openButton.onClick.AddListener(() => Open());
				
			if(confirmButton != null)
				confirmButton.onClick.AddListener(() => ConfirmResponse());

			if(cancelButton != null)
				cancelButton.onClick.AddListener(() => CancelResponse());
		}

		protected virtual bool ConfirmResponse()
		{
			if(!Close())
				return false;

			if(OnConfirm != null)
				OnConfirm();

			return true;
		}

		protected virtual bool CancelResponse()
		{
			if(!Close())
				return false;
			
			if(OnCancel != null)
				OnCancel();
		
			return true;
		}

		private void SetContext(string context)
		{
			if(displayedContext != null)
				displayedContext.text = context;
		}
		#endregion


		#region Actions
		public virtual bool Open()
		{
			return Open("");
		}

		public virtual bool Open(string context)
		{
			if(visible)
			{
				Debug.Log("Already open.");
				return false;
			}

			SetContext(context);
			SetVisible(true);

			if(OnOpen != null)
				OnOpen();

			return true;
		}

		public virtual bool Close()
		{
			if(!visible)
			{
				Debug.Log("Already closed.");
				return false;
			}

			SetVisible(false);

			return true;
		}
		#endregion
	}
}