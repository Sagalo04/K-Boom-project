using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CalculoFuerza : MonoBehaviour
{
    public TMP_InputField inputmass1;
    public TMP_InputField inputmass2;
    public TMP_InputField inputmass3;
    public GameObject ResultadoIncorrecto;
    public GameObject ResultadoCorrecto;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnclickedForce()
    {
        float F1 = float.Parse(inputmass1.text);
        bool A1 = F1 >= 154.2 && F1 <= 154.4;

        float F2 = float.Parse(inputmass2.text);
        bool A2 = F2 >= 101.1 && F2 <= 101.3;

        float F3 = float.Parse(inputmass3.text);
        bool A3 = F3 >= 49.7 && F3 <= 49.9;

        Debug.Log(A1);
        Debug.Log(A2);
        Debug.Log(A3);
        
        if (A1 && A2 && A3) {

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
