using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using UnityStandardAssets.Characters.FirstPerson;

public class StartAtwood : MonoBehaviour
{
#pragma warning disable CS0108 // El miembro oculta el miembro heredado. Falta una contraseña nueva
    public Camera camera;
#pragma warning restore CS0108 // El miembro oculta el miembro heredado. Falta una contraseña nueva

    public LayerMask layerMask;

    public GameObject MessagePanel;

    public GameObject MessagePanel2;

    public GameObject Weight, Weight2;

    private Item itemBeingPickUp;

    private FirstPersonController firstPersonController;

    private int cont = 0;

    private List<float> points = new List<float>();


    // Update is called once per frame
    void Update()
    {

        if (Weight.transform.localPosition.y > 0 && Weight.transform.localPosition.y < 0.92)
        {
            points.Add(Weight.transform.localPosition.y);
        }

        SelectItemPickedFromRay();
        if (HasItemTargetted() && cont == 0)
        {
            OpenMessagePanel();

            if (Input.GetKeyDown(KeyCode.F))
            {
                StartMedition();
            }
        }else if(HasItemTargetted() && cont == 1)
        {
            OpenMessagePanel2();
            if (Input.GetKeyDown(KeyCode.F))
            {
                SendData();
            }
        }
        else
        {
            CloseMessagePanel();
            CloseMessagePanel2();
        }
    }

    private bool HasItemTargetted()
    {
        return itemBeingPickUp != null;
    }

    public void OpenMessagePanel()
    {
        MessagePanel.SetActive(true);
    }

    public void OpenMessagePanel2()
    {
        MessagePanel2.SetActive(true);
    }

    public void CloseMessagePanel()
    {
        MessagePanel.SetActive(false);

    }

    public void CloseMessagePanel2()
    {
        MessagePanel2.SetActive(false);

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
        Weight.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX;
        Weight2.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX;
        cont++;
        itemBeingPickUp = null;
        CloseMessagePanel();
    }

    private void SendData()
    {
        var data = StringSerializationAPI.Serialize(typeof(List<float>), points);
        var Email = FirebaseAuthHandler.Email;
        var payLoad = $"{{\"user\":\"{Email}\",\"data\":{data}}}";
        RestClient.Post("http://localhost:5000/points", payLoad).Then(
            response =>
            {
                string S = response.Text;
                Debug.Log(S);
            }).Catch(Debug.Log);
    }
}
