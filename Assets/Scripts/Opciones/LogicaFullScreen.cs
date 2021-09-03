using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogicaFullScreen : MonoBehaviour
{
    public Toggle toggle;
    public TMP_Dropdown resolucionDropDown;
    Resolution[] resoluciones;
    // Start is called before the first frame update
    void Start()
    {
        if (Screen.fullScreen)
        {
            toggle.isOn = true;
        }
        else
        {
            toggle.isOn = false;
        }
        RevisarResolucion();
    }

    public void ActivarFullScreen(bool pantallaCompleta)
    {
        Screen.fullScreen = pantallaCompleta;
    }
    
    public void RevisarResolucion()
    {
        resoluciones = Screen.resolutions;
        resolucionDropDown.ClearOptions();
        List<string> opciones = new List<string>();
        int resolucionActual = 0;

        for (int i = 0; i < resoluciones.Length; i++)
        {
            string opcion = resoluciones[i].width + "x" + resoluciones[i].height;
            opciones.Add(opcion);

            if(Screen.fullScreen && resoluciones[i].width == Screen.currentResolution.width && 
                resoluciones[i].height == Screen.currentResolution.height)
            {
                resolucionActual = i;
            }
        }
        resolucionDropDown.AddOptions(opciones);
        resolucionDropDown.value = resolucionActual;
        resolucionDropDown.RefreshShownValue();

    }

    public void CambiarResolucion(int indiceResolucion)
    {
        Resolution resolucion = resoluciones[indiceResolucion];
        Screen.SetResolution(resolucion.width, resolucion.height, Screen.fullScreen);
    }

}
