using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ButtonSpriteAnimation : MonoBehaviour
{
	private const float FrameInterval = 0.016f;
	private enum State
	{
		StartOfFrame,
		EndOfFrame
	}

	[SerializeField]
	private List<Sprite> sprites = new List<Sprite>();

	private State currentState = State.StartOfFrame;
	private Coroutine animationRoutine = null;
	private Image image = null;
	private int currentFrame = -1;

	private void Awake()
	{
		image = GetComponent<Image>();
	}

	public void Animate()
	{
		if(animationRoutine != null)
			StopCoroutine(animationRoutine);

		animationRoutine = StartCoroutine(AnimationRoutine());
	}

	private bool AnimationNotFinished(State state, int frame)
	{
		if(state == State.StartOfFrame)
			return frame < sprites.Count;
		else
			return frame >= 0;
	}

	private int AnimationDirection(State state)
	{
		if(state == State.StartOfFrame)
			return 1;
		else
			return -1;
	}

	private void FlipState()
	{
		if(currentState == State.StartOfFrame)
			currentState = State.EndOfFrame;
		else
			currentState = State.StartOfFrame;
	}

	private void NormalizeFrame()
	{
		if(currentFrame < 0)
			currentFrame = 0;
		else if(currentFrame >= sprites.Count)
			currentFrame = sprites.Count - 1;
	}

	private IEnumerator AnimationRoutine()
	{
		WaitForSeconds interval = new WaitForSeconds(FrameInterval);
		State state = currentState;

		NormalizeFrame();
		FlipState();

		for(int frame = currentFrame; AnimationNotFinished(state, frame); frame += AnimationDirection(state))
		{
			currentFrame = frame;
			image.sprite = sprites[frame];
			yield return interval;
		}
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(ButtonSpriteAnimation))]
public class ButtonSpriteAnimationEditor : Editor
{
	private ButtonSpriteAnimation buttonSpriteAnimation = null;

	private void OnEnable()
	{
		buttonSpriteAnimation = target as ButtonSpriteAnimation;
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		if(!Application.isPlaying)
			return;
		
		EditorGUILayout.Space();

		if(GUILayout.Button("Animate"))
			buttonSpriteAnimation.Animate();
	}
}
#endif