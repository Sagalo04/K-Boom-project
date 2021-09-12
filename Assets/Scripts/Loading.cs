using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Loading : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string levelToLoad = LevelLoader.nextLevel;

        StartCoroutine(this.MakeTheLoad(levelToLoad));
    }

    IEnumerator MakeTheLoad(string level)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(level);

        while(operation.isDone == false)
        {
            yield return null;
        }

    }
}
