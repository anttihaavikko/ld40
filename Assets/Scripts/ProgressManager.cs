using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour {

	public int level = 0;

	public bool spaceTold = false;
	public bool multiTold = false;

	/******/

	private static ProgressManager instance = null;
	public static ProgressManager Instance {
		get { return instance; }
	}

	void Awake() {
		if (instance != null && instance != this) {
			Destroy (this.gameObject);
			return;
		} else {
			instance = this;
		}

		DontDestroyOnLoad(instance.gameObject);
	}

	public void SpaceTutorial() {
		if (!spaceTold) {
			Manager.Instance.bubble.QueMessage ("You can also (calculate) by pressing (space)!");
			spaceTold = true;
		}
	}

	public void MultiTutorial() {
		if (!multiTold) {
			Manager.Instance.bubble.QueMessage ("Multi help");
			multiTold = true;
		}
	}
}
