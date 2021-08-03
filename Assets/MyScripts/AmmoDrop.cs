using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CoverShooter;

//[RequireComponent(typeof(AudioSource))]
//[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(CharacterMotor))]


[ExecuteInEditMode]
public class AmmoDrop : MonoBehaviour
{
	/// <summary>
	/// Select the weaponType you want to add ammo for
	/// </summary>
	public enum AmmoType { Pistol, Rifle, Shotgun, Sniper }

	[Tooltip("Select the Type of Ammo you want to restock")]
	public AmmoType refNumber;
	private int AmmoSelector;

	/// <summary>
	/// referance for the the script to send too
	/// </summary>
	[Tooltip("Player Object Goes here")]
	public GameObject Player;

 CharacterMotor Motor;


	/// <summary>
	/// Amount of Ammo you want to have picked up
	/// </summary>
	[Tooltip("Ammo you want sent")]
	public int AmmoDropAmount = 5;




	public string PlayerTag = "Player";


	[Tooltip("Animations and related assets to be used with this weapon.")]
	public WeaponType Type;

	/// <summary>
	/// Name of the gun ammo to be display on the HUD.
	/// </summary>
	[Tooltip("Name of the gun to be display on the HUD.")]
	public string Name = "Ammo";



		public AudioClip take;
		
	
	// Use this for initialization
	void Start()
	{
		PlayerTag = Player.tag;
		AmmoSelector = (int)refNumber;

		//Debug.Log(PlayerTag + " is the players assigned tag");
		//Debug.Log(refNumber + " is whats selected, its index is "+AmmoSelector);
		//Debug.Log(AmmoDropAmount+" is how much this is set too");
		//Motor=GetComponent<CharacterMotor>();
	}

    void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player" && other.gameObject.layer == 10)
        {
       
            other.GetComponent<CoverShooter.CharacterMotor>().EquippedWeapon.Gun.GetComponent<CoverShooter.Gun>().BulletInventory += AmmoDropAmount;
            //  other.GetComponent<CoverShooter.CharacterInventory> ().Weapons [AmmoSelector].RightItem.GetComponent<CoverShooter.Gun> ().BulletInventory += AmmoDropAmount;

            AudioSource.PlayClipAtPoint(take, transform.position);


            Destroy(gameObject);
        }

    }
	}
	


