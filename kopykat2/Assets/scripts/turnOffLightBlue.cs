using UnityEngine;
using System.Collections;

public class turnOffLightBlue : MonoBehaviour {

    Light lightBlue;
    Light lightRed;

    void Start()
    {
        lightBlue = GameObject.Find("Point light blue").GetComponent<Light>();
        lightRed = GameObject.Find("Point light red").GetComponent<Light>();
    }

    void OnPreRender()
    {
        //Debug.Log("Penis2");
        lightRed.enabled = false;
        lightBlue.enabled = true;
    }

    void OnPostRender()
    {
        lightRed.enabled = true;
        lightBlue.enabled = false;
    }
}
