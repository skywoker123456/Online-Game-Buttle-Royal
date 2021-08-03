using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoverShooter
{
	
	
public class СameraChange : MonoBehaviour {

	public GameObject Cam1;
	public GameObject Cam2;
	public bool whichcam = false;


	// Update is called once per frame
	void OnTriggerEnter(Collider other) {

			if (other.gameObject.tag == "Player") {

				if (whichcam == false ) {
						whichcam = true;
					Cam1.SetActive (true);
					Cam2.SetActive (false);
				} else if (whichcam == true ) {
							whichcam = false;
					Cam1.SetActive (false);
					Cam2.SetActive (true);

				}

			}
		
		}

        void OnTriggerExit(Collider other)
        {

            if (other.gameObject.tag == "Player")
            {

                if (whichcam == true)
                {
                    whichcam = false;
                    Cam1.SetActive(false);
                    Cam2.SetActive(true);
                }
                else if (whichcam == false)
                {
                    whichcam = true;
                    Cam1.SetActive(true);
                    Cam2.SetActive(false);

                }

            }

        }






    }
}


