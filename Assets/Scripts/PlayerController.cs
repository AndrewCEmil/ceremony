using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	private GameObject currentWaypoint;
	private float speed;
	private IList<string> path;
	void Start () {
		Physics.gravity = new Vector3(0, -0.2F, 0);
		Physics.bounceThreshold = 0;
		GetPath ();
		currentWaypoint = GameObject.Find ("W0");
		speed = 0.029f;
	}

	private void GetPath() {
		path = new List<string> () {
			"W0", 
			"END"
		};
	}

	void Update () {
		HandleMovement ();
	}

	private void HandleMovement() {
		if (AtTarget ()) {
			MaybeUpdateTarget ();
		} else {
			DoMovement ();
		}
	}

	private bool AtTarget() {
		return Vector3.Distance (transform.position, currentWaypoint.transform.position) < 1;
	}

	private void MaybeUpdateTarget() {
		//TODO delay on moving to next target
		if(ShouldUpdateTarget()) {
			currentWaypoint = GameObject.Find (path [path.IndexOf (currentWaypoint.name) + 1]);
		}
	}

	private bool ShouldUpdateTarget() {
		if (InTerminalPosition ()) {
			return false;
		}
		return true;
	}

	private bool InTerminalPosition() {
		return currentWaypoint.name == "END";
	}

	private void DoMovement() {
		transform.position = transform.position + ((currentWaypoint.transform.position - transform.position).normalized * speed);
	}
}
