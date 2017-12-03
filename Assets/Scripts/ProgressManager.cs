using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour {

	public int level = 0;

	public bool spaceTold = false;
	public bool multiTold = false;
	public bool loopTold = false;

	public int selectedPlayerColor = 0;
	public int opponentColor = 0;

	public Sprite[] numberSprites;

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

	public void GenerateOpponentColor() {
		opponentColor = (selectedPlayerColor + Random.Range(1, 9)) % 10;
	}

	public void LoopTutorial() {
		if (!loopTold) {
			Manager.Instance.bubble.QueMessage ("You see, there are (limits). The values must be between (0 and 9) or it (loops) around...");
			loopTold = true;
		}
	}

	public void SpaceTutorial() {
		if (!spaceTold) {
			Manager.Instance.bubble.QueMessage ("You can also (calculate) by pressing (space)!");
			spaceTold = true;
		}
	}

	public void MultiTutorial() {
		if (!multiTold) {
			Manager.Instance.bubble.QueMessage ("Oh righ, (multiplication)!\nI bet a newb such as you doesn't even know (how to) do it...");
			Manager.Instance.bubble.QueMessage ("IMAGE2");
			Manager.Instance.bubble.QueMessage ("IMAGE3");
			Manager.Instance.bubble.QueMessage ("And (so on). You get the drift. Lets get (crackin')!");
			multiTold = true;
		}
	}

	public bool WillAnalyze() {
		if (level == 0) {
			return false;
		}

		if (level == 1) {
			return (Random.value < 0.01f);
		}

		if (level == 2) {
			return (Random.value < 0.1f);
		}

		if (level == 3) {
			return (Random.value < 0.25f);
		}

		if (level == 4) {
			return (Random.value < 0.5f);
		}

		return true;
	}
}
