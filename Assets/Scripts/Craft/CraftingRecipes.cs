using System.Collections.Generic;
using RedstoneinventeGameStudio;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftingRecipes", menuName = "Scriptable Objects/CraftingRecipes")]
public class CraftingRecipes : ScriptableObject
{
    public InventoryItemData product;
    public int amount;

    public InventoryItemData[] Ressources;
}
