using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour {
	public Sprite[] operationSprites;

	public CardHolder matrixArea, operatorArea, handArea;
	public BlockMatrix resultMatrix;

	public CustomButton calcButton, nextButton, retryButton, resumeButton, quitButton;

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

	public ScaleShower[] turnIndicators;

	private bool escMenu = false;

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
		playerNum = ProgressManager.Instance.selectedPlayerColor;
		opponentNum = ProgressManager.Instance.opponentColor;

		playerBlock.SetNumber (playerNum);
		opponentBlock.SetNumber (opponentNum);

		resultMatrix.FillNormal ();

		bubble.SetColor (Block.TextColor (opponentNum));

		if (ProgressManager.Instance.level == 0) {
			Invoke ("Intro", 1.2f);
		} else {
			locked = false;
			Invoke ("GiveCards", 1f);
		}

		AudioManager.Instance.Highpass (false);
	}

	void Intro() {
		bubble.QueMessage ("Howdy!");
		bubble.QueMessage ("You up for a match of good old (Tic-Tac-Matrix)?");
		bubble.QueMessage ("Huh, dunno how to (play)? Well it's (suuuuper) easy...");
		bubble.QueMessage ("CARDS");
		bubble.QueMessage ("Just (match three of a kind) to in any direction to (win).");
		bubble.QueMessage ("(Whaat)? You don't even know how to (add up matrices). Well, it's easy too. Lemme show ya...");
		bubble.QueMessage ("IMAGE1");
		bubble.QueMessage ("TURN");
		bubble.QueMessage ("Kay, (ready to go)?\nLets see if you can (handle the heat)!");
		bubble.CheckQueuedMessages ();
		locked = false;
	}

	public void GiveCards() {
		int num = StartCards ();
		handArea.SpawnCards (3, num);
	}

	public void Calculate() {
		if (matrixArea.CardCount() > 0 && operatorArea.CardCount() > 0) {

			cam.BaseEffect ();

			CancelInvoke ("HintOrTaunt");

			locked = true;

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

			EffectManager.Instance.AddEffect (0, c.transform.position);
			EffectManager.Instance.AddEffect (0, oc.transform.position);

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

//			currentTurn = 0; // always player turn

			if (!roundEnded) {
				UpdateTurnIndicators ();

				AudioManager.Instance.Highpass (currentTurn == 1);

			} else {
				turnIndicators [0].Hide ();
				turnIndicators [1].Hide ();
			}

			if (turnNumber >= 4 && currentTurn == 0) {
				ProgressManager.Instance.SpaceTutorial ();
			}

			CancelInvoke ("HintOrTaunt");

			if (currentTurn == 0 && !roundEnded) {
				bubble.CheckQueuedMessages ();
				Invoke ("HintOrTaunt", Random.Range(5f, 20f));
			}

			if (currentTurn == 1 && !roundEnded) {
				OpponentTurn ();
			}

			if (!roundEnded) {
				locked = false;
			} else {
				AudioManager.Instance.Highpass (true);
			}
		}
	}

	public void HintOrTaunt() {

		Debug.Log ("Message time... " + Time.time);

		if (bubble.QueCount () > 0 || bubble.IsShown()) {
			return;
		}

		if (Input.GetMouseButton (0)) {
			return;
		}

		if (Random.value > 0.5f) {
			return;
		}

		bool canWin = handArea.Analyze (resultMatrix.GetMatrix (), false);

		if (canWin && Random.value < 0.5f) {
			
			string[] hintParts1 = {
				"Wanna (hint)?",
				"Need some (help)?",
				"You're taking (forever)!"
			};

			string[] hintParts2 = {
				"\nThere is a (winning move combination) in the hand right now.",
				"\nThere just might be a (killer combination) in there as we speak...",
				"\nJust pick any of em, there is no way to win (;])"
			};

			bubble.QueMessage (hintParts1[Random.Range(0, hintParts1.Length)] + hintParts2[Random.Range(0, hintParts2.Length)]);

			return;
		}

		string plat = "Did your (browser) crash? Try using (IE), I heard it's (the best)!";

		if (Application.platform == RuntimePlatform.LinuxPlayer || Application.platform == RuntimePlatform.LinuxEditor) {
			plat = "Huh, you're still thinking. Should I put up the (hourglass cursor)?";
		}

		if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor) {
			plat = "Huh, you're still thinking. Should I put up the (rainbow wheel)?";
		}

		if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
			plat = "Huh, did your computer (bluescreen) or what is going on? Need the (hourglass cursor)?";
		}

		string[] adjectives = {
			"slow",
			"sluggish",
			"lame",
			"[INSERT INSULTING ADJECTIVE]",
			"weak",
			"uncool",
			"inconsiderate",
			"loitering",
			"apathetic",
			"pathetic",
			"passive",
			"tardy"
		};

		string adj = adjectives [Random.Range (0, adjectives.Length)];
		string adj2 = adjectives [Random.Range (0, adjectives.Length)];

		string[] questions = {
			"Need to",
			"Wanna",
			"Maybe"
		};

		string que = questions [Random.Range (0, questions.Length)];

		string[] suggestions = {
			"How about",
			"Try",
			"I'd use"
		};

		string sug = suggestions [Random.Range (0, suggestions.Length)];

		string[] taunts = {
			"Did you fall sleep?",
			"Still there?",
			"Need some help?",
			"Need a hint?",
			plat,
			que + " use a (lifeline)? " + sug + " (50:50)?",
			que + " use a (lifeline)? " + sug + " (Phone-a-Friend)?",
			que + " use a (lifeline)? " + sug + " (Ask the Audience)?",
			"I'm waiting! Stop being so (" + adj + ").",
			"Zzzzzzzzz....",
			"How on earth can you be so (" + adj + ") and (" + adj2 + ")?",
			"Work those (" + adj + " brain cells)!",
			"([INSERT YOUR MOM JOKE])...",
			"You come here often?",
		};

		bubble.QueMessage (taunts [Random.Range (0, taunts.Length)]);
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

		cam.Shake (0.15f, 0.15f);
		cam.Chromate (0.75f, 0.1f);

		if (currentTurn == 1) {
			AudioManager.Instance.PlayEffectAt (15, Vector3.zero, 0.5f);
		} else {
			AudioManager.Instance.PlayEffectAt (8, Vector3.zero, 0.5f);
		}

		quitButton.ChangeVisibility (true);
	}

	public void ResumeGame() {
		escMenu = false;
		resumeButton.ChangeVisibility (false);
		quitButton.ChangeVisibility (false);
		infoDimmerAnim.Hide ();
		infoTextAnim.Hide ();
	}

	public void BackToMenu() {
		cam.Fade (true, 0.5f);
		Invoke ("LoadMenu", 1f);
	}

	private void LoadMenu() {
		SceneManager.LoadSceneAsync ("Start");
	}

	void Update() {

		if (Input.GetKeyDown (KeyCode.Escape) && (CanInteract() || escMenu)) {
			escMenu = !escMenu;

			if (escMenu) {
				DisplayText ("Really? <color=" + Block.HexColor (ProgressManager.Instance.selectedPlayerColor) + ">:(</color>", resumeButton, 0f);
			} else {
				ResumeGame ();
			}
		}

		if (Input.GetKeyDown (KeyCode.Space)) {
			AudioManager.Instance.PlayEffectAt (1, Vector3.zero, 0.5f);
			Calculate ();
		}

		if (Application.isEditor && Input.GetKeyDown (KeyCode.A) && CanInteract ()) {
			handArea.Analyze (resultMatrix.GetMatrix());
		}

		if (Application.isEditor && Input.GetKeyDown (KeyCode.Q) && CanInteract()) {
			handArea.PickRandoms ();
			Invoke ("Calculate", 0.25f);
		}

		CheckCalcButtonVisibility ();
	}

	public void CheckCalcButtonVisibility() {
		bool state = (matrixArea.CardCount () > 0 && operatorArea.CardCount () > 0 && currentTurn == 0);
		calcButton.ChangeVisibility (state);
	}

	public bool CanInteract() {
		return (currentTurn == 0 && !roundEnded && !bubble.IsShown() && !locked && !escMenu);
	}

	private void OpponentTurn() {

		float wait = 1f;
		float opponentSpeed = 0.5f;

		if(ProgressManager.Instance.WillAnalyze()) {
			Debug.Log ("Running analyze...");
			Invoke ("DoAnalyze", opponentSpeed);
		}

		Invoke ("PickOperation", opponentSpeed + wait);
		Invoke ("PickMatrix", opponentSpeed * 3 + wait);
		Invoke ("Calculate", opponentSpeed * 6 + wait);

		Invoke ("CalcSound", opponentSpeed * 6 + wait);
	}

	private void CalcSound() {
		AudioManager.Instance.PlayEffectAt (1, Vector3.zero, 0.5f);
	}

	private void DoAnalyze() {
		handArea.Analyze (resultMatrix.GetMatrix());
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

			ProgressManager.Instance.GenerateOpponentColor ();

			if (ProgressManager.Instance.level > 5) {
				Invoke ("ToEndScene", 0.75f);
			} else {
				Invoke ("ReloadScene", 0.75f);
			}
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

	void ToEndScene() {
		SceneManager.LoadSceneAsync ("End");
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

	public void UpdateTurnIndicators() {
		turnIndicators [currentTurn].Show ();
		turnIndicators [(currentTurn + 1) % 2].Hide ();

		AudioManager.Instance.PlayEffectAt (17, turnIndicators [currentTurn].transform.position, 0.15f);
	}
}
