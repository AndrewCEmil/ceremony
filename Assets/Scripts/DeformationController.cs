using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DeformMode
{
	Off,
	SmallCircle,
	BigCircle,
	ExpandingCircle,
	ContractingCircle,
	RisingCircle
}


public class DeformationController : MonoBehaviour {

	private DeformMode deformMode;
	private bool isOff;
	private float smallRadius;
	private float bigRadius;
	private float innerRadius;
	private float maxHeight;
	private float epsilon;
	private Vector3 middlePosition;
	private float currentRadius;
	private bool isWaiting;
	private float startTime;
	private float waitTime;
	private DeformMode nextMode;
	private float circleSpeed;
	// Use this for initialization
	void Start () {
		deformMode = DeformMode.Off;
		isOff = false;
		smallRadius = 2f;
		bigRadius = 4.5f;
		innerRadius = .5f;
		maxHeight = 15f;
		epsilon = .5f;
		middlePosition = new Vector3 (5, 5, 0);
		currentRadius = smallRadius;
		isWaiting = false;
		circleSpeed = .005f;
	}
	
	// Update is called once per frame
	void Update () {
		if (isWaiting) {
			DoWait ();
		}
		DoDeform ();
	}

	void DoWait() {
		if (Time.time - startTime > waitTime) {
			isWaiting = false;
			deformMode = nextMode;
		}
	}

	void DoDeform() {
		switch (deformMode) {
		case DeformMode.Off:
			DoOff ();
			break; //Noop
		case DeformMode.SmallCircle:
			SmallCircle ();
			break;
		case DeformMode.BigCircle:
			BigCircle ();
			break;
		case DeformMode.ExpandingCircle:
			ExpandCircle ();
			break;
		case DeformMode.ContractingCircle:
			ContractCircle ();
			break;
		case DeformMode.RisingCircle:
			RisingCircle ();
			break;
		}

		if (deformMode != DeformMode.Off)
			isOff = false;
	}

	public void UpdateDeformMode(DeformMode newDeformMode) {
		deformMode = newDeformMode;
	}

	void DoOff() {
		if (isOff) {
			return;
		}
		//TODO
		Circle(1000);
		isOff = true;
	}

	void BigCircle() {
		Circle (bigRadius);
	}

	void SmallCircle() {
		Circle (smallRadius);
	}

	void RisingCircle() {
		Circle (innerRadius);
	}

	void ExpandCircle() {
		Circle (currentRadius);
		currentRadius += Time.time * circleSpeed;
		if (currentRadius >= bigRadius) {
			ExpandedTrigger ();
		}
	}

	void ContractCircle() {
		Circle (currentRadius);
		currentRadius -= Time.time * circleSpeed;
		if (currentRadius <= innerRadius) {
			ContractedTrigger ();
		}
	}

	public void StartCircleSequence() {
		isWaiting = true;
		startTime = Time.time;
		waitTime = 10;
		nextMode = DeformMode.ExpandingCircle;
		deformMode = DeformMode.SmallCircle;
	}

	void ExpandedTrigger() {
		isWaiting = true;
		startTime = Time.time;
		waitTime = 10;
		nextMode = DeformMode.ContractingCircle;
		deformMode = DeformMode.BigCircle;
	}

	void ContractedTrigger() {
		isWaiting = true;
		startTime = Time.time;
		waitTime = 10;
		nextMode = DeformMode.Off;
		deformMode = DeformMode.RisingCircle;
	}

	float GetHeight(Vector3 position, float radius) {
		//compute but with height as function of distance from radius
		float currentRadius = GetRadius(position);
		float distance = Mathf.Abs (currentRadius - radius);
		if (distance < epsilon) {
			return maxHeight * (epsilon - distance);
		}
		return -1f;
	}

	float GetRadius(Vector3 position) {
		return Vector3.Distance (position, middlePosition);
	}

	void Circle(float radius) {
		MeshFilter meshFilter = gameObject.GetComponent<MeshFilter> ();
		Mesh mesh = meshFilter.mesh;
		Vector3[] baseVerticies = mesh.vertices;
		Vector3[] vertices = new Vector3[baseVerticies.Length];

		float timez = Time.time / 100f;
		for (var i=0; i < baseVerticies.Length; i++) {
			var vertex = baseVerticies[i];
			float noise = -1 * Mathf.PerlinNoise(timez + vertex.x, timez + vertex.y);
			float scale = GetHeight (vertex, radius);
			vertex.z = noise * scale;
			vertices[i] = vertex;
		}

		mesh.vertices = vertices;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
	}
}
