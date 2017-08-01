using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuadController : MonoBehaviour {

	public void HandlePointerEnter() {
		Debug.Log ("Pointer enter");
	}

	public void HandlePointerExit() {
		Debug.Log ("Pointer exit");
	}

	public void HandlePointerTrigger() {
		SceneManager.LoadScene ("Scenes/Ceremony");
	}
}
