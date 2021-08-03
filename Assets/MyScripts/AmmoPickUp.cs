using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoverShooter
{
	public class AmmoPickUp : MonoBehaviour {
		public AudioClip take;
	//	public BulletType BType ;
		public int amount = 20;
		public BaseGun Target;
		public string BulletName;
	// Use this for initialization

		void Start(){
		}
	// Update is called once per frame
		void OnTriggerEnter (Collider other) {

			//if (other.gameObject.tag == "Gun") {//попытаться выполнить функцию "MakeDamage" с параметром "damage" на другом объекте

				other.SendMessage ("Bullet",amount, SendMessageOptions.DontRequireReceiver);

				AudioSource.PlayClipAtPoint(take,transform.position);
				Destroy (gameObject);

			//}
		}
	}
}
