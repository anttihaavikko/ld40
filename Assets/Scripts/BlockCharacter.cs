using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCharacter : MonoBehaviour {

	private Animator anim;
	public GameObject[] accessories;

	public bool isPlayer = false;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		RandomizeSpeed ();

		if (isPlayer) {
			ActivatePreviousAccessories ();
		} else {
			ActivateCurrentAccessory ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Random.value < 0.01f) {
			RandomizeSpeed ();
		}
	}

	void RandomizeSpeed() {
		anim.speed = Random.Range (0.8f, 1.2f);
	}

	public void ActivateCurrentAccessory() {
		if (ProgressManager.Instance.level < accessories.Length) {
			accessories [ProgressManager.Instance.level].SetActive (true);
		}
	}

	public void ActivatePreviousAccessories() {
		for(int i = 0; i < ProgressManager.Instance.level; i++) {
			if (i < accessories.Length) {
				accessories [i].SetActive (true);
			}
		}
	}
}
