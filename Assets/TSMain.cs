using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TSMain : MonoBehaviour {

	public GameObject textureScreen;
	public TextAsset txt;

	bool playing = false;

	[HideInInspector]
	public Chip8 chip;
	[HideInInspector]
	public Texture2D screen;

	// Use this for initialization
	void Start () {
		chip = GetComponent<Chip8>();
		startEmulation(txt.bytes);
	}
	
	// Update is called once per frame
	void Update () {
		if (playing) {
			chip.emulateCycle(100);
			drawGraphics();
		}
	}

	public void startGame() {
		SceneManager.LoadScene("SampleScene");
	}
	public void quitGame() {
		Application.Quit();
	}

	void startEmulation(byte[] fileData) {
		chip.loadProgram(fileData);
		playing = true;
	}

	void drawGraphics() {
		int width = chip.screen_width;
		for (int i = 0; i < chip.canvas.Length; i++) {
			Color col = screen.GetPixel((width - i % width), (int)Mathf.Floor(i / width) + 1);
			col -= new Color(0.25f, 0.25f, 0.25f);
			col = Color.black;
			screen.SetPixel((width - i % width), (int)Mathf.Floor(i / width) + 1, chip.canvas[i] == 0xff ? Color.white : col);
		}
		screen.Apply();
	}

	public void changeResolution(int width) {
		Debug.Log("Resolution set to " + width + "x" + width / 2);
		textureScreen.transform.root.gameObject.SetActive(true);
		textureScreen.SetActive(true);
		generateScreenTexture();
		textureScreen.GetComponent<Renderer>().materials[0].mainTexture = screen;
		drawGraphics();
	}

	void generateScreenTexture() {
		int width = chip.screen_width;
		screen = new Texture2D(width + 2, width / 2 + 2);
		for (int y = 0; y < screen.height; y++) {
			for (int x = 0; x < screen.width; x++) {
				screen.SetPixel(x, y, Color.black);
			}
		}
		screen.filterMode = FilterMode.Point;
		screen.wrapMode = TextureWrapMode.Clamp;
	}
}
