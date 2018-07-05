using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignScreen : MonoBehaviour {

	public Main main;

	Renderer rend;

	// Use this for initialization
	void Start () {
		rend = GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (main.screen != null)
			rend.material.SetTexture("_MainTex", main.screen);
		rend.material.SetInt("_Res", main.chip.screen_height);
	}
}
