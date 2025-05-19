using RedstoneinventeGameStudio;
using UnityEngine;

public class InvManag2 : MonoBehaviour
{
    public InventoryItemData inventoryItemData;
    public InventoryUsing inventoryUsing;
    public GameObject inventoryPanel;
    //private bool isOpen = false;
    void Start()
    {
        inventoryPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("H pressed: trying to add item");
            inventoryUsing.AddItem(inventoryItemData);
            ToggleInventory();
        }
    }


    public void ToggleInventory()
    {
        //isOpen = !isOpen;
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }
}
