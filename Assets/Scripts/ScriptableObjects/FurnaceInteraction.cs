using System.Collections.Generic;
using RedstoneinventeGameStudio;
using ScriptableObjects;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class FurnaceInteraction : MonoBehaviour
{
    public bool IsInteracting;
    [SerializeField] private GameObject FurnaceUI;
    public GameObject playerInRange;
    public GameObject Panel;
    

    public void Interact()
    {
        if (!IsInteracting)
        {
            IsInteracting = true;
            SetFurnaceInventory();
            FurnaceUI.SetActive(true);
            Debug.Log("");
            Debug.Log("childs open :" + playerInRange.transform.childCount);
        }

        else if (IsInteracting)
        {
            IsInteracting = false;
            SetPlayerBackInventory();
            FurnaceUI.SetActive(false);
        }
    }

    public void SetFurnaceInventory()
    {
        Debug.Log("SetFurnaceInventory");

        if (playerInRange == null || Panel == null) return;

        // Trouve Inventaire et Panel
        Transform inventaireTransform = FindDeepChild(playerInRange.transform, "Inventaire");
        if (inventaireTransform == null)
        {
            Debug.LogError("Inventaire introuvable !");
            return;
        }

        Transform panelTransform = inventaireTransform.Find("Panel");
        if (panelTransform == null)
        {
            Debug.LogError("Panel introuvable dans Inventaire !");
            return;
        }

        var playerCards = GetInventoryCards(panelTransform);
        var furnaceCards = GetInventoryCards(Panel.transform);

        int count = Mathf.Min(playerCards.Count, furnaceCards.Count);
        for (int i = 0, j = 0; i < count; i++, j++)
        {
            // Ignore les slots machine
            while (j < furnaceCards.Count && furnaceCards[j].machineProperty)
                j++;

            if (j >= furnaceCards.Count) break;

            furnaceCards[j].SetItem(playerCards[i].itemData);
        }
        foreach (var cm in playerCards)
        {
            cm.UnSetItem();
        }
    }


    public void SetPlayerBackInventory()
    {
        if (playerInRange == null || Panel == null) return;

        Transform inventaireTransform = FindDeepChild(playerInRange.transform, "Inventaire");
        if (inventaireTransform == null)
        {
            Debug.LogError("Inventaire introuvable !");
            return;
        }

        Transform panelTransform = inventaireTransform.Find("Panel");
        if (panelTransform == null)
        {
            Debug.LogError("Panel introuvable dans Inventaire !");
            return;
        }

        var playerCards = GetInventoryCards(panelTransform);
        var furnaceCards = GetInventoryCards(Panel.transform);

        int count = Mathf.Min(playerCards.Count, furnaceCards.Count);
        for (int i = 0, j = 0; i < count; i++, j++)
        {
            while (j < furnaceCards.Count && furnaceCards[j].machineProperty)
                j++;

            if (j >= furnaceCards.Count) break;

            playerCards[i].SetItem(furnaceCards[j].itemData);
        }

        foreach (var cm in Panel.GetComponentsInChildren<CardManager>())
        {
            cm.UnSetItem();
        }
    }

    private List<CardManager> GetInventoryCards(Transform root)
    {
        List<CardManager> cards = new();
        foreach (var cm in root.GetComponentsInChildren<CardManager>())
        {
            // Optionnel : filtrer si tu veux ignorer ceux qui sont dans la machine
            if (!cm.machineProperty)
                cards.Add(cm);
        }
        return cards;
    }
    public static Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            var result = FindDeepChild(child, name);
            if (result != null) return result;
        }
        return null;
    }


}

