using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DeformMode
{
	Off,
	Rumble,
	Wave
}


public class DeformationController : MonoBehaviour {

	private DeformMode deformMode;
	private bool isOff;
	// Use this for initialization
	void Start () {
		deformMode = DeformMode.Rumble;
		isOff = false;
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
}
