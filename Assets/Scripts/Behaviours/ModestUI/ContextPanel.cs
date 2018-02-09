using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModestUI.Behaviour;
using UnityEngine.UI;

namespace ModestUI.Panels
{
	public class ContextPanel : PanelBehaviour
	{
		#region Serialized Field
		[SerializeField]
		private Button confirmButton = null;

		[SerializeField]
		private Button cancelButton = null;

		[SerializeField]
		private Text context = null;
		#endregion


		#region Hidden Fields
		public event Action OnConfirm;
		public event Action OnCancel;
		#endregion


		#region MonoBehaviour Implementation
		private void Awake()
		{
			Initialize();
		}

		private void OnDisable()
		{
			Uninitialize();
		}
		#endregion


		#region Initializers
		private void Initialize()
		{
			if(confirmButton != null)
				confirmButton.onClick.AddListener(() => InvokeResponse(true));
			
			if(cancelButton != null)
				cancelButton.onClick.AddListener(() => InvokeResponse(false));
		}

		private void Uninitialize()
		{
			if(cancelButton != null)
				cancelButton.onClick.RemoveListener(() => InvokeResponse(true));

			if(confirmButton != null)
				confirmButton.onClick.RemoveListener(() => InvokeResponse(false));
		}
		#endregion


		#region Actions
		public void Open()
		{
			Open("");
		}

		public void Open(string context)
		{
			if(visible)
			{
				Debug.Log("Panel is already open.");
				return;
			}

			visible = true;
			SetContext(context);
			Internal_Open();
		}

		public void Close()
		{
			if(!visible)
			{
				Debug.Log("Panel is already closed.");
				return;
			}

			visible = false;
			Internal_Close();
		}
		#endregion


		#region Helpers
		private void InvokeResponse(bool confirmed)
		{
			if(confirmed)
			{
				if(OnConfirm != null)
					OnConfirm();
			}
			else
			{
				if(OnCancel != null)
					OnCancel();
			}

			Close();
		}

		private void SetContext(string text)
		{
			if(context != null)
				context.text = text;
		}
		#endregion


		#region Abstract Actions
		protected virtual void Internal_Open(){}

		protected virtual void Internal_Close(){}
		#endregion
	}
}