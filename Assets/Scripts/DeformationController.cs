﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum DeformMode
{
	Off,
	StartingSmallCircle,
	SmallCircle,
	BigCircle,
	ExpandingCircle,
	ContractingCircle,
	RisingCircle,
	FallingCircle,
	FinalFallingCircle,
	SweepUp,
	WindDown,
	End
}

public delegate float HeightFunction(Vector3 vertex);

public class DeformationController : MonoBehaviour {

	public GameObject monolith;
	public GameObject diamond;

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
	private float finalDuration;
	private float finalStartTime;
	private float startingSmallCircleScale;
	private IntensityController intensityController;
	private float sweepUpRadius;
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
		circleSpeed = .003f;
		storedVector = new Vector3 (0, 0, 0);
		risingDuration = 3f;
		fallingDuration = 6f;
		finalDuration = 5f;
		startingSmallCircleScale = 0f;
		intensityController = GameObject.Find ("IntensityObj").GetComponent<IntensityController> ();
		sweepUpRadius = 5f;
	}
	
	// Update is called once per frame
	void Update () {
		if (isWaiting) {
			DoWait ();
		}
		DoDeform ();
	}

	void EndScene() {
		SceneManager.LoadScene ("Scenes/Blackness");
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
		case DeformMode.StartingSmallCircle:
			StartingSmallCircle ();
			break;
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
		case DeformMode.FinalFallingCircle:
			FinalFallingCircle ();
			break;
		case DeformMode.SweepUp:
			SweepUp ();
			break;
		case DeformMode.WindDown:
			WindDown ();
			break;
		case DeformMode.End:
			EndScene ();
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
		Circle(OffCircleHeight);
		isOff = true;
	}

	float OffCircleHeight(Vector3 vertex) {
		return -1f;
	}

	void BigCircle() {
		Circle (BigCircleHeight);
		intensityController.IncreaseIntensity ();
	}

	void SmallCircle() {
		Circle (SmallCircleHeight);
		intensityController.IncreaseIntensity ();
	}

	void StartingSmallCircle() {
		if (startingSmallCircleScale >= 1) {
			StartedSmallTrigger ();
		} else {
			Circle (StartingSmallCircleHeight);
			//TODO really should be growing by time, not by frame
			startingSmallCircleScale += .01f;
			intensityController.IncreaseIntensity ();
		}
	}

	float BigCircleHeight(Vector3 vertex) {
		return CircleHeight (vertex, bigRadius);
	}

	float StartingSmallCircleHeight(Vector3 vertext) {
		if (startingSmallCircleScale <= 0) {
			return -1f;
		}
		return CircleHeight (vertext, smallRadius) * startingSmallCircleScale;
	}
	float SmallCircleHeight(Vector3 vertex) {
		return CircleHeight (vertex, smallRadius);
	}

	float CircleHeight(Vector3 vertex, float radius) {
		float noise = GetNoise (vertex, Time.time); 
		float height = GetHeight (vertex, radius);
		if (height < 0) {
			return height;
		}
		return noise * height;
	}


	void RisingCircle() {
		float time = Time.time;
		if (Time.time - risingStartTime >= risingDuration) {
			RoseTrigger ();
		} else {
			Circle (RisingCircleHeight);
		}
	}

	void FallingCircle() {
		float time = Time.time;
		if (Time.time - fallingStartTime > fallingDuration) {
			FallenTrigger ();
		} else {
			Circle (FallingCircleHeight);
		}
	}

	void FinalFallingCircle() {
		float time = Time.time;
		if (Time.time - finalStartTime > finalDuration) {
			FinalFallenTrigger ();
		} else {
			Circle (FinalFallingCircleHeight);
		}
	}

	void SweepUp() {
		sweepUpRadius = sweepUpRadius - Time.deltaTime * 2;
		Circle (SweepUpHeight);
	}

	void WindDown() {
		if (intensityController.LightOff ()) {
			StartTimerAndSetMode (12, DeformMode.End, DeformMode.Off);
		}
		intensityController.DecreaseIntensity ();
	}

	float SweepUpHeight(Vector3 vertex) {
		if (GetRadius (vertex) > sweepUpRadius) {
			return -1f;
		}
		return Mathf.Clamp ((-1f * vertex.z) - .002f, .01f, 1f);
	}

	float RisingCircleHeight(Vector3 vertex) {
		float radius = GetRadius (vertex);
		if (radius > 1) {
			return -1f;
		}
		float duration = (Time.time - risingStartTime) / risingDuration;
		float maxRadius = (1f - duration);
		radius = Mathf.Clamp (1f - radius, 0.001f, maxRadius - .001f);
		radius = radius / maxRadius;
		float height = Mathf.Clamp (Mathf.Tan (radius * Mathf.PI / 2f), 0, 100);
		return height;
	}

	//needs to start at all risen and smooth out
	//Start: radius = 1, radius_offset = 1
	//End: radius = 10, radius_offset = 0
	//as time goes on radius 1 -> 10, offset 1 -> 0
	float FallingCircleHeight(Vector3 vertex) {
		float radius = GetRadius (vertex);
		if (radius > 1) {
			return FallingCircleOuterHeight (vertex, radius);
		}
		float duration = (Time.time - fallingStartTime) / fallingDuration;
		float maxRadius = (duration);
		radius = Mathf.Clamp (1f - radius, 0.001f, maxRadius - .001f);
		radius = radius / maxRadius;
		float height = Mathf.Clamp (Mathf.Tan (radius * Mathf.PI / 2f), 0, 100);
		return height;
	}

	//TODO basically everything is moving away from the center, there is an "inner radius now"
	float FinalFallingCircleHeight(Vector3 vertex) {
		float radius = GetRadius (vertex);
		if (radius > 1) {
			return 0.01f;
		}
		float duration = (Time.time - finalStartTime) / finalDuration;
		float maxRadius = (1f - duration);
		radius = Mathf.Clamp (1f - radius, 0.001f, maxRadius - .001f);
		float height = Mathf.Clamp (Mathf.Tan (radius * Mathf.PI / 2f), 0, 100);
		return height;
	}

	float FallingCircleOuterHeight(Vector3 vertex, float radius) {
		float duration = (Time.time - fallingStartTime) / fallingDuration;
		if (radius > (duration * bigRadius)) {
			return -1f;
		}
		return .01f;
	}

	float CurrentCircleHeight(Vector3 vertex) {
		return CircleHeight (vertex, currentRadius);
	}

	void ExpandCircle() {
		//Circle takes a funcion that takes a vertext, this function calls CircleHeight with current radius
		Circle (CurrentCircleHeight);
		intensityController.IncreaseIntensity ();
		currentRadius += Time.time * circleSpeed;
		maxHeight += Time.time * circleSpeed;
		if (currentRadius >= bigRadius) {
			ExpandedTrigger ();
		}
	}

	void ContractCircle() {
		Circle (CurrentCircleHeight);
		intensityController.IncreaseIntensity ();
		currentRadius -= Time.time * circleSpeed;
		if (currentRadius <= innerRadius) {
			ContractedTrigger ();
		}
	}

	public void StartCircleSequence() {
		StartTimerAndSetMode (3, DeformMode.StartingSmallCircle, DeformMode.Off);
	}

	void ExpandedTrigger() {
		StartTimerAndSetMode (2, DeformMode.ContractingCircle, DeformMode.BigCircle);
	}

	void ContractedTrigger() {
		StartRising ();
	}

	void StartedSmallTrigger() {
		StartTimerAndSetMode (6, DeformMode.ExpandingCircle, DeformMode.SmallCircle);
	}

	void RoseTrigger() {
		StartFalling ();
		SwitchObjects ();
	}

	void SwitchObjects() {
		monolith.SetActive (false);
		diamond.SetActive (true);
	}

	void FallenTrigger() {
		StartFinalFalling ();
	}

	void FinalFallenTrigger() {
		StartTimerAndSetMode (15, DeformMode.WindDown, DeformMode.SweepUp);
	}

	void StartRising() {
		deformMode = DeformMode.RisingCircle;
		risingStartTime = Time.time;
	}

	void StartFalling() {
		deformMode = DeformMode.FallingCircle;
		fallingStartTime = Time.time;
	}

	void StartFinalFalling() {
		deformMode = DeformMode.FinalFallingCircle;
		finalStartTime = Time.time;
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
