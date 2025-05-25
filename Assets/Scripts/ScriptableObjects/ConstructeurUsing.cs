using RedstoneinventeGameStudio;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ConstructeurUsing : MonoBehaviour
{
    public CraftingRecipes Recipe;

    public GameObject CardIn1;
    public GameObject CardIn2;
    public GameObject CardOut;
    
    public TMP_Dropdown dropdown;
    public CraftingRecipes[] craftingRecipes;
    
    void Update()
    {
        int index = dropdown.value;
        Recipe = craftingRecipes[index];
        if (Recipe == null) return;
        
        Debug.Log("tryCraft");
        if (CanCraft())
        {
            Debug.Log("CanCraft");
            DoCraft();
        }
    }

    private bool CanCraft()
    {
        Debug.Log("CanCraft1");
        
        // Créer la liste des ressources nécessaires
        List<string> neededNames = new List<string>();
        foreach (var item in Recipe.Ressources)
        {
            neededNames.Add(item.itemName);
        }

        Debug.Log("CanCraft2 - Recette nécessite " + neededNames.Count + " ressources");

        // Recette à 1 élément
        if (neededNames.Count == 1)
        {
            // Vérifier que CardIn1 a l'item nécessaire et CardIn2 est vide
            if (CardIn1 != null && CardIn1.GetComponent<CardManager>().itemData != null &&
                CardIn1.GetComponent<CardManager>().itemData.itemName == neededNames[0] &&
                (CardIn2 == null || CardIn2.GetComponent<CardManager>().itemData == null))
            {
                return true;
            }
            
            // Ou que CardIn2 a l'item nécessaire et CardIn1 est vide
            if (CardIn2 != null && CardIn2.GetComponent<CardManager>().itemData != null &&
                CardIn2.GetComponent<CardManager>().itemData.itemName == neededNames[0] &&
                (CardIn1 == null || CardIn1.GetComponent<CardManager>().itemData == null))
            {
                return true;
            }
            
            return false;
        }
        
        // Recette à 2 éléments - vérifier que les deux cartes ont des items
        if (neededNames.Count == 2)
        {
            if (CardIn1 == null || CardIn1.GetComponent<CardManager>().itemData == null) 
            {
                Debug.Log("card1 item null");
                return false;
            }
            
            if (CardIn2 == null || CardIn2.GetComponent<CardManager>().itemData == null) 
            {
                Debug.Log("card2 item null");
                return false;
            }

            string item1 = CardIn1.GetComponent<CardManager>().itemData.itemName;
            string item2 = CardIn2.GetComponent<CardManager>().itemData.itemName;

            // Recette avec 2 items différents
            if (neededNames[0] != neededNames[1])
            {
                return neededNames.Contains(item1) && neededNames.Contains(item2) && item1 != item2;
            }
            // Recette avec 2 fois le même item
            else
            {
                return item1 == neededNames[0] && item2 == neededNames[0];
            }
        }
        
        return false;
    }

    private void DoCraft()
    {
        // Retirer les items selon le type de recette
        List<string> neededNames = new List<string>();
        foreach (var item in Recipe.Ressources)
        {
            neededNames.Add(item.itemName);
        }

        if (neededNames.Count == 1)
        {
            // Pour recette à 1 élément, retirer seulement la carte qui a l'item
            if (CardIn1 != null && CardIn1.GetComponent<CardManager>().itemData != null &&
                CardIn1.GetComponent<CardManager>().itemData.itemName == neededNames[0])
            {
                CardIn1.GetComponent<CardManager>().UnSetItem();
            }
            else if (CardIn2 != null && CardIn2.GetComponent<CardManager>().itemData != null &&
                     CardIn2.GetComponent<CardManager>().itemData.itemName == neededNames[0])
            {
                CardIn2.GetComponent<CardManager>().UnSetItem();
            }
        }
        else if (neededNames.Count == 2)
        {
            // Pour recette à 2 éléments, retirer les deux cartes
            if (CardIn1 != null) CardIn1.GetComponent<CardManager>().UnSetItem();
            if (CardIn2 != null) CardIn2.GetComponent<CardManager>().UnSetItem();
        }

        // Mettre le résultat dans CardOut
        if (CardOut.GetComponent<CardManager>().itemData == null)
        {
            CardOut.GetComponent<CardManager>().SetItem(Recipe.product.CreateCopyWithQuantity(Recipe.amount));
        }
        else if (CardOut.GetComponent<CardManager>().itemData.itemName == Recipe.product.itemName)
        {
            int total = CardOut.GetComponent<CardManager>().itemData.itemNb + Recipe.amount;
            CardOut.GetComponent<CardManager>().UnSetItem();
            CardOut.GetComponent<CardManager>().SetItem(Recipe.product.CreateCopyWithQuantity(total));
        }

        // Pour éviter de crafter en boucle à chaque Update
        Recipe = null;
    }
}