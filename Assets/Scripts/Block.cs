using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

	public SpriteRenderer numberSprite;
	public SpriteRenderer colorSprite;
	public Animator anim;

	public void SetNumber(int num) {
		numberSprite.sprite = Manager.Instance.numberSprites [num];
		colorSprite.color = BlockColor(num);
	}

	public static Color BlockColor(int idx) {
		float baseSat = 0.6f;
		float baseVal = 0.9f;

		Color[] blockColors = {
			new Color(0.7f, 0.7f, 0.7f),
			Color.HSVToRGB(0f, baseSat, baseVal),
			Color.HSVToRGB(0.1f, baseSat, baseVal),
			Color.HSVToRGB(0.2f, baseSat, baseVal),
			Color.HSVToRGB(0.3f, baseSat, baseVal),
			Color.HSVToRGB(0.4f, baseSat, baseVal),
			Color.HSVToRGB(0.5f, baseSat, baseVal),
			Color.HSVToRGB(0.6f, baseSat, baseVal),
			Color.HSVToRGB(0.7f, baseSat, baseVal),
			Color.HSVToRGB(0.8f, baseSat, baseVal)
		};

		return blockColors [idx];
	}

	public void Pulse() {
		anim.SetTrigger ("pulse");
	}
}
