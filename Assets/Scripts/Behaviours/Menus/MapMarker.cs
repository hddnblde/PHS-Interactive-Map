using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Navigation;

namespace Menus
{
	public class MapMarker : MonoBehaviour
	{
		[SerializeField]
		private Text text = null;

		[SerializeField]
		private Image icon = null;

		private float viewLowerBounds = 0f;
		private float viewUpperBounds = 1f;

		private const float MinScale = 0.35f;
		private const float MaxScale = 0.05f;

		private Transform cameraTransform = null;

		private void Awake()
		{
			if(Camera.main != null)
				cameraTransform = Camera.main.transform;
		}

		private void OnEnable()
		{
			NavigationCamera.OnViewAdjust += OnViewAdjust;
		}

		private void OnDisable()
		{
			NavigationCamera.OnViewAdjust -= OnViewAdjust;
		}

		public void Set(Sprite icon, string text, Vector3 position)
		{
			Set(icon, text, position, 0f, 1f);
		}

		public void Set(Sprite icon, string text, Vector3 position, float viewLowerBounds, float viewUpperBounds)
		{
			if(this.icon != null)
			{
				this.icon.sprite = icon;
				this.icon.enabled = icon != null;
			}

			if(this.text != null)
				this.text.text = text;

			transform.position = new Vector3(position.x, NavigationCamera.CameraHeight - 1f, position.z);

			this.viewLowerBounds = viewLowerBounds;
			this.viewUpperBounds = viewUpperBounds;
		}

		private void OnViewAdjust(float view)
		{
			if(cameraTransform == null)
				return;

			transform.rotation = cameraTransform.rotation;
			Vector3 scale = Vector3.Lerp(Vector3.one * MinScale, Vector3.one * MaxScale, view);
			transform.localScale = scale;
			
			ShowText(WithinViewingBounds(view));
		}

		private void ShowText(bool show)
		{
			if(text == null)
				return;
				
			text.CrossFadeAlpha((show ? 1f : 0f), 0.1f, true);
		}

		private bool WithinViewingBounds(float view)
		{
			return (view >= viewLowerBounds) && (view <= viewUpperBounds);
		}
	}
}