using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public TMP_InputField code;
    public Button Sign;

    public Sprite blockSprite;
    public Sprite signSprite;

    public void OnValueChange()
    {
        if(code.text != "")
        {
            Sign.GetComponent<Image>().sprite = signSprite;
        }
        else
        {
            Sign.GetComponent<Image>().sprite = blockSprite;
        }
    }

    public void OnClickGetToken()
    {
        GoogleAuthHandler.GetAuthCode();
    }

    public void OnClickLogin()
    {
        GoogleAuthHandler.ExchangeAuthCodeWithIdToken(code.text, idToken =>
        {
            //Debug.LogWarning(idToken);
            FirebaseAuthHandler.SingInWithToken(idToken, "google.com");
        });
    }
}
