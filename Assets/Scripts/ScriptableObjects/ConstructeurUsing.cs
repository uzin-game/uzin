using RedstoneinventeGameStudio;
using UnityEngine;
using System.Collections.Generic;
using QuestsScrpit;
using TMPro;
using Unity.Netcode;

public class ConstructeurUsing : NetworkBehaviour
{
    public CraftingRecipes Recipe;

    public GameObject CardIn1;
    public GameObject CardIn2;
    public GameObject CardOut;
    
    public TMP_Dropdown dropdown;
    public CraftingRecipes[] craftingRecipes;
    
    private QuestManager questManager;
    private float lastCraftTime = 0f;
    private float craftCooldown = 3f;
    
    void Update()
    {
        int index = dropdown.value;
        Recipe = craftingRecipes[index];
        if (Recipe == null) return;
        
        Debug.Log("tryCraft");
        // Vérifier si assez de temps s'est écoulé depuis le dernier craft
        if (Time.time - lastCraftTime >= craftCooldown && CanCraft())
        {
            Debug.Log("CanCraft");
            DoCraft();
            lastCraftTime = Time.time; // Enregistrer le temps du craft
        }
    }
    
    public override void OnNetworkSpawn()
    {
        questManager = FindFirstObjectByType<QuestManager>();
    }

    private bool CanCraft()
    {
        Debug.Log("CanCraft1");
        
        // Créer un dictionnaire des ressources nécessaires avec leurs quantités
        Dictionary<string, int> neededResources = new Dictionary<string, int>();
        foreach (var item in Recipe.Ressources)
        {
            if (neededResources.ContainsKey(item.itemName))
            {
                neededResources[item.itemName] += item.itemNb;
            }
            else
            {
                neededResources[item.itemName] = item.itemNb;
            }
        }

        Debug.Log("CanCraft2 - Recette nécessite " + neededResources.Count + " types de ressources");

        // Créer un dictionnaire des ressources disponibles
        Dictionary<string, int> availableResources = new Dictionary<string, int>();
        
        // Ajouter les ressources de CardIn1
        if (CardIn1 != null && CardIn1.GetComponent<CardManager>().itemData != null)
        {
            string itemName = CardIn1.GetComponent<CardManager>().itemData.itemName;
            int quantity = CardIn1.GetComponent<CardManager>().itemData.itemNb;
            availableResources[itemName] = quantity;
        }
        
        // Ajouter les ressources de CardIn2
        if (CardIn2 != null && CardIn2.GetComponent<CardManager>().itemData != null)
        {
            string itemName = CardIn2.GetComponent<CardManager>().itemData.itemName;
            int quantity = CardIn2.GetComponent<CardManager>().itemData.itemNb;
            
            if (availableResources.ContainsKey(itemName))
            {
                availableResources[itemName] += quantity;
            }
            else
            {
                availableResources[itemName] = quantity;
            }
        }

        // Vérifier si on a assez de chaque ressource
        foreach (var needed in neededResources)
        {
            if (!availableResources.ContainsKey(needed.Key) || 
                availableResources[needed.Key] < needed.Value)
            {
                Debug.Log($"Pas assez de {needed.Key}: besoin de {needed.Value}, disponible: {(availableResources.ContainsKey(needed.Key) ? availableResources[needed.Key] : 0)}");
                return false;
            }
        }
        
        return true;
    }

    private void DoCraft()
    {
        // Créer un dictionnaire des ressources à retirer
        Dictionary<string, int> toRemove = new Dictionary<string, int>();
        foreach (var item in Recipe.Ressources)
        {
            if (toRemove.ContainsKey(item.itemName))
            {
                toRemove[item.itemName] += item.itemNb;
            }
            else
            {
                toRemove[item.itemName] = item.itemNb;
            }
        }

        // Sauvegarder les données avant modification pour éviter les références perdues
        InventoryItemData card1Item = null;
        InventoryItemData card2Item = null;
        
        if (CardIn1 != null && CardIn1.GetComponent<CardManager>().itemData != null)
        {
            card1Item = CardIn1.GetComponent<CardManager>().itemData;
        }
        
        if (CardIn2 != null && CardIn2.GetComponent<CardManager>().itemData != null)
        {
            card2Item = CardIn2.GetComponent<CardManager>().itemData;
        }

        // Retirer les ressources de CardIn1 d'abord
        if (card1Item != null && toRemove.ContainsKey(card1Item.itemName))
        {
            int amountToRemove = Mathf.Min(toRemove[card1Item.itemName], card1Item.itemNb);
            int remainingQuantity = card1Item.itemNb - amountToRemove;
            
            toRemove[card1Item.itemName] -= amountToRemove;
            if (toRemove[card1Item.itemName] <= 0)
            {
                toRemove.Remove(card1Item.itemName);
            }
            
            CardIn1.GetComponent<CardManager>().UnSetItem();
            if (remainingQuantity > 0)
            {
                CardIn1.GetComponent<CardManager>().SetItem(card1Item.CreateCopyWithQuantity(remainingQuantity));
            }
            
            Debug.Log($"Retiré {amountToRemove} {card1Item.itemName} de CardIn1, reste: {remainingQuantity}");
        }

        // Retirer les ressources restantes de CardIn2
        if (card2Item != null && toRemove.ContainsKey(card2Item.itemName))
        {
            int amountToRemove = Mathf.Min(toRemove[card2Item.itemName], card2Item.itemNb);
            int remainingQuantity = card2Item.itemNb - amountToRemove;
            
            toRemove[card2Item.itemName] -= amountToRemove;
            if (toRemove[card2Item.itemName] <= 0)
            {
                toRemove.Remove(card2Item.itemName);
            }
            
            CardIn2.GetComponent<CardManager>().UnSetItem();
            if (remainingQuantity > 0)
            {
                CardIn2.GetComponent<CardManager>().SetItem(card2Item.CreateCopyWithQuantity(remainingQuantity));
            }
            
            Debug.Log($"Retiré {amountToRemove} {card2Item.itemName} de CardIn2, reste: {remainingQuantity}");
        }

        // Vérifier que toutes les ressources ont été retirées
        if (toRemove.Count > 0)
        {
            Debug.LogError("Erreur: Toutes les ressources n'ont pas pu être retirées!");
            foreach (var remaining in toRemove)
            {
                Debug.LogError($"Manque encore: {remaining.Value} {remaining.Key}");
            }
            return;
        }

        // Mettre le résultat dans CardOut
        if (CardOut != null)
        {
            if (CardOut.GetComponent<CardManager>().itemData == null)
            {
                CardOut.GetComponent<CardManager>().SetItem(Recipe.product.CreateCopyWithQuantity(Recipe.amount));
                Debug.Log($"Produit créé: {Recipe.amount} {Recipe.product.itemName}");
            }
            else if (CardOut.GetComponent<CardManager>().itemData.itemName == Recipe.product.itemName)
            {
                int total = CardOut.GetComponent<CardManager>().itemData.itemNb + Recipe.amount;
                CardOut.GetComponent<CardManager>().UnSetItem();
                CardOut.GetComponent<CardManager>().SetItem(Recipe.product.CreateCopyWithQuantity(total));
                Debug.Log($"Produit ajouté: total maintenant {total} {Recipe.product.itemName}");
            }
            else
            {
                Debug.LogWarning("CardOut contient déjà un item différent, impossible d'ajouter le produit");
                return;
            }
        }
        
        // Progression des quêtes
        if (questManager != null && questManager.currentQuestIndex == 6)
        {
            questManager.Quests[6].Progress(1f);
        }

        Debug.Log("Craft terminé avec succès");
    }
}