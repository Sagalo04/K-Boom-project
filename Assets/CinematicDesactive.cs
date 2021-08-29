using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CinematicDesactive : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera CamaraFPS;
    public Camera CamaraCinematic;
    public GameObject MessagePanel;
    public PlayableDirector director;

    void OnTriggerEnter(Collider d)
    {
        if (d.CompareTag("Player"))
        {
            
        }
    }
    void OnEnable()
    {
        director.stopped += OnPlayableDirectorStopped;
    }

    void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
        if (director == aDirector)
        { 
            Debug.Log("PlayableDirector named " + aDirector.name + " is now stopped.");

            //CamaraFPS.enabled = true;
            //CamaraCinematic.enabled = false;

            SceneManager.LoadScene(2);

        }
    }

    void OnDisable()
    {
        director.stopped -= OnPlayableDirectorStopped;
    }
    
}
