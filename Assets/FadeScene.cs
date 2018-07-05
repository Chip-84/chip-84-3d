using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class FadeScene : MonoBehaviour {

	public float fadeSpeed = 5;
	public RawImage fadeObject;
	public UnityEvent fadedToClear;
	public UnityEvent fadedToBlack;

	bool fading = false;
	int fadeMode = 0;

	// Use this for initialization
	void Awake() {
		fading = false;
		FadeToClear();
	}

	// Update is called once per frame
	void Update() {
		if (fading == true) {
			if (fadeMode == 0) {
				fadeObject.color = Color.Lerp(fadeObject.color, Color.clear, Time.deltaTime * fadeSpeed);
				if (fadeObject.color.a < 0.02f) {
					fadeObject.color = Color.clear;
					fading = false;
					fadedToClear.Invoke();
				}
			} else if (fadeMode == 1) {
				fadeObject.color = Color.Lerp(fadeObject.color, Color.black, Time.deltaTime * fadeSpeed);
				if (fadeObject.color.a > 0.98f) {
					fadeObject.color = Color.black;
					fading = false;
					fadedToBlack.Invoke();
				}
			}
		}
	}

	public void FadeToBlack() {
		fadeObject.color = Color.clear;
		fading = true;
		fadeMode = 1;
	}

	public void FadeToClear() {
		fadeObject.color = Color.black;
		fading = true;
		fadeMode = 0;
	}

	public void LoadScene(string sceneName) {
		SceneManager.LoadScene(sceneName);
	}
}
