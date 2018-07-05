using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SFB;

public class Main : MonoBehaviour {

	public Transform pixelParent;

	public GameObject pixel;
	public GameObject classic;
	public GameObject tvScreen;
	public GameObject streetSign;
	public GameObject paper;
	public GameObject hologram;

	public GameObject chooseRomScreen;

	public InputField cpfTextbox;

	public string gamePath;

	public TextAsset txt;

	[HideInInspector]
	public Chip8 chip;
	GameObject[] pixels = new GameObject[128 * 64];
	[HideInInspector]
	public Texture2D screen;
	GameObject textureScreen;

	// Use this for initialization
	void Start () {
		chip = GetComponent<Chip8>();

		Manager.instance.cyclesPerFrame = PlayerPrefs.GetInt("cpf", 10);
		if(cpfTextbox != null)
			cpfTextbox.text = Manager.instance.cyclesPerFrame.ToString();

		tvScreen.transform.root.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Escape") && !Manager.instance.openingRom) Manager.instance.paused = !Manager.instance.paused;
		if (!Manager.instance.paused && !Manager.instance.openingRom && chip.started) {
			chip.emulateCycle(Manager.instance.cyclesPerFrame);
			if (chip.drawFlag || Manager.instance.drawingMode == Manager.DrawingMode.TV) {
				drawGraphics();
			}
		}
	}

	void startEmulation(string fileName) {
		Manager.instance.paused = false;
		changeResolution(64);
		chip.loadProgram(fileName);
	}

	void startEmulation(byte[] fileData) {
		Manager.instance.paused = false;
		changeResolution(64);
		chip.loadProgram(fileData);
	}

	void drawGraphics() {
		if (Manager.instance.drawingMode == Manager.DrawingMode.Cubes) {
			for (int i = 0; i < chip.canvas.Length; i++) {
				pixels[i].SetActive(chip.canvas[i] == 0xff);
			}
		} else if (Manager.instance.drawingMode == Manager.DrawingMode.Texture || 
			Manager.instance.drawingMode == Manager.DrawingMode.TV ||
			Manager.instance.drawingMode == Manager.DrawingMode.Sign ||
			Manager.instance.drawingMode == Manager.DrawingMode.Paper ||
			Manager.instance.drawingMode == Manager.DrawingMode.Hologram) {
			int width = chip.screen_width;
			for (int i = 0; i < chip.canvas.Length; i++) {
				Color col = screen.GetPixel((width - i % width), (int)Mathf.Floor(i / width) + 1);
				col -= new Color(0.25f, 0.25f, 0.25f);
				if (Manager.instance.drawingMode != Manager.DrawingMode.TV) col = Color.black;
				screen.SetPixel((width - i % width), (int)Mathf.Floor(i / width) + 1, chip.canvas[i] == 0xff ? Color.white : col);
			}
			screen.Apply();
		}
		chip.drawFlag = false;
	}

	public void changeResolution(int width) {
		Debug.Log("Resolution set to " + width + "x" + width/2);
		pixelParent.gameObject.SetActive(false);
		if (Manager.instance.drawingMode == Manager.DrawingMode.Cubes) {
			pixelParent.gameObject.SetActive(true);
			float factor = width / 64;
			for (int i = 0; i < pixels.Length; i++) {
				Destroy(pixels[i]);
			}
			pixels = new GameObject[width * (width / 2)];
			for (int i = 0; i < pixels.Length; i++) {
				pixels[i] = Instantiate(pixel, new Vector3((i % width) / factor, -Mathf.Floor(i / width) / factor, 0), Quaternion.identity);
				pixels[i].transform.localScale /= factor;
				pixels[i].transform.parent = pixelParent;
			}
		} else if (Manager.instance.drawingMode == Manager.DrawingMode.Texture ||
			Manager.instance.drawingMode == Manager.DrawingMode.TV) {
			if (Manager.instance.drawingMode == Manager.DrawingMode.TV) {
				textureScreen.transform.root.gameObject.SetActive(true);
			}
			textureScreen.SetActive(true);
			generateScreenTexture(width);
			textureScreen.GetComponent<Renderer>().materials[0].mainTexture = screen;
		} else if (Manager.instance.drawingMode == Manager.DrawingMode.Sign ||
			Manager.instance.drawingMode == Manager.DrawingMode.Paper ||
			Manager.instance.drawingMode == Manager.DrawingMode.Hologram) {
			generateScreenTexture(width);
		}
	}

	void generateScreenTexture(int width) {
		screen = new Texture2D(width + 2, width / 2 + 2);
		for (int y = 0; y < screen.height; y++) {
			for (int x = 0; x < screen.width; x++) {
				screen.SetPixel(x, y, Color.black);
			}
		}
		screen.filterMode = FilterMode.Point;
		screen.wrapMode = TextureWrapMode.Clamp;
	}

	public void viewModeChanged(GameObject go) {
		int selection = go.GetComponent<Dropdown>().value;

		if (textureScreen != null && Manager.instance.drawingMode == Manager.DrawingMode.TV) {
			textureScreen.transform.root.gameObject.SetActive(false);
		}
		if (Manager.instance.drawingMode == Manager.DrawingMode.Sign) {
			streetSign.SetActive(false);
		} else if (Manager.instance.drawingMode == Manager.DrawingMode.Paper) {
			paper.SetActive(false);
		} else if (Manager.instance.drawingMode == Manager.DrawingMode.Hologram) {
			hologram.SetActive(false);
		}

		for (int i = 0; i < pixels.Length; i++) {
			Destroy(pixels[i]);
		}

		if(textureScreen != null)
			textureScreen.SetActive(false);

		if (selection == 0)
			Manager.instance.drawingMode = Manager.DrawingMode.Cubes;
		else if (selection == 1) {
			textureScreen = classic;
			Manager.instance.drawingMode = Manager.DrawingMode.Texture;
		} else if (selection == 2) {
			textureScreen = tvScreen;
			textureScreen.transform.root.gameObject.SetActive(true);
			Manager.instance.drawingMode = Manager.DrawingMode.TV;
		} else if (selection == 3) {
			streetSign.SetActive(true);
			Manager.instance.drawingMode = Manager.DrawingMode.Sign;
		} else if (selection == 4) {
			paper.SetActive(true);
			Manager.instance.drawingMode = Manager.DrawingMode.Paper;
		} else if (selection == 5) {
			hologram.SetActive(true);
			Manager.instance.drawingMode = Manager.DrawingMode.Hologram;
		}

		changeResolution(chip.screen_width);
	}

	public void OpenROMChooser() {
		string[] files = StandaloneFileBrowser.OpenFilePanel("Open ROM", "", "", false);
		if (files.Length > 0) {
			chip.started = false;
			gamePath = files[0];
			Debug.Log("Opening ROM: " + gamePath);
			Manager.instance.openingRom = false;
			chooseRomScreen.SetActive(false);
			startEmulation(gamePath);
		}
	}

	public void changeCPF(string cpf) {
		Manager.instance.cyclesPerFrame = int.Parse(cpf);
		PlayerPrefs.SetInt("cpf", Manager.instance.cyclesPerFrame);
	}

	public void quitGame() {
		Application.Quit();
	}
}
