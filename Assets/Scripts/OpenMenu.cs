using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OpenMenu : MonoBehaviour
{
    public GameObject MessagePanel;
    public GameObject MessagePanel2;


    private bool state;

    // Start is called before the first frame update
    void Start()
    {
        state = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            state = !state;

            if (state)
            {
                MessagePanel.GetComponent<RectTransform>().localPosition = new Vector3(709f,98f,0);
                MessagePanel2.SetActive(false);
                //MessagePanel2.GetComponent<RectTransform>().localPosition = new Vector3(235f, 323f, 0);
            }
            else
            {
                MessagePanel.GetComponent<RectTransform>().localPosition = new Vector3(1211f, 98f, 0);
                MessagePanel2.SetActive(true);
                //MessagePanel2.GetComponent<RectTransform>().localPosition = new Vector3(886f, 323f, 0);
            }

        }
    }
}
