using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

	public enum DrawingMode { Cubes, Texture, TV, Sign, Paper, Hologram };

	[HideInInspector]
	public static Manager instance = null;
	public bool paused = false;
	public bool openingRom = true;
	public int cyclesPerFrame = 10;
	public DrawingMode drawingMode = DrawingMode.Cubes;

	// Use this for initialization
	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
