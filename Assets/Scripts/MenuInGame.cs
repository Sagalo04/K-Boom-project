using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class MenuInGame : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public OptManager panelOpciones;
    public GameObject fpsObj;
    public FirstPersonController fpsScript;


    public void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    void Start()
    {

        panelOpciones = GameObject.FindGameObjectWithTag("opciones").GetComponent<OptManager>();
        

    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(GameIsPaused)
            {
                Resume();

            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        fpsObj = GameObject.Find("FPSController");
        fpsScript = fpsObj.GetComponent<FirstPersonController>();
        fpsScript.enabled = true;
        Cursor.visible = false;
        pauseMenuUI.SetActive(false);
        panelOpciones.pantallaOpciones.SetActive(false);
        GameIsPaused = false;
        Time.timeScale = 1f;
    }
    public void Pause()
    {
        fpsObj = GameObject.Find("FPSController");
        fpsScript = fpsObj.GetComponent<FirstPersonController>();
        //Disable FPS script
        fpsScript.enabled = false;
        //Unlock Mouse and make it visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pauseMenuUI.SetActive(true);
        GameIsPaused = true;
        Time.timeScale = 0f;
    }

    public void Quit()
    {
        SceneManager.LoadScene(3);
    }

    public void MostrarOpciones()
    {
        Cursor.visible = true;
        panelOpciones.pantallaOpciones.SetActive(true);
        
    }
    
}
