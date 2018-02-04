using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Navigation;

namespace Menus.New
{
	public class MarkContextMenu : ContextMenu
	{
		#region Serialized Field
		[Header("Marker Reference")]
		[SerializeField]
		private RectTransform cursor = null;
		#endregion


		#region Hidden Fields
		public delegate void MarkAction(LocationMarker location);
		private event MarkAction OnMark;
		private string markerLabel = "Custom Marker";
		#endregion


		#region Methods
		public override void Open(string context, Action confirmAction, Action cancelAction)
		{
			if(shown)
				return;
			
			Action compoundedConfirmAction = () => { confirmAction(); MarkLocation(); };
			base.Open(context, compoundedConfirmAction, cancelAction);
		}

		public void AddListener(MarkAction markAction)
		{
			AddListener(markAction, "Marker");
		}

		public void AddListener(MarkAction markAction, string markerLabel)
		{
			OnMark = markAction;
			this.markerLabel = markerLabel;
		}
		#endregion


		#region Helpers
		private Rect RectTransformToScreenSpace(RectTransform rectTransform)
		{
			Vector2 position = new Vector2(rectTransform.position.x, Screen.height - rectTransform.position.y);
			Vector2 size = Vector2.Scale(rectTransform.rect.size, rectTransform.lossyScale);
			return new Rect(position, size);
		}

		private void MarkLocation()
		{
			if(cursor == null)
				return;

			Rect cursorRect = RectTransformToScreenSpace(cursor);
			Vector3 position = NavigationCamera.GetPosition(cursorRect.position);
			LocationMarker marker = new LocationMarker(markerLabel, position);

			if(OnMark != null)
				OnMark(marker);
		}
		#endregion
	}
}