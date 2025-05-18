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
        for (int i = 0; i < Panel.transform.childCount; i++)
        {
            var child = Panel.transform.GetChild(i);
            if (child.GetComponent<InventoryItemData>() == it)
            {
                it.itemNb += child.GetComponent<InventoryItemData>().itemNb;
                child.GetComponent<CardManager>().SetItem(it);
                return true;
            } 
        }

        for (int i = 0; i < Panel.transform.childCount; i++)
        {
            var child = Panel.transform.GetChild(i);
            if (child.GetComponent<InventoryItemData>() == null)
            {
                child.GetComponent<CardManager>().SetItem(it);
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
            if (child.GetComponent<InventoryItemData>() == it)
            {
                if (child.GetComponent<InventoryItemData>().itemNb < it.itemNb) return false;
                it.itemNb += child.GetComponent<InventoryItemData>().itemNb;
                child.GetComponent<CardManager>().SetItem(it);
                if (child.GetComponent<InventoryItemData>().itemNb == it.itemNb)
                {
                    child.gameObject.SetActive(false);
                }
                return true;
            }
        }
        return false;
    }
}