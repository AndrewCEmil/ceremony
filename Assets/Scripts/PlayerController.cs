using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum MoveMode
{
	Wait,
	Stop,
	Move
}


public class PlayerController : MonoBehaviour {

	private GameObject currentWaypoint;
	private float speed;
	private IList<string> path;
	private MoveMode moveMode;
	DeformationController deformationController;
	private float waitTime;
	private float startTime;
	private GameObject startCanvas;
	void Start () {
		Physics.gravity = new Vector3(0, -0.2F, 0);
		Physics.bounceThreshold = 0;
		speed = 0.029f;
		InitMovement ();
		deformationController = GameObject.Find ("MovingGrid").GetComponent<DeformationController> ();
		startTime = Time.time;
		waitTime = 5f;
		moveMode = MoveMode.Wait;
		startCanvas = GameObject.Find ("StartCanvas");
	}

	private void InitMovement() {
		GetPath ();
		currentWaypoint = GameObject.Find ("monolith");
	}

	private void GetPath() {
		path = new List<string> () {
			"monolith"
		};
	}

	void Update () {
		if (GvrControllerInput.AppButtonUp) {
			HandleAppButton ();
		}

		if (GvrControllerInput.ClickButtonDown) {
			PointerClicked ();
		}

		HandleMovement ();
	}

	//Reload the scene
	private void HandleAppButton() {
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	private void PointerClicked() {
		moveMode = MoveMode.Move;
		startCanvas.SetActive (false);
	}

	private void HandleMovement() {
		if (moveMode == MoveMode.Move) {
			if (AtTarget ()) {
				if (InTerminalPosition ()) {
					moveMode = MoveMode.Stop;
					StopTrigger ();
				} else {
					UpdateTarget ();
				}
			} else {
				DoMovement ();
			}
		} else if (moveMode == MoveMode.Wait) {
			//Do nothing
		}
	}

	private void StopTrigger() {
		deformationController.StartCircleSequence ();
	}

	private bool AtTarget() {
		return Vector3.Distance (transform.position, currentWaypoint.transform.position) < 7;
	}

	private void UpdateTarget() {
		//TODO delay on moving to next target
		currentWaypoint = GameObject.Find (path [path.IndexOf (currentWaypoint.name) + 1]);
	}

	private bool InTerminalPosition() {
		return currentWaypoint.name == "monolith";
	}

	private void DoMovement() {
		transform.position = transform.position + ((currentWaypoint.transform.position - transform.position).normalized * speed);
	}
}
