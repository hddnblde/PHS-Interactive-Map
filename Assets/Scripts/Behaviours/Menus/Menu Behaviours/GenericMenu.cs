using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Menus.New
{
	public class GenericMenu : MenuBehaviour
	{
		[SerializeField]
		private Button cancelButton = null;

		public void Open()
		{
			Open(null);
		}

		public void Open(Action closeAction)
		{
			if(shown)
				return;
			
			if(cancelButton != null && closeAction != null)
			{
				cancelButton.onClick.AddListener(() => { closeAction(); });
				cancelButton.onClick.AddListener(Close);
			}

			base.Show();
		}

		public void Close()
		{
			if(!shown)
				return;

			if(cancelButton != null)
				cancelButton.onClick.RemoveAllListeners();
			
			base.Hide();
		}
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(GenericMenu))]
	public class GenericMenuEditor : Editor
	{
		private GenericMenu menu = null;

		private void OnEnable()
		{
			menu = target as GenericMenu;
		}

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			DrawButtons();
		}

		private void DrawButtons()
		{
			bool isPlaying = Application.isPlaying;
			string label = (menu.shown ? "Close" : "Open");

			GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);

			if(!isPlaying)
				buttonStyle.normal = buttonStyle.active;

			if(GUILayout.Button(label, buttonStyle) && isPlaying)
			{
				if(!menu.shown)
					menu.Open();
				else
					menu.Close();
			}
		}
	}
	#endif
}