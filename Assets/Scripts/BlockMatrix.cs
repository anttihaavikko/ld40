using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMatrix : MonoBehaviour {

	public Block[] blocks;
	private Matrix matrix;

	void UpdateNumbers(bool animated = false) {
		Validate ();

		for (int i = 0; i < blocks.Length; i++) {
			int num = (int)matrix.mat [i % 3, i / 3];

			if (animated) {
				blocks [i].NumberPulse (0.05f * i, num);
			} else {
				blocks [i].SetNumber (num);
			}
		}
	}

	public void FillNormal() {
		matrix = Matrix.RandomMatrix (3, 3, 0, 10);
		UpdateNumbers ();
	}

	public void FillStarter() {
		matrix = Matrix.RandomMatrix (3, 3, 0, 2);
		UpdateNumbers ();
	}
	
	public void Add(Matrix other) {
		matrix = matrix + other;
		UpdateNumbers (true);
	}

	public void Subtract(Matrix other) {
		matrix = matrix - other;
		UpdateNumbers (true);
	}

	public void Multiply(Matrix other) {
		matrix = other * matrix;
		UpdateNumbers (true);
	}

	private void Validate() {
		for (int i = 0; i < blocks.Length; i++) {
			double num = matrix.mat [i % 3, i / 3];

			if (num < 0 || num > 9) {
				ProgressManager.Instance.LoopTutorial ();
			}

			num = num % 10;
			num = (num < 0) ? num + 10 : num;
			matrix.mat [i % 3, i / 3] = num;
		}
	}

	public Matrix GetMatrix() {
		return matrix;
	}

	public void SetMatrix(Matrix m) {
		matrix = m;
	}

	public int CheckLines(int plrNum, int oppNum) {
		int winner = -1;

		Block[] winnerBlocks = new Block[3];

		for (int i = 0; i < 3; i++) {
			
			if (matrix.mat [i, 0] == matrix.mat [i, 1] && matrix.mat [i, 1] == matrix.mat [i, 2]) {
				winner = (int)matrix.mat [i, 0];
				winnerBlocks[0] = blocks [i];
				winnerBlocks[1] = blocks [i + 3];
				winnerBlocks[2] = blocks [i + 6];

				if (winner == plrNum || winner == oppNum) {
					break;
				}
			}

			if (matrix.mat [0, i] == matrix.mat [1, i] && matrix.mat [1, i] == matrix.mat [2, i]) {
				winner = (int)matrix.mat [0, i];
				winnerBlocks[0] = blocks [3 * i];
				winnerBlocks[1] = blocks [3 * i + 1];
				winnerBlocks[2] = blocks [3 * i + 2];

				if (winner == plrNum || winner == oppNum) {
					break;
				}
			}
		}

		if (winner != plrNum && winner != oppNum) {
			if (matrix.mat [0, 0] == matrix.mat [1, 1] && matrix.mat [1, 1] == matrix.mat [2, 2]) {
				winner = (int)matrix.mat [1, 1];
				winnerBlocks[0] = blocks [0];
				winnerBlocks[1] = blocks [4];
				winnerBlocks[2] = blocks [8];
			}
		}

		if (winner != plrNum && winner != oppNum) {
			if (matrix.mat [2, 0] == matrix.mat [1, 1] && matrix.mat [1, 1] == matrix.mat [0, 2]) {
				winner = (int)matrix.mat [1, 1];
				winnerBlocks[0] = blocks [2];
				winnerBlocks[1] = blocks [4];
				winnerBlocks[2] = blocks [6];
			}
		}

		if (winner != -1) {
			Debug.Log ("Match for " + winner);

			for (int i = 0; i < 3; i++) {
				winnerBlocks [i].Pulse ();
			}
		}

		return winner;
	}
}
