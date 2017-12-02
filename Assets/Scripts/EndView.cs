using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndView : MonoBehaviour {

	public BlockCharacter plr;
	public ScaleShower text1, text2;
	public CustomButton menuButton;
	public Animator playerAnimator;
	public Face face;

	private bool collapsed = false;

	// Use this for initialization
	void Start () {
		playerAnimator.speed = 1f;

		Invoke ("Collapse", 7f);
	}
	
	// Update is called once per frame
	void Update () {
		if (!collapsed) {
			playerAnimator.speed = Mathf.MoveTowards (playerAnimator.speed, 0f, Time.deltaTime * 0.1f);
		}
	}

	void Collapse() {
		collapsed = true;

		playerAnimator.speed = 1f;
		playerAnimator.SetTrigger ("end");

		face.Emote (Face.Emotion.Sad);

		text1.Show ();
		text2.Show ();
		menuButton.GetComponent<BoxCollider2D> ().size = menuButton.GetComponent<BoxCollider2D> ().size;
		menuButton.ChangeVisibility (true);
	}
}
