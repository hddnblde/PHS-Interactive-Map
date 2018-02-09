using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModestUI.Panels;
using Navigation;

namespace Menus.PHS
{
	public class MapMarkerPanel : ContextPanel
	{
		#region Serialized Field
		[SerializeField]
		private RectTransform cursor = null;
		#endregion


		#region Unserialized Field
		public delegate void Mark(LocationMarker marker);
		public event Mark OnMark;
		#endregion


		#region Response Implementation
		protected override bool ConfirmResponse()
		{
			if(!base.ConfirmResponse())
				return false;

			MarkByCursor();

			return true;
		}
		#endregion


		#region Helpers
		private void MarkByCursor()
		{
			if(cursor == null)
				return;

			Rect cursorRect = RectTransformToScreenSpace(cursor);
			Vector3 position = NavigationCamera.GetPosition(cursorRect.position);
			LocationMarker marker = new LocationMarker("Custom Marker", position);

			if(OnMark != null)
				OnMark(marker);
		}

		private Rect RectTransformToScreenSpace(RectTransform rectTransform)
		{
			Vector2 position = new Vector2(rectTransform.position.x, Screen.height - rectTransform.position.y);
			Vector2 size = Vector2.Scale(rectTransform.rect.size, rectTransform.lossyScale);
			return new Rect(position, size);
		}		
		#endregion
	}
}