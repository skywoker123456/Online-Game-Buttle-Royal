using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighter : MonoBehaviour {


  Light light;



    private void Start()
    {
        light = GetComponent<Light>();
    }
    void Update()
    {

        if (Input.GetButton("Light"))
        {

            light.enabled = !light.enabled;

        }

    }
}
