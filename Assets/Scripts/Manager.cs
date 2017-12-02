using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

	public Sprite[] numberSprites;
	public Sprite[] operationSprites;

	public CardHolder matrixArea, operatorArea, handArea;
	public BlockMatrix resultMatrix;

	public CustomButton calcButton;

	public Block playerBlock, opponentBlock;

	private int playerNum, opponentNum;

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
		opponentNum = (playerNum + 5) % 10;

		playerBlock.SetNumber (playerNum);
		opponentBlock.SetNumber (opponentNum);
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

			int winner = resultMatrix.CheckLines ();

			if (winner == playerNum) {
				Debug.Log ("Player Wins!");
			} else if(winner == opponentNum) {
				Debug.Log ("Opponent Wins!");
			}
		}
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.Space)) {
			Calculate ();
		}

		if (Input.GetKeyDown (KeyCode.Q)) {
			handArea.PickRandoms ();
			Invoke ("Calculate", 0.25f);
		}

		CheckCalcButtonVisibility ();
	}

	public void CheckCalcButtonVisibility() {
		if (matrixArea.CardCount() > 0 && operatorArea.CardCount() > 0) {
			calcButton.ChangeVisibility (true);
		}
	}
}
