using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class moviePlay : MonoBehaviour {
	public Renderer movieRenderer;
	public MovieTexture movie;
	public AudioSource audio;

	// Use this for initialization
	void Start () {
		movieRenderer = GetComponent<Renderer> ();
		movie = (MovieTexture)movieRenderer.material.mainTexture;
		movie.Play ();
		audio.Play ();
		StartCoroutine (FindEnd (finished));
	}

	private IEnumerator FindEnd(Action returnFunction){
		while (movie.isPlaying) {
			yield return 0;
		}

		returnFunction ();
		yield break;
	}

	void finished(){
		this.gameObject.SetActive (false);
		_root.state = GameState.READY;
	}

	// Update is called once per frame
	void Update () {
	
	}
}
