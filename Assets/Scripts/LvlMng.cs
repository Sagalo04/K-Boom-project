using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LvlMng : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CargarNivel()
    {
        SceneManager.LoadScene(1);
    }
    public void CerrarJuego()
    {
        Application.Quit();
    }
}
