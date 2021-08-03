using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyAiming : MonoBehaviour {

    public Transform crosshairs;
    public GameObject Player;
    public Camera camera;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.LookAt(crosshairs);
        //  _controller.LookAt(crosshairs.position);

        Vector3 difference = target - Player.transform.position;
        float rotationZ = Mathf.Atan2(difference.y, difference.x);
       Player.transform.rotation = Quaternion.Euler(0, 0, rotationZ);
    }
}
