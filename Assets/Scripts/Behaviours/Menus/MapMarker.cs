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

		private bool isVisible = true;

		private float viewLowerBounds = 0f;
		private float viewUpperBounds = 1f;
		private int floor = 0;
		private int currentFloor = 1;

		private const float MinScale = 0.35f;
		private const float MaxScale = 0.15f;

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

		private void OnSelectLayer(int index)
		{
			if(this.floor == 0)
				return;
			
			this.currentFloor = index;
			ShowText(isVisible);
		}

		public void Set(Sprite icon, string text, Vector3 position, int floor = 0)
		{
			Set(icon, text, position, 0f, 1f, floor);
		}

		public void Set(Sprite icon, string text, Vector3 position, float viewLowerBounds, float viewUpperBounds, int floor = 0)
		{
			this.floor = floor;

			if(this.icon != null)
			{
				this.icon.sprite = icon;
				this.icon.gameObject.SetActive(icon != null);
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
			if(text == null || show == isVisible)
				return;

			bool showFloor = (floor == 0 ? true : currentFloor == floor);
			isVisible = show & showFloor;				
			text.CrossFadeAlpha((isVisible ? 1f : 0f), 0.1f, true);
		}

		private bool WithinViewingBounds(float view)
		{
			return (view >= viewLowerBounds) && (view <= viewUpperBounds);
		}
	}
}