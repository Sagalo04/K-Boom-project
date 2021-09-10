using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicaFinal : MonoBehaviour
{
    public GameObject Weight, Weight2, sonido;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        StartMedition();
        if (Weight2.transform.localPosition.y > -9)
        {
            Weight2.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            sonido.SetActive(true);

        }
    }
    private void StartMedition()
    {
        Weight.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        Weight2.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }

    }
