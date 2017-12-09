using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAttr : MonoBehaviour {

	public GameObject target;
	void Start () {
		if (target == null) {
			target = GameObject.Find ("Player");
		}
		transform.LookAt(2 * transform.position - target.transform.position);
	}
}