using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour {

	public Sprite[] numberSprites;
	public Sprite[] operationSprites;

	public CardHolder matrixArea, operatorArea, handArea;
	public BlockMatrix resultMatrix;

	public CustomButton calcButton, nextButton, retryButton;

	public Block playerBlock, opponentBlock;

	private int playerNum, opponentNum;

	private int currentTurn = 0;
	private int turnNumber = 0;
	private bool roundEnded = false;
	private bool locked = true;

	public Text infoText;

	public ScaleShower infoDimmerAnim, infoTextAnim;
	public EffectCamera cam;

	private bool loading = false;

	private CustomButton buttonToShow;

	public SpeechBubble bubble;

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

	void Start() {
		playerNum = Random.Range (0, 10);
		opponentNum = (playerNum + Random.Range(1, 9)) % 10;

		playerBlock.SetNumber (playerNum);
		opponentBlock.SetNumber (opponentNum);

		resultMatrix.FillNormal ();

		int num = StartCards ();
		handArea.SpawnCards (3, num);

		bubble.SetColor (Block.TextColor (opponentNum));

		if (ProgressManager.Instance.level == 0) {
			Invoke ("Intro", 1.2f);
		} else {
			locked = false;
		}
	}

	void Intro() {
		bubble.QueMessage ("Howdy!");
		bubble.QueMessage ("You up for a match of good old (Tic-Tac-Matrix)?");
		bubble.QueMessage ("Huh, dunno how to (play)? Well it's (suuuuper) easy...");
		bubble.QueMessage ("Just (match three of a kind) to in any direction to (win).");
		bubble.QueMessage ("Whaat? You don't even know how to (add up matrices). Well, it's easy too. Lemme show ya...");
		bubble.QueMessage ("IMAGE1");
		bubble.QueMessage ("Kay, (ready to go)?\n\nLets see if you can (handle the heat)!");
		bubble.CheckQueuedMessages ();
		locked = false;
	}

	public void Calculate() {
		if (matrixArea.CardCount() > 0 && operatorArea.CardCount() > 0) {

			Card c = matrixArea.PopCard ();
			Card oc = operatorArea.PopCard ();
			int op = oc.GetOperation ();

			if (op == 0) { 
				resultMatrix.Add (c.GetMatrix ());
			}

			if (op == 1) { 
				resultMatrix.Subtract (c.GetMatrix ());
			}

			if (op == 2) { 
				resultMatrix.Multiply (c.GetMatrix ());
			}

			Destroy (c.gameObject);
			Destroy (oc.gameObject);

			handArea.SpawnCards (1, 1);

			calcButton.ChangeVisibility (false);

			int winner = resultMatrix.CheckLines (playerNum, opponentNum);

			roundEnded = false;

			if (winner != -1 && currentTurn == 0) {
				playerBlock.face.Emote (Face.Emotion.Happy);
				opponentBlock.face.Emote (Face.Emotion.Sad);
				roundEnded = true;

				DisplayText ("You win!", nextButton, 1f);

			}

			if(winner != -1 && currentTurn == 1) {
				opponentBlock.face.Emote (Face.Emotion.Happy);
				playerBlock.face.Emote (Face.Emotion.Sad);
				roundEnded = true;

				DisplayText ("You lose!", retryButton, 1f);
			}

			turnNumber++;
			currentTurn = (currentTurn + 1) % 2;

			if (turnNumber >= 4 && currentTurn == 0) {
				ProgressManager.Instance.SpaceTutorial ();
			}

			if (currentTurn == 0 && !roundEnded) {
				bubble.CheckQueuedMessages ();
			}

			if (currentTurn == 1 && !roundEnded) {
				OpponentTurn ();
			}
		}
	}

	void DisplayText(string str, CustomButton btn, float delay) {
		infoText.text = str;
		buttonToShow = btn;
		Invoke ("DelayedDisplayText", delay);
	}

	void DelayedDisplayText() {
		buttonToShow.ChangeVisibility (true);
		infoDimmerAnim.Show ();
		infoTextAnim.Show ();
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.Space)) {
			Calculate ();
		}

		if (Application.isEditor && Input.GetKeyDown (KeyCode.Q) && CanInteract()) {
			handArea.PickRandoms ();
			Invoke ("Calculate", 0.25f);
		}

		CheckCalcButtonVisibility ();
	}

	public void CheckCalcButtonVisibility() {
		if (matrixArea.CardCount() > 0 && operatorArea.CardCount() > 0 && currentTurn == 0) {
			calcButton.ChangeVisibility (true);
		}
	}

	public bool CanInteract() {
		return (currentTurn == 0 && !roundEnded && !bubble.IsShown() && !locked);
	}

	private void OpponentTurn() {

		float wait = 1f;
		float opponentSpeed = 0.5f;

		Invoke ("PickOperation", opponentSpeed + wait);
		Invoke ("PickMatrix", opponentSpeed * 3 + wait);
		Invoke ("Calculate", opponentSpeed * 6 + wait);
	}

	private void PickOperation() {
		handArea.PickRandomOperation ();
	}

	private void PickMatrix() {
		handArea.PickRandomMatrix ();
	}

	public void NextMatch() {
		if (!loading) {
			Debug.Log ("Next match");
			cam.Fade (true, 0.5f);
			loading = true;
			ProgressManager.Instance.level++;
			Invoke ("ReloadScene", 0.75f);
		}
	}

	public void RestartMatch() {
		if (!loading) {
			Debug.Log ("Restarting match");
			cam.Fade (true, 0.5f);
			loading = true;
			Invoke ("ReloadScene", 0.75f);
		}
	}

	void ReloadScene() {
		SceneManager.LoadSceneAsync ("Main");
	}

	public bool StarterMatrix() {
		return ((ProgressManager.Instance.level == 0 && turnNumber < 10) || Random.value < 0.25f);
	}

	public int OperatorMax() {
		int omax = 3;
		omax = (ProgressManager.Instance.level == 0 || (ProgressManager.Instance.level == 1 && turnNumber < 2)) ? 2 : omax;
		omax = (ProgressManager.Instance.level == 0 && turnNumber < 5) ? 1 : omax;
		return omax;
	}

	public int StartCards() {
		return (ProgressManager.Instance.level == 0) ? 2 : 3;
	}
}
