using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public int max;
    public static int current;
    public Image mask;

    // Start is called before the first frame update
    void Start()
    {
        current = 2; 
    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentFill();
    }

    void GetCurrentFill()
    {
        float fillAmount = (float)current / (float)max;

        if(mask.fillAmount < fillAmount)
        {
            mask.fillAmount += 2f * Time.deltaTime;
        }
    }
}
