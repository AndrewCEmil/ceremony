using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeformationController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		MeshFilter meshFilter = gameObject.GetComponent<MeshFilter> ();
		Mesh mesh = meshFilter.mesh;
		Vector3[] baseVerticies = mesh.vertices;
		Vector3[] vertices = new Vector3[baseVerticies.Length];

		float timez = Time.time;
		for (var i=0; i < baseVerticies.Length; i++) {
			var vertex = baseVerticies[i];
			float noise = Mathf.PerlinNoise(timez + vertex.x, timez + vertex.y);
			vertex.z = noise;
			vertices[i] = vertex;
		}

		mesh.vertices = vertices;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
	}
}
