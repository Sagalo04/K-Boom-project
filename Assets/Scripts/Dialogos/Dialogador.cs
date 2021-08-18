using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogador : MonoBehaviour
{
    public int estadoActual = 0;
    public EstadoDialogo[] estados;

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {

        }
    }
}
