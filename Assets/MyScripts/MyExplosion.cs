using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyExplosion : MonoBehaviour {
	public float radius;
	public float forse;
	// Use this for initialization
	void Start () {
		Collider[] col = Physics.OverlapSphere (transform.position, radius);
		foreach (Collider c in col) {
			if (c.name != "Enemy") {
				c.GetComponent<Rigidbody> ().AddExplosionForce (forse, transform.position, radius);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
