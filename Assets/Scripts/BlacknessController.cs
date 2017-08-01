using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BlacknessController : MonoBehaviour {

	// Use this for initialization
	private float duration;
	private float startTime;
	void Start () {
		duration = 20f;
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time - startTime > duration) {
			SceneManager.LoadScene ("Scenes/CeremonyOpener");
		}
	}
}
