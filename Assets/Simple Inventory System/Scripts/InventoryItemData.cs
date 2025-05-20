using System;
using UnityEngine;

namespace RedstoneinventeGameStudio
{
    [CreateAssetMenu(fileName = "Inventory Item", menuName = "Inventory Item")]
    public class InventoryItemData : ScriptableObject
    {
        public string itemName;
        public string itemDescription;
        public int itemNb;

        public string itemTooltip;
        public Sprite itemIcon;

        // Méthode d'initialisation simulant un constructeur
        public void Init(string name, string description, int quantity, string tooltip, Sprite icon)
        {
            itemName = name;
            itemDescription = description;
            itemNb = quantity;
            itemTooltip = tooltip;
            itemIcon = icon;
        }
        public InventoryItemData CreateCopyWithQuantity(int quantity)
        {
            InventoryItemData copy = ScriptableObject.CreateInstance<InventoryItemData>();
            copy.Init(itemName, itemDescription, quantity, itemTooltip, itemIcon);
            return copy;
        }

    }
    
}