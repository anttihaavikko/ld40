using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class EffectCamera : MonoBehaviour {

	public Material transitionMaterial;

	private float cutoff = 1f, targetCutoff = 0f;
	private float cutoffPos = 0f;
	private float transitionTime = 5f;

	private PostProcessingBehaviour filters;
	private float chromaAmount = 0f;
	private float chromaSpeed = 0.1f;

	void Start() {
		filters = GetComponent<PostProcessingBehaviour>();
	}

	void Update() {
		cutoff = Mathf.Lerp (cutoff, targetCutoff, cutoffPos);
		transitionMaterial.SetFloat ("_Cutoff", cutoff);
		cutoffPos += Time.deltaTime / transitionTime;

		// chromatic aberration update
		if (filters) {
			chromaAmount = Mathf.MoveTowards (chromaAmount, 0, Time.deltaTime * chromaSpeed);
			ChromaticAberrationModel.Settings g = filters.profile.chromaticAberration.settings;
			g.intensity = chromaAmount;
			filters.profile.chromaticAberration.settings = g;
		}
	}

	void OnRenderImage(RenderTexture src, RenderTexture dst) {
		if (transitionMaterial) {
			Graphics.Blit (src, dst, transitionMaterial);
		}
	}

	public void Fade(bool show, float delay) {
		targetCutoff = show ? 1f : 0f;
		cutoffPos = 0f;
		transitionTime = delay;
	}

	public void Chromate(float amount, float speed) {
		chromaAmount = amount;
		chromaSpeed = speed;
	}
}
