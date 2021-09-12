using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PasarEscena : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LevelLoader.LoadLevel("Mision1");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
