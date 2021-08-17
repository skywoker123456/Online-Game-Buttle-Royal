using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

public class PlayerScript : NetworkBehaviour
{
    void Start()
    {
        //Если не локальный Player
        if (!isLocalPlayer)
        {
            //Удаляем компоненты управления персонажем
            Destroy(gameObject.GetComponent<CoverShooter.CharacterMotor>());
            Destroy(gameObject.GetComponent<CoverShooter.ExitToEscape>());
            Destroy(gameObject.GetComponent<CoverShooter.CharacterPlatform>());
            Destroy(gameObject.GetComponent<CoverShooter.CharacterFace>());
            Destroy(gameObject.GetComponent<CoverShooter.CharacterAlerts>());
            Destroy(gameObject.GetComponent<CoverShooter.CharacterInventory>());
            Destroy(gameObject.GetComponent<CoverShooter.CharacterEffects>());
            Destroy(gameObject.GetComponent<CoverShooter.ThirdPersonInput>());
            Destroy(gameObject.GetComponent<CoverShooter.ThirdPersonController>());
            Destroy(gameObject.GetComponent<CoverShooter.Actor>());
            Destroy(gameObject.GetComponent<CoverShooter.CharacterSounds>());
        }
    }

    void Update()
    {
        
    }

    public override void OnStartLocalPlayer()
    {
        //Находим цель для камеры
        Camera.main.gameObject.GetComponent<CoverShooter.ThirdPersonCamera>().Target = gameObject.GetComponent<CoverShooter.CharacterMotor>();
        //Прячем мышь
        Camera.main.gameObject.GetComponent<CoverShooter.MouseLock>().enabled = true;
    }
}