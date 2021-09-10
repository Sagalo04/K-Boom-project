using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogoFinal : MonoBehaviour
{
    public int estadoActual = 0;
    public EstadoDialogo[] estados;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bomba"))
        {
            StartCoroutine(DialogManager.singleton.Decir(estados[estadoActual].frases));
        }
    }
}
