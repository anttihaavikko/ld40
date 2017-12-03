using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleShower : MonoBehaviour {

	private Vector3 originalScale, targetScale;
	private bool hovering = false;
	public Vector3 hiddenScale = new Vector3(1.1f, 0f, 1f);
	public float speed = 6f;

	// Use this for initialization
	void Start () {
		originalScale = transform.localScale;
		targetScale = hiddenScale;
		transform.localScale = targetScale;
	}
	
	// Update is called once per frame
	void Update () {
		transform.localScale = Vector3.MoveTowards (transform.localScale, targetScale, Time.deltaTime * speed);
	}

	public void Hide() {
		targetScale = hiddenScale;
		AudioManager.Instance.PlayEffectAt (11, transform.position, 0.5f);
	}

	public void Show() {
		targetScale = originalScale;
		AudioManager.Instance.PlayEffectAt (11, transform.position, 0.5f);
	}
}
