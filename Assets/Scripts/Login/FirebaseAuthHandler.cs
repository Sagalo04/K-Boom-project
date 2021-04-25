using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirebaseAuthHandler
{
    private const string ApiKey = "AIzaSyBNWF1e7VFUbH9fgDxDGl90NQouk1MjMYw";

    public static void SingInWithToken(string token, string providerId)
    {
        var payLoad =
            $"{{\"postBody\":\"id_token={token}&providerId={providerId}\",\"requestUri\":\"http://localhost\",\"returnIdpCredential\":true,\"returnSecureToken\":true}}";
        RestClient.Post($"https://identitytoolkit.googleapis.com/v1/accounts:signInWithIdp?key={ApiKey}", payLoad).Then(
            response =>
            {
                string S = response.Text;
                //Debug.Log(S);
                var data = StringSerializationAPI.Deserialize(typeof(GoogleEmail), response.Text) as GoogleEmail;
                //Debug.Log(data.email);

                if (data.email.Contains("@uao.edu.co"))
                {
                    Debug.Log(data.email);
                    SceneManager.LoadScene(1);
                }
                else
                {
                    Debug.Log("No pertenece al dominio UAO");
                }

            }).Catch(Debug.Log);
    }
}
