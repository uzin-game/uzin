using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RedstoneinventeGameStudio
{
    public class CardManager : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
#nullable enable
        public InventoryItemData? itemData;
        public bool isOccupied;
        [FormerlySerializedAs("MachineProperty")] public bool machineProperty;
#nullable disable

        public bool useAsDrag;
        [SerializeField] GameObject emptyCard;

        [SerializeField] TMP_Text itemName;
        [SerializeField] TMP_Text itemNb;
        [SerializeField] Image itemIcon;

        public bool InputCard;
        public bool CoalCard;
        public bool OutputCard;

        private void Awake()
        {
            if (useAsDrag)
            {
                ItemDraggingManager.dragCard = this;
                isOccupied = true;
                gameObject.SetActive(false);
            }


            if (itemData == null)
            {
                RefreshDisplay();
            }
            else
            {
                SetItem(itemData);
            }
        }

        public int GetNb()
        {
            return int.Parse(itemNb.text);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (useAsDrag || !isOccupied)
            {
                return;
            }

            ItemDraggingManager.fromCard = this;
            TooltipManagerInventory.UnSetToolTip();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isOccupied)
            {
                ItemDraggingManager.toCard = ItemDraggingManager.fromCard;

                if (ItemDraggingManager.toCard == default)
                {
                    TooltipManagerInventory.SetTooltip(itemData);
                }

                return;
            }

            ItemDraggingManager.toCard = this;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isOccupied)
            {
                return;
            }

            TooltipManagerInventory.UnSetToolTip();
        }

        public bool SetItem(InventoryItemData itemData)
        {
            if ((isOccupied && !useAsDrag) || itemData is null)
                return false;

            this.itemData = itemData;
            itemName.text = itemData.itemName;
            itemNb.text = itemData.itemNb.ToString();
            itemIcon.sprite = itemData.itemIcon;

            // Toujours activer le nom, même si quantité > 1
            itemName.gameObject.SetActive(true);
            itemNb.gameObject.SetActive(true);
            itemIcon.gameObject.SetActive(true);

            isOccupied = true;
            RefreshDisplay();
            return true;
        }

        public void UnSetItem()
        {
            itemData = null;
            this.isOccupied = false;

            RefreshDisplay();
        }

        void RefreshDisplay()
        {
            emptyCard.SetActive(!isOccupied);

            // Si le slot est occupé, activer tous les éléments UI
            if (isOccupied)
            {
                itemName.gameObject.SetActive(true);
                itemNb.gameObject.SetActive(true);
                itemIcon.gameObject.SetActive(true);
            }
        }
    }

}