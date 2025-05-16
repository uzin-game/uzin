using UnityEngine;

public class InvManag2 : MonoBehaviour
{
    public GameObject inventoryPanel;
    //private bool isOpen = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) // Touche "I" pour ouvrir/fermer
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        //isOpen = !isOpen;
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }
}
