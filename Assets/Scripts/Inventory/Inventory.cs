using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

public class Inventory : NetworkBehaviour
{
    // Liste synchronisée des items
    public NetworkList<Item> items;

    private void Awake()
    {
        // Initialiser la NetworkList
        items = new();
    }

    // Ajouter un item à l'inventaire
    public void AddItem(Item item)
    {
        if (!IsServer) return; // Seul le serveur peut modifier l'inventaire

        items.Add(item);
        Debug.Log($"Added {item.itemName} to inventory.");
    }

    // Retirer un item de l'inventaire
    public void RemoveItem(Item item)
    {
        if (!IsServer) return; // Seul le serveur peut modifier l'inventaire

        if (items.Contains(item))
        {
            items.Remove(item);
            Debug.Log($"Removed {item.itemName} from inventory.");
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