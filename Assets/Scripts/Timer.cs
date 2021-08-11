using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public TMP_Text txtTime;
    //public Text txtTime;
    public float minutos;
    float mstowait;
    private ulong iniciotimer;
    float segundosrestantes = 0.0f;
    void Start()
    {
        iniciartimer();
    }

    public void iniciartimer()
    {
        ConvertirTimetoMS();
        iniciotimer = (ulong)DateTime.Now.Ticks;
    }

    public void ConvertirTimetoMS()
    {
        mstowait = minutos * 60000;
    }
    void Update()
    {
        segundosrestantes = sabertotalsegundos();

        string auxtimer = "";
        if (segundosrestantes < 0)
        {
            Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
            segundosrestantes = segundosrestantes * -1;
            auxtimer += "-";
            auxtimer += ((int)segundosrestantes / 60).ToString("00") + ":";
            auxtimer += ((int)segundosrestantes % 60).ToString("00") + "";
        }
        else
        {
            auxtimer += ((int)segundosrestantes / 60).ToString("00") + ":";
            auxtimer += ((int)segundosrestantes % 60).ToString("00") + "";
        }
        txtTime.text = auxtimer;
    }

    float sabertotalsegundos()
    {
        ulong diff = ((ulong)DateTime.Now.Ticks - iniciotimer);
        ulong aux = diff / TimeSpan.TicksPerMillisecond;

        return (float)(mstowait - aux) / 1000.0f;
    }
}
