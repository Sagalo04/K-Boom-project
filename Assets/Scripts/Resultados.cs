using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resultados : MonoBehaviour
{
    public Text masaRSL;
    public Text masaAmSL;
    public Text masaAzSL;
    public Text masaRTTE;
    public Text masaAmTTE;
    public Text masaAzTTE;
    // Start is called before the first frame update
    void Start()
    {
        masaRTTE.text = CalculoFuerzaTrabajo.MasaRoja.ToString();
        masaAmTTE.text = CalculoFuerzaTrabajo.MasaAmarilla.ToString();
        masaAzTTE.text = CalculoFuerzaTrabajo.MasaAzul.ToString();

        masaRSL.text = CalculoFuerza.MasaRoja.ToString();
        masaAmSL.text = CalculoFuerza.MasaAmarilla.ToString();
        masaAzSL.text = CalculoFuerza.MasaAzul.ToString();

        
    }

    
}
