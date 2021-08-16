using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine;

public class CinematicDesactive : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera CamaraFPS;
    public Camera CamaraCinematic;
    // Start is called before the first frame update
    void Start()
    {

    }


    void OnTriggerEnter(Collider d)
    {
        if (d.gameObject.tag == "CameraCinematic")
        {
            CamaraFPS.enabled = true;
            CamaraCinematic.enabled = false;
        }
    }
}
