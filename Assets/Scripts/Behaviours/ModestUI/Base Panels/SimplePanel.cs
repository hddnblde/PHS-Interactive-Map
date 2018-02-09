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
				openButton.onClick.AddListener(() => Open());
				
			if(closeButton != null)
				closeButton.onClick.AddListener(() => Close());
		}
		#endregion


		#region Actions
		public virtual bool Open()
		{
			if(visible)
			{
				Debug.Log("Already open.");
				return false;
			}

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

			if(OnClose != null)
				OnClose();

			return true;
		}
		#endregion	
	}
}