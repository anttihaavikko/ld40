using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour {

	public Text textArea;

	private Vector3 hiddenSize = Vector3.zero;
	private Vector3 shownSize;

	private Vector3 originalPos;

	private bool shown;
	private string message = "";
	private int messagePos = -1;

	public bool done = false;

	private AudioSource audioSource;
	public AudioClip closeClip;

	public GameObject clickHelp;

	private List<string> messageQue;

	private string hiliteColor;

	public Image image;
	public Sprite[] sprites;

	// Use this for initialization
	void Awake () {
		textArea.text = "";
		shownSize = transform.localScale;
		transform.localScale = hiddenSize;
		originalPos = transform.position;
		audioSource = GetComponent<AudioSource> ();

		messageQue = new List<string> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (shown && Input.anyKeyDown) {
			if (!done) {
				done = true;
				messagePos = -1;
				textArea.text = textArea.text = message.Replace("(", "<color=" + hiliteColor + ">").Replace(")", "</color>");;
			} else {
				if (messageQue.Count > 0) {
					PopMessage ();
				} else {
					Hide ();
				}
			}
		}

		if (shown) {
			transform.localScale = Vector3.MoveTowards (transform.localScale, shownSize, Time.deltaTime * 2f);
		} else {
			transform.localScale = Vector3.MoveTowards (transform.localScale, hiddenSize, Time.deltaTime * 2f);
		}

		if (messagePos >= 0 && !done) {
			messagePos++;

			string msg = message.Substring (0, messagePos);

			int openCount = msg.Split('(').Length - 1;
			int closeCount = msg.Split(')').Length - 1;

			if (openCount > closeCount) {
				msg += ")";
			}

			textArea.text = msg.Replace("(", "<color=" + hiliteColor + ">").Replace(")", "</color>");

			string letter = message.Substring (messagePos - 1, 1);

			if (audioSource && letter != " " && letter != "." && letter != "!"  && letter != "?") {
				audioSource.pitch = Random.Range (0.8f, 1.2f);
				audioSource.PlayOneShot (audioSource.clip, 0.15f);
			}

			if (messagePos >= message.Length) {
				messagePos = -1;

				done = true;
			}
		}
	}

	public int QueCount() {
		return messageQue.Count;
	}

	public void SkipMessage() {
		done = true;
		messagePos = -1;
		textArea.text = message;
	}

	private void ShowMessage(string str) {

//		AudioManager.Instance.PlayEffectAt (8, transform.position, 0.4f);

		if (closeClip) {
			audioSource.PlayOneShot (closeClip, 1f);
		}

		done = false;
		shown = true;
		message = str;
		textArea.text = "";

		image.enabled = false;

		if (str == "IMAGE1") {
			ShowImage (0);
			return;
		}

		if (str == "IMAGE2") {
			ShowImage (1);
			return;
		}

		if (str == "IMAGE3") {
			ShowImage (2);
			return;
		}

		if (str == "CARDS") {
			Manager.Instance.GiveCards ();
			PopMessage ();
			return;
		}

		if (str == "TURN") {
			Manager.Instance.UpdateTurnIndicators ();
			PopMessage ();
			return;
		}

		Invoke ("ShowText", 0.2f);
	}

	private void ShowImage(int idx) {
		message = "";
		image.sprite = sprites [idx];
		image.enabled = true;
		done = true;
	}

	public void QueMessage(string str) {
		messageQue.Add (str);

		if (!shown && Manager.Instance.CanInteract()) {
			PopMessage ();
		}
	}

	public void CheckQueuedMessages() {
		if (messageQue.Count > 0 && !shown) {
			PopMessage ();
		}
	}

	private void PopMessage() {
		string msg = messageQue [0];
		messageQue.RemoveAt (0);
		ShowMessage (msg);
	}

	private void ShowText() {
		messagePos = 0;
	}

	public void HideAfter (float delay) {
		Invoke ("Hide", delay);
	}

	public void Hide() {

//		AudioManager.Instance.PlayEffectAt (8, transform.position, 0.4f);

		if (closeClip) {
			audioSource.PlayOneShot (closeClip, 1f);
		}

		shown = false;
		textArea.text = "";
	}

	public bool IsShown() {
		return shown;
	}

	public void SetColor(Color color) {
		image.color = color;
		hiliteColor = "#" + ColorUtility.ToHtmlStringRGB (color);
	}
}
