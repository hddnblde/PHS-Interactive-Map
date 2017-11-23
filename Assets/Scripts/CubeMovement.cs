using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMovement : MonoBehaviour
{
	private void OnEnable()
	{
		TouchInput.OnDrag += OnSwipe;
		TouchInput.OnPinch += OnPinch;
	}

	private void OnDisable()
	{
		TouchInput.OnDrag -= OnSwipe;
		TouchInput.OnPinch -= OnPinch;
	}

	private void OnSwipe(Vector2 deltaPosition)
	{
		transform.position += new Vector3(deltaPosition.x, 0f, deltaPosition.y);
	}

	private void OnPinch(float amount)
	{
		transform.localScale += Vector3.one * amount;

		if(transform.localScale.magnitude > 5f)
			transform.localScale = Vector3.ClampMagnitude(transform.localScale, 5f);
	}
}
