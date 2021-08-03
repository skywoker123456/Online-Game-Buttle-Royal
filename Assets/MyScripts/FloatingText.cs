using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoverShooter;

public class FloatingText : MonoBehaviour
{

	public float DestroyTime = 3f;
	public Vector3 Offset = new Vector3(0, 1, 0);
	// Use this for initialization
	void Start()
	{
		Destroy(gameObject, DestroyTime);

		transform.localPosition += Offset;
	}

}