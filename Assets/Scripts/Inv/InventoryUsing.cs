using System;
using RedstoneinventeGameStudio;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class InventoryUsing : MonoBehaviour
{
    public GameObject Panel;

    public bool AddItem(InventoryItemData it)
    {
        Debug.Log("Trying to add item: " + it.itemName + ", quantité = " + it.itemNb);

        for (int i = 0; i < Panel.transform.childCount; i++)
        {
            var child = Panel.transform.GetChild(i);
            var card = child.GetComponent<CardManager>();

            if (card != null && card.itemData != null && card.itemData.itemName == it.itemName)
            {
                // Crée une nouvelle copie avec la quantité totale (ajout)
                InventoryItemData combined = it.CreateCopyWithQuantity(card.itemData.itemNb + it.itemNb);
                card.UnSetItem();
                card.SetItem(combined);
                return true;
            }
        }

        for (int i = 0; i < Panel.transform.childCount; i++)
        {
            var child = Panel.transform.GetChild(i);
            var card = child.GetComponent<CardManager>();

            if (card != null && card.itemData == null)
            {
                // On peut directement mettre l'item (1ère fois)
                card.SetItem(it);
                return true;
            }
        }

        return false;
    }




    public bool RemoveItem(InventoryItemData it)
    {
        for (int i = 0; i < Panel.transform.childCount; i++)
        {
            var child = Panel.transform.GetChild(i);
            var inventoryItemData = child.GetComponent<CardManager>().itemData;
            if (inventoryItemData != null && inventoryItemData.itemName == it.itemName)
            {
                if (child.GetComponent<CardManager>().itemData.itemNb < it.itemNb) return false;
                InventoryItemData itemtemp = child.GetComponent<CardManager>().itemData.CreateCopyWithQuantity(child.GetComponent<CardManager>().itemData.itemNb - it.itemNb);
                child.GetComponent<CardManager>().UnSetItem();
                child.GetComponent<CardManager>().SetItem(itemtemp);
                if (child.GetComponent<CardManager>().itemData.itemNb == it.itemNb)
                {
                    child.GetComponent<CardManager>().UnSetItem();
                }
                return true;
            }
        }
        return false;
    }

    public bool IsPresent(InventoryItemData it)
    {
        for (int i = 0; i < Panel.transform.childCount; i++)
        {
            var child = Panel.transform.GetChild(i);
            var inventoryItemData = child.GetComponent<CardManager>().itemData;
            if (inventoryItemData != null && inventoryItemData.itemName == it.itemName) return true;
        }
        return false;
    }

    public bool Increment(InventoryItemData it)
    {
        Debug.Log("Trying to add item: " + it.itemName + ", quantité = " + it.itemNb);

        for (int i = 0; i < Panel.transform.childCount; i++)
        {
            var child = Panel.transform.GetChild(i);
            var card = child.GetComponent<CardManager>();

            if (card != null && card.itemData != null && card.itemData.itemName == it.itemName)
            {
                InventoryItemData combined = it.CreateCopyWithQuantity(card.itemData.itemNb + 1);
                card.UnSetItem();
                card.SetItem(combined);
                return true;
            }
        }

        for (int i = 0; i < Panel.transform.childCount; i++)
        {
            var child = Panel.transform.GetChild(i);
            var card = child.GetComponent<CardManager>();

            if (card != null && card.itemData == null)
            {
                card.SetItem(it.CreateCopyWithQuantity(1));
                return true;
            }
        }
        return false;
    }
    public bool Decrement(InventoryItemData it)
    {
        for (int i = 0; i < Panel.transform.childCount; i++)
        {
            var child = Panel.transform.GetChild(i);
            if (child.GetComponent<CardManager>().itemData.itemName == it.itemName)
            {
                int quantity = child.GetComponent<CardManager>().itemData.itemNb - 1;
                child.GetComponent<CardManager>().UnSetItem();
                if (quantity > 0) child.GetComponent<CardManager>().SetItem(it.CreateCopyWithQuantity(quantity));
                return true;
            }
        }
        return false;
    }
}