using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PresstoPickUp : MonoBehaviour
{
#pragma warning disable CS0108 // El miembro oculta el miembro heredado. Falta una contraseña nueva
    public Camera camera;
#pragma warning restore CS0108 // El miembro oculta el miembro heredado. Falta una contraseña nueva

    public LayerMask layerMask;

    public RectTransform pickupImageRoot;

    public GameObject MessagePanel;

    private Item itemBeingPickUp;

    private FirstPersonController firstPersonController;

    // Update is called once per frame
    void Update()
    {
        SelectItemPickedFromRay();
        if (HasItemTargetted())
        {
            OpenMessagePanel();

            if (Input.GetKeyDown(KeyCode.F))
            {
                MoveItemToInventory();
            }
        }
        else
        {
            CloseMessagePanel();
        }
    }

    private bool HasItemTargetted()
    {
        return itemBeingPickUp !=null;
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
            
            if(hitItem == null)
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
        Destroy(itemBeingPickUp.gameObject);
        FirstPersonController.item[itemBeingPickUp.name] = true;
        itemBeingPickUp = null;
    }


}
