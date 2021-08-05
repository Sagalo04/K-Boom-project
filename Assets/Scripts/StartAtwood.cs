using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using UnityStandardAssets.Characters.FirstPerson;
using ZXing;
using ZXing.QrCode;
using UnityEngine.UI;

public class StartAtwood : MonoBehaviour
{
#pragma warning disable CS0108 // El miembro oculta el miembro heredado. Falta una contraseña nueva
    public Camera camera;
#pragma warning restore CS0108 // El miembro oculta el miembro heredado. Falta una contraseña nueva

    public LayerMask layerMask;

    public GameObject MessagePanel;

    public GameObject MessagePanel2;

    public GameObject MessagePanel3;

    public GameObject QR;

    public GameObject Weight, Weight2;

    private Item itemBeingPickUp;

    private FirstPersonController firstPersonController;

    private int cont = 0;

    private List<float> points = new List<float>();

    public Image img;

    // Update is called once per frame
    void Update()
    {

        if (Weight2.transform.localPosition.y > -32 && Weight2.transform.localPosition.y < -9)
        {
            points.Add(Weight2.transform.localPosition.y);
        }
        if (Weight2.transform.localPosition.y > -9) Weight2.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;


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
            if (Input.GetKeyDown(KeyCode.F))
            {
                SendData();
            }
        }else if (HasItemTargetted() && cont == 2)
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
        var data = StringSerializationAPI.Serialize(typeof(List<float>), points);
        var Email = FirebaseAuthHandler.Email;
        var payLoad = $"{{\"user\":\"{Email}\",\"data\":{data}}}";
        RestClient.Post("https://kboombackend.herokuapp.com/points", payLoad).Then(
            response =>
            {
                string S = response.Text;
                Debug.Log(S);
            }).Catch(Debug.Log);
        cont++;
        CloseMessagePanel();
    }

    private void ShowQR()
    {
        QR.SetActive(true);
        var Email = FirebaseAuthHandler.Email;
        Texture2D myQR = generateQR($"https://kboomfront.vercel.app/{Email}");
        var mySprite = Sprite.Create(myQR, new Rect(0, 0, myQR.width, myQR.height), new Vector2(0.5f, 0.5f), 100.0f);
        Time.timeScale = 0;
        img.sprite = mySprite;
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
