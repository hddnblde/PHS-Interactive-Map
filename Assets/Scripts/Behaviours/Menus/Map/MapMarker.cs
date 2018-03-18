using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Navigation;
using Menus.PHS;

namespace Menus
{
	public class MapMarker : MonoBehaviour
	{
		[SerializeField]
		private Text text = null;

		[SerializeField]
		private Image icon = null;

		[SerializeField]
		private GameObject pin = null;

		[SerializeField]
		private List<Graphic> pinComponents = new List<Graphic>();

		private bool textIsVisible = true;
		private bool pinIsVisible = true;
		private bool rotateWithCamera = true;

		private float viewLowerBounds = 0f;
		private float viewUpperBounds = 1f;
		private float currentView = 0f;
		private int assignedFloor = 0;
		private int highestFloor = 1;
		private int selectedFloor = 1;

		private const float MinScale = 0.29f;
		private const float MaxScale = 0.07f;

		private Transform cameraTransform = null;

		private void Awake()
		{
			if(Camera.main != null)
				cameraTransform = Camera.main.transform;
		}

		private void OnEnable()
		{
			NavigationCamera.OnViewAdjust += OnViewAdjust;
			FloorPanel.OnFloorSelect += OnFloorSelect;
		}

		private void OnDisable()
		{
			NavigationCamera.OnViewAdjust -= OnViewAdjust;
			FloorPanel.OnFloorSelect -= OnFloorSelect;
		}

		private void OnFloorSelect(int index)
		{
			if(this.assignedFloor == 0)
				return;
			
			this.selectedFloor = index;
			ShowText();
		}

		public void Set(Sprite icon, string text, Vector3 position, bool rotateWithCamera, int totalFloors, int floor = 0)
		{
			Set(icon, text, position, 0f, 1f, rotateWithCamera, floor);
		}

		public void Set(Sprite icon, string text, Vector3 position, float viewLowerBounds, float viewUpperBounds, bool rotateWithCamera, int highestFloor, int floor = 0)
		{
			this.assignedFloor = floor;
			this.highestFloor = highestFloor;

			if(this.icon != null)
				this.icon.sprite = icon;

			if(pin != null)
				pin.SetActive(HasIcon());

			if(this.text != null)
				this.text.text = text;

			transform.position = new Vector3(position.x, NavigationCamera.CameraHeight - 1f, position.z);

			this.viewLowerBounds = viewLowerBounds;
			this.viewUpperBounds = viewUpperBounds;

			this.rotateWithCamera = rotateWithCamera;
		}

		private void OnViewAdjust(float view)
		{
			currentView = view;
			
			if(cameraTransform == null)
				return;

			ScaleByView();
			ShowText();
			ShowPin();
		}

		private void ScaleByView()
		{
			if(rotateWithCamera)
				transform.rotation = cameraTransform.rotation;

			Vector3 scale = Vector3.Lerp(Vector3.one * MinScale, Vector3.one * MaxScale, currentView);
			transform.localScale = scale;
		}

		private void ShowText()
		{
			bool shown = IsVisibleBySelectedFloor() && WithinViewingBounds();

			if(text == null || shown == textIsVisible)
				return;

			textIsVisible = shown;
			text.CrossFadeAlpha((textIsVisible ? 1f : 0f), 0.1f, true);
		}

		private void ShowPin()
		{
			bool shown = HasIcon() && (AboveViewingBounds() || WithinViewingBounds());

			if(pin == null || shown == pinIsVisible || !pin.activeInHierarchy)
				return;

			pinIsVisible = shown;
			
			foreach(Graphic pinComponent in pinComponents)
				pinComponent.CrossFadeAlpha((pinIsVisible ? 1f : 0f), 0.1f, true);
		}

		private bool WithinViewingBounds()
		{
			return (currentView >= viewLowerBounds) && (currentView <= viewUpperBounds);
		}

		private bool AboveViewingBounds()
		{
			return (currentView <= viewLowerBounds);
		}

		private bool IsVisibleBySelectedFloor()
		{
			if(assignedFloor == 0)
				return true;
			else
				return (selectedFloor == assignedFloor) || ((selectedFloor > highestFloor) && (highestFloor == assignedFloor));
		}
		
		private bool HasIcon()
		{
			if(icon == null)
				return false;
			else
				return icon.sprite != null;
		}
	}
}