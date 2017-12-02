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

	public void Subtract(Matrix other) {
		matrix = matrix - other;
		UpdateNumbers ();
	}

	public void Multiply(Matrix other) {
		matrix = matrix * other;
		UpdateNumbers ();
	}

	private void Validate() {
		for (int i = 0; i < blocks.Length; i++) {
			double num = matrix.mat [i % 3, i / 3];
			num = num % 10;
			num = (num < 0) ? num + 10 : num;
			matrix.mat [i % 3, i / 3] = num;
		}
	}

	public Matrix GetMatrix() {
		return matrix;
	}

	public void CheckLines() {
		int winner = -1;

		for (int i = 0; i < 3; i++) {
			
			if (matrix.mat [i, 0] == matrix.mat [i, 1] && matrix.mat [i, 1] == matrix.mat [i, 2]) {
				winner = (int)matrix.mat [i, 0];
				blocks [i].Pulse ();
				blocks [i + 3].Pulse ();
				blocks [i + 6].Pulse ();
				break;
			}

			if (matrix.mat [0, i] == matrix.mat [1, i] && matrix.mat [1, i] == matrix.mat [2, i]) {
				winner = (int)matrix.mat [0, i];
				blocks [3 * i].Pulse ();
				blocks [3 * i + 1].Pulse ();
				blocks [3 * i + 2].Pulse ();
				break;
			}
		}

		if (winner == -1) {
			if (matrix.mat [0, 0] == matrix.mat [1, 1] && matrix.mat [1, 1] == matrix.mat [2, 2]) {
				winner = (int)matrix.mat [1, 1];
				blocks [0].Pulse ();
				blocks [4].Pulse ();
				blocks [8].Pulse ();
			}
		}

		if (winner == -1) {
			if (matrix.mat [2, 0] == matrix.mat [1, 1] && matrix.mat [1, 1] == matrix.mat [0, 2]) {
				winner = (int)matrix.mat [1, 1];
				blocks [2].Pulse ();
				blocks [4].Pulse ();
				blocks [6].Pulse ();
			}
		}

//		Debug.Log ("Match for " + winner);
	}
}
