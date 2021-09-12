using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public static class LevelLoader 
{
    public static string nextLevel;

    public static void LoadLevel(string name)
    {
        nextLevel = name;
        SceneManager.LoadScene("Loading");
        
    }

}
