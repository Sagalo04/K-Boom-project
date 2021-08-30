using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LvlMng : MonoBehaviour
{
    public string lvl;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CargarNivel()
    {
        SceneManager.LoadScene(lvl);
    }
    public void CerrarJuego()
    {
        Application.Quit();
    }
}
