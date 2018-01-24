using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
	public class MenuContent
	{
		public MenuContent(Sprite thumbnail, string text)
		{
			m_thumbnail = thumbnail;
			m_text = text;
		}

		private Sprite m_thumbnail = null;

		private string m_text = "";

		public Sprite thumbnail
		{
			get { return m_thumbnail; }
		}

		public string text
		{
			get { return m_text; }
		}
	}

	public class MenuContentLayout : MonoBehaviour
	{
		[SerializeField]
		private Image thumbnail = null;

		[SerializeField]
		private Text text = null;

		[SerializeField]
		private Button button = null;

		public delegate void Select(int index);
		public event Select OnSelect;

		private void OnEnable()
		{
			if(button != null)
				button.onClick.AddListener(SelectEvent);
		}

		private void OnDisable()
		{
			if(button != null)
				button.onClick.RemoveListener(SelectEvent);
		}

		private void SelectEvent()
		{
			if(OnSelect != null)
				OnSelect(transform.GetSiblingIndex());
		}

		public void Set(Sprite thumbnail, string text)
		{
			if(this.thumbnail != null)
			{
				this.thumbnail.sprite = thumbnail;
				this.thumbnail.enabled = thumbnail != null;
			}

			if(this.text != null)
				this.text.text = text;
		}
	}
}