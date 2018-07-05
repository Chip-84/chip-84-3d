using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

	public RectTransform pauseBg;

	RectTransform rectTransform;

	// Use this for initialization
	void Start () {
		rectTransform = GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!Manager.instance.paused) {
			rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, new Vector2(0, -Screen.height), Time.deltaTime * 10);
			pauseBg.anchoredPosition = Vector2.Lerp(pauseBg.anchoredPosition, new Vector2(0, Screen.height), Time.deltaTime * 10);
		} else {
			rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, new Vector2(0, 0), Time.deltaTime * 10);
			pauseBg.anchoredPosition = Vector2.Lerp(pauseBg.anchoredPosition, new Vector2(0, 0), Time.deltaTime * 10);
		}
	}
}
