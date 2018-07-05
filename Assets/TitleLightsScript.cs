using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TitleLightsScript : MonoBehaviour {

	public float timer = 1f;
	public UnityEvent logoDone;

	int fadingMode = 0;

	RawImage img;

	// Use this for initialization
	void Start () {
		fadingMode = 0;
		img = GetComponent<RawImage>();
		img.color = Color.clear;
	}
	
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;

		if (timer < 0 && fadingMode == 0) {
			fadingMode = 1;
		}

		if (fadingMode == 1) {
			img.color = Color.Lerp(img.color, Color.white, Time.deltaTime * 3);
			if (timer < -1) {
				fadingMode = 2;
			}
		}
		if (fadingMode == 2) {
			img.color = Color.Lerp(img.color, Color.clear, Time.deltaTime * 1);
		}
		if (timer < -3) {
			timer = 10000;
			logoDone.Invoke();
		}
	}
}
