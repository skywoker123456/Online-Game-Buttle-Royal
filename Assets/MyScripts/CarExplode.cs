using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoverShooter;

public class CarExplode : MonoBehaviour {
	
	public float Health = 100f;
	Rigidbody rb;
	public GameObject car;
	public GameObject exploteedCar;
	float damage=5f;
	MeshCollider mesh;
	public AudioClip Explote;
	public float Forse = 500f;
	public GameObject explosion;
	public float Radius;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		mesh = GetComponent<MeshCollider> ();


	}

	public void OnHit(Hit hit)
	{
		Health=Health-damage;

			

		if (Health <= 0 )
			Die();
	}


	void Die(){
		car.SetActive (true);
		exploteedCar.SetActive (true);
		explosion.SetActive (true);
		AudioSource.PlayClipAtPoint (Explote, transform.position);
		mesh.enabled = false;
	}

}
