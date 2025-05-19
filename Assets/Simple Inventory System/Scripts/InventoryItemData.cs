using System;
using UnityEngine;

namespace RedstoneinventeGameStudio
{
    [CreateAssetMenu(fileName = "Inventory Item", menuName = "Inventory Item")]
    public class InventoryItemData : ScriptableObject, IComparable<InventoryItemData>
    {
        protected bool Equals(InventoryItemData other)
        {
            return base.Equals(other) && itemName == other.itemName && itemDescription == other.itemDescription && itemNb == other.itemNb && itemTooltip == other.itemTooltip && Equals(itemIcon, other.itemIcon);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((InventoryItemData)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), itemName, itemDescription, itemNb, itemTooltip, itemIcon);
        }

        public string itemName;
        public string itemDescription;
        public int itemNb;

        public string itemTooltip;
        public Sprite itemIcon;

        public int CompareTo(InventoryItemData other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;
            var itemNameComparison = string.Compare(itemName, other.itemName, StringComparison.Ordinal);
            if (itemNameComparison != 0) return itemNameComparison;

            return string.Compare(itemTooltip, other.itemTooltip, StringComparison.Ordinal);
        }

        public static bool operator ==(InventoryItemData a, InventoryItemData b)
        {
            if (a is null || b is null) return false;
            if (a.itemName == b.itemName) return true;
            return false;
        }

        public static bool operator !=(InventoryItemData a, InventoryItemData b)
        {
            return !(a == b);
        }

        public static bool operator +(InventoryItemData a, InventoryItemData b)
        {
            if (a == b)
            {
                a.itemNb += b.itemNb;
                return true;
            }
            return false;
        }

        public static bool operator -(InventoryItemData a, InventoryItemData b)
        {
            if (a == b)
            {
                if (a.itemNb >= b.itemNb)
                {
                    a.itemNb -= b.itemNb;
                    return true;
                }
            }
            return false;
        }
    }
}