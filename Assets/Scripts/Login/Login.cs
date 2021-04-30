using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public TMP_InputField code;
    public TMP_Text ErrorDominio;
    public TMP_Text ErrorToken;
    public Button Sign;

    public Sprite blockSprite;
    public Sprite signSprite;

    public void OnValueChange()
    {
        if(code.text != "")
        {
            Sign.GetComponent<Button>().interactable = true;
            Sign.GetComponent<Image>().sprite = signSprite;
        }
        else
        {
            Sign.GetComponent<Button>().interactable = false;
            Sign.GetComponent<Image>().sprite = blockSprite;
        }
    }

    public void OnClickGetToken()
    {
        GoogleAuthHandler.GetAuthCode();
    }

    public void OnClickLogin()
    {
        ErrorDominio.gameObject.SetActive(false);
        ErrorToken.gameObject.SetActive(false);
        GoogleAuthHandler.ExchangeAuthCodeWithIdToken(code.text, ErrorToken, idToken =>
        {
            FirebaseAuthHandler.SingInWithToken(idToken, ErrorDominio, "google.com");
        });
    }
}
