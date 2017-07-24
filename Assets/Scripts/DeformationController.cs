using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DeformMode
{
	Off,
	Rumble,
	SmallCircle,
	BigCircle
}


public class DeformationController : MonoBehaviour {

	private DeformMode deformMode;
	private bool isOff;
	private float smallRadius;
	private float bigRadius;
	private float maxHeight;
	private float epsilon;
	private Vector3 middlePosition;
	// Use this for initialization
	void Start () {
		deformMode = DeformMode.SmallCircle;
		isOff = false;
		smallRadius = 2f;
		maxHeight = 15f;
		epsilon = .5f;
		middlePosition = new Vector3 (5, 5, 0);
	}
	
	// Update is called once per frame
	void Update () {
		switch (deformMode) {
		case DeformMode.Off:
			DoOff ();
			break; //Noop
		case DeformMode.Rumble:
			BasicRumble (false);
			break;
		case DeformMode.SmallCircle:
			SmallCircle ();
			break;
		case DeformMode.BigCircle:
			BigCircle ();
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
		BasicRumble (true);
		isOff = true;
	}

	void BasicRumble(bool zero) {
		MeshFilter meshFilter = gameObject.GetComponent<MeshFilter> ();
		Mesh mesh = meshFilter.mesh;
		Vector3[] baseVerticies = mesh.vertices;
		Vector3[] vertices = new Vector3[baseVerticies.Length];

		float timez = Time.time;
		for (var i=0; i < baseVerticies.Length; i++) {
			var vertex = baseVerticies[i];
			float noise = Mathf.PerlinNoise(timez + vertex.x, timez + vertex.y);
			if (zero)
				noise = 0f;
			vertex.z = noise;
			vertices[i] = vertex;
		}

		mesh.vertices = vertices;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
	}

	float GetSmallCircleHeight(Vector3 position) {
		//compute but with height as function of distance from radius
		float currentRadius = GetRadius(position);
		float distance = Mathf.Abs (currentRadius - smallRadius);
		if (distance < epsilon) {
			return maxHeight * (epsilon - distance);
		}
		return 0f;
	}

	float GetBigCircleHeight(Vector3 position) {
		//compute but with height as function of distance from radius
		float currentRadius = GetRadius(position);
		float distance = Mathf.Abs (currentRadius - bigRadius);
		if (distance < epsilon) {
			return maxHeight * (epsilon - distance);
		}
		return 0f;
	}

	float GetRadius(Vector3 position) {
		return Vector3.Distance (position, middlePosition);
	}

	void SmallCircle() {
		MeshFilter meshFilter = gameObject.GetComponent<MeshFilter> ();
		Mesh mesh = meshFilter.mesh;
		Vector3[] baseVerticies = mesh.vertices;
		Vector3[] vertices = new Vector3[baseVerticies.Length];

		float timez = Time.time;
		for (var i=0; i < baseVerticies.Length; i++) {
			var vertex = baseVerticies[i];
			float noise = Mathf.PerlinNoise(timez + vertex.x, timez + vertex.y);
			float scale = GetSmallCircleHeight (vertex);
			vertex.z = noise * scale;
			vertices[i] = vertex;
		}

		mesh.vertices = vertices;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
	}

	void BigCircle() {
		MeshFilter meshFilter = gameObject.GetComponent<MeshFilter> ();
		Mesh mesh = meshFilter.mesh;
		Vector3[] baseVerticies = mesh.vertices;
		Vector3[] vertices = new Vector3[baseVerticies.Length];

		float timez = Time.time;
		for (var i=0; i < baseVerticies.Length; i++) {
			var vertex = baseVerticies[i];
			float noise = Mathf.PerlinNoise(timez + vertex.x, timez + vertex.y);
			float scale = GetSmallCircleHeight (vertex);
			vertex.z = noise * scale;
			vertices[i] = vertex;
		}

		mesh.vertices = vertices;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
	}
 
}
