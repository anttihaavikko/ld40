using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ColorPicker : MonoBehaviour {

	public Block[] blocks;
	public Text heading;
	public bool locked = false;

	public EffectCamera cam;

	public void SetColor(int colorIndex) {
		Color c = Block.BlockColor (colorIndex);
		string hiliteColor = "#" + ColorUtility.ToHtmlStringRGB (c);
		heading.text = "Pick your <color=" + hiliteColor + ">Color</color>!";
	}

	public void Choose(int num) {
		locked = true;
		cam.BaseEffect ();
		ProgressManager.Instance.selectedPlayerColor = num;
		ProgressManager.Instance.GenerateOpponentColor ();
		Invoke ("StartFade", 0.25f);
		Invoke ("StartLoading", 2f);
	}

	private void StartFade() {
		cam.Fade (true, 0.5f);
	}

	private void StartLoading() {
		SceneManager.LoadSceneAsync ("Main");
	}
}
