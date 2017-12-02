using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

	public Sprite[] numberSprites;
	public CardHolder matrixArea;
	public BlockMatrix resultMatrix;

	private static Manager instance = null;
	public static Manager Instance {
		get { return instance; }
	}

	void Awake() {
		if (instance != null && instance != this) {
			Destroy (this.gameObject);
			return;
		} else {
			instance = this;
		}
	}

	public void Calculate() {
		Card c = matrixArea.PopCard ();

		if (c) {
			resultMatrix.Add (c.GetMatrix());
			Destroy (c.gameObject);
		}
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.A)) {
			Calculate ();
		}
	}
}
