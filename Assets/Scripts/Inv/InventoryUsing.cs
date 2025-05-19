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
        Debug.Log("Trying to add item: " + it.itemName);

        for (int i = 0; i < Panel.transform.childCount; i++)
        {
            var child = Panel.transform.GetChild(i);
            var card = child.GetComponent<CardManager>();
        
            if (card != null && card.itemData != null && card.itemData.itemName == it.itemName)
            {
                it.itemNb += card.itemData.itemNb;
                card.SetItem(it);
                return true;
            }
        }

        for (int i = 0; i < Panel.transform.childCount; i++)
        {
            var child = Panel.transform.GetChild(i);
            var card = child.GetComponent<CardManager>();

            if (card != null && card.itemData == null)
            {
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
            if (child.GetComponent<CardManager>().itemData.itemName == it.itemName)
            {
                if (child.GetComponent<CardManager>().itemData.itemNb < it.itemNb) return false;
                it.itemNb += child.GetComponent<CardManager>().itemData.itemNb;
                child.GetComponent<CardManager>().SetItem(it);
                if (child.GetComponent<CardManager>().itemData.itemNb == it.itemNb)
                {
                    child.gameObject.SetActive(false);
                }
                return true;
            }
        }
        return false;
    }
}