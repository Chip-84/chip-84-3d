using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundReactor : MonoBehaviour {

	public AudioSource source;

	Renderer rend;

	// Use this for initialization
	void Start () {
		rend = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (source.isPlaying)
			rend.enabled = true;
		else
			rend.enabled = false;
	}
}
