using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace CoverShooter
{

	public class FirstAid :MonoBehaviour{
		public GameObject HealthBar; 


		public AudioClip take;
		public float Health = 200f; 

		void Start () { 
			
		}



		void OnTriggerEnter (Collider other) {

			if (other.gameObject.tag == "Player") {//попытаться выполнить функцию "MakeDamage" с параметром "damage" на другом объекте

				other.SendMessage ("Heal", Health, SendMessageOptions.DontRequireReceiver);
				AudioSource.PlayClipAtPoint(take,transform.position);
				Destroy (gameObject);

			}
		}
		}
}
