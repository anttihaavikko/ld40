using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartView : MonoBehaviour {

	public CustomButton startButton, quitButton;
	public EffectCamera cam;
	private bool locked = false;

	// Use this for initialization
	void Start () {
		startButton.ChangeVisibility (true);

		if (Application.platform != RuntimePlatform.WebGLPlayer) {
			quitButton.ChangeVisibility (true);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void DoStart() {
		if (!locked) {
			locked = true;
			Invoke ("StartFade", 0.25f);
			Invoke ("StartLoading", 2f);
		}
	}

	public void DoQuit() {
		if (!locked) {
			locked = true;
			Invoke ("StartFade", 0.25f);
			Invoke ("DelayedQuit", 2f);
		}
	}

	private void StartFade() {
		cam.Fade (true, 0.5f);
	}

	private void StartLoading() {
		SceneManager.LoadSceneAsync ("Color");
	}

	private void DelayedQuit() {
		Debug.Log ("Quit...");
		Application.Quit ();
	}
}
