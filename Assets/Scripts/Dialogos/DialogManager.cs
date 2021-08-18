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
    public Text txtDialogo;
    public Frase[] dialogoEnsayo;
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
        dialogo.SetActive(true);
        for(int i= 0; i < _dialogo.Length; i++)
        {
            txtDialogo.text = _dialogo[i].texto;
            yield return new WaitForSeconds(0.5f);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        }
        dialogo.SetActive(false);
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
public class EstadoDialogo
{
    public Frase[] frases;
}
