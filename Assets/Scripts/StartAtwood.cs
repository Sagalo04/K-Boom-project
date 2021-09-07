using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using UnityStandardAssets.Characters.FirstPerson;
using ZXing;
using ZXing.QrCode;
using UnityEngine.UI;
using TMPro;


public class StartAtwood : MonoBehaviour
{
#pragma warning disable CS0108 // El miembro oculta el miembro heredado. Falta una contraseña nueva
    public Camera camera;
#pragma warning restore CS0108 // El miembro oculta el miembro heredado. Falta una contraseña nueva

    public LayerMask layerMask;

    public GameObject MessagePanel;

    public GameObject MessagePanel2;

    public GameObject MessagePanel3;

    public GameObject Masa1;

    public GameObject Masa2;

    public GameObject Masa3;

    public GameObject QR;

    public GameObject Weight, Weight2;

    private Item itemBeingPickUp;

    private FirstPersonController firstPersonController;

    private int cont = 0;

    private List<float> points = new List<float>();
    private List<float> vel = new List<float>();
    private List<float> timePoints = new List<float>();

    public Image img;

    public TMP_Text textqr;

    private double timestart = 0;
    private int contador = 0;
    float initial = 0;

    private int contmass = 0;

    string dataYx;
    string dataYv;
    string dataX;
    string dataYxm2;
    string dataYvm2;
    string dataXm2;
    string dataYxm3;
    string dataYvm3;
    string dataXm3;
    string Email;

    // Update is called once per frame
    void Update()
    {
        if (Weight2.transform.localPosition.y >= -32 && Weight2.transform.localPosition.y < -9)
        {

            timePoints.Add((float)timestart);
            var timeaux = (double)Time.deltaTime;
            timestart += System.Math.Round(timeaux, 3);
            points.Add(Weight2.transform.localPosition.y);
            if(contador == 0 )
            {
                contador++;
                initial = Weight2.transform.localPosition.y;
            }
            float numeratorAux = ((float)(2 *9.8*(initial - Weight2.transform.localPosition.y)*(Weight2.GetComponent<Rigidbody>().mass- Weight.GetComponent<Rigidbody>().mass)));
            float denominator = Weight2.GetComponent<Rigidbody>().mass + Weight.GetComponent<Rigidbody>().mass;
            vel.Add((float)System.Math.Sqrt(numeratorAux/ denominator));
        }
        if (Weight2.transform.localPosition.y > -9) {
            Weight2.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseQR();
            Time.timeScale = 1;
        }

        SelectItemPickedFromRay();
        if (HasItemTargetted() && cont == 0)
        {
            OpenMessagePanels(MessagePanel);

            if (Input.GetKeyDown(KeyCode.F))
            {
                StartMedition();
            }
        }else if(HasItemTargetted() && cont == 1)
        {
            OpenMessagePanels(MessagePanel2);
            if (Input.GetKeyDown(KeyCode.F) && contmass==0)
            {
                SendData();
            }
            else if (Input.GetKeyDown(KeyCode.F) && contmass == 1)
            {
                SendData2();
            }
            else if (Input.GetKeyDown(KeyCode.F) && contmass == 2)
            {
                SendData3();
            }
        }
        else if (HasItemTargetted() && cont == 2)
        {
            OpenMessagePanels(MessagePanel3);
            if (Input.GetKeyDown(KeyCode.F))
            {
                ShowQR();
                Time.timeScale = 0;
            }
        }
        else
        {
            CloseMessagePanel();
        }
    }

    private bool HasItemTargetted()
    {
        return itemBeingPickUp != null;
    }

    public void OpenMessagePanels(GameObject MessagePanel)
    {
        MessagePanel.SetActive(true);
    }
    public void CloseMessagePanel()
    {
        MessagePanel.SetActive(false);
        MessagePanel2.SetActive(false);
        MessagePanel3.SetActive(false);
    }

    public void CloseQR()
    {
        QR.SetActive(false);
        Time.timeScale = 1;
    }

    private void SelectItemPickedFromRay()
    {
        Ray ray = camera.ViewportPointToRay(Vector3.one / 2f);
        Debug.DrawRay(ray.origin, ray.direction * 2f, Color.red);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 3f, layerMask))
        {
            var hitItem = hitInfo.collider.GetComponent<Item>();

            if (hitItem == null)
            {
                itemBeingPickUp = null;
            }
            else if (hitItem != null && hitItem != itemBeingPickUp)
            {
                itemBeingPickUp = hitItem;

                Debug.Log(itemBeingPickUp.name);

            }
        }
        else
        {
            itemBeingPickUp = null;
        }

    }

    private void StartMedition()
    {
        Weight.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        Weight2.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;


        cont++;
        itemBeingPickUp = null;
        CloseMessagePanel();
    }

    private void SendData()
    {
        dataYx = StringSerializationAPI.Serialize(typeof(List<float>), points);
        dataYv = StringSerializationAPI.Serialize(typeof(List<float>), vel);
        dataX = StringSerializationAPI.Serialize(typeof(List<float>), timePoints);
        /*var payLoad = $"{{\"user\":\"{Email}\",\"dataYx\":{dataYx},\"dataYv\":{dataYv},\"dataX\":{dataX}}}";
        RestClient.Post("https://kboombackend.herokuapp.com/points", payLoad).Then(
            response =>
            {
                string S = response.Text;
                Debug.Log(S);
            }).Catch(Debug.Log);*/
        contmass++;
        contador = 0;
        cont = 0;

        points.Clear();
        vel.Clear();
        timePoints.Clear();

        timestart = 0;

        Masa1.SetActive(false);
        Masa2.SetActive(true);


        Weight.GetComponent<Transform>().localPosition = new Vector3(Weight.transform.localPosition.x, -32.8f, Weight.transform.localPosition.z);
        Weight2.GetComponent<Transform>().localPosition = new Vector3(Weight2.transform.localPosition.x, -32.8f, Weight2.transform.localPosition.z);
        Weight2.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        Weight.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        Weight.GetComponent<Rigidbody>().mass = 3f;
        CloseMessagePanel();
    }

    private void SendData2()
    {
        dataYxm2 = StringSerializationAPI.Serialize(typeof(List<float>), points);
        dataYvm2 = StringSerializationAPI.Serialize(typeof(List<float>), vel);
        dataXm2 = StringSerializationAPI.Serialize(typeof(List<float>), timePoints);
       /* var payLoad = $"{{\"user\":\"{Email}\",\"dataYx\":{dataYx},\"dataYv\":{dataYv},\"dataX\":{dataX}}}";
        RestClient.Post("https://kboombackend.herokuapp.com/points", payLoad).Then(
            response =>
            {
                string S = response.Text;
                Debug.Log(S);
            }).Catch(Debug.Log);*/
        contmass++;
        contador = 0;
        cont = 0;

        points.Clear();
        vel.Clear();
        timePoints.Clear();

        timestart = 0;

        Masa2.SetActive(false);
        Masa3.SetActive(true);

        Weight.GetComponent<Transform>().localPosition = new Vector3(Weight.transform.localPosition.x, -32.8f, Weight.transform.localPosition.z);
        Weight2.GetComponent<Transform>().localPosition = new Vector3(Weight2.transform.localPosition.x, -32.8f, Weight2.transform.localPosition.z);
        Weight2.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        Weight.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        Weight.GetComponent<Rigidbody>().mass = 3.1f;
        CloseMessagePanel();
    }

    private void SendData3()
    {
        dataYxm3 = StringSerializationAPI.Serialize(typeof(List<float>), points);
        dataYvm3 = StringSerializationAPI.Serialize(typeof(List<float>), vel);
        dataXm3 = StringSerializationAPI.Serialize(typeof(List<float>), timePoints);
        var Email = FirebaseAuthHandler.Email;
        var payLoad = $"{{\"user\":\"{Email}\",\"dataYx\":{dataYx},\"dataYv\":{dataYv},\"dataX\":{dataX},\"dataYxm2\":{dataYxm2},\"dataYvm2\":{dataYvm2},\"dataXm2\":{dataXm2},\"dataYxm3\":{dataYxm3},\"dataYvm3\":{dataYvm3},\"dataXm3\":{dataXm3}}}";
        RestClient.Post("https://kboombackend.herokuapp.com/points", payLoad).Then(
            response =>
            {
                string S = response.Text;
                Debug.Log(S);
            }).Catch(Debug.Log);
        contmass++;
        cont ++;

        points.Clear();
        vel.Clear();
        timePoints.Clear();

        timestart = 0;

        Weight.GetComponent<Transform>().localPosition = new Vector3(Weight.transform.localPosition.x, -32.8f, Weight.transform.localPosition.z);
        Weight2.GetComponent<Transform>().localPosition = new Vector3(Weight2.transform.localPosition.x, -32.8f, Weight2.transform.localPosition.z);
        Weight2.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        Weight.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        CloseMessagePanel();
    }

    private void ShowQR()
    {
        QR.SetActive(true);
        var Email = FirebaseAuthHandler.Email;
        Texture2D myQR = generateQR($"https://kboomfront.vercel.app/{Email}");
        textqr.text = $"https://kboomfront.vercel.app/{Email}";
        var mySprite = Sprite.Create(myQR, new Rect(0, 0, myQR.width, myQR.height), new Vector2(0.5f, 0.5f), 100.0f);
        Time.timeScale = 0;
        img.sprite = mySprite;
        Debug.Log(timePoints.Count);
        Debug.Log(points.Count);
    }

    private static Color32[] Encode(string textForEncoding, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }

    public Texture2D generateQR(string text)
    {
        var encoded = new Texture2D(256, 256);
        var color32 = Encode(text, encoded.width, encoded.height);
        encoded.SetPixels32(color32);
        encoded.Apply();
        return encoded;
    }


}
