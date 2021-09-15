using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class MountPulley : MonoBehaviour
{
#pragma warning disable CS0108 // El miembro oculta el miembro heredado. Falta una contraseña nueva
    public Camera camera;
#pragma warning restore CS0108 // El miembro oculta el miembro heredado. Falta una contraseña nueva

    public LayerMask layerMask;

   /* public RectTransform pickupImageRoot;*/

    public GameObject MessagePanel;
    public GameObject activador;
    public GameObject tienda, tiendaOut;

    private Item itemBeingPickUp;

    public TMP_Text text;

    public Image Mission4;
    public Sprite Normal;
    public Sprite Check;

    void Update()
    {
        if ((FirstPersonController.item["Masa"] == true && FirstPersonController.item["Polea"] == true && FirstPersonController.item["Cuerda"] == true))
        { 
            text.color = Color.black;
            Mission4.sprite = Normal;
            tienda.SetActive(false);
            tiendaOut.SetActive(true);

        }
        SelectItemPickedFromRay();
        if (HasItemTargetted() && (FirstPersonController.item["Masa"] == true && FirstPersonController.item["Polea"] == true && FirstPersonController.item["Cuerda"] == true))
        {
            OpenMessagePanel();

            if (Input.GetKeyDown(KeyCode.F))
            {
                ProgressBar.current = ProgressBar.current + 25;
                Mission4.sprite = Check;
                activador.SetActive(true);
                FirstPersonController.item["Masa"] = false;
                FirstPersonController.item["Polea"] = false;
                FirstPersonController.item["Cuerda"] = false;
                MessagePanel.SetActive(false);

            }
            else
            {
                activador.SetActive(false);
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

    public void OpenMessagePanel()
    {
        MessagePanel.SetActive(true);
    }

    public void CloseMessagePanel()
    {
        MessagePanel.SetActive(false);
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

    private void MoveItemToInventory()
    {
        Debug.Log("Correcto");
    }

}
