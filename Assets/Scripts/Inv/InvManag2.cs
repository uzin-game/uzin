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
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
        if (inventoryPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape)) ToggleInventory();
    }


    public void ToggleInventory()
    {
        //isOpen = !isOpen;
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }
}
