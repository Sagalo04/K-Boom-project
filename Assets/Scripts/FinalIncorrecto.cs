using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalIncorrecto : MonoBehaviour
{
    public GameObject pantalla;
    public GameObject activador;
    public GameObject sonido;

    void Start()
    {
        Cursor.visible = false;
    }

        // Start is called before the first frame update
        public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bomba"))
        {
            pantalla.SetActive(true);
            Cursor.visible = true;
            sonido.SetActive(true);
            activador.SetActive(false);

        }
    }
}
