using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour {

	public Texture2D[] cursors;
	private int cursor;

	public bool hovering;
	public bool pointing;

	private int w = 48;
	private int h = 48;

	private static CursorManager instance = null;
	public static CursorManager Instance {
		get { return instance; }
	}

	void Awake() {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
			return;
		} else {
			instance = this;
		}
	}

	void Start() {
		Cursor.visible = false;
		cursor = 0;
	}

	void Update() {
		Cursor.visible = false;

		cursor = pointing ? 3 : 0;

		if (hovering && (!Manager.Instance || Manager.Instance.CanInteract())) {
			cursor = Input.GetMouseButton (0) ? 2 : 1;
		}
	}

	void OnGUI() {
		Cursor.visible = false;
		GUI.DrawTexture(new Rect(Event.current.mousePosition.x - (w / 2), Event.current.mousePosition.y - (h / 2), w, h), cursors[cursor]);
	}
}
