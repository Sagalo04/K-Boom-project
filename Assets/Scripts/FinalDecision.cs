using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalDecision : MonoBehaviour
{
#pragma warning disable CS0108 // El miembro oculta el miembro heredado. Falta una contraseña nueva
    public Camera camera;
#pragma warning restore CS0108 // El miembro oculta el miembro heredado. Falta una contraseña nueva

    public string lvlMasa1;
    public string lvlMasa2;
    public string lvlMasa3;

    public LayerMask layerMasa1;
    public LayerMask layerMasa2;
    public LayerMask layerMasa3;

    /* public RectTransform pickupImageRoot;*/

    public GameObject MessagePanelMasa1;
    public GameObject MessagePanelMasa2;
    public GameObject MessagePanelMasa3;

    bool m1 = false;
    bool m2 = false;
    bool m3 = false;

    public GameObject activador;

    private Item itemBeingPickUp;

    void Update()
    {
        SelectItemPickedFromRay();
        if (HasItemTargetted())
        {

            if (Input.GetKeyDown(KeyCode.F) && m1 == true)
            {
                SceneManager.LoadScene(lvlMasa1);

            }
            else if (Input.GetKeyDown(KeyCode.F) && m2 == true)
            {
                SceneManager.LoadScene(lvlMasa2);
            }
            else if (Input.GetKeyDown(KeyCode.F) && m3 == true)
            {
                SceneManager.LoadScene(lvlMasa3);
            }
        }
        
    }

    private bool HasItemTargetted()
    {
        return itemBeingPickUp != null;
    }

  
    private void SelectItemPickedFromRay()
    {
        Ray ray = camera.ViewportPointToRay(Vector3.one / 2f);
        Debug.DrawRay(ray.origin, ray.direction * 2f, Color.red);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 3f, layerMasa1))
        {
            var hitItem = hitInfo.collider.GetComponent<Item>();

            if (hitItem == null)
            {
                itemBeingPickUp = null;
            }
            else if (hitItem != null && hitItem != itemBeingPickUp)
            {
                itemBeingPickUp = hitItem;
                m1 = true;
                m2 = false;
                m3 = false;
                MessagePanelMasa1.SetActive(true);
                MessagePanelMasa2.SetActive(false);
                MessagePanelMasa3.SetActive(false);
                Debug.Log(itemBeingPickUp.name);
            }
        }
        else if (Physics.Raycast(ray, out hitInfo, 3f, layerMasa2))
        {
            var hitItem = hitInfo.collider.GetComponent<Item>();

            if (hitItem == null)
            {
                itemBeingPickUp = null;
            }
            else if (hitItem != null && hitItem != itemBeingPickUp)
            {
                itemBeingPickUp = hitItem;
                m2 = true;
                m1 = false;
                m3 = false;
                MessagePanelMasa2.SetActive(true);
                MessagePanelMasa1.SetActive(false);
                MessagePanelMasa3.SetActive(false);
                Debug.Log(itemBeingPickUp.name);
            }
        }
        else if (Physics.Raycast(ray, out hitInfo, 3f, layerMasa3))
        {
            var hitItem = hitInfo.collider.GetComponent<Item>();

            if (hitItem == null)
            {
                itemBeingPickUp = null;
            }
            else if (hitItem != null && hitItem != itemBeingPickUp)
            {
                itemBeingPickUp = hitItem;
                m3 = true;
                m2 = false;
                m1 = false;
                MessagePanelMasa3.SetActive(true);
                MessagePanelMasa2.SetActive(false);
                MessagePanelMasa1.SetActive(false);
                Debug.Log(itemBeingPickUp.name);
            }
        }
        else
        {
            itemBeingPickUp = null;
            m1 = false;
            m2 = false;
            m3 = false;
            MessagePanelMasa1.SetActive(false);
            MessagePanelMasa2.SetActive(false);
            MessagePanelMasa3.SetActive(false);
        }

    }

    private void MoveItemToInventory()
    {
        Debug.Log("Correcto");
    }
}
