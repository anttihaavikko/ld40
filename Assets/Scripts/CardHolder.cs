using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHolder : MonoBehaviour {

	private List<Card> cards;
	public int cardMax = 1;

	public int numberOfCards = 1;
	public Card cardPrefab, operationCardPrefab;

	public CardHolder[] targetHolders;

	public int cardType = -1;

	// Use this for initialization
	void Start () {
		cards = new List<Card> ();

		if (cardType == -1) {
			SpawnCards (3, 3);
		}
	}

	public bool Allows(int type) {
		return (cardType == -1 || type == cardType);
	}

	public void PickRandoms() {
		Card c = null;

		while (c == null || !c.isMatrix) {
			c = cards [Random.Range (0, cards.Count)];
		}

		c.UseCard ();
		c = null;

		while (c == null || c.isMatrix) {
			c = cards [Random.Range (0, cards.Count)];
		}

		c.UseCard ();
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
}
