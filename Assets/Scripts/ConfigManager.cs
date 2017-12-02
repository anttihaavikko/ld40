using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigManager : MonoBehaviour {

	public bool showFPS = false;
	public bool lowFPS = false;
	public bool cameraShake = true;
	public float soundVolume = 0.5f;
	public float musicVolume = 0.5f;

	/******/

	private static ConfigManager instance = null;
	public static ConfigManager Instance {
		get { return instance; }
	}

	void Awake() {
		if (instance != null && instance != this) {
			Destroy (this.gameObject);
			return;
		} else {
			instance = this;
		}

		LoadPrefs ();

		DontDestroyOnLoad(instance.gameObject);
	}

	private void LoadPrefs() {
		if (PlayerPrefs.HasKey ("MusicVolume")) {
			musicVolume = PlayerPrefs.GetFloat ("MusicVolume");
		}

		if (PlayerPrefs.HasKey ("SoundVolume")) {
			soundVolume = PlayerPrefs.GetFloat ("SoundVolume");
		}

		if (PlayerPrefs.HasKey ("ShowFps")) {
			showFPS = (PlayerPrefs.GetInt ("ShowFps") == 1);
		}

		if (PlayerPrefs.HasKey ("LowFps")) {
			lowFPS = (PlayerPrefs.GetInt ("LowFps") == 1);
		}

		if (PlayerPrefs.HasKey ("CameraShake")) {
			cameraShake = (PlayerPrefs.GetInt ("CameraShake") == 1);
		}
	}

	public void SavePrefs() {
		PlayerPrefs.SetFloat ("MusicVolume", musicVolume);
		PlayerPrefs.SetFloat ("SoundVolume", soundVolume);
		PlayerPrefs.SetInt ("ShowFps", showFPS ? 1 : 0);
		PlayerPrefs.SetInt ("LowFps", lowFPS ? 1 : 0);
		PlayerPrefs.SetInt ("CameraShake", cameraShake ? 1 : 0);
	}
}
