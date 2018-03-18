using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ModestUI.Behaviour;

namespace ModestUI.Panels
{
	public class ContextPanel : SimplePanel
	{
		#region Serialized Fields
		[SerializeField]
		private Button cancelButton = null;

		[SerializeField]
		private Button confirmButton = null;

		[SerializeField]
		private Text displayedContext = null;
		#endregion


		#region Unserialized Fields
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
			if(confirmButton != null)
				confirmButton.onClick.AddListener(() => ConfirmResponse());

			if(cancelButton != null)
				cancelButton.onClick.AddListener(() => CancelResponse());
		}

		protected virtual bool ConfirmResponse()
		{
			if(!base.Close())
				return false;

			if(OnConfirm != null)
				OnConfirm();

			return true;
		}

		protected virtual bool CancelResponse()
		{
			if(!base.Close())
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
		public override bool Open()
		{
			return Open("");
		}

		public bool Open(string context)
		{
			if(!base.Open())
				return false;

			SetContext(context);

			return true;
		}
		#endregion
	}
}