using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	public AudioSource curMusic;
	public AudioSource[] musics;

	public float volume = 0.5f;
	private float musVolume = 0.5f;
	public SoundEffect effectPrefab;
	public AudioClip[] effects;

	public const int DING = 0;
	public const int BLIP = 1;
	public const int BLING = 2;
	public const int WRONG = 3;
	public const int EXPLOSION = 4;
	public const int CHARGE = 5;
	public const int SWOOSH = 6;
	public const int BEAM = 7;

	public bool inMenu = false;

	public AudioLowPassFilter lowpass;
	public AudioHighPassFilter highpass;

	private AudioReverbFilter reverb;
	private AudioReverbPreset fromReverb, toReverb;

	private Animator anim;
	private AudioSource prevMusic;

	private float fadeOutPos = 0f, fadeInPos = 0f;
	private float fadeOutDuration = 1f, fadeInDuration = 3f;

	/******/

	private static AudioManager instance = null;
	public static AudioManager Instance {
		get { return instance; }
	}

	void Awake() {
		if (instance != null && instance != this) {
			Destroy (this.gameObject);
			return;
		} else {
			instance = this;
		}

		UpdateAudioFilters ();

		reverb = GetComponent<AudioReverbFilter> ();

		fromReverb = AudioReverbPreset.Hallway;
		toReverb = AudioReverbPreset.Off;

		DontDestroyOnLoad(instance.gameObject);
	}

	public void UpdateAudioFilters() {
		anim = Camera.main.GetComponent<Animator> ();
		lowpass = Camera.main.GetComponent<AudioLowPassFilter> ();
		highpass = Camera.main.GetComponent<AudioHighPassFilter> ();
	}

	public void BackToDefaultMusic() {
		if (curMusic != musics [0]) {
			ChangeMusic (0, 0.5f, 2f, 1f);
		}
	}

	public void ChangeMusic(int next, float fadeOutDur, float fadeInDur, float startDelay) {
		fadeOutPos = 0f;
		fadeInPos = -1f;

		fadeOutDuration = fadeOutDur;
		fadeInDuration = fadeInDur;

		prevMusic = curMusic;
		curMusic = musics [next];

		prevMusic.time = 0f;

		Invoke ("StartNext", startDelay);
	}

	private void StartNext() {
		fadeInPos = 0f;
		curMusic.time = 0f;
		curMusic.volume = 0f;
		curMusic.Play ();
	}

	void Start() {
		volume = ConfigManager.Instance.soundVolume;
		musVolume = ConfigManager.Instance.musicVolume * 1.5f;
	}

	void Update() {

		if (fadeInPos < 1f && (!HudManager.Instance || !HudManager.Instance.paused)) {
			fadeInPos += Time.unscaledDeltaTime / fadeInDuration;
		}

		if (fadeOutPos < 1f) {
			fadeOutPos += Time.unscaledDeltaTime / fadeOutDuration;
		}

		if (curMusic && fadeInPos >= 0f && (!HudManager.Instance || !HudManager.Instance.paused)) {
			curMusic.volume = Mathf.Lerp (0f, musVolume, fadeInPos);
		}

		if (prevMusic) {
			prevMusic.volume = Mathf.Lerp (musVolume, 0f, fadeOutPos);

			if (prevMusic.volume <= 0f) {
				prevMusic.Stop ();
			}
		}

		if (GameManager.Instance) {
//			reverb.reverbPreset = toReverb;
			anim.SetBool ("reverb", false);

			float targetPitch = GameManager.Instance.stasisTimer > 0 ? 0.75f : 1f;
			float targetLowpass = (HudManager.Instance.paused) ? 5000f : 22000;
			float targetHighpass = 10f;
			float volumeMod = (HudManager.Instance.paused) ? 0.5f : 1f;

			float changeSpeed = 1f;

			if (!GameManager.Instance.waveOn) {
				targetPitch = 0.99f;
//				targetLowpass = 7000f;
				targetHighpass = 350f;
				volumeMod = 0.9f;
				changeSpeed = 0.1f;
			}

			curMusic.pitch = Mathf.MoveTowards (curMusic.pitch, targetPitch, 0.005f * changeSpeed);
			lowpass.cutoffFrequency = Mathf.MoveTowards (lowpass.cutoffFrequency, targetLowpass, 750f * changeSpeed);
			highpass.cutoffFrequency = Mathf.MoveTowards (highpass.cutoffFrequency, targetHighpass, 50f * changeSpeed);
			curMusic.volume = Mathf.MoveTowards (curMusic.volume, volumeMod * musVolume, 0.01f * changeSpeed);
		} else {

			anim.SetBool ("reverb", true);

			lowpass.cutoffFrequency = Mathf.MoveTowards (lowpass.cutoffFrequency, 22000, 750f * 1f);
			curMusic.volume = Mathf.MoveTowards (curMusic.volume, 0.9f * musVolume, 0.01f * 1f);
//			reverb.reverbPreset = fromReverb;
		}
	}

	public void PlayEffectAt(AudioClip clip, Vector3 pos, float volume, bool pitchShift = true) {
		SoundEffect se = Instantiate (effectPrefab, pos, Quaternion.identity);
		se.Play (clip, volume, pitchShift);
		se.transform.parent = transform;
	}

	public void PlayEffectAt(AudioClip clip, Vector3 pos, bool pitchShift = true) {
		PlayEffectAt (clip, pos, 1f, pitchShift);
	}

	public void PlayEffectAt(int effect, Vector3 pos, bool pitchShift = true) {
		PlayEffectAt (effects [effect], pos, 1f, pitchShift);
	}

	public void PlayEffectAt(int effect, Vector3 pos, float volume, bool pitchShift = true) {
		PlayEffectAt (effects [effect], pos, volume, pitchShift);
	}

	public void ChangeMusicVolume(float vol) {
		ConfigManager.Instance.musicVolume = vol;
		curMusic.volume = vol * 1.5f;
		musVolume = vol * 1.5f;
	}
}
