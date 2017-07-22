using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveMode
{
	Stop,
	Move
}


public class PlayerController : MonoBehaviour {
	private GameObject currentWaypoint;
	private float speed;
	private IList<string> path;
	private MoveMode moveMode;
	DeformationController deformationController;
	void Start () {
		Physics.gravity = new Vector3(0, -0.2F, 0);
		Physics.bounceThreshold = 0;
		speed = 0.029f;
		InitMovement ();
		deformationController = GameObject.Find ("MovingGrid").GetComponent<DeformationController> ();
	}

	private void InitMovement() {
		GetPath ();
		currentWaypoint = GameObject.Find ("W0");
		moveMode = MoveMode.Move;
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
		}
	}

	private void StopTrigger() {
		deformationController.UpdateDeformMode (DeformMode.Off);
		//Start Timer player
	}

	private bool AtTarget() {
		return Vector3.Distance (transform.position, currentWaypoint.transform.position) < 1;
	}

	private void UpdateTarget() {
		//TODO delay on moving to next target
		currentWaypoint = GameObject.Find (path [path.IndexOf (currentWaypoint.name) + 1]);
	}

	private bool InTerminalPosition() {
		return currentWaypoint.name == "END";
	}

	private void DoMovement() {
		transform.position = transform.position + ((currentWaypoint.transform.position - transform.position).normalized * speed);
	}
}
