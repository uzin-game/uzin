using RedstoneinventeGameStudio;
using UnityEngine;
using System.Collections.Generic;

public class ConstructeurUsing : MonoBehaviour
{
    public CraftingRecipes Recipe;

    public GameObject CardIn1;
    public GameObject CardIn2;
    public GameObject CardOut;

    private CardManager card1;
    private CardManager card2;
    private CardManager cardOut;



    void Update()
    {
        if (card1 != null)card1 = CardIn1.GetComponent<CardManager>();
        
        if (card2 != null)card2 = CardIn2.GetComponent<CardManager>();
        if (cardOut != null)cardOut = CardOut.GetComponent<CardManager>();
        if (Recipe == null) return;

        if (CanCraft())
        {
            DoCraft();
        }
    }

    private bool CanCraft()
    {
        if (card1.itemData == null || card2.itemData == null) return false;

        List<string> neededNames = new List<string>();
        foreach (var item in Recipe.Ressources)
        {
            neededNames.Add(item.itemName);
        }

        string item1 = card1.itemData.itemName;
        string item2 = card2.itemData.itemName;

        return (neededNames.Contains(item1) && neededNames.Contains(item2) &&
                item1 != item2 ? neededNames.Count == 2 : neededNames.FindAll(n => n == item1).Count == 2);
    }

    private void DoCraft()
    {
        // Retirer les items (1 par carte)
        card1.UnSetItem();
        card2.UnSetItem();

        // Mettre le résultat dans CardOut
        if (cardOut.itemData == null)
        {
            cardOut.SetItem(Recipe.product.CreateCopyWithQuantity(Recipe.amount));
        }
        else if (cardOut.itemData.itemName == Recipe.product.itemName)
        {
            int total = cardOut.itemData.itemNb + Recipe.amount;
            cardOut.UnSetItem();
            cardOut.SetItem(Recipe.product.CreateCopyWithQuantity(total));
        }

        // Pour éviter de crafter en boucle à chaque Update
        Recipe = null;
    }
}