using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHolder : MonoBehaviour {

	private List<Card> cards;
	public int cardMax = 1;

	public int numberOfCards = 1;
	public Card cardPrefab, operationCardPrefab;

	public CardHolder[] targetHolders;

	private Card winningOperation, winningCard;

	public int cardType = -1;

	private bool pulsating = false;
	private float pulsePhase = 0f;
	private float pulsePos = 0f;

	private Vector3 originalScale;

	// Use this for initialization
	void Start () {
		cards = new List<Card> ();
		originalScale = transform.localScale;
	}

	public bool Allows(int type) {
		return (cardType == -1 || type == cardType);
	}

	public void Pulse(bool state) {
		pulsating = state;
		pulsePhase = 0f;
	}

	void Update() {

		if (pulsating && cards.Count == 0 && Manager.Instance.CanInteract()) {
			pulsePhase += Time.deltaTime;
			pulsePos = Mathf.Abs(Mathf.Sin (pulsePhase * 5f));
		} else {
			pulsePos = Mathf.MoveTowards (pulsePos, 0f, Time.deltaTime * 2.5f);
			pulsating = false;
		}

		transform.localScale = Vector3.Lerp (1f * originalScale, 1.05f * originalScale, pulsePos);
	}

	public void PickRandomMatrix() {
		Card c = null;

		if (winningCard) {
			winningCard.UseCard ();
			return;
		}

		while (c == null || !c.isMatrix) {
			c = cards [Random.Range (0, cards.Count)];
		}

		c.UseCard ();
	}

	public void PickRandomOperation() {
		Card c = null;

		if (winningOperation) {
			winningOperation.UseCard ();
			return;
		}

		while (c == null || c.isMatrix) {
			c = cards [Random.Range (0, cards.Count)];
		}

		c.UseCard ();
	}

	public void PickRandoms() {
		PickRandomMatrix ();
		PickRandomOperation ();
	}

	public void SpawnCards(int mats, int ops) {
		if (cards.Count < cardMax) {

			for (int i = 0; i < ops; i++) {
				Card oc = Instantiate (operationCardPrefab, transform.position + (cards.Count + 1) * 0.5f * 1.1f * Vector3.right, Quaternion.identity);
				AddCard (oc, true);
			}

			for (int i = 0; i < mats; i++) {
				Card c = Instantiate (cardPrefab, transform.position + (cards.Count + 1) * 0.5f * 1.1f * Vector3.right, Quaternion.identity);
				AddCard (c, true);
			}

			PositionCards ();
		}
	}

	public void RemoveCard(Card c) {

		if (cards.Contains (c)) {
			cards.Remove (c);
		}

		PositionCards ();
	}

	public void AddCard(Card c, bool toEnd) {

		if(!cards.Contains(c)) {

			if (cards.Count >= cardMax) {
				Card swap = cards [0];
				cards.RemoveAt (0);
				swap.currentHolder.targetHolders[0].AddCard (swap, false);
			}

			int slot = 0;

			if (toEnd) {
				
				cards.Add (c);

			} else {

				for (int i = 0; i < cards.Count; i++) {
					if (c.transform.position.x > cards [i].transform.position.x) {
						slot = i + 1;
					}
				}

				cards.Insert (slot, c);
			}
		}

		c.currentHolder = this;
		PositionCards ();
	}

	public void PositionCards() {
		float areaWidth = (cards.Count -1 ) * 0.1f;

		for (int i = 0; i < cards.Count; i++) {
			areaWidth += cards [i].transform.localScale.x;
		}

		float curPos = 0f;

		for (int i = 0; i < cards.Count; i++) {
			curPos += cards [i].transform.localScale.x * 0.5f;
			cards [i].Move(transform.position + (-areaWidth * 0.5f + curPos) * Vector3.right + Vector3.back * 0.01f);
			curPos += cards [i].transform.localScale.x * 0.5f + 0.2f;
		}
	}

	public int CardCount() {
		return cards.Count;
	}

	public Card PopCard() {
		if (cards.Count > 0) {
			Card c = cards [0];
			cards.RemoveAt (0);
			return c;
		}

		return null;
	}

	public bool Analyze(Matrix mat) {

		winningCard = null;
		winningOperation = null;

		foreach (Card oc in cards) {

			if (oc.isMatrix) {
				continue;
			}
			
			foreach (Card c in cards) {

				if (!c.isMatrix) {
					continue;
				}

				Matrix tempMatrix = Matrix.ZeroMatrix(3, 3);

				if (oc.GetOperation () == 0) {
					tempMatrix = mat + c.GetMatrix ();
				}

				if (oc.GetOperation () == 1) {
					tempMatrix = mat - c.GetMatrix ();
				}

				if (oc.GetOperation () == 2) {
					tempMatrix = mat * c.GetMatrix ();
				}

				int winner = -1;

				Block[] winnerBlocks = new Block[3];

				for (int i = 0; i < 3; i++) {

					if (tempMatrix.mat [i, 0] == tempMatrix.mat [i, 1] && tempMatrix.mat [i, 1] == tempMatrix.mat [i, 2]) {
						winner = (int)tempMatrix.mat [i, 0];
						break;
					}

					if (tempMatrix.mat [0, i] == tempMatrix.mat [1, i] && tempMatrix.mat [1, i] == tempMatrix.mat [2, i]) {
						winner = (int)tempMatrix.mat [0, i];
						break;
					}
				}

				if (winner == -1) {
					if (tempMatrix.mat [0, 0] == tempMatrix.mat [1, 1] && tempMatrix.mat [1, 1] == tempMatrix.mat [2, 2]) {
						winner = (int)tempMatrix.mat [1, 1];
					}
				}

				if (winner == -1) {
					if (tempMatrix.mat [2, 0] == tempMatrix.mat [1, 1] && tempMatrix.mat [1, 1] == tempMatrix.mat [0, 2]) {
						winner = (int)tempMatrix.mat [1, 1];
					}
				}

				if (winner != -1) {
					Debug.Log ("Found winner with card " + cards.IndexOf(c) + " with operation " + oc.GetOperation ());
					winningCard = c;
					winningOperation = oc;
					return true;
				}
			}
		}

		return false;
	}
}
