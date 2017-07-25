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
	RisingCircle,
	FallingCircle
}

public delegate float HeightFunction(Vector3 vertex);

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
	private Vector3 storedVector;
	private float risingDuration;
	private float risingStartTime;
	private float fallingDuration;
	private float fallingStartTime;
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
		storedVector = new Vector3 (0, 0, 0);
		risingDuration = 5f;
		fallingDuration = 5f;
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
		case DeformMode.FallingCircle:
			FallingCircle ();
			break;
		}

		if (deformMode != DeformMode.Off) {
			isOff = false;
		}
	}

	public void UpdateDeformMode(DeformMode newDeformMode) {
		deformMode = newDeformMode;
	}

	void DoOff() {
		if (isOff) {
			return;
		}
		//TODO
		Circle(OffCircleHeight);
		isOff = true;
	}

	float OffCircleHeight(Vector3 vertex) {
		return -1f;
	}

	void BigCircle() {
		Circle (BigCircleHeight);
	}

	void SmallCircle() {
		Circle (SmallCircleHeight);
	}

	float BigCircleHeight(Vector3 vertex) {
		return CircleHeight (vertex, bigRadius);
	}

	float SmallCircleHeight(Vector3 vertex) {
		return CircleHeight (vertex, smallRadius);
	}

	float CircleHeight(Vector3 vertex, float radius) {
		return GetNoise (vertex, Time.time) * GetHeight (vertex, radius);
	}


	void RisingCircle() {
		Circle (RisingCircleHeight);
		if (Time.time - risingStartTime >= risingDuration) {
			RoseTrigger ();
		}
	}

	void FallingCircle() {
		Circle (FallingCircleHeight);
		if (Time.time - fallingStartTime > fallingDuration) {
			FallenTrigger ();
		}
	}

	float RisingCircleHeight(Vector3 vertex) {
		float radius = GetRadius (vertex);
		if (radius > 1) {
			return -1f;
		}
		float startRadius = radius;

		float duration = (Time.time - risingStartTime) / risingDuration;
		float maxRadius = (1f - duration);
		radius = Mathf.Clamp (1f - radius, 0.001f, maxRadius - .001f);
		radius = radius / maxRadius;
		float height = Mathf.Clamp (Mathf.Tan (radius * Mathf.PI / 2f), 0, 100);
		return height;
	}

	//needs to start at all risen and smooth out
	//TODO
	float FallingCircleHeight(Vector3 vertex) {
		return -1f;
		/*
		float radius = GetRadius (vertex);
		if (radius > 1) {
			return -1f;
		}

		if (radius == 0) {
			radius = .001f;
		}
		radius = radius * (1 - ((Time.time - risingStartTime) / risingDuration));
		float height = Mathf.Clamp (Mathf.Tan ((1f - radius) * Mathf.PI / 2f), 0, 100);
*/
	}

	float CurrentCircleHeight(Vector3 vertex) {
		return CircleHeight (vertex, currentRadius);
	}

	void ExpandCircle() {
		//Circle takes a funcion that takes a vertext, this function calls CircleHeight with current radius
		Circle (CurrentCircleHeight);
		currentRadius += Time.time * circleSpeed;
		if (currentRadius >= bigRadius) {
			ExpandedTrigger ();
		}
	}

	void ContractCircle() {
		Circle (CurrentCircleHeight);
		currentRadius -= Time.time * circleSpeed;
		if (currentRadius <= innerRadius) {
			ContractedTrigger ();
		}
	}

	public void StartCircleSequence() {
		StartTimerAndSetMode (10, DeformMode.ExpandingCircle, DeformMode.SmallCircle);
	}

	void ExpandedTrigger() {
		StartTimerAndSetMode (10, DeformMode.ContractingCircle, DeformMode.BigCircle);
	}

	void ContractedTrigger() {
		StartRising ();
	}

	void RoseTrigger() {
		//StartFalling ();
	}

	void FallenTrigger() {
		//TODO
	}

	void StartRising() {
		deformMode = DeformMode.RisingCircle;
		risingStartTime = Time.time;
	}

	void StartFalling() {
		deformMode = DeformMode.FallingCircle;
		fallingStartTime = Time.time;
	}

	void StartTimerAndSetMode(float length, DeformMode next, DeformMode current) {
		deformMode = current;
		StartTimer (length, next);
	}
	void StartTimer(float length, DeformMode next) {
		isWaiting = true;
		startTime = Time.time;
		waitTime = length;
		nextMode = next;
	}

	float GetHeight(Vector3 position, float radius) {
		float currentRadius = GetRadius(position);
		float distance = Mathf.Abs (currentRadius - radius);
		if (distance < epsilon) {
			return maxHeight * (epsilon - distance);
		}
		return -1f;
	}

	float GetNoise(Vector3 vertex, float time) {
		return Mathf.PerlinNoise(time + vertex.x, time + vertex.y);
	}

	float GetRadius(Vector3 position) {
		storedVector.x = position.x;
		storedVector.y = position.y;
		return Vector3.Distance (storedVector, middlePosition);
	}

	void Circle(HeightFunction hf) {
		MeshFilter meshFilter = gameObject.GetComponent<MeshFilter> ();
		Mesh mesh = meshFilter.mesh;
		Vector3[] baseVerticies = mesh.vertices;
		Vector3[] vertices = new Vector3[baseVerticies.Length];

		float timez = Time.time / 100f;
		for (var i=0; i < baseVerticies.Length; i++) {
			var vertex = baseVerticies[i];
			vertex.z = -1 * hf (vertex);
			vertices[i] = vertex;
		}

		mesh.vertices = vertices;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
	}
}
