using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpShotGun : MonoBehaviour {
    public AudioClip take;
    public GameObject ShotGun1;
    public GameObject ShotGun2;
    public GameObject ShotGun3;
    void Start () {
		
	}

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {

        ShotGun1.SetActive(true);
        ShotGun2.SetActive(true);
        ShotGun3.SetActive(true);
        AudioSource.PlayClipAtPoint(take, transform.position);
        Destroy(gameObject);

        
    }
}
