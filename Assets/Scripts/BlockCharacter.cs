using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCharacter : MonoBehaviour {

	private Animator anim;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		RandomizeSpeed ();
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
}
