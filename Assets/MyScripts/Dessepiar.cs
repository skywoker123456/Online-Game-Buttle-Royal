using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Dessepiar : MonoBehaviour {
    public GameObject[] OffRegdolParticles0;
    public GameObject[] OffRegdolParticles1;
    public GameObject[] AllPartsOff;
    public SkinnedMeshRenderer[] bodyRender;
    bool bump;
    float bumpValue=30;
    float time;
    float bumpSpeed = 2f;
    public float destroyAfter;

    IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(2);
        bump = true;
        foreach (GameObject go in OffRegdolParticles0) if (go) go.SetActive(true);
        yield return new WaitForSecondsRealtime(1);
        foreach (GameObject go in OffRegdolParticles1) if (go) go.SetActive(true);
        yield return new WaitForSecondsRealtime(1);
        foreach (GameObject go in AllPartsOff) if (go) go.SetActive(false);
        Destroy(gameObject, destroyAfter);

    }


    // Update is called once per frame
    void Update () {

        if (bump)
        {
            foreach(SkinnedMeshRenderer rend in bodyRender)
            {
                rend.material.SetFloat("_BumpScale", Mathf.Lerp(1, bumpValue, time));
                rend.material.SetColor("_Color", Color.Lerp(Color.white, Color.black, time));
            }
            time += Time.deltaTime / bumpSpeed;
        }

	}
}
