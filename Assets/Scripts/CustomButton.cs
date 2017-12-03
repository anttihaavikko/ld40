using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomButton : MonoBehaviour {

	private Vector3 originalScale, targetScale;
	private bool hovering = false;
	private Vector3 hiddenScale = new Vector3(1.1f, 0f, 1f);

	private bool clicked = false;

	public UnityEvent clickEvent;

	void Awake() {
		originalScale = transform.localScale;
		targetScale = hiddenScale;
		transform.localScale = targetScale;
	}

	public void OnMouseEnter() {
		CursorManager.Instance.pointing = true;
		hovering = true;
		AudioManager.Instance.PlayEffectAt (14, transform.position, 0.1f);
	}

	public void OnMouseExit() {
		CursorManager.Instance.pointing = false;
		hovering = false;
		AudioManager.Instance.PlayEffectAt (14, transform.position, 0.04f);
	}

	public void OnMouseDown() {
	}

	public void OnMouseUp() {
//		Manager.Instance.Calculate ();
		clicked = true;
		clickEvent.Invoke ();
		AudioManager.Instance.PlayEffectAt (1, Vector3.zero, 0.5f);
	}

	void Update() {
		float mod = hovering ? 1.1f : 1.0f;
		transform.localScale = Vector3.MoveTowards (transform.localScale, targetScale * mod, Time.deltaTime * 6f);
	}

	public void ChangeVisibility(bool visible) {
		targetScale = visible ? originalScale : hiddenScale;
	}
}
