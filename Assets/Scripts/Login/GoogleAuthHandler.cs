using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GoogleAuthHandler 
{
    //public static string ClientId = "180141893260-dmsd2vtri3tmkn00tr8a4nov7juf05l9.apps.googleusercontent.com";
    //public static string ClientSecret = "cNY459ziYYxsoelGPuo63m4S";
    private static string ClientId = "180141893260-b01ss5d2m41a9dkh23nc5ad3qcq6r4vj.apps.googleusercontent.com";
    private static string ClientSecret = "OOgMdsnlE9YCZPrLnA041mvE";
    public static void GetAuthCode()
    {
        Application.OpenURL($"https://accounts.google.com/o/oauth2/v2/auth?client_id={ClientId}&redirect_uri=urn:ietf:wg:oauth:2.0:oob&response_type=code&scope=email");
    }

    /// <summary>
    /// Exchanges the Auth Code with the user's Id Token
    /// </summary>
    /// <param name="code"> Auth Code </param>
    /// <param name="callback"> What to do after this is successfully executed </param>
    public static void ExchangeAuthCodeWithIdToken(string code, Action<string> callback)
    {
        RestClient.Post($"https://oauth2.googleapis.com/token?code={code}&client_id={ClientId}&client_secret={ClientSecret}&redirect_uri=urn:ietf:wg:oauth:2.0:oob&grant_type=authorization_code", null).Then(
            response => {
                var data = StringSerializationAPI.Deserialize(typeof(GoogleIdTokenResponse), response.Text) as GoogleIdTokenResponse;
                callback(data.id_token);
            }).Catch(Debug.Log);
    }
}

