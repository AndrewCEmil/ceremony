﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MovingGrid : MonoBehaviour {
	public float xSize;
	public float ySize;
	public float targetSize;
	private Vector3[] verticies;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	private void Awake () {
		Generate();
	}

	private void Generate() {
		Mesh mesh = new Mesh ();
		mesh.name = "MovingMesh";
		GetComponent<MeshFilter> ().mesh = mesh;
		verticies = new Vector3[((int)xSize + 1) * ((int)ySize + 1)];
		Vector2[] uv = new Vector2[verticies.Length];
		for (int i = 0, y = 0; y <= ySize; y++) {
			for (float x = 0; x <= xSize; x++, i++) {
				verticies [i] = new Vector3 (x * targetSize / xSize, y * targetSize / ySize);
				uv [i] = new Vector2 ((float)x / xSize, (float)y / ySize);
			}
		}

		mesh.vertices = verticies;
		mesh.uv = uv;

		int[] triangles = new int[(int)xSize * (int)ySize * 6];
		for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
			for (int x = 0; x < xSize; x++, ti += 6, vi++) {
				triangles[ti] = vi;
				triangles[ti + 3] = triangles[ti + 2] = vi + 1;
				triangles[ti + 4] = triangles[ti + 1] = vi + (int)xSize + 1;
				triangles[ti + 5] = vi + (int)xSize + 2;
			}
		}
		mesh.triangles = triangles;

		mesh.RecalculateNormals();
	}

	private void OnDrawGizmos () {
		if (verticies == null) {
			return;
		}
		Gizmos.color = Color.black;
		for (int i = 0; i < verticies.Length; i++) {
			Gizmos.DrawSphere(verticies[i], 0.1f);
		}
	}
}