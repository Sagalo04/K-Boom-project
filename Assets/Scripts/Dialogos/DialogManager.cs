using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    //Singleton
    public static DialogManager singleton;


    public GameObject dialogo;
    public GameObject MenuUI;
    public GameObject dialogador;
    public Text txtDialogo;
    public Frase[] dialogoEnsayo;
    public ConfigDialogos config;
    // Start is called before the first frame update

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
    void Start()
    {
        dialogo.SetActive(false);
        
    }

    public IEnumerator Decir(Frase[] _dialogo)
    {
        dialogador.SetActive(true);
        dialogo.SetActive(true);
        MenuUI.SetActive(false);
        for (int i= 0; i < _dialogo.Length; i++)
        {
            txtDialogo.text = "";
            for(int j = 0; j < _dialogo[i].texto.Length + 1;j++)
            {
                yield return new WaitForSeconds(config.tiempoLetra);
                if (Input.GetKey(config.tecladoSkip))
                {
                    j = _dialogo[i].texto.Length;
                }
                txtDialogo.text = _dialogo[i].texto.Substring(0,j);
            }
            txtDialogo.text = _dialogo[i].texto;
            yield return new WaitForSeconds(0.5f);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        }
        dialogo.SetActive(false);
        MenuUI.SetActive(true);
        dialogador.SetActive(false);
    }
    [ContextMenu("Activar prueba")]
    public void Prueba()
    {
        StartCoroutine(Decir(dialogoEnsayo));
    }
}
[System.Serializable]
public class Frase
{
    public string texto;
}

[System.Serializable]
public class EstadoDialogo
{
    public Frase[] frases;
}
