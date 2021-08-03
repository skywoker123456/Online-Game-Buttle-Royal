using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoText : MonoBehaviour {

	Text text;
	public static int ammoAmount = 0;

	// Use this for initialization
	void Start () {
		text = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (ammoAmount > 0)
			text.text = "Ammo: " + ammoAmount;
		else
			text.text = "Out of Ammo!";
	}
}
