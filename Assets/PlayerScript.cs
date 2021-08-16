using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

public class PlayerScript : NetworkBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public override void OnStartLocalPlayer()
    {
        Camera.main.gameObject.GetComponent<CoverShooter.ThirdPersonCamera>().Target = gameObject.GetComponent<CoverShooter.CharacterMotor>();

        Camera.main.gameObject.GetComponent<CoverShooter.MouseLock>().enabled = true;
    }
}