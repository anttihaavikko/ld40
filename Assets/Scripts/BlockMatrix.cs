using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMatrix : MonoBehaviour {

	public Block[] blocks;
	private Matrix matrix;

	// Use this for initialization
	void Start () {
		matrix = Matrix.RandomMatrix (3, 3, 0, 10);
		UpdateNumbers ();
	}

	void UpdateNumbers() {
		Validate ();

		for (int i = 0; i < blocks.Length; i++) {
			int num = (int)matrix.mat [i % 3, i / 3];
			blocks [i].SetNumber (num);
		}
	}
	
	public void Add(Matrix other) {
		matrix = matrix + other;
		UpdateNumbers ();
	}

	private void Validate() {
		for (int i = 0; i < blocks.Length; i++) {
			double num = matrix.mat [i % 3, i / 3];
			num = num % 10;
			matrix.mat [i % 3, i / 3] = num;
		}
	}

	public Matrix GetMatrix() {
		return matrix;
	}
}
