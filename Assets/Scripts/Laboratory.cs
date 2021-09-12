using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Laboratory : MonoBehaviour
{
    public GameObject msgAbrir;
    // Start is called before the first frame update
    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            msgAbrir.SetActive(true);
            if (Input.GetKeyDown(KeyCode.F))
            {
                LevelLoader.LoadLevel("Mision3");
            }
        }
        else
        {
            msgAbrir.SetActive(false);
        }
           
    }
}
