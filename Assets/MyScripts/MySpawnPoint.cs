using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MySpawnPoint : MonoBehaviour {
    public GameObject player;
    
    public GameObject[] zombie;
    public float Delay;
    public  float Distance;
    public float rang = 25;
    public float DestroyAfter;
    int count;
    
	void Start () {

        
    }
	
	
	void Update () {
       Distance = Vector3.Distance(player.transform.position,gameObject.transform.position);
      if (Distance < rang)
      {
          Spawn();
            Destroy(gameObject,DestroyAfter);
   }
	}
    void Spawn()
       
    {
        if (count >= zombie.Length)
            return;
       

        int i = Random.Range(0, zombie.Length);
     
  GameObject spawn= Instantiate(zombie[i], transform.position, Quaternion.identity) as GameObject;
        count++;
    }

}
