using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

public class Inventory : NetworkBehaviour
{
    public NetworkList<Item> items;
    public NetworkList<int> quantity;

    private void Awake()
    {
        // Initialiser la NetworkList
        items = new();
        quantity = new();
    }

    // Ajouter un item à l'inventaire
    public void AddItem(Item item)
    {
        if (!IsServer) return;

        bool found = false;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemName == item.itemName)
            {
                found = true;
                quantity[i] += 1;
            }
        }
        
        if (!found)
        {
            quantity.Add(1);
            items.Add(item);
        }
        Debug.Log($"Added {item.itemName} to inventory.");
    }

    // Retirer un item de l'inventaire
    public void RemoveItem(Item item)
    {
        if (!IsServer) return; // Seul le serveur peut modifier l'inventaire

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemName == item.itemName)
            {
                if (quantity[i] == 1)
                {
                    items.RemoveAt(i);
                }
                else
                {
                    quantity[i] -= 1;
                }
            }
        }
    }

    // Afficher l'inventaire (pour le débogage)
    public void DisplayInventory()
    {
        foreach (var item in items)
        {
            Debug.Log(item.itemName);
        }
    }
}