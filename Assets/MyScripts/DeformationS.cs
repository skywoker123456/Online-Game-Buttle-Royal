using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeformationS : MonoBehaviour {
	Mesh mesh;
	public float minVelocity = 5f;
	public float radiusDeformate = 0.5f;
	public float multiplay = 0.04f;
	// Use this for initialization
	void Start () {
		mesh = GetComponent<MeshFilter> ().mesh;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnCollisionEnter(Collision collision)
	{
		if (collision.relativeVelocity.magnitude > minVelocity) {
			bool isDeformated = false;
			Vector3[] verticles = mesh.vertices;
			for (int i = 0; i < mesh.vertexCount; i++) {
				for (int j = 0; j < collision.contacts.Length; j++) {
					Vector3 point = transform.InverseTransformPoint (collision.contacts [j].point);
					Vector3 velocity = transform.InverseTransformVector (collision.relativeVelocity);
					float distance = Vector3.Distance (point, verticles [i]);
					if (distance < radiusDeformate) {
						Vector3 deformate = velocity * (radiusDeformate - distance) * multiplay;
						verticles [i] += deformate;
						isDeformated = true;
					}
				}
			}
			if(isDeformated)
			{
				mesh.vertices = verticles;
				mesh.RecalculateNormals ();
				mesh.RecalculateBounds ();
				GetComponent<MeshCollider> ().sharedMesh = mesh;
			}
		}
	}
}
