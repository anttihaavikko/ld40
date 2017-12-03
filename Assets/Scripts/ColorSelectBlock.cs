using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSelectBlock : MonoBehaviour {

	private Block block;
	public int number = 0;
	public ColorPicker picker;

	// Use this for initialization
	void Start () {
		block = GetComponent<Block> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnMouseEnter() {
		CursorManager.Instance.pointing = true;
		block.face.Emote (Face.Emotion.Brag);
		picker.SetColor (number);
		block.NumberPulse (0f, number);
	}

	public void OnMouseExit() {
		CursorManager.Instance.pointing = false;
	}

	public void OnMouseDown() {
	}

	public void OnMouseUp() {
		if (!picker.locked) {
			block.PulseOnce ();
			picker.Choose (number);
		}
	}
}
