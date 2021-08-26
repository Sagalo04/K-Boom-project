using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine;

public class CinematicActive : MonoBehaviour
{
    public Camera CamaraFPS;
    public Camera CamaraCinematic;
    public GameObject Cinematica;
    public GameObject Polea;
    public GameObject activador;
    // Start is called before the first frame update
    void Start()    
    {
        
    }


    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "Player")
        {
            Cinematica.SetActive(true);
            Polea.SetActive(true);
            CamaraFPS.enabled = false;
            CamaraCinematic.enabled = true;
            activador.SetActive(false);
        }
    }
}
