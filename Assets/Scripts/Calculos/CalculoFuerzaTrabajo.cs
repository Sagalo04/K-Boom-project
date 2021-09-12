using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CalculoFuerzaTrabajo : MonoBehaviour
{
    public TMP_InputField inputmass1;
    public TMP_InputField inputmass2;
    public TMP_InputField inputmass3;
    public GameObject ResultadoIncorrecto;
    public GameObject ResultadoCorrecto;

    public static float MasaRoja;
    public static float MasaAmarilla;
    public static float MasaAzul;

    

    public void OnclickedForce()
    {
        MasaRoja = float.Parse(inputmass1.text);
        bool A1 = MasaRoja >= 154.2 && MasaRoja <= 154.4;

        MasaAmarilla = float.Parse(inputmass2.text);
        bool A2 = MasaAmarilla >= 101.1 && MasaAmarilla <= 101.3;

        MasaAzul = float.Parse(inputmass3.text);
        bool A3 = MasaAzul >= 49.7 && MasaAzul <= 49.9;

        Debug.Log(A1);
        Debug.Log(A2);
        Debug.Log(A3);

        if (A1 && A2 && A3)
        {

            ResultadoIncorrecto.SetActive(false);
            ResultadoCorrecto.SetActive(true);
            Debug.Log("CORRECTO");
        }
        else
        {
            ResultadoIncorrecto.SetActive(true);
            ResultadoCorrecto.SetActive(false);
            Debug.Log("INCORRECTO");
        }
    }
}
